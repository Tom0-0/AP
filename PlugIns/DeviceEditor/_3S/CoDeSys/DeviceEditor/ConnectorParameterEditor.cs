using System;
using System.Collections.Generic;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class ConnectorParameterEditor : ParameterEditor, IConnectorEditor, IBaseDeviceEditor, IBaseDeviceEditor2, IEditorBasedFindReplace
	{
		private LDictionary<object, int> _dictOnParameter = new LDictionary<object, int>();

		private IConnectorEditorFrame _connectorEditor;

		private int _nConnectorId = -1;

		internal override string PageName
		{
			get
			{
				string arg = "<?>";
				if (_connectorEditor != null)
				{
					IConnector connector = _connectorEditor.GetConnector(_nConnectorId, false);
					if (connector != null)
					{
						arg = connector.VisibleInterfaceName;
					}
				}
				return string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "PageNameConnectorConfiguration"), arg);
			}
		}

		internal override IHasOnlineMode HasOnlineMode
		{
			get
			{
				IConnectorEditorFrame connectorEditor = _connectorEditor;
				return (IHasOnlineMode)(object)((connectorEditor is IHasOnlineMode) ? connectorEditor : null);
			}
		}

		public IConnectorEditorFrame ConnectorEditorFrame
		{
			get
			{
				return _connectorEditor;
			}
			set
			{
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Expected O, but got Unknown
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Expected O, but got Unknown
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Expected O, but got Unknown
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Expected O, but got Unknown
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Expected O, but got Unknown
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Expected O, but got Unknown
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Expected O, but got Unknown
				if (_connectorEditor != null)
				{
					_connectorEditor.OnlineStateChanged-=((EventHandler)OnOnlineStateChanged);
					((IParameterSetEditorFrame)_connectorEditor).ParameterAdded-=(new ParameterEventHandler(OnParameterAdded));
					((IParameterSetEditorFrame)_connectorEditor).ParameterRemoved-=(new ParameterEventHandler(OnParameterRemoved));
					((IParameterSetEditorFrame)_connectorEditor).ParameterChanged-=(new ParameterChangedEventHandler(OnParameterChanged));
					((IParameterSetEditorFrame3)_connectorEditor).ParameterSectionChanged-=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
				}
				if (value != null)
				{
					_connectorEditor = value;
					if (_connectorEditor != null)
					{
						_connectorEditor.OnlineStateChanged+=((EventHandler)OnOnlineStateChanged);
						((IParameterSetEditorFrame)_connectorEditor).ParameterAdded+=(new ParameterEventHandler(OnParameterAdded));
						((IParameterSetEditorFrame)_connectorEditor).ParameterRemoved+=(new ParameterEventHandler(OnParameterRemoved));
						((IParameterSetEditorFrame)_connectorEditor).ParameterChanged+=(new ParameterChangedEventHandler(OnParameterChanged));
						((IParameterSetEditorFrame3)_connectorEditor).ParameterSectionChanged+=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
					}
				}
			}
		}

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

		public bool HideGenericEditor => true;

		public ConnectorParameterEditor(HideParameterDelegate paramFilter)
			: base(paramFilter)
		{
		}

		internal override IDeviceObject GetHost()
		{
			try
			{
				if (((IEditorView)_connectorEditor).Editor is DeviceEditor)
				{
					return (((IEditorView)_connectorEditor).Editor as DeviceEditor).GetHost(bToModify: false);
				}
			}
			catch
			{
			}
			return null;
		}

		public HideParameterDelegate GetParameterFilter()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			return new HideParameterDelegate(HideParameter);
		}

		public bool HideParameter(int nParameterId, string[] componentPath)
		{
			return true;
		}

		protected override void OnOnlineStateChanged(object sender, EventArgs e)
		{
			if (base.EditorPage != null)
			{
				base.EditorPage.EnableMonitoringRange();
			}
		}

		protected override void OnParameterAdded(object sender, ParameterEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.ConnectorId >= 0 && e.ConnectorId == ConnectorId && base.Model != null)
			{
				_dictOnParameter.Add((object)e, 0);
			}
		}

		protected override void OnParameterRemoved(object sender, ParameterEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.ConnectorId >= 0 && e.ConnectorId == ConnectorId && base.Model != null)
			{
				_dictOnParameter.Add((object)e, 1);
			}
		}

		protected override void OnParameterChanged(object sender, ParameterChangedEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (((ParameterEventArgs)e).ConnectorId >= 0 && ((ParameterEventArgs)e).ConnectorId == ConnectorId && base.Model != null)
			{
				TreeTableView val = ((base.EditorPage != null) ? base.EditorPage.ParametersTree : null);
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
			if (e.Section != null && base.Model != null)
			{
				base.Model.UpdateSection((IParameterSection)(object)e.Section);
			}
		}

		internal override IDeviceObject GetDeviceObject(bool bToModify)
		{
			return _connectorEditor.GetAssociatedDeviceObject(bToModify);
		}

		internal override IDeviceObject GetHost(bool bToModify)
		{
			try
			{
				if (((IEditorView)_connectorEditor).Editor is DeviceEditor)
				{
					return (((IEditorView)_connectorEditor).Editor as DeviceEditor).GetHost(bToModify);
				}
			}
			catch
			{
			}
			return null;
		}

		internal override IParameterSetProvider GetParameterSetProvider()
		{
			return new ConnectorParameterSetProvider(_connectorEditor, _nConnectorId);
		}

		internal void UpdateView()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (_dictOnParameter.Count <= 0 || base.Model == null)
			{
				return;
			}
			foreach (KeyValuePair<object, int> item in _dictOnParameter)
			{
				if (item.Value == 0)
				{
					base.Model.AddParameter((item.Key as ParameterEventArgs).Parameter, ParameterTreeTableModelView.Parameters, _paramFilter);
				}
				if (item.Value == 1)
				{
					base.Model.RemoveParameterNode((item.Key as ParameterEventArgs).Parameter);
				}
				if (item.Value == 2)
				{
					ParameterChangedEventArgs parameterChangedEventArgs = item.Key as ParameterChangedEventArgs;
					base.Model.UpdateNode(parameterChangedEventArgs.Parameter, parameterChangedEventArgs.DataElement, parameterChangedEventArgs.Path);
				}
			}
			_dictOnParameter.Clear();
		}
	}
}
