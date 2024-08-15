#define DEBUG
using System.Diagnostics;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	public class ByteAddressFlatAdressAssignmentStrategy : FlatAdressAssignmentStrategy
	{
		public ByteAddressFlatAdressAssignmentStrategy(int iMinStructureGranularity)
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
			if (num > 1)
			{
				num = 1;
			}
			array[0] = components[0] * num;
			if (components.Length == 2)
			{
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
			int num = GetByteSize(size);
			if (num > 1)
			{
				num = 1;
			}
			int[] result;
			if (num == 0)
			{
				result = new int[2]
				{
					original[0],
					original[1]
				};
			}
			else
			{
				Debug.Assert(original[0] % num == 0);
				result = new int[1] { original[0] / num };
			}
			return result;
		}
	}
}
