namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class Converter
	{
		internal string CreateString(IStringRef stringRef)
		{
			if (stringRef != null && !string.IsNullOrEmpty(stringRef.Default))
			{
				return stringRef.Default;
			}
			return null;
		}

		internal string CreateString(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}
	}
}
