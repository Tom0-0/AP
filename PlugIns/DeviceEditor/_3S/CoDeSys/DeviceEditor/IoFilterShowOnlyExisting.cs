using System.Collections;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{9BA9442D-C618-4329-B04F-09E54AC379C5}")]
	public class IoFilterShowOnlyExisting : IIoMappingEditorFilterFactory
	{
		public string FilterName => Strings.IoFilterShowOnlyExisting;

		public bool MatchSubElements => true;

		public bool ShowFilter(IDeviceObject device, IParameterSet parameterSet)
		{
			return true;
		}

		public bool FilterChannel(IDeviceObject device, IParameterSet parameterSet, IParameter parameter, IDataElement dataElement)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = true;
			foreach (IVariableMapping item in (IEnumerable)dataElement.IoMapping.VariableMappings)
			{
				if (item.CreateVariable)
				{
					flag = true;
				}
				if (!string.IsNullOrEmpty(item.Variable))
				{
					flag2 = false;
				}
			}
			if (!flag2)
			{
				return !flag;
			}
			return false;
		}
	}
}
