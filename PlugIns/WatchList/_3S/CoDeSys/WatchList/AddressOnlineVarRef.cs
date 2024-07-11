using System;
using System.Runtime.CompilerServices;
using System.Threading;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.WatchList
{
	internal sealed class AddressOnlineVarRef : IOnlineVarRef2, IOnlineVarRef
	{
		private ulong _ulAddress;

		private VarRefState _eState = (VarRefState)5;

		private object _monat;

		private WatchListNode _owningNode;

		private IOnlineVarRef _onlineVarRefReference;

		private bool _bMonitoringSuspended;

		public DateTime Timestamp => DateTime.Now;

		public VarRefState State => _eState;

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

		public object Value => _ulAddress;

		public byte[] RawValue => null;

		public byte[] PreparedRawValue => null;

		public IExpression Expression => null;

		public object Tag
		{
			get
			{
				return _monat;
			}
			set
			{
				_monat = value;
			}
		}

		public event OnlineVarRefEventHandler Changed;
		//{
			//[CompilerGenerated]
			//add
			//{
			//	//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//	//IL_0016: Expected O, but got Unknown
			//	OnlineVarRefEventHandler val = this.Changed;
			//	OnlineVarRefEventHandler val2;
			//	do
			//	{
			//		val2 = val;
			//		OnlineVarRefEventHandler value2 = (OnlineVarRefEventHandler)Delegate.Combine((Delegate)(object)val2, (Delegate)(object)value);
			//		val = Interlocked.CompareExchange(ref System.Runtime.CompilerServices.Unsafe.As<OnlineVarRefEventHandler, OnlineVarRefEventHandler>(ref this.Changed), value2, val2);
			//	}
			//	while (val != val2);
			//}
			//[CompilerGenerated]
			//remove
			//{
			//	//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//	//IL_0016: Expected O, but got Unknown
			//	OnlineVarRefEventHandler val = this.Changed;
			//	OnlineVarRefEventHandler val2;
			//	do
			//	{
			//		val2 = val;
			//		OnlineVarRefEventHandler value2 = (OnlineVarRefEventHandler)Delegate.Remove((Delegate)(object)val2, (Delegate)(object)value);
			//		val = Interlocked.CompareExchange(ref System.Runtime.CompilerServices.Unsafe.As<OnlineVarRefEventHandler, OnlineVarRefEventHandler>(ref this.Changed), value2, val2);
			//	}
			//	while (val != val2);
			//}
		//}

		internal AddressOnlineVarRef(WatchListNode owningNode, Guid gdApplication)
		{
			_owningNode = owningNode;
			IAddressInfo addressInfo = owningNode.VarRef.AddressInfo;
			IAddressAddressInfo val = (IAddressAddressInfo)(object)((addressInfo is IAddressAddressInfo) ? addressInfo : null);
			if (val != null)
			{
				_ulAddress = val.Address;
				_eState = (VarRefState)0;
				CreateReferenceOnlineVarRef();
			}
		}

		private void CreateReferenceOnlineVarRef()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			_onlineVarRefReference = ((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(_owningNode.VarRef);
			_onlineVarRefReference.Changed+=(new OnlineVarRefEventHandler(OnOnlineVarRefReferenceChanged));
			if (!_bMonitoringSuspended)
			{
				_onlineVarRefReference.ResumeMonitoring();
			}
		}

		private void OnOnlineVarRefReferenceChanged(IOnlineVarRef onlineVarRef)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (onlineVarRef == _onlineVarRefReference && (int)onlineVarRef.State == 0)
			{
				_owningNode?.AddReferencedInstanceChildNode();
				_owningNode?.RaiseValueChanged();
				OnChanged(onlineVarRef);
			}
		}

		public void Release()
		{
			IOnlineVarRef onlineVarRefReference = _onlineVarRefReference;
			if (onlineVarRefReference != null)
			{
				onlineVarRefReference.Release();
			}
		}

		public void ResumeMonitoring()
		{
			try
			{
				IOnlineVarRef onlineVarRefReference = _onlineVarRefReference;
				if (onlineVarRefReference != null)
				{
					onlineVarRefReference.ResumeMonitoring();
				}
			}
			finally
			{
				_bMonitoringSuspended = false;
			}
		}

		public void SuspendMonitoring()
		{
			try
			{
				IOnlineVarRef onlineVarRefReference = _onlineVarRefReference;
				if (onlineVarRefReference != null)
				{
					onlineVarRefReference.SuspendMonitoring();
				}
			}
			finally
			{
				_bMonitoringSuspended = true;
			}
		}

		public string GetStateMessage()
		{
			return string.Empty;
		}

		private void OnChanged(IOnlineVarRef varRef)
		{
			if ((object)this.Changed != null)
			{
				this.Changed.Invoke(varRef);
			}
		}

		internal bool DetermineTypeIfPossible(out ICompiledType type)
		{
			IOnlineVarRef onlineVarRefReference = _onlineVarRefReference;
			object obj;
			if (onlineVarRefReference == null)
			{
				obj = null;
			}
			else
			{
				IExpression expression = onlineVarRefReference.Expression;
				obj = ((expression != null) ? expression.Type : null);
			}
			type = (ICompiledType)obj;
			return type != null;
		}
	}
}
