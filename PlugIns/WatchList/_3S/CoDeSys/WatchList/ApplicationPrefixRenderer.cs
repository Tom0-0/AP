using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.WatchList
{
	public class ApplicationPrefixRenderer : LabelTreeTableViewRenderer
	{
		private static ITreeTableViewRenderer s_singleton = (ITreeTableViewRenderer)(object)new ApplicationPrefixRenderer();

		public static ITreeTableViewRenderer Singleton => s_singleton;

		public ApplicationPrefixRenderer()
			: base(false)
		{
		}

		protected override string ConvertToString(object value)
		{
			ApplicationPrefixItem applicationPrefixItem = value as ApplicationPrefixItem;
			if (applicationPrefixItem != null)
			{
				return applicationPrefixItem.ApplicationPrefix;
			}
			return base.ConvertToString(value);
		}
	}
}
