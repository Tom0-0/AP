using System;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{aa0cb0a0-7543-41c5-b545-212d898b58f7}")]
	[StorageVersion("3.3.0.0")]
	public class ErrorConnector : Connector
	{
		private static XmlDocument s_xmldoc;

		static ErrorConnector()
		{
			s_xmldoc = new XmlDocument();
			s_xmldoc.LoadXml("<Connector moduleType=\"256\" interface=\"Common.Error\" role=\"parent\" explicit=\"false\"><Var/></Connector>");
		}

		public ErrorConnector()
		{
		}

		public ErrorConnector(ErrorConnector orig)
			: base(orig)
		{
		}

		internal ErrorConnector(TypeList types, IDeviceIdentification deviceId, int nHostPath)
			: base(s_xmldoc.DocumentElement, types, deviceId, bCreateBitChannels: false)
		{
			_nHostpath = nHostPath;
		}

		public override object Clone()
		{
			ErrorConnector errorConnector = new ErrorConnector(this);
			((GenericObject)errorConnector).AfterClone();
			return errorConnector;
		}

		internal void AddModules(LList<Guid> modules)
		{
			VarAdapter varAdapter = (VarAdapter)(object)_adapters[0];
			foreach (Guid module in modules)
			{
				varAdapter.Append(module);
			}
		}
	}
}
