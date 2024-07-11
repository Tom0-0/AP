using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.WatchList
{
	internal class WatchPointRenderer : LabelTreeTableViewRenderer
	{
		private static ITreeTableViewRenderer s_singleton = (ITreeTableViewRenderer)(object)new WatchPointRenderer();

		public static ITreeTableViewRenderer Singleton => s_singleton;

		public WatchPointRenderer()
			: base(false)
		{
		}

		protected override string ConvertToString(object value)
		{
			WatchPointItem watchPointItem = value as WatchPointItem;
			if (watchPointItem != null)
			{
				return watchPointItem.ToString();
			}
			return base.ConvertToString(value);
		}
	}
}
