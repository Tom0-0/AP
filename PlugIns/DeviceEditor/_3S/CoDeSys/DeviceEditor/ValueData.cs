namespace _3S.CoDeSys.DeviceEditor
{
	internal abstract class ValueData
	{
		public abstract object Value { get; }

		public abstract string Text { get; }

		public abstract bool Toggleable { get; }

		public abstract bool Constant { get; }

		public abstract bool Forced { get; }

		public override string ToString()
		{
			return Text;
		}
	}
}
