#define DEBUG
using System.Diagnostics;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	public class BitWordAddressFlatAdressAssignmentStrategy : FlatAdressAssignmentStrategy
	{
		public BitWordAddressFlatAdressAssignmentStrategy(int iMinStructureGranularity)
			: base(iMinStructureGranularity)
		{
		}

		public override int[] GetComponentsNormalized(IDirectVariable addr)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			int[] components = addr.Components;
			Debug.Assert(components.Length != 0);
			Debug.Assert(components.Length <= 2);
			int[] array = new int[2];
			int num = GetByteSize(addr.Size);
			if (num == 0)
			{
				num = 1;
			}
			array[0] = components[0] * num;
			if (components.Length == 2)
			{
				array[0] *= 2;
				array[1] = components[1] % 8;
				array[0] += components[1] / 8;
			}
			else
			{
				array[1] = 0;
			}
			return array;
		}

		public override int[] GetComponentsNative(DirectVariableSize size, int[] original)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Debug.Assert(original.Length == 2, "Expected a normalized representation of the components");
			int byteSize = GetByteSize(size);
			int[] result;
			if (byteSize == 0)
			{
				result = new int[2]
				{
					original[0] / 2,
					original[1] + original[0] % 2 * 8
				};
			}
			else
			{
				Debug.Assert(original[0] % byteSize == 0);
				result = new int[1] { original[0] / byteSize };
			}
			return result;
		}
	}
}
