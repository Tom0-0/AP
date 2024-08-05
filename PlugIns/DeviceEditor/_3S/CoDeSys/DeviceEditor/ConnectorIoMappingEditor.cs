using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	public class ConnectorIoMappingEditor : IOMappingEditor, IConnectorEditor, IBaseDeviceEditor, IBaseDeviceEditor2, IEditorBasedFindReplace, ILocalizableEditor
	{
		private LDictionary<object, int> _dictOnParameter = new LDictionary<object, int>();

		private int _nConnectorId = -1;

		private IConnectorEditorFrame _frame;

		public int ConnectorId
		{
			get
			{
				return _nConnectorId;
			}
			set
			{
				_nConnectorId = value;
			}
		}

		internal override IHasOnlineMode HasOnlineMode
		{
			get
			{
				IConnectorEditorFrame frame = _frame;
				return (IHasOnlineMode)(object)((frame is IHasOnlineMode) ? frame : null);
			}
		}

		public IConnectorEditorFrame ConnectorEditorFrame
		{
			get
			{
				return _frame;
			}
			set
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Expected O, but got Unknown
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Expected O, but got Unknown
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Expected O, but got Unknown
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Expected O, but got Unknown
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Expected O, but got Unknown
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Expected O, but got Unknown
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_013a: Expected O, but got Unknown
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Expected O, but got Unknown
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Expected O, but got Unknown
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_017d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Expected O, but got Unknown
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Expected O, but got Unknown
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c7: Expected O, but got Unknown
				if (_frame != null)
				{
					_frame.OnlineStateChanged-=((EventHandler)base.OnOnlineStateChanged);
					((IParameterSetEditorFrame)_frame).ParameterAdded-=(new ParameterEventHandler(OnParameterAdded));
					((IParameterSetEditorFrame)_frame).ParameterRemoved-=(new ParameterEventHandler(OnParameterRemoved));
					((IParameterSetEditorFrame)_frame).ParameterChanged-=(new ParameterChangedEventHandler(OnParameterChanged));
					((IParameterSetEditorFrame3)_frame).ParameterSectionChanged-=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
					if (_frame is DeviceEditor)
					{
						((DeviceEditor)(object)_frame).ParameterMoved -= new ParameterMovedEventHandler(OnParameterMoved);
					}
					APEnvironment.DeviceController.ModuleStatusChanged-=(new ModuleStatusEventHandler(base.OnModuleStatusChanged));
					if (((IEditorView)_frame).Editor != null)
					{
						(((IEditorView)_frame).Editor as DeviceEditor).TaskConfigChanged -= OnTaskConfigChanged;
					}
				}
				if (value == null)
				{
					return;
				}
				_frame = value;
				if (_frame != null)
				{
					_frame.OnlineStateChanged+=((EventHandler)base.OnOnlineStateChanged);
					((IParameterSetEditorFrame)_frame).ParameterAdded+=(new ParameterEventHandler(OnParameterAdded));
					((IParameterSetEditorFrame)_frame).ParameterRemoved+=(new ParameterEventHandler(OnParameterRemoved));
					((IParameterSetEditorFrame)_frame).ParameterChanged+=(new ParameterChangedEventHandler(OnParameterChanged));
					((IParameterSetEditorFrame3)_frame).ParameterSectionChanged+=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
					if (_frame is DeviceEditor)
					{
						((DeviceEditor)(object)_frame).ParameterMoved += new ParameterMovedEventHandler(OnParameterMoved);
					}
					APEnvironment.DeviceController.ModuleStatusChanged+=(new ModuleStatusEventHandler(base.OnModuleStatusChanged));
					if (((IEditorView)_frame).Editor != null)
					{
						(((IEditorView)_frame).Editor as DeviceEditor).TaskConfigChanged += OnTaskConfigChanged;
					}
				}
			}
		}

		internal override string PageName
		{
			get
			{
				string arg = "<?>";
				if (_frame != null)
				{
					IConnector connector = _frame.GetConnector(_nConnectorId, false);
					if (connector != null)
					{
						arg = connector.VisibleInterfaceName;
					}
				}
				return string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "PageNameConnectorMapping"), arg);
			}
		}

		internal override IConnector Connector(bool bModify)
		{
			return _frame.GetConnector(_nConnectorId, bModify);
		}

		public HideParameterDelegate GetParameterFilter()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			return new HideParameterDelegate(base.HideParameter);
		}

		internal override IParameterSetProvider GetParameterSetProvider()
		{
			return new ConnectorParameterSetProvider(_frame, _nConnectorId)
			{
				LocalizationActive = base.LocalizationActive
			};
		}

		internal override IDriverInfo GetDriverInfo(bool bToModify)
		{
			IConnector connector = _frame.GetConnector(_nConnectorId, bToModify);
			IConnector2 val = (IConnector2)(object)((connector is IConnector2) ? connector : null);
			if (val != null)
			{
				return val.DriverInfo;
			}
			return null;
		}

		internal override IDeviceObject GetDeviceObject(bool bToModify)
		{
			return _frame.GetAssociatedDeviceObject(bToModify);
		}

		protected override void OnParameterAdded(object sender, ParameterEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.ConnectorId < 0 || e.ConnectorId != _nConnectorId || base.ChannelsModel == null)
			{
				return;
			}
			if (base.MappingPage != null && base.MappingPage.IsHidden && ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount > 0)
			{
				base.MappingPage.SetReload();
				return;
			}
			int childCount = ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount;
			base.ChannelsModel.AddParameter(e.Parameter, ParameterTreeTableModelView.IOMappingsOffline, null);
			if (childCount == 0 && ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount > 0)
			{
				base.Page.ChannelPanelVisible = true;
				IConnectorEditorFrame connectorEditorFrame = ConnectorEditorFrame;
				IParameterSetEditorFrame2 val = (IParameterSetEditorFrame2)(object)((connectorEditorFrame is IParameterSetEditorFrame2) ? connectorEditorFrame : null);
				if (val != null)
				{
					val.UpdatePages((IBaseDeviceEditor)(object)this);
				}
			}
		}

		protected override void OnParameterRemoved(object sender, ParameterEventArgs e)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.ConnectorId < 0 || e.ConnectorId != _nConnectorId || base.ChannelsModel == null)
			{
				return;
			}
			int num = 0;
			if (base.MappingPage != null && base.MappingPage.IsHidden)
			{
				base.MappingPage.SetReload();
				foreach (IParameter item in (IEnumerable)base.ChannelsModel.ParameterSet)
				{
					if ((int)item.ChannelType != 0)
					{
						num++;
					}
				}
				if (num > 0)
				{
					return;
				}
			}
			int childCount = ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount;
			base.ChannelsModel.RemoveParameterNode(e.Parameter);
			if (base.MappingPage == null)
			{
				num = ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount;
			}
			else
			{
				foreach (IParameter item2 in (IEnumerable)base.ChannelsModel.ParameterSet)
				{
					if ((int)item2.ChannelType != 0)
					{
						num++;
					}
				}
			}
			if (childCount > 0 && num == 0)
			{
				Reload();
				base.Page.ChannelPanelVisible = false;
				IConnectorEditorFrame connectorEditorFrame = ConnectorEditorFrame;
				IParameterSetEditorFrame2 val = (IParameterSetEditorFrame2)(object)((connectorEditorFrame is IParameterSetEditorFrame2) ? connectorEditorFrame : null);
				if (val != null)
				{
					val.UpdatePages((IBaseDeviceEditor)(object)this);
				}
			}
		}

		protected override void OnParameterChanged(object sender, ParameterChangedEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (((ParameterEventArgs)e).ConnectorId < 0 || ((ParameterEventArgs)e).ConnectorId != _nConnectorId || base.ChannelsModel == null)
			{
				return;
			}
			TreeTableView val = (TreeTableView)(object)((base.Page != null) ? base.Page.MappingTreeTableView : null);
			if (val == null || !((Control)(object)val).IsDisposed)
			{
				if (base.Page != null && base.Page.Controls.Count > 0 && base.Page.Controls[0].Visible)
				{
					base.ChannelsModel.UpdateNode(((ParameterEventArgs)e).Parameter, e.DataElement, e.Path);
				}
				else
				{
					_dictOnParameter.Add((object)e, 2);
				}
			}
		}

		protected override void OnParameterSectionChanged(object sender, ParameterSectionChangedEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.Section != null && base.ChannelsModel != null)
			{
				base.ChannelsModel.UpdateSection((IParameterSection)(object)e.Section);
			}
		}

		protected override void OnParameterMoved(object sender, ParameterMovedEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (((ParameterEventArgs)e).Parameter.Section != null && base.ChannelsModel != null)
			{
				base.ChannelsModel.UpdateSection(((ParameterEventArgs)e).Parameter);
			}
		}

		internal override IDeviceObject GetHost()
		{
			try
			{
				if (((IEditorView)_frame).Editor is DeviceEditor)
				{
					return (((IEditorView)_frame).Editor as DeviceEditor).GetHost(bToModify: false);
				}
			}
			catch
			{
			}
			return null;
		}

		internal new void Reload()
		{
			_dictOnParameter.Clear();
			base.Reload();
		}

		internal void Paint()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (_dictOnParameter.Count <= 0 || base.ChannelsModel == null)
			{
				return;
			}
			foreach (KeyValuePair<object, int> item in _dictOnParameter)
			{
				if (item.Value == 2)
				{
					ParameterChangedEventArgs parameterChangedEventArgs = item.Key as ParameterChangedEventArgs;
					base.ChannelsModel.UpdateNode(parameterChangedEventArgs.Parameter, parameterChangedEventArgs.DataElement, parameterChangedEventArgs.Path);
				}
			}
			_dictOnParameter.Clear();
		}

		internal override IConnectorEditorFrame GetFrame()
		{
			return _frame;
		}

		internal void OnTaskConfigChanged(object sender, EventArgs e)
		{
			if (base.MappingPage != null)
			{
				base.MappingPage.RefillBusCycleList();
			}
		}
	}
}
