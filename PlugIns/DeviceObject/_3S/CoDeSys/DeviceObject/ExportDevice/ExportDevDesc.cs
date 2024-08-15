using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject.DevDesc;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.PLCopenXML;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject.ExportDevice
{
	[TypeGuid("{F78AA927-46C8-46a5-8925-776CC25877FF}")]
	public class ExportDevDesc : IStandardCommand, ICommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "export", "device xml" };

		internal static readonly Lazy<XmlSerializer> S_DevDescSerializer = new Lazy<XmlSerializer>(delegate
		{
			XmlAttributes attributes = new XmlAttributes
			{
				XmlIgnore = true
			};
			XmlAttributeOverrides xmlAttributeOverrides = new XmlAttributeOverrides();
			xmlAttributeOverrides.Add(typeof(ParameterTypeAttributes), "channel", attributes);
			return new XmlSerializer(typeof(DeviceDescription), xmlAttributeOverrides);
		});

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string Name => Strings.ExportDevDescName;

		public string Description => Strings.ExportDevDescDescription;

		public string ToolTipText => Strings.ExportDevDescTooltip;

		public Icon SmallIcon => null;

		public Icon LargeIcon => null;

		public bool Enabled
		{
			get
			{
				IMetaObjectStub selectedDevice = GetSelectedDevice();
				if (selectedDevice == null)
				{
					return false;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedDevice.ProjectHandle, selectedDevice.ObjectGuid);
				if (objectToRead != null && objectToRead.Object is IDeviceObject)
				{
					IObject @object = objectToRead.Object;
					IObject obj = ((@object is IDeviceObject) ? @object : null);
					IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((obj is IDeviceObject5) ? obj : null)).DeviceIdentificationNoSimulation;
					if (!(deviceIdentificationNoSimulation is IModuleIdentification) && ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(deviceIdentificationNoSimulation) != null)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string[] BatchCommand => new string[0];

		private IMetaObjectStub GetSelectedDevice()
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return null;
			}
			ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length == 0)
			{
				return null;
			}
			if (selectedSVNodes.Length > 1)
			{
				return null;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(selectedSVNodes[0].ProjectHandle, selectedSVNodes[0].ObjectGuid);
			if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return metaObjectStub;
			}
			return null;
		}

		public string[] CreateBatchArguments()
		{
			IMetaObjectStub selectedDevice = GetSelectedDevice();
			string fileName = string.Empty;
			if (selectedDevice != null)
			{
				fileName = selectedDevice.Name;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = ".devdesc.xml";
			saveFileDialog.Filter = "Device description xml files (*.devdesc.xml)|*.devdesc.xml";
			saveFileDialog.FileName = fileName;
			string[] array = null;
			return (saveFileDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) != DialogResult.OK) ? new string[0] : new string[1] { saveFileDialog.FileName };
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (bContextMenu)
			{
				if (Enabled)
				{
					return ((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl;
				}
				return false;
			}
			return true;
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Expected O, but got Unknown
			if (arguments.Length == 0)
			{
				return;
			}
			string text = arguments[0];
			IMetaObjectStub selectedDevice = GetSelectedDevice();
			if (selectedDevice == null)
			{
				return;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedDevice.ProjectHandle, selectedDevice.ObjectGuid);
			if (objectToRead == null || !(objectToRead.Object is IDeviceObject))
			{
				return;
			}
			IObject @object = objectToRead.Object;
			IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
			IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((val is IDeviceObject5) ? val : null)).DeviceIdentificationNoSimulation;
			IRepositorySource val2 = default(IRepositorySource);
			((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(deviceIdentificationNoSimulation, out val2);
			string localPath = new Uri(val2.LocationUrl).LocalPath;
			string text2 = EscapeString(deviceIdentificationNoSimulation.Id);
			string text3 = EscapeString(deviceIdentificationNoSimulation.Version);
			localPath = Path.Combine(localPath, deviceIdentificationNoSimulation.Type + "\\" + text2 + "\\" + text3 + "\\");
			string text4 = Path.Combine(localPath, "device.xml");
			if (!AuthFile.Exists(text4))
			{
				return;
			}
			string[] files = Directory.GetFiles(localPath, "*.*");
			for (int i = 0; i < files.Length; i++)
			{
				string fileName = Path.GetFileName(files[i]);
				if (fileName.ToLowerInvariant() != "device.xml")
				{
					try
					{
						string text5 = Path.Combine(Path.GetDirectoryName(text), fileName);
						AuthFile.Copy(Path.Combine(localPath, fileName), text5, true);
					}
					catch
					{
					}
				}
			}
			Stream stream = null;
			DeviceDescription deviceDescription = null;
			try
			{
				stream = (Stream)(object)AuthFile.Open(text4, FileMode.Open);
				if (stream != null)
				{
					XmlTextReader xmlTextReader = new XmlTextReader(stream);
					XmlSerializer value = S_DevDescSerializer.Value;
					if (value.CanDeserialize(xmlTextReader))
					{
						deviceDescription = value.Deserialize(xmlTextReader) as DeviceDescription;
					}
					xmlTextReader.Close();
				}
			}
			catch (Exception ex)
			{
				APEnvironment.MessageService.Error(text4 + ": " + ex.Message, "Exception", Array.Empty<object>());
			}
			finally
			{
				stream?.Close();
			}
			if (deviceDescription == null)
			{
				APEnvironment.MessageService.Error(string.Format(Strings.ExportErrorFileEncryped, text4), "ExportErrorFileEncryped", Array.Empty<object>());
			}
			stream = null;
			if (deviceDescription == null)
			{
				return;
			}
			foreach (IParameter item in (IEnumerable)val.DeviceParameterSet)
			{
				_ = item;
				object[] items = new ExportParameterSetHelper().ExportParameterSet(val.DeviceParameterSet as ParameterSet, deviceDescription);
				deviceDescription.Device.DeviceParameterSet.Items = items;
			}
			foreach (IConnector item2 in (IEnumerable)val.Connectors)
			{
				IConnector val3 = item2;
				if (deviceDescription.Device == null || deviceDescription.Device.Connector == null)
				{
					continue;
				}
				DeviceDescriptionDeviceConnector[] connector = deviceDescription.Device.Connector;
				foreach (DeviceDescriptionDeviceConnector deviceDescriptionDeviceConnector in connector)
				{
					if (deviceDescriptionDeviceConnector.connectorId == val3.ConnectorId)
					{
						object[] array2 = (deviceDescriptionDeviceConnector.HostParameterSet = new ExportParameterSetHelper().ExportParameterSet(val3.HostParameterSet as ParameterSet, deviceDescription));
					}
				}
			}
			try
			{
				stream = (Stream)(object)AuthFile.Open(text, FileMode.Create);
				if (stream != null)
				{
					Encoding encoding = Encoding.GetEncoding("UTF-8");
					XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, encoding);
					xmlTextWriter.Formatting = Formatting.Indented;
					S_DevDescSerializer.Value.Serialize(xmlTextWriter, deviceDescription);
				}
			}
			catch (Exception ex2)
			{
				APEnvironment.MessageService.Error(ex2.Message, "Exception", Array.Empty<object>());
			}
			finally
			{
				stream?.Close();
			}
		}

		internal static string EscapeString(string stName)
		{
			char[] array = new char[1];
			StringBuilder stringBuilder = new StringBuilder(stName.Length * 2);
			foreach (char c in stName)
			{
				if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || "_-. ".IndexOf(c) >= 0)
				{
					stringBuilder.Append(c);
					continue;
				}
				array[0] = c;
				byte[] bytes = Encoding.UTF8.GetBytes(array);
				for (int j = 0; j < bytes.Length; j++)
				{
					stringBuilder.AppendFormat("%{0:X2}", bytes[j]);
				}
			}
			return stringBuilder.ToString();
		}

		internal static DeviceDescription LoadDevdesc(IDeviceIdentification devIdent)
		{
			FindDevDesc(ref devIdent, out var source);
			string localPath = new Uri(source.LocationUrl).LocalPath;
			string text = EscapeString(devIdent.Id);
			string text2 = EscapeString(devIdent.Version);
			string text3 = Path.Combine(Path.Combine(localPath, devIdent.Type + "\\" + text + "\\" + text2 + "\\"), "device.xml");
			if (AuthFile.Exists(text3))
			{
				Stream stream = null;
				DeviceDescription deviceDescription = null;
				try
				{
					stream = (Stream)(object)AuthFile.Open(text3, FileMode.Open);
					if (stream != null)
					{
						XmlTextReader xmlTextReader = new XmlTextReader(stream);
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(xmlTextReader);
						Process(xmlDocument);
						XmlNodeReader xmlReader = new XmlNodeReader(xmlDocument.DocumentElement);
						XmlSerializer value = S_DevDescSerializer.Value;
						if (value.CanDeserialize(xmlReader))
						{
							deviceDescription = value.Deserialize(xmlReader) as DeviceDescription;
						}
						xmlTextReader.Close();
					}
				}
				catch (Exception ex)
				{
					APEnvironment.MessageService.Error(text3 + ": " + ex.Message, "Exception", Array.Empty<object>());
				}
				finally
				{
					stream?.Close();
				}
				if (deviceDescription == null)
				{
					APEnvironment.MessageService.Error(string.Format(Strings.ExportErrorFileEncryped, text3), "ExportErrorFileEncryped", Array.Empty<object>());
				}
				return deviceDescription;
			}
			return null;
		}

		internal static IDeviceDescription FindDevDesc(ref IDeviceIdentification devIdent, out IRepositorySource source)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(devIdent, out source);
			if (device == null)
			{
				IDeviceIdentification val = null;
				foreach (IDeviceDescription item in (IEnumerable)((IDeviceRepository)APEnvironment.DeviceRepository).GetAllDevices())
				{
					IDeviceDescription val2 = item;
					if (val2.DeviceIdentification.Type == devIdent.Type && val2.DeviceIdentification.Id == devIdent.Id)
					{
						Version result;
						Version result2;
						if (val == null)
						{
							val = val2.DeviceIdentification;
						}
						else if (Version.TryParse(val.Version, out result) && Version.TryParse(val2.DeviceIdentification.Version, out result2) && result2 > result)
						{
							val = val2.DeviceIdentification;
						}
					}
				}
				if (val != null)
				{
					devIdent = val;
					device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(devIdent, out source);
				}
			}
			return device;
		}

		internal static DeviceDescription LoadDevdesc(ref IDeviceIdentification devIdent, IPLCopenXMLImportReporter reporter)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			FindDevDesc(ref devIdent, out var source);
			if (source == null)
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)5, "Device Type: " + devIdent.Type + " ID: " + devIdent.Id + " Version: " + devIdent.Version + " not found !");
			}
			string localPath = new Uri(source.LocationUrl).LocalPath;
			string text = EscapeString(devIdent.Id);
			string text2 = EscapeString(devIdent.Version);
			string text3 = Path.Combine(Path.Combine(localPath, devIdent.Type + "\\" + text + "\\" + text2 + "\\"), "device.xml");
			if (AuthFile.Exists(text3))
			{
				using (Stream input = AuthFile.Open(text3, FileMode.Open))
				{
					using XmlTextReader reader = new XmlTextReader(input);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(reader);
					Process(xmlDocument);
					XmlNodeReader xmlReader = new XmlNodeReader(xmlDocument.DocumentElement);
					XmlSerializer value = S_DevDescSerializer.Value;
					if (value.CanDeserialize(xmlReader))
					{
						try
						{
							return value.Deserialize(xmlReader) as DeviceDescription;
						}
						catch (InvalidOperationException ex)
						{
							string message = ex.InnerException.Message;
							reporter.ReportError(("Device Description '" + text3 + "' invalid: " + message) ?? "");
							throw ex;
						}
					}
				}
				return null;
			}
			throw new DeviceObjectException((DeviceObjectExeptionReason)14, "File " + text3 + " not found !");
		}

		private static void Process(XmlNode node)
		{
			Process(node, 0);
		}

		private static void Process(XmlNode node, int level)
		{
			if (node.Attributes != null)
			{
				foreach (System.Xml.XmlAttribute attribute in node.Attributes)
				{
					switch (attribute.LocalName)
					{
					case "alwaysmapping":
					case "hideInStatusPage":
					case "updateAllowed":
					case "alwaysmappingDisabled":
					{
						if (bool.TryParse(attribute.Value, out var result))
						{
							attribute.Value = XmlConvert.ToString(result);
						}
						break;
					}
					}
				}
			}
			if (node.ChildNodes == null)
			{
				return;
			}
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					Process(childNode, level + 1);
				}
			}
		}

		internal static XmlDocument ConvertToXmlDocument(DeviceDescription devdesc)
		{
			XmlSerializer value = S_DevDescSerializer.Value;
			XmlDocument xmlDocument = new XmlDocument();
			using XmlWriter xmlWriter = xmlDocument.CreateNavigator().AppendChild();
			xmlWriter.WriteStartDocument();
			value.Serialize(xmlWriter, devdesc);
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			return xmlDocument;
		}
	}
}
