using System.Collections;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{782BA78A-EC5D-4372-BE86-BE2347D07958}")]
	public class IoFilterShowOnlyMapped : IIoMappingEditorFilterFactory
	{
		public string FilterName => Strings.IoFilterShowOnlyMapped;

		public bool MatchSubElements => true;

		public bool ShowFilter(IDeviceObject device, IParameterSet parameterSet)
		{
			return true;
		}

		public bool FilterChannel(IDeviceObject device, IParameterSet parameterSet, IParameter parameter, IDataElement dataElement)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			bool flag = true;
			foreach (IVariableMapping item in (IEnumerable)dataElement.IoMapping.VariableMappings)
			{
				if (!string.IsNullOrEmpty(item.Variable))
				{
					flag = false;
				}
			}
			return !flag;
		}
	}
}
