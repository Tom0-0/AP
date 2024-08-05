using System;
using System.Collections.Generic;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class ConnectorEditorFactoryManager : FactoryManagerBase<IConnectorEditorFactory, Tuple<IDeviceObject, IConnector>>
	{
		private static ConnectorEditorFactoryManager s_instance;

		public static ConnectorEditorFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new ConnectorEditorFactoryManager();
				}
				return s_instance;
			}
		}

		public IEnumerable<FactoryListElement<IConnectorEditorFactory>> GetFactories(IDeviceObject deviceObject, IConnector connector)
		{
			return GetFactories(new Tuple<IDeviceObject, IConnector>(deviceObject, connector));
		}

		public IConnectorEditorFactory GetFactory(IDeviceObject deviceObject, IConnector connector)
		{
			return GetFactory(new Tuple<IDeviceObject, IConnector>(deviceObject, connector));
		}

		protected override int GetMatch(IConnectorEditorFactory factory, Tuple<IDeviceObject, IConnector> obj)
		{
			return factory.GetMatch(obj.Item1, obj.Item2);
		}

		private ConnectorEditorFactoryManager()
			: base(APEnvironment.ConnectorEditorFactories)
		{
		}
	}
}
