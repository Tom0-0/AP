using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.BrowserCommands;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;
using _3S.CoDeSys.WatchList;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_iec_objects.htm")]
	public class IECObjectsPage : UserControl, IEditorPage3, IEditorPage2, IEditorPage, IEditorPageAppearance3, IEditorPageAppearance2, IEditorPageAppearance, IVisibleEditor, IRefactoringCommandContext, IEditorBasedFindReplace, IBrowserCommandsTarget
	{
		private readonly IOMappingEditor _editor;

		private TreetableViewWithColumnStorage _instancesTreeTableView;

		private Panel _panel;

		private Panel _panelInstance;

		private ToolStrip _toolStrip;

		private ToolStripButton _btAdd;

		private ToolStripButton _btDelete;

		private ToolStripButton _btGoIoChannel;

		private ToolStripButton _btEdit;

		private bool _selectionPending;

		private long _lPositionToSelect;

		private int _nOffsetToSelect;

		private int _nLengthToSelect;

		private bool _bIsHidden = true;

		private bool _bIsOnline;

		private IWatchListView3 _watchListView;

		public bool HasOnlineMode => true;

		public string PageName
		{
			get
			{
				string arg = "<?>";
				if (_editor?.GetFrame() != null)
				{
					IConnector val = _editor.Connector(bModify: false);
					if (val != null)
					{
						arg = val.VisibleInterfaceName;
					}
				}
				else if (_editor != null)
				{
					IParameterSet deviceParameterSet = _editor.GetDeviceObject(bToModify: false).DeviceParameterSet;
					if (deviceParameterSet != null)
					{
						arg = DeviceParameterEditor.ParameterSet5_EditorName(deviceParameterSet);
					}
				}
				return string.Format(Strings.PageNameConnectorIECObjects, arg).Trim();
			}
		}

		public Icon Icon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceEditor.Resources.IOMapping.ico");

		public Control Control => this;

		public string PageIdentifier => PageIdentifierString;

		public static string PageIdentifierString => "IOMappingEditor";

		public bool IsHidden
		{
			get
			{
				return _bIsHidden;
			}
			set
			{
				_bIsHidden = value;
				if (!_bIsHidden)
				{
					UpdateView();
				}
			}
		}

		public IList<Control> OnlineModeExcludeList => new List<Control> { _panelInstance };

		public bool IsOnline
		{
			set
			{
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				_bIsOnline = value;
				_toolStrip.Enabled = !_bIsOnline;
				if (_bIsOnline)
				{
					if (_watchListView == null)
					{
						_watchListView = APEnvironment.CreateWatchListView();
					}
					((IWatchListView)_watchListView).PersistenceGuid=(Guid.Empty);
					((IWatchListView)_watchListView).ReadOnly=(true);
					((IView)_watchListView).Control.Dock = DockStyle.Fill;
					((IWatchListView)_watchListView).Refill();
					_panelInstance.Controls.Clear();
					_panelInstance.Controls.Add(((IView)_watchListView).Control);
					while (((IWatchListView2)_watchListView).GetExpressions().Length != 0)
					{
						((IWatchListView2)_watchListView).RemoveExpressionAt(0);
					}
					string text = string.Empty;
					IDeviceObject host = _editor.GetHost();
					if (host != null)
					{
						text = ((IObject)host).MetaObject.Name;
						IDriverInfo driverInfo = ((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
						IDriverInfo4 val = (IDriverInfo4)(object)((driverInfo is IDriverInfo4) ? driverInfo : null);
						if (((IDriverInfo2)val).IoApplication != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IObject)host).MetaObject.ProjectHandle, ((IDriverInfo2)val).IoApplication))
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)host).MetaObject.ProjectHandle, ((IDriverInfo2)val).IoApplication);
							text = text + "." + metaObjectStub.Name + ".";
						}
					}
					FbInstanceProvider fbInstanceProvider = _editor?.InstancesModel?.InstanceProvider;
					if (fbInstanceProvider == null)
					{
						return;
					}
					int num = 0;
					foreach (IFbInstance item in fbInstanceProvider)
					{
						string variable = item.Instance.Variable;
						((IWatchListView2)_watchListView).InsertExpression(num++, text + variable);
					}
				}
				else
				{
					_panelInstance.Controls.Clear();
					_panelInstance.Controls.Add((Control)(object)_instancesTreeTableView);
					if (_watchListView != null)
					{
						((IDisposable)_watchListView).Dispose();
						_watchListView = null;
					}
				}
			}
		}

		internal IUndoManager UndoMgr => _editor.EditorUndoMgr;

		private bool CanUndo
		{
			get
			{
				if (UndoMgr != null)
				{
					return UndoMgr.CanUndo;
				}
				return false;
			}
		}

		private bool CanRedo
		{
			get
			{
				if (UndoMgr != null)
				{
					return UndoMgr.CanRedo;
				}
				return false;
			}
		}

		private IDataElement MappedIOChannel
		{
			get
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				LDictionary<TreeTableViewNode, ITreeTableNode> selectedNodes = GetSelectedNodes();
				if (selectedNodes != null && selectedNodes.Count == 1)
				{
					foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode> item in selectedNodes)
					{
						if (!(item.Value is FbInstanceTreeTableNode))
						{
							continue;
						}
						FbInstanceTreeTableNode fbInstanceTreeTableNode = item.Value as FbInstanceTreeTableNode;
						IParameterSet parameterSet = _editor?.GetParameterSetProvider()?.GetParameterSet(bToModify: false);
						if (!(fbInstanceTreeTableNode.FbInstance is IFbInstance5) || parameterSet == null)
						{
							continue;
						}
						string baseName = (fbInstanceTreeTableNode.FbInstance as IFbInstance5).BaseName;
						foreach (DataElementNode node in _editor.ChannelsModel.Nodes)
						{
							IDataElement dataElement = node.DataElement;
							if (dataElement == null) continue;
							if (dataElement.IoMapping.VariableMappings.Count > 0 && dataElement.IoMapping.VariableMappings[0] is IVariableMapping3 && (dataElement.IoMapping.VariableMappings[0] as IVariableMapping3).IoChannelFBInstance == baseName)
							{
								return dataElement;
							}
						}
					}
				}
				return null;
			}
		}

		public IECObjectsPage()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			InitializeComponent();
			_toolStrip.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
		}

		public IECObjectsPage(IOMappingEditor editor)
			: this()
		{
			_editor = editor;
			int a = -1;
			if (_editor is ConnectorIoMappingEditor && _editor is ConnectorIoMappingEditor)
			{
				a = (_editor as ConnectorIoMappingEditor).ConnectorId;
			}
			_instancesTreeTableView.ObjectGuid = _editor.ObjectGuid;
			_instancesTreeTableView.IdentificationGuid = new Guid((uint)a, 48063, 18571, 138, 188, 243, 18, 117, 93, 77, 159);
			if (_editor.InstancesModel != null)
			{
				((TreeTableView)_instancesTreeTableView).Model=((ITreeTableModel)(object)_editor.InstancesModel);
			}
		}

		public void GetSelection(out long lPosition, out short sOffset, out int nLength)
		{
			if (((TreeTableView)_instancesTreeTableView).FocusedNode != null)
			{
				TreeTableViewNode focusedNode = ((TreeTableView)_instancesTreeTableView).FocusedNode;
				FbInstanceTreeTableNode fbInstanceTreeTableNode = ((TreeTableView)_instancesTreeTableView).GetModelNode(((TreeTableView)_instancesTreeTableView).FocusedNode) as FbInstanceTreeTableNode;
				if (focusedNode != null && fbInstanceTreeTableNode != null)
				{
					IFbInstance fbInstance = fbInstanceTreeTableNode.FbInstance;
					lPosition = ((IFbInstance5)((fbInstance is IFbInstance5) ? fbInstance : null)).LanguageModelPositionId;
					sOffset = 0;
					nLength = fbInstanceTreeTableNode.FbInstance.Instance.Variable.Length - sOffset;
				}
			}
			lPosition = -1L;
			sOffset = -1;
			nLength = 0;
		}

		public RefactoringContextType GetRefactoringContext(out Guid objectGuid, out string stVariableName)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (((TreeTableView)_instancesTreeTableView).FocusedNode != null)
			{
				FbInstanceTreeTableNode fbInstanceTreeTableNode = ((TreeTableView)_instancesTreeTableView).GetModelNode(((TreeTableView)_instancesTreeTableView).FocusedNode) as FbInstanceTreeTableNode;
				if (fbInstanceTreeTableNode != null)
				{
					return fbInstanceTreeTableNode.GetRefactoringContext(out objectGuid, out stVariableName);
				}
			}
			stVariableName = string.Empty;
			objectGuid = Guid.Empty;
			return (RefactoringContextType)0;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		private void AdjustColumnWidths(bool bForceAdjust)
		{
			int num;
			using (Graphics graphics = CreateGraphics())
			{
				num = graphics.MeasureString("BCDRTBCDRTBCDRT", ((Control)(object)_instancesTreeTableView).Font).ToSize().Width;
				num += ((TreeTableView)_instancesTreeTableView).Indent * 3;
			}
			if (((TreeTableView)_instancesTreeTableView).Columns.Count > 0 && !_instancesTreeTableView.RestoreTreetableColumnWidth(0))
			{
				((TreeTableView)_instancesTreeTableView).Columns[0].Width = num;
			}
			for (int i = 1; i < ((TreeTableView)_instancesTreeTableView).Columns.Count; i++)
			{
				_instancesTreeTableView.AdjustColumnWidth(i, bConsiderHeaderText: true);
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.IECObjectsPage));
			_panel = new System.Windows.Forms.Panel();
			_panelInstance = new System.Windows.Forms.Panel();
			_instancesTreeTableView = new _3S.CoDeSys.DeviceEditor.TreetableViewWithColumnStorage();
			_toolStrip = new System.Windows.Forms.ToolStrip();
			_btAdd = new System.Windows.Forms.ToolStripButton();
			_btEdit = new System.Windows.Forms.ToolStripButton();
			_btDelete = new System.Windows.Forms.ToolStripButton();
			_btGoIoChannel = new System.Windows.Forms.ToolStripButton();
			_panel.SuspendLayout();
			_panelInstance.SuspendLayout();
			_toolStrip.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_panel, "_panel");
			_panel.Controls.Add(_panelInstance);
			_panel.Controls.Add(_toolStrip);
			_panel.Name = "_panel";
			resources.ApplyResources(_panelInstance, "_panelInstance");
			_panelInstance.Controls.Add((System.Windows.Forms.Control)(object)_instancesTreeTableView);
			_panelInstance.Name = "_panelInstance";
			((TreeTableView)_instancesTreeTableView).AllowColumnReorder=(false);
			((TreeTableView)_instancesTreeTableView).AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_instancesTreeTableView).BackColor = System.Drawing.SystemColors.Window;
			((TreeTableView)_instancesTreeTableView).BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			resources.ApplyResources(_instancesTreeTableView, "_instancesTreeTableView");
			((TreeTableView)_instancesTreeTableView).DoNotShrinkColumnsAutomatically=(false);
			((TreeTableView)_instancesTreeTableView).ForceFocusOnClick=(false);
			((TreeTableView)_instancesTreeTableView).GridLines=(true);
			((TreeTableView)_instancesTreeTableView).HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Nonclickable);
			((TreeTableView)_instancesTreeTableView).HideSelection=(false);
			_instancesTreeTableView.IdentificationGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
			((TreeTableView)_instancesTreeTableView).ImmediateEdit=(false);
			((TreeTableView)_instancesTreeTableView).Indent=(20);
			((TreeTableView)_instancesTreeTableView).KeepColumnWidthsAdjusted=(false);
			((TreeTableView)_instancesTreeTableView).Model=((ITreeTableModel)null);
			((TreeTableView)_instancesTreeTableView).MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_instancesTreeTableView).Name = "_instancesTreeTableView";
			((TreeTableView)_instancesTreeTableView).NoSearchStrings=(false);
			_instancesTreeTableView.ObjectGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
			((TreeTableView)_instancesTreeTableView).OnlyWhenFocused=(false);
			((TreeTableView)_instancesTreeTableView).OpenEditOnDblClk=(true);
			((TreeTableView)_instancesTreeTableView).ReadOnly=(false);
			((TreeTableView)_instancesTreeTableView).Scrollable=(true);
			((TreeTableView)_instancesTreeTableView).ShowLines=(true);
			((TreeTableView)_instancesTreeTableView).ShowPlusMinus=(true);
			((TreeTableView)_instancesTreeTableView).ShowRootLines=(true);
			((TreeTableView)_instancesTreeTableView).SmallImageList=((System.Windows.Forms.ImageList)null);
			((TreeTableView)_instancesTreeTableView).ToggleOnDblClk=(false);
			((TreeTableView)_instancesTreeTableView).SelectionChanged+=(new System.EventHandler(_instancesTreeTableView_SelectionChanged));
			((System.Windows.Forms.Control)(object)_instancesTreeTableView).DoubleClick += new System.EventHandler(_btEdit_Click);
			((System.Windows.Forms.Control)(object)_instancesTreeTableView).MouseUp += new System.Windows.Forms.MouseEventHandler(_instancesTreeTableView_MouseUp);
			_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			_toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { _btAdd, _btEdit, _btDelete, _btGoIoChannel });
			resources.ApplyResources(_toolStrip, "_toolStrip");
			_toolStrip.Name = "_toolStrip";
			_btAdd.Image = _3S.CoDeSys.DeviceEditor.Strings.Add;
			resources.ApplyResources(_btAdd, "_btAdd");
			_btAdd.Name = "_btAdd";
			_btAdd.Click += new System.EventHandler(_btAdd_Click);
			resources.ApplyResources(_btEdit, "_btEdit");
			_btEdit.Image = _3S.CoDeSys.DeviceEditor.Strings.Edit;
			_btEdit.Name = "_btEdit";
			_btEdit.Click += new System.EventHandler(_btEdit_Click);
			resources.ApplyResources(_btDelete, "_btDelete");
			_btDelete.Image = _3S.CoDeSys.DeviceEditor.Strings.EditDelete;
			_btDelete.Name = "_btDelete";
			_btDelete.Click += new System.EventHandler(_btDelete_Click);
			resources.ApplyResources(_btGoIoChannel, "_btGoIoChannel");
			_btGoIoChannel.Image = _3S.CoDeSys.DeviceEditor.Strings.GoToDefinitionSmall;
			_btGoIoChannel.Name = "_btGoIoChannel";
			_btGoIoChannel.Click += new System.EventHandler(_btGoIoChannel_Click);
			resources.ApplyResources(this, "$this");
			base.Controls.Add(_panel);
			base.Name = "IECObjectsPage";
			_panel.ResumeLayout(false);
			_panel.PerformLayout();
			_panelInstance.ResumeLayout(false);
			_toolStrip.ResumeLayout(false);
			_toolStrip.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		private void UpdateView()
		{
			AdjustColumnWidths(bForceAdjust: false);
			if (_editor?.InstancesModel != null)
			{
				_editor.InstancesModel.Refresh();
			}
			if (_selectionPending)
			{
				_selectionPending = false;
				TrySelect(_lPositionToSelect, _nOffsetToSelect, _nLengthToSelect);
			}
		}

		internal bool TrySelect(long lPosition, int nOffset, int nLength)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			if (nOffset == -1)
			{
				return false;
			}
			if ((lPosition & 0x40000000) == 0L)
			{
				FbInstanceTreeTableNode nodeByPosition = _editor.InstancesModel.GetNodeByPosition(lPosition);
				if (nodeByPosition != null)
				{
					if (IsHidden)
					{
						_selectionPending = true;
						_lPositionToSelect = lPosition;
						_nOffsetToSelect = nOffset;
						_nLengthToSelect = nLength;
						return true;
					}
					foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)((TreeTableView)_instancesTreeTableView).SelectedNodes)
					{
						item.Selected=(false);
					}
					TreeTableViewNode viewNode = ((TreeTableView)_instancesTreeTableView).GetViewNode((ITreeTableNode)(object)nodeByPosition);
					((Control)(object)_instancesTreeTableView).Focus();
					int num = 0;
					viewNode.EnsureVisible(num);
					viewNode.Selected=(true);
					viewNode.Focus(num);
					return true;
				}
			}
			return false;
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
			if (commandGuid == IoMappingEditorPage.GUID_EDITDELETE && _btDelete.Enabled)
			{
				return true;
			}
			if (commandGuid == IoMappingEditorPage.GUID_EDITSELECTALL)
			{
				return true;
			}
			if (commandGuid == IoMappingEditorPage.GUID_EDITUNDO)
			{
				return CanUndo;
			}
			if (commandGuid == IoMappingEditorPage.GUID_EDITREDO)
			{
				return CanRedo;
			}
			if (commandGuid == IoMappingEditorPage.GUID_REFACTORING)
			{
				return true;
			}
			if (commandGuid == IoMappingEditorPage.GUID_GOTODEFINITION)
			{
				return false;
			}
			if (commandGuid == IoMappingEditorPage.GUID_CROSSREFERENCES)
			{
				return true;
			}
			return false;
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (commandGuid == IoMappingEditorPage.GUID_EDITDELETE)
			{
				_btDelete_Click(null, null);
			}
			if (commandGuid == IoMappingEditorPage.GUID_EDITREDO)
			{
				Redo();
			}
			if (commandGuid == IoMappingEditorPage.GUID_EDITUNDO)
			{
				Undo();
			}
			if (!(commandGuid == IoMappingEditorPage.GUID_EDITSELECTALL))
			{
				return;
			}
			try
			{
				((TreeTableView)_instancesTreeTableView).BeginUpdate();
				foreach (TreeTableViewNode node in ((TreeTableView)_instancesTreeTableView).Nodes)
				{
					node.Selected=(true);
				}
			}
			finally
			{
				((TreeTableView)_instancesTreeTableView).EndUpdate();
			}
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			return false;
		}

		public bool GetSelectionForBrowserCommands(out string expression, out Guid objectGuid, out IPreCompileContext pcc)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			expression = null;
			objectGuid = Guid.Empty;
			pcc = null;
			if (((TreeTableView)_instancesTreeTableView).FocusedNode != null)
			{
				try
				{
					_=((TreeTableView)_instancesTreeTableView).FocusedNode;
					FbInstanceTreeTableNode fbInstanceTreeTableNode = ((TreeTableView)_instancesTreeTableView).GetModelNode(((TreeTableView)_instancesTreeTableView).FocusedNode) as FbInstanceTreeTableNode;
					if (fbInstanceTreeTableNode != null)
					{
						expression = fbInstanceTreeTableNode.FbInstance.Instance.Variable;
						objectGuid = _editor.ObjectGuid;
						IDeviceObject host = _editor.GetHost();
						if (host != null)
						{
							IDriverInfo4 val = (IDriverInfo4)((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
							if (val != null)
							{
								pcc = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IDriverInfo2)val).IoApplication);
							}
						}
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		public bool GetSelectionForCrossReferenceCommand(out string expression, out Guid objectGuid)
		{
			expression = string.Empty;
			objectGuid = Guid.Empty;
			return false;
		}

		private void Undo()
		{
			if (CanUndo)
			{
				UndoMgr.Undo();
			}
		}

		private void Redo()
		{
			if (CanRedo)
			{
				UndoMgr.Redo();
			}
		}

		internal LDictionary<TreeTableViewNode, ITreeTableNode> GetSelectedNodes()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			LDictionary<TreeTableViewNode, ITreeTableNode> val = new LDictionary<TreeTableViewNode, ITreeTableNode>();
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)((TreeTableView)_instancesTreeTableView).SelectedNodes)
			{
				TreeTableViewNode val2 = item;
				ITreeTableNode modelNode = ((TreeTableView)_instancesTreeTableView).GetModelNode(val2);
				if (modelNode != null)
				{
					val.Add(val2, modelNode);
				}
			}
			return val;
		}

		private void _instancesTreeTableView_SelectionChanged(object sender, EventArgs e)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			LDictionary<TreeTableViewNode, ITreeTableNode> selectedNodes = GetSelectedNodes();
			if (selectedNodes != null && selectedNodes.Count > 0)
			{
				bool enabled = false;
				bool enabled2 = false;
				IDeviceObject host = _editor.GetHost();
				if (host != null)
				{
					IDriverInfo driverInfo = ((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
					IDriverInfo4 val = (IDriverInfo4)(object)((driverInfo is IDriverInfo4) ? driverInfo : null);
					if (((IDriverInfo2)val).IoApplication != Guid.Empty)
					{
						enabled = true;
						enabled2 = true;
						string strA = string.Empty;
						bool flag = false;
						bool flag2 = false;
						foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode> item in selectedNodes)
						{
							if (item.Value is FbInstanceTreeTableNode)
							{
								FbInstanceTreeTableNode fbInstanceTreeTableNode = item.Value as FbInstanceTreeTableNode;
								if (FBIoChannels.FindIoFbs(host.MetaObject.ProjectHandle, val.IoApplication, fbInstanceTreeTableNode.FbInstance.FbName, out var stType, out var bIsOutput) != null)
								{
									if (!flag2)
									{
										strA = stType;
										flag = bIsOutput;
										flag2 = true;
									}
									else if (string.Compare(strA, stType, StringComparison.InvariantCultureIgnoreCase) != 0 || flag != bIsOutput)
									{
										enabled = false;
									}
									continue;
								}
								enabled = false;
								enabled2 = false;
								break;
							}
							enabled = false;
							enabled2 = false;
							break;
						}
					}
				}
				_btGoIoChannel.Enabled = MappedIOChannel != null;
				_btEdit.Enabled = enabled;
				_btDelete.Enabled = enabled2;
			}
			else
			{
				_btGoIoChannel.Enabled = false;
				_btEdit.Enabled = false;
				_btDelete.Enabled = false;
			}
		}

		private void _btAdd_Click(object sender, EventArgs e)
		{
			IDeviceObject host = _editor.GetHost();
			if (host == null)
			{
				return;
			}
			IDeviceObject obj = ((host is IDeviceObject2) ? host : null);
			IDriverInfo obj2 = ((obj != null) ? ((IDeviceObject2)obj).DriverInfo : null);
			IDriverInfo4 val = (IDriverInfo4)(object)((obj2 is IDriverInfo4) ? obj2 : null);
			if (((IDriverInfo2)val).IoApplication != Guid.Empty)
			{
				FBCreateForm fBCreateForm = new FBCreateForm(((IObject)host).MetaObject.ProjectHandle, ((IDriverInfo2)val).IoApplication, string.Empty, bIsOutput: false, bAll: true);
				if (fBCreateForm.ShowDialog() == DialogResult.OK)
				{
					MaintenanceFBAction maintenanceFBAction = new MaintenanceFBAction(_editor, _editor.GetParameterSetProvider(), fBCreateForm.SelectedFB, ((IObject)host).MetaObject.ProjectHandle, ((IDriverInfo2)val).IoApplication, fBCreateForm.IsOutput);
					UndoMgr.AddAction((IUndoableAction)(object)maintenanceFBAction);
					maintenanceFBAction.Redo();
				}
			}
		}

		private void _btDelete_Click(object sender, EventArgs e)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			LDictionary<TreeTableViewNode, ITreeTableNode> selectedNodes = GetSelectedNodes();
			if (selectedNodes == null || selectedNodes.Count <= 0)
			{
				return;
			}
			IDeviceObject host = _editor.GetHost();
			if (host == null)
			{
				return;
			}
			IDeviceObject obj = ((host is IDeviceObject2) ? host : null);
			IDriverInfo obj2 = ((obj != null) ? ((IDeviceObject2)obj).DriverInfo : null);
			IDriverInfo4 val = (IDriverInfo4)(object)((obj2 is IDriverInfo4) ? obj2 : null);
			if (!(((IDriverInfo2)val).IoApplication != Guid.Empty))
			{
				return;
			}
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode> item in selectedNodes)
			{
				if (item.Value is FbInstanceTreeTableNode)
				{
					MaintenanceFBChangeAction maintenanceFBChangeAction = new MaintenanceFBChangeAction(item.Value as FbInstanceTreeTableNode, null, host.MetaObject.ProjectHandle, val.IoApplication);
					UndoMgr.AddAction(maintenanceFBChangeAction);
					maintenanceFBChangeAction.Redo();
				}
			}
		}

		private void _btEdit_Click(object sender, EventArgs e)
		{
			LDictionary<TreeTableViewNode, ITreeTableNode> selectedNodes = GetSelectedNodes();
			if (selectedNodes == null || selectedNodes.Count <= 0)
			{
				return;
			}
			IDeviceObject host = _editor.GetHost();
			if (host == null)
			{
				return;
			}
			IDeviceObject obj = ((host is IDeviceObject2) ? host : null);
			IDriverInfo obj2 = ((obj != null) ? ((IDeviceObject2)obj).DriverInfo : null);
			IDriverInfo4 val = (IDriverInfo4)(object)((obj2 is IDriverInfo4) ? obj2 : null);
			if (!(((IDriverInfo2)val).IoApplication != Guid.Empty))
			{
				return;
			}
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode> item in selectedNodes)
			{
				if (item.Value is FbInstanceTreeTableNode)
				{
					MaintenanceFBChangeAction maintenanceFBChangeAction = new MaintenanceFBChangeAction(item.Value as FbInstanceTreeTableNode, null, host.MetaObject.ProjectHandle, val.IoApplication);
					UndoMgr.AddAction(maintenanceFBChangeAction);
					maintenanceFBChangeAction.Redo();
				}
			}
		}

		private void _instancesTreeTableView_MouseUp(object sender, MouseEventArgs e)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			if (e.Button == MouseButtons.Right && ((TreeTableView)_instancesTreeTableView).FocusedNode != null)
			{
				_=((TreeTableView)_instancesTreeTableView).FocusedNode;
				try
				{
					ContextMenuFilterCallback val = new ContextMenuFilterCallback(CanExecuteStandardCommand);
					((IEngine)APEnvironment.Engine).Frame.DisplayContextMenu(Guid.Empty, (Guid[])null, val, (Control)(object)_instancesTreeTableView, e.Location);
				}
				catch
				{
				}
			}
		}

		private void _btGoIoChannel_Click(object sender, EventArgs e)
		{
			IDataElement mappedIOChannel = MappedIOChannel;
			if (mappedIOChannel != null)
			{
				IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(((IObject)_editor.GetDeviceObject(bToModify: false)).MetaObject);
				if (editors != null && editors.Length != 0 && editors[0] is IEditorView)
				{
					IEditor obj = editors[0];
					((IEditorView)((obj is IEditorView) ? obj : null)).Select(((IDataElement2)((mappedIOChannel is IDataElement2) ? mappedIOChannel : null)).LanguageModelPositionId, 0);
				}
			}
		}
	}
}
