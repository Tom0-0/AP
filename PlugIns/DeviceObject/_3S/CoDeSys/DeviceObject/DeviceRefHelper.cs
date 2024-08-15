#define DEBUG
using System;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core.Device;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class DeviceRefHelper
	{
		public static DeviceIdentification ReadDeviceRef(XmlReader reader, IDeviceIdentification masterDeviceId)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(reader.ReadOuterXml());
			return ReadDeviceRef(xmlDocument.DocumentElement, masterDeviceId);
		}

		public static DeviceIdentification ReadDeviceRef(XmlNode xnDeviceRef, IDeviceIdentification masterDeviceId)
		{
			try
			{
				XmlElement xmlElement = null;
				XmlNode xmlNode = null;
				string text = ((XmlElement)xnDeviceRef).GetAttribute("basename");
				if (text == "")
				{
					text = "$(DeviceName)";
				}
				foreach (XmlNode childNode in xnDeviceRef.ChildNodes)
				{
					if (childNode.NodeType == XmlNodeType.Element)
					{
						if (childNode.Name == "DeviceIdentification")
						{
							xmlElement = (XmlElement)childNode;
						}
						else if (childNode.Name == "LocalModuleId")
						{
							xmlNode = childNode;
						}
					}
				}
				if (xmlNode == null)
				{
					if (xmlElement == null)
					{
						xmlElement = (XmlElement)xnDeviceRef;
					}
					return new DeviceIdentification
					{
						Type = int.Parse(xmlElement.GetAttribute("deviceType")),
						Id = xmlElement.GetAttribute("deviceId"),
						Version = xmlElement.GetAttribute("version"),
						BaseName = text
					};
				}
				if (masterDeviceId != null)
				{
					return new ModuleIdentification
					{
						ModuleId = xmlNode.InnerText,
						Type = masterDeviceId.Type,
						Id = masterDeviceId.Id,
						Version = masterDeviceId.Version,
						BaseName = text
					};
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			return null;
		}
	}
}
