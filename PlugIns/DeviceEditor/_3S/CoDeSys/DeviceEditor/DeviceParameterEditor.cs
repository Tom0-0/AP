using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DeviceParameterEditor : ParameterEditor, IDeviceEditor, IBaseDeviceEditor, IBaseDeviceEditor2, IEditorBasedFindReplace
	{
		internal IDeviceEditorFrame _deviceEditor;

		private LDictionary<object, int> _dictOnParameter = new LDictionary<object, int>();

		internal override string PageName
		{
			get
			{
				string text = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "PageNameConfiguration");
				if (_deviceEditor != null)
				{
					IParameterSet deviceParameterSet = _deviceEditor.GetDeviceObject(false).DeviceParameterSet;
					if (deviceParameterSet != null)
					{
						text = ParameterSet5_EditorName(deviceParameterSet) + " " + text;
					}
				}
				return text;
			}
		}

		public bool HideGenericEditor => true;

		internal override IHasOnlineMode HasOnlineMode
		{
			get
			{
				IDeviceEditorFrame deviceEditor = _deviceEditor;
				return (IHasOnlineMode)(object)((deviceEditor is IHasOnlineMode) ? deviceEditor : null);
			}
		}

		IDeviceEditorFrame IDeviceEditor.DeviceEditorFrame
		{
			get
			{
				return _deviceEditor;
			}
			set
			{
				
				if (_deviceEditor != null)
				{
					_deviceEditor.OnlineStateChanged-=((EventHandler)OnOnlineStateChanged);
					((IParameterSetEditorFrame)_deviceEditor).ParameterAdded-=(new ParameterEventHandler(OnParameterAdded));
					((IParameterSetEditorFrame)_deviceEditor).ParameterRemoved-=(new ParameterEventHandler(OnParameterRemoved));
					((IParameterSetEditorFrame)_deviceEditor).ParameterChanged-=(new ParameterChangedEventHandler(OnParameterChanged));
					((IParameterSetEditorFrame3)_deviceEditor).ParameterSectionChanged-=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
				}
				_deviceEditor = value;
				if (_deviceEditor != null)
				{
					_deviceEditor.OnlineStateChanged+=((EventHandler)OnOnlineStateChanged);
					((IParameterSetEditorFrame)_deviceEditor).ParameterAdded+=(new ParameterEventHandler(OnParameterAdded));
					((IParameterSetEditorFrame)_deviceEditor).ParameterRemoved+=(new ParameterEventHandler(OnParameterRemoved));
					((IParameterSetEditorFrame)_deviceEditor).ParameterChanged+=(new ParameterChangedEventHandler(OnParameterChanged));
					((IParameterSetEditorFrame3)_deviceEditor).ParameterSectionChanged-=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
					((IParameterSetEditorFrame3)_deviceEditor).ParameterSectionChanged+=(new ParameterSectionChangedEventHandler(OnParameterSectionChanged));
				}
			}
		}

		public DeviceParameterEditor(HideParameterDelegate paramFilter)
			: base(paramFilter)
		{
		}

		internal override IDeviceObject GetHost()
		{
			if (_deviceEditor == null)
			{
				return null;
			}
			return _deviceEditor.GetDeviceObject(false);
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
			if (e.ConnectorId < 0)
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
			if (e.ConnectorId < 0)
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
			if (((ParameterEventArgs)e).ConnectorId < 0)
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
			if (e.Section != null)
			{
				base.Model.UpdateSection((IParameterSection)(object)e.Section);
			}
		}

		internal override IDeviceObject GetDeviceObject(bool bToModify)
		{
			return _deviceEditor.GetDeviceObject(bToModify);
		}

		internal override IDeviceObject GetHost(bool bToModify)
		{
			return _deviceEditor.GetDeviceObject(bToModify);
		}

		internal override IParameterSetProvider GetParameterSetProvider()
		{
			return new DeviceParameterSetProvider(_deviceEditor);
		}

		internal static string ParameterSet5_EditorName(IParameterSet parameterSet)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (parameterSet is IParameterSet4)
			{
				return ((IParameterSet4)parameterSet).EditorName;
			}
			if (parameterSet is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)parameterSet).IsFunctionAvailable("GetAlwaysMapping"))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
				return ((IGenericInterfaceExtensionProvider)parameterSet).CallFunction("EditorName", xmlDocument).DocumentElement.InnerText;
			}
			return string.Empty;
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
