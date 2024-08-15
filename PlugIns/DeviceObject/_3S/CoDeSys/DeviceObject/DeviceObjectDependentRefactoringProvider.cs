using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{760284e8-db88-4386-9017-6c992b44069d}")]
	public class DeviceObjectDependentRefactoringProvider : IProvideDependentRefactorings
	{
		private static void GetRefactoringFbInstances(IDriverInfo driverInfo, LList<FBInstance> liFbInstances)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			foreach (IRequiredLib item in (IEnumerable)driverInfo.RequiredLibs)
			{
				foreach (FBInstance item2 in ((IEnumerable)item.FbInstances).OfType<FBInstance>())
				{
					if (item2.BaseName.Contains("$(DeviceName)"))
					{
						liFbInstances.Add(item2);
					}
				}
			}
		}

		public void ProvideDependentRefactorings(RefactoringCollectDependingRefactoringsEventArgs args)
		{
			HashSet<Tuple<Guid, string>> hashSet = new HashSet<Tuple<Guid, string>>();
			foreach (IRefactoringRenameVariableOperation item3 in ((IEnumerable)((AbstractRefactoringEventArgs)args).Operation).OfType<IRefactoringRenameVariableOperation>())
			{
				Tuple<Guid, string> item = Tuple.Create(((IRefactoringRenameOperation)item3).Signature.ObjectGuid, ((IRefactoringRenameOperation)item3).OldName);
				hashSet.Add(item);
			}
			foreach (IRefactoringRenameLanguageModelProvidingObjectOperation item4 in ((IEnumerable)((AbstractRefactoringEventArgs)args).Operation).OfType<IRefactoringRenameLanguageModelProvidingObjectOperation>().ToList())
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(item4.ProjectHandle, item4.ObjectGuid);
				if (!typeof(DeviceObjectBase).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					continue;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(item4.ProjectHandle, item4.ObjectGuid);
				DeviceObjectBase baseObject = objectToRead.Object as DeviceObjectBase;
				if (DeviceObjectHelper.IsExcludedFromBuild(objectToRead))
				{
					continue;
				}
				LList<FBInstance> refactoringFbInstances = GetRefactoringFbInstances(baseObject);
				if (refactoringFbInstances.Count == 0)
				{
					continue;
				}
				IMetaObjectStub plcDevice = APEnvironment.DeviceMgr.GetPlcDevice(objectToRead.ProjectHandle, objectToRead.ObjectGuid);
				if (plcDevice == null)
				{
					continue;
				}
				DeviceObject deviceObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(plcDevice.ProjectHandle, plcDevice.ObjectGuid).Object as DeviceObject;
				if (deviceObject == null)
				{
					continue;
				}
				Guid ioApplication = (deviceObject.DriverInfo as DriverInfo).IoApplication;
				if (ioApplication == Guid.Empty || !((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(objectToRead.ProjectHandle, ioApplication))
				{
					continue;
				}
				Guid lmIoConfigGlobals = deviceObject.LmIoConfigGlobals;
				if (lmIoConfigGlobals == Guid.Empty)
				{
					continue;
				}
				string text = string.Format(Strings.Refactoring_DependendOperationReason, ((IRefactoringRenameOperation)item4).OldName);
				foreach (FBInstance item5 in refactoringFbInstances)
				{
					if (!string.IsNullOrEmpty(item5.Instance.Variable))
					{
						Tuple<Guid, string> item2 = Tuple.Create(lmIoConfigGlobals, item5.Instance.Variable);
						if (!hashSet.Contains(item2))
						{
							string baseName = DeviceObjectHelper.GetBaseName(item5.BaseName, ((IRefactoringRenameOperation)item4).NewName);
							IRefactoringRenameVariableOperation val = APEnvironment.RefactoringServiceOrNull.Factories.CreateRenameVariableOperation(lmIoConfigGlobals, item5.Instance.Variable, text, baseName);
							args.AddOperation((IRefactoringOperation)(object)val);
							hashSet.Add(item2);
						}
					}
				}
			}
		}

		private static LList<FBInstance> GetRefactoringFbInstances(DeviceObjectBase baseObject)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			LList<FBInstance> val = new LList<FBInstance>();
			DeviceObject deviceObject = baseObject as DeviceObject;
			if (deviceObject != null)
			{
				GetRefactoringFbInstances(deviceObject.DriverInfo, val);
			}
			SlotDeviceObject slotDeviceObject = baseObject as SlotDeviceObject;
			if (slotDeviceObject != null)
			{
				GetRefactoringFbInstances(slotDeviceObject.DriverInfo, val);
			}
			if (baseObject.Connectors != null)
			{
				foreach (IConnector2 item in (IEnumerable)baseObject.Connectors)
				{
					GetRefactoringFbInstances(item.DriverInfo, val);
				}
				return val;
			}
			return val;
		}
	}
}
