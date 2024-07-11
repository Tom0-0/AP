#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Controls.Controls.Utilities;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.WatchList.Comment;

namespace _3S.CoDeSys.WatchList
{
	[DebuggerDisplay("({DisplayExpression})")]
	public class WatchListNode : ITreeTableNode2, ITreeTableNode, IDisposable, IExpansionPreservableTreeTableNode
	{
		private delegate void DoSomethingInPrimaryDelegate();

		private delegate void GenericModelModifyingAction();

		protected WatchListModel _model;

		protected WatchListNode _parentNode;

		private ArrayList _childNodes;

		private string _stExpression;

		private string _stDisplayExpression;

		private string _stInstancePath;

		private Guid _explicitApplicationGuid = Guid.Empty;

		private IVarRef _varRef;

		private IOnlineVarRef _onlineVarRef;

		private WatchPointItem _watchpoint;

		private ApplicationPrefixItem _prefixItem;

		private string _stComment;

		private string _stDirectAddress;

		private Image _image;

		protected IConverterToIEC _converter;

		private bool _bCallToGetEffectiveConverterNeeded;

		private IConverterToIEC _globalConverter;

		private bool _bNodeWasExpandedBeforeRemovingChildren;

		private bool _bDisposed;

		private bool _bShowCommentColumn;

		private bool _bInvalidExpression;

		private bool _bIsOutdated;

		private int _nProjectHandle = -1;

		private Guid _guidObject = Guid.Empty;

		private Guid _editorguidObject = Guid.Empty;

		private Tuple<int, int, int, int, bool> _tupleMonitoringRange;

		private bool _bMonitoringEnabled;

		private int COLUMN_APPLICATION_PREFIX => _model.COLUMN_APPLICATION_PREFIX;

		internal int COLUMN_EXPRESSION => _model.COLUMN_EXPRESSION;

		private int COLUMN_WP => _model.COLUMN_WP;

		private int COLUMN_TYPE => _model.COLUMN_TYPE;

		internal int COLUMN_VALUE => _model.COLUMN_VALUE;

		internal int COLUMN_PREPARED_VALUE => _model.COLUMN_PREPARED_VALUE;

		private int COLUMN_DIRECT_ADDR => _model.COLUMN_DIRECT_ADDR;

		private int COLUMN_COMMENT => _model.COLUMN_COMMENT;

		public bool ImplicitInstancePointer { get; set; }

		public IConverterToIEC Converter
		{
			get
			{
				return GetConverter();
			}
			set
			{
				if (value == GetConverter())
				{
					return;
				}
				SetDelayedCallToGetEffectiveConverterNeeded(value);
				RaiseValueChanged();
				if (_childNodes == null)
				{
					return;
				}
				foreach (WatchListNode childNode in _childNodes)
				{
					childNode.Converter = value;
				}
			}
		}

		private WatchListModel Model
		{
			get
			{
				if (_model == null)
				{
					return _parentNode.Model;
				}
				return _model;
			}
		}

