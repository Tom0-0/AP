using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{4631006A-E9A2-4B78-9107-DDCF519440ED}")]
	public class IoFilterShowOnlyOutputs : IIoMappingEditorFilterFactory
	{
		public string FilterName => Strings.IoFilterShowOnlyOutputs;

		public bool MatchSubElements => true;

		public bool ShowFilter(IDeviceObject device, IParameterSet parameterSet)
		{
			return true;
		}

		public bool FilterChannel(IDeviceObject device, IParameterSet parameterSet, IParameter parameter, IDataElement dataElement)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			return ((int)parameter.ChannelType == 2) | ((int)parameter.ChannelType == 3);
		}
	}
}
