using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal interface IParameterSetProvider
	{
		bool LocalizationActive { get; set; }

		IIoProvider GetIoProvider(bool bToModify);

		IParameterSet GetParameterSet(bool bToModify);

		IDeviceObject GetHost();

		IDeviceObject GetDevice();
	}
}