		internal IVarRef VarRef
		{
			get
			{
				//IL_0211: Unknown result type (might be due to invalid IL or missing references)
				Debug.Assert(_stExpression != null);
				if (_varRef == null && _watchpoint != null && _watchpoint.Breakpoint != null)
				{
					IBP4 breakpoint = _watchpoint.Breakpoint;
					_varRef = APEnvironment.BPMgr.AddWatchVariable(breakpoint, _stExpression);
					if (_varRef != null)
					{
						int bInvalidExpression;
						if (WatchListNodeUtils.IsValidExpression(_stExpression, _explicitApplicationGuid, ref _editorguidObject))
						{
							IVarRef varRef = _varRef;
							bInvalidExpression = ((!WatchListNodeUtils.IsValidCastType((varRef != null) ? varRef.WatchExpression : null)) ? 1 : 0);
						}
						else
						{
							bInvalidExpression = 1;
						}
						_bInvalidExpression = (byte)bInvalidExpression != 0;
					}
				}
				if (_varRef == null)
				{
					Guid guidApplication = GuidApplication;
					if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && APEnvironment.ObjectMgr.ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidApplication) && ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(guidApplication) == null)
					{
						IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidApplication);
						string name = metaObjectStub.Name;
						while (metaObjectStub.ParentObjectGuid != Guid.Empty)
						{
							metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, metaObjectStub.ParentObjectGuid);
						}
						name = metaObjectStub.Name + "." + name + ".";
						if (_stExpression.ToUpperInvariant().StartsWith(name.ToUpperInvariant()))
						{
							throw new InvalidOperationException(Strings.MonitoringCodeNotAvailable);
						}
					}
					_varRef = APEnvironment.LanguageModelMgr.GetVarReference(guidApplication, _stInstancePath, _stExpression, _nProjectHandle, _guidObject);
					_bInvalidExpression = true;
					if (WatchListNodeUtils.IsValidExpression(_stExpression, _explicitApplicationGuid, ref _editorguidObject))
					{
						IVarRef varRef2 = _varRef;
						if (WatchListNodeUtils.IsValidCastType((varRef2 != null) ? varRef2.WatchExpression : null))
						{
							_bInvalidExpression = false;
						}
					}
				}
				if (_bInvalidExpression || _varRef == null)
				{
					throw new InvalidVarRefException(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "Monitoring_InvalidExpression"));
				}
				return _varRef;
			}
		}

		internal Tuple<int, int, int, int, bool> MonitoringRange
		{
			get
			{
				return _tupleMonitoringRange;
			}
			set
			{
				_tupleMonitoringRange = value;
				if (value == null)
				{
					return;
				}
				Release();
				RemoveChildNodesIfExist();
				try
				{
					_model.BeginUpdate();
					FillChildren();
					RaiseExpressionChanged();
					RaiseStructureChanged();
				}
				catch
				{
				}
				finally
				{
					_model.EndUpdate();
				}
			}
		}

		internal virtual bool CanWatch
		{
			get
			{
				if (IsExplicitlySpecifiedAddress(bCheckOnlyThisNode: false))
				{
					return true;
				}
				return false;
			}
		}

		private string DirectAddress
		{
			get
			{
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				Debug.Assert(_stExpression != null);
				if (_stDirectAddress == null)
				{
					_stDirectAddress = string.Empty;
					string stDirectAddress = string.Empty;
					string stParentDirectAddress = string.Empty;
					if (_parentNode != null)
					{
						stParentDirectAddress = _parentNode.DirectAddress;
					}
					IVariable val = WatchListNodeUtils.FindVariableAndOptionalDirectAddress(_stExpression, bLookForDirectAddressedStructMembers: true, stParentDirectAddress, out stDirectAddress);
					if (stDirectAddress != string.Empty)
					{
						_stDirectAddress = stDirectAddress;
					}
					else if (val != null && val.Address != null)
					{
						string text = "?";
						string variableLocationPrefix = WatchListNodeUtils.GetVariableLocationPrefix(val.Address.Location);
						if (val.Address.Incomplete)
						{
							text = "*";
						}
						else if ((int)val.Address.Size != 0)
						{
							text = Enum.GetName(typeof(DirectVariableSize), val.Address.Size);
						}
						_stDirectAddress = "%" + variableLocationPrefix + text;
						for (int i = 0; i < val.Address.Components.Length; i++)
						{
							int num = val.Address.Components[i];
							_stDirectAddress += num;
							if (i < val.Address.Components.Length - 1)
							{
								_stDirectAddress += ".";
							}
						}
					}
				}
				return _stDirectAddress;
			}
		}

		private string Comment
		{
			get
			{
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Invalid comparison between Unknown and I4
				Debug.Assert(_stExpression != null);
				if (_stComment == null)
				{
					_stComment = string.Empty;
					IVariable val = null;
					IPreCompileContext val2 = default(IPreCompileContext);
					ISignature val3 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindSignature(_editorguidObject, out val2);
					if (val3 != null && _stExpression != null && _stInstancePath != null && _stExpression.Length > _stInstancePath.Length)
					{
						string text = _stExpression.Substring(_stInstancePath.Length);
						if (text.StartsWith("."))
						{
							text = text.Substring(1);
						}
						val = val3[text];
					}
					if (val == null)
					{
						val = WatchListNodeUtils.FindVariableAndOptionalDirectAddress(_stExpression, bLookForDirectAddressedStructMembers: false, string.Empty, out var _);
					}
					if (val == null)
					{
						val = WatchListNodeUtils.FindVariableSimple(_stExpression);
						if (val != null && val.Type != null && (int)val.Type.Class == 26)
						{
							_stComment = string.Empty;
							return _stComment;
						}
					}
					if (val == null)
					{
						_stComment = string.Empty;
						return _stComment;
					}
					string comment = val.Comment;
					if (!string.IsNullOrEmpty(comment))
					{
						comment = comment.Replace("<summary>", string.Empty).Replace("</summary>", string.Empty).Replace('\t', ' ');
						comment = NormalizeAndTrimMultiWhiteSpaces(comment);
						comment = comment.Trim();
						if (comment.Length > 80)
						{
							comment = comment.Substring(0, 80);
						}
						if (!string.IsNullOrEmpty(comment))
						{
							_stComment = comment;
						}
					}
				}
				return _stComment;
			}
		}

		internal IOnlineVarRef OnlineVarRef
		{
			get
			{
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Invalid comparison between Unknown and I4
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Expected O, but got Unknown
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0167: Expected O, but got Unknown
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Expected O, but got Unknown
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_018f: Expected O, but got Unknown
				//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ed: Expected O, but got Unknown
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Expected O, but got Unknown
				//IL_0207: Unknown result type (might be due to invalid IL or missing references)
				//IL_0211: Expected O, but got Unknown
				//IL_021d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0227: Expected O, but got Unknown
				//IL_0251: Unknown result type (might be due to invalid IL or missing references)
				//IL_0258: Invalid comparison between Unknown and I4
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0271: Invalid comparison between Unknown and I4
				//IL_0305: Unknown result type (might be due to invalid IL or missing references)
				//IL_030f: Expected O, but got Unknown
				//IL_031b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0325: Expected O, but got Unknown
				//IL_0343: Unknown result type (might be due to invalid IL or missing references)
				//IL_034d: Expected O, but got Unknown
				if (_onlineVarRef == null && MonitoringEnabled)
				{
					if (VarRef == null || VarRef.GetFlag((VarRefFlag)4))
					{
						throw new InvalidVarRefException(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "Monitoring_InvalidExpression"));
					}
					if (VarRef.GetFlag((VarRefFlag)8))
					{
						if (VarRef.GetFlag((VarRefFlag)16))
						{
							if (!(VarRef.AddressInfo is IPropertyAddressInfoExtended) && (VarRef.WatchExpression.Type == null || (int)((IType)VarRef.WatchExpression.Type).Class == 28))
							{
								throw new InvalidVarRefException(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "MonitoringImpossible"));
							}
						}
						else if (!VarRef.GetFlag((VarRefFlag)32))
						{
							throw new InvalidVarRefException(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "MonitoringDisabled"));
						}
					}
					Guid guid = WatchListNodeUtils.DetermineApplicationGuidIfNecessary(_explicitApplicationGuid);
					if (VarRef.GetFlag((VarRefFlag)512))
					{
						IInterfaceOnlineVarRef val = APEnvironment.CreateInterfaceOnlineVarRef();
						((IReferencingOnlineVarRef)val).Initialize(Model.InterfaceMonitoringHelper, VarRef, guid);
						((IReferencingOnlineVarRef)val).ReferencedInstanceAvailable+=(new OnlineVarRefEventHandler(OnReferencedInstanceAvailable));
						((IReferencingOnlineVarRef)val).ValueChanged+=(new OnlineVarRefEventHandler(OnValueChanged));
						((IOnlineVarRef)val).Changed+=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
						((IBPManager)APEnvironment.BPMgr).BreakpointChanged+=(new BPEventHandler(bpmanager_BreakpointChanged));
						_onlineVarRef = (IOnlineVarRef)(object)val;
					}
					else if (IsExplicitlySpecifiedAddress())
					{
						_onlineVarRef = (IOnlineVarRef)(object)new AddressOnlineVarRef(this, guid);
					}
					else if (IsPointerToUserDefType())
					{
						IPointerOnlineVarRef val2 = APEnvironment.CreatePointerOnlineVarRef();
						((IReferencingOnlineVarRef)val2).Initialize(Model.InterfaceMonitoringHelper, VarRef, guid);
						((IReferencingOnlineVarRef)val2).ReferencedInstanceAvailable+=(new OnlineVarRefEventHandler(OnReferencedInstanceAvailable));
						((IReferencingOnlineVarRef)val2).ValueChanged+=(new OnlineVarRefEventHandler(OnValueChanged));
						((IOnlineVarRef)val2).Changed+=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
						((IBPManager)APEnvironment.BPMgr).BreakpointChanged+=(new BPEventHandler(bpmanager_BreakpointChanged));
						_onlineVarRef = (IOnlineVarRef)(object)val2;
					}
					else if (VarRef.GetFlag((VarRefFlag)2) && (int)((IType)VarRef.WatchExpression.Type).Class != 22 && (int)((IType)VarRef.WatchExpression.Type).Class != 45)
					{
						_onlineVarRef = (IOnlineVarRef)(object)new CompoundOnlineVarRef();
					}
					else
					{
						PointerInstanceWatchListNode pointerInstanceWatchListNode = Parent as PointerInstanceWatchListNode;
						IVarRef varRef = VarRef;
						IVarRef4 val3 = (IVarRef4)(object)((varRef is IVarRef4) ? varRef : null);
						if (pointerInstanceWatchListNode != null && val3 != null)
						{
							int num = _stExpression.LastIndexOf('.');
							string text = ((num >= 0) ? _stExpression.Substring(num) : _stExpression);
							val3.DisplayExpression=(pointerInstanceWatchListNode.DereferencedPointer + text);
						}
						_onlineVarRef = ((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(VarRef);
						_onlineVarRef.Changed+=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
						((IBPManager)APEnvironment.BPMgr).BreakpointChanged+=(new BPEventHandler(bpmanager_BreakpointChanged));
						OnOnlineVarRefChanged(_onlineVarRef);
					}
					Model.View.BeforeEdit+=(new TreeTableViewEditEventHandler(OnBeforePreparedValueEditing));
				}
				return _onlineVarRef;
			}
		}

		private Image Image
		{
			get
			{
				if (_image == null && !IsEmpty && !_bInvalidExpression)
				{
					_image = WatchListNodeUtils.DetermineImageForExpression(_stExpression);
					try
					{
						if (IsExplicitlySpecifiedAddress() || CastExpressionFormatter.Instance.IsCastExpression(_stExpression))
						{
							_image = WatchListNodeUtils.IMAGE_VAR;
						}
					}
					catch
					{
					}
				}
				return _image;
			}
		}

		public string DisplayExpression => _stDisplayExpression;

		public string Expression => _stExpression;

		public bool IsEmpty => _stExpression.Trim().Length == 0;

		public bool HasChildren
		{
			get
			{
				try
				{
					return !IsEmpty && VarRef.GetFlag((VarRefFlag)2);
				}
				catch
				{
					return false;
				}
			}
		}

		public int ChildCount
		{
			get
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Invalid comparison between Unknown and I4
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Invalid comparison between Unknown and I4
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Invalid comparison between Unknown and I4
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Invalid comparison between Unknown and I4
				int result = 0;
				try
				{
					if (VarRef != null && VarRef.GetFlag((VarRefFlag)8) && VarRef.GetFlag((VarRefFlag)16))
					{
						bool flag = false;
						if ((int)((IType)VarRef.WatchExpression.Type).Class == 28)
						{
							ICompiledType type = VarRef.WatchExpression.Type;
							IUserdefType2 val = (IUserdefType2)(object)((type is IUserdefType2) ? type : null);
							Guid guidApplication = GuidApplication;
							if (guidApplication == Guid.Empty)
							{
								return 0;
							}
							ICompileContext referenceContext = Common.GetReferenceContext(guidApplication);
							if (referenceContext == null)
							{
								return 0;
							}
							ISignature val2 = referenceContext.CreateGlobalIScope()[((IUserdefType)val).SignatureId];
							if ((int)val2.POUType == 119 || val2.GetFlag((SignatureFlag)32768))
							{
								flag = true;
							}
						}
						if (VarRef.WatchExpression.Type == null || ((int)((IType)VarRef.WatchExpression.Type).Class != 22 && (int)((IType)VarRef.WatchExpression.Type).Class != 23 && !flag))
						{
							return 0;
						}
					}
					FillChildren();
					result = _childNodes.Count;
					return result;
				}
				catch (InvalidVarRefException)
				{
					return result;
				}
			}
		}

		internal Guid GuidApplication
		{
			get
			{
				if (_stExpression == null)
				{
					return Guid.Empty;
				}
				IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(_stExpression, false, false, false, false);
				val.AllowMultipleUnderlines=(true);
				IToken val2 = default(IToken);
				if ((int)val.GetNext(out val2) != 13)
				{
					return Guid.Empty;
				}
				string identifier = val.GetIdentifier(val2);
				string stApplication = string.Empty;
				IToken val3 = default(IToken);
				if ((int)val.GetNext(out val3) == 15 && (int)val.GetOperator(val3) == 162 && (int)val.GetNext(out val2) == 13)
				{
					stApplication = val.GetIdentifier(val2);
				}
				else
				{
					val.SetPosition(val3);
				}
				if ((int)val.GetNext(out val2) != 15 || (int)val.GetOperator(val2) != 162)
				{
					return Guid.Empty;
				}
				if (_explicitApplicationGuid != Guid.Empty)
				{
					return _explicitApplicationGuid;
				}
				return Common.GetApplicationGuid(identifier, stApplication);
			}
		}

		public string InstancePath => _stInstancePath;

		public ITreeTableNode Parent => (ITreeTableNode)(object)_parentNode;

		public bool MonitoringEnabled
		{
			get
			{
				if (!IsEmpty)
				{
					return _bMonitoringEnabled;
				}
				return false;
			}
			set
			{
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Expected O, but got Unknown
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Expected O, but got Unknown
				try
				{
					if (!IsEmpty && value != _bMonitoringEnabled)
					{
						_bMonitoringEnabled = value;
						if (value)
						{
							OnlineVarRef.ResumeMonitoring();
						}
						else if (_onlineVarRef != null)
						{
							_onlineVarRef.Changed-=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
							((IBPManager)APEnvironment.BPMgr).BreakpointChanged-=(new BPEventHandler(bpmanager_BreakpointChanged));
							_onlineVarRef.Release();
							_onlineVarRef = null;
						}
					}
				}
				catch
				{
				}
			}
		}

		private int Index
		{
			get
			{
				if (_parentNode == null)
				{
					return ((AbstractTreeTableModel)Model).Sentinel.GetIndex((ITreeTableNode)(object)this);
				}
				return _parentNode.GetIndex((ITreeTableNode)(object)this);
			}
		}

		public bool IsNetVarGVL { get; set; }

		public bool IsInstanceNode { get; set; }

		public bool Ready
		{
			get
			{
				bool result = false;
				try
				{
					_ = ChildCount;
					result = true;
					return result;
				}
				catch
				{
					return result;
				}
			}
		}

		public virtual string NodeLabel
		{
			get
			{
				ExpressionData valueExpression = GetValueExpression();
				if (valueExpression == null)
				{
					return string.Empty;
				}
				object valueApplicationPrefix = GetValueApplicationPrefix();
				string text = ((valueApplicationPrefix == null) ? string.Empty : valueApplicationPrefix.ToString());
				string text2 = valueExpression.DisplayExpression;
				if (!string.IsNullOrEmpty(text))
				{
					text2 = $"{text}.{text2}";
				}
				return text2;
			}
		}

		public WatchListNode(WatchListModel model, string stInstancePath, string stExpression, IConverterToIEC converter, bool bShowCommentColumn, int nProjectHandle, Guid guidObject, Guid guidApplication)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			if (stExpression == null)
			{
				throw new ArgumentNullException("stExpression");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			_model = model;
			_stExpression = stExpression;
			_stDisplayExpression = stExpression;
			_nProjectHandle = nProjectHandle;
			_guidObject = guidObject;
			_stInstancePath = stInstancePath;
			_explicitApplicationGuid = guidApplication;
			if (_stExpression == string.Empty)
			{
				_converter = converter;
			}
			else
			{
				SetDelayedCallToGetEffectiveConverterNeeded(converter);
				if (COLUMN_APPLICATION_PREFIX > -1)
				{
					_prefixItem = WatchListNodeUtils.CreateAppPrefixItem(_stExpression, bIgnoreActiveApp: true, out _stDisplayExpression);
				}
			}
			_bShowCommentColumn = bShowCommentColumn;
		}

		public WatchListNode(WatchListNode parentNode, string stInstancePath, string stExpression, bool bShowCommentColumn, int nProjectHandle, Guid guidObject, Guid guidApplication)
		{
			if (parentNode == null)
			{
				throw new ArgumentNullException("parentNode");
			}
			if (stExpression == null)
			{
				throw new ArgumentNullException("stExpression");
			}
			_model = parentNode.Model;
			_parentNode = parentNode;
			_stExpression = stExpression;
			_stDisplayExpression = stExpression;
			_stInstancePath = stInstancePath;
			_nProjectHandle = nProjectHandle;
			_guidObject = guidObject;
			_explicitApplicationGuid = guidApplication;
			SetDelayedCallToGetEffectiveConverterNeeded(parentNode.Converter);
			_bShowCommentColumn = bShowCommentColumn;
		}

		private void SetDelayedCallToGetEffectiveConverterNeeded(IConverterToIEC globalConverter)
		{
			_globalConverter = globalConverter;
			_bCallToGetEffectiveConverterNeeded = true;
		}

		protected IConverterToIEC GetConverter()
		{
			if (_bCallToGetEffectiveConverterNeeded)
			{
				_bCallToGetEffectiveConverterNeeded = false;
				if (_globalConverter == null)
				{
					_globalConverter = WatchListModel.GetConverter(GlobalOptionsHelper.DisplayMode);
				}
				_converter = WatchListNodeUtils.GetEffectiveConverter(_globalConverter, VarRef);
			}
			return _converter;
		}

		internal void SetWatchPoint(WatchPointItem wpi)
		{
			_watchpoint = wpi;
			_varRef = null;
			Release();
			if (_varRef == null && _watchpoint != null && _watchpoint.Breakpoint != null)
			{
				IBP4 breakpoint = _watchpoint.Breakpoint;
				_varRef = APEnvironment.BPMgr.AddWatchVariable(breakpoint, _stExpression);
				if (_varRef != null)
				{
					int bInvalidExpression;
					if (WatchListNodeUtils.IsValidExpression(_stExpression, _explicitApplicationGuid, ref _editorguidObject))
					{
						IVarRef varRef = _varRef;
						bInvalidExpression = ((!WatchListNodeUtils.IsValidCastType((varRef != null) ? varRef.WatchExpression : null)) ? 1 : 0);
					}
					else
					{
						bInvalidExpression = 1;
					}
					_bInvalidExpression = (byte)bInvalidExpression != 0;
				}
			}
			RaiseWatchPointChanged();
		}

		internal void AddReferencedInstanceChildNode()
		{
			DoGenericModelModifyingActionAndPreserveExpansionState(delegate
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Expected O, but got Unknown
				WatchListModel model = Model;
				if (model != null)
				{
					((AbstractTreeTableModel)model).RaiseStructureChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)this, -1, (ITreeTableNode)null));
				}
			});
		}

		private void OnValueChanged(IOnlineVarRef varRef)
		{
			RaiseValueChanged();
		}

		private void OnReferencedInstanceAvailable(IOnlineVarRef varRef)
		{
			AddReferencedInstanceChildNode();
		}

		internal bool IsExplicitlySpecifiedAddress()
		{
			return IsExplicitlySpecifiedAddress(bCheckOnlyThisNode: true);
		}

		internal bool IsExplicitlySpecifiedAddress(bool bCheckOnlyThisNode)
		{
			if (IsEmpty)
			{
				return false;
			}
			bool flag = false;
			try
			{
				flag = VarRef != null && VarRef.AddressInfo is IAddressAddressInfo;
			}
			catch (ArgumentException)
			{
			}
			catch (InvalidVarRefException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			if (!flag && Parent != null && !bCheckOnlyThisNode)
			{
				flag = _parentNode.IsExplicitlySpecifiedAddress(bCheckOnlyThisNode: false);
			}
			return flag;
		}

		internal bool IsPointerToUserDefType()
		{
			bool flag = VarRef.GetFlag((VarRefFlag)2);
			if (flag)
			{
				flag = APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)14, (ushort)0);
			}
			IType val = null;
			if (flag)
			{
				ICompiledType type = VarRef.WatchExpression.Type;
				IPointerType val2 = (IPointerType)(object)((type is IPointerType) ? type : null);
				if (val2 == null)
				{
					flag = false;
				}
				else
				{
					val = val2.Base;
					flag = val != null;
				}
			}
			if (flag)
			{
				flag = val is IUserdefType;
			}
			return flag;
		}

		internal void bpmanager_BreakpointChanged(object sender, BPEventArgs e)
		{
			if (_watchpoint != null && e.Breakpoint == _watchpoint.Breakpoint)
			{
				RaiseValueChanged();
			}
		}

		private void Release()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			if (_childNodes != null)
			{
				foreach (WatchListNode childNode in _childNodes)
				{
					childNode.Release();
				}
			}
			if (_onlineVarRef != null)
			{
				_onlineVarRef.Changed-=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
				_onlineVarRef.Release();
				((IBPManager)APEnvironment.BPMgr).BreakpointChanged-=(new BPEventHandler(bpmanager_BreakpointChanged));
				_onlineVarRef = null;
				Model.View.BeforeEdit-=(new TreeTableViewEditEventHandler(OnBeforePreparedValueEditing));
			}
		}

		internal ICompiledType GetCompiledType()
		{
			if (VarRef == null)
			{
				return null;
			}
			if (VarRef.WatchExpression == null)
			{
				return null;
			}
			return VarRef.WatchExpression.Type;
		}

		public virtual object GetValue(int nColumnIndex)
		{
			if (nColumnIndex == _model.COLUMN_APPLICATION_PREFIX)
			{
				return GetValueApplicationPrefix();
			}
			if (nColumnIndex == _model.COLUMN_EXPRESSION)
			{
				return GetValueExpression();
			}
			if (nColumnIndex == COLUMN_WP)
			{
				return GetValueWatchpoint();
			}
			if (nColumnIndex == COLUMN_COMMENT)
			{
				return GetValueComment();
			}
			if (nColumnIndex == COLUMN_TYPE)
			{
				return GetValueType();
			}
			if (nColumnIndex == COLUMN_VALUE)
			{
				return GetValueValue();
			}
			if (nColumnIndex == COLUMN_PREPARED_VALUE)
			{
				return GetValuePreparedValue();
			}
			if (nColumnIndex == COLUMN_DIRECT_ADDR)
			{
				return GetValueDirectAddress();
			}
			throw new ArgumentOutOfRangeException("nColumnIndex");
		}

		private object GetValueApplicationPrefix()
		{
			if (_parentNode == null && !string.IsNullOrEmpty(_stExpression))
			{
				if (_prefixItem == null)
				{
					_prefixItem = WatchListNodeUtils.CreateAppPrefixItem(_stExpression, bIgnoreActiveApp: true, out _stDisplayExpression);
				}
				return _prefixItem;
			}
			return string.Empty;
		}

		private ExpressionData GetValueExpression()
		{
			if (_parentNode != null || (_model.InstancePath != null && _model.InstancePath.Length > 0))
			{
				if (IsInstanceNode)
				{
					return new ExpressionData(GetLocalizedExpression(_stExpression), _stDisplayExpression, Image);
				}
				int num = _stExpression.IndexOf("__CAST");
				int num2 = 0;
				num2 = ((0 > num) ? _stExpression.LastIndexOf('.') : _stExpression.Substring(0, num).LastIndexOf('.'));
				string empty = string.Empty;
				if (ImplicitInstancePointer)
				{
					empty = "THIS";
				}
				else if (_parentNode != null && _parentNode.ImplicitInstancePointer)
				{
					empty = "THIS^";
				}
				else
				{
					empty = ((num2 >= 0) ? _stExpression.Substring(num2 + 1) : _stExpression);
					empty = GetLocalizedExpression(empty);
				}
				return new ExpressionData(_stExpression, empty, Image)
				{
					IsOutdated = _bIsOutdated
				};
			}
			return new ExpressionData(GetLocalizedExpression(_stExpression), _stDisplayExpression, Image);
		}

		private object GetValueWatchpoint()
		{
			if (IsEmpty)
			{
				return string.Empty;
			}
			if (_watchpoint == null)
			{
				_watchpoint = new WatchPointItem(null);
			}
			return _watchpoint;
		}

		private string GetValueComment()
		{
			try
			{
				if (IsEmpty)
				{
					return string.Empty;
				}
				return GetLocalizedExpression(Comment);
			}
			catch (Exception ex)
			{
				return $"<{ex.Message}>";
			}
		}

		private string GetValueType()
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Invalid comparison between Unknown and I4
			if (!IsEmpty)
			{
				try
				{
					IVarRef varRef = VarRef;
					if (varRef == null)
					{
						return string.Empty;
					}
					if (varRef.WatchExpression == null)
					{
						return string.Empty;
					}
					if (varRef.WatchExpression.Type == null)
					{
						return string.Empty;
					}
					if ((int)((IType)varRef.WatchExpression.Type).Class == 1)
					{
						return "BIT";
					}
					return GetLocalizedExpression(((object)varRef.WatchExpression.Type).ToString());
				}
				catch (InvalidVarRefException)
				{
					return string.Empty;
				}
				catch (Exception ex)
				{
					return $"<{ex.Message}>";
				}
			}
			return string.Empty;
		}

		private ValueData GetValueValue()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Invalid comparison between Unknown and I4
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Invalid comparison between Unknown and I4
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Invalid comparison between Unknown and I4
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Invalid comparison between Unknown and I4
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Invalid comparison between Unknown and I4
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Invalid comparison between Unknown and I4
			try
			{
				if (IsExplicitlySpecifiedAddress())
				{
					IAddressInfo addressInfo = VarRef.AddressInfo;
					IAddressAddressInfo val = (IAddressAddressInfo)(object)((addressInfo is IAddressAddressInfo) ? addressInfo : null);
					return new RawValueData(this, ((IType)VarRef.WatchExpression.Type).Class, Common.ConvertToSmallestPointer(val.Address), GetConverter(), bConstant: false, bForced: false);
				}
				if (!IsEmpty && MonitoringEnabled && (int)OnlineVarRef.State != 1)
				{
					if (_watchpoint != null && _watchpoint.Breakpoint != null && !_watchpoint.Breakpoint.Executed && !_watchpoint.Breakpoint.Reached)
					{
						return new ErrorValueData(this, Strings.EP_NotReached);
					}
					if ((int)OnlineVarRef.State == 0)
					{
						object obj = OnlineVarRef.Value;
						ICompiledType val2 = VarRef.WatchExpression.Type;
						if ((int)((IType)val2).Class == 23)
						{
							val2 = val2.BaseType;
						}
						if ((int)((IType)val2).Class == 24)
						{
							val2 = val2.DeRefType;
						}
						if ((int)((IType)val2).Class == 45)
						{
							return new VectorValueData(this, obj);
						}
						if (obj != null && typeof(string).IsAssignableFrom(obj.GetType()))
						{
							obj = ((string)obj).Replace("\r", "$R");
							obj = ((string)obj).Replace("\n", "$N");
							obj = ((string)obj).Replace("$$", "$");
							obj = ((string)obj).Replace("$'", "'");
						}
						return new RawValueData(this, ((IType)val2).Class, obj, GetConverter(), VarRef.GetFlag((VarRefFlag)1), OnlineVarRef.Forced);
					}
					if ((int)OnlineVarRef.State == 4)
					{
						return new ErrorValueData(this, Strings.MonitoringNotLoggedIn);
					}
					if ((int)OnlineVarRef.State == 3)
					{
						if (_stInstancePath == string.Empty && GuidApplication != Guid.Empty && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && APEnvironment.ObjectMgr.ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, GuidApplication))
						{
							IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(GuidApplication);
							if (application != null && !application.IsLoggedIn)
							{
								return new ErrorValueData(this, Strings.MonitoringNotLoggedIn);
							}
						}
						return new ErrorValueData(this, OnlineVarRef.GetStateMessage());
					}
					return new ErrorValueData(this, OnlineVarRef.GetStateMessage());
				}
				return new EmptyValueData(this);
			}
			catch (InvalidOperationException)
			{
				return new EmptyValueData(this);
			}
			catch (Exception ex2)
			{
				return new ErrorValueData(this, ex2.Message);
			}
		}

		private ValueData GetValuePreparedValue()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Invalid comparison between Unknown and I4
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!IsEmpty && MonitoringEnabled && (int)OnlineVarRef.State != 1)
				{
					ICompiledType val = VarRef.WatchExpression.Type;
					if ((int)((IType)val).Class == 23)
					{
						val = val.BaseType;
					}
					if ((int)((IType)val).Class == 24)
					{
						val = val.DeRefType;
					}
					return new RawValueData(this, ((IType)val).Class, OnlineVarRef.PreparedValue, GetConverter(), VarRef.GetFlag((VarRefFlag)1), OnlineVarRef.Forced);
				}
				return new EmptyValueData(this);
			}
			catch (InvalidOperationException)
			{
				return new EmptyValueData(this);
			}
			catch (InvalidVarRefException)
			{
				return new EmptyValueData(this);
			}
			catch (Exception ex2)
			{
				return new ErrorValueData(this, ex2.Message);
			}
		}

		private string GetValueDirectAddress()
		{
			try
			{
				if (!IsEmpty)
				{
					return DirectAddress;
				}
				return string.Empty;
			}
			catch (Exception ex)
			{
				return $"<{ex.Message}>";
			}
		}

		private string GetLocalizedExpression(string stDisplayExpression)
		{
			if (Model.LocalizationActive && APEnvironment.LocalizationManagerOrNull != null)
			{
				return APEnvironment.LocalizationManagerOrNull.GetLocalization(stDisplayExpression);
			}
			return stDisplayExpression;
		}

		public void SetValue(int nColumnIndex, object value)
		{
			if (nColumnIndex == COLUMN_APPLICATION_PREFIX)
			{
				SetValueApplicationPrefix(value);
				return;
			}
			if (nColumnIndex == COLUMN_EXPRESSION)
			{
				SetValueExpression(value);
				return;
			}
			if (nColumnIndex == COLUMN_WP)
			{
				SetValuePreparedWatchpoint(value);
				return;
			}
			if (nColumnIndex == COLUMN_PREPARED_VALUE)
			{
				SetValuePreparedValue(value);
				return;
			}
			if (nColumnIndex == COLUMN_TYPE || nColumnIndex == COLUMN_VALUE || nColumnIndex == COLUMN_DIRECT_ADDR)
			{
				throw new InvalidOperationException("This node is read-only.");
			}
			throw new ArgumentOutOfRangeException("nColumnIndex");
		}

		private void SetValueApplicationPrefix(object value)
		{
			if (value == null)
			{
				return;
			}
			if (!(value is string))
			{
				throw new ArgumentException("Value must be of type string.");
			}
			if (_prefixItem == null)
			{
				_prefixItem = WatchListNodeUtils.CreateAppPrefixItem(_stExpression, bIgnoreActiveApp: false, out _stDisplayExpression);
			}
			if ((string)value == _prefixItem.ApplicationPrefix)
			{
				return;
			}
			if (!string.IsNullOrEmpty(_prefixItem.ApplicationPrefix))
			{
				if (_stExpression.StartsWith(_prefixItem.ApplicationPrefix))
				{
					_stExpression = _stExpression.Replace(_prefixItem.ApplicationPrefix, "");
					_stExpression = (string)value + _stExpression;
				}
				if (_stDisplayExpression.StartsWith(_prefixItem.ApplicationPrefix))
				{
					_stDisplayExpression = _stDisplayExpression.Replace(_prefixItem.ApplicationPrefix, "");
				}
			}
			_prefixItem.ApplicationPrefix = (string)value;
			_varRef = null;
			_image = null;
			_stComment = null;
			_stDirectAddress = null;
			_watchpoint = null;
			MonitoringRange = null;
			SetDelayedCallToGetEffectiveConverterNeeded(_converter);
			Release();
			RemoveChildNodesIfExist();
			try
			{
				RaiseExpressionChanged();
				RaiseStructureChanged();
			}
			catch
			{
			}
			try
			{
				MonitoringEnabled = true;
			}
			catch
			{
			}
		}

		private void SetValueExpression(object value)
		{
			if (value == null)
			{
				return;
			}
			if (!(value is ExpressionData))
			{
				throw new ArgumentException("value must be of type ExpressionData.");
			}
			if (_model.ReadOnly)
			{
				throw new InvalidOperationException("The entire watchlist is read-only.");
			}
			if (_parentNode != null)
			{
				throw new InvalidOperationException("Child nodes are read-only.");
			}
			if (_stDisplayExpression == ((ExpressionData)value).DisplayExpression && _stExpression == ((ExpressionData)value).DisplayExpression)
			{
				return;
			}
			if (((ExpressionData)value).DisplayExpression == ((ExpressionData)value).FullExpression && !string.IsNullOrEmpty(((ExpressionData)value).FullExpression))
			{
				if (_prefixItem == null || _prefixItem.ApplicationPrefix == string.Empty)
				{
					_prefixItem = WatchListNodeUtils.CreateAppPrefixItem(((ExpressionData)value).DisplayExpression, bIgnoreActiveApp: false, out _stDisplayExpression);
				}
				else
				{
					WatchListNodeUtils.CreateAppPrefixItem(((ExpressionData)value).DisplayExpression, bIgnoreActiveApp: false, out _stDisplayExpression);
				}
				_stExpression = ((ExpressionData)value).DisplayExpression;
				if (_prefixItem != null && !string.IsNullOrEmpty(_prefixItem.ApplicationPrefix) && !_stExpression.StartsWith(_prefixItem.ApplicationPrefix))
				{
					_stExpression = _prefixItem.ApplicationPrefix + "." + _stExpression;
				}
			}
			else
			{
				_stExpression = ((ExpressionData)value).DisplayExpression;
				_stDisplayExpression = _stExpression;
			}
			_varRef = null;
			_image = null;
			_stComment = null;
			_stDirectAddress = null;
			_watchpoint = null;
			MonitoringRange = null;
			if (!string.IsNullOrEmpty(_stExpression))
			{
				SetDelayedCallToGetEffectiveConverterNeeded(_converter);
			}
			Release();
			RemoveChildNodesIfExist();
			if (!IsEmpty)
			{
				try
				{
					RaiseExpressionChanged();
					RaiseStructureChanged();
				}
				catch
				{
				}
				try
				{
					MonitoringEnabled = true;
				}
				catch
				{
				}
			}
			else
			{
				Model.Remove(this);
			}
		}

		private void SetValuePreparedWatchpoint(object value)
		{
			if (value != null)
			{
				WatchPointItem watchPointItem = value as WatchPointItem;
				if (watchPointItem == null)
				{
					throw new ArgumentException("value must be of type WatchPointItem");
				}
				if (_watchpoint.Breakpoint != null && _watchpoint.Breakpoint != watchPointItem.Breakpoint)
				{
					APEnvironment.BPMgr.RemoveWatchVariable(_watchpoint.Breakpoint, _stExpression);
				}
				SetWatchPoint(watchPointItem);
			}
		}

		private void OnBeforePreparedValueEditing(object sender, TreeTableViewEditEventArgs e)
		{
			if (e.ColumnIndex != COLUMN_PREPARED_VALUE)
			{
				return;
			}
			ITreeTableNode modelNode = Model.View.GetModelNode(e.Node);
			if (!modelNode.IsEditable(COLUMN_PREPARED_VALUE) || modelNode != this)
			{
				return;
			}
			ValueData valuePreparedValue = GetValuePreparedValue();
			if (valuePreparedValue.Toggleable || !(valuePreparedValue.Text == string.Empty))
			{
				return;
			}
			try
			{
				ValueData valueValue = GetValueValue();
				if (!(valueValue is ErrorValueData) && !valueValue.Forced)
				{
					SetValuePreparedValue(valueValue);
				}
			}
			catch
			{
			}
		}

		private void SetValuePreparedValue(object value)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			if (value == null)
			{
				return;
			}
			ValueData valueData = value as ValueData;
			if (valueData == null)
			{
				throw new ArgumentException("value must be of type ValueData");
			}
			if (IsEmpty)
			{
				throw new InvalidOperationException();
			}
			APEnvironment.MonitoringUtilities.CheckValidAddress(_stExpression);
			if (!string.IsNullOrEmpty(valueData.Text))
			{
				try
				{
					APEnvironment.MonitoringUtilities.CheckValidValue(VarRef, valueData.Text, GuidApplication);
				}
				catch (FormatException innerException)
				{
					throw new FormatException(string.Format(Strings.IncompatibleValue, valueData.Text, ((object)VarRef.WatchExpression.Type).ToString()), innerException);
				}
				catch (System.OverflowException innerException2)
				{
					throw new FormatException(string.Format(Strings.IncompatibleValue, valueData.Text, ((object)VarRef.WatchExpression.Type).ToString()), innerException2);
				}
			}
			try
			{
				OnlineVarRef.PreparedValue=(valueData.Value);
			}
			catch
			{
				if (!WatchListNodeUtils.TryToConvertBinHexValue(valueData.Text, OnlineVarRef, ((IType)VarRef.WatchExpression.Type).Class, out var _))
				{
					string text = valueData.Text;
					string arg = (string)GetValue(COLUMN_TYPE);
					throw new ApplicationException(string.Format(Strings.IncompatibleValue, text, arg));
				}
			}
		}

		public int GetIndex(ITreeTableNode node)
		{
			return -1;
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			FillChildren();
			return (ITreeTableNode)_childNodes[nIndex];
		}

		internal void ClearChildren()
		{
			if (_childNodes == null)
			{
				return;
			}
			foreach (WatchListNode childNode in _childNodes)
			{
				childNode.BeforeClear();
			}
			_childNodes.Clear();
		}

		protected virtual void BeforeClear()
		{
		}

		internal bool IsInterfaceNode()
		{
			if (!IsEmpty && VarRef != null)
			{
				return VarRef.GetFlag((VarRefFlag)512);
			}
			return false;
		}

		internal bool DetermineInterfaceInstanceSignatureIfPossible(out bool bAddressChanged, out ulong ulAddressInstance, out ISignature signInstance)
		{
			ulAddressInstance = 0uL;
			signInstance = null;
			bAddressChanged = false;
			bool result = false;
			IOnlineVarRef onlineVarRef = OnlineVarRef;
			IInterfaceOnlineVarRef val = (IInterfaceOnlineVarRef)(object)((onlineVarRef is IInterfaceOnlineVarRef) ? onlineVarRef : null);
			if (val != null)
			{
				result = ((IReferencingOnlineVarRef)val).DetermineInstanceSignatureIfPossible(true, out bAddressChanged, out ulAddressInstance, out signInstance);
			}
			return result;
		}

		internal bool DeterminePointerInstanceSignatureIfPossible(out bool bAddressChanged, out ulong ulAddressInstance, out ISignature signInstance)
		{
			ulAddressInstance = 0uL;
			signInstance = null;
			bAddressChanged = false;
			bool result = false;
			IOnlineVarRef onlineVarRef = OnlineVarRef;
			IPointerOnlineVarRef val = (IPointerOnlineVarRef)(object)((onlineVarRef is IPointerOnlineVarRef) ? onlineVarRef : null);
			if (val != null)
			{
				result = ((IReferencingOnlineVarRef)val).DetermineInstanceSignatureIfPossible(true, out bAddressChanged, out ulAddressInstance, out signInstance);
			}
			return result;
		}

		internal bool DetermineAddressIfPossible(out ulong ulAddress, out ICompiledType type)
		{
			ulAddress = 0uL;
			type = null;
			bool result = false;
			if (IsExplicitlySpecifiedAddress())
			{
				IAddressInfo addressInfo = VarRef.AddressInfo;
				IAddressAddressInfo val = (IAddressAddressInfo)(object)((addressInfo is IAddressAddressInfo) ? addressInfo : null);
				AddressOnlineVarRef addressOnlineVarRef = OnlineVarRef as AddressOnlineVarRef;
				if (val != null && addressOnlineVarRef != null)
				{
					ulAddress = val.Address;
					result = addressOnlineVarRef.DetermineTypeIfPossible(out type);
				}
			}
			return result;
		}

		internal void FillChildren()
		{
			if (_childNodes != null)
			{
				return;
			}
			_childNodes = new ArrayList();
			try
			{
				if (IsEmpty || !VarRef.GetFlag((VarRefFlag)2))
				{
					return;
				}
				WatchListNode[] array = _model.CreateChildNodes(_stExpression, -1, Guid.Empty, this);
				Debug.Assert(array != null);
				try
				{
					//IVariableCommentProvider obj = APEnvironment.CreateVariableCommentProvider();
					VariableCommentProvider val = new VariableCommentProvider();
					int num = _stExpression.IndexOf(".");
					string text = _stExpression.Substring(num + 1);
					if (_stExpression.Contains("Device"))
					{
						num = text.IndexOf(".");
						text = text.Substring(num + 1);
					}
					if (_stExpression.Contains("Application"))
					{
						num = text.IndexOf(".");
						text = text.Substring(num + 1);
					}
					if ((int)((IType)VarRef.WatchExpression.Type).Class == 26)
					{
						VariableCommentResult patchCommentElementForArray = val.GetPatchCommentElementForArray(text, _editorguidObject, new int[1]);
						if (patchCommentElementForArray != null)
						{
							int num2 = ((MonitoringRange != null) ? MonitoringRange.Item1 : 0);
							for (int i = 0; i < array.Count(); i++)
							{
								array[i]._stComment = patchCommentElementForArray.ChildComments[num2 + i].Comment;
							}
						}
					}
					if ((int)((IType)VarRef.WatchExpression.Type).Class == 28)
					{
						VariableCommentResult patchCommentElementForUserdef = val.GetPatchCommentElementForUserdef(text, _editorguidObject);
						if (patchCommentElementForUserdef != null)
						{
							SetChildNodeComment(array, patchCommentElementForUserdef);
						}
					}
				}
				catch
				{
					for (int j = 0; j < array.Count(); j++)
					{
						array[j]._stComment = string.Empty;
					}
				}
				_childNodes.AddRange(array);
				if (_bNodeWasExpandedBeforeRemovingChildren)
				{
					((IEngine)APEnvironment.Engine).InvokeInPrimaryThread((Delegate)new DoSomethingInPrimaryDelegate(Collapse), new object[0], true);
				}
			}
			catch
			{
			}
		}

		private void SetChildNodeComment(WatchListNode[] array, VariableCommentResult result)
		{
			if (array == null || array.Length == 0 || result == null || result.ChildComments == null || result.ChildComments.Count == 0)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string expression = array[i].Expression;
				if (expression.StartsWith(_stExpression))
				{
					string property = expression.Remove(0, _stExpression.Length + 1);
					VariableCommentResult val = (from t in result.ChildComments
												 where string.Equals(property, t.Property, StringComparison.OrdinalIgnoreCase)
												 select t).FirstOrDefault();
					if (val != null)
					{
						array[i]._stComment = val.Comment;
					}
				}
			}
		}

		private void RemoveChildNodesIfExist()
		{
			if (_childNodes != null)
			{
				WatchListModel model = _model;
				object obj;
				if (model == null)
				{
					obj = null;
				}
				else
				{
					TreeTableView view = model.View;
					obj = ((view != null) ? view.GetViewNode((ITreeTableNode)(object)this) : null);
				}
				TreeTableViewNode val = (TreeTableViewNode)obj;
				if (val != null)
				{
					_bNodeWasExpandedBeforeRemovingChildren = val.Expanded;
				}
				_childNodes.Clear();
				_childNodes = null;
			}
		}

		private void Collapse()
		{
			_model.View.GetViewNode((ITreeTableNode)(object)this).Collapse();
		}

		public bool IsEditable(int nColumnIndex)
		{
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			if (Model.IsForceListView && nColumnIndex != COLUMN_PREPARED_VALUE)
			{
				return false;
			}
			if (nColumnIndex == COLUMN_APPLICATION_PREFIX)
			{
				if (!Model.ReadOnly && _parentNode == null && !IsEmpty && _prefixItem != null)
				{
					return !string.IsNullOrEmpty(_prefixItem.ApplicationPrefix);
				}
				return false;
			}
			if (nColumnIndex == COLUMN_EXPRESSION)
			{
				if (!Model.ReadOnly)
				{
					return _parentNode == null;
				}
				return false;
			}
			if (nColumnIndex == COLUMN_COMMENT || nColumnIndex == COLUMN_TYPE || nColumnIndex == COLUMN_VALUE || nColumnIndex == COLUMN_DIRECT_ADDR)
			{
				return false;
			}
			if (nColumnIndex == COLUMN_WP)
			{
				if (_varRef != null && _varRef.AddressInfo != null)
				{
					if (!_varRef.GetFlag((VarRefFlag)2))
					{
						return !(_varRef.AddressInfo is IComplexAddressInfo);
					}
					return false;
				}
				return false;
			}
			if (nColumnIndex == COLUMN_PREPARED_VALUE)
			{
				if (_varRef != null)
				{
					if (_varRef.GetFlag((VarRefFlag)32))
					{
						if (((IEngine)APEnvironment.Engine).MessageService is IMessageService3)
						{
							((IMessageService3)((IEngine)APEnvironment.Engine).MessageService).Error(Strings.ErrorProperties, "Error_Properties", (object[])null);
						}
						else
						{
							((IEngine)APEnvironment.Engine).MessageService.Error(Strings.ErrorProperties);
						}
						return false;
					}
					if (_varRef.GetFlag((VarRefFlag)1))
					{
						if (((IEngine)APEnvironment.Engine).MessageService is IMessageService3)
						{
							((IMessageService3)((IEngine)APEnvironment.Engine).MessageService).Error(Strings.ErrorConstant, "Error_Constant", (object[])null);
						}
						else
						{
							((IEngine)APEnvironment.Engine).MessageService.Error(Strings.ErrorConstant);
						}
						return false;
					}
				}
				if (_onlineVarRef != null && _onlineVarRef is IOnlineVarRef4)
				{
					IOnlineVarRef onlineVarRef = _onlineVarRef;
					return ((IOnlineVarRef4)((onlineVarRef is IOnlineVarRef4) ? onlineVarRef : null)).Writeable;
				}
				if (!IsEmpty && _onlineVarRef != null && !(_onlineVarRef is CompoundOnlineVarRef) && _varRef != null && _varRef.AddressInfo != null)
				{
					if (!(_varRef.AddressInfo is IAbsoluteAddressInfo) && !(_varRef.AddressInfo is IStackRelativeAddressInfo) && !(_varRef.AddressInfo is IPropertyAddressInfo) && !(_varRef.AddressInfo is IArrayAccessAddressInfo) && !(_varRef.AddressInfo is IDeRefAccessInfo))
					{
						return _varRef.AddressInfo is ICompoAddressInfo;
					}
					return true;
				}
				return false;
			}
			throw new ArgumentOutOfRangeException("nColumnIndex");
		}

		internal void MarkAsOutdated()
		{
			_bIsOutdated = true;
		}

		private TreeTableModelEventArgs CreateEventArgs()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			return new TreeTableModelEventArgs((ITreeTableNode)(object)_parentNode, Index, (ITreeTableNode)(object)this);
		}

		private void RaiseExpressionChanged()
		{
			Model.RaiseExpressionChanged(CreateEventArgs());
		}

		private void RaiseApplicationPrefixChanged()
		{
			Model.RaiseApplicationPrefixChanged(CreateEventArgs());
		}

		private void RaiseWatchPointChanged()
		{
			Model.RaiseWatchPointChanged(CreateEventArgs());
		}

		internal void RaiseValueChanged()
		{
			if (IsInterfaceNode() || IsPointerToUserDefType() || IsExplicitlySpecifiedAddress())
			{
				DoGenericModelModifyingActionAndPreserveExpansionState(delegate
				{
					ClearChildren();
					_childNodes = null;
					FillChildren();
					RaiseExpressionChanged();
					RaiseStructureChanged();
					Model.View.GetViewNode((ITreeTableNode)(object)this).Collapse();
				});
			}
			Model.RaiseValueChanged(CreateEventArgs());
		}

		private void RaisePreparedValueChanged()
		{
			Model.RaisePreparedValueChanged(CreateEventArgs());
		}

		private void RaiseStructureChanged()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			((AbstractTreeTableModel)Model).RaiseStructureChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)this, -1, (ITreeTableNode)null));
		}

		internal void OnOnlineVarRefChanged(IOnlineVarRef onlineVarRef)
		{
			if (onlineVarRef == _onlineVarRef)
			{
				RaiseValueChanged();
			}
		}

		public void Dispose()
		{
			Dispose(bDisposing: true);
			GC.SuppressFinalize(this);
		}

		~WatchListNode()
		{
			Dispose(bDisposing: false);
		}

		private void Dispose(bool bDisposing)
		{
			if (!_bDisposed)
			{
				if (bDisposing)
				{
					Release();
				}
				_bDisposed = true;
			}
		}

		public string GetToolTipText(int nColumnIndex)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Invalid comparison between Unknown and I4
			if (_bIsOutdated)
			{
				return Strings.WatchListNodeIsOutdated;
			}
			if (nColumnIndex == COLUMN_VALUE && Expression != null && Expression != string.Empty && OnlineVarRef != null && (int)OnlineVarRef.State == 0 && VarRef != null && VarRef.WatchExpression != null && VarRef.WatchExpression.Type != null && (int)((IType)VarRef.WatchExpression.Type).Class == 25 && VarRef.WatchExpression.Type.BaseType != null)
			{
				try
				{
					return APEnvironment.CreateToolTipService().CreateWatchBoxToolTip(OnlineVarRef, VarRef, (string)null);
				}
				catch
				{
				}
			}
			return string.Empty;
		}

		private string NormalizeAndTrimMultiWhiteSpaces(string stInputComment)
		{
			char[] separator = new char[1] { ' ' };
			string[] array = stInputComment.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			string text = "";
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				text = text + text2 + " ";
			}
			return text.Trim();
		}

		private void DoGenericModelModifyingActionAndPreserveExpansionState(GenericModelModifyingAction action)
		{
			IEnumerable<string> expandedNodes = NodeExpansionHelper.Instance.GetExpandedNodes(_model.View);
			action();
			ICollection<ExpandedModelNodeDescription> collection = NodeExpansionHelper.Instance.CreateExpandedModelNodeDescriptionInstances(expandedNodes);
			NodeExpansionHelper.Instance.ExpandNodes(_model.View, collection);
		}
	}
}
