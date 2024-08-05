using System;
using System.Collections;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DeviceInformationPrintPainter : DeviceParameterPrintPainter
	{
		internal DeviceInformationPrintPainter(IDeviceObject deviceObject)
		{
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			if (deviceObject == null)
			{
				throw new ArgumentNullException("deviceObject");
			}
			DeviceParameterPrintParam deviceParameterPrintParam = new DeviceParameterPrintParam();
			_liData.Add(deviceParameterPrintParam);
			deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_Name);
			deviceParameterPrintParam.liData.Add((deviceObject != null && deviceObject.DeviceInfo.Name != null) ? deviceObject.DeviceInfo.Name : string.Empty);
			deviceParameterPrintParam = new DeviceParameterPrintParam();
			_liData.Add(deviceParameterPrintParam);
			deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_Vendor);
			deviceParameterPrintParam.liData.Add((deviceObject != null && deviceObject.DeviceInfo.Vendor != null) ? deviceObject.DeviceInfo.Vendor : string.Empty);
			if (deviceObject != null)
			{
				string text = DeviceInformationControl.GetCategoriesString(deviceObject.DeviceInfo.Categories);
				string familiesString = DeviceInformationControl.GetFamiliesString(deviceObject.DeviceInfo);
				if (!string.IsNullOrEmpty(familiesString))
				{
					text = (string.IsNullOrEmpty(text) ? familiesString : (text + ", " + familiesString));
				}
				deviceParameterPrintParam = new DeviceParameterPrintParam();
				_liData.Add(deviceParameterPrintParam);
				deviceParameterPrintParam.liData.Add(Strings.InformationGroups);
				deviceParameterPrintParam.liData.Add(text);
			}
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			string text5 = string.Empty;
			if (deviceObject != null && deviceObject is IDeviceObject5 && ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation.Version != null)
			{
				IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation;
				text2 = deviceIdentificationNoSimulation.Version;
				if (Version.TryParse(text2, out var result))
				{
					text2 = result.ToString();
				}
				text3 = deviceIdentificationNoSimulation.Type.ToString();
				text4 = deviceIdentificationNoSimulation.Id;
				if (deviceIdentificationNoSimulation is IModuleIdentification)
				{
					text5 = ((IModuleIdentification)((deviceIdentificationNoSimulation is IModuleIdentification) ? deviceIdentificationNoSimulation : null)).ModuleId;
				}
			}
			if (!string.IsNullOrEmpty(text3))
			{
				deviceParameterPrintParam = new DeviceParameterPrintParam();
				_liData.Add(deviceParameterPrintParam);
				deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_DeviceType);
				deviceParameterPrintParam.liData.Add(text3);
			}
			if (!string.IsNullOrEmpty(text4))
			{
				deviceParameterPrintParam = new DeviceParameterPrintParam();
				_liData.Add(deviceParameterPrintParam);
				deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_ID);
				deviceParameterPrintParam.liData.Add(text4);
			}
			if (!string.IsNullOrEmpty(text5))
			{
				deviceParameterPrintParam = new DeviceParameterPrintParam();
				_liData.Add(deviceParameterPrintParam);
				deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_Module);
				deviceParameterPrintParam.liData.Add(text5);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				deviceParameterPrintParam = new DeviceParameterPrintParam();
				_liData.Add(deviceParameterPrintParam);
				deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_Version);
				deviceParameterPrintParam.liData.Add(text2);
			}
			if (!string.IsNullOrEmpty(deviceObject.DeviceInfo.OrderNumber))
			{
				deviceParameterPrintParam = new DeviceParameterPrintParam();
				_liData.Add(deviceParameterPrintParam);
				deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_Ordernumber);
				deviceParameterPrintParam.liData.Add((deviceObject != null && deviceObject.DeviceInfo.OrderNumber != null) ? deviceObject.DeviceInfo.OrderNumber : string.Empty);
			}
			deviceParameterPrintParam = new DeviceParameterPrintParam();
			_liData.Add(deviceParameterPrintParam);
			deviceParameterPrintParam.liData.Add(Strings.Print_Parameter_DescriptionTab);
			deviceParameterPrintParam.liData.Add((deviceObject != null && deviceObject.DeviceInfo.Description != null) ? deviceObject.DeviceInfo.Description : string.Empty);
			if (deviceObject != null && deviceObject.DeviceInfo != null)
			{
				_image = deviceObject.DeviceInfo.Image;
			}
			object value = default(object);
			TypeClass val = default(TypeClass);
			foreach (IConnector item in (IEnumerable)deviceObject.Connectors)
			{
				IParameter parameter = item.HostParameterSet.GetParameter(1879052288L);
				if (parameter == null || string.IsNullOrEmpty(((IDataElement)parameter).Value))
				{
					continue;
				}
				IConverterFromIEC converterFromIEC = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
				try
				{
					converterFromIEC.GetInteger(((IDataElement)parameter).Value, out value, out val);
					byte[] bytes = BitConverter.GetBytes(Convert.ToUInt32(value));
					if (bytes.Length == 4)
					{
						Version version = new Version(bytes[3], bytes[2], bytes[1], bytes[0]);
						deviceParameterPrintParam = new DeviceParameterPrintParam();
						_liData.Add(deviceParameterPrintParam);
						deviceParameterPrintParam.liData.Add(Strings.InformationConfigVersion);
						deviceParameterPrintParam.liData.Add(version.ToString());
					}
				}
				catch
				{
				}
			}
		}
	}
}
