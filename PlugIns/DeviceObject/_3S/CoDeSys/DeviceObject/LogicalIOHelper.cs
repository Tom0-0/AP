#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.ImplementationObject;
using _3S.CoDeSys.POUObject;
using _3S.CoDeSys.STObject;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.VarDeclObject;

namespace _3S.CoDeSys.DeviceObject
{
	public class LogicalIOHelper
	{
		public static readonly string stLogicalDeviceSection = "logical-devices";

		private static bool _bIsCheckingLogicalAppsInProgress = false;

		public static SortedList<Guid, string> GetLogicalApplications(int nProjectHandle)
		{
			SortedList<Guid, string> sortedList = new SortedList<Guid, string>();
			if (DeviceObjectHelper.LogicalDevices != null)
			{
				foreach (Guid deviceGuid in DeviceObjectHelper.LogicalDevices.DeviceGuids)
				{
					string stName;
					Guid logicalAppForDevice = GetLogicalAppForDevice(nProjectHandle, deviceGuid, out stName);
					if (logicalAppForDevice != Guid.Empty && !sortedList.ContainsKey(logicalAppForDevice))
					{
						sortedList.Add(logicalAppForDevice, stName);
					}
				}
			}
			if (DeviceObjectHelper.AdditionalModules != null && DeviceObjectHelper.AdditionalModules.DeviceGuids != null)
			{
				foreach (Guid deviceGuid2 in DeviceObjectHelper.AdditionalModules.DeviceGuids)
				{
					string stName2;
					Guid logicalAppForDevice2 = GetLogicalAppForDevice(nProjectHandle, deviceGuid2, out stName2);
					if (logicalAppForDevice2 != Guid.Empty && !sortedList.ContainsKey(logicalAppForDevice2))
					{
						sortedList.Add(logicalAppForDevice2, stName2);
					}
				}
				return sortedList;
			}
			return sortedList;
		}

