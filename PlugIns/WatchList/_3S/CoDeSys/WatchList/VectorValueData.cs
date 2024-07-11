using System;
using System.Globalization;
using System.Text;

namespace _3S.CoDeSys.WatchList
{
	internal class VectorValueData : ValueData
	{
		private object _value;

		public override object Value => _value;

		public override string Text
		{
			get
			{
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("[");
					object[] array = _value as object[];
					for (int i = 0; i < array.Length; i++)
					{
						string value = ((array[i] is float) ? ((float)array[i]).ToString("R", NumberFormatInfo.InvariantInfo) : ((!(array[i] is double)) ? array[i].ToString() : ((double)array[i]).ToString(NumberFormatInfo.InvariantInfo)));
						stringBuilder.Append(value);
						if (i + 1 < array.Length)
						{
							stringBuilder.Append(", ");
						}
					}
					stringBuilder.Append("]");
					return stringBuilder.ToString();
				}
				catch (Exception ex)
				{
					if (_value is string)
					{
						return (string)_value;
					}
					return $"{ex.Message}";
				}
			}
		}

		public override bool Toggleable => false;

		public override bool Constant => false;

		public override bool Forced => false;

		public override bool IsEnum => false;

		public VectorValueData(WatchListNode node, object value)
			: base(node)
		{
			_value = value;
		}
	}
}
