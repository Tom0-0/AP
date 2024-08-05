using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class GenericeEditorColumnHideFactoryManager
	{
		public static bool HideColumn(IIoProvider ioProvider, GenericEditorColumn column)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			int num = -1;
			if (APEnvironment.ParameterColumnHideFactories != null)
			{
				bool flag = default(bool);
				foreach (IDeviceParameterHideColumnFactory parameterColumnHideFactory in APEnvironment.ParameterColumnHideFactories)
				{
					int num2 = parameterColumnHideFactory.HideEditorColumn(ioProvider, column, out flag);
					if (num2 > num)
					{
						num = num2;
						result = flag;
					}
				}
				return result;
			}
			return result;
		}
	}
}
