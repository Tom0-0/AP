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
	public class DeviceIoMappingEditor : IOMappingEditor, IDeviceEditor, IBaseDeviceEditor, IBaseDeviceEditor2, IEditorBasedFindReplace, ILocalizableEditor
	{
		private IDeviceEditorFrame _deviceEditor;

		private LDictionary<object, int> _dictOnParameter = new LDictionary<object, int>();

		internal override IHasOnlineMode HasOnlineMode
		{
			get
			{
				IDeviceEditorFrame deviceEditor = _deviceEditor;
				return (IHasOnlineMode)(object)((deviceEditor is IHasOnlineMode) ? deviceEditor : null);
			}
		}

		public IDeviceEditorFrame DeviceEditorFrame
		{
			get
			{
				return _deviceEditor;
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
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Expected O, but got Unknown
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Expected O, but got Unknown
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Expected O, but got Unknown
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Expected O, but got Unknown
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Expected O, but got Unknown
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Expected O, but got Unknown
				if (_deviceEditor != null)
				{
					_deviceEditor.OnlineStateChanged-=((EventHandler)base.OnOnlineStateChanged);
					((IParameterSetEditorFrame)_deviceEditor).ParameterAdded-=(new ParameterEventHandler(OnParameterAdded));
					((IParameterSetEditorFrame)_deviceEditor).ParameterRemoved-=(new ParameterEventHandler(OnParameterRemoved));
					((IParameterSetEditorFrame)_deviceEditor).ParameterChanged-=(new ParameterChangedEventHandler(OnParameterChanged));
					((IParameterSetEditorFrame3)_deviceEditor).ParameterSectionChanged-=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
					if (_deviceEditor is DeviceEditor)
					{
						((DeviceEditor)(object)_deviceEditor).ParameterMoved -= new ParameterMovedEventHandler(OnParameterMoved);
					}
					APEnvironment.DeviceController.ModuleStatusChanged-=(new ModuleStatusEventHandler(OnModuleStatusChangedDevice));
				}
				_deviceEditor = value;
				if (_deviceEditor != null)
				{
					_deviceEditor.OnlineStateChanged+=((EventHandler)base.OnOnlineStateChanged);
					((IParameterSetEditorFrame)_deviceEditor).ParameterAdded+=(new ParameterEventHandler(OnParameterAdded));
					((IParameterSetEditorFrame)_deviceEditor).ParameterRemoved+=(new ParameterEventHandler(OnParameterRemoved));
					((IParameterSetEditorFrame)_deviceEditor).ParameterChanged+=(new ParameterChangedEventHandler(OnParameterChanged));
					((IParameterSetEditorFrame3)_deviceEditor).ParameterSectionChanged+=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
					if (_deviceEditor is DeviceEditor)
					{
						((DeviceEditor)(object)_deviceEditor).ParameterMoved += new ParameterMovedEventHandler(OnParameterMoved);
					}
					APEnvironment.DeviceController.ModuleStatusChanged+=(new ModuleStatusEventHandler(OnModuleStatusChangedDevice));
				}
			}
		}

		internal override string PageName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "PageNameMapping");

		internal override Guid ObjectGuid
		{
			get
			{
				if (_deviceEditor != null && ((IEditorView)_deviceEditor).Editor != null)
				{
					return ((IEditorView)_deviceEditor).Editor.ObjectGuid;
				}
				return Guid.Empty;
			}
		}

		internal override IConnector Connector(bool bModify)
		{
			return null;
		}

		public HideParameterDelegate GetParameterFilter()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			return new HideParameterDelegate(base.HideParameter);
		}

		internal override IParameterSetProvider GetParameterSetProvider()
		{
			return new DeviceParameterSetProvider(_deviceEditor)
			{
				LocalizationActive = base.LocalizationActive
			};
		}

		internal override IDriverInfo GetDriverInfo(bool bToModify)
		{
			IDeviceObject deviceObject = _deviceEditor.GetDeviceObject(bToModify);
			IDeviceObject2 val = (IDeviceObject2)(object)((deviceObject is IDeviceObject2) ? deviceObject : null);
			if (val != null)
			{
				return val.DriverInfo;
			}
			return null;
		}

		internal override IDeviceObject GetDeviceObject(bool bToModify)
		{
			return _deviceEditor.GetDeviceObject(bToModify);
		}

		protected override void OnParameterAdded(object sender, ParameterEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.ConnectorId >= 0 || base.ChannelsModel == null)
			{
				return;
			}
			if (base.MappingPage != null && base.MappingPage.IsHidden && ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount > 0)
			{
				base.MappingPage.SetReload();
				if (((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount > 0)
				{
					return;
				}
			}
			int childCount = ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount;
			base.ChannelsModel.AddParameter(e.Parameter, ParameterTreeTableModelView.IOMappingsOffline, null);
			if (childCount == 0 && ((AbstractTreeTableModel)base.ChannelsModel).Sentinel.ChildCount > 0)
			{
				IDeviceEditorFrame deviceEditorFrame = DeviceEditorFrame;
				IParameterSetEditorFrame2 val = (IParameterSetEditorFrame2)(object)((deviceEditorFrame is IParameterSetEditorFrame2) ? deviceEditorFrame : null);
				if (val != null)
				{
					val.UpdatePages((IBaseDeviceEditor)(object)this);
				}
			}
		}

		protected override void OnParameterRemoved(object sender, ParameterEventArgs e)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.ConnectorId >= 0 || base.ChannelsModel == null)
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
			if (childCount > 0 && num == 0)
			{
				Reload();
				IDeviceEditorFrame deviceEditorFrame = DeviceEditorFrame;
				IParameterSetEditorFrame2 val = (IParameterSetEditorFrame2)(object)((deviceEditorFrame is IParameterSetEditorFrame2) ? deviceEditorFrame : null);
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
			if (((ParameterEventArgs)e).ConnectorId < 0)
			{
				TreeTableView val = (TreeTableView)(object)((base.Page != null) ? base.Page.MappingTreeTableView : null);
				if (val == null || !((Control)(object)val).IsDisposed)
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
			if (e.Section != null)
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
			if (((ParameterEventArgs)e).Parameter.Section != null)
			{
				base.ChannelsModel.UpdateSection(((ParameterEventArgs)e).Parameter.Section);
			}
		}

		internal override IDeviceObject GetHost()
		{
			if (_deviceEditor == null)
			{
				return null;
			}
			return _deviceEditor.GetDeviceObject(false);
		}

		internal override IConnectorEditorFrame GetFrame()
		{
			return null;
		}

		internal void OnModuleStatusChangedDevice(object sender, IModuleStatusEventArgs e)
		{
			if (e.ObjectGuid == ObjectGuid && base.MappingPage != null)
			{
				base.MappingPage.OnModuleStatusChanged(sender, e);
			}
		}

		internal void UpdateBusCycleList(object sender, EventArgs e)
		{
			if (base.MappingPage != null)
			{
				base.MappingPage.RefillBusCycleList();
			}
		}

		internal new void Reload()
		{
			_dictOnParameter.Clear();
			base.Reload();
		}

		internal void Paint()
		{
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
	}
}
