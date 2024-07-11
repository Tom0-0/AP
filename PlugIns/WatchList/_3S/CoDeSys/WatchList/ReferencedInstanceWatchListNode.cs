using System;
using _3S.CoDeSys.LanguageModelUtilities;

namespace _3S.CoDeSys.WatchList
{
	internal abstract class ReferencedInstanceWatchListNode : WatchListNode
	{
		protected ulong _ulAddressInstance;

		private ITemporaryWatchVarOnlineVarRef _ovrPointer;

		protected IReferencedInstanceWatchVarDescription _referencedInstanceWatchVarDescription;

		internal override bool CanWatch => true;

		internal ReferencedInstanceWatchListNode(WatchListNode parentNode, IReferencedInstanceWatchVarDescription referencedInstanceWatchVarDescription, string stInstancePath, string stExpression, bool bShowCommentColumn, int nProjectHandle, Guid guidObject, Guid guidApplication)
			: base(parentNode, stInstancePath, stExpression, bShowCommentColumn, nProjectHandle, guidObject, guidApplication)
		{
			_referencedInstanceWatchVarDescription = referencedInstanceWatchVarDescription;
			_ulAddressInstance = referencedInstanceWatchVarDescription.AddressInstance;
			_ovrPointer = APEnvironment.CreateTemporaryWatchVarOnlineVarRef();
			_ovrPointer.Initialize(guidApplication, referencedInstanceWatchVarDescription);
		}

		protected override void BeforeClear()
		{
			_ovrPointer.WriteAddress(0uL);
		}
	}
}
