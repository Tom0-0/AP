using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class CompileOptions
	{
		internal static readonly string MAX_COMPILER_WARNINGS = "MaxCompilerWarnings";

		internal static readonly string SUB_KEY_COMPILE = "{E709B08B-B6E4-4966-8EED-D793A13114C6}";

		internal static int MaxCompilerWarnings => GetCompileOption(MAX_COMPILER_WARNINGS, 100);

		internal static int GetCompileOption(string stKey, int nDefault)
		{
			IOptionKey projectOptionKey = GetProjectOptionKey();
			if (projectOptionKey != null && projectOptionKey.HasValue(stKey, typeof(int)))
			{
				return (int)projectOptionKey[stKey];
			}
			return nDefault;
		}

		private static IOptionKey GetProjectOptionKey()
		{
			return APEnvironment.OptionStorage.GetRootKey((OptionRoot)0).OpenSubKey(SUB_KEY_COMPILE);
		}
	}
}
