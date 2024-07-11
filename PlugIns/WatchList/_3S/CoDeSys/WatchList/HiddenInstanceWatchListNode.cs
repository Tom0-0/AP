using System;

namespace _3S.CoDeSys.WatchList
{
	internal sealed class HiddenInstanceWatchListNode : WatchListNode
	{
		internal HiddenInstanceWatchListNode(WatchListNode parentNode, bool bShowCommentColumn, int nProjectHandle, Guid guidObject)
			: base(parentNode, string.Empty, string.Empty, bShowCommentColumn, nProjectHandle, guidObject, Guid.Empty)
		{
		}

		public override object GetValue(int nColumnIndex)
		{
			if (nColumnIndex == _model.COLUMN_EXPRESSION)
			{
				return new ExpressionData(string.Empty, Strings.HiddenInstance, WatchListNodeUtils.IMAGE_VAR);
			}
			if (nColumnIndex == _model.COLUMN_TYPE)
			{
				return string.Empty;
			}
			return base.GetValue(nColumnIndex);
		}
	}
}
