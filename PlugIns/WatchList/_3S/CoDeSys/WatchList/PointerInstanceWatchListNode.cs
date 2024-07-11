using System;
using _3S.CoDeSys.LanguageModelUtilities;

namespace _3S.CoDeSys.WatchList
{
	internal sealed class PointerInstanceWatchListNode : ReferencedInstanceWatchListNode
	{
		private string _stDereferencedPointer;

		internal string DereferencedPointer => _stDereferencedPointer;

		public override string NodeLabel => _stDereferencedPointer;

		internal PointerInstanceWatchListNode(WatchListNode parentNode, IPointerInstanceWatchVarDescription pointerInstanceWatchVarDescription, string stInstancePath, string stExpression, bool bShowCommentColumn, int nProjectHandle, Guid guidObject, Guid guidApplication)
			: base(parentNode, (IReferencedInstanceWatchVarDescription)(object)pointerInstanceWatchVarDescription, stInstancePath, stExpression, bShowCommentColumn, nProjectHandle, guidObject, guidApplication)
		{
			_stDereferencedPointer = pointerInstanceWatchVarDescription.DereferencedPointer;
		}

		public override object GetValue(int nColumnIndex)
		{
			if (nColumnIndex == _model.COLUMN_EXPRESSION)
			{
				if (_parentNode != null && _parentNode.ImplicitInstancePointer)
				{
					return new ExpressionData(_stDereferencedPointer, "THIS^", WatchListNodeUtils.IMAGE_VAR);
				}
				return new ExpressionData(_stDereferencedPointer, _stDereferencedPointer, WatchListNodeUtils.IMAGE_VAR);
			}
			if (nColumnIndex == _model.COLUMN_TYPE)
			{
				return _referencedInstanceWatchVarDescription.InstanceType;
			}
			return base.GetValue(nColumnIndex);
		}
	}
}
