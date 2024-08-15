using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.OnlineCommands;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[SystemInterface("_3S.CoDeSys.DeviceObject.IDeviceController")]
	[TypeGuid("{4038553c-4c10-4817-9c44-edbae205219b}")]
	public class DeviceController : IDeviceController3, IDeviceController2, IDeviceController, ISystemInstanceRequiresInitialization
	{
		private class ApplicationCollection : IEnumerable
		{
			private static LDictionary<Guid, ApplicationController> s_htAppGuidToAppController = new LDictionary<Guid, ApplicationController>();

			private DeviceController _devicecontroller;

			public ApplicationController this[Guid guidApp]
			{
				get
				{
					ApplicationController applicationController = default(ApplicationController);
					s_htAppGuidToAppController.TryGetValue(guidApp, out applicationController);
					if (applicationController == null)
					{
						applicationController = new ApplicationController(guidApp, _devicecontroller.ProjectHandle, _devicecontroller);
						s_htAppGuidToAppController[guidApp]= applicationController;
					}
					return applicationController;
				}
			}

			public ApplicationCollection(DeviceController devicecontroller)
			{
				_devicecontroller = devicecontroller;
			}

			public void Remove(Guid guidApp)
			{
				ApplicationController applicationController = default(ApplicationController);
				s_htAppGuidToAppController.TryGetValue(guidApp, out applicationController);
				if (applicationController != null)
				{
					applicationController.Clear();
					s_htAppGuidToAppController.Remove(guidApp);
				}
			}

			public void Clear()
			{
				foreach (ApplicationController applicationController in DeviceController.ApplicationCollection.s_htAppGuidToAppController.Values)
				{
					applicationController.Clear();
				}
				DeviceController.ApplicationCollection.s_htAppGuidToAppController.Clear();
			}

			public bool GetModuleStatus(Guid guidObject, int nConnectorId, out ModuleStatus status)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				status = (ModuleStatus)0;
				bool result;
				try
				{
					foreach (ApplicationController applicationController in DeviceController.ApplicationCollection.s_htAppGuidToAppController.Values)
					{
						if (APEnvironment.ObjectMgr.ExistsObject(applicationController.ProjectHandle, guidObject))
						{
							IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(applicationController.ProjectHandle, guidObject);
							if (objectToRead.Object is ISlotDeviceObject2 && !(objectToRead.Object as ISlotDeviceObject2).HasDevice)
							{
								break;
							}
							IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(applicationController.ProjectHandle, guidObject);
							if (hostStub != null)
							{
								Guid ioApplication = DeviceObjectHelper.GetIoApplication(applicationController.ProjectHandle, hostStub.ObjectGuid);
								if (!(ioApplication == Guid.Empty) && (!(ioApplication != Guid.Empty) || !(ioApplication != applicationController.ApplicationGuid)))
								{
									IOnlineVarRef onlineVarRef = applicationController.GetOnlineVarRef(guidObject, nConnectorId);
									if (onlineVarRef != null)
									{
										if (onlineVarRef.State != VarRefState.Good)
										{
											return false;
										}
										status = (ModuleStatus)((uint)onlineVarRef.Value);
										if ((ModuleStatus.DiagProvider & status) == ModuleStatus.DiagProvider)
										{
											if ((ModuleStatus.DeviceEnabled & status) == ModuleStatus.DeviceEnabled)
											{
												status |= ModuleStatus.ModuleEnabled;
											}
											else
											{
												status &= ~ModuleStatus.ModuleEnabled;
											}
										}
										return true;
									}
								}
							}
						}
					}
					result = false;
				}
				catch (Exception)
				{
					result = false;
				}
				return result;
			}

			public IEnumerator GetEnumerator()
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return (IEnumerator)(object)s_htAppGuidToAppController.Values.GetEnumerator();
			}
		}

		private ApplicationCollection _applications;

		private bool _bPauseApplicationEvents;

		private LList<Guid> _appsSuspendMonitoring = new LList<Guid>();

		private LList<Guid> _appsToUpdate = new LList<Guid>();

		private ILoginService _svcLogin;

		internal int ProjectHandle
		{
			get
			{
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject == null)
				{
					return -1;
				}
				return primaryProject.Handle;
			}
		}

		public event ModuleStatusEventHandler ModuleStatusChanged;


		public event DeviceProvidingLanguageModelHandler DeviceProvidingLanguageModel;

		public DeviceController()
		{
			_applications = new ApplicationCollection(this);
		}

		public void OnAllSystemInstancesAvailable()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected O, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Expected O, but got Unknown
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			((IEngine)APEnvironment.Engine).Projects.BeforePrimaryProjectSwitched+=(new PrimaryProjectSwitchedEventHandler(OnBeforePrimaryProjectSwitched));
			_svcLogin = (ILoginService)(object)APEnvironment.CreateLoginServiceWrapper();
			_svcLogin.CompoundLoginStarted+=(new CompoundLoginStartedHandler(CompoundLoginStarted));
			_svcLogin.CompoundLoginAborted+=(new CompoundLoginAbortedHandler(CompoundLoginAborted));
			_svcLogin.CompoundAfterLoginFinished+=(new CompoundAfterLoginFinishedHandler(CompoundAfterLoginFinished));
			((IOnlineManager)APEnvironment.OnlineMgr).BeforeApplicationLogin+=(new BeforeApplicationLoginEventHandler(OnBeforeApplicationLogin));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin+=(new AfterApplicationLoginEventHandler(OnAfterApplicationLogin));
			((IOnlineManager2)APEnvironment.OnlineMgr).AfterApplicationDownload+=(new AfterApplicationDownloadEventHandler(OnAfterApplicationDownload));
			((IOnlineManager)APEnvironment.OnlineMgr).ApplicationStateChanged+=(new ApplicationStateChangedEventHandler(OnAfterApplicationStateChanged));
			APEnvironment.DeviceMgr.DeviceUpdated += new DeviceEventHandler(OnDeviceUpdated);
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved+=(new ObjectRemovedEventHandler(OnObjectRemoved));
		}

		public bool GetModuleStatus(int nProjectHandle, Guid guidObject, int nConnectorId, out ModuleStatus status)
		{
			if (!IsPrimaryProject(nProjectHandle))
			{
				status = (ModuleStatus)0;
				return false;
			}
			return _applications.GetModuleStatus(guidObject, nConnectorId, out status);
		}

		public IModuleRuntimeInfo[] GetDeviceInfo(int nProjectHandle, Guid guidObject)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			_=((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidObject);
			if (objectToRead.Object is IExplicitConnector)
			{
				IExplicitConnector val = (IExplicitConnector)objectToRead.Object;
				ModuleStatus status;
				bool moduleStatus = GetModuleStatus(nProjectHandle, guidObject, ((IConnector)val).ConnectorId, out status);
				return (IModuleRuntimeInfo[])(object)new IModuleRuntimeInfo[1]
				{
					new ModuleRuntimeInfo(nProjectHandle, guidObject, ((IConnector)val).ConnectorId, status, moduleStatus)
				};
			}
			if (objectToRead.Object is IDeviceObject)
			{
				LList<IModuleRuntimeInfo> val2 = new LList<IModuleRuntimeInfo>();
				bool moduleStatus2 = GetModuleStatus(nProjectHandle, guidObject, -1, out var status2);
				val2.Add((IModuleRuntimeInfo)(object)new ModuleRuntimeInfo(nProjectHandle, guidObject, -1, status2, moduleStatus2));
				foreach (IConnector item in (IEnumerable)((IDeviceObject)objectToRead.Object).Connectors)
				{
					IConnector val3 = item;
					if (!FilterConnector(val3, objectToRead))
					{
						moduleStatus2 = GetModuleStatus(nProjectHandle, guidObject, val3.ConnectorId, out status2);
						val2.Add((IModuleRuntimeInfo)(object)new ModuleRuntimeInfo(nProjectHandle, guidObject, val3.ConnectorId, status2, moduleStatus2));
					}
				}
				IModuleRuntimeInfo[] array = (IModuleRuntimeInfo[])(object)new IModuleRuntimeInfo[val2.Count];
				val2.CopyTo(array);
				return array;
			}
			return (IModuleRuntimeInfo[])(object)new IModuleRuntimeInfo[0];
		}

		internal void RaiseModuleStateChanged(int nProjectHandle, Guid guidObject, int nConnectorId, Guid guidApplication)
		{
			if ((object)this.ModuleStatusChanged != null)
			{
				this.ModuleStatusChanged.Invoke((object)this, (IModuleStatusEventArgs)(object)new ModuleStatusEventArgs(nProjectHandle, guidObject, nConnectorId, guidApplication));
			}
		}

		internal void RaiseDeviceProvidingLanguageModel(DeviceLanguageModelEventArgsImpl e)
		{
			if ((object)this.DeviceProvidingLanguageModel != null)
			{
				this.DeviceProvidingLanguageModel.Invoke((object)this, (IDeviceLanguageModelEventArgs)(object)e);
			}
		}

		private bool IsPrimaryProject(int nProjectHandle)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return false;
			}
			return primaryProject.Handle == nProjectHandle;
		}

		private bool FilterConnector(IConnector connector, IMetaObject mo)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if ((int)connector.ConnectorRole == 1)
			{
				if (mo.ParentObjectGuid == Guid.Empty)
				{
					return true;
				}
				foreach (IAdapter item in (IEnumerable)connector.Adapters)
				{
					if (Array.IndexOf(item.Modules, mo.ParentObjectGuid) >= 0)
					{
						return false;
					}
				}
				return true;
			}
			if (connector.IsExplicit)
			{
				return true;
			}
			return false;
		}

		private void OnDeviceUpdated(object sender, DeviceEventArgs e)
		{
			_applications.Clear();
			_appsToUpdate.Clear();
		}

		private void OnBeforePrimaryProjectSwitched(IProject oldProject, IProject newProject)
		{
			_applications.Clear();
			_appsToUpdate.Clear();
		}

		private void OnObjectRemoved(object sender, ObjectRemovedEventArgs e)
		{
			if (e.MetaObject != null && e.MetaObject.Object != null && e.MetaObject.Object is IApplicationObject)
			{
				RemoveApplication(e.MetaObject.ObjectGuid);
			}
		}

		private void OnAfterApplicationDownload(object sender, OnlineEventArgs e)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			if (e is OnlineDownloadEventArgs2 && ((OnlineDownloadEventArgs2)e).DownloadException != null)
			{
				return;
			}
			if (_bPauseApplicationEvents || _appsSuspendMonitoring.Contains(e.GuidObject))
			{
				if (!_appsToUpdate.Contains(e.GuidObject))
				{
					_appsToUpdate.Add(e.GuidObject);
				}
				return;
			}
			try
			{
				KindOfDownloadCommand value = (KindOfDownloadCommand)0;
				if (e is OnlineDownloadEventArgs)
				{
					value = ((OnlineDownloadEventArgs)((e is OnlineDownloadEventArgs) ? e : null)).KindOf;
				}
				LogicalIOHelper.CheckLogicalAppsDownload(e.GuidObject, value);
				_applications[e.GuidObject].Rebuild();
			}
			catch
			{
			}
		}

		private void OnAfterApplicationStateChanged(object sender, OnlineEventArgs e)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (_bPauseApplicationEvents || _appsSuspendMonitoring.Contains(e.GuidObject))
			{
				return;
			}
			try
			{
				LogicalIOHelper.CheckLogicalAppsDownload(e.GuidObject, null);
				ApplicationController applicationController = this._applications[e.GuidObject];
				if (!applicationController.IsInitialized)
				{
					applicationController.Rebuild();
				}
				if (applicationController.ModuleKeyToVarRef != null)
				{
					foreach (IOnlineVarRef2 onlineVarRef in applicationController.ModuleKeyToVarRef.Values)
					{
						ModuleKey moduleKey = onlineVarRef.Tag as ModuleKey;
						this.RaiseModuleStateChanged(applicationController.ProjectHandle, moduleKey.ObjectGuid, moduleKey.ConnectorId, applicationController.ApplicationGuid);
					}
				}
			}
			catch
			{
			}
		}

		private void OnAfterApplicationLogin(object sender, OnlineEventArgs e)
		{
			try
			{
				ApplicationController applicationController = _applications[e.GuidObject];
				if (applicationController != null && !applicationController.IsInitialized && !_appsToUpdate.Contains(e.GuidObject))
				{
					_appsToUpdate.Add(e.GuidObject);
				}
				if (!_bPauseApplicationEvents && !_appsSuspendMonitoring.Contains(e.GuidObject))
				{
					LogicalIOHelper.CheckLogicalAppsDownload(e.GuidObject, (KindOfDownloadCommand)0);
					if (!applicationController.IsInitialized)
					{
						applicationController.Rebuild();
					}
				}
			}
			catch
			{
			}
		}

		private void OnBeforeApplicationLogin(object sender, OnlineCancelEventArgs e)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return;
			}
			int handle = primaryProject.Handle;
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, e.ObjectGuid);
			if (typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return;
			}
			IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(handle, e.ObjectGuid);
			if (hostStub != null)
			{
				Guid guid = DeviceObjectHelper.ConfigModeApplication(hostStub.ObjectGuid);
				if (guid != Guid.Empty && guid != e.ObjectGuid)
				{
					e.Cancel((Exception)new DeviceObjectException((DeviceObjectExeptionReason)22, Strings.LoginConfigModeError));
				}
			}
		}

		private void CompoundAfterLoginFinished(object sender, LoginServiceEventArgs e)
		{
			_bPauseApplicationEvents = false;
			Guid[] array = new Guid[_appsToUpdate.Count];
			_appsToUpdate.CopyTo(array);
			Guid[] array2 = array;
			foreach (Guid guid in array2)
			{
				if (!_appsSuspendMonitoring.Contains(guid))
				{
					RebuildAfterLoginFinished(guid);
					_appsToUpdate.Remove(guid);
				}
			}
		}

		private void CompoundLoginAborted(object sender, LoginServiceEventArgs e)
		{
			foreach (Guid applicationObjectGuid in e.ApplicationObjectGuids)
			{
				if (_appsToUpdate.Contains(applicationObjectGuid))
				{
					_appsToUpdate.Remove(applicationObjectGuid);
				}
			}
		}

		private void CompoundLoginStarted(object sender, LoginServiceEventArgs e)
		{
			_bPauseApplicationEvents = true;
		}

		private void RebuildAfterLoginFinished(Guid appGuid)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				LogicalIOHelper.CheckLogicalAppsDownload(appGuid, new KindOfDownloadCommand?(KindOfDownloadCommand.Download));
				ApplicationController applicationController = this._applications[appGuid];
				applicationController.Rebuild();
				if (applicationController.ModuleKeyToVarRef != null)
				{
					foreach (IOnlineVarRef2 onlineVarRef in applicationController.ModuleKeyToVarRef.Values)
					{
						ModuleKey moduleKey = onlineVarRef.Tag as ModuleKey;
						this.RaiseModuleStateChanged(applicationController.ProjectHandle, moduleKey.ObjectGuid, moduleKey.ConnectorId, applicationController.ApplicationGuid);
					}
				}
			}
			catch
			{
			}
		}

		internal void RemoveApplication(Guid guidApp)
		{
			_applications.Remove(guidApp);
		}

		public void SuspendMonitoring(Guid appGuid)
		{
			if (!_appsSuspendMonitoring.Contains(appGuid))
			{
				_appsSuspendMonitoring.Add(appGuid);
			}
			foreach (ApplicationController application in _applications)
			{
				if (application.ApplicationGuid == appGuid)
				{
					application.Clear();
				}
			}
			if (((IEngine)APEnvironment.Engine).Frame == null)
			{
				return;
			}
			IView[] views = ((IEngine)APEnvironment.Engine).Frame.GetViews();
			if (views == null)
			{
				return;
			}
			IView[] array = views;
			foreach (IView obj in array)
			{
				INavigatorControl val = (INavigatorControl)(object)((obj is INavigatorControl) ? obj : null);
				if (val != null)
				{
					val.Refill();
				}
			}
		}

		public void ResumeMonitoring(Guid appGuid)
		{
			_appsSuspendMonitoring.Remove(appGuid);
			foreach (ApplicationController application in _applications)
			{
				if (application.ApplicationGuid == appGuid && !_appsToUpdate.Contains(application.ApplicationGuid))
				{
					RebuildAfterLoginFinished(application.ApplicationGuid);
				}
			}
			CompoundAfterLoginFinished(this, null);
		}

		public bool IsMonitoringSuspended(Guid appGuid)
		{
			return _appsSuspendMonitoring.Contains(appGuid);
		}
	}
}
