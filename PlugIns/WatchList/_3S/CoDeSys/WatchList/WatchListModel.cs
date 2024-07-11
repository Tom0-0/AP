#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.GVLObject;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.NVLObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	public class WatchListModel : AbstractTreeTableModel, IDisposable
	{
		internal static readonly IConverterToIEC s_binaryConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)0);

		internal static readonly IConverterToIEC s_decimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)1);

		internal static readonly IConverterToIEC s_hexadecimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)2);

		private int _nUpdateCount;

		private string _stInstancePath = string.Empty;

		private Guid _objectGuid = Guid.Empty;

		private int _nProjectHandle = -1;

		private Guid _guidApplication = Guid.Empty;

		private bool _bReadOnly;

		private WatchListNode[] _monitoringNodes = new WatchListNode[0];

		private TreeTableView _view;

		private bool _bDisposed;

		private bool _bShowCommentColumn;

		private bool _bShowWatchpointColumn;

		private bool _bLoadActiveWatchPoints;

		private bool _bEventsUnregistered;

		private bool _bIsOutdated;

		private bool _bIsForceListView;

		internal static int MAX_MONITORING_ELEMENTS = -1;

		internal static int MAX_MONITORING_ELEMENTS_PER_ARRAY = -1;

		internal static int DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY = -1;

		private static readonly string OPTIONKEY_DISPLAYMODE = "{EFD5A0D8-B437-41b2-87B6-1D745A74AD55}";

		private int _COLUMN_APPLICATION_PREFIX = -1;

		private int _COLUMN_EXPRESSION = -1;

		private int _COLUMN_WP = -1;

		private int _COLUMN_TYPE = -1;

		private int _COLUMN_VALUE = -1;

		private int _COLUMN_PREPARED_VALUE = -1;

		private int _COLUMN_DIRECT_ADDR = -1;

		private int _COLUMN_COMMENT = -1;

		private SortOrder _sortOrder;

		private int _sortColumn = -1;

		private IInterfaceMonitoringHelper _interfaceMonitoringHelper;

		private static readonly Guid INSTANCEPOINTER_GUID = new Guid("{E3941166-44AC-4CBD-84DE-0201A0496A2B}");

		internal bool LocalizationActive { get; set; }

		internal IInterfaceMonitoringHelper InterfaceMonitoringHelper => _interfaceMonitoringHelper;

		internal int COLUMN_APPLICATION_PREFIX => _COLUMN_APPLICATION_PREFIX;

		internal int COLUMN_EXPRESSION => _COLUMN_EXPRESSION;

		internal int COLUMN_WP => _COLUMN_WP;

		internal int COLUMN_TYPE => _COLUMN_TYPE;

		internal int COLUMN_VALUE => _COLUMN_VALUE;

		internal int COLUMN_PREPARED_VALUE => _COLUMN_PREPARED_VALUE;

		internal int COLUMN_DIRECT_ADDR => _COLUMN_DIRECT_ADDR;

		internal int COLUMN_COMMENT => _COLUMN_COMMENT;

		internal TreeTableView View
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value;
			}
		}

		public string InstancePath
		{
			get
			{
				return _stInstancePath;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_stInstancePath = value.Trim();
			}
		}

		public bool ReadOnly
		{
			get
			{
				return _bReadOnly;
			}
			set
			{
				_bReadOnly = value;
				EnsureEmptyNode();
			}
		}

		internal bool IsForceListView => _bIsForceListView;

		internal bool IsOutdated => _bIsOutdated;

		internal bool LoadActiveWatchPoints
		{
			get
			{
				return _bLoadActiveWatchPoints;
			}
			set
			{
				_bLoadActiveWatchPoints = value;
			}
		}

		public bool ShowCommentColumn => _bShowCommentColumn;

		internal int SortColumn => _sortColumn;

		internal SortOrder SortOrder => _sortOrder;

		public event EventHandler ExpressionInserted;

		public event EventHandler ExpressionChanged;

		public event EventHandler ExpressionRemoved;

		public event EventHandler AllExpressionsChanged;

		public WatchListModel(bool bShowCommentColumn, bool bShowWatchpointColumn, bool bIsForceListView)
			: base()
		{
			
			_bShowCommentColumn = bShowCommentColumn;
			_bShowWatchpointColumn = bShowWatchpointColumn;
			_bIsForceListView = bIsForceListView;
			_interfaceMonitoringHelper = APEnvironment.CreateInterfaceMonitoringHelper();
			int num = 0;
			UnderlyingModel.AddColumn(Strings.Expression, HorizontalAlignment.Left, ExpressionRenderer.Singleton, ExpressionEditor.Singleton, true);
			_COLUMN_EXPRESSION = num++;
			if (bShowWatchpointColumn)
			{
				UnderlyingModel.AddColumn(Strings.Application, HorizontalAlignment.Left, ApplicationPrefixRenderer.Singleton, ApplicationPrefixEditor.Singleton, true);
				_COLUMN_APPLICATION_PREFIX = num++;
			}
			UnderlyingModel.AddColumn(Strings.Type, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			_COLUMN_TYPE = num++;
			UnderlyingModel.AddColumn(Strings.Value, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithForceIndicator, (ITreeTableViewEditor)(object)ValueDataEditor.Singleton, false);
			_COLUMN_VALUE = num++;
			UnderlyingModel.AddColumn(Strings.PreparedValue, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithoutForceIndicator, (ITreeTableViewEditor)(object)ValueDataEditor.Singleton, true);
			_COLUMN_PREPARED_VALUE = num++;
			if (bShowWatchpointColumn)
			{
				UnderlyingModel.AddColumn(Strings.Executionpoint, HorizontalAlignment.Left, WatchPointRenderer.Singleton, WatchPointEditor.Singleton, true);
				_COLUMN_WP = num++;
			}
			UnderlyingModel.AddColumn(Strings.Address, HorizontalAlignment.Left, MultilineTextTreeTableViewRenderer.MultilineString, TextBoxTreeTableViewEditor.TextBox, false);
			_COLUMN_DIRECT_ADDR = num++;
			if (bShowCommentColumn)
			{
				UnderlyingModel.AddColumn(Strings.Comment, HorizontalAlignment.Left, MultilineTextTreeTableViewRenderer.MultilineString, TextBoxTreeTableViewEditor.TextBox, false);
				_COLUMN_COMMENT = num++;
			}
			EnsureEmptyNode();
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AfterCompile+=(new CompileEventHandler(OnAfterCompile));
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CodeChanged+=(new CompileEventHandler(OnCodeChanged));
			APEnvironment.OptionStorage.OptionChanged+=(new OptionEventHandler(OnOptionChanged));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin+=(new AfterApplicationLoginEventHandler(OnAfterApplicationLogin));
			APEnvironment.ObjectMgr.ObjectRenamed+=(new ObjectRenamedEventHandler(OnObjectRenamed));
			((IOnlineManager2)APEnvironment.OnlineMgr).AfterApplicationDownload+=(new AfterApplicationDownloadEventHandler(OnAfterApplicationDownload));
			if (_bShowWatchpointColumn)
			{
				APEnvironment.BPMgr.ActiveExecutionpointsRestored+=((EventHandler)OnActiveWatchPointsRestored);
			}
			if (MAX_MONITORING_ELEMENTS == -1)
			{
				InitMonitoringLimits();
			}
			_sortColumn = COLUMN_EXPRESSION;
		}

		internal void UnregisterEvents()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			if (!_bEventsUnregistered)
			{
				_bEventsUnregistered = true;
				((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AfterCompile-=(new CompileEventHandler(OnAfterCompile));
				((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CodeChanged-=(new CompileEventHandler(OnCodeChanged));
				APEnvironment.OptionStorage.OptionChanged-=(new OptionEventHandler(OnOptionChanged));
				((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin-=(new AfterApplicationLoginEventHandler(OnAfterApplicationLogin));
				((IOnlineManager2)APEnvironment.OnlineMgr).AfterApplicationDownload-=(new AfterApplicationDownloadEventHandler(OnAfterApplicationDownload));
				if (_bShowWatchpointColumn)
				{
					APEnvironment.BPMgr.ActiveExecutionpointsRestored-=((EventHandler)OnActiveWatchPointsRestored);
				}
			}
		}

		public void SetObject(int nProjectHandle, Guid ObjectGuid)
		{
			_nProjectHandle = nProjectHandle;
			_objectGuid = ObjectGuid;
		}

		internal void MarkModelAsOutdated()
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			if (_bReadOnly)
			{
				return;
			}
			_bIsOutdated = true;
			try
			{
				BeginUpdate();
				WatchListNode[] monitoringNodes = _monitoringNodes;
				foreach (WatchListNode watchListNode in monitoringNodes)
				{
					if (!watchListNode.HasChildren)
					{
						continue;
					}
					watchListNode.MarkAsOutdated();
					for (int num = watchListNode.ChildCount - 1; num >= 0; num--)
					{
						WatchListNode watchListNode2 = watchListNode.GetChild(num) as WatchListNode;
						if (watchListNode2 != null)
						{
							Remove(watchListNode2);
							base.RaiseRemoved(new TreeTableModelEventArgs((ITreeTableNode)(object)watchListNode, num, (ITreeTableNode)(object)watchListNode2));
						}
					}
				}
			}
			catch
			{
			}
			finally
			{
				EndUpdate();
			}
		}

		internal void ReplacePlaceholderNodes(ITreeTableNode firstVisibleModelNode, ITreeTableNode lastVisibleModelNode)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			ITreeTableNode sentinel = UnderlyingModel.Sentinel;
			ITreeTableNode3 val = (ITreeTableNode3)(object)((sentinel is ITreeTableNode3) ? sentinel : null);
			if (val == null)
			{
				return;
			}
			int index = ((ITreeTableNode)val).GetIndex(firstVisibleModelNode);
			int index2 = ((ITreeTableNode)val).GetIndex(lastVisibleModelNode);
			if (0 > index || 0 > index2)
			{
				return;
			}
			TreeTableModelEventArgs2 val2 = null;
			for (int i = index; i <= index2; i++)
			{
				PlaceholderWatchListNode placeholderWatchListNode = ((ITreeTableNode)val).GetChild(i) as PlaceholderWatchListNode;
				if (placeholderWatchListNode != null)
				{
					val.SetChild(i, (ITreeTableNode)(object)placeholderWatchListNode.WatchListNode);
					val2 = new TreeTableModelEventArgs2((ITreeTableNode)(object)val, i, (ITreeTableNode)(object)placeholderWatchListNode.WatchListNode, index, index2);
				}
			}
			if (val2 != null)
			{
				base.RaiseStructureChanged((TreeTableModelEventArgs)(object)val2);
			}
		}

		private IEnumerable<ITreeTableNode> GetRootNodes(WatchListNode[] rootNodes)
		{
			int iCountVisibleRow = 100;
			if (View != null)
			{
				iCountVisibleRow = View.VisibleRowCount;
				if (iCountVisibleRow == 0)
				{
					iCountVisibleRow = Screen.GetWorkingArea((Control)(object)View).Height / 20;
				}
			}
			if (rootNodes.Length < 500)
			{
				iCountVisibleRow = int.MaxValue;
			}
			for (int j = 0; j < Math.Min(rootNodes.Length, iCountVisibleRow); j++)
			{
				yield return (ITreeTableNode)(object)rootNodes[j];
			}
			if (rootNodes.Length > iCountVisibleRow)
			{
				for (int i = iCountVisibleRow; i < rootNodes.Length; i++)
				{
					yield return (ITreeTableNode)(object)new PlaceholderWatchListNode(rootNodes[i]);
				}
			}
		}

		public void Refill(Guid guidApplication)
		{
			try
			{
				if (_stInstancePath.Length <= 0 || (!(_guidApplication == Guid.Empty) && !(_guidApplication == guidApplication)))
				{
					return;
				}
				_guidApplication = guidApplication;
				if (ReadOnly)
				{
					UnderlyingModel.ClearRootNodes();
				}
				else
				{
					while (UnderlyingModel.Sentinel.ChildCount > 1)
					{
						UnderlyingModel.RemoveRootNode(0);
					}
				}
				WatchListNode[] array = CreateChildNodes(_stInstancePath, _nProjectHandle, _objectGuid, null);
				Debug.Assert(array != null);
				foreach (ITreeTableNode rootNode in GetRootNodes(array))
				{
					UnderlyingModel.InsertRootNode(UnderlyingModel.Sentinel.ChildCount - ((!ReadOnly) ? 1 : 0), rootNode);
				}
			}
			catch
			{
			}
		}

		public void BeginUpdate()
		{
			_nUpdateCount++;
		}

		public void EndUpdate()
		{
			_nUpdateCount = Math.Max(0, _nUpdateCount - 1);
			if (_nUpdateCount == 0)
			{
				OnAllExpressionsChanged(EventArgs.Empty);
			}
		}

		public WatchListNode Insert(int nIndex, string stExpression)
		{
			IConverterToIEC converter = GetConverter(GlobalOptionsHelper.DisplayMode);
			Debug.Assert(converter != null);
			WatchListNode watchListNode = new WatchListNode(this, _stInstancePath, stExpression, converter, _bShowCommentColumn, _nProjectHandle, _objectGuid, Guid.Empty);
			UnderlyingModel.InsertRootNode(nIndex, (ITreeTableNode)(object)watchListNode);
			return watchListNode;
		}

		public ExecutionPointWarningNode InsertExecutionPointWarning()
		{
			ExecutionPointWarningNode executionPointWarningNode = new ExecutionPointWarningNode();
			UnderlyingModel.InsertRootNode(base.Sentinel.ChildCount - 1, (ITreeTableNode)(object)executionPointWarningNode);
			return executionPointWarningNode;
		}

		public void RemoveExecutionPointWarning()
		{
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				if (base.Sentinel.GetChild(i) is ExecutionPointWarningNode)
				{
					UnderlyingModel.RemoveRootNode(i);
				}
			}
		}

		public void Remove(int nIndex)
		{
			UnderlyingModel.RemoveRootNode(nIndex);
		}

		public void Remove(WatchListNode node)
		{
			UnderlyingModel.RemoveRootNode((ITreeTableNode)(object)node);
		}

		public void Update(string stValue, int nColumn)
		{
		}

		public string[] GetExpressions()
		{
			LList<string> val = new LList<string>();
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				WatchListNode watchListNode = base.Sentinel.GetChild(i) as WatchListNode;
				if (watchListNode != null && !watchListNode.IsEmpty)
				{
					val.Add(watchListNode.Expression);
				}
			}
			return val.ToArray();
		}

		private void InitMonitoringLimits()
		{
			MAX_MONITORING_ELEMENTS = 100000;
			MAX_MONITORING_ELEMENTS_PER_ARRAY = 20000;
			DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY = 1000;
			if (APEnvironment.Engine.OEMCustomization == null)
			{
				return;
			}
			try
			{
				if (APEnvironment.Engine.OEMCustomization.HasValue("WatchList", "MaxNumberOfElements"))
				{
					int intValue = APEnvironment.Engine.OEMCustomization.GetIntValue("WatchList", "MaxNumberOfElements");
					if (intValue >= 1000)
					{
						MAX_MONITORING_ELEMENTS = intValue;
					}
				}
			}
			catch
			{
			}
			try
			{
				if (APEnvironment.Engine.OEMCustomization.HasValue("WatchList", "MaxNumberOfArrayElements"))
				{
					int intValue2 = APEnvironment.Engine.OEMCustomization.GetIntValue("WatchList", "MaxNumberOfArrayElements");
					if (intValue2 >= 1000)
					{
						MAX_MONITORING_ELEMENTS_PER_ARRAY = intValue2;
					}
				}
			}
			catch
			{
			}
		}

		private void InsertWatchedExpressions(string stVarExpression, IBP4 bp)
		{
			bool flag = false;
			WatchPointItem watchPoint = new WatchPointItem(bp);
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				WatchListNode watchListNode = base.Sentinel.GetChild(i) as WatchListNode;
				if (watchListNode != null && !watchListNode.IsEmpty && watchListNode.Expression == stVarExpression)
				{
					watchListNode.SetWatchPoint(watchPoint);
					flag = true;
				}
			}
			if (!flag)
			{
				Insert(0, stVarExpression).SetWatchPoint(watchPoint);
			}
		}

		internal void MarkChildrenAsOutdated(WatchListNode node)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			if (!node.HasChildren)
			{
				return;
			}
			for (int i = 0; i < node.ChildCount; i++)
			{
				WatchListNode watchListNode = node.GetChild(i) as WatchListNode;
				if (watchListNode != null)
				{
					watchListNode.MarkAsOutdated();
					base.RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)node, i, (ITreeTableNode)(object)watchListNode));
					if (watchListNode.HasChildren)
					{
						MarkChildrenAsOutdated(watchListNode);
					}
				}
			}
		}

		public string Save()
		{
			string text = string.Empty;
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				WatchListNode watchListNode = base.Sentinel.GetChild(i) as WatchListNode;
				if (watchListNode != null && !watchListNode.IsEmpty)
				{
					text = text + watchListNode.Expression + Environment.NewLine;
				}
			}
			return text;
		}

		public void Load(string stInfo)
		{
			try
			{
				BeginUpdate();
				for (int num = base.Sentinel.ChildCount - 1; num >= 0; num--)
				{
					WatchListNode watchListNode = base.Sentinel.GetChild(num) as WatchListNode;
					if (watchListNode != null && !watchListNode.IsEmpty)
					{
						Remove(num);
					}
				}
				if (stInfo == null)
				{
					return;
				}
				StringReader stringReader = new StringReader(stInfo);
				string text = stringReader.ReadLine();
				EnsureEmptyNode();
				while (text != null)
				{
					if (text.Trim().Length > 0)
					{
						Insert(base.Sentinel.ChildCount - ((!ReadOnly) ? 1 : 0), text);
					}
					text = stringReader.ReadLine();
				}
				if (_bLoadActiveWatchPoints)
				{
					RestoreWatchpoints();
				}
			}
			finally
			{
				EndUpdate();
			}
		}

		public void EnableMonitoring(WatchListNode[] nodes)
		{
			if (nodes == null)
			{
				throw new ArgumentNullException("nodes");
			}
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			WatchListNode[] monitoringNodes = _monitoringNodes;
			foreach (WatchListNode key in monitoringNodes)
			{
				hashtable[key] = null;
			}
			monitoringNodes = nodes;
			foreach (WatchListNode key2 in monitoringNodes)
			{
				hashtable2[key2] = null;
			}
			foreach (DictionaryEntry item in hashtable)
			{
				WatchListNode watchListNode = (WatchListNode)item.Key;
				if (!hashtable2.ContainsKey(watchListNode))
				{
					watchListNode.MonitoringEnabled = false;
				}
			}
			foreach (DictionaryEntry item2 in hashtable2)
			{
				WatchListNode watchListNode2 = (WatchListNode)item2.Key;
				if (!hashtable.ContainsKey(watchListNode2))
				{
					watchListNode2.MonitoringEnabled = true;
				}
			}
			_monitoringNodes = nodes;
		}

		public void RaiseExpressionChanged(TreeTableModelEventArgs e)
		{
			base.RaiseChanged(e);
			OnExpressionChanged(EventArgs.Empty);
			EnsureEmptyNode();
		}

		public void RaiseApplicationPrefixChanged(TreeTableModelEventArgs e)
		{
			base.RaiseChanged(e);
		}

		public void RaiseWatchPointChanged(TreeTableModelEventArgs e)
		{
			base.RaiseChanged(e);
		}

		public void RaiseValueChanged(TreeTableModelEventArgs e)
		{
			base.RaiseChanged(e);
		}

		public void RaisePreparedValueChanged(TreeTableModelEventArgs e)
		{
			base.RaiseChanged(e);
		}

		public override void RaiseInserted(TreeTableModelEventArgs e)
		{
			base.RaiseInserted(e);
			OnExpressionInserted(EventArgs.Empty);
		}

		public override void RaiseRemoved(TreeTableModelEventArgs e)
		{
			base.RaiseRemoved(e);
			OnExpressionRemoved(EventArgs.Empty);
			EnsureEmptyNode();
		}

		public WatchListNode[] CreateChildNodes(string stExpression, int nProjectHandle, Guid guidObject, WatchListNode parentNode)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Invalid comparison between Unknown and I4
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Invalid comparison between Unknown and I4
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Invalid comparison between Unknown and I4
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
			if (stExpression == null)
			{
				throw new ArgumentNullException("stExpression");
			}
			try
			{
				Common.SplitExpression(stExpression, out var stResource, out var stApplication, out var stSignature);
				ICompileContext val = null;
				val = ((!(_guidApplication != Guid.Empty)) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContextGuidByName(stResource, stApplication)) : ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(_guidApplication));
				PromptResult val2 = (PromptResult)0;
				if (val == null)
				{
					return new WatchListNode[0];
				}
				IList<string> list = (IList<string>)new LList<string>();
				IList<string> list2 = (IList<string>)new LList<string>();
				string text = string.Empty;
				_ = string.Empty;
				if (stSignature.Contains("."))
				{
					int num = stSignature.IndexOf('.');
					text = stSignature.Substring(num + 1);
					stSignature.Substring(0, num);
					stSignature = stSignature.Substring(0, num);
				}
				ISignature val3 = null;
				val3 = ((!(guidObject == Guid.Empty)) ? ((ICompileContextCommon)val).GetSignature(guidObject) : FindSignatureToExpand(stSignature, val));
				if (val3 != null && val3.HasAttribute("unqualified-access"))
				{
					val3 = null;
				}
				bool flag = false;
				int nMaxElementsToAdd = 0;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				IDynamicInterfaceInstanceWatchVarDescription val4 = null;
				IPointerInstanceWatchVarDescription val5 = null;
				IAddressWatchVarDescription val6 = null;
				bool flag5 = false;
				if (val3 != null && (int)val3.POUType == 109 && text == string.Empty)
				{
					bool flag6 = true;
					try
					{
						list.Clear();
						list2.Clear();
						IMetaObjectStub val7 = null;
						if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && APEnvironment.ObjectMgr.ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, val3.ObjectGuid))
						{
							val7 = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, val3.ObjectGuid);
						}
						INetVarProperties netVarProps = null;
						if (val7 != null && typeof(IGVLObject).IsAssignableFrom(val7.ObjectType))
						{
							netVarProps = ((IGVLObject2)APEnvironment.ObjectMgr.GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, val3.ObjectGuid).Object).NetVarProperties;
						}
						else if (val7 != null && typeof(INVLObject).IsAssignableFrom(val7.ObjectType))
						{
							netVarProps = ((INVLObject)APEnvironment.ObjectMgr.GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, val3.ObjectGuid).Object).NetVarProperties;
						}
						flag2 = MaybeAddChildrenForNetVarGVL(stResource, stApplication, val, list, list2, netVarProps);
						if (val7 != null && typeof(IPersistentGVLObject).IsAssignableFrom(val7.ObjectType))
						{
							flag6 = AddChildrenForPersistentVarList(stResource, stApplication, list, list2, val3);
						}
					}
					catch
					{
					}
					if (flag6)
					{
						IVariable[] all = val3.All;
						foreach (IVariable val8 in all)
						{
							if (!VisibilityUtil.IsHiddenVariable(val3, val8, (GUIHidingFlags)5764607523034234881L))
							{
								string stSignature2 = val3.OrgName + "." + val8.OrgName;
								list.Add(CombineExpression(stResource, stApplication, stSignature2));
								list2.Add(_stInstancePath);
							}
						}
					}
					nMaxElementsToAdd = Math.Min(MAX_MONITORING_ELEMENTS, list.Count);
				}
				else
				{
					string[] array = null;
					bool flag7 = default(bool);
					if (!(val is ICompileContext20))
					{
						array = ((!(val is ICompileContext13)) ? val.SubElements(stSignature, text, out flag7) : ((ICompileContext13)((val is ICompileContext13) ? val : null)).SubElements(stSignature, nProjectHandle, guidObject, text, out flag7));
					}
					else
					{
						int num2 = 0;
						int num3 = DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY - 1;
						ICompiledType val9 = null;
						if (parentNode != null)
						{
							val9 = parentNode.GetCompiledType();
							if (parentNode.MonitoringRange != null)
							{
								num2 = parentNode.MonitoringRange.Item1;
								num3 = parentNode.MonitoringRange.Item2;
								int item = parentNode.MonitoringRange.Item3;
								_ = parentNode.MonitoringRange.Item4;
								if (parentNode.MonitoringRange.Item5)
								{
									num2 -= item;
									num3 -= item;
								}
							}
						}
						if (val9 != null)
						{
							string text2 = text;
							if (!string.IsNullOrEmpty(stSignature))
							{
								text2 = ((!(text2 == string.Empty) && !text2.StartsWith(".")) ? (stSignature + "." + text2) : (stSignature + text2));
							}
							if ((int)((IType)val9).Class == 23)
							{
								val9 = val9.DeRefType;
							}
							int num4 = num2;
							int num5 = num3;
							if ((int)((IType)val9).Class == 28)
							{
								num4 = (num5 = -1);
							}
							ILMCompiledApplicationQuery obj2 = APEnvironment.LMServiceProvider.CompileService.QueryCompiledApplicationSet(((ICompileContextCommon)val).ApplicationGuid);
							ILMCompiledApplicationQuery2 val10 = (ILMCompiledApplicationQuery2)(object)((obj2 is ILMCompiledApplicationQuery2) ? obj2 : null);
							if (val10 == null)
							{
								array = ((ICompileContext20)((val is ICompileContext20) ? val : null)).SubElementsWithRange((IType)(object)val9, text2, num4, num5, out flag7);
							}
							else
							{
								IEnumerable<string> enumerable = val10.SubElementsWithRange((IType)(object)val9, text2, num4, num5, GUIHidingFlags.AllCommon, out flag7);
								if (enumerable != null)
								{
									array = enumerable.ToArray();
								}
							}
						}
						else
						{
							array = ((ICompileContext18)((val is ICompileContext18) ? val : null)).SubElementsWithRange(stSignature, nProjectHandle, guidObject, text, num2, num3, out flag7);
						}
					}
					if (array == null && text.Contains("."))
					{
						string[] array2 = text.Split('.');
						array = val.SubElements(array2[0], array2[1], out flag7);
					}
					if (array == null)
					{
						return new WatchListNode[0];
					}
					if (!flag7)
					{
						APEnvironment.MessageService.Error(Strings.ErrorMonitoringLengthExceeded, "Error_MonitoringLengthExceeded", (object[])null);
						return new WatchListNode[0];
					}
					flag = IsMaxMonitoringLengthExceeded(array.Length, out nMaxElementsToAdd);
					if (flag)
					{
						string text3 = string.Format(Strings.PromptMonitoringLengthExceeded, MAX_MONITORING_ELEMENTS.ToString());
						val2 = APEnvironment.MessageService.Prompt(text3, (PromptChoice)1, (PromptResult)0, "Prompt_MonitoringLengthExceeded", (object[])null);
						if ((int)val2 == 0)
						{
							flag = false;
						}
					}
					string text4 = _stInstancePath;
					if (!flag && nMaxElementsToAdd > 0)
					{
						if (parentNode != null && parentNode.IsNetVarGVL)
						{
							string text5 = ((val3 != null) ? val3.OrgName : stSignature);
							text4 = stResource + "." + stApplication + "." + text5;
						}
						else if (parentNode != null && parentNode != null)
						{
							text4 = parentNode.InstancePath;
						}
						string[] array3 = array;
						foreach (string text6 in array3)
						{
							string item2 = CombineExpression(stResource, stApplication, text6);
							if (CanWatch(val3, stResource, stApplication, stSignature, text6, _nProjectHandle, _objectGuid) || (parentNode != null && parentNode.ImplicitInstancePointer))
							{
								list2.Add(text4);
								list.Add(item2);
							}
						}
					}
					if ((int)val2 == 0 && parentNode == null)
					{
						AddImplicitThisPointerIfPossible(val, nProjectHandle, text4, list2, list, ref nMaxElementsToAdd);
					}
				}
				ArrayList arrayList = new ArrayList();
				if (parentNode != null)
				{
					ulong ulAddressInstance = 0uL;
					ISignature signInstance = null;
					bool bAddressChanged = false;
					if (parentNode.IsInterfaceNode())
					{
						ValueData valueData = parentNode.GetValue(parentNode.COLUMN_VALUE) as ValueData;
						flag5 = false;
						string interfaceInstancePath = _interfaceMonitoringHelper.GetInterfaceInstancePath(GetApplicationGuid(stResource, stApplication), valueData?.Value, out flag5);
						bool num6 = flag5;
						if (flag5)
						{
							flag5 = list.Count == 0;
						}
						if (!num6 || flag5)
						{
							if (flag5)
							{
								list2.Insert(0, string.Empty);
								list.Insert(0, string.Empty);
								flag3 = true;
								nMaxElementsToAdd++;
							}
							else if (string.IsNullOrEmpty(interfaceInstancePath))
							{
								if (parentNode.DetermineInterfaceInstanceSignatureIfPossible(out bAddressChanged, out ulAddressInstance, out signInstance) && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)14, (ushort)0))
								{
									Guid guid = WatchListNodeUtils.DetermineApplicationGuidIfNecessary(_guidApplication);
									val4 = APEnvironment.CreateDynamicInterfaceInstanceWatchVarDescription();
									val4.Initialize(guid, stResource, stApplication, ulAddressInstance, bAddressChanged, signInstance, parentNode.Expression);
									list2.Insert(0, string.Empty);
									list.Insert(0, ((IReferencedInstanceWatchVarDescription)val4).FullyQualifiedWatchExpression);
									flag3 = true;
									nMaxElementsToAdd++;
								}
							}
							else
							{
								string item3 = CombineExpression(stResource, stApplication, interfaceInstancePath);
								list2.Insert(0, string.Empty);
								list.Insert(0, item3);
								flag3 = true;
								nMaxElementsToAdd++;
							}
						}
					}
					else if (parentNode.IsExplicitlySpecifiedAddress())
					{
						ulong ulAddress = 0uL;
						ICompiledType type = null;
						if (parentNode.DetermineAddressIfPossible(out ulAddress, out type))
						{
							Guid guid2 = WatchListNodeUtils.DetermineApplicationGuidIfNecessary(_guidApplication);
							val6 = APEnvironment.CreateAddressWatchVarDescription();
							val6.Initialize(guid2, stResource, stApplication, ulAddress, type, parentNode.InstancePath, parentNode.Expression);
							list2.Insert(0, string.Empty);
							list.Insert(0, ((IReferencedInstanceWatchVarDescription)val6).FullyQualifiedWatchExpression);
							flag4 = true;
							nMaxElementsToAdd++;
						}
					}
					else if (parentNode.IsPointerToUserDefType() && parentNode.DeterminePointerInstanceSignatureIfPossible(out bAddressChanged, out ulAddressInstance, out signInstance))
					{
						Guid guid3 = WatchListNodeUtils.DetermineApplicationGuidIfNecessary(_guidApplication);
						val5 = APEnvironment.CreatePointerInstanceWatchVarDescription();
						val5.Initialize(guid3, stResource, stApplication, ulAddressInstance, bAddressChanged, signInstance, parentNode.InstancePath, parentNode.Expression);
						list2.Insert(0, string.Empty);
						list.Insert(0, ((IReferencedInstanceWatchVarDescription)val5).FullyQualifiedWatchExpression);
						flag4 = true;
						nMaxElementsToAdd++;
					}
				}
				nMaxElementsToAdd = Math.Min(nMaxElementsToAdd, list.Count);
				if (list.Count > 0 && (int)val2 == 0)
				{
					for (int j = 0; j < nMaxElementsToAdd; j++)
					{
						WatchListNode watchListNode;
						if (parentNode == null)
						{
							IConverterToIEC converter = GetConverter(GlobalOptionsHelper.DisplayMode);
							string text7 = list[j];
							Guid iNSTANCEPOINTER_GUID = INSTANCEPOINTER_GUID;
							if (text7 == iNSTANCEPOINTER_GUID.ToString())
							{
								watchListNode = new WatchListNode(this, list2[j], list2[j] + ".__INSTANCEPOINTER", converter, _bShowCommentColumn, _nProjectHandle, _objectGuid, _guidApplication);
								watchListNode.ImplicitInstancePointer = true;
							}
							else
							{
								watchListNode = new WatchListNode(this, list2[j], list[j], converter, _bShowCommentColumn, _nProjectHandle, _objectGuid, _guidApplication);
							}
						}
						else if (flag5)
						{
							watchListNode = new HiddenInstanceWatchListNode(parentNode, _bShowCommentColumn, _nProjectHandle, _objectGuid);
						}
						else if (val4 != null)
						{
							Guid guidApplication = WatchListNodeUtils.DetermineApplicationGuidIfNecessary(_guidApplication);
							watchListNode = new DynamicInterfaceInstanceWatchListNode(parentNode, val4, list2[j], list[j], _bShowCommentColumn, _nProjectHandle, _objectGuid, guidApplication);
						}
						else if (val6 != null)
						{
							Guid guidApplication2 = WatchListNodeUtils.DetermineApplicationGuidIfNecessary(_guidApplication);
							watchListNode = new AddressWatchListNode(parentNode, val6, list2[j], list[j], _bShowCommentColumn, _nProjectHandle, _objectGuid, guidApplication2);
						}
						else if (val5 != null)
						{
							Guid guidApplication3 = WatchListNodeUtils.DetermineApplicationGuidIfNecessary(_guidApplication);
							watchListNode = new PointerInstanceWatchListNode(parentNode, val5, list2[j], list[j], _bShowCommentColumn, _nProjectHandle, _objectGuid, guidApplication3);
						}
						else
						{
							watchListNode = new WatchListNode(parentNode, list2[j], list[j], _bShowCommentColumn, _nProjectHandle, _objectGuid, _guidApplication);
						}
						watchListNode.IsInstanceNode = j == 0 && flag3;
						watchListNode.IsNetVarGVL = flag2 || (parentNode?.IsNetVarGVL ?? false);
						arrayList.Add(watchListNode);
						if (flag4)
						{
							break;
						}
					}
				}
				WatchListNode[] array4 = new WatchListNode[arrayList.Count];
				arrayList.CopyTo(array4);
				return array4;
			}
			catch
			{
				return new WatchListNode[0];
			}
		}

		private void AddImplicitThisPointerIfPossible(ICompileContext comcon, int nProjectHandle, string stInstancePath, IList<string> subElementsInstancePath, IList<string> subElements, ref int nMaxLength)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Invalid comparison between I4 and Unknown
			ISignature val = null;
			try
			{
				ICompileUtilities compileUtils = APEnvironment.LanguageModelUtilities.CompileUtils;
				ICompileUtilities obj = ((compileUtils is ICompileUtilities4) ? compileUtils : null);
				val = ((obj != null) ? ((ICompileUtilities4)obj).FindSignature(comcon, nProjectHandle, _objectGuid) : null);
			}
			catch
			{
				val = null;
			}
			if (val == null || (int)val.POUType != 118)
			{
				return;
			}
			bool flag = false;
			if (val != null && -1 != val.ParentSignatureId)
			{
				ISignature signatureById = comcon.GetSignatureById(val.ParentSignatureId);
				if (signatureById != null)
				{
					flag = 88 == (int)signatureById.POUType;
				}
			}
			if (flag)
			{
				subElementsInstancePath.Insert(0, stInstancePath);
				Guid iNSTANCEPOINTER_GUID = INSTANCEPOINTER_GUID;
				subElements.Insert(0, iNSTANCEPOINTER_GUID.ToString());
				nMaxLength++;
			}
		}

		private static ISignature FindSignatureToExpand(string stSignature, ICompileContext comcon)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Invalid comparison between Unknown and I4
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			ISignature result = null;
			IScope obj = comcon.CreateGlobalIScope();
			IScope obj2 = ((obj is IScope5) ? obj : null);
			ILanguageModelBuilder obj3 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateLanguageModelBuilder();
			IExpression obj4 = ((obj3 is ILanguageModelBuilder5) ? obj3 : null).ParseExpression(stSignature);
			IExpression5 val = (IExpression5)(object)((obj4 is IExpression5) ? obj4 : null);
			IVariable[] array = default(IVariable[]);
			ISignature[] array2 = default(ISignature[]);
			IScope val2 = default(IScope);
			((IScope5)obj2).FindDeclaration((IExpression)(object)val, out array, out array2, out val2);
			if (array != null && array.Length != 0)
			{
				IVariable val3 = array[0];
				if ((int)((IType)val3.CompiledType).Class == 28)
				{
					result = comcon.GetSignatureById(((IUserdefType)(IUserdefType2)val3.CompiledType).SignatureId);
				}
			}
			else if (array2 != null && array2.Length != 0)
			{
				result = array2[0];
			}
			return result;
		}

		private bool AddChildrenForPersistentVarList(string stResource, string stApplication, IList<string> subElements, IList<string> subElementsInstancePath, ISignature signature)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			string[] array = new string[signature.All.Length];
			string[] array2 = new string[signature.All.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (signature.All[i].HasAttribute("map_to") || (!VisibilityUtil.IsHiddenVariable(signature, signature.All[i], (GUIHidingFlags)5764607523034234881L) && !signature.All[i].HasAttribute("hide") && !signature.All[i].OrgName.StartsWith("__")))
				{
					int result = -1;
					if (signature.All[i] is IVariable2 && ((IVariable)(IVariable2)signature.All[i]).HasAttribute("order_in_persistent_editor") && !int.TryParse(((IVariable)(IVariable2)signature.All[i]).GetAttributeValue("order_in_persistent_editor"), out result))
					{
						flag = true;
						break;
					}
					if (result < 0 || result >= array.Length || array[result] != null)
					{
						flag = true;
						break;
					}
					string text = null;
					if (signature.All[i].HasAttribute("map_to"))
					{
						text = signature.All[i].GetAttributeValue("map_to");
						array2[result] = string.Empty;
					}
					else
					{
						text = signature.OrgName + "." + signature.All[i].OrgName;
						array2[result] = _stInstancePath;
					}
					array[result] = CombineExpression(stResource, stApplication, text);
				}
			}
			if (!flag)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (!string.IsNullOrEmpty(array[j]))
					{
						subElements.Add(array[j]);
						subElementsInstancePath.Add(array2[j]);
					}
				}
			}
			return flag;
		}

		private static bool MaybeAddChildrenForNetVarGVL(string stResource, string stApplication, ICompileContext comcon, IList<string> subElements, IList<string> subElementsInstancePath, INetVarProperties netVarProps)
		{
			bool flag = netVarProps != null && netVarProps.Enabled;
			if (flag)
			{
				string text = "NetVar_" + stApplication + "_GVL";
				ISignature signature = ((ICompileContextCommon)comcon).GetSignature(text);
				if (signature != null)
				{
					IVariable[] all = signature.All;
					foreach (IVariable val in all)
					{
						if (!val.OrgName.Contains("DataItems") && !val.OrgName.StartsWith("__"))
						{
							string stSignature = text + "." + val.OrgName;
							subElements.Add(CombineExpression(stResource, stApplication, stSignature));
							subElementsInstancePath.Add(stResource + "." + stApplication + "." + text);
						}
					}
				}
			}
			return flag;
		}

		private bool IsMaxMonitoringLengthExceeded(int nElementsToAdd, out int nMaxElementsToAdd)
		{
			nMaxElementsToAdd = nElementsToAdd;
			int num = _monitoringNodes.Length;
			if (_view != null)
			{
				num = _view.VisibleRowCount;
			}
			if (nElementsToAdd < 0 || nElementsToAdd + num < 0)
			{
				((IEngine)APEnvironment.Engine).Frame.MessageService.Information(Strings.MonitoringTooManyExpressions);
				nMaxElementsToAdd = 0;
				return false;
			}
			if (nElementsToAdd + num < MAX_MONITORING_ELEMENTS)
			{
				return false;
			}
			nMaxElementsToAdd = MAX_MONITORING_ELEMENTS - num;
			if (nMaxElementsToAdd < 0)
			{
				nMaxElementsToAdd = 0;
			}
			return true;
		}

		private Guid GetApplicationGuid(string stResource, string stApplication)
		{
			if (_guidApplication != Guid.Empty)
			{
				return _guidApplication;
			}
			return ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContextGuidByName(stResource, stApplication);
		}

		private bool CanWatch(ISignature sign, string stResource, string stApplication, string stSignature, string stSubExpression, int nProjectHandle, Guid objectGuid)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Invalid comparison between Unknown and I4
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Invalid comparison between Unknown and I4
			if (sign != null && (int)sign.POUType == 109)
			{
				string text = CombineExpression(stResource, stApplication, stSignature);
				if (((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVariable(text, true) != null && stSubExpression.StartsWith(stSignature) && stSubExpression.Length > stSignature.Length)
				{
					stSubExpression = stSubExpression.Remove(0, stSignature.Length + 1);
				}
			}
			IVariable val = null;
			string text2 = CombineExpression(stResource, stApplication, stSubExpression);
			Debug.Assert(text2 != null);
			if (nProjectHandle != -1 && objectGuid != Guid.Empty)
			{
				ICompileContext val2 = null;
				try
				{
					val2 = ((!(_guidApplication != Guid.Empty)) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContextGuidByName(stResource, stApplication)) : ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(_guidApplication));
				}
				catch
				{
				}
				if (val2 == null)
				{
					return false;
				}
				ICompileUtilities compileUtils = APEnvironment.LanguageModelUtilities.CompileUtils;
				ISignature val3 = ((ICompileUtilities4)((compileUtils is ICompileUtilities4) ? compileUtils : null)).FindSignature(val2, nProjectHandle, objectGuid);
				if (val3 == null)
				{
					return false;
				}
				IScope val4 = val2.CreateIScope(val3.Id);
				if (text2.StartsWith(_stInstancePath))
				{
					string text3 = text2.Substring(_stInstancePath.Length + 1);
					ILanguageModelBuilder obj2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateLanguageModelBuilder();
					IExpression obj3 = ((obj2 is ILanguageModelBuilder5) ? obj2 : null).ParseExpression(text3);
					IExpression5 val5 = (IExpression5)(object)((obj3 is IExpression5) ? obj3 : null);
					IExpressionTypifier obj4 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateTypifier(((ICompileContextCommon)val2).ApplicationGuid, val3.Id, false, false);
					((IExpressionTypifier2)((obj4 is IExpressionTypifier6) ? obj4 : null)).TypifyExpression((IExpression)(object)val5);
					ISignature signatureEx = val5.GetSignatureEx(val4);
					val = ((IExpression3)val5).GetVariable(val4);
					if (signatureEx != null && (int)signatureEx.POUType == 118 && signatureEx.Name == "__MAIN" && val.HasFlag((VarFlag)65536))
					{
						return false;
					}
					if (VisibilityUtil.IsHiddenVariable(signatureEx, val, (GUIHidingFlags)8070450532247928833L))
					{
						return false;
					}
				}
			}
			if (val == null)
			{
				val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVariable(text2, true);
				if (VisibilityUtil.IsHiddenVariable(sign, val, (GUIHidingFlags)8070450532247928833L))
				{
					return false;
				}
			}
			if (val.HasAttribute(CompileAttributes.ATTRIBUTE_PROPERTY))
			{
				if (val.HasAttribute(CompileAttributes.ATTRIBUTE_MONITORING))
				{
					string attributeValue = val.GetAttributeValue(CompileAttributes.ATTRIBUTE_MONITORING);
					if (attributeValue == CompileAttributes.ATTRIBUTEVALUE_CALL)
					{
						if (val.Type == null)
						{
							return false;
						}
						if ((int)val.Type.Class == 28)
						{
							try
							{
								IVarRef varReference = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(CombineExpression(stResource, stApplication, stSubExpression));
								if (varReference != null && varReference.AddressInfo is IPropertyAddressInfoExtended)
								{
									return true;
								}
							}
							catch
							{
							}
							return false;
						}
						return true;
					}
					_ = attributeValue == CompileAttributes.ATTRIBUTEVALUE_VARIABLE;
					return true;
				}
				return false;
			}
			return true;
		}

		public void Dispose()
		{
			Dispose(bDisposing: true);
		}

		~WatchListModel()
		{
			try
			{
				Dispose(bDisposing: false);
				GC.SuppressFinalize(this);
			}
			finally
			{
				//Finalize();
			}
		}

		private void Dispose(bool bDisposing)
		{
			if (_bDisposed)
			{
				return;
			}
			if (bDisposing)
			{
				EnableMonitoring(new WatchListNode[0]);
				((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AfterCompile-=(new CompileEventHandler(OnAfterCompile));
				((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CodeChanged-=(new CompileEventHandler(OnCodeChanged));
				APEnvironment.OptionStorage.OptionChanged-=(new OptionEventHandler(OnOptionChanged));
				((IOnlineManager2)APEnvironment.OnlineMgr).AfterApplicationDownload-=(new AfterApplicationDownloadEventHandler(OnAfterApplicationDownload));
				for (int i = 0; i < UnderlyingModel.Sentinel.ChildCount; i++)
				{
					(UnderlyingModel.Sentinel.GetChild(i) as WatchListNode)?.Dispose();
				}
			}
			_bDisposed = true;
		}

		protected virtual void OnExpressionInserted(EventArgs e)
		{
			if (_nUpdateCount == 0 && this.ExpressionInserted != null)
			{
				this.ExpressionInserted(this, e);
			}
		}

		protected virtual void OnExpressionChanged(EventArgs e)
		{
			if (_nUpdateCount == 0 && this.ExpressionChanged != null)
			{
				this.ExpressionChanged(this, e);
			}
		}

		protected virtual void OnExpressionRemoved(EventArgs e)
		{
			if (_nUpdateCount == 0 && this.ExpressionRemoved != null)
			{
				this.ExpressionRemoved(this, e);
			}
		}

		protected virtual void OnAllExpressionsChanged(EventArgs e)
		{
			if (_nUpdateCount == 0 && this.AllExpressionsChanged != null)
			{
				this.AllExpressionsChanged(this, e);
			}
		}

		private void EnsureEmptyNode()
		{
			if (ReadOnly)
			{
				return;
			}
			bool flag = true;
			if (base.Sentinel.ChildCount > 0)
			{
				WatchListNode watchListNode = base.Sentinel.GetChild(base.Sentinel.ChildCount - 1) as WatchListNode;
				if (watchListNode != null)
				{
					flag = !watchListNode.IsEmpty;
				}
			}
			if (flag)
			{
				Insert(base.Sentinel.ChildCount, string.Empty);
			}
		}

		private void RefreshValues()
		{
			_bIsOutdated = false;
			Load(Save());
			OnAllExpressionsChanged(EventArgs.Empty);
		}

		private void AdaptGlobalDisplayMode()
		{
			IConverterToIEC converter = GetConverter(GlobalOptionsHelper.DisplayMode);
			Debug.Assert(converter != null);
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				try
				{
					WatchListNode watchListNode = base.Sentinel.GetChild(i) as WatchListNode;
					if (watchListNode != null)
					{
						watchListNode.Converter = converter;
					}
				}
				catch
				{
				}
			}
		}

		public static IConverterToIEC GetConverter(int nDisplayMode)
		{
			if (nDisplayMode == GlobalOptionsHelper.DISPLAYMODE_BINARY)
			{
				return s_binaryConverter;
			}
			if (nDisplayMode == GlobalOptionsHelper.DISPLAYMODE_HEXADECIMAL)
			{
				return s_hexadecimalConverter;
			}
			return s_decimalConverter;
		}

		private static string CombineExpression(string stResource, string stApplication, string stSignature)
		{
			Debug.Assert(stResource != null);
			Debug.Assert(stApplication != null);
			Debug.Assert(stSignature != null);
			return $"{stResource}.{stApplication}.{stSignature}";
		}

		private void OnAfterCompile(object sender, CompileEventArgs e)
		{
			if (!_bDisposed)
			{
				RefreshValues();
			}
		}

		private void OnAfterApplicationDownload(object sender, OnlineEventArgs e)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if ((e is OnlineDownloadEventArgs2 && ((OnlineDownloadEventArgs2)e).DownloadException != null) || _bDisposed)
			{
				return;
			}
			if (!string.IsNullOrEmpty(_stInstancePath) && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				try
				{
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, e.GuidObject);
					if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						while (metaObjectStub.ParentObjectGuid != Guid.Empty)
						{
							metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, metaObjectStub.ParentObjectGuid);
						}
						if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && _stInstancePath.StartsWith(metaObjectStub.Name + "."))
						{
							Refill(e.GuidObject);
						}
					}
				}
				catch
				{
					RefreshValues();
				}
			}
			else
			{
				RefreshValues();
			}
		}

		private void OnCodeChanged(object sender, CompileEventArgs e)
		{
			if (!_bDisposed)
			{
				_interfaceMonitoringHelper.DiscardAllDataAreaAddresses();
				RefreshValues();
			}
		}

		private void OnObjectRenamed(object sender, ObjectRenamedEventArgs e)
		{
			if (!_bDisposed && _stInstancePath == string.Empty && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle == e.ProjectHandle && APEnvironment.ObjectMgr.ExistsObject(e.ProjectHandle, e.ObjectGuid))
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
				if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType) || (metaObjectStub.ParentObjectGuid == Guid.Empty && typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType)))
				{
					RefreshValues();
				}
			}
		}

		private void OnOptionChanged(object sender, OptionEventArgs e)
		{
			if (!_bDisposed && e.OptionKey != null && e.OptionKey.Name == OPTIONKEY_DISPLAYMODE)
			{
				AdaptGlobalDisplayMode();
			}
		}

		private void OnAfterApplicationLogin(object sender, OnlineEventArgs e)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (!_bDisposed)
			{
				try
				{
					IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(e.GuidObject);
					ApplicationState applicationState = application.ApplicationState;
					OperatingState operatingState = application.OperatingState;
					_interfaceMonitoringHelper.ReadAllDataAreaAddresses(e.GuidObject);
					RefreshValues();
				}
				catch
				{
				}
			}
		}

		private void OnActiveWatchPointsRestored(object sender, EventArgs e)
		{
			if (_bLoadActiveWatchPoints)
			{
				try
				{
					BeginUpdate();
					RestoreWatchpoints();
				}
				finally
				{
					EndUpdate();
				}
			}
		}

		private void RestoreWatchpoints()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			if (_bDisposed)
			{
				return;
			}
			IBP[] breakpoints = ((IBPManager)APEnvironment.BPMgr).GetBreakpoints();
			for (int i = 0; i < breakpoints.Length; i++)
			{
				IBP4 val = (IBP4)breakpoints[i];
				if (((IBP2)val).Watchpoint)
				{
					string[] watchedVariables = val.GetWatchedVariables();
					foreach (string stVarExpression in watchedVariables)
					{
						InsertWatchedExpressions(stVarExpression, val);
					}
				}
			}
		}

		internal void Sort(int column, SortOrder order)
		{
			_sortColumn = column;
			_sortOrder = order;
			UnderlyingModel.Sort((ITreeTableNode)null, false, (IComparer)new WatchListComparer(SortColumn, SortOrder, _view));
		}
	}
}
