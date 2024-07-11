using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.InputAssistant;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{7CF147FD-C3B0-4a4d-9C7A-CA5F0D3E52FF}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch_all_forces.htm")]
	[AssociatedOnlineHelpTopic("core.WatchList.View.chm::/Watch.htm")]
	public class ForceListView : UserControl, IWatchListView3, IWatchListView2, IWatchListView, IView, IDisposable
	{
		private bool _bIsInitialized;

		private WatchListView _watchListView = new WatchListView(bShowWatchpointColumn: false, bIsForceListView: true);

		internal const bool UNFORCE_AND_RESTORE = true;

		internal const bool UNFORCE_AND_KEEP = false;

		private const bool ONLINE = true;

		private const bool OFFLINE = false;

		private bool _bExecutionPointsActive;

		private static readonly Guid GUID_UNFORCE_SELECTED_KEEP_VALUES_COMMAND = new Guid("{0AACF5AE-1D29-4140-9922-144527CDB752}");

		private static readonly Guid GUID_UNFORCE_SELECTED_RESTORE_VALUES_COMMAND = new Guid("{2187FF0A-D565-452B-A410-C261BD0109CA}");

		private static readonly Guid GUID_FACTORY = new Guid("{6DAF8F8C-0A99-4f30-B4BA-67D3819A8AD2}");

		private static readonly Guid GUID_EDITCUT = new Guid("{586FB001-60CA-4dd1-A2F9-F9319EE13C95}");

		private static readonly Guid GUID_EDITCOPY = new Guid("{E76B8E0B-9247-41e7-93D5-80FE36AF9449}");

		private static readonly Guid GUID_EDITPASTE = new Guid("{73A7678B-2707-44f6-963B-8A4B3C05A331}");

		private static readonly Guid GUID_EDITDELETE = new Guid("{C5AAECF0-F55A-4864-871E-4584D1CCC9AF}");

		private static readonly Guid GUID_EDITSELECTALL = new Guid("{1C36CA5E-E26D-4edc-9AB7-C7D87690C328}");

		private ToolStrip toolStrip1;

		private ToolStripDropDownButton toolStripUnforceButton;

		private ToolStripMenuItem unforceKeepToolStripMenuItem;

		private ToolStripMenuItem unforceResetToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator2;

		private Panel _panel;

		private IContainer components;

		public Control Control => this;

		public Control[] Panes => _watchListView.Panes;

		public Icon LargeIcon => SmallIcon;

		public string Caption => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceListViewCommand_Name");

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceListViewCommand_Description");

		public DockingPosition DefaultDockingPosition => (DockingPosition)16;

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.WatchList.Resources.ForceListSmall.ico");

		public DockingPosition PossibleDockingPositions => _watchListView.PossibleDockingPositions;

		public bool IsEnabled => _watchListView.IsEnabled;

		public IInputAssistantArgumentsFormatter ActionArgumentsFormatter => APEnvironment.STActionArgumentsFormatter;

		public IInputAssistantArgumentsFormatter FunctionArgumentsFormatter => APEnvironment.STFunctionArgumentsFormatter;

		public IInputAssistantArgumentsFormatter FunctionBlockArgumentsFormatter => APEnvironment.STFunctionBlockArgumentsFormatter;

		public IInputAssistantArgumentsFormatter MethodArgumentsFormatter => APEnvironment.STMethodArgumentsFormatter;

		public IInputAssistantArgumentsFormatter ProgramArgumentsFormatter => APEnvironment.STProgramArgumentsFormatter;

		public string InstancePath
		{
			get
			{
				return _watchListView.InstancePath;
			}
			set
			{
				_watchListView.InstancePath = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return _watchListView.ReadOnly;
			}
			set
			{
				_watchListView.ReadOnly = value;
			}
		}

		public Guid PersistenceGuid
		{
			get
			{
				return _watchListView.PersistenceGuid;
			}
			set
			{
				_watchListView.PersistenceGuid = value;
			}
		}

		public ForceListView()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			_watchListView.SetAppearance(SmallIcon, LargeIcon, base.Name, (DockingPosition)16, (DockingPosition)31, BorderStyle.FixedSingle);
			_watchListView.PersistenceGuid = Guid.Empty;
			_watchListView.InstancePath = string.Empty;
			_watchListView.ReadOnly = true;
			_watchListView.Dock = DockStyle.Fill;
			InitializeComponent();
			toolStrip1.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
			_panel.Controls.Add(_watchListView);
			if (toolStrip1.Items == null || toolStrip1.Items.Count <= 0)
			{
				return;
			}
			ToolStripDropDownItem toolStripDropDownItem = toolStrip1.Items[0] as ToolStripDropDownItem;
			if (toolStripDropDownItem != null && toolStripDropDownItem.DropDownItems != null && toolStripDropDownItem.DropDownItems.Count == 2)
			{
				if (toolStripDropDownItem.DropDownItems[0].Name == "unforceResetToolStripMenuItem")
				{
					toolStripDropDownItem.DropDownItems[0].Text = Strings.UnforceAllSelectedRestoreValuesCommand_Name;
					toolStripDropDownItem.DropDownItems[1].Text = Strings.UnforceAllSelectedKeepValuesCommand_Name;
				}
				else
				{
					toolStripDropDownItem.DropDownItems[0].Text = Strings.UnforceAllSelectedKeepValuesCommand_Name;
					toolStripDropDownItem.DropDownItems[1].Text = Strings.UnforceAllSelectedRestoreValuesCommand_Name;
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (!_bIsInitialized)
			{
				InitAfterLoad();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!DoLateInit())
			{
				InitAfterLoad();
			}
		}

		private void InitAfterLoad()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			_bIsInitialized = true;
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin+=(new AfterApplicationLoginEventHandler(OnAppLogin));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogout+=(new AfterApplicationLogoutEventHandler(OnAppLogout));
			((IOnlineManager)APEnvironment.OnlineMgr).OperatingStateChanged+=(new OperatingStateChangedEventHandler(OnOperatingStateChanged));
			((IOnlineManager5)APEnvironment.OnlineMgr).OnChangingPreparedValues+=(new OnChangingPreparedValuesEventHandler(ChangingPreparedValues));
			IOnlineApplication6[] allApplications = _watchListView.GetAllApplications(bGetOnline: true);
			if (allApplications != null && allApplications.Length != 0)
			{
				IOnlineApplication6[] array = allApplications;
				foreach (IOnlineApplication6 obj in array)
				{
					obj.OnForcingVariables+=(new OnForcingVariablesEventHandler(ForcingVariablesPerformed));
					obj.OnWritingVariables+=(new OnWritingVariablesEventHandler(WritingVariablesPerformed));
				}
				_watchListView.UpdateAllForcedExpressions(Guid.Empty, GUID_FACTORY);
				toolStripUnforceButton.Enabled = true;
				unforceKeepToolStripMenuItem.Enabled = true;
				unforceResetToolStripMenuItem.Enabled = true;
			}
			else
			{
				unforceKeepToolStripMenuItem.Enabled = false;
				unforceResetToolStripMenuItem.Enabled = false;
				_watchListView.DisableTreeTableViewColumns();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
				_watchListView.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.WatchList.ForceListView));
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			toolStripUnforceButton = new System.Windows.Forms.ToolStripDropDownButton();
			unforceKeepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			unforceResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			_panel = new System.Windows.Forms.Panel();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { toolStripUnforceButton, toolStripSeparator2 });
			resources.ApplyResources(toolStrip1, "toolStrip1");
			toolStrip1.Name = "toolStrip1";
			toolStripUnforceButton.AutoToolTip = false;
			toolStripUnforceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			toolStripUnforceButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { unforceKeepToolStripMenuItem, unforceResetToolStripMenuItem });
			resources.ApplyResources(toolStripUnforceButton, "toolStripUnforceButton");
			toolStripUnforceButton.Name = "toolStripUnforceButton";
			toolStripUnforceButton.MouseEnter += new System.EventHandler(toolStripUnforceButton_Enter);
			unforceKeepToolStripMenuItem.Name = "unforceKeepToolStripMenuItem";
			resources.ApplyResources(unforceKeepToolStripMenuItem, "unforceKeepToolStripMenuItem");
			unforceKeepToolStripMenuItem.Click += new System.EventHandler(unforceKeepToolStripMenuItem_Click);
			unforceResetToolStripMenuItem.Name = "unforceResetToolStripMenuItem";
			resources.ApplyResources(unforceResetToolStripMenuItem, "unforceResetToolStripMenuItem");
			unforceResetToolStripMenuItem.Click += new System.EventHandler(unforceRestoreToolStripMenuItem_Click);
			toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
			_panel.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(_panel, "_panel");
			_panel.Name = "_panel";
			base.Controls.Add(_panel);
			base.Controls.Add(toolStrip1);
			resources.ApplyResources(this, "$this");
			base.Name = "ForceListView";
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITCUT)
			{
				return false;
			}
			if (commandGuid == GUID_EDITPASTE)
			{
				return false;
			}
			if (commandGuid == GUID_EDITDELETE)
			{
				return false;
			}
			if (commandGuid == GUID_EDITCOPY)
			{
				return _watchListView.CanExecuteStandardCommand(commandGuid);
			}
			if (commandGuid == GUID_EDITSELECTALL)
			{
				return _watchListView.CanExecuteStandardCommand(commandGuid);
			}
			return false;
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			_watchListView.ExecuteStandardCommand(commandGuid);
		}

		public IInputAssistantCategory[] GetInputAssistantCategories()
		{
			return _watchListView.GetInputAssistantCategories();
		}

		public void InputAssistantCompleted(string stText)
		{
			_watchListView.InputAssistantCompleted(stText);
		}

		public void SetAppearance(Icon smallIcon, Icon largeIcon, string stCaption, DockingPosition defaultDockingPosition, DockingPosition possibleDockingPositions, BorderStyle borderStyle)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			_watchListView.SetAppearance(smallIcon, largeIcon, stCaption, defaultDockingPosition, possibleDockingPositions, borderStyle);
		}

		public void Refill()
		{
			_watchListView.EmptyList();
		}

		public string[] GetExpressions()
		{
			return _watchListView.GetExpressions();
		}

		public void InsertExpression(int nIndex, string stExpression)
		{
			_watchListView.InsertExpression(nIndex, stExpression);
		}

		public void RemoveExpressionAt(int nIndex)
		{
			_watchListView.RemoveExpressionAt(nIndex);
		}

		public void LoadAllForcedExpressions()
		{
			_watchListView.UpdateAllForcedExpressions(Guid.Empty, Guid.Empty);
		}

		public IOnlineApplication6[] GetAllOnlineApplications()
		{
			return _watchListView.GetAllApplications(bGetOnline: true);
		}

		private void toolStripUnforceButton_Enter(object sender, EventArgs e)
		{
			try
			{
				IOnlineApplication6[] allApplications = _watchListView.GetAllApplications(bGetOnline: true);
				if (allApplications != null)
				{
					IOnlineApplication6[] array = allApplications;
					for (int i = 0; i < array.Length; i++)
					{
						IOnlineApplication val = (IOnlineApplication)(object)array[i];
						if (APEnvironment.OnlineMgr.CheckOnlineApplicationFeatureSupport((OnlineFeatureEnum)4, val))
						{
							unforceKeepToolStripMenuItem.Enabled = true;
							unforceResetToolStripMenuItem.Enabled = true;
							return;
						}
					}
					unforceKeepToolStripMenuItem.Enabled = false;
					unforceResetToolStripMenuItem.Enabled = false;
				}
				else
				{
					unforceKeepToolStripMenuItem.Enabled = false;
					unforceResetToolStripMenuItem.Enabled = false;
				}
			}
			catch
			{
				unforceKeepToolStripMenuItem.Enabled = false;
				unforceResetToolStripMenuItem.Enabled = false;
			}
		}

		private void unforceRestoreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			((ICommandManager2)(ICommandManager3)((IEngine)APEnvironment.Engine).CommandManager).ExecuteCommand(GUID_UNFORCE_SELECTED_RESTORE_VALUES_COMMAND, new string[0]);
		}

		private void unforceKeepToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			((ICommandManager2)(ICommandManager3)((IEngine)APEnvironment.Engine).CommandManager).ExecuteCommand(GUID_UNFORCE_SELECTED_KEEP_VALUES_COMMAND, new string[0]);
		}

		internal void UnforceAllSelected(bool bKeepOrRestore)
		{
			_watchListView.UnforceAllSelectedVariables(bKeepOrRestore);
		}

		internal WatchListNode[] GetSelectedNodes()
		{
			return _watchListView.GetSelectedNodes();
		}

		private void ForcingVariablesPerformed(object sender, OnlineEventArgs e)
		{
			_watchListView.UpdateAllForcedExpressions(e.GuidObject, GUID_FACTORY);
			_watchListView.UnloadAllReleasedVariables();
		}

		private void WritingVariablesPerformed(object sender, OnlineEventArgs e)
		{
			_watchListView.UnloadAllReleasedVariables();
		}

		private void OnAppLogin(object sender, OnlineEventArgs e)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(e.GuidObject);
			IOnlineApplication6 val = (IOnlineApplication6)(object)((application is IOnlineApplication6) ? application : null);
			if (val != null)
			{
				val.OnForcingVariables+=(new OnForcingVariablesEventHandler(ForcingVariablesPerformed));
				val.OnWritingVariables+=(new OnWritingVariablesEventHandler(WritingVariablesPerformed));
			}
			_watchListView.UpdateAllForcedExpressions(e.GuidObject, GUID_FACTORY);
		}

		private void OnAppLogout(object sender, OnlineEventArgs e)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(e.GuidObject);
			IOnlineApplication6 val = (IOnlineApplication6)(object)((application is IOnlineApplication6) ? application : null);
			if (val != null)
			{
				val.OnForcingVariables-=(new OnForcingVariablesEventHandler(ForcingVariablesPerformed));
				val.OnWritingVariables-=(new OnWritingVariablesEventHandler(WritingVariablesPerformed));
			}
			_watchListView.UnloadAllVariablesFromOfflineApps();
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin-=(new AfterApplicationLoginEventHandler(OnAppLogin));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogout-=(new AfterApplicationLogoutEventHandler(OnAppLogout));
			((IOnlineManager)APEnvironment.OnlineMgr).OperatingStateChanged-=(new OperatingStateChangedEventHandler(OnOperatingStateChanged));
			((IOnlineManager5)APEnvironment.OnlineMgr).OnChangingPreparedValues-=(new OnChangingPreparedValuesEventHandler(ChangingPreparedValues));
			if (_watchListView != null)
			{
				IOnlineApplication6[] allApplications = _watchListView.GetAllApplications(bGetOnline: true);
				if (allApplications != null && allApplications.Length != 0)
				{
					IOnlineApplication6[] array = allApplications;
					foreach (IOnlineApplication6 obj in array)
					{
						obj.OnForcingVariables-=(new OnForcingVariablesEventHandler(ForcingVariablesPerformed));
						obj.OnWritingVariables-=(new OnWritingVariablesEventHandler(WritingVariablesPerformed));
					}
				}
			}
			base.OnHandleDestroyed(e);
		}

		private void OnOperatingStateChanged(object sender, OnlineEventArgs e)
		{
			IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(e.GuidObject);
			IOnlineApplication6 val = (IOnlineApplication6)(object)((application is IOnlineApplication6) ? application : null);
			if (val != null && ((IOnlineApplication)val).IsLoggedIn)
			{
				if ((val.OperatingState & OperatingState.force_active) != OperatingState.force_active && ((int)((IOnlineApplication)val).ApplicationState == 0 || (int)((IOnlineApplication)val).ApplicationState == 2))
				{
					_watchListView.UpdateAllForcedExpressions(e.GuidObject, GUID_FACTORY);
					_watchListView.UnloadAllReleasedVariables();
				}
				if (((Enum)((IOnlineApplication)val).OperatingState).HasFlag((Enum)(object)(OperatingState)262144) && !_bExecutionPointsActive)
				{
					_watchListView.UpdateAllForcedExpressions(e.GuidObject, GUID_FACTORY);
					_watchListView.UnloadAllReleasedVariables();
					_bExecutionPointsActive = true;
				}
				else if (!((Enum)((IOnlineApplication)val).OperatingState).HasFlag((Enum)(object)(OperatingState)262144) && _bExecutionPointsActive)
				{
					_watchListView.UpdateAllForcedExpressions(e.GuidObject, GUID_FACTORY);
					_watchListView.UnloadAllReleasedVariables();
					_bExecutionPointsActive = false;
				}
			}
		}

		private void ChangingPreparedValues(object sender, IVarRef varRef)
		{
			_watchListView.AddExpressionOnChangingPreparedValue(varRef);
		}

		private bool DoLateInit()
		{
			bool result = false;
			if (APEnvironment.Engine.OEMCustomization.HasValue("ForceList", "LateInit"))
			{
				result = APEnvironment.Engine.OEMCustomization.GetBoolValue("ForceList", "LateInit");
			}
			return result;
		}
	}
}