		public static Guid GetLogicalAppForDevice(int nProjectHandle, Guid objectGuid, out string stName)
		{
			stName = string.Empty;
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid);
			while (metaObjectStub.ParentObjectGuid != Guid.Empty)
			{
				metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
				if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					stName = metaObjectStub.Name;
					return metaObjectStub.ObjectGuid;
				}
			}
			return Guid.Empty;
		}

		public static int GetProxyModuleIndex(IIoProvider ioProvider, Guid guidProxy)
		{
			int nModuleIndex = 0;
			GetProxyModuleIndex(ioProvider, guidProxy, ref nModuleIndex);
			return nModuleIndex;
		}

		public static bool GetProxyModuleIndex(IIoProvider ioProvider, Guid guidProxy, ref int nModuleIndex)
		{
			IMetaObject metaObject = ioProvider.GetMetaObject();
			if (metaObject != null && metaObject.ObjectGuid == guidProxy)
			{
				return true;
			}
			nModuleIndex++;
			IIoProvider[] children = ioProvider.Children;
			for (int i = 0; i < children.Length; i++)
			{
				if (GetProxyModuleIndex(children[i], guidProxy, ref nModuleIndex))
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveUnusedLogicalApps(int nProjectHandle)
		{
			if (LogicalIONotification.LogicalMappingApps == null)
			{
				return;
			}
			Guid[] array = new Guid[LogicalIONotification.LogicalMappingApps.DeviceGuids.Count];
			LogicalIONotification.LogicalMappingApps.DeviceGuids.CopyTo(array, 0);
			Guid[] array2 = array;
			foreach (Guid guid in array2)
			{
				if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, guid))
				{
					continue;
				}
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				IObjectProperty[] properties = metaObjectStub.Properties;
				foreach (IObjectProperty val in properties)
				{
					if (!(val is ILogicalApplicationProperty))
					{
						continue;
					}
					Guid logicalApplication = (val as ILogicalApplicationProperty).LogicalApplication;
					bool flag = false;
					if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, logicalApplication))
					{
						flag = true;
					}
					if (!flag)
					{
						if (((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, logicalApplication).Name + "_Mapping" != metaObjectStub.Name)
						{
							flag = true;
						}
						if (DeviceObjectHelper.GetHostStub(nProjectHandle, guid) != DeviceObjectHelper.GetHostStub(nProjectHandle, logicalApplication))
						{
							flag = true;
						}
					}
					if (flag)
					{
						try
						{
							((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(nProjectHandle, guid);
						}
						catch
						{
						}
					}
				}
			}
		}

		public static void CreateLogicalApp(int nProjectHandle, Guid logicalAppGuid, string stLogicalAppName)
		{
			Guid guid = logicalAppGuid;
			string text = stLogicalAppName + "_Mapping";
			bool flag = false;
			string stTaskName = "MapTask";
			string text2 = "MapPou";
			bool bEvent = false;
			string stEvent = string.Empty;
			string stInterval = "t#20ms";
			string stPriority = "2";
			bool bWatchdog = false;
			string stWatchdogSensitivity = string.Empty;
			string stWatchdogTime = string.Empty;
			string stWatchdogTimeUnit = string.Empty;
			while (guid != Guid.Empty)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (metaObjectStub == null)
				{
					break;
				}
				if (typeof(DeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, metaObjectStub.ObjectGuid);
					if (objectToRead != null && objectToRead.Object is DeviceObject)
					{
						DeviceObject deviceObject = objectToRead.Object as DeviceObject;
						if (((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(deviceObject.DeviceIdentificationNoSimulation) == null)
						{
							return;
						}
						ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceObject.DeviceIdentificationNoSimulation);
						if (targetSettingsById != null && targetSettingsById.Sections[stLogicalDeviceSection] != null)
						{
							flag = LocalTargetSettings.LogicalmapApplicationVisible.GetBoolValue(targetSettingsById);
							stTaskName = LocalTargetSettings.TaskName.GetStringValue(targetSettingsById);
							text2 = LocalTargetSettings.TaskPou.GetStringValue(targetSettingsById);
							bEvent = LocalTargetSettings.TaskEventEnable.GetBoolValue(targetSettingsById);
							stEvent = LocalTargetSettings.TaskEvent.GetStringValue(targetSettingsById);
							stInterval = LocalTargetSettings.TaskInterval.GetStringValue(targetSettingsById);
							stPriority = LocalTargetSettings.TaskPriority.GetStringValue(targetSettingsById);
							bWatchdog = LocalTargetSettings.WatchdogEnable.GetBoolValue(targetSettingsById);
							stWatchdogSensitivity = LocalTargetSettings.WatchDogSensitivity.GetStringValue(targetSettingsById);
							stWatchdogTime = LocalTargetSettings.WatchdogTime.GetStringValue(targetSettingsById);
							stWatchdogTimeUnit = LocalTargetSettings.WatchdogTimeUnit.GetStringValue(targetSettingsById);
							if (!LocalTargetSettings.DisableChildApp.GetBoolValue(targetSettingsById))
							{
								break;
							}
							return;
						}
					}
				}
				guid = metaObjectStub.ParentObjectGuid;
			}
			IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(nProjectHandle, logicalAppGuid);
			if (hostStub == null || !typeof(DeviceObject).IsAssignableFrom(hostStub.ObjectType))
			{
				return;
			}
			IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, hostStub.ObjectGuid);
			if (objectToRead2 == null || !(objectToRead2.Object is DeviceObject))
			{
				return;
			}
			IMetaObject application = (objectToRead2.Object as DeviceObject).GetApplication();
			if (application == null)
			{
				return;
			}
			Guid objectGuid = application.ObjectGuid;
			bool flag2 = false;
			Guid[] subObjectGuids = application.SubObjectGuids;
			foreach (Guid guid2 in subObjectGuids)
			{
				IMetaObjectStub mosTest = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid2);
				if (!typeof(IApplicationObject).IsAssignableFrom(mosTest.ObjectType) || !(mosTest.Name == text))
				{
					continue;
				}
				if ((!typeof(IHiddenObject).IsAssignableFrom(mosTest.ObjectType) && !APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mosTest))) != flag)
				{
					try
					{
						((IObjectManager)APEnvironment.ObjectMgr).RemoveObject(nProjectHandle, mosTest.ObjectGuid);
					}
					catch
					{
					}
				}
				else
				{
					flag2 = true;
					objectGuid = mosTest.ObjectGuid;
				}
			}
			IMetaObject val = null;
			val = (flag2 ? ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, objectGuid) : CreateApplikation(nProjectHandle, objectGuid, text, !flag, logicalAppGuid));
			if (val == null)
			{
				return;
			}
			IMetaObject val2 = CreateTaskConfigObject(val);
			if (val2 != null && CreateTaskObject(nProjectHandle, val2.ObjectGuid, stTaskName, text2, bEvent, stEvent, stInterval, stPriority, bWatchdog, stWatchdogSensitivity, stWatchdogTime, stWatchdogTimeUnit))
			{
				CreateEmptyTaskPou(val, text2);
			}
			IObject @object = val.Object;
			IChildOnlineApplicationObject val3 = (IChildOnlineApplicationObject)(object)((@object is IChildOnlineApplicationObject) ? @object : null);
			if (val3 == null || val3.KeepApplicationOnParentOnlineChange)
			{
				return;
			}
			IMetaObject val4 = null;
			try
			{
				val4 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val.ProjectHandle, val.ObjectGuid);
				if (val4 != null)
				{
					IObject object2 = val4.Object;
					val3 = (IChildOnlineApplicationObject)(object)((object2 is IChildOnlineApplicationObject) ? object2 : null);
					val3.KeepApplicationOnParentOnlineChange=(true);
				}
			}
			catch
			{
			}
			finally
			{
				if (val4 != null && val4.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val4, true, (object)null);
				}
			}
		}

		public static IMetaObject CreateApplikation(int nProjectHandle, Guid ParentObjectGuid, string stApplicationName, bool bHidden, Guid logicalAppGuid)
		{
			try
			{
				IApplicationObject val = null;
				val = ((!bHidden) ? APEnvironment.CreateApplicationObject() : APEnvironment.CreateHiddenApplicationObject());
				if (val is IChildOnlineApplicationObject)
				{
					((IChildOnlineApplicationObject)((val is IChildOnlineApplicationObject) ? val : null)).KeepApplicationOnParentOnlineChange=(true);
				}
				Guid guid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, ParentObjectGuid, Guid.NewGuid(), (IObject)(object)val, stApplicationName, -1);
				if (guid != Guid.Empty)
				{
					if (logicalAppGuid != Guid.Empty)
					{
						IMetaObject val2 = null;
						try
						{
							val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, guid);
							val2.AddProperty((IObjectProperty)(object)new LogicalApplicationProperty(logicalAppGuid));
						}
						catch
						{
						}
						finally
						{
							if (val2 != null && val2.IsToModify)
							{
								((IObjectManager)APEnvironment.ObjectMgr).SetObject(val2, true, (object)null);
							}
						}
					}
					return ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
				}
			}
			catch
			{
			}
			return null;
		}

		public static IMetaObject CreateTaskConfigObject(IMetaObject moApplication)
		{
			IMetaObject val = ((moApplication == null) ? null : DeviceObjectHelper.GetTaskConfig(moApplication));
			if (moApplication != null && val == null)
			{
				try
				{
					ITaskConfigObject val2 = APEnvironment.CreateTaskConfigObject();
					((IObjectManager)APEnvironment.ObjectMgr).AddObject(moApplication.ProjectHandle, moApplication.ObjectGuid, Guid.NewGuid(), (IObject)(object)val2, "Task configuration", -1);
					val = DeviceObjectHelper.GetTaskConfig(moApplication);
					return val;
				}
				catch
				{
					return val;
				}
			}
			return val;
		}

		public static bool CreateTaskObject(int nProjectHandle, Guid parentObjectGuid, string stTaskName, string stTaskPou, bool bEvent, string stEvent, string stInterval, string stPriority, bool bWatchdog, string stWatchdogSensitivity, string stWatchdogTime, string stWatchdogTimeUnit)
		{
			Debug.Assert(parentObjectGuid != Guid.Empty);
			Debug.Assert(!string.IsNullOrEmpty(stTaskName));
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, parentObjectGuid);
			if (metaObjectStub != null)
			{
				Guid[] subObjectGuids = metaObjectStub.SubObjectGuids;
				foreach (Guid guid in subObjectGuids)
				{
					if (((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid).Name == stTaskName)
					{
						return false;
					}
				}
			}
			ITaskObject val = APEnvironment.CreateTaskObject();
			if (bEvent)
			{
				Debug.Assert(!string.IsNullOrEmpty(stEvent));
				val.ExternalEvent=(stEvent);
				val.KindOf=((KindOfTask)4);
			}
			else
			{
				Debug.Assert(!string.IsNullOrEmpty(stInterval));
				val.Interval=(stInterval);
				val.KindOf=((KindOfTask)1);
			}
			Debug.Assert(!string.IsNullOrEmpty(stPriority));
			val.Priority=(stPriority);
			val.Watchdog.Enabled=(bWatchdog);
			ITaskObject6 val2 = (ITaskObject6)(object)((val is ITaskObject6) ? val : null);
			if (val2 != null)
			{
				val2.AllowEmpty=(true);
			}
			if (val.Watchdog.Enabled)
			{
				Debug.Assert(!string.IsNullOrEmpty(stWatchdogSensitivity));
				Debug.Assert(!string.IsNullOrEmpty(stWatchdogTime));
				Debug.Assert(!string.IsNullOrEmpty(stWatchdogTimeUnit));
				val.Watchdog.Sensitivity=(stWatchdogSensitivity);
				val.Watchdog.Time=(stWatchdogTime);
				val.Watchdog.TimeUnit=(stWatchdogTimeUnit);
			}
			try
			{
				Guid guid2 = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, parentObjectGuid, Guid.NewGuid(), (IObject)(object)val, stTaskName, -1);
				if (guid2 != Guid.Empty)
				{
					if (!string.IsNullOrEmpty(stTaskPou))
					{
						IMetaObject val3 = null;
						try
						{
							val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, guid2);
							if (val3 != null)
							{
								IObject @object = val3.Object;
								val = (ITaskObject)(object)((@object is ITaskObject) ? @object : null);
								IPouObject val4 = val.CreatePOU(stTaskPou);
								if (val4 != null)
								{
									val.POUs.Add(val4);
								}
								((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
							}
						}
						catch
						{
						}
					}
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		public static bool CreateEmptyTaskPou(IMetaObject moApplication, string stPou)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			Guid[] subObjectGuids = moApplication.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				if (((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(moApplication.ProjectHandle, guid).Name == stPou)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				try
				{
					IPOUObject val = APEnvironment.CreatePOUObject();
					ISTImplementationObject val2 = APEnvironment.CreateSTImplementationObject();
					((ITextVarDeclObject)val.Interface).TextDocument.Text=($"PROGRAM {stPou}\nVAR\nEND_VAR\n");
					val.Implementation=((IImplementationObject)(object)val2);
					val2.TextDocument.Text=("");
					if (((IObjectManager)APEnvironment.ObjectMgr).AddObject(moApplication.ProjectHandle, moApplication.ObjectGuid, Guid.NewGuid(), (IObject)(object)val, stPou, -1) != Guid.Empty)
					{
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		public static void CheckLogicalAppsDownload(Guid objectGuid, KindOfDownloadCommand? kindof)
		{
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Invalid comparison between Unknown and I4
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			if (_bIsCheckingLogicalAppsInProgress)
			{
				return;
			}
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return;
			}
			int handle = primaryProject.Handle;
			if (!DeviceObjectHelper.GenerateCodeForLogicalDevices || LogicalIONotification.LogicalMappingApps == null || LogicalIONotification.LogicalMappingApps.DeviceGuids.Contains(objectGuid))
			{
				return;
			}
			IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(handle, objectGuid);
			Guid[] array = new Guid[LogicalIONotification.LogicalMappingApps.DeviceGuids.Count];
			LogicalIONotification.LogicalMappingApps.DeviceGuids.CopyTo(array, 0);
			Guid[] array3;
			if (DeviceObjectHelper.PhysicalDevices != null)
			{
				Guid[] array2 = new Guid[DeviceObjectHelper.PhysicalDevices.DeviceGuids.Count];
				DeviceObjectHelper.PhysicalDevices.DeviceGuids.CopyTo(array2, 0);
				bool flag = false;
				array3 = array2;
				foreach (Guid objectGuid2 in array3)
				{
					IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(handle, objectGuid2);
					if (hostStub.ObjectGuid == hostStub2.ObjectGuid)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			array3 = array;
			Guid g = default(Guid);
			Guid g2 = default(Guid);
			Guid g3 = default(Guid);
			Guid g4 = default(Guid);
			foreach (Guid guid in array3)
			{
				if (!(DeviceObjectHelper.GetHostStub(handle, guid).ObjectGuid == hostStub.ObjectGuid))
				{
					continue;
				}
				string text = string.Empty;
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
				if (metaObjectStub.ParentObjectGuid != objectGuid)
				{
					continue;
				}
				if (metaObjectStub != null && metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, metaObjectStub.ParentObjectGuid);
					if (objectToRead.Object is IOnlineApplicationObject2)
					{
						IObject @object = objectToRead.Object;
						text = ((IOnlineApplicationObject2)((@object is IOnlineApplicationObject2) ? @object : null)).ApplicationName;
						IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(objectToRead.ObjectGuid);
						IOnlineApplication10 val = (IOnlineApplication10)(object)((application is IOnlineApplication10) ? application : null);
						if (val != null)
						{
							((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetDownloadIds(objectToRead.ObjectGuid, out g, out g2);
							if (((IOnlineApplication)val).CodeId == Guid.Empty || ((IOnlineApplication)val).DataId == Guid.Empty || !((IOnlineApplication)val).CodeId.Equals(g) || !((IOnlineApplication)val).DataId.Equals(g2))
							{
								break;
							}
						}
					}
				}
				IOnlineApplication application2 = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guid);
				IOnlineApplication10 val2 = (IOnlineApplication10)(object)((application2 is IOnlineApplication10) ? application2 : null);
				if (val2 == null)
				{
					continue;
				}
				try
				{
					_bIsCheckingLogicalAppsInProgress = true;
					bool flag2 = false;
					if (!((IOnlineApplication)val2).IsLoggedIn && kindof.HasValue)
					{
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetDownloadIds(guid, out g3, out g4);
						if (((IOnlineApplication)val2).CodeId == Guid.Empty || ((IOnlineApplication)val2).DataId == Guid.Empty)
						{
							((IOnlineApplication)val2).Login(true);
						}
						if (!((IOnlineApplication)val2).CodeId.Equals(g3) || !((IOnlineApplication)val2).DataId.Equals(g4) || ((IOnlineApplication)val2).CodeId == Guid.Empty || ((IOnlineApplication)val2).DataId == Guid.Empty)
						{
							if ((int)((IOnlineApplication)val2).ApplicationState == 1)
							{
								((IOnlineApplication)val2).Stop();
							}
							((IOnlineApplication4)val2).ReinitAppOnDevice(text, true);
							((IOnlineApplication)val2).Logout();
							flag2 = true;
						}
					}
					if (!((IOnlineApplication)val2).IsLoggedIn && !((IOnlineApplication)val2).Login(true) && kindof.HasValue)
					{
						((IOnlineApplication)val2).CreateAppOnDevice(text);
						((IOnlineApplication)val2).Login(true);
						flag2 = true;
					}
					if (((IOnlineApplication)val2).IsLoggedIn && flag2)
					{
						((IOnlineApplication)val2).Download(false);
					}
					if (((IOnlineApplication)val2).IsLoggedIn && kindof.HasValue && kindof == (KindOfDownloadCommand?)2)
					{
						((IOnlineApplication3)val2).CreateBootProject();
					}
					if (((IOnlineApplication)val2).IsLoggedIn)
					{
						((IOnlineApplication)val2).Start();
					}
				}
				catch
				{
				}
				finally
				{
					try
					{
						if (((IOnlineApplication)val2).IsLoggedIn)
						{
							((IOnlineApplication)val2).Logout();
						}
					}
					catch
					{
					}
					_bIsCheckingLogicalAppsInProgress = false;
				}
			}
		}
	}
}
