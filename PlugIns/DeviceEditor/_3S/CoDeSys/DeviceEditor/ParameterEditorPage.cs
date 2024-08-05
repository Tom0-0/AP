#define DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.LegacyOnlineManager;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_configuration.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceEditor.Editor.chm::/Configuration.htm")]
	public class ParameterEditorPage : UserControl, IEditorPage3, IEditorPage2, IEditorPage, IEditorPageAppearance3, IEditorPageAppearance2, IEditorPageAppearance, IPrintableEx, IHasAssociatedOnlineHelpTopic, IVisibleEditor, IGenericConfigurationPage, IEditorBasedFindReplace
	{
		internal enum CommandGroup : ushort
		{
			ONLINEPARAMETER = 13
		}

		internal enum IOMgrCommands : ushort
		{
			WRITEONLINEPARAMETER = 1,
			READONLINEPARAMETER
		}

		private Button _btnWriteParams;

		private TextBox _descriptionTextBox;

		private TreetableViewWithColumnStorage _parametersTree;

		private IContainer components;

		private bool _bInEnableWatchListRange;

		private IOnlineDevice _onlineDevice;

		private Button _btnReadParams;

		private ParameterEditor _editor;

		private Panel panel1;

		private Panel panel3;

		private Panel _panelButtons;

		private Guid additionalContextMenuGuid = Guid.Empty;

		internal bool _bColumnsAdjusted;

		private bool _bGetParameterInProgress;

		public const uint TAG_ONLINEPARAMETER_LIST = 129u;

		public const uint TAG_ONLINEPARAMETER_COUNT = 2u;

		public const uint TAG_ONLINEPARAMETER_PARAMETER = 3u;

		public const uint TAG_REPLY_ERROR = 4u;

		private bool _bIsHidden = true;

		public static readonly Guid GUID_EDITCUT = new Guid("{586FB001-60CA-4dd1-A2F9-F9319EE13C95}");

		public static readonly Guid GUID_EDITCOPY = new Guid("{E76B8E0B-9247-41e7-93D5-80FE36AF9449}");

		public static readonly Guid GUID_EDITPASTE = new Guid("{73A7678B-2707-44f6-963B-8A4B3C05A331}");

		public static readonly Guid GUID_EDITDELETE = new Guid("{C5AAECF0-F55A-4864-871E-4584D1CCC9AF}");

		public static readonly Guid GUID_EDITUNDO = new Guid("{9ECCAF22-3293-4165-943E-88C2C40B4A58}");

		public static readonly Guid GUID_EDITREDO = new Guid("{871B29A1-9E9F-47f9-A5CE-D56C40976743}");

		public static readonly Guid GUID_EDITSELECTALL = new Guid("{1C36CA5E-E26D-4edc-9AB7-C7D87690C328}");

		private IUndoManager _undoMgr = APEnvironment.CreateUndoMgr();

		public string PageName => _editor.PageName;

		public Icon Icon => null;

		public Control Control => this;

		internal TreeTableView ParametersTree => (TreeTableView)(object)_parametersTree;

		internal ParameterTreeTableModel Model => _editor.Model;

		public IList<IGenericConfigurationNode> Nodes
		{
			get
			{
				LList<IGenericConfigurationNode> val = new LList<IGenericConfigurationNode>();
				if (((TreeTableView)_parametersTree).Model.Sentinel.HasChildren)
				{
					for (int i = 0; i < ((TreeTableView)_parametersTree).Model.Sentinel.ChildCount; i++)
					{
						ITreeTableNode child = ((TreeTableView)_parametersTree).Model.Sentinel.GetChild(i);
						IGenericConfigurationNode val2 = (IGenericConfigurationNode)(object)((child is IGenericConfigurationNode) ? child : null);
						if (val2 != null)
						{
							val.Add(val2);
						}
					}
				}
				return (IList<IGenericConfigurationNode>)val;
			}
		}

		public IList<IGenericConfigurationNode> SelectedNodes
		{
			get
			{
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Expected O, but got Unknown
				LList<IGenericConfigurationNode> val = new LList<IGenericConfigurationNode>();
				TreetableViewWithColumnStorage parametersTree = _parametersTree;
				if (((parametersTree != null) ? ((TreeTableView)parametersTree).SelectedNodes : null) != null)
				{
					foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)((TreeTableView)_parametersTree).SelectedNodes)
					{
						TreeTableViewNode val2 = item;
						ITreeTableNode modelNode = ((TreeTableView)_parametersTree).GetModelNode(val2);
						IGenericConfigurationNode val3 = (IGenericConfigurationNode)(object)((modelNode is IGenericConfigurationNode) ? modelNode : null);
						if (val3 != null)
						{
							val.Add(val3);
						}
					}
					return (IList<IGenericConfigurationNode>)val;
				}
				return (IList<IGenericConfigurationNode>)val;
			}
		}

		public string PageIdentifier => "ParameterEditor";

		public bool HasOnlineMode => true;

		public bool IsHidden
		{
			get
			{
				return _bIsHidden;
			}
			set
			{
				_bIsHidden = value;
				if (_bIsHidden)
				{
					Model.ReleaseMonitoring(bClose: false);
				}
				else
				{
					EnableMonitoringRange();
				}
				if (!_bIsHidden)
				{
					UpdateView();
				}
			}
		}

		internal IUndoManager UndoMgr => _undoMgr;

		private bool CanUndo => _undoMgr.CanUndo;

		private bool CanRedo => _undoMgr.CanRedo;

		public IList<Control> OnlineModeExcludeList => new List<Control>();

		public bool IsOnline
		{
			set
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				if (Model != null)
				{
					Model.OnlineApplication = _editor.OnlineState.OnlineApplication;
				}
			}
		}

		public event EventHandler<ConfigurationNodeEventArgs> ConfigurationNodeChanged
		{
			add
			{
				IParameterTreeTable parameterTreeTable = ((TreeTableView)_parametersTree).Model as IParameterTreeTable;
				if (parameterTreeTable != null)
				{
					parameterTreeTable.ConfigurationNodeChanged = WeakMulticastDelegate.CombineUnique(parameterTreeTable.ConfigurationNodeChanged, (Delegate)value);
				}
			}
			remove
			{
				IParameterTreeTable parameterTreeTable = ((TreeTableView)_parametersTree).Model as IParameterTreeTable;
				if (parameterTreeTable != null)
				{
					parameterTreeTable.ConfigurationNodeChanged = WeakMulticastDelegate.Remove(parameterTreeTable.ConfigurationNodeChanged, (Delegate)value);
				}
			}
		}

		public ParameterEditorPage()
		{
			InitializeComponent();
		}

		internal ParameterEditorPage(ParameterEditor editor)
			: this()
		{
			_editor = editor;
			try
			{
				object customizationValue = DeviceEditorFactory.GetCustomizationValue(DeviceEditorFactory.stAdditionalContextMenue);
				if (customizationValue is Guid)
				{
					additionalContextMenuGuid = (Guid)customizationValue;
				}
			}
			catch
			{
			}
			int a = -1;
			if (_editor is ConnectorParameterEditor)
			{
				a = (_editor as ConnectorParameterEditor).ConnectorId;
			}
			_parametersTree.ObjectGuid = ((IObject)editor.GetDeviceObject(bToModify: false)).MetaObject.ObjectGuid;
			_parametersTree.IdentificationGuid = new Guid((uint)a, 58631, 18728, 189, 232, 27, 248, 118, 145, 99, 195);
			try
			{
				IDeviceObject host = _editor.GetHost(bToModify: false);
				if (host != null)
				{
					_onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)host).MetaObject.ObjectGuid);
				}
			}
			catch
			{
			}
		}

		public void GetSelection(out long lPosition, out short sOffset, out int nLength)
		{
			if (((TreeTableView)_parametersTree).FocusedNode != null)
			{
				DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(((TreeTableView)_parametersTree).FocusedNode) as DataElementNode;
				if (dataElementNode != null)
				{
					IDataElement dataElement = dataElementNode.DataElement;
					IDataElement2 val = (IDataElement2)(object)((dataElement is IDataElement2) ? dataElement : null);
					if (val != null)
					{
						lPosition = val.EditorPositionId | 0x40000000;
						sOffset = 0;
						nLength = ((IDataElement)val).Value.Length;
						return;
					}
				}
			}
			lPosition = -1L;
			sOffset = -1;
			nLength = 0;
		}

		internal bool TrySelect(long lPosition, int nOffset, int nLength)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (nOffset == -1)
			{
				return false;
			}
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)((TreeTableView)_parametersTree).SelectedNodes)
			{
				item.Selected=(false);
			}
			if ((lPosition & 0x40000000) != 0L)
			{
				lPosition &= -1073741825;
				DataElementNode nodeByPosition = Model.GetNodeByPosition(lPosition);
				if (nodeByPosition == null)
				{
					return false;
				}
				TreeTableViewNode viewNode = ((TreeTableView)_parametersTree).GetViewNode((ITreeTableNode)(object)nodeByPosition);
				if (viewNode == null)
				{
					bool isInRestore = _parametersTree.IsInRestore;
					try
					{
						UpdateView();
						_parametersTree.IsInRestore = true;
						((TreeTableView)_parametersTree).ExpandAll();
						viewNode = ((TreeTableView)_parametersTree).GetViewNode((ITreeTableNode)(object)nodeByPosition);
						_parametersTree.RestoreExpandedNodes();
					}
					finally
					{
						_parametersTree.IsInRestore = isInRestore;
					}
				}
				if (viewNode != null)
				{
					((Control)(object)_parametersTree).Focus();
					int indexOfColumn = Model.GetIndexOfColumn(5);
					viewNode.EnsureVisible(indexOfColumn);
					viewNode.Selected=(true);
					viewNode.Focus(indexOfColumn);
				}
				return true;
			}
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			AttachEventHandlers();
			if (((TreeTableView)_parametersTree).Model == null)
			{
				UpdateView();
			}
			if (((TreeTableView)_parametersTree).Model != null)
			{
				((ParameterTreeTableModel)(object)((TreeTableView)_parametersTree).Model).AttachEventHandlers();
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
			DetachEventHandlers();
			if (((TreeTableView)_parametersTree).Model != null)
			{
				((ParameterTreeTableModel)(object)((TreeTableView)_parametersTree).Model).DetachEventHandlers();
			}
			if (_onlineDevice != null && Model != null)
			{
				Model.ReleaseMonitoring(bClose: true);
			}
		}

		protected void AttachEventHandlers()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			if (_onlineDevice != null)
			{
				_onlineDevice.AfterDeviceLogin+=(new AfterDeviceLoginEventHandler(OnAfterDeviceLogin));
				_onlineDevice.AfterDeviceLogout+=(new AfterDeviceLogoutEventHandler(OnAfterDeviceLogout));
				IDeviceObject host = _editor.GetHost(bToModify: false);
				if (_onlineDevice.IsConnected && host != null)
				{
					OnAfterDeviceLogin(this, new OnlineEventArgs(((IObject)host).MetaObject.ObjectGuid));
				}
			}
		}

		protected void DetachEventHandlers()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			if (_onlineDevice != null)
			{
				_onlineDevice.AfterDeviceLogin-=(new AfterDeviceLoginEventHandler(OnAfterDeviceLogin));
				_onlineDevice.AfterDeviceLogout-=(new AfterDeviceLogoutEventHandler(OnAfterDeviceLogout));
			}
		}

		private void AdjustColumnWidths()
		{
			try
			{
				((TreeTableView)_parametersTree).BeginUpdate();
				int num = ((Control)(object)_parametersTree).Height - 2 * SystemInformation.Border3DSize.Height;
				int num2 = ((Control)(object)_parametersTree).Width - 2 * SystemInformation.Border3DSize.Width;
				LList<int> val = new LList<int>();
				val.Add(_editor.Model.GetIndexOfColumn(5));
				val.Add(_editor.Model.GetIndexOfColumn(8));
				int num3 = 0;
				for (int i = 0; i < ((TreeTableView)_parametersTree).Columns.Count; i++)
				{
					if (val.Contains(i))
					{
						_parametersTree.AdjustColumnWidth(i, bConsiderHeaderText: true, bSave: false, bForceAdjust: false);
					}
					else
					{
						_parametersTree.AdjustColumnWidth(i, bConsiderHeaderText: true);
					}
					num3 += ((TreeTableView)_parametersTree).Columns[i].Width;
				}
				if (((TreeTableView)_parametersTree).Nodes.Count > 0 && ((TreeTableView)_parametersTree).Nodes[((TreeTableView)_parametersTree).Nodes.Count - 1].Bounds
					.Y + ((TreeTableView)_parametersTree).Nodes[((TreeTableView)_parametersTree).Nodes.Count - 1].Bounds
					.Height > num)
				{
					num2 -= SystemInformation.VerticalScrollBarWidth;
				}
				if (num3 > num2 && ((Control)(object)_parametersTree).Visible)
				{
					int num4 = 0;
					for (int j = 0; j < ((TreeTableView)_parametersTree).Columns.Count; j++)
					{
						if (!val.Contains(j))
						{
							num4 += ((TreeTableView)_parametersTree).Columns[j].Width;
						}
					}
					int num5 = num2 - num4;
					if (num5 > 0)
					{
						for (int k = 0; k < val.Count; k++)
						{
							int num6 = val[k];
							if (((TreeTableView)_parametersTree).Columns[num6].Width < num5 / val.Count)
							{
								num5 -= ((TreeTableView)_parametersTree).Columns[num6].Width;
								val.Remove(num6);
								_parametersTree.AdjustColumnWidth(num6, bConsiderHeaderText: true);
							}
						}
						for (int l = 0; l < val.Count; l++)
						{
							int nColumnIndex = val[l];
							_parametersTree.SetColumnWidth(nColumnIndex, num5 / val.Count, bForceAdjust: false);
						}
					}
				}
				_bColumnsAdjusted = true;
			}
			finally
			{
				((TreeTableView)_parametersTree).EndUpdate();
			}
		}

		private void InitializeComponent()
		{
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Expected O, but got Unknown
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Expected O, but got Unknown
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Expected O, but got Unknown
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.ParameterEditorPage));
			_descriptionTextBox = new System.Windows.Forms.TextBox();
			_btnWriteParams = new System.Windows.Forms.Button();
			_btnReadParams = new System.Windows.Forms.Button();
			panel1 = new System.Windows.Forms.Panel();
			panel3 = new System.Windows.Forms.Panel();
			_parametersTree = new _3S.CoDeSys.DeviceEditor.TreetableViewWithColumnStorage();
			_panelButtons = new System.Windows.Forms.Panel();
			panel1.SuspendLayout();
			panel3.SuspendLayout();
			_panelButtons.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_descriptionTextBox, "_descriptionTextBox");
			_descriptionTextBox.Name = "_descriptionTextBox";
			_descriptionTextBox.ReadOnly = true;
			resources.ApplyResources(_btnWriteParams, "_btnWriteParams");
			_btnWriteParams.Name = "_btnWriteParams";
			_btnWriteParams.Click += new System.EventHandler(_btnWriteParams_Click);
			resources.ApplyResources(_btnReadParams, "_btnReadParams");
			_btnReadParams.Name = "_btnReadParams";
			_btnReadParams.Click += new System.EventHandler(_btnReadParams_Click);
			panel1.Controls.Add(panel3);
			panel1.Controls.Add(_panelButtons);
			resources.ApplyResources(panel1, "panel1");
			panel1.Name = "panel1";
			panel3.Controls.Add((System.Windows.Forms.Control)(object)_parametersTree);
			panel3.Controls.Add(_descriptionTextBox);
			resources.ApplyResources(panel3, "panel3");
			panel3.Name = "panel3";
			((TreeTableView)_parametersTree).AllowColumnReorder=(false);
			resources.ApplyResources(_parametersTree, "_parametersTree");
			((TreeTableView)_parametersTree).AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_parametersTree).BackColor = System.Drawing.SystemColors.Window;
			((TreeTableView)_parametersTree).BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			((TreeTableView)_parametersTree).DoNotShrinkColumnsAutomatically=(false);
			((TreeTableView)_parametersTree).ForceFocusOnClick=(false);
			((TreeTableView)_parametersTree).GridLines=(true);
			((TreeTableView)_parametersTree).HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Clickable);
			((TreeTableView)_parametersTree).HideSelection=(false);
			_parametersTree.IdentificationGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
			((TreeTableView)_parametersTree).ImmediateEdit=(true);
			((TreeTableView)_parametersTree).Indent=(20);
			((TreeTableView)_parametersTree).KeepColumnWidthsAdjusted=(false);
			((TreeTableView)_parametersTree).Model=((ITreeTableModel)null);
			((TreeTableView)_parametersTree).MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_parametersTree).Name = "_parametersTree";
			((TreeTableView)_parametersTree).NoSearchStrings=(false);
			_parametersTree.ObjectGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
			((TreeTableView)_parametersTree).OnlyWhenFocused=(false);
			((TreeTableView)_parametersTree).OpenEditOnDblClk=(true);
			((TreeTableView)_parametersTree).ReadOnly=(false);
			((TreeTableView)_parametersTree).Scrollable=(true);
			((TreeTableView)_parametersTree).ShowLines=(true);
			((TreeTableView)_parametersTree).ShowPlusMinus=(true);
			((TreeTableView)_parametersTree).ShowRootLines=(true);
			((TreeTableView)_parametersTree).ToggleOnDblClk=(false);
			((TreeTableView)_parametersTree).BeforeEdit+=(new TreeTableViewEditEventHandler(OnParametersTreeBeforeEdit));
			((TreeTableView)_parametersTree).ColumnClick+=(new System.Windows.Forms.ColumnClickEventHandler(_parametersTree_ColumnClick));
			((TreeTableView)_parametersTree).SelectionChanged+=(new System.EventHandler(OnParametersTreeSelectionChanged));
			((TreeTableView)_parametersTree).AfterCollapse+=(new TreeTableViewEventHandler(OnParametersTreeAfterCollapse));
			((TreeTableView)_parametersTree).AfterExpand+=(new TreeTableViewEventHandler(OnParametersTreeAfterExpand));
			((TreeTableView)_parametersTree).Scroll+=(new System.EventHandler(OnParametersTreeScroll));
			((System.Windows.Forms.Control)(object)_parametersTree).SizeChanged += new System.EventHandler(OnParametersTreeSizeChanged);
			((System.Windows.Forms.Control)(object)_parametersTree).MouseUp += new System.Windows.Forms.MouseEventHandler(_parametersTree_MouseUp);
			_panelButtons.Controls.Add(_btnReadParams);
			_panelButtons.Controls.Add(_btnWriteParams);
			resources.ApplyResources(_panelButtons, "_panelButtons");
			_panelButtons.Name = "_panelButtons";
			base.Controls.Add(panel1);
			base.Name = "ParameterEditorPage";
			resources.ApplyResources(this, "$this");
			panel1.ResumeLayout(false);
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			_panelButtons.ResumeLayout(false);
			ResumeLayout(false);
		}

		private void OnParametersTreeBeforeEdit(object sender, TreeTableViewEditEventArgs e)
		{
			if (_bGetParameterInProgress)
			{
				((CancelEventArgs)(object)e).Cancel = true;
				return;
			}
			try
			{
				_bGetParameterInProgress = true;
				int indexOfColumn = Model.GetIndexOfColumn(7);
				if (e.ColumnIndex != indexOfColumn && _editor.GetParameterSetProvider().GetParameterSet(bToModify: true) == null)
				{
					((CancelEventArgs)(object)e).Cancel = true;
				}
			}
			finally
			{
				_bGetParameterInProgress = false;
			}
		}

		private void OnParametersTreeSelectionChanged(object sender, EventArgs e)
		{
			if (((TreeTableView)_parametersTree).FocusedNode != null)
			{
				DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(((TreeTableView)_parametersTree).FocusedNode) as DataElementNode;
				SectionNode sectionNode = ((TreeTableView)_parametersTree).GetModelNode(((TreeTableView)_parametersTree).FocusedNode) as SectionNode;
				if (dataElementNode != null)
				{
					string description = dataElementNode.DataElement.Description;
					description = description.Replace("\r\n", "  ");
					description = description.Replace("\r", "  ");
					description = description.Replace("\n", "  ");
					_descriptionTextBox.Text = description;
					return;
				}
				if (sectionNode != null)
				{
					string description2 = sectionNode.Section.Description;
					description2 = description2.Replace("\r\n", "  ");
					description2 = description2.Replace("\r", "  ");
					description2 = description2.Replace("\n", "  ");
					_descriptionTextBox.Text = description2;
					return;
				}
			}
			_descriptionTextBox.Text = string.Empty;
		}

		internal void EnableMonitoringRange()
		{
			if (_bInEnableWatchListRange || Model == null || Model.IsExcludedFromBuild)
			{
				return;
			}
			try
			{
				_bInEnableWatchListRange = true;
				bool flag = false;
				if (_onlineDevice == null)
				{
					return;
				}
				if (_onlineDevice.IsConnected)
				{
					TreeTableViewNode val = ((TreeTableView)_parametersTree).TopNode;
					LList<DataElementNode> val2 = new LList<DataElementNode>();
					while (val != null && val.Displayed)
					{
						DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(val) as DataElementNode;
						if (dataElementNode != null)
						{
							IDataElement dataElement = dataElementNode.DataElement;
							if (Parameter_GetOnlineParameter((IParameter)(object)((dataElement is IParameter) ? dataElement : null)))
							{
								flag = true;
							}
							val2.Add(dataElementNode);
						}
						val = val.NextVisibleNode;
					}
					DataElementNode[] array = new DataElementNode[val2.Count];
					val2.CopyTo(array);
					Model.EnableMonitoring(array);
					if (flag)
					{
						_btnReadParams.Visible = true;
					}
				}
				else
				{
					Model.EnableMonitoring(null);
				}
			}
			finally
			{
				_bInEnableWatchListRange = false;
			}
		}

		public static ICompiledType ParseType(string stType)
		{
			IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stType, false, false, false, false);
			return ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val).ParseTypeDeclaration();
		}

		public static ushort GetTypeId(string stType, ushort usDefault)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Invalid comparison between Unknown and I4
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if (stType.ToUpperInvariant() == "BIT" || stType.ToUpperInvariant() == "SAFEBIT")
			{
				return 1;
			}
			ICompiledType val = ParseType(stType);
			if ((int)((IType)val).Class == 28)
			{
				Debug.WriteLine("Unknown type '" + stType + "' in GetTypeIds");
				return usDefault;
			}
			return (ushort)((IType)val).Class;
		}

		private void WriteParameterToService(IDataNodeWriter tagParameter, DataElementNode node)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected I4, but got Unknown
			if (node.PreparedOnlineParameter == null)
			{
				return;
			}
			try
			{
				string preparedOnlineParameter = node.PreparedOnlineParameter;
				tagParameter.Write((int)node.Parameter.Id);
				ushort num = (ushort)((IDataElement)node.Parameter).GetBitSize();
				ushort typeId = GetTypeId(((IDataElement)node.Parameter).BaseType, ushort.MaxValue);
				if (typeId == 16)
				{
					num = (ushort)((preparedOnlineParameter.Length + 1) * 8);
				}
				tagParameter.Write(num);
				tagParameter.Write(typeId);
				TypeClass val = (TypeClass)typeId;
				switch ((int)val)
				{
				case 16:
					tagParameter.Write(preparedOnlineParameter, Encoding.ASCII);
					break;
				case 17:
					tagParameter.Write(preparedOnlineParameter, Encoding.Unicode);
					break;
				case 0:
				{
					if (!bool.TryParse(preparedOnlineParameter, out var result9))
					{
						result9 = ((preparedOnlineParameter == "1") ? true : false);
					}
					byte result8 = (byte)(result9 ? 1 : 0);
					tagParameter.Write(result8);
					break;
				}
				case 1:
				case 2:
				case 10:
				{
					byte.TryParse(preparedOnlineParameter, out var result8);
					tagParameter.Write(result8);
					break;
				}
				case 6:
				{
					sbyte.TryParse(preparedOnlineParameter, out var result7);
					tagParameter.Write((short)result7);
					break;
				}
				case 3:
				case 11:
				{
					ushort.TryParse(preparedOnlineParameter, out var result6);
					tagParameter.Write(result6);
					break;
				}
				case 4:
				case 12:
				{
					uint.TryParse(preparedOnlineParameter, out var result5);
					tagParameter.Write(result5);
					break;
				}
				case 8:
				{
					int.TryParse(preparedOnlineParameter, out var result4);
					tagParameter.Write(result4);
					break;
				}
				case 7:
				{
					short.TryParse(preparedOnlineParameter, out var result3);
					tagParameter.Write(result3);
					break;
				}
				case 9:
				{
					long.TryParse(preparedOnlineParameter, out var result2);
					tagParameter.Write(result2);
					break;
				}
				case 5:
				case 13:
				{
					ulong.TryParse(preparedOnlineParameter, out var result);
					tagParameter.Write(result);
					break;
				}
				case 14:
				case 15:
					break;
				}
			}
			finally
			{
				tagParameter.AlignWithFillByte((byte)0);
			}
		}

		private string GetStringFromService(IServiceTagReader str, TypeClass type, long lParameterID, DataElementNode node)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected I4, but got Unknown
			string result = string.Empty;
			if (1 == 0)
			{
				return result;
			}
			try
			{
				switch ((int)type)
				{
				case 16:
					result = str.ReadString(Encoding.ASCII);
					return result;
				case 17:
					result = str.ReadString(Encoding.Unicode);
					return result;
				case 0:
					result = ((str.ReadByte() != 0) ? "TRUE" : "FALSE");
					return result;
				case 1:
				case 2:
				case 10:
					result = str.ReadByte().ToString();
					return result;
				case 6:
					result = ((sbyte)str.ReadByte()).ToString();
					return result;
				case 3:
				case 11:
					result = str.ReadUShort().ToString();
					return result;
				case 4:
				case 12:
					result = str.ReadUInt().ToString();
					return result;
				case 8:
					result = str.ReadInt().ToString();
					return result;
				case 7:
					result = str.ReadShort().ToString();
					return result;
				case 9:
					result = str.ReadLong().ToString();
					return result;
				case 5:
				case 13:
					result = str.ReadULong().ToString();
					return result;
				default:
					return result;
				case 14:
				case 15:
					return result;
				}
			}
			catch
			{
				return result;
			}
		}

		private void ReadParameterToService(IDataNodeWriter tagParameter, DataElementNode node)
		{
			try
			{
				string preparedOnlineParameter = node.PreparedOnlineParameter;
				tagParameter.Write((int)node.Parameter.Id);
				ushort num = (ushort)((IDataElement)node.Parameter).GetBitSize();
				ushort typeId = GetTypeId(((IDataElement)node.Parameter).BaseType, ushort.MaxValue);
				if (typeId == 16)
				{
					num = (ushort)((preparedOnlineParameter.Length + 1) * 8);
				}
				tagParameter.Write(num);
				tagParameter.Write(typeId);
			}
			finally
			{
				tagParameter.AlignWithFillByte((byte)0);
			}
		}

		private void WriteOnlineParameters(IOnlineDevice2 device, LList<DataElementNode> modelNodeList)
		{
			if (!((IOnlineDevice)device).IsConnected)
			{
				return;
			}
			ITaggedServiceWriter val = ((IOnlineDevice)device).CreateService(13L, 1);
			IComplexNodeWriter val2 = ((IComplexNodeWriter)val).AddComplexTag(129);
			IDataNodeWriter val3 = val2.AddDataTag(2, _3S.CoDeSys.Core.Online.ContentAlignment.Align40);
			val3.Write(modelNodeList.Count);
			val3.AlignWithFillByte((byte)0);
			foreach (DataElementNode modelNode in modelNodeList)
			{
				if (modelNode.PreparedOnlineParameter != string.Empty)
				{
					val3 = val2.AddDataTag(3, _3S.CoDeSys.Core.Online.ContentAlignment.Align40);
					WriteParameterToService(val3, modelNode);
					modelNode.PreparedOnlineParameter = string.Empty;
				}
			}
			try
			{
				IServiceTagEnumerator enumerator2 = ((IOnlineDevice)device).ExecuteService((IServiceWriter)(object)val).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						IServiceTagReader current2 = enumerator2.Current;
						if (current2.IsDataTag && current2.TagId == 65407)
						{
							ushort num = current2.ReadUShort();
							if (num == 770 || num == 769)
							{
								((IEngine)APEnvironment.Engine).MessageService.Error(Strings.ErrorWriteCommmandNotSupported);
							}
						}
						if (current2.IsDataTag || (long)current2.TagId != 129)
						{
							continue;
						}
						IServiceTagEnumerator enumerator3 = current2.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								IServiceTagReader current3 = enumerator3.Current;
								if (!current3.IsDataTag)
								{
									continue;
								}
								switch (current3.TagId)
								{
								case 2:
									current3.ReadUShort();
									break;
								case 4:
								{
									long num2 = current3.ReadUInt();
									if (current3.ReadUShort() != 2)
									{
										break;
									}
									foreach (DataElementNode modelNode2 in modelNodeList)
									{
										if (modelNode2.ParameterId == num2)
										{
											modelNode2.OnlineOnlineParameter = Strings.ErrorParameterNotAvailable;
										}
									}
									break;
								}
								}
							}
						}
						finally
						{
							(enumerator3 as IDisposable)?.Dispose();
						}
					}
				}
				finally
				{
					(enumerator2 as IDisposable)?.Dispose();
				}
			}
			catch
			{
			}
		}

		private void ReadOnlineParameters(IOnlineDevice2 device, LList<DataElementNode> modelNodeList)
		{
			if (!((IOnlineDevice)device).IsConnected)
			{
				return;
			}
			ITaggedServiceWriter val = ((IOnlineDevice)device).CreateService(13L, 2);
			IComplexNodeWriter val2 = ((IComplexNodeWriter)val).AddComplexTag(129);
			IDataNodeWriter val3 = val2.AddDataTag(2, _3S.CoDeSys.Core.Online.ContentAlignment.Align40);
			val3.Write(modelNodeList.Count);
			val3.AlignWithFillByte((byte)0);
			foreach (DataElementNode modelNode in modelNodeList)
			{
				val3 = val2.AddDataTag(3, _3S.CoDeSys.Core.Online.ContentAlignment.Align40);
				ReadParameterToService(val3, modelNode);
			}
			IServiceReader val4 = ((IOnlineDevice)device).ExecuteService((IServiceWriter)(object)val);
			try
			{
				IServiceTagEnumerator enumerator2 = val4.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						IServiceTagReader current2 = enumerator2.Current;
						if (current2.IsDataTag && current2.TagId == 65407)
						{
							ushort num = current2.ReadUShort();
							if (num == 770 || num == 769)
							{
								((IEngine)APEnvironment.Engine).MessageService.Error(Strings.ErrorReadCommmandNotSupported);
							}
						}
						if (current2.IsDataTag || (long)current2.TagId != 129)
						{
							continue;
						}
						IServiceTagEnumerator enumerator3 = current2.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								IServiceTagReader current3 = enumerator3.Current;
								if (!current3.IsDataTag)
								{
									continue;
								}
								switch (current3.TagId)
								{
								case 2:
									current3.ReadUShort();
									break;
								case 3:
								{
									string empty = string.Empty;
									long num2 = current3.ReadUInt();
									current3.ReadUShort();
									ushort num3 = current3.ReadUShort();
									foreach (DataElementNode modelNode2 in modelNodeList)
									{
										if (modelNode2.ParameterId == num2)
										{
											empty = (modelNode2.OnlineOnlineParameter = GetStringFromService(current3, (TypeClass)num3, num2, modelNode2));
										}
									}
									break;
								}
								case 4:
								{
									long num2 = current3.ReadUInt();
									if (current3.ReadUShort() != 2)
									{
										break;
									}
									foreach (DataElementNode modelNode3 in modelNodeList)
									{
										if (modelNode3.ParameterId == num2)
										{
											modelNode3.OnlineOnlineParameter = Strings.ErrorParameterNotAvailable;
										}
									}
									break;
								}
								}
							}
						}
						finally
						{
							(enumerator3 as IDisposable)?.Dispose();
						}
					}
				}
				finally
				{
					(enumerator2 as IDisposable)?.Dispose();
				}
			}
			catch
			{
			}
		}

		public void WritePreparedParameters()
		{
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Expected O, but got Unknown
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			Debug.Assert(_onlineDevice != null);
			try
			{
				TreeTableViewNode val = ((TreeTableView)_parametersTree).TopNode;
				LList<DataElementNode> val2 = new LList<DataElementNode>();
				LList<IOnlineVarRef> val3 = new LList<IOnlineVarRef>();
				while (val != null && val.Displayed)
				{
					DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(val) as DataElementNode;
					if (dataElementNode != null && dataElementNode.Parameter is IParameter17 && !string.IsNullOrEmpty((dataElementNode.Parameter as IParameter17).FbInstanceVariable) && dataElementNode.OnlineVarRef != null && dataElementNode.OnlineVarRef.PreparedValue != null)
					{
						val3.Add(dataElementNode.OnlineVarRef);
					}
					if (dataElementNode != null && dataElementNode.DataElement is IParameter && Parameter_GetOnlineParameter(dataElementNode.DataElement as IParameter) && dataElementNode.PreparedOnlineParameter != string.Empty)
					{
						val2.Add(dataElementNode);
					}
					val = val.NextVisibleNode;
				}
				IOnlineDevice2 val4 = (IOnlineDevice2)_onlineDevice;
				if (val2.Count > 0)
				{
					WriteOnlineParameters(val4, val2);
				}
				if (val3.Count > 0)
				{
					IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(_editor.OnlineState.OnlineApplication);
					if (application != null)
					{
						application.WriteVariables(val3.ToArray());
					}
				}
				val4.WriteVariables(val4.PreparedVarRefs);
			}
			catch (Exception ex)
			{
				((IEngine)APEnvironment.Engine).MessageService.Error(ex.ToString());
			}
			ReadParams();
		}

		public void ExpandAll()
		{
			try
			{
				((TreeTableView)_parametersTree).BeginUpdate();
				((TreeTableView)_parametersTree).ExpandAll();
			}
			finally
			{
				((TreeTableView)_parametersTree).EndUpdate();
			}
		}

		public void CollapseAll()
		{
			try
			{
				((TreeTableView)_parametersTree).BeginUpdate();
				((TreeTableView)_parametersTree).CollapseAll();
			}
			finally
			{
				((TreeTableView)_parametersTree).EndUpdate();
			}
		}

		private void ReadParams()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected O, but got Unknown
			Debug.Assert(_onlineDevice != null);
			try
			{
				TreeTableViewNode val = ((TreeTableView)_parametersTree).TopNode;
				LList<DataElementNode> val2 = new LList<DataElementNode>();
				while (val != null && val.Displayed)
				{
					DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(val) as DataElementNode;
					if (dataElementNode != null && dataElementNode.DataElement is IParameter && Parameter_GetOnlineParameter(dataElementNode.DataElement as IParameter))
					{
						val2.Add(dataElementNode);
					}
					val = val.NextVisibleNode;
				}
				IOnlineDevice2 device = (IOnlineDevice2)_onlineDevice;
				if (val2.Count > 0)
				{
					ReadOnlineParameters(device, val2);
				}
			}
			catch (Exception ex)
			{
				((IEngine)APEnvironment.Engine).MessageService.Error(ex.ToString());
			}
		}

		private void OnAfterDeviceLogin(object sender, OnlineEventArgs e)
		{
			if (_editor.LogicalDeviceIsMapped || Model == null || Model.IsExcludedFromBuild)
			{
				return;
			}
			((TreeTableView)_parametersTree).BeginUpdate();
			CheckFocuseNode(bCancel: true);
			try
			{
				if (_bColumnsAdjusted && ((Control)(object)_parametersTree).Visible)
				{
					_parametersTree.SaveTreetableColumnsWidth();
				}
				bool flag = false;
				ILegacyOnlineManager legacyOnlineMgrOrNull = APEnvironment.LegacyOnlineMgrOrNull;
				if (legacyOnlineMgrOrNull != null)
				{
					IDeviceObject host = _editor.GetHost();
					if (((host != null) ? ((IObject)host).MetaObject : null) != null && legacyOnlineMgrOrNull.IsOnline(((IObject)host).MetaObject.ObjectGuid))
					{
						flag = true;
					}
				}
				Model.ShowOnlineValue(bIsMappingEditor: false, !flag, !flag);
				AdjustColumnWidths();
				EnableMonitoringRange();
				bool flag2 = false;
				try
				{
					object customizationValue = DeviceEditorFactory.GetCustomizationValue(DeviceEditorFactory.stHideGenericEditorButtons);
					if (customizationValue is bool)
					{
						flag2 = (bool)customizationValue;
					}
				}
				catch
				{
				}
				if (!flag && !flag2)
				{
					_panelButtons.Visible = true;
					_btnWriteParams.Visible = true;
				}
			}
			catch (Exception value)
			{
				Debug.WriteLine(value);
			}
			finally
			{
				_parametersTree.RestoreExpandedNodes(bExpandIfNoData: false);
				((TreeTableView)_parametersTree).EndUpdate();
			}
		}

		internal void CheckFocuseNode(bool bCancel)
		{
			if (_parametersTree != null && ((TreeTableView)_parametersTree).FocusedNode != null)
			{
				TreeTableViewNode focusedNode = ((TreeTableView)_parametersTree).FocusedNode;
				int num = default(int);
				if (focusedNode != null && focusedNode.IsFocused(out num) && focusedNode.IsEditing(num))
				{
					focusedNode.EndEdit(num, bCancel);
				}
			}
		}

		private void OnAfterDeviceLogout(object sender, OnlineEventArgs e)
		{
			((TreeTableView)_parametersTree).BeginUpdate();
			CheckFocuseNode(bCancel: true);
			try
			{
				_parametersTree.SaveTreetableColumnsWidth();
				Model.ReleaseMonitoring(bClose: false);
				Model.HideOnlineValue();
				AdjustColumnWidths();
				EnableMonitoringRange();
				_panelButtons.Visible = false;
				_btnWriteParams.Visible = false;
				_btnReadParams.Visible = false;
			}
			catch (Exception value)
			{
				Debug.WriteLine(value);
			}
			finally
			{
				_parametersTree.RestoreExpandedNodes(bExpandIfNoData: false);
				((TreeTableView)_parametersTree).EndUpdate();
			}
		}

		private void OnParametersTreeAfterExpand(object sender, TreeTableViewEventArgs e)
		{
			EnableMonitoringRange();
		}

		private void OnParametersTreeAfterCollapse(object sender, TreeTableViewEventArgs e)
		{
			EnableMonitoringRange();
		}

		private void OnParametersTreeScroll(object sender, EventArgs e)
		{
			EnableMonitoringRange();
		}

		private void OnParametersTreeSizeChanged(object sender, EventArgs e)
		{
			EnableMonitoringRange();
		}

		private void _btnWriteParams_Click(object sender, EventArgs e)
		{
			WritePreparedParameters();
		}

		private void _btnReadParams_Click(object sender, EventArgs e)
		{
			ReadParams();
		}

		private bool Parameter_GetOnlineParameter(IParameter parameter)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (parameter is IParameter6)
			{
				return ((IParameter6)parameter).OnlineParameter;
			}
			if (parameter is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)parameter).IsFunctionAvailable("GetOnlineParameter"))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
				return XmlConvert.ToBoolean(((IGenericInterfaceExtensionProvider)parameter).CallFunction("GetOnlineParameter", xmlDocument).DocumentElement.InnerText);
			}
			return false;
		}

		public IPrintPainterEx CreatePrintPainter(long nPosition, int nLength)
		{
			return (IPrintPainterEx)(object)new DeviceParameterPrintPainter(_editor, _editor.ParameterFilter);
		}

		private void UpdateView()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (_editor.Model != null)
				{
					((TreeTableView)_parametersTree).BeginUpdate();
					_editor.Model.OnlineApplication = _editor.OnlineState.OnlineApplication;
					if (((TreeTableView)_parametersTree).Model == null)
					{
						((TreeTableView)_parametersTree).Model=((ITreeTableModel)(object)_editor.Model);
						_parametersTree.RestoreExpandedNodes(bExpandIfNoData: true);
						(((TreeTableView)_parametersTree).Model as ParameterTreeTableModel).View = (TreeTableView)(object)_parametersTree;
					}
					AdjustColumnWidths();
					if (_editor is ConnectorParameterEditor)
					{
						(_editor as ConnectorParameterEditor).UpdateView();
					}
					if (_editor is DeviceParameterEditor)
					{
						(_editor as DeviceParameterEditor).UpdateView();
					}
				}
			}
			catch
			{
			}
			finally
			{
				((TreeTableView)_parametersTree).EndUpdate();
			}
		}

		public void Mark(long nPosition, int nLength, object tag)
		{
		}

		public void UnmarkAll(object tag)
		{
		}

		public void Select(long nPosition, int nLength)
		{
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			if (((TreeTableView)_parametersTree).FocusedNode != null)
			{
				TreeTableViewNode focusedNode = ((TreeTableView)_parametersTree).FocusedNode;
				if (((TreeTableView)_parametersTree).GetModelNode(focusedNode) is DataElementNode)
				{
					ICommandManager commandManager = ((IEngine)APEnvironment.Engine).CommandManager;
					ICommandManager3 val = (ICommandManager3)(object)((commandManager is ICommandManager3) ? commandManager : null);
					if (val != null && ((ICommandManager)val).GetCommand(commandGuid) != null && val.GetCommandCategory(commandGuid) == additionalContextMenuGuid)
					{
						return true;
					}
					int num = default(int);
					if ((focusedNode.IsFocused(out num) && num == Model.GetIndexOfColumn(6)) || num == Model.GetIndexOfColumn(8))
					{
						DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(focusedNode) as DataElementNode;
						if (dataElementNode != null && !dataElementNode.DataElement.HasSubElements && commandGuid == GUID_EDITCOPY)
						{
							return true;
						}
					}
					if (focusedNode.IsFocused(out num) && num == Model.GetIndexOfColumn(7))
					{
						DataElementNode dataElementNode2 = ((TreeTableView)_parametersTree).GetModelNode(focusedNode) as DataElementNode;
						if (dataElementNode2 != null)
						{
							bool result = !dataElementNode2.DataElement.HasSubElements && !dataElementNode2.ReadOnly;
							if (commandGuid == GUID_EDITCOPY || commandGuid == GUID_EDITCUT || commandGuid == GUID_EDITPASTE || commandGuid == GUID_EDITDELETE)
							{
								return result;
							}
						}
					}
					if (focusedNode.IsFocused(out num) && num == Model.GetIndexOfColumn(5))
					{
						DataElementNode dataElementNode3 = ((TreeTableView)_parametersTree).GetModelNode(focusedNode) as DataElementNode;
						if (dataElementNode3 != null)
						{
							if (commandGuid == GUID_EDITCOPY && !dataElementNode3.DataElement.HasSubElements)
							{
								return true;
							}
							bool result2 = !dataElementNode3.DataElement.HasSubElements && !dataElementNode3.ReadOnly;
							if (commandGuid == GUID_EDITCUT || commandGuid == GUID_EDITPASTE || commandGuid == GUID_EDITDELETE)
							{
								return result2;
							}
						}
					}
				}
			}
			if (commandGuid == GUID_EDITREDO)
			{
				return CanRedo;
			}
			if (commandGuid == GUID_EDITUNDO)
			{
				return CanUndo;
			}
			if (commandGuid == GUID_EDITSELECTALL)
			{
				return true;
			}
			return false;
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITCUT)
			{
				Cut();
			}
			else if (commandGuid == GUID_EDITCOPY)
			{
				Copy();
			}
			else if (commandGuid == GUID_EDITPASTE)
			{
				Paste();
			}
			else if (commandGuid == GUID_EDITDELETE)
			{
				Delete();
			}
			else if (commandGuid == GUID_EDITREDO)
			{
				Redo();
			}
			else if (commandGuid == GUID_EDITUNDO)
			{
				Undo();
			}
			else if (commandGuid == GUID_EDITSELECTALL)
			{
				for (TreeTableViewNode val = ((TreeTableView)_parametersTree).TopNode; val != null; val = val.NextVisibleNode)
				{
					val.Selected=(true);
				}
			}
		}

		private void Cut()
		{
			Copy();
			Delete();
		}

		private void Copy()
		{
			if (((TreeTableView)_parametersTree).FocusedNode == null)
			{
				return;
			}
			TreeTableViewNode focusedNode = ((TreeTableView)_parametersTree).FocusedNode;
			int num = default(int);
			if (focusedNode.IsFocused(out num) && (num == Model.GetIndexOfColumn(6) || num == Model.GetIndexOfColumn(7)))
			{
				DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(focusedNode) as DataElementNode;
				if (dataElementNode != null)
				{
					bool bPreparedValue = num == Model.GetIndexOfColumn(7);
					string value = dataElementNode.GetOnlineValue(bPreparedValue).ToString();
					if (!string.IsNullOrEmpty(value))
					{
						Clipboard.SetText(value);
					}
				}
			}
			if (!focusedNode.IsFocused(out num) || (num != Model.GetIndexOfColumn(5) && num != Model.GetIndexOfColumn(8)))
			{
				return;
			}
			DataElementNode dataElementNode2 = ((TreeTableView)_parametersTree).GetModelNode(focusedNode) as DataElementNode;
			if (dataElementNode2 == null)
			{
				return;
			}
			if (num != Model.GetIndexOfColumn(8))
			{
				if (!string.IsNullOrEmpty(dataElementNode2.DataElement.Value))
				{
					Clipboard.SetText(dataElementNode2.DataElement.Value);
				}
			}
			else if (!string.IsNullOrEmpty(dataElementNode2.DataElement.DefaultValue))
			{
				Clipboard.SetText(dataElementNode2.DataElement.DefaultValue);
			}
		}

		private void Paste()
		{
			string text = Clipboard.GetText();
			if (text != string.Empty)
			{
				SetValue(text);
			}
		}

		private void Delete()
		{
			SetValue(string.Empty);
		}

		private void SetValue(string stText)
		{
			if (_editor.GetParameterSetProvider().GetParameterSet(bToModify: true) == null || ((TreeTableView)_parametersTree).FocusedNode == null)
			{
				return;
			}
			TreeTableViewNode focusedNode = ((TreeTableView)_parametersTree).FocusedNode;
			int num = default(int);
			if (focusedNode.IsFocused(out num) && (num == Model.GetIndexOfColumn(5) || num == Model.GetIndexOfColumn(7)))
			{
				DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(focusedNode) as DataElementNode;
				if (dataElementNode != null && !dataElementNode.DataElement.HasSubElements && !dataElementNode.ReadOnly)
				{
					dataElementNode.SetValue(num, stText);
				}
			}
		}

		private void Undo()
		{
			if (CanUndo)
			{
				_undoMgr.Undo();
			}
		}

		private void Redo()
		{
			if (CanRedo)
			{
				_undoMgr.Redo();
			}
		}

		private void _parametersTree_MouseUp(object sender, MouseEventArgs e)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			if (e.Button == MouseButtons.Right && ((TreeTableView)_parametersTree).FocusedNode != null)
			{
				_=((TreeTableView)_parametersTree).FocusedNode;
				try
				{
					ContextMenuFilterCallback val = new ContextMenuFilterCallback(CanExecuteStandardCommand);
					((IEngine)APEnvironment.Engine).Frame.DisplayContextMenu(Guid.Empty, (Guid[])null, val, (Control)(object)_parametersTree, e.Location);
				}
				catch
				{
				}
			}
		}

		public string GetOnlineHelpKeyword()
		{
			return null;
		}

		public string GetOnlineHelpUrl()
		{
			if (((TreeTableView)_parametersTree).FocusedNode != null)
			{
				DataElementNode dataElementNode = ((TreeTableView)_parametersTree).GetModelNode(((TreeTableView)_parametersTree).FocusedNode) as DataElementNode;
				if (dataElementNode != null && dataElementNode.Parameter != null && dataElementNode.Parameter is IParameter15 && !string.IsNullOrEmpty((dataElementNode.Parameter as IParameter15).OnlineHelpUrl))
				{
					IParameter parameter = dataElementNode.Parameter;
					return ((IParameter15)((parameter is IParameter15) ? parameter : null)).OnlineHelpUrl;
				}
			}
			return null;
		}

		private void _parametersTree_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (_editor.Model == null)
			{
				return;
			}
			int num = _editor.Model.MapColumn(e.Column);
			if (num != 2 && num != 10 && num != 9 && num != 4)
			{
				return;
			}
			try
			{
				((TreeTableView)_parametersTree).BeginUpdate();
				if (e.Column == _editor.Model.SortColumn)
				{
					if (_editor.Model.SortOrder == SortOrder.Ascending)
					{
						_editor.Model.Sort(e.Column, SortOrder.Descending);
					}
					else if (_editor.Model.SortOrder == SortOrder.Descending)
					{
						_editor.Model.Sort(e.Column, SortOrder.None);
					}
					else
					{
						_editor.Model.Sort(e.Column, SortOrder.Ascending);
					}
					((TreeTableView)_parametersTree).SetColumnHeaderSortOrderIcon(e.Column, _editor.Model.SortOrder);
				}
				else
				{
					((TreeTableView)_parametersTree).SetColumnHeaderSortOrderIcon(_editor.Model.SortColumn, SortOrder.None);
					_editor.Model.Sort(e.Column, SortOrder.Ascending);
					((TreeTableView)_parametersTree).SetColumnHeaderSortOrderIcon(e.Column, _editor.Model.SortOrder);
				}
			}
			finally
			{
				_parametersTree.RestoreExpandedNodes(bExpandIfNoData: true);
				((TreeTableView)_parametersTree).EndUpdate();
			}
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			short startIndex = default(short);
			long num = default(long);
			PositionHelper.SplitPosition(nPosition, out num, out startIndex);
			if (num == 2147483648u)
			{
				return true;
			}
			if ((num & 0x40000000) != 0L)
			{
				num &= -1073741825;
				try
				{
					TreetableViewWithColumnStorage parametersTree = _parametersTree;
					DataElementNode dataElementNode = (((parametersTree != null) ? ((TreeTableView)parametersTree).Model : null) as ParameterTreeTableModel)?.GetNodeByPosition(num);
					if (dataElementNode != null)
					{
						string text = dataElementNode.DataElement.Value.Remove(startIndex, nLength);
						text = text.Insert(startIndex, stReplacement);
						ValueAction valueAction = new ValueAction(dataElementNode, bIsComment: false, text);
						UndoMgr.AddAction((IUndoableAction)(object)valueAction);
						valueAction.Redo();
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}
	}
}
