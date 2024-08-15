using _3S.CoDeSys.DeviceObject.DevDesc;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject.ExportDevice
{
	internal class ParameterExport
	{
		internal ParameterSection section;

		internal ParameterSectionType sectionType;

		internal LList<ParameterExport> liSubs = new LList<ParameterExport>();

		internal LList<ParameterType> liParams = new LList<ParameterType>();

		internal object[] objects;
	}
}
