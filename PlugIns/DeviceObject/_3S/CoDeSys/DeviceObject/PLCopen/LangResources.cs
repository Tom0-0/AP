using _3S.CoDeSys.DeviceObject.DeviceDescriptionBuilder;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class LangResources
	{
		private IBuilder3 _builder;

		private LDictionary<string, IBuilderStringRef> _res = new LDictionary<string, IBuilderStringRef>();

		internal LangResources(IBuilder3 builder)
		{
			_builder = builder;
		}

		internal IBuilderStringRef Get(string key, string defaultValue)
		{
			if (_res.ContainsKey(key))
			{
				return _res[key];
			}
			return Create(key, defaultValue);
		}

		internal IBuilderStringRef Create(string key, string defaultValue)
		{
			IBuilderStringRef val = ((IBuilder)_builder).AddStringRef("", key, defaultValue);
			_res.Add(key, val);
			return val;
		}
	}
}
