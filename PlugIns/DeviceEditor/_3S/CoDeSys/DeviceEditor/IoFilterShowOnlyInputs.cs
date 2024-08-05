using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{A06DFF1B-15A4-4C04-A311-3DEE4A46AD8D}")]
	public class IoFilterShowOnlyInputs : IIoMappingEditorFilterFactory
	{
		public string FilterName => Strings.IoFilterShowOnlyInputs;

		public bool MatchSubElements => true;

		public bool ShowFilter(IDeviceObject device, IParameterSet parameterSet)
		{
			return true;
		}

		public bool FilterChannel(IDeviceObject device, IParameterSet parameterSet, IParameter parameter, IDataElement dataElement)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			return (int)parameter.ChannelType == 1;
		}
	}
}
