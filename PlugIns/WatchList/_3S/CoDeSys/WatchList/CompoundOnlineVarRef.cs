using System;
using System.Runtime.CompilerServices;
using System.Threading;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.WatchList
{
	internal class CompoundOnlineVarRef : IOnlineVarRef
	{
		public DateTime Timestamp => DateTime.Now;

		public VarRefState State => (VarRefState)0;

		public bool Forced => false;

		public object PreparedValue
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public object PreparedValueForReleaseForce => null;

		public object Value => null;

		public byte[] RawValue => null;

		public byte[] PreparedRawValue => null;

		public IExpression Expression => null;

		public event OnlineVarRefEventHandler Changed;
		

		public void Release()
		{
		}

		public void ResumeMonitoring()
		{
		}

		public void SuspendMonitoring()
		{
		}

		public string GetStateMessage()
		{
			return "";
		}

		protected virtual void OnChanged(IOnlineVarRef varRef)
		{
			if (this.Changed != null)
			{
				this.Changed(varRef);
			}
		}
	}
}
