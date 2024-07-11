using System;
using _3S.CoDeSys.LanguageModelUtilities;

namespace _3S.CoDeSys.WatchList
{
	internal sealed class AddressWatchListNode : ReferencedInstanceWatchListNode
	{
		private string _stDereferencedPointer;

		internal string DereferencedPointer => _stDereferencedPointer;

		internal AddressWatchListNode(WatchListNode parentNode, IAddressWatchVarDescription addressWatchVarDescription, string stInstancePath, string stExpression, bool bShowCommentColumn, int nProjectHandle, Guid guidObject, Guid guidApplication)
			: base(parentNode, (IReferencedInstanceWatchVarDescription)(object)addressWatchVarDescription, stInstancePath, stExpression, bShowCommentColumn, nProjectHandle, guidObject, guidApplication)
		{
			_stDereferencedPointer = addressWatchVarDescription.DereferencedPointer;
		}

		public override object GetValue(int nColumnIndex)
		{
			if (nColumnIndex == _model.COLUMN_EXPRESSION)
			{
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
