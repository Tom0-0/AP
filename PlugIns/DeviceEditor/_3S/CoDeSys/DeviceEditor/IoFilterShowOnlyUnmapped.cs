using System.Collections;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{0010215E-EDB9-4FC5-A542-DA3EBDC02B29}")]
	public class IoFilterShowOnlyUnmapped : IIoMappingEditorFilterFactory
	{
		public string FilterName => Strings.IoFilterShowOnlyUnmapped;

		public bool MatchSubElements => false;

		public bool ShowFilter(IDeviceObject device, IParameterSet parameterSet)
		{
			return true;
		}

		public bool FilterChannel(IDeviceObject device, IParameterSet parameterSet, IParameter parameter, IDataElement dataElement)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			bool result = true;
			foreach (IVariableMapping item in (IEnumerable)dataElement.IoMapping.VariableMappings)
			{
				if (!string.IsNullOrEmpty(item.Variable))
				{
					result = false;
				}
			}
			return result;
		}
	}
}
