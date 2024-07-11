using System;
using _3S.CoDeSys.LanguageModelUtilities;

namespace _3S.CoDeSys.WatchList
{
	internal sealed class DynamicInterfaceInstanceWatchListNode : ReferencedInstanceWatchListNode
	{
		public override string NodeLabel => Strings.DynamicInstance;

		internal DynamicInterfaceInstanceWatchListNode(WatchListNode parentNode, IDynamicInterfaceInstanceWatchVarDescription dynamicInterfaceInstanceWatchVarDescription, string stInstancePath, string stExpression, bool bShowCommentColumn, int nProjectHandle, Guid guidObject, Guid guidApplication)
			: base(parentNode, (IReferencedInstanceWatchVarDescription)(object)dynamicInterfaceInstanceWatchVarDescription, stInstancePath, stExpression, bShowCommentColumn, nProjectHandle, guidObject, guidApplication)
		{
		}

		public override object GetValue(int nColumnIndex)
		{
			if (nColumnIndex == _model.COLUMN_EXPRESSION)
			{
				return new ExpressionData(Strings.DynamicInstance, Strings.DynamicInstance, WatchListNodeUtils.IMAGE_VAR);
			}
			if (nColumnIndex == _model.COLUMN_TYPE)
			{
				return _referencedInstanceWatchVarDescription.InstanceType;
			}
			if (nColumnIndex == _model.COLUMN_VALUE)
			{
				string pointer = GetConverter().GetPointer(Common.ConvertToSmallestPointer(_ulAddressInstance));
				return new TextValueData(this, pointer, bForced: false);
			}
			return base.GetValue(nColumnIndex);
		}
	}
}
