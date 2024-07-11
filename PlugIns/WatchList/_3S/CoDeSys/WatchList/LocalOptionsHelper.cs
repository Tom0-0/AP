using System;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.WatchList
{
	internal abstract class LocalOptionsHelper
	{
		private static readonly string WATCH_EXPRESSIONS = "WatchExpressions";

		private static readonly string EXPANDED_EXPRESSIONS = "ExpandedExpressions";

		private static readonly string SUB_KEY = "{339D6482-5AF0-4fe5-94C3-DD0657F146E4}";

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).CreateSubKey(SUB_KEY);

		public static void SaveWatchExpressions(Guid persistentGuid, string watchExpressions, string expandedExpressions, bool forceSaveExpandedExpressions)
		{
			if (watchExpressions == null)
			{
				throw new ArgumentNullException("watchExpressions");
			}
			if (expandedExpressions == null)
			{
				throw new ArgumentNullException("expandedExpressions");
			}
			IOptionKey val = OptionKey.CreateSubKey(persistentGuid.ToString());
			val[WATCH_EXPRESSIONS] = (object)watchExpressions;
			if (!string.IsNullOrWhiteSpace(expandedExpressions) || forceSaveExpandedExpressions)
			{
				val[EXPANDED_EXPRESSIONS] = (object)expandedExpressions;
			}
		}

		public static void LoadWatchExpressions(Guid persistentGuid, out string watchExpressions, out string expandedExpressions)
		{
			watchExpressions = null;
			expandedExpressions = null;
			if (OptionKey.HasSubKey(persistentGuid.ToString()))
			{
				IOptionKey val = OptionKey.OpenSubKey(persistentGuid.ToString());
				if (val.HasValue(WATCH_EXPRESSIONS, typeof(string)))
				{
					watchExpressions = (string)val[WATCH_EXPRESSIONS];
				}
				if (val.HasValue(EXPANDED_EXPRESSIONS, typeof(string)))
				{
					expandedExpressions = (string)val[EXPANDED_EXPRESSIONS];
				}
			}
		}
	}
}
