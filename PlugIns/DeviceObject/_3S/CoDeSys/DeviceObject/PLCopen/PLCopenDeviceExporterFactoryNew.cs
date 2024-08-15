using System.Xml;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.PLCopenXML;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[TypeGuid("{BA96D5DF-9F29-43F3-B5BD-4C613474D548}")]
	internal class PLCopenDeviceExporterFactoryNew : IPLCopenDeviceImporterFactory
	{
		public IPLCopenDeviceImporter CreateImporter(IPLCopenXMLConfigImportObject importObject, int index, XmlNode dataNode)
		{
			PLCopenDeviceImporter pLCopenDeviceImporter = new PLCopenDeviceImporter();
			pLCopenDeviceImporter.Init(importObject, index, dataNode);
			return (IPLCopenDeviceImporter)(object)pLCopenDeviceImporter;
		}
	}
}
