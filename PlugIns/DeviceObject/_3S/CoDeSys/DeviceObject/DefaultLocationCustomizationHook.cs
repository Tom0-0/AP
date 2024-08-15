using System.Globalization;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.RevisionControlHooks;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{73DED246-8F0E-4bba-9F92-64D76ABC7679}")]
	public class DefaultLocationCustomizationHook : IRcsLocationCustomizationHook
	{
		public void CustomizeLocationCreation(IRcsLocationCustomizationArgs args)
		{
			IPSNode node = args.Node;
			if (typeof(ISlotDeviceObject).IsAssignableFrom(node.ObjectType) && node.Index >= 0)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "[{0}]", node.Index);
				string text2 = string.Format(CultureInfo.InvariantCulture, "[{0}_{1}]", node.Index, "{0}");
				args.ModifyLocation(text, text2);
			}
		}
	}
}
