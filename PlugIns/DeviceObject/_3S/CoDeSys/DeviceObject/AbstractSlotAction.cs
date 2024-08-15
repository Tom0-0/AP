using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class AbstractSlotAction
	{
		private int _nProjectHandle;

		private Guid _guidSlot;

		private DeviceObject _device;

		private string _stDeviceName;

		protected void Init(int nProjectHandle, Guid guidSlot, DeviceObject device, string stDeviceName)
		{
			_nProjectHandle = nProjectHandle;
			_guidSlot = guidSlot;
			_device = device;
			_stDeviceName = stDeviceName;
		}

		protected object Plug()
		{
			if (_device != null)
			{
				DeviceObject deviceObject = (DeviceObject)((GenericObject)_device).Clone();
				deviceObject.PreparePaste();
				IMetaObject val = null;
				((IObjectManager)APEnvironment.ObjectMgr).RenameObject(_nProjectHandle, _guidSlot, _stDeviceName);
				try
				{
					val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, _guidSlot);
					((SlotDeviceObject)(object)val.Object).PlugDevice(deviceObject);
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
					val = null;
				}
				catch
				{
					if (val != null)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
					}
					throw;
				}
			}
			return _guidSlot;
		}

		protected object Unplug()
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected O, but got Unknown
			if (_device != null)
			{
				string text = Strings.EmptySlotName;
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, _guidSlot);
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, metaObjectStub.ParentObjectGuid))
				{
					IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, metaObjectStub.ParentObjectGuid).Object;
					IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
					if (val != null)
					{
						foreach (IConnector item in (IEnumerable)val.Connectors)
						{
							foreach (IAdapter item2 in (IEnumerable)item.Adapters)
							{
								IAdapter val2 = item2;
								int num = Array.IndexOf(val2.Modules, _guidSlot);
								if (!(val2 is SlotAdapter) || num < 0)
								{
									continue;
								}
								string slotName = (val2 as SlotAdapter).GetSlotName(num);
								if (!string.IsNullOrEmpty(slotName))
								{
									if (!slotName.StartsWith("<"))
									{
										slotName = DeviceObjectHelper.BuildIecIdentifier(slotName);
										IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(_nProjectHandle, _guidSlot);
										text = DeviceObjectHelper.CreateUniqueIdentifier(_nProjectHandle, slotName, Guid.Empty, hostStub, bCheckAll: false, bCheckLogical: false);
									}
									else
									{
										text = slotName;
									}
								}
							}
						}
					}
				}
				((IObjectManager)APEnvironment.ObjectMgr).RenameObject(_nProjectHandle, _guidSlot, text);
				IMetaObject val3 = null;
				try
				{
					val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, _guidSlot);
					((SlotDeviceObject)(object)val3.Object).UnplugDevice();
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
				}
				catch
				{
					if (val3 != null)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, false, (object)null);
					}
					throw;
				}
			}
			return _guidSlot;
		}
	}
}
