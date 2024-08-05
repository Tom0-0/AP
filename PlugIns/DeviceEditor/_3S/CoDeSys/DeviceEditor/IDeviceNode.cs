using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal interface IDeviceNode
	{
		IParameterTreeTable TreeModel { get; }

		SectionNode GetOrCreateSectionNode(IParameterSection section, IDeviceNode device, IParameterTreeNode parent);
	}
}
