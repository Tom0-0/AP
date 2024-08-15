using System;
using System.Runtime.CompilerServices;
using System.Threading;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.DeviceObject
{
	public class DataElementOnlineVarRef : IOnlineVarRef2, IOnlineVarRef
	{
		private class ErrorVarRef : IOnlineVarRef2, IOnlineVarRef
		{
			private object _tag;

			private IExpression _expression;

			private DateTime _timestamp;

			private string _stMessage;

			public object Tag
			{
				get
				{
					return _tag;
				}
				set
				{
					_tag = value;
				}
			}

			public IExpression Expression => _expression;

			public DateTime Timestamp => _timestamp;

			public VarRefState State => (VarRefState)2;

			public bool Forced => false;

			public object Value
			{
				get
				{
					throw new InvalidOperationException("<Invalid VarRefState>");
				}
			}

			public byte[] RawValue
			{
				get
				{
					throw new InvalidOperationException("<Invalid VarRefState>");
				}
			}

			public object PreparedValue
			{
				get
				{
					throw new InvalidOperationException("<Invalid VarRefState>");
				}
				set
				{
					throw new InvalidOperationException("<Invalid VarRefState>");
				}
			}

			public byte[] PreparedRawValue
			{
				get
				{
					throw new InvalidOperationException("<Invalid VarRefState>");
				}
			}

			public event OnlineVarRefEventHandler Changed;

			internal ErrorVarRef(IExpression exp, string stMessage)
			{
				_expression = exp;
				_timestamp = DateTime.Now;
				_stMessage = stMessage;
			}

			public void SuspendMonitoring()
			{
			}

			public void ResumeMonitoring()
			{
			}

			public string GetStateMessage()
			{
				return _stMessage;
			}

			public void Release()
			{
			}

			protected virtual void RaiseChanged()
			{
				if ((object)this.Changed != null)
				{
					this.Changed.Invoke((IOnlineVarRef)(object)this);
				}
			}
		}

		private IOnlineVarRef2 _onlineVarRef;

		private int _nProjectHandle = -1;

		private Guid _guidObject;

		private int _nConnectorId = -9999;

		private long _lParameterId = 281474976710655L;

		private int _nBitOffset = -1;

		private string _stType = "";

		private object _tag;

		private Guid _guidHost = Guid.Empty;

		private bool _bClientControlled;

		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		public IExpression Expression => ((IOnlineVarRef)_onlineVarRef).Expression;

		public DateTime Timestamp => ((IOnlineVarRef)_onlineVarRef).Timestamp;

		public VarRefState State => ((IOnlineVarRef)_onlineVarRef).State;

		public bool Forced => ((IOnlineVarRef)_onlineVarRef).Forced;

		public object Value => ((IOnlineVarRef)_onlineVarRef).Value;

		public byte[] RawValue => ((IOnlineVarRef)_onlineVarRef).RawValue;

		public object PreparedValue
		{
			get
			{
				return ((IOnlineVarRef)_onlineVarRef).PreparedValue;
			}
			set
			{
				if (_onlineVarRef != null)
				{
					((IOnlineVarRef)_onlineVarRef).PreparedValue=(value);
				}
			}
		}

		public byte[] PreparedRawValue => ((IOnlineVarRef)_onlineVarRef).PreparedRawValue;

		public event OnlineVarRefEventHandler Changed;

		internal DataElementOnlineVarRef(int nProjectHandle, Guid guidObject, int nConnectorId, long lParameterId, int nBitOffset, string stType, bool bClientControlled)
		{
			_nProjectHandle = nProjectHandle;
			_guidObject = guidObject;
			_nConnectorId = nConnectorId;
			_lParameterId = lParameterId;
			_nBitOffset = nBitOffset;
			_stType = stType;
			_bClientControlled = bClientControlled;
			_guidHost = UpdateOnlineVarRefInternal();
		}

		public void SuspendMonitoring()
		{
			((IOnlineVarRef)_onlineVarRef).SuspendMonitoring();
		}

		public void ResumeMonitoring()
		{
			if (_onlineVarRef != null)
			{
				((IOnlineVarRef)_onlineVarRef).ResumeMonitoring();
			}
		}

		public string GetStateMessage()
		{
			return ((IOnlineVarRef)_onlineVarRef).GetStateMessage();
		}

		public void Release()
		{
			if (_onlineVarRef != null)
			{
				((IOnlineVarRef)_onlineVarRef).Release();
				_onlineVarRef = null;
			}
			DeviceObjectHelper.OvrManager.Remove(this, _guidHost);
		}

		public override int GetHashCode()
		{
			return _nProjectHandle ^ _guidObject.GetHashCode() ^ _lParameterId.GetHashCode() ^ _nBitOffset;
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public void UpdateOnlineVarRef()
		{
			try
			{
				Guid guid = UpdateOnlineVarRefInternal();
				if (_guidHost != guid)
				{
					DeviceObjectHelper.OvrManager.Remove(this, _guidHost);
					DeviceObjectHelper.OvrManager.Add(this, _guidHost);
					_guidHost = guid;
				}
			}
			catch (Exception ex)
			{
				_onlineVarRef = (IOnlineVarRef2)(object)new ErrorVarRef(null, ex.Message);
			}
		}

		private Guid UpdateOnlineVarRefInternal()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Expected O, but got Unknown
			if (_onlineVarRef != null)
			{
				((IOnlineVarRef)_onlineVarRef).Changed-=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
				((IOnlineVarRef)_onlineVarRef).Release();
				_onlineVarRef = null;
			}
			((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _guidObject);
			IIoProvider ioProvider = GetIoProvider();
			Guid objectGuid = ((IObject)(ioProvider.GetHost() ?? throw new DeviceObjectException((DeviceObjectExeptionReason)10, "Unknown host"))).MetaObject.ObjectGuid;
			uint num = default(uint);
			uint num2 = default(uint);
			ioProvider.GetModuleAddress(out num, out num2);
			try
			{
				_onlineVarRef = APEnvironment.OnlineMgr.CreateParameterWatch(objectGuid, (int)num, (int)num2, _lParameterId, _nBitOffset, _stType, _bClientControlled);
				((IOnlineVarRef)_onlineVarRef).Changed+=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
			}
			catch
			{
			}
			OnOnlineVarRefChanged((IOnlineVarRef)(object)this);
			return objectGuid;
		}

		private void OnOnlineVarRefChanged(IOnlineVarRef varRef)
		{
			if ((object)this.Changed != null)
			{
				this.Changed.Invoke((IOnlineVarRef)(object)this);
			}
		}

		private IIoProvider GetIoProvider()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _guidObject);
			if (_nConnectorId < 0)
			{
				if (objectToRead.Object is SlotDeviceObject)
				{
					SlotDeviceObject slotDeviceObject = (SlotDeviceObject)(object)objectToRead.Object;
					if (slotDeviceObject.HasDevice)
					{
						return (IIoProvider)(object)slotDeviceObject.GetDevice();
					}
					throw new DeviceObjectException((DeviceObjectExeptionReason)19, "Invalid device");
				}
				return (IIoProvider)(object)(DeviceObject)(object)objectToRead.Object;
			}
			if (objectToRead.Object is IDeviceObject)
			{
				return (IIoProvider)(object)(ConnectorBase)(object)((IConnectorCollection2)((IDeviceObject)objectToRead.Object).Connectors).GetById(_nConnectorId);
			}
			if (objectToRead.Object is ExplicitConnector)
			{
				ConnectorBase obj = (ConnectorBase)(object)objectToRead.Object;
				if (obj.ConnectorId != _nConnectorId)
				{
					throw new DeviceObjectException((DeviceObjectExeptionReason)20, "Invalid connector id");
				}
				return (IIoProvider)(object)obj;
			}
			throw new DeviceObjectException((DeviceObjectExeptionReason)21, "Invalid object type " + ((object)objectToRead.Object).GetType().FullName);
		}
	}
}
