#define DEBUG
using System;
using System.Diagnostics;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ApplicationController
	{
		private DeviceController _devicecontroller;

		private LDictionary<ModuleKey, IOnlineVarRef2> _htModuleKeyToVarRef = new LDictionary<ModuleKey, IOnlineVarRef2>();

		private Guid _guidApplication;

		private int _nProjectHandle = -1;

		private bool _bInitialized;

		public bool IsInitialized => _bInitialized;

		public Guid ApplicationGuid => _guidApplication;

		public int ProjectHandle => _nProjectHandle;

		public LDictionary<ModuleKey, IOnlineVarRef2> ModuleKeyToVarRef => _htModuleKeyToVarRef;

		public ApplicationController(Guid guidApplication, int nProjectHandle, DeviceController devicecontroller)
		{
			_guidApplication = guidApplication;
			_nProjectHandle = nProjectHandle;
			_devicecontroller = devicecontroller;
		}

		public void Clear()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				foreach (IOnlineVarRef2 onlineVarRef in this._htModuleKeyToVarRef.Values)
				{
					onlineVarRef.Release();
				}
			}
			catch
			{
			}
			_htModuleKeyToVarRef.Clear();
		}

		public void Rebuild()
		{
			Clear();
			DeviceObject deviceObject = GetDeviceObject();
			if (deviceObject != null && !deviceObject.NoIoDownload)
			{
				int nModuleIndex = 0;
				if (((ILanguageModelManager2)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(_guidApplication) != null && CreateVarRefs((IIoProvider)(object)deviceObject, ref nModuleIndex))
				{
					_bInitialized = true;
				}
			}
		}

		public IOnlineVarRef GetOnlineVarRef(Guid guidObject, int nConnectorId)
		{
			ModuleKey moduleKey = new ModuleKey(guidObject, nConnectorId);
			IOnlineVarRef2 result = default(IOnlineVarRef2);
			_htModuleKeyToVarRef.TryGetValue(moduleKey, out result);
			return (IOnlineVarRef)(object)result;
		}

		private bool CreateVarRefs(IIoProvider provider, ref int nModuleIndex)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			try
			{
				IVarRef varReference = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetVarReference(_guidApplication, "modulelist[" + nModuleIndex + "].dwFlags");
				if (varReference.AddressInfo != null)
				{
					IOnlineVarRef2 val = (IOnlineVarRef2)((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(varReference);
					ModuleKey moduleKey = new ModuleKey(nConnectorId: (!(provider is IConnector)) ? (-1) : ((IConnector)provider).ConnectorId, guidObject: provider.GetMetaObject().ObjectGuid);
					val.Tag=((object)moduleKey);
					((IOnlineVarRef)val).Changed+=(new OnlineVarRefEventHandler(OnOvrChanged));
					_htModuleKeyToVarRef[moduleKey]= val;
				}
			}
			catch (Exception value)
			{
				Debug.WriteLine(value);
				return false;
			}
			nModuleIndex++;
			IIoProvider[] children = provider.Children;
			foreach (IIoProvider provider2 in children)
			{
				if (!CreateVarRefs(provider2, ref nModuleIndex))
				{
					return false;
				}
			}
			return true;
		}

		private void OnOvrChanged(IOnlineVarRef ovr)
		{
			try
			{
				IOnlineVarRef2 val = (IOnlineVarRef2)(object)((ovr is IOnlineVarRef2) ? ovr : null);
				if (val != null)
				{
					ModuleKey moduleKey = (ModuleKey)val.Tag;
					_devicecontroller.RaiseModuleStateChanged(_nProjectHandle, moduleKey.ObjectGuid, moduleKey.ConnectorId, _guidApplication);
				}
			}
			catch
			{
			}
		}

		private DeviceObject GetDeviceObject()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, _guidApplication))
			{
				return null;
			}
			IOnlineApplicationObject val = (IOnlineApplicationObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _guidApplication).Object;
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, val.DeviceGuid);
			DeviceObject deviceObject = ((!(objectToRead.Object is SlotDeviceObject)) ? ((DeviceObject)(object)objectToRead.Object) : ((SlotDeviceObject)(object)objectToRead.Object).GetDevice());
			if (val.DeviceGuid == deviceObject.MetaObject.ObjectGuid)
			{
				return deviceObject;
			}
			return null;
		}
	}
}
