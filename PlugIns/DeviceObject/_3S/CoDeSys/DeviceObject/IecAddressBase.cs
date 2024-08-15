using System.Text;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	public abstract class IecAddressBase : IDirectVariable
	{
		protected DirectVariableSize _varSize;

		protected DirectVariableLocation _location;

		protected int[] _components;

		public DirectVariableSize Size
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _varSize;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_varSize = value;
			}
		}

		public DirectVariableLocation Location
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _location;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_location = value;
			}
		}

		public int[] Components
		{
			get
			{
				return _components;
			}
			set
			{
				_components = value;
			}
		}

		public bool Incomplete => false;

		public bool IsEqual(IDirectVariable rhs)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			int[] components = rhs.Components;
			if (rhs.Location != Location)
			{
				return false;
			}
			if (rhs.Size != Size)
			{
				return false;
			}
			if (components.Length != _components.Length)
			{
				return false;
			}
			for (int i = 0; i < _components.Length; i++)
			{
				if (components[i] != _components[i])
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected I4, but got Unknown
			StringBuilder stringBuilder = new StringBuilder("%");
			DirectVariableLocation location = _location;
			switch ((int)location - 1)
			{
			case 0:
				stringBuilder.Append("I");
				break;
			case 1:
				stringBuilder.Append("Q");
				break;
			case 2:
				stringBuilder.Append("M");
				break;
			default:
				return "";
			}
			stringBuilder.Append(GetSizeString());
			for (int i = 0; i < _components.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append('.');
				}
				stringBuilder.Append(_components[i]);
			}
			return stringBuilder.ToString();
		}

		protected string GetSizeString()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected I4, but got Unknown
			DirectVariableSize varSize = _varSize;
			return ((int)varSize - 1) switch
			{
				0 => "X", 
				1 => "B", 
				2 => "W", 
				3 => "D", 
				4 => "L", 
				_ => "?", 
			};
		}
	}
}
