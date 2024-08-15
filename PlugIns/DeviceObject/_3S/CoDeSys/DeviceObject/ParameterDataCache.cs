using System.Collections.Generic;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ParameterDataCache
	{
		private static LDictionary<StringRef, StringRef> _stringRefs = new LDictionary<StringRef, StringRef>((IEqualityComparer<StringRef>)new StringRef.EqualityComparer());

		private static LDictionary<string, CustomItem> _htCustomItems = new LDictionary<string, CustomItem>();

		internal static StringRef AddStringRef(StringRef newstring)
		{
			StringRef result = null;
			if (newstring != null)
			{
				if (_stringRefs.TryGetValue(newstring, out result))
				{
					return result;
				}
				_stringRefs[newstring]= newstring;
				return newstring;
			}
			return null;
		}

		internal static CustomItem AddCustomItem(CustomItem item)
		{
			if (item != null)
			{
				string text = item.Name + item.Data;
				if (!_htCustomItems.ContainsKey(text))
				{
					_htCustomItems.Add(text, item);
				}
				return _htCustomItems[text];
			}
			return null;
		}
	}
}
