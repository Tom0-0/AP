#define DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.LibManEditor;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_dlg_select_function_block.htm")]
	public class FBCreateForm : Form
	{
		private readonly Guid _gdApp;

		private readonly int _nProjectHandle;

		private readonly bool _bIsOutput;

		private FbCreateTreeTableModel _model;

		private readonly string _stType = string.Empty;

		private IEnumerable<ILibManEditorViewFactory> _viewFactories;

		private bool _bHandleDestroyed;

		private bool _bDoRefill;

		private long _delayTicks;

		internal static Guid docviewGuid = new Guid("{6D8A06A8-188F-49ad-BD1C-C29ACDE9F49D}");

		private IContainer components;

		private Button _btOK;

		private Button _btCancel;

		private TreeTableView _treetable;

		private TabControl _tabControl;

		private SplitContainer _splitContainer;

		private ToolStrip _toolStrip;

		private ToolStripLabel toolStripLabel1;

		private ToolStripTextBox _tbFilter;

		private Timer Timer;

		internal bool IsOutput
		{
			get
			{
				FBCreateNode selectedFB = SelectedFB;
				if (selectedFB != null)
				{
					return selectedFB.Variable.GetFlag((VarFlag)4);
				}
				return false;
			}
		}

		internal FBCreateNode SelectedFB
		{
			get
			{
				if (_treetable.SelectedNodes != null && ((TreeTableViewNodeCollection)_treetable.SelectedNodes).Count > 0)
				{
					TreeTableViewNode val = ((TreeTableViewNodeCollection)_treetable.SelectedNodes)[0];
					return _treetable.GetModelNode(val) as FBCreateNode;
				}
				return null;
			}
		}

		internal LibraryNode SelectedLib
		{
			get
			{
				if (_treetable.SelectedNodes != null && ((TreeTableViewNodeCollection)_treetable.SelectedNodes).Count > 0)
				{
					TreeTableViewNode val = ((TreeTableViewNodeCollection)_treetable.SelectedNodes)[0];
					return _treetable.GetModelNode(val) as LibraryNode;
				}
				return null;
			}
		}

		public FBCreateForm(int nProjectHandle, Guid gdApp, string stType, bool bIsOutput, bool bAll = false)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			_nProjectHandle = nProjectHandle;
			_gdApp = gdApp;
			_stType = stType;
			_bIsOutput = bIsOutput;
			InitializeComponent();
			_toolStrip.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
		}

		protected override void OnLoad(EventArgs e)
		{
			if (UserOptionsHelper.IOChannelWindowContainerHeight != 0)
			{
				base.Location = UserOptionsHelper.IOChannelWindowLocation;
				base.Size = UserOptionsHelper.IOChannelWindowSize;
				UIHelper.AssureFormIsVisibleOnScreen((Form)this);
				if (UserOptionsHelper.IOChannelWindowContainerHeight != 0)
				{
					_splitContainer.SplitterDistance = UserOptionsHelper.IOChannelWindowContainerHeight;
				}
			}
			else
			{
				CenterToParent();
			}
			base.OnLoad(e);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_treetable.Model=((ITreeTableModel)(object)(_model = new FbCreateTreeTableModel()));
			CollectFbs();
			AdjustColumnWidth();
			DoControls();
			Timer.Tick += Timer_Tick;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			UserOptionsHelper.IOChannelWindowLocation = base.Location;
			if (base.WindowState == FormWindowState.Normal)
			{
				UserOptionsHelper.IOChannelWindowSize = base.Size;
			}
			else
			{
				UserOptionsHelper.IOChannelWindowSize = base.RestoreBounds.Size;
			}
			UserOptionsHelper.IOChannelWindowContainerHeight = _splitContainer.SplitterDistance;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			_bHandleDestroyed = true;
			Timer.Tick -= Timer_Tick;
			base.OnHandleDestroyed(e);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (_bDoRefill && DateTime.Now.Ticks - _delayTicks > 2500000)
			{
				_bDoRefill = false;
				CollectFbs();
			}
		}

		private void AdjustColumnWidth()
		{
			for (int i = 0; i < _treetable.Columns.Count; i++)
			{
				_treetable.AdjustColumnWidth(i, true);
			}
		}

		private void CollectFbs()
		{
			foreach (KeyValuePair<ISignature, IVariable> ioFb in FBIoChannels.GetIoFbs(_nProjectHandle, _gdApp, _stType, _bIsOutput))
			{
				string stNodeName = string.Empty;
				if (string.IsNullOrEmpty(ioFb.Key.LibraryPath))
				{
					stNodeName = APEnvironment.ObjectMgr.GetMetaObjectStub(_nProjectHandle, _gdApp).Name;
				}
				else
				{
					ILibManItem libItem = GetLibItem(_nProjectHandle, _gdApp, ioFb.Key);
					if (libItem != null && APEnvironment.LibraryLoader.ParseDisplayName(libItem.Name, out var stTitle, out var _, out var _))
					{
						stNodeName = stTitle;
					}
				}
				_model.AddType(ioFb.Key, ioFb.Value, stNodeName);
			}
			try
			{
				_treetable.BeginUpdate();
				_model.Sort(0, SortOrder.Ascending);
				_treetable.ExpandAll();
			}
			finally
			{
				_treetable.EndUpdate();
			}
		}

		private void DoControls()
		{
			_btOK.Enabled = SelectedFB != null;
		}

		private void _treetable_SelectionChanged(object sender, EventArgs e)
		{
			DoControls();
			RefreshViews();
		}

		private void _treetable_DoubleClick(object sender, EventArgs e)
		{
			if (_btOK.Enabled)
			{
				base.DialogResult = DialogResult.OK;
				Close();
			}
		}

		internal static ILibManItem GetLibItem(int nProjectHandle, Guid gdApp, ISignature sign)
		{
			string libraryPath = sign.LibraryPath;
			foreach (ILibManItem toplevelLib in LibH.GetToplevelLibs(nProjectHandle, gdApp))
			{
				string strB = ((toplevelLib is IManagedLibManItem) ? ((IManagedLibManItem)toplevelLib).ManagedLibrary.DisplayName : ((!(toplevelLib is IPlaceholderLibManItem) || ((IPlaceholderLibManItem)toplevelLib)?.EffectiveResolution == null) ? toplevelLib.Name : ((IPlaceholderLibManItem)toplevelLib)?.EffectiveResolution?.DisplayName));
				if (string.Compare(libraryPath, strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return toplevelLib;
				}
			}
			return null;
		}

		internal IMetaObjectStub GetSelectedObject(out ILibManItem libItem)
		{
			libItem = null;
			FBCreateNode selectedFB = SelectedFB;
			if (selectedFB != null)
			{
				_=selectedFB.Signature.LibraryPath;
				string orgName = selectedFB.Signature.OrgName;
				ILibManItem libItem2 = GetLibItem(_nProjectHandle, _gdApp, selectedFB.Signature);
				if (libItem2 != null)
				{
					IProject projectFromLibManItem = LibH.GetProjectFromLibManItem(libItem2);
					Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(projectFromLibManItem.Handle, orgName);
					if (allObjects != null && allObjects.Length != 0)
					{
						libItem = libItem2;
						return ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectFromLibManItem.Handle, allObjects[0]);
					}
				}
				else
				{
					Guid[] allObjects2 = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(_nProjectHandle, orgName);
					if (allObjects2 != null && allObjects2.Length != 0)
					{
						return ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, allObjects2[0]);
					}
				}
			}
			LibraryNode selectedLib = SelectedLib;
			if (selectedLib != null)
			{
				ILibManItem libItem3 = GetLibItem(_nProjectHandle, _gdApp, selectedLib.Signature);
				if (libItem3 != null)
				{
					libItem = libItem3;
				}
			}
			return null;
		}

		internal void RefreshViews()
		{
			if (_bHandleDestroyed)
			{
				return;
			}
			ILibManEditorViewFactory val = ((_tabControl.SelectedTab != null) ? (_tabControl.SelectedTab.Tag as ILibManEditorViewFactory) : null);
			foreach (TabPage tabPage6 in _tabControl.TabPages)
			{
				if (tabPage6.Tag != null && tabPage6.Tag is ILibManEditorViewFactory)
				{
					TypeGuidAttribute val2 = TypeGuidAttribute.FromObject(tabPage6.Tag);
					if (val2 != null && val2.Guid == docviewGuid && tabPage6.Controls != null && tabPage6.Controls.Count > 0)
					{
						tabPage6.Controls.RemoveAt(0);
					}
				}
				tabPage6.Dispose();
			}
			_tabControl.TabPages.Clear();
			if (_viewFactories == null)
			{
				_viewFactories = APEnvironment.CreateLibManEditorViewFactories();
			}
			Debug.Assert(_viewFactories != null);
			try
			{
				ILibManItem libItem = null;
				IMetaObjectStub selectedObject = GetSelectedObject(out libItem);
				if (selectedObject != null)
				{
					if (libItem == null)
					{
						return;
					}
					LList<ILibManEditorViewFactory> val3 = new LList<ILibManEditorViewFactory>();
					TabPage tabPage2 = null;
					TabPage tabPage3 = null;
					foreach (ILibManEditorViewFactory viewFactory in _viewFactories)
					{
						try
						{
							if (viewFactory.AcceptsObject(libItem, selectedObject.ProjectHandle, selectedObject.ObjectGuid))
							{
								val3.Add(viewFactory);
							}
						}
						catch
						{
						}
					}
					foreach (ICustomizedLibManEditorViewFactorySelector customizedLibManEditorViewFactorySelector in APEnvironment.CustomizedLibManEditorViewFactorySelectors)
					{
						customizedLibManEditorViewFactorySelector.EditEditorViewFactoryList((IList<ILibManEditorViewFactory>)val3, libItem, selectedObject.ProjectHandle, selectedObject.ObjectGuid);
					}
					foreach (ILibManEditorViewFactory item in val3)
					{
						try
						{
							Control control = item.CreateView(libItem, selectedObject.ProjectHandle, selectedObject.ObjectGuid);
							if (control != null)
							{
								TabPage tabPage4 = new TabPage();
								tabPage4.Text = item.Name;
								tabPage4.Tag = item;
								_tabControl.TabPages.Add(tabPage4);
								control.Dock = DockStyle.Fill;
								tabPage4.Controls.Add(control);
								if (tabPage2 == null || val == item)
								{
									tabPage2 = tabPage4;
								}
							}
						}
						catch
						{
						}
					}
					if (tabPage3 != null)
					{
						_tabControl.SelectedTab = tabPage3;
					}
					else if (tabPage2 != null)
					{
						_tabControl.SelectedTab = tabPage2;
					}
					return;
				}
				if (libItem == null || libItem.FileReference == null)
				{
					return;
				}
				IManagedLibrary val4 = null;
				if (libItem is IManagedLibManItem)
				{
					val4 = ((IManagedLibManItem)((libItem is IManagedLibManItem) ? libItem : null)).ManagedLibrary;
				}
				else if (libItem is IPlaceholderLibManItem)
				{
					val4 = ((IPlaceholderLibManItem)((libItem is IPlaceholderLibManItem) ? libItem : null)).EffectiveResolution;
				}
				if (val4 == null)
				{
					return;
				}
				ILibManEditorViewFactory val5 = null;
				foreach (ILibManEditorViewFactory viewFactory2 in _viewFactories)
				{
					try
					{
						if (TypeGuidAttribute.FromObject((object)viewFactory2).Guid == docviewGuid)
						{
							val5 = viewFactory2;
						}
					}
					catch
					{
					}
				}
				if (val5 != null)
				{
					Control control2 = val5.CreateView(libItem, -1, Guid.Empty);
					if (control2 != null)
					{
						TabPage tabPage5 = new TabPage();
						tabPage5.Text = val5.Name;
						tabPage5.Tag = val5;
						_tabControl.TabPages.Add(tabPage5);
						control2.Dock = DockStyle.Fill;
						tabPage5.Controls.Add(control2);
						_tabControl.SelectedTab = tabPage5;
					}
				}
			}
			catch
			{
			}
		}

		private void _tbFilter_TextChanged(object sender, EventArgs e)
		{
			_model.SearchText = _tbFilter.Text;
			_delayTicks = DateTime.Now.Ticks;
			_bDoRefill = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.FBCreateForm));
			_btOK = new System.Windows.Forms.Button();
			_btCancel = new System.Windows.Forms.Button();
			_treetable = new TreeTableView();
			_tabControl = new System.Windows.Forms.TabControl();
			_splitContainer = new System.Windows.Forms.SplitContainer();
			_toolStrip = new System.Windows.Forms.ToolStrip();
			toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			_tbFilter = new System.Windows.Forms.ToolStripTextBox();
			Timer = new System.Windows.Forms.Timer(components);
			((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
			_splitContainer.Panel1.SuspendLayout();
			_splitContainer.Panel2.SuspendLayout();
			_splitContainer.SuspendLayout();
			_toolStrip.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_btOK, "_btOK");
			_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			_btOK.Name = "_btOK";
			_btOK.UseVisualStyleBackColor = true;
			resources.ApplyResources(_btCancel, "_btCancel");
			_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_btCancel.Name = "_btCancel";
			_btCancel.UseVisualStyleBackColor = true;
			_treetable.AllowColumnReorder=(false);
			_treetable.AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_treetable).BackColor = System.Drawing.SystemColors.Window;
			_treetable.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			resources.ApplyResources(_treetable, "_treetable");
			_treetable.DoNotShrinkColumnsAutomatically=(false);
			_treetable.ForceFocusOnClick=(false);
			_treetable.GridLines=(false);
			_treetable.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Nonclickable);
			_treetable.ImmediateEdit=(false);
			_treetable.Indent=(20);
			_treetable.KeepColumnWidthsAdjusted=(false);
			_treetable.Model=((ITreeTableModel)null);
			_treetable.MultiSelect=(false);
			((System.Windows.Forms.Control)(object)_treetable).Name = "_treetable";
			_treetable.NoSearchStrings=(false);
			_treetable.OnlyWhenFocused=(false);
			_treetable.OpenEditOnDblClk=(false);
			_treetable.ReadOnly=(false);
			_treetable.Scrollable=(true);
			_treetable.ShowLines=(true);
			_treetable.ShowPlusMinus=(true);
			_treetable.ShowRootLines=(true);
			_treetable.ToggleOnDblClk=(false);
			_treetable.SelectionChanged+=(new System.EventHandler(_treetable_SelectionChanged));
			((System.Windows.Forms.Control)(object)_treetable).DoubleClick += new System.EventHandler(_treetable_DoubleClick);
			resources.ApplyResources(_tabControl, "_tabControl");
			_tabControl.Name = "_tabControl";
			_tabControl.SelectedIndex = 0;
			resources.ApplyResources(_splitContainer, "_splitContainer");
			_splitContainer.Name = "_splitContainer";
			_splitContainer.Panel1.Controls.Add((System.Windows.Forms.Control)(object)_treetable);
			_splitContainer.Panel2.Controls.Add(_tabControl);
			_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			_toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { toolStripLabel1, _tbFilter });
			resources.ApplyResources(_toolStrip, "_toolStrip");
			_toolStrip.Name = "_toolStrip";
			toolStripLabel1.Name = "toolStripLabel1";
			resources.ApplyResources(toolStripLabel1, "toolStripLabel1");
			_tbFilter.Name = "_tbFilter";
			resources.ApplyResources(_tbFilter, "_tbFilter");
			_tbFilter.TextChanged += new System.EventHandler(_tbFilter_TextChanged);
			Timer.Enabled = true;
			base.AcceptButton = _btOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = _btCancel;
			base.Controls.Add(_toolStrip);
			base.Controls.Add(_splitContainer);
			base.Controls.Add(_btCancel);
			base.Controls.Add(_btOK);
			base.Name = "FBCreateForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			_splitContainer.Panel1.ResumeLayout(false);
			_splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
			_splitContainer.ResumeLayout(false);
			_toolStrip.ResumeLayout(false);
			_toolStrip.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
