#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.ProjectCompare;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public class DeviceObjectHelper
	{
		internal class RemoveTaskData
		{
			internal string stMetaObjectName;

			internal Guid ObjectGuid;

			internal Guid appGuid;

			internal Guid HostGuid;
		}

		private static DeviceObjectHelper _instance = null;

		private static Hashtable _htDevicesInGetLanguageModel = new Hashtable();

		private static Hashtable _htDevicesDuringCreate = new Hashtable();

		private static DataElementOnlineVarRefManager _ovrManager = new DataElementOnlineVarRefManager();

		public const string DEVICENAME = "$(DeviceName)";

		public const string DEVICEDIAGNAMESPACE = "DED";

		public const string DEVICEDIAGFB = "CAADiagDeviceDefault";

		public const string DEVICEDIAGFBWITHNAMESPACE = "DED.CAADiagDeviceDefault";

		internal static readonly Guid GUID_PLCLOGICOBJECTFACTORY = new Guid("{8ceeba4e-ac7a-4fbd-9415-bfb2d98668ab}");

		private static bool _bRefactoring = false;

		private static LList<Guid> _liPLCRenew = new LList<Guid>();

		private static Guid CMDGUID_CLEANALL = new Guid("{024a0dc9-987d-42be-add4-5775c60d128a}");

		private IProjectStructure _projectStructure;

		private static bool _bProjectRecursion = false;

		private static LDictionary<Guid, int> _dictObjectToUpdate = new LDictionary<Guid, int>();

		private static bool _bAfterLoadFinished = true;

		private static LDictionary<string, LList<Guid>> _dictLogicalNames = new LDictionary<string, LList<Guid>>();

		private static LDictionary<Guid, Guid> _dictHosts = new LDictionary<Guid, Guid>();

		private static LDictionary<string, LList<Guid>> _dictMappedDevices = new LDictionary<string, LList<Guid>>();

		private static IDeviceManagerBuffer _rootDeviceBuffer;

		private static IDeviceManagerBuffer _additionalModules;

		private static IDeviceManagerBuffer _logicalDeviceBuffer;

		private static IDeviceManagerBuffer _physicalDeviceBuffer;

		private static LDictionary<Guid, ISequenceStatement> _dictIoConfigPou = new LDictionary<Guid, ISequenceStatement>();

		private static readonly Version V3_3_2_0 = new Version("3.3.2.0");

		private static readonly Guid GUID_DEVICEOBJECTPLUGIN = new Guid("{2A785A6D-F546-47d1-9107-6BCAA0F232CF}");

		private static IObject _lastObject = null;

		internal static bool _bInDeleteUnusedTasks = false;

		private static LList<Guid> _liLogicalDevicesToRemove = new LList<Guid>();

		private static LDictionary<RequiredTask, RemoveTaskData> _dictRemovedTasks = new LDictionary<RequiredTask, RemoveTaskData>();

		private static LDictionary<IRequiredLib, IIoProvider> _dictRemovedLibs = new LDictionary<IRequiredLib, IIoProvider>();

		private static bool bRenameInRecursion = false;

		private static LDictionary<string, LanguageModelData> _dictLanguageModelValues = new LDictionary<string, LanguageModelData>();

		private static IUndoManager2 s_undoMgr = null;

		private static LDictionary<string, LList<object>> _htUpdateDeviceFactories = new LDictionary<string, LList<object>>();

		private static Hashtable s_htPackModes = new Hashtable();

		private static LDictionary<Guid, IAddressAssignmentStrategy> _liStrategies = new LDictionary<Guid, IAddressAssignmentStrategy>();

		protected static bool _bEnableLogicalDevices = false;

		protected static bool _bFeatureRead = false;

		private static bool _bUpdateObjectSuppressed = false;

		internal static readonly Guid ParamModeGuid = new Guid("{15CC04EF-EB29-4405-A0DC-1F5337E25436}");

		private static Hashtable _htConfigModeApplication = new Hashtable();

		private static Hashtable _htIecAddresses = new Hashtable();

		private static bool _bIsInLateLanguageModel = false;

		private static LDictionary<IIoProvider, LList<IIoProvider>> _dictChildrens = new LDictionary<IIoProvider, LList<IIoProvider>>();

		private static LDictionary<Guid, Guid> _dictLanguageModelGuids = new LDictionary<Guid, Guid>();

		private static LDictionary<Guid, bool> _dictHasManualAddresses = new LDictionary<Guid, bool>();

		internal static bool bSkipPlaceholderResolution = false;

		internal static bool MessageShown { get; set; }

		public static bool AfterLoadFinished => _bAfterLoadFinished;

		public static LDictionary<string, LList<Guid>> LogicalNames => _dictLogicalNames;

		internal static LDictionary<Guid, Guid> HostsForLogicalDevices => _dictHosts;

		internal static LDictionary<string, LList<Guid>> MappedDevices => _dictMappedDevices;

		internal static IDeviceManagerBuffer RootDevices => _rootDeviceBuffer;

		internal static IDeviceManagerBuffer AdditionalModules => _additionalModules;

		internal static IDeviceManagerBuffer LogicalDevices => _logicalDeviceBuffer;

		internal static IDeviceManagerBuffer PhysicalDevices => _physicalDeviceBuffer;

		internal static LDictionary<Guid, ISequenceStatement> IoConfigPou => _dictIoConfigPou;

		internal static DataElementOnlineVarRefManager OvrManager => _ovrManager;

		internal static LDictionary<RequiredTask, RemoveTaskData> RemovedTasks => _dictRemovedTasks;

		internal static LDictionary<IRequiredLib, IIoProvider> RemovedLibs => _dictRemovedLibs;

		public static bool EnableLogicalDevices
		{
			get
			{
				if (_bFeatureRead)
				{
					return _bEnableLogicalDevices;
				}
				return ReadFeatureEnableLogicalDevices;
			}
		}

		public static bool GenerateCodeForLogicalDevices
		{
			get
			{
				if (_logicalDeviceBuffer != null && _logicalDeviceBuffer.DeviceGuids != null && _logicalDeviceBuffer.DeviceGuids.Count > 0)
				{
					return true;
				}
				return EnableLogicalDevices;
			}
		}

		public static bool ReadFeatureEnableLogicalDevices
		{
			get
			{
				IFeatureSettingsManager featureSettingsMgrOrNull = APEnvironment.FeatureSettingsMgrOrNull;
				if (featureSettingsMgrOrNull != null)
				{
					_bEnableLogicalDevices = featureSettingsMgrOrNull.GetFeatureSettingValue("device-management", "enable-logicaldevices", false);
					_bFeatureRead = true;
					return _bEnableLogicalDevices;
				}
				return true;
			}
		}

		internal static bool IsUpdateObjectSuppressed => _bUpdateObjectSuppressed;

		internal static Hashtable ConfigModeList => _htConfigModeApplication;

		internal static Hashtable HashIecAddresses => _htIecAddresses;

		internal static bool IsInLateLanguageModel
		{
			get
			{
				return _bIsInLateLanguageModel;
			}
			set
			{
				_bIsInLateLanguageModel = value;
			}
		}

		internal static LDictionary<IIoProvider, LList<IIoProvider>> IoProviderChildrens => _dictChildrens;

		internal static LDictionary<Guid, Guid> LanguageModelGuids => _dictLanguageModelGuids;

		internal static LDictionary<Guid, bool> HasManualAddresses => _dictHasManualAddresses;

		internal static bool SkipPlaceholderResolution
		{
			get
			{
				return bSkipPlaceholderResolution;
			}
			set
			{
				bSkipPlaceholderResolution = value;
			}
		}

		private DeviceObjectHelper()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Expected O, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected O, but got Unknown
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Expected O, but got Unknown
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Expected O, but got Unknown
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Expected O, but got Unknown
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Expected O, but got Unknown
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Expected O, but got Unknown
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Expected O, but got Unknown
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Expected O, but got Unknown
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Expected O, but got Unknown
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Expected O, but got Unknown
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Expected O, but got Unknown
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Expected O, but got Unknown
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded+=(new ObjectEventHandler(OnObjectAdded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdding+=(new ObjectAddingEventHandler(OnObjectAdding));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved+=(new ObjectRemovedEventHandler(OnObjectRemoved));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectModified+=(new ObjectModifiedEventHandler(ObjectMgr_ObjectModified));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectLoaded+=(new ObjectEventHandler(OnObjectLoaded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRenamed+=(new ObjectRenamedEventHandler(OnObjectRenamed));
			((IObjectManager)APEnvironment.ObjectMgr).ProjectClosed+=(new ProjectClosedEventHandler(OnProjectClosed));
			((IObjectManager)APEnvironment.ObjectMgr).ProjectSaved+=(new ProjectSavedEventHandler(ObjectMgr_ProjectSaved));
			((IObjectManager)APEnvironment.ObjectMgr).ProjectSaving+=(new ProjectSavingEventHandler(ObjectMgr_ProjectSaving));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectMoving+=(new ObjectMovingEventHandler(ObjectMgr_ObjectMoving));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectMoved+=(new ObjectMovedEventHandler(ObjectMgr_ObjectMoved));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectModifying+=(new ObjectEventHandler(ObjectMgr_ObjectModifying));
			((IObjectManager6)APEnvironment.ObjectMgr).ProjectLoadFinished+=(new ProjectLoadFinishedEventHandler(DeviceObjectHelper_ProjectLoadFinished));
			((IOnlineManager2)APEnvironment.OnlineMgr).BeforeApplicationDownload+=(new BeforeApplicationDownloadEventHandler(BeforeApplicationDownload));
			((ILanguageModelManager6)APEnvironment.LanguageModelMgr).TaskConfigChanged+=(new CompileEventHandler(DeviceObjectHelper_TaskConfigChanged));
			((ILanguageModelManager)APEnvironment.LanguageModelMgr).BeforeClearAll+=((EventHandler)DeviceObjectHelper_BeforeClearAll);
			((ILanguageModelManager)APEnvironment.LanguageModelMgr).BeforeCompile+=(new CompileEventHandler(LanguageModelMgr_BeforeCompile));
			((ILanguageModelManager)APEnvironment.LanguageModelMgr).AddLateLanguageModel+=(new AddLanguageModelEventHandler(OnLanguageModelMgr_AddLateLanguageModel));
			IObjectManager8 objectMgr = APEnvironment.ObjectMgr;
			ISVInterceptionProvider val = (ISVInterceptionProvider)(object)((objectMgr is ISVInterceptionProvider) ? objectMgr : null);
			if (val != null)
			{
				val.SVNodesRemovingInterception+=(new SVNodesRemovingInterceptionHandler(OnSVNodesRemovingInterception));
				val.SVNodesPastingInterception+=(new SVNodesPastingInterceptionHandler(OnSVNodesPastingInterception));
			}
			if (val is ISVInterceptionProvider4)
			{
				((ISVInterceptionProvider3)((val is ISVInterceptionProvider4) ? val : null)).SVNodesDragDropInterception+=(new SVNodesDragDropInterceptionHandler(OnSVNodesDragDropInterception));
				((ISVInterceptionProvider4)((val is ISVInterceptionProvider4) ? val : null)).SVNodesDragOverInterception+=((EventHandler<SVNodesDragOverEventArgs>)OnSVNodesDragOverInterception);
			}
			((IDeviceRepository3)APEnvironment.DeviceRepository).DescriptionInstalled+=(new DescriptionEventHandler(OnDescriptionInstalled));
			((IDeviceRepository3)APEnvironment.DeviceRepository).DescriptionUpdated+=(new DescriptionEventHandler(OnDescriptionInstalled));
			_logicalDeviceBuffer = APEnvironment.DeviceMgr.CreateDeviceBuffer(IsLogicalDevice);
			_physicalDeviceBuffer = APEnvironment.DeviceMgr.CreateDeviceBuffer(IsPhysicalDevice);
			_additionalModules = APEnvironment.DeviceMgr.CreateDeviceBuffer(IsAdditionalModule);
			_rootDeviceBuffer = APEnvironment.DeviceMgr.CreateDeviceBuffer(IsRootDevice);
			((IEngine)APEnvironment.Engine).Projects.AfterPrimaryProjectSwitched+=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
			if (APEnvironment.RefactoringServiceOrNull != null)
			{
				APEnvironment.RefactoringServiceOrNull.RefactoringPrepared+=((EventHandler<RefactoringCancelEventArgs>)RefactoringService_RefactoringPrepared);
				APEnvironment.RefactoringServiceOrNull.RefactoringPerformed+=((EventHandler<RefactoringExecutionEventArgs>)RefactoringService_RefactoringPerformed);
			}
			if (((IEngine)APEnvironment.Engine).CommandManager is ICommandManager7)
			{
				((ICommandManager3)(ICommandManager7)((IEngine)APEnvironment.Engine).CommandManager).CommandExecuted+=((EventHandler<CommandExecutedEventArgs>)DeviceObjectHelper_CommandExecuted);
			}
			OnPrimaryProjectSwitched(null, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject);
		}

		private void RefactoringService_RefactoringPrepared(object sender, RefactoringCancelEventArgs e)
		{
			_bRefactoring = true;
		}

		private void RefactoringService_RefactoringPerformed(object sender, RefactoringExecutionEventArgs e)
		{
			_bRefactoring = false;
		}

		private void DeviceObjectHelper_CommandExecuted(object sender, CommandExecutedEventArgs e)
		{
			if (((AbstractCommandExecutionEventArgs)e).CommandGuid == CMDGUID_CLEANALL && _liPLCRenew.Count > 0 && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(handle);
				if (undoManager is IUndoManager4 && undoManager.InCompoundAction)
				{
					((IUndoManager4)((undoManager is IUndoManager4) ? undoManager : null)).AfterEndCompoundAction2+=((EventHandler)DeviceObjectHelper_AfterEndCompoundAction2);
				}
				else
				{
					RenewPLCLanguageModel();
				}
			}
		}

		private void LanguageModelMgr_BeforeCompile(object sender, CompileEventArgs e)
		{
			RenewPLCLanguageModel();
		}

		private void DeviceObjectHelper_AfterEndCompoundAction2(object sender, EventArgs e)
		{
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(handle);
				if (undoManager is IUndoManager4)
				{
					((IUndoManager4)((undoManager is IUndoManager4) ? undoManager : null)).AfterEndCompoundAction2-=((EventHandler)DeviceObjectHelper_AfterEndCompoundAction2);
				}
				RenewPLCLanguageModel();
			}
		}

		private static void RenewPLCLanguageModel()
		{
			if (_liPLCRenew.Count <= 0 || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return;
			}
			int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
			foreach (Guid item in _liPLCRenew)
			{
				try
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, item);
					_003F val = APEnvironment.LanguageModelMgr;
					IObject @object = objectToRead.Object;
					((ILanguageModelManager)val).PutLanguageModel((ILanguageModelProvider)(object)((@object is ILanguageModelProvider) ? @object : null), false);
				}
				catch
				{
				}
			}
			_liPLCRenew.Clear();
		}

		private void OnPrimaryProjectSwitched(IProject oldProject, IProject newProject)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			MessageShown = false;
			if (_projectStructure != null && _projectStructure is IProjectStructure4)
			{
				((IProjectStructure4)_projectStructure).QueryRollback-=((EventHandler<PSQueryRollbackEventArgs>)OnQueryRollback);
				_projectStructure.Changed-=((EventHandler<PSChangedEventArgs>)ProjectChanged);
				_projectStructure = null;
			}
			if (newProject == null)
			{
				_dictLogicalNames.Clear();
				_dictMappedDevices.Clear();
				_dictHosts.Clear();
				return;
			}
			_projectStructure = APEnvironment.ObjectMgr.GetProjectStructure(newProject.Handle);
			_projectStructure.Changed+=((EventHandler<PSChangedEventArgs>)ProjectChanged);
			if (_projectStructure is IProjectStructure4)
			{
				((IProjectStructure4)_projectStructure).QueryRollback+=((EventHandler<PSQueryRollbackEventArgs>)OnQueryRollback);
			}
		}

		private void ProjectChanged(object sender, PSChangedEventArgs e)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Invalid comparison between Unknown and I4
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Invalid comparison between Unknown and I4
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Invalid comparison between Unknown and I4
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Expected O, but got Unknown
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Invalid comparison between Unknown and I4
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Invalid comparison between Unknown and I4
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Invalid comparison between Unknown and I4
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			if (_bProjectRecursion)
			{
				return;
			}
			try
			{
				_bProjectRecursion = true;
				LDictionary<Guid, IPSChange> val = new LDictionary<Guid, IPSChange>();
				IPSChange[] changes = e.Changes;
				foreach (IPSChange val2 in changes)
				{
					if (((int)val2.Action == 0 || (int)val2.Action == 4) && (typeof(IDeviceObjectBase).IsAssignableFrom(val2.AffectedNode.ObjectType) || typeof(ExplicitConnector).IsAssignableFrom(val2.AffectedNode.ObjectType)) && (s_undoMgr == null || (s_undoMgr != null && !((IUndoManager)s_undoMgr).InRedo && !((IUndoManager)s_undoMgr).InUndo)) && !val.ContainsKey(val2.AffectedNode.ObjectGuid))
					{
						val.set_Item(val2.AffectedNode.ObjectGuid, val2);
					}
				}
				Enumerator<Guid, IPSChange> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<Guid, IPSChange> current = enumerator.Current;
						bool bAutoInsert = (int)current.Value.Action == 0;
						if ((int)current.Value.Action == 4)
						{
							Enumerator<Guid, IPSChange> enumerator2 = val.GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									KeyValuePair<Guid, IPSChange> current2 = enumerator2.Current;
									if ((int)current2.Value.Action != 0 || current.Value.AffectedNode.ParentNode == null)
									{
										continue;
									}
									for (IPSNode parentNode = current.Value.AffectedNode.ParentNode; parentNode != null; parentNode = parentNode.ParentNode)
									{
										if (parentNode.ObjectGuid == current2.Value.AffectedNode.ObjectGuid)
										{
											bAutoInsert = true;
											break;
										}
									}
								}
							}
							finally
							{
								((IDisposable)enumerator2).Dispose();
							}
						}
						AutoInsertLogicalDevice((ObjectEventArgs)new ObjectAddedEventArgs(current.Value.AffectedNode.ProjectHandle, current.Key, 0, (IPastedObject)null), bAutoInsert);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				changes = e.Changes;
				foreach (IPSChange val3 in changes)
				{
					if ((int)val3.Action == 4)
					{
						if (typeof(DeviceObject).IsAssignableFrom(val3.AffectedNode.ObjectType))
						{
							foreach (Connector item in (IEnumerable)(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(val3.AffectedNode.ProjectHandle, val3.AffectedNode.ObjectGuid).Object as DeviceObject).ConnectorList(bAllGroups: true))
							{
								if (item.ConnectorGroup == 0)
								{
									continue;
								}
								IMetaObject val4 = null;
								try
								{
									val4 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val3.AffectedNode.ProjectHandle, val3.AffectedNode.ObjectGuid);
									if (val4 != null && val4.Object is DeviceObject)
									{
										(val4.Object as DeviceObject).UpdateAllIoProviders();
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
								break;
							}
						}
						if (typeof(DeviceObject).IsAssignableFrom(val3.AffectedNode.ObjectType) || typeof(ExplicitConnector).IsAssignableFrom(val3.AffectedNode.ObjectType))
						{
							MergeObjectData(val3);
						}
					}
					if ((int)val3.Action == 1 || (int)val3.Action == 2)
					{
						DeleteUnusedTasks(val3.Action);
					}
				}
				if (_liLogicalDevicesToRemove.Count <= 0)
				{
					return;
				}
				int num = -1;
				if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
				{
					num = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				}
				foreach (Guid item2 in _liLogicalDevicesToRemove)
				{
					try
					{
						if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(num, item2))
						{
							((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(num, item2);
						}
					}
					catch
					{
					}
				}
				_liLogicalDevicesToRemove.Clear();
			}
			finally
			{
				RemovedTasks.Clear();
				_bProjectRecursion = false;
			}
		}

		private static void MergeObjectData(IPSChange change)
		{
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Invalid comparison between Unknown and I4
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Expected O, but got Unknown
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Expected O, but got Unknown
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Expected O, but got Unknown
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Expected O, but got Unknown
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Expected O, but got Unknown
			if (_lastObject == null || !(change.AffectedNode.ObjectGuid == _lastObject.MetaObject.ObjectGuid))
			{
				return;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(change.AffectedNode.ProjectHandle, change.AffectedNode.ObjectGuid);
			if (metaObjectStub.SubObjectGuids.Length != _lastObject.MetaObject.SubObjectGuids.Length)
			{
				return;
			}
			for (int i = 0; i < metaObjectStub.SubObjectGuids.Length; i++)
			{
				if (metaObjectStub.SubObjectGuids[i] != _lastObject.MetaObject.SubObjectGuids[i])
				{
					return;
				}
			}
			LList<ConnectorBase> val = new LList<ConnectorBase>();
			LList<ConnectorBase> val2 = new LList<ConnectorBase>();
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(change.AffectedNode.ProjectHandle, change.AffectedNode.ObjectGuid);
			if (_lastObject is ExplicitConnector && typeof(ExplicitConnector).IsAssignableFrom(change.AffectedNode.ObjectType))
			{
				val.Add((ConnectorBase)(_lastObject as ExplicitConnector));
				val2.Add((ConnectorBase)(objectToRead.Object as ExplicitConnector));
			}
			if (_lastObject is DeviceObject && typeof(DeviceObject).IsAssignableFrom(change.AffectedNode.ObjectType))
			{
				foreach (Connector rawConnector in (_lastObject as DeviceObject).RawConnectorList)
				{
					val.Add((ConnectorBase)rawConnector);
				}
				foreach (Connector rawConnector2 in (objectToRead.Object as DeviceObject).RawConnectorList)
				{
					val2.Add((ConnectorBase)rawConnector2);
				}
			}
			LList<Guid> val3 = new LList<Guid>();
			foreach (ConnectorBase item in val)
			{
				if ((int)item.ConnectorRole == 1)
				{
					continue;
				}
				IConnector val4 = null;
				foreach (ConnectorBase item2 in val2)
				{
					if (DeviceManager.CheckMatchInterface((IConnector7)(object)item, (IConnector7)(object)item2))
					{
						val4 = (IConnector)(object)item2;
						break;
					}
				}
				if (val4 != null)
				{
					continue;
				}
				foreach (IAdapter item3 in (IEnumerable)item.Adapters)
				{
					IAdapter val5 = item3;
					val3.AddRange((IEnumerable<Guid>)val5.Modules);
				}
			}
			foreach (Guid item4 in val3)
			{
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(change.AffectedNode.ProjectHandle, item4))
				{
					try
					{
						((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(change.AffectedNode.ProjectHandle, item4);
					}
					catch
					{
					}
				}
			}
			LList<Guid> val6 = new LList<Guid>();
			LList<Guid> val7 = new LList<Guid>();
			bool flag = false;
			foreach (ConnectorBase item5 in val2)
			{
				foreach (IAdapter item6 in (IEnumerable)item5.Adapters)
				{
					IAdapter val8 = item6;
					val6.AddRange((IEnumerable<Guid>)val8.Modules);
				}
			}
			foreach (ConnectorBase item7 in val)
			{
				foreach (IAdapter item8 in (IEnumerable)item7.Adapters)
				{
					IAdapter val9 = item8;
					val7.AddRange((IEnumerable<Guid>)val9.Modules);
				}
			}
			Guid[] subObjectGuids = objectToRead.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				if (!val6.Contains(guid) && val7.Contains(guid))
				{
					flag = true;
				}
			}
			foreach (Guid item9 in val6)
			{
				if (item9 != Guid.Empty && !objectToRead.SubObjectGuids.Contains(item9) && objectToRead.ParentObjectGuid != item9)
				{
					flag = true;
				}
			}
			if (flag)
			{
				IMetaObject val10 = null;
				try
				{
					val10 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(change.AffectedNode.ProjectHandle, change.AffectedNode.ObjectGuid);
					if (val10 != null && val10.Object is DeviceObject)
					{
						foreach (Connector item10 in (IEnumerable)(val10.Object as DeviceObject).Connectors)
						{
							foreach (ConnectorBase item11 in val)
							{
								if (DeviceManager.CheckMatchInterface((IConnector7)(object)item11, (IConnector7)(object)item10))
								{
									item10.Adapters = (IAdapterList)(object)(((GenericObject)(item11.Adapters as AdapterList)).Clone() as AdapterList);
									break;
								}
							}
							if ((int)item10.ConnectorRole != 0)
							{
								continue;
							}
							foreach (IAdapter item12 in (IEnumerable)item10.Adapters)
							{
								IAdapter val11 = item12;
								subObjectGuids = val11.Modules;
								foreach (Guid guid2 in subObjectGuids)
								{
									if (!val10.SubObjectGuids.Contains(guid2))
									{
										if (val11 is VarAdapter)
										{
											(val11 as VarAdapter).Remove(guid2);
										}
										if (val11 is SlotAdapter)
										{
											(val11 as SlotAdapter).RemoveDevice(guid2);
										}
										if (val11 is FixedAdapter)
										{
											(val11 as FixedAdapter).RemoveDevice(guid2);
										}
									}
								}
							}
						}
						IDeviceProperty val12 = (IDeviceProperty)(object)new DeviceProperty(((IDeviceObject5)(val10.Object as IDeviceObjectBase)).DeviceIdentificationNoSimulation);
						val10.AddProperty((IObjectProperty)(object)val12);
					}
					if (val10 != null && val10.Object is ExplicitConnector)
					{
						ExplicitConnector explicitConnector = val10.Object as ExplicitConnector;
						foreach (ConnectorBase item13 in val)
						{
							if (DeviceManager.CheckMatchInterface((IConnector7)(object)item13, (IConnector7)(object)explicitConnector))
							{
								explicitConnector.Adapters = (IAdapterList)(object)(((GenericObject)(item13.Adapters as AdapterList)).Clone() as AdapterList);
								break;
							}
						}
						foreach (IAdapter item14 in (IEnumerable)explicitConnector.Adapters)
						{
							IAdapter val13 = item14;
							subObjectGuids = val13.Modules;
							foreach (Guid guid3 in subObjectGuids)
							{
								if (!val10.SubObjectGuids.Contains(guid3))
								{
									if (val13 is VarAdapter)
									{
										(val13 as VarAdapter).Remove(guid3);
									}
									if (val13 is SlotAdapter)
									{
										(val13 as SlotAdapter).RemoveDevice(guid3);
									}
									if (val13 is FixedAdapter)
									{
										(val13 as FixedAdapter).RemoveDevice(guid3);
									}
								}
							}
						}
						IObject @object = val10.Object;
						IExplicitConProperty val14 = (IExplicitConProperty)(object)new ModuleType(((IConnector)((@object is IExplicitConnector) ? @object : null)).ModuleType);
						val10.AddProperty((IObjectProperty)(object)val14);
					}
				}
				catch
				{
				}
				finally
				{
					if (val10 != null && val10.IsToModify)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val10, true, (object)null);
					}
				}
			}
			_lastObject = null;
		}

		private void OnQueryRollback(object sender, PSQueryRollbackEventArgs e)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Invalid comparison between Unknown and I4
			if (_bProjectRecursion)
			{
				return;
			}
			int num = -1;
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				num = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
			}
			IPSChange[] changes = ((PSChangedEventArgs)e.ChangedEventArgs).Changes;
			foreach (IPSChange val in changes)
			{
				if (val.AffectedNode.ProjectHandle != num || (int)val.Action != 4 || (!typeof(IDeviceObject).IsAssignableFrom(val.AffectedNode.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(val.AffectedNode.ObjectType) && !typeof(ISlotDeviceObject).IsAssignableFrom(val.AffectedNode.ObjectType)))
				{
					continue;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(val.AffectedNode.ProjectHandle, val.AffectedNode.ObjectGuid);
				if (!(objectToRead.Object is IOrderedSubObjects) || objectToRead.SubObjectGuids.Length == 0)
				{
					continue;
				}
				IObject @object = objectToRead.Object;
				IOrderedSubObjects val2 = (IOrderedSubObjects)(object)((@object is IOrderedSubObjects) ? @object : null);
				Guid[] subObjectGuids = objectToRead.SubObjectGuids;
				foreach (Guid guid in subObjectGuids)
				{
					if (val2.GetChildIndex(guid) < 0)
					{
						e.DoRollback();
						return;
					}
				}
			}
		}

		public static void AddObjectsToUpdate(int nProjectHandle, Guid ObjectGuid)
		{
			_dictObjectToUpdate[ObjectGuid]= nProjectHandle;
		}

		public static void DeviceObjectHelper_ProjectLoadFinished(object sender, ProjectLoadFinishedEventArgs e)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle != e.ProjectHandle)
			{
				return;
			}
			try
			{
				_bAfterLoadFinished = false;
				if (_dictObjectToUpdate.Count > 0)
				{
					LList<ILanguageModelProvider> val = new LList<ILanguageModelProvider>();
					Enumerator<Guid, int> enumerator = _dictObjectToUpdate.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<Guid, int> current = enumerator.Current;
							if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(current.Value, current.Key))
							{
								continue;
							}
							IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(current.Value, current.Key);
							IObject @object = objectToRead.Object;
							ILanguageModelProvider val2 = (ILanguageModelProvider)(object)((@object is ILanguageModelProvider) ? @object : null);
							if (val2 != null)
							{
								if (objectToRead.ParentObjectGuid == Guid.Empty)
								{
									val.Add(val2);
								}
								else
								{
									val.Insert(0, val2);
								}
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					foreach (ILanguageModelProvider item in val)
					{
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel(item, false);
						if (!(item is DeviceObject))
						{
							continue;
						}
						DeviceObject deviceObject = (DeviceObject)(object)item;
						if (deviceObject.MetaObject.ParentObjectGuid == Guid.Empty)
						{
							try
							{
								deviceObject.SetTaskLanguage(bEnable: true);
								((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)deviceObject, true);
							}
							finally
							{
								deviceObject.SetTaskLanguage(bEnable: false);
							}
						}
					}
				}
				Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(e.ProjectHandle, OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION);
				if (allObjects == null || allObjects.Length == 0)
				{
					return;
				}
				Guid[] array = allObjects;
				foreach (Guid guid in array)
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, guid);
					if (!typeof(IHiddenObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						continue;
					}
					IProject projectByHandle = ((IEngine)APEnvironment.Engine).Projects.GetProjectByHandle(e.ProjectHandle);
					IProject3 val3 = (IProject3)(object)((projectByHandle is IProject3) ? projectByHandle : null);
					bool dirty = ((IProject)val3).Dirty;
					try
					{
						((IObjectManager)APEnvironment.ObjectMgr).RemoveObject(e.ProjectHandle, guid);
					}
					catch
					{
					}
					finally
					{
						if (!dirty)
						{
							val3.SetDirty(false);
						}
					}
				}
			}
			finally
			{
				_bAfterLoadFinished = true;
			}
		}

		private bool IsLogicalDevice(IDeviceManagerInfo deviceInfo)
		{
			return IsLogicalDevice(deviceInfo.ProjectHandle, deviceInfo.ObjectGuid);
		}

		internal static bool IsLogicalDevice(int nProjectHandle, Guid objectGuid)
		{
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, objectGuid);
			if (objectToRead.Object is ILogicalDevice && ((ILogicalDevice)/*isinst with value type is only supported in some contexts*/).IsLogical)
			{
				LList<Guid> val = default(LList<Guid>);
				if (_dictLogicalNames.TryGetValue(objectToRead.Name, ref val))
				{
					if (!val.Contains(objectGuid))
					{
						val.Add(objectGuid);
					}
				}
				else
				{
					val = new LList<Guid>();
					val.Add(objectGuid);
					_dictLogicalNames.set_Item(objectToRead.Name, val);
				}
				IMetaObjectStub hostStub = GetHostStub(nProjectHandle, objectToRead.ParentObjectGuid);
				_dictHosts.set_Item(objectToRead.ObjectGuid, hostStub.ObjectGuid);
				return true;
			}
			return false;
		}

		private bool IsPhysicalDevice(IDeviceManagerInfo deviceInfo)
		{
			return IsPhysicalDevice(deviceInfo.ProjectHandle, deviceInfo.ObjectGuid);
		}

		internal static bool IsPhysicalDevice(int nProjectHandle, Guid objectGuid)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, objectGuid);
			if (objectToRead.Object is ILogicalDevice)
			{
				IObject @object = objectToRead.Object;
				ILogicalDevice val = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
				if (val.IsPhysical && val.MappedDevices != null && ((ICollection)val.MappedDevices).Count > 0)
				{
					IMetaObjectStub hostStub = GetHostStub(nProjectHandle, objectToRead.ParentObjectGuid);
					_dictHosts[objectGuid]= hostStub.ObjectGuid;
					LList<string> val2 = new LList<string>();
					LList<Guid> val3 = default(LList<Guid>);
					foreach (IMappedDevice item in (IEnumerable)val.MappedDevices)
					{
						string mappedDevice = item.MappedDevice;
						if (string.IsNullOrEmpty(mappedDevice))
						{
							continue;
						}
						val2.Add(mappedDevice);
						if (_dictMappedDevices.TryGetValue(mappedDevice, ref val3))
						{
							if (!val3.Contains(objectGuid))
							{
								val3.Add(objectGuid);
							}
						}
						else
						{
							val3 = new LList<Guid>();
							val3.Add(objectGuid);
							_dictMappedDevices[mappedDevice]= val3;
						}
					}
					Enumerator<string, LList<Guid>> enumerator2 = _dictMappedDevices.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<string, LList<Guid>> current = enumerator2.Current;
							if (!val2.Contains(current.Key) && current.Value.Contains(objectGuid))
							{
								current.Value.Remove(objectGuid);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2).Dispose();
					}
					return true;
				}
			}
			return false;
		}

		private bool IsAdditionalModule(IDeviceManagerInfo deviceInfo)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(deviceInfo.ProjectHandle, deviceInfo.ObjectGuid);
			if (typeof(IAdditionalModules).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return true;
			}
			return false;
		}

		internal static bool IsExcludedFromBuild(IMetaObject metaObject)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			IObjectProperty[] properties = metaObject.Properties;
			foreach (IObjectProperty val in properties)
			{
				if (val is IBuildProperty)
				{
					result = ((IBuildProperty)val).ExcludeFromBuild;
					break;
				}
			}
			return result;
		}

		private bool IsRootDevice(IDeviceManagerInfo deviceInfo)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(deviceInfo.ProjectHandle, deviceInfo.ObjectGuid);
			if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && metaObjectStub.ParentObjectGuid == Guid.Empty)
			{
				return true;
			}
			return false;
		}

		public void OnDescriptionInstalled(object sender, DeviceRepositoryEventArgs e)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return;
			}
			List<Guid> list = new List<Guid>();
			int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
			Enumerator<Guid, IAddressAssignmentStrategy> enumerator = _liStrategies.Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Guid current = enumerator.Current;
					if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, current))
					{
						continue;
					}
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, current);
					if (objectToRead.Object is IDeviceObject)
					{
						IObject @object = objectToRead.Object;
						IDeviceObject5 val = (IDeviceObject5)(object)((@object is IDeviceObject5) ? @object : null);
						if (val != null && ((object)val.DeviceIdentificationNoSimulation).Equals((object)e.DeviceIdentification))
						{
							list.Add(current);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			if (list.Count > 0)
			{
				ClearLanguageModel(handle, list.ToArray(), null);
			}
		}

		internal void UpdateLanguageModel(int nProjectHandle, Guid[] objectGuids)
		{
			foreach (Guid guid in objectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (metaObjectStub != null && (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(ISlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType)))
				{
					((IEngine2)APEnvironment.Engine).UpdateLanguageModel(nProjectHandle, guid);
				}
				if (metaObjectStub != null)
				{
					UpdateLanguageModel(nProjectHandle, metaObjectStub.SubObjectGuids);
				}
			}
		}

		internal static void UpdateLanguageModelAll(int nProjectHandle, Guid[] objectGuids, LList<DeviceEventArgs2> liUpdated)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			foreach (Guid guid in objectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (metaObjectStub != null && typeof(ILanguageModelProvider).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					ILanguageModelProvider val = (ILanguageModelProvider)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid).Object;
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel(val, false);
					bool flag = false;
					if (liUpdated != null)
					{
						foreach (DeviceEventArgs2 item in liUpdated)
						{
							if (((DeviceEventArgs)item).ObjectGuid == guid)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(nProjectHandle, guid);
						if (editors != null && editors.Length != 0)
						{
							IEditor[] array = editors;
							for (int j = 0; j < array.Length; j++)
							{
								array[j].Reload();
							}
						}
					}
				}
				if (metaObjectStub != null)
				{
					UpdateLanguageModelAll(nProjectHandle, metaObjectStub.SubObjectGuids, liUpdated);
				}
			}
		}

		internal static void ClearLanguageModel(int nProjectHandle, Guid[] objectGuids, LList<DeviceEventArgs2> liUpdated)
		{
			foreach (Guid guid in objectGuids)
			{
				if (HasPlcLogicObject(nProjectHandle, guid))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
					ResetPackMode();
					ResetStrategy();
					if (objectToRead.Object is DeviceObject)
					{
						UpdateAddresses((IIoProvider)(object)(objectToRead.Object as DeviceObject));
						UpdateLanguageModelAll(nProjectHandle, new Guid[1] { guid }, liUpdated);
					}
				}
			}
		}

		public static void CheckInstance()
		{
			if (_instance == null)
			{
				_instance = new DeviceObjectHelper();
			}
		}

		public static int ParseInt(string st, int nDefault)
		{
			try
			{
				if (st == null || st == string.Empty)
				{
					return nDefault;
				}
				return int.Parse(st);
			}
			catch
			{
				return nDefault;
			}
		}

		public static uint ParseUInt(string st, uint nDefault)
		{
			if (st != null && st != string.Empty)
			{
				if (st.StartsWith("16#") && uint.TryParse(st.Substring(3), NumberStyles.HexNumber, null, out var result))
				{
					return result;
				}
				if (uint.TryParse(st, out result))
				{
					return result;
				}
			}
			return nDefault;
		}

		public static bool ParseBool(string st, bool bDefault)
		{
			if (st == null)
			{
				return bDefault;
			}
			switch (st.ToLowerInvariant())
			{
			case "true":
			case "1":
				return true;
			case "false":
			case "0":
				return false;
			default:
				return bDefault;
			}
		}

		internal static AlwaysMappingMode ParseAlwaysMappingMode(string st, AlwaysMappingMode Default)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (st == null)
			{
				return Default;
			}
			string text = st.ToLowerInvariant();
			if (!(text == "onlyifunused"))
			{
				if (text == "alwaysinbuscycle")
				{
					return (AlwaysMappingMode)1;
				}
				return Default;
			}
			return (AlwaysMappingMode)0;
		}

		internal static IMetaObjectStub GetHostStub(int nProjectHandle, Guid ObjectGuid)
		{
			if (ObjectGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, ObjectGuid))
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, ObjectGuid);
				while (metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
				}
				return metaObjectStub;
			}
			return null;
		}

		public static string ParseString(string st, string stDefault)
		{
			if (st == null || st == "")
			{
				return stDefault;
			}
			return st;
		}

		internal static InteractiveLoginMode ReadDefaultLoginModeFromTargetSettings(IDeviceIdentification ident)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			InteractiveLoginMode result = (InteractiveLoginMode)0;
			if (ident != null)
			{
				ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(ident);
				result = (InteractiveLoginMode)LocalTargetSettings.InteractiveLoginMode.GetIntValue(targetSettingsById);
			}
			return result;
		}

		public static string BuildIecIdentifier(string stBase)
		{
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Invalid comparison between Unknown and I4
			bool flag = false;
			if (string.IsNullOrEmpty(stBase))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if ("0123456789".IndexOf(stBase[0]) >= 0)
			{
				stringBuilder.Append('_');
				flag = true;
			}
			foreach (char c in stBase)
			{
				if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
				{
					stringBuilder.Append(c);
					flag = false;
				}
				else if (!flag)
				{
					stringBuilder.Append('_');
					flag = true;
				}
			}
			string text = ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)15, (ushort)0) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)16, (ushort)0)) ? stringBuilder.ToString() : stringBuilder.ToString().TrimEnd('_'));
			IToken val = default(IToken);
			((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(text, false, false, false, false).GetNext(ref val);
			if ((int)val.Type != 13)
			{
				return text + "_";
			}
			return text;
		}

		public static bool CheckUniqueIdentifier(int nProjectHandle, string stIdentifier, IMetaObjectStub host)
		{
			return CheckUniqueIdentifier(nProjectHandle, stIdentifier, Guid.Empty, host, bCheckAll: false, bCheckLogical: false);
		}

		public static bool CheckUniqueIdentifier(int nProjectHandle, string stIdentifier, Guid OwnObjectGuid, IMetaObjectStub host, bool bCheckAll)
		{
			return CheckUniqueIdentifier(nProjectHandle, stIdentifier, OwnObjectGuid, host, bCheckAll, bCheckLogical: false);
		}

		public static bool CheckUniqueIdentifier(int nProjectHandle, string stIdentifier, Guid OwnObjectGuid, IMetaObjectStub host, bool bCheckAll, bool bCheckLogical)
		{
			Guid existingObjectGuid;
			return CheckUniqueIdentifier(nProjectHandle, stIdentifier, OwnObjectGuid, host, bCheckAll, bCheckLogical, out existingObjectGuid);
		}

		public static bool CheckUniqueIdentifier(int nProjectHandle, string stIdentifier, Guid OwnObjectGuid, IMetaObjectStub host, bool bCheckAll, bool bCheckLogical, out Guid existingObjectGuid)
		{
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Invalid comparison between Unknown and I4
			existingObjectGuid = Guid.Empty;
			if (stIdentifier == "")
			{
				return false;
			}
			bool flag = bCheckLogical;
			if (OwnObjectGuid != Guid.Empty)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, OwnObjectGuid);
				if (typeof(SlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, OwnObjectGuid);
					if (objectToRead.Object is SlotDeviceObject && !((SlotDeviceObject)(object)objectToRead.Object).HasDevice)
					{
						return true;
					}
					if (objectToRead.Name.StartsWith("<") && objectToRead.Name.EndsWith(">"))
					{
						return true;
					}
				}
				if (typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					flag = true;
				}
			}
			else
			{
				IMetaObjectStub val = DeviceCommandHelper.GetSelectedStub(bAll: true);
				if (val != null)
				{
					while (val.ParentObjectGuid != Guid.Empty)
					{
						if (typeof(ILogicalObject).IsAssignableFrom(val.ObjectType))
						{
							flag = true;
							break;
						}
						val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val.ProjectHandle, val.ParentObjectGuid);
					}
				}
			}
			Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(nProjectHandle, stIdentifier);
			foreach (Guid guid in allObjects)
			{
				if (!(guid != OwnObjectGuid))
				{
					continue;
				}
				IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				bool flag2 = false;
				if (metaObjectStub2.Namespace == LogicalExchangeGVLObject.GUID_POUNAMESPACE && !typeof(LogicalExchangeGVLObject).IsAssignableFrom(metaObjectStub2.ObjectType))
				{
					flag2 = true;
				}
				bool flag3 = typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub2.ObjectType);
				if ((!flag && !flag3 && typeof(IDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType)) || (flag && flag3) || bCheckAll || flag2)
				{
					existingObjectGuid = guid;
					if (host == null)
					{
						return false;
					}
					metaObjectStub2 = GetHostStub(nProjectHandle, guid);
					if (metaObjectStub2 == null)
					{
						return false;
					}
					if (host.ObjectGuid == metaObjectStub2.ObjectGuid)
					{
						return false;
					}
				}
			}
			IScanner obj = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(stIdentifier, false, false, false, false);
			obj.AllowMultipleUnderlines=(true);
			IToken val2 = default(IToken);
			obj.GetNext(ref val2);
			if ((int)val2.Type != 13)
			{
				return false;
			}
			return true;
		}

		public static string CreateUniqueIdentifier(int nProjectHandle, string stBaseName, IMetaObjectStub host)
		{
			return CreateUniqueIdentifier(nProjectHandle, stBaseName, Guid.Empty, host, bCheckAll: false, bCheckLogical: false);
		}

		public static string CreateUniqueIdentifier(int nProjectHandle, string stBaseName, Guid ownGuid, IMetaObjectStub host)
		{
			return CreateUniqueIdentifier(nProjectHandle, stBaseName, ownGuid, host, bCheckAll: false, bCheckLogical: false);
		}

		public static string CreateUniqueIdentifier(int nProjectHandle, string stBaseName, IMetaObjectStub host, bool bCheckAll)
		{
			return CreateUniqueIdentifier(nProjectHandle, stBaseName, Guid.Empty, host, bCheckAll, bCheckLogical: false);
		}

		public static string CreateUniqueIdentifier(int nProjectHandle, string stBaseName, Guid ownGuid, IMetaObjectStub host, bool bCheckAll, bool bCheckLogical)
		{
			int num = 1;
			if (host != null && host.ParentObjectGuid != Guid.Empty && !typeof(IApplicationObject).IsAssignableFrom(host.ObjectType))
			{
				host = GetHostStub(host.ProjectHandle, host.ObjectGuid);
			}
			string text = BuildIecIdentifier(stBaseName);
			while (!CheckUniqueIdentifier(nProjectHandle, text, ownGuid, host, bCheckAll, bCheckLogical))
			{
				text = ((!stBaseName.EndsWith("_")) ? (stBaseName + "_" + num) : (stBaseName + num));
				num++;
			}
			return text;
		}

		public static IMetaObject GetTaskConfig(IMetaObject moApp)
		{
			Debug.Assert(moApp.Object is IApplicationObject);
			Guid[] subObjectGuids = moApp.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(moApp.ProjectHandle, guid);
				if (typeof(ITaskConfigObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(moApp.ProjectHandle, guid);
				}
			}
			return null;
		}

		public static IMetaObject GetTask(IMetaObject moTaskConfig, string stName)
		{
			Debug.Assert(moTaskConfig.Object is ITaskConfigObject);
			Guid[] subObjectGuids = moTaskConfig.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(moTaskConfig.ProjectHandle, guid);
				if (typeof(ITaskObject).IsAssignableFrom(metaObjectStub.ObjectType) && metaObjectStub.Name == stName)
				{
					return ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
				}
			}
			return null;
		}

		public static void UpdateCatalogueContext(IDeviceCatalogue catalogue)
		{
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Expected O, but got Unknown
			catalogue.BeginUpdate();
			try
			{
				catalogue.Filter=((IDeviceCatalogueFilter)null);
				catalogue.Context=((IDeviceIdentification)null);
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject == null)
				{
					Debug.Fail("Expected a primary project");
					return;
				}
				ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
				if (selectedSVNodes == null || selectedSVNodes.Length == 0)
				{
					catalogue.Filter=(catalogue.CreateChildConnectorFilter((string[])null));
					return;
				}
				ISVNode[] array = selectedSVNodes;
				for (int i = 0; i < array.Length; i++)
				{
					IMetaObject objectToRead = array[i].GetObjectToRead();
					if (objectToRead == null)
					{
						continue;
					}
					IObject @object = objectToRead.Object;
					IConnector val = (IConnector)(object)((@object is IConnector) ? @object : null);
					IDeviceObjectBase deviceObjectBase = objectToRead.Object as IDeviceObjectBase;
					if (val != null)
					{
						IDeviceCatalogueFilter filter = catalogue.CreateChildConnectorFilter(new string[1] { val.Interface });
						catalogue.Filter=(filter);
						IDeviceObject deviceObject = val.GetDeviceObject();
						if (deviceObject != null)
						{
							catalogue.Context=(((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation);
						}
						if (catalogue is IDeviceCatalogue3)
						{
							IDeviceIdentification[] array2 = null;
							ArrayList arrayList = new ArrayList();
							if (val is IConnector8)
							{
								arrayList.AddRange(((IConnector8)((val is IConnector8) ? val : null)).AllowOnlyDevices);
							}
							array2 = (IDeviceIdentification[])(object)new IDeviceIdentification[arrayList.Count];
							arrayList.CopyTo(array2);
							((IDeviceCatalogue3)((catalogue is IDeviceCatalogue3) ? catalogue : null)).AllowOnly=(array2);
						}
						break;
					}
					if (deviceObjectBase == null)
					{
						continue;
					}
					IDeviceCatalogueFilter filter2 = catalogue.CreateChildConnectorFilter(((IDeviceObject)deviceObjectBase).GetPossibleInterfacesForInsert(objectToRead.SubObjectGuids.Length));
					catalogue.Filter=(filter2);
					catalogue.Context=(((IDeviceObject5)deviceObjectBase).DeviceIdentificationNoSimulation);
					if (!(catalogue is IDeviceCatalogue3))
					{
						break;
					}
					IDeviceIdentification[] array3 = null;
					ArrayList arrayList2 = new ArrayList();
					foreach (IConnector8 item in (IEnumerable)((IDeviceObject)deviceObjectBase).Connectors)
					{
						IConnector8 val2 = item;
						arrayList2.AddRange(val2.AllowOnlyDevices);
					}
					array3 = (IDeviceIdentification[])(object)new IDeviceIdentification[arrayList2.Count];
					arrayList2.CopyTo(array3);
					((IDeviceCatalogue3)((catalogue is IDeviceCatalogue3) ? catalogue : null)).AllowOnly=(array3);
					break;
				}
			}
			finally
			{
				catalogue.EndUpdate();
			}
		}

		public static bool BeginGetLanguageModel(Guid guidObject)
		{
			if (_htDevicesInGetLanguageModel.Contains(guidObject))
			{
				return false;
			}
			_htDevicesInGetLanguageModel[guidObject] = new object();
			return true;
		}

		public static void EndGetLanguageModel(Guid guidObject)
		{
			_htDevicesInGetLanguageModel.Remove(guidObject);
		}

		public static void BeginCreateDevice(Guid guidDevice)
		{
			_htDevicesDuringCreate[guidDevice] = new object();
		}

		public static void EndCreateDevice(Guid guidDevice)
		{
			_htDevicesDuringCreate.Remove(guidDevice);
		}

		public static int GetEmptyChildConnectorForInterface(string stInterface, IDeviceObject2 device)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I4
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Invalid comparison between Unknown and I4
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Expected O, but got Unknown
			foreach (IConnector item in (IEnumerable)((IDeviceObject)device).Connectors)
			{
				IConnector val = item;
				if (DeviceManager.CompareInterfaces(val.Interface, stInterface) && (int)val.ConnectorRole == 1)
				{
					foreach (IAdapter item2 in (IEnumerable)val.Adapters)
					{
						IAdapter val2 = item2;
						if (val2 is SlotAdapter && val2.ModulesCount == 1 && val2.Modules[0] == Guid.Empty)
						{
							return val.ConnectorId;
						}
					}
				}
				if (!(val is IConnector7))
				{
					continue;
				}
				foreach (string additionalInterface in ((IConnector7)((val is IConnector7) ? val : null)).AdditionalInterfaces)
				{
					if (!DeviceManager.CompareInterfaces(additionalInterface, stInterface) || (int)val.ConnectorRole != 1)
					{
						continue;
					}
					foreach (IAdapter item3 in (IEnumerable)val.Adapters)
					{
						IAdapter val3 = item3;
						if (val3 is SlotAdapter && val3.ModulesCount == 1 && val3.Modules[0] == Guid.Empty)
						{
							return val.ConnectorId;
						}
					}
				}
			}
			return -1;
		}

		public static string GetApplicationName(IApplicationObject app)
		{
			if (app is IOnlineApplicationObject2)
			{
				return ((IObject)app).MetaObject.Name;
			}
			return ((IOnlineApplicationObject)app).Name;
		}

		public static string GetBaseName(string stPattern, string stDeviceName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (num < stPattern.Length)
			{
				int num2 = stPattern.IndexOf("$(", num);
				if (num2 > num)
				{
					stringBuilder.Append(stPattern.Substring(num, num2 - num));
				}
				if (num2 < 0)
				{
					stringBuilder.Append(stPattern, num, stPattern.Length - num);
					break;
				}
				int num3 = num2 + 2;
				int num4 = ((num3 < stPattern.Length) ? stPattern.IndexOf(")", num3) : (-1));
				if (num4 < 0)
				{
					break;
				}
				if (stPattern.Substring(num3, num4 - num3).Equals("DeviceName"))
				{
					stringBuilder.Append(stDeviceName);
				}
				num = num4 + 1;
			}
			string text = stringBuilder.ToString();
			if (text.StartsWith("@"))
			{
				return text;
			}
			return BuildIecIdentifier(text);
		}

		public static string CreateInstanceNameBase(IDeviceInfo info)
		{
			IDeviceInfo2 val = (IDeviceInfo2)(object)((info is IDeviceInfo2) ? info : null);
			if (val != null && val.DefaultInstanceName != null)
			{
				return BuildIecIdentifier(val.DefaultInstanceName);
			}
			return BuildIecIdentifier(info.Name);
		}

		private void OnProjectClosed(object sender, ProjectClosedEventArgs e)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject != null && primaryProject.Handle == e.ProjectHandle)
			{
				_htDevicesInGetLanguageModel.Clear();
				_htConfigModeApplication.Clear();
				_htIecAddresses.Clear();
				_liStrategies.Clear();
				LanguageModelHelper.ClearDiagnosisInstance();
			}
		}

		private void ObjectMgr_ProjectSaving(object sender, ProjectSavingEventArgs e)
		{
			bool flag = true;
			ProjectSavingEventArgs2 val = (ProjectSavingEventArgs2)(object)((e is ProjectSavingEventArgs2) ? e : null);
			VersionConstraint val2 = null;
			if (val != null && val.Profile != null)
			{
				val2 = val.Profile.GetVersionConstraint(GUID_DEVICEOBJECTPLUGIN);
				if (val2 is ExactVersionConstraint && ((ExactVersionConstraint)((val2 is ExactVersionConstraint) ? val2 : null)).Version < V3_3_2_0)
				{
					flag = false;
				}
			}
			Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(e.ProjectHandle);
			foreach (Guid guid in allObjects)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, guid);
				if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					continue;
				}
				bool flag2 = false;
				IObjectProperty[] properties = metaObjectStub.Properties;
				foreach (IObjectProperty obj in properties)
				{
					if (obj is IDeviceProperty)
					{
						flag2 = true;
					}
					if (obj is IExplicitConProperty)
					{
						flag2 = true;
					}
				}
				if (!(!flag2 && flag) && (!flag2 || flag))
				{
					continue;
				}
				IMetaObject val3 = null;
				try
				{
					APEnvironment.Engine.BeginSuppressUpdateLanguageModel();
					val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(e.ProjectHandle, guid);
					if (val3 == null)
					{
						continue;
					}
					if (flag)
					{
						if (val3.Object is IDeviceObjectBase)
						{
							IDeviceProperty val4 = (IDeviceProperty)(object)new DeviceProperty(((IDeviceObject5)(val3.Object as IDeviceObjectBase)).DeviceIdentificationNoSimulation);
							val3.AddProperty((IObjectProperty)(object)val4);
						}
						else if (val3.Object is ExplicitConnector)
						{
							IObject @object = val3.Object;
							IExplicitConProperty val5 = (IExplicitConProperty)(object)new ModuleType(((IConnector)((@object is IExplicitConnector) ? @object : null)).ModuleType);
							val3.AddProperty((IObjectProperty)(object)val5);
						}
					}
					else if (val3.Object is IDeviceObjectBase)
					{
						val3.RemoveProperty(DeviceProperty.Guid);
					}
					else if (val3.Object is ExplicitConnector)
					{
						val3.RemoveProperty(ModuleType.Guid);
					}
				}
				catch
				{
				}
				finally
				{
					if (val3 != null && val3.IsToModify)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
					}
					APEnvironment.Engine.EndSuppressUpdateLanguageModel();
				}
			}
		}

		private void ObjectMgr_ProjectSaved(object sender, ProjectSavedEventArgs e)
		{
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			if (e.Stream is MemoryStream)
			{
				return;
			}
			Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(e.ProjectHandle);
			foreach (Guid guid in allObjects)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, guid);
				if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					continue;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, guid);
				bool flag = false;
				if (objectToRead != null && objectToRead.Object is IDeviceObject)
				{
					IObject @object = objectToRead.Object;
					IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
					if (val.DeviceParameterSet is ParameterSet && (val.DeviceParameterSet as ParameterSet).HasFileParams)
					{
						flag = true;
					}
					foreach (IConnector item in (IEnumerable)val.Connectors)
					{
						IConnector val2 = item;
						if (val2.HostParameterSet is ParameterSet && (val2.HostParameterSet as ParameterSet).HasFileParams)
						{
							flag = true;
						}
					}
					if (flag && val is IDeviceObjectBase)
					{
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(val as IDeviceObjectBase), true);
					}
				}
				if (objectToRead != null && objectToRead.Object is IExplicitConnector)
				{
					IObject object2 = objectToRead.Object;
					IExplicitConnector val3 = (IExplicitConnector)(object)((object2 is IExplicitConnector) ? object2 : null);
					if (((IConnector)val3).HostParameterSet is ParameterSet && (((IConnector)val3).HostParameterSet as ParameterSet).HasFileParams)
					{
						flag = true;
					}
					if (flag && val3 is ExplicitConnector)
					{
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)(val3 as ExplicitConnector), true);
					}
				}
			}
		}

		private void DownloadParamFile(Parameter param, IOnlineDevice3 onlineDevice)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Expected I4, but got Unknown
			IProgressCallback val = ((IEngine)APEnvironment.Engine).StartLengthyOperation();
			val.Abortable=(true);
			try
			{
				APEnvironment.FrameForm.Cursor=(Cursors.WaitCursor);
				string text = param.ResolveFileParameter(param.Value.Replace("'", ""));
				((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (AuthFile.Exists(text))
				{
					Stream stream = (Stream)new AuthFileStream(text, FileMode.Open, FileAccess.Read);
					string fileName = Path.GetFileName(text);
					IFileDownload obj = ((IOnlineDevice2)onlineDevice).CreateFileDownload(stream, fileName);
					IFileDownload2 val2 = (IFileDownload2)(object)((obj is IFileDownload2) ? obj : null);
					if (val2 == null)
					{
						return;
					}
					if (val2 is ICheckedFileTransfer)
					{
						((ICheckedFileTransfer)val2).DisableCheckedFileTransfer();
					}
					val.NextTask(text, (int)stream.Length, " bytes");
					IAsyncResult asyncResult = ((IFileDownload)val2).BeginDownload(true, (AsyncCallback)null, (object)null);
					long num = 0L;
					long num2 = -1L;
					long num3 = 0L;
					try
					{
						DownloadProgress val3 = default(DownloadProgress);
						while (!asyncResult.AsyncWaitHandle.WaitOne(10, exitContext: false))
						{
							val2.GetProgress(asyncResult, ref val3, ref num, ref num2);
							switch (val3 - 1)
							{
							case 0:
								val.TaskProgress("Initialize", 0);
								break;
							case 1:
								if (num > num3)
								{
									val.TaskProgress($"Download {num}", (int)(num - num3));
									num3 = num;
								}
								break;
							case 2:
								val.TaskProgress("Finish", 0);
								break;
							}
							if (val.Aborting)
							{
								((IFileDownload)val2).CancelDownload(asyncResult);
							}
							Application.DoEvents();
						}
					}
					catch
					{
					}
					finally
					{
						stream.Close();
						((IFileDownload)val2).EndDownload(asyncResult);
					}
					return;
				}
				string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ErrorFileMissing");
				Guid objectGuid = Guid.Empty;
				long lPosition = -1L;
				if (param != null)
				{
					objectGuid = ((IObject)param.GetAssociatedDeviceObject).MetaObject.ObjectGuid;
					lPosition = param.LanguageModelPositionId;
				}
				DeviceMessage deviceMessage = new DeviceMessage(string.Format(@string, text), (Severity)2, objectGuid, lPosition);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
			}
			catch
			{
			}
			finally
			{
				APEnvironment.FrameForm.Cursor=(Cursors.Default);
				val.Finish();
			}
		}

		private void DownloadAllFiles(int nProjectHandle, Guid deviceGuid, IOnlineDevice3 onlineDevice)
		{
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Expected O, but got Unknown
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, deviceGuid);
			Guid[] subObjectGuids = metaObjectStub.SubObjectGuids;
			foreach (Guid deviceGuid2 in subObjectGuids)
			{
				DownloadAllFiles(nProjectHandle, deviceGuid2, onlineDevice);
			}
			if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, deviceGuid);
			if (objectToRead != null && objectToRead.Object is IDeviceObject)
			{
				IObject @object = objectToRead.Object;
				IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				if (val.DeviceParameterSet is ParameterSet && (val.DeviceParameterSet as ParameterSet).HasFileParams)
				{
					foreach (Parameter item in (IEnumerable)val.DeviceParameterSet)
					{
						if (item.IsFileType)
						{
							DownloadParamFile(item, onlineDevice);
						}
					}
				}
				foreach (IConnector item2 in (IEnumerable)val.Connectors)
				{
					IConnector val2 = item2;
					if (!(val2.HostParameterSet is ParameterSet) || !(val2.HostParameterSet as ParameterSet).HasFileParams)
					{
						continue;
					}
					foreach (Parameter item3 in (IEnumerable)val2.HostParameterSet)
					{
						if (item3.IsFileType)
						{
							DownloadParamFile(item3, onlineDevice);
						}
					}
				}
			}
			if (objectToRead == null || !(objectToRead.Object is IExplicitConnector))
			{
				return;
			}
			IObject object2 = objectToRead.Object;
			IExplicitConnector val3 = (IExplicitConnector)(object)((object2 is IExplicitConnector) ? object2 : null);
			if (!(((IConnector)val3).HostParameterSet is ParameterSet) || !(((IConnector)val3).HostParameterSet as ParameterSet).HasFileParams)
			{
				return;
			}
			foreach (Parameter item4 in (IEnumerable)((IConnector)val3).HostParameterSet)
			{
				if (item4.IsFileType)
				{
					DownloadParamFile(item4, onlineDevice);
				}
			}
		}

		private void BeforeApplicationDownload(object sender, OnlineCancelEventArgs e)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			APEnvironment.MessageStorage.ClearMessages((IMessageCategory)(object)DeviceMessageCategory.Instance);
			if (primaryProject != null)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(primaryProject.Handle, e.ObjectGuid);
				while (metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(primaryProject.Handle, metaObjectStub.ParentObjectGuid);
				}
				IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(metaObjectStub.ObjectGuid);
				IOnlineDevice3 val = (IOnlineDevice3)(object)((onlineDevice is IOnlineDevice3) ? onlineDevice : null);
				if (val != null && ((IOnlineDevice)val).IsConnected)
				{
					DownloadAllFiles(primaryProject.Handle, metaObjectStub.ObjectGuid, val);
				}
			}
		}

		private void OnObjectAdding(object sender, ObjectAddingEventArgs e)
		{
			if (!DeviceManager.DoNotSkipEvents && (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || e.ProjectHandle != ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle))
			{
				return;
			}
			if (e is ObjectAddingEventArgs3)
			{
				ObjectAddingEventArgs3 val = (ObjectAddingEventArgs3)(object)((e is ObjectAddingEventArgs3) ? e : null);
				if (((ObjectAddingEventArgs2)val).PastedObject != null)
				{
					if (((ObjectAddingEventArgs2)val).PastedObject.Object is IDeviceObject5 && ((IDeviceObject5)/*isinst with value type is only supported in some contexts*/).DeviceIdentificationNoSimulation is IModuleIdentification)
					{
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ParentObjectGuid);
						if (objectToRead != null && objectToRead.Object is IDeviceObject5)
						{
							IObject @object = objectToRead.Object;
							IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((@object is IDeviceObject5) ? @object : null)).DeviceIdentificationNoSimulation;
							IObject object2 = ((ObjectAddingEventArgs2)val).PastedObject.Object;
							IDeviceIdentification deviceIdentificationNoSimulation2 = ((IDeviceObject5)((object2 is IDeviceObject5) ? object2 : null)).DeviceIdentificationNoSimulation;
							if (deviceIdentificationNoSimulation2.Type != deviceIdentificationNoSimulation.Type || deviceIdentificationNoSimulation2.Id != deviceIdentificationNoSimulation.Id || deviceIdentificationNoSimulation2.Version != deviceIdentificationNoSimulation.Version)
							{
								throw new Exception(Strings.ErrorAddingModules);
							}
						}
					}
					if (((ObjectAddingEventArgs2)val).PastedObject.ParentSVNodeGuid.Equals(((ObjectAddingEventArgs2)val).PastedObject.OldParentSVNodeGuid) && val.NewGuid.Equals(((ObjectAddingEventArgs2)val).PastedObject.OldObjectGuid) && !IsObjectAddedInRCSContext(((ObjectAddingEventArgs2)val).PastedObject))
					{
						return;
					}
				}
			}
			IMetaObject val2 = null;
			val2 = null;
			bool flag = false;
			try
			{
				if (!(e.Object is IDeviceObject) || !(e.ParentObjectGuid != Guid.Empty))
				{
					goto IL_01be;
				}
				if (_htDevicesDuringCreate.Contains(e.ParentObjectGuid))
				{
					return;
				}
				val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(e.ProjectHandle, e.ParentObjectGuid);
				goto IL_01be;
				IL_01be:
				IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(e.ProjectHandle);
				if (undoManager != null && undoManager.InUndo && undoManager.NextRedoAction == ReplaceDeviceAction.REPLACEACTION)
				{
					return;
				}
				if (e.Object is IDeviceObjectBase)
				{
					((IDeviceObjectBase)e.Object).PreparePaste();
				}
				else if (e.Object is ExplicitConnector)
				{
					((ExplicitConnector)(object)e.Object).PreparePaste();
				}
				if (val2 != null)
				{
					if (e is ObjectAddingEventArgs2 && ((ObjectAddingEventArgs2)((e is ObjectAddingEventArgs2) ? e : null)).PastedObject != null)
					{
						IPastedObject pastedObject = ((ObjectAddingEventArgs2)((e is ObjectAddingEventArgs2) ? e : null)).PastedObject;
						if (val2.Object is IDeviceObjectBase && pastedObject.OldObjectGuid != pastedObject.ObjectGuid)
						{
							IDeviceObjectBase deviceObjectBase = (IDeviceObjectBase)val2.Object;
							if (((IOrderedSubObjects)deviceObjectBase).GetChildIndex(pastedObject.OldObjectGuid) >= 0)
							{
								deviceObjectBase.UpdatePasteModuleGuid(pastedObject.OldObjectGuid, pastedObject.ObjectGuid);
								flag = true;
							}
						}
						if (val2.Object is ExplicitConnector && pastedObject.OldObjectGuid != pastedObject.ObjectGuid)
						{
							ExplicitConnector explicitConnector = (ExplicitConnector)(object)val2.Object;
							if (explicitConnector.GetChildIndex(pastedObject.OldObjectGuid) >= 0)
							{
								explicitConnector.UpdatePasteModuleGuid(pastedObject.OldObjectGuid, pastedObject.ObjectGuid);
								flag = true;
							}
						}
					}
					if (undoManager != null && !undoManager.InRedo && !undoManager.InUndo && e is ObjectAddingEventArgs2)
					{
						Guid guid = Guid.Empty;
						if (((ObjectAddingEventArgs2)((e is ObjectAddingEventArgs2) ? e : null)).PastedObject != null)
						{
							guid = ((ObjectAddingEventArgs2)((e is ObjectAddingEventArgs2) ? e : null)).PastedObject.ObjectGuid;
						}
						if (val2.Object is DeviceObject)
						{
							((DeviceObject)(object)val2.Object).InsertChild(e.Index, e.Object, guid);
						}
						else if (val2.Object is ExplicitConnector)
						{
							((ExplicitConnector)(object)val2.Object).InsertChild(e.Index, e.Object, guid);
						}
					}
				}
				if (e is ObjectAddingEventArgs2 && ((ObjectAddingEventArgs2)((e is ObjectAddingEventArgs2) ? e : null)).PastedObject != null)
				{
					IPastedObject pastedObject2 = ((ObjectAddingEventArgs2)((e is ObjectAddingEventArgs2) ? e : null)).PastedObject;
					if (pastedObject2.Object is IDeviceObjectBase && pastedObject2.OldParentSVNodeGuid != pastedObject2.ParentSVNodeGuid)
					{
						((IDeviceObjectBase)pastedObject2.Object).UpdatePasteModuleGuid(pastedObject2.OldParentSVNodeGuid, pastedObject2.ParentSVNodeGuid);
					}
				}
			}
			catch (Exception ex)
			{
				e.Cancel(ex);
			}
			finally
			{
				if (val2 != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val2, flag, (object)null);
				}
			}
		}

		private void OnObjectLoaded(object sender, ObjectEventArgs e)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null || primaryProject.Handle != e.ProjectHandle)
			{
				return;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
			if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return;
			}
			IMetaObject val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ObjectGuid);
			Debug.Assert(val != null);
			IDeviceObjectBase deviceObjectBase = val.Object as IDeviceObjectBase;
			if (deviceObjectBase == null || deviceObjectBase.CheckLanguageModelGuids())
			{
				return;
			}
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val);
				if (val != null && val.IsToModify)
				{
					Debug.Assert(val != null);
					deviceObjectBase = (IDeviceObjectBase)val.Object;
					deviceObjectBase.UpdateLanguageModelGuids(bUpgrade: true);
				}
			}
			catch (Exception value)
			{
				Debug.WriteLine(value);
			}
			finally
			{
				if (val != null && val.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
				}
			}
		}

		internal static bool MapLogicalDevice(IMetaObject metaAdded, string stNameLogical, int iMappedIndex)
		{
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Expected O, but got Unknown
			IObject @object = metaAdded.Object;
			ILogicalDevice val = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
			Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(metaAdded.ProjectHandle, stNameLogical);
			foreach (Guid guid in allObjects)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaAdded.ProjectHandle, guid);
				if (!typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType) || GetHostStub(metaAdded.ProjectHandle, guid) != GetHostStub(metaAdded.ProjectHandle, metaAdded.ObjectGuid) || ((ICollection)val.MappedDevices).Count <= iMappedIndex)
				{
					continue;
				}
				LogicalIODevice obj = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaAdded.ProjectHandle, guid).Object as LogicalIODevice;
				bool flag = false;
				foreach (IMappedDevice item in (IEnumerable)obj.MappedDevices)
				{
					IMappedDevice val2 = item;
					flag |= val2.IsMapped;
				}
				if (flag)
				{
					continue;
				}
				IMappedDevice val3 = val.MappedDevices[iMappedIndex];
				if (val.IsPhysical && !val3.IsMapped)
				{
					if (metaAdded.IsToModify)
					{
						val3.MappedDevice=(stNameLogical);
					}
					return true;
				}
			}
			return false;
		}

		internal static bool IsObjectAddedInRCSContext(IPastedObject po)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			IPastedObject3 val = (IPastedObject3)(object)((po is IPastedObject3) ? po : null);
			if (val != null)
			{
				if ((int)val.ActionContext == 2)
				{
					return true;
				}
			}
			else
			{
				string[] rcsPlugins = new string[2] { "svn.plugin.dll", "git.plugin.dll" };
				if (new StackTrace().GetFrames().Any((StackFrame f) => rcsPlugins.Any((string rcs) => f.GetMethod().Module.Name.Equals(rcs, StringComparison.InvariantCultureIgnoreCase))))
				{
					return true;
				}
			}
			return false;
		}

		private void OnObjectAdded(object sender, ObjectEventArgs e)
		{
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Expected O, but got Unknown
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Expected O, but got Unknown
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(e.ProjectHandle);
			if ((undoManager != null && (undoManager.InRedo || undoManager.InUndo)) || (!DeviceManager.DoNotSkipEvents && (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || e.ProjectHandle != ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle)))
			{
				return;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
			if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject == null || primaryProject.Handle != e.ProjectHandle)
				{
					return;
				}
				IMetaObjectStub val = null;
				if (metaObjectStub.ParentObjectGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(primaryProject.Handle, metaObjectStub.ParentObjectGuid))
				{
					val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(primaryProject.Handle, metaObjectStub.ParentObjectGuid);
				}
				if (val != null && typeof(IDeviceApplication).IsAssignableFrom(val.ObjectType))
				{
					return;
				}
				Guid deviceGuid = ((IOnlineApplicationObject)(IApplicationObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ObjectGuid).Object).DeviceGuid;
				if (!(deviceGuid != Guid.Empty))
				{
					return;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, deviceGuid);
				IDeviceObjectBase deviceObjectBase = objectToRead.Object as IDeviceObjectBase;
				if (deviceObjectBase == null)
				{
					return;
				}
				IDriverInfo driverInfo = ((IDeviceObject2)deviceObjectBase).DriverInfo;
				Guid guid = ((IDriverInfo2)((driverInfo is IDriverInfo2) ? driverInfo : null)).IoApplication;
				if (e is ObjectAddedEventArgs && ((ObjectAddedEventArgs)((e is ObjectAddedEventArgs) ? e : null)).PastedObject != null)
				{
					IPastedObject pastedObject = ((ObjectAddedEventArgs)((e is ObjectAddedEventArgs) ? e : null)).PastedObject;
					if (pastedObject.OldObjectGuid == guid && GetHostStub(e.ProjectHandle, pastedObject.OldObjectGuid) != GetHostStub(e.ProjectHandle, pastedObject.ObjectGuid))
					{
						guid = Guid.Empty;
					}
				}
				if (guid == Guid.Empty || !((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.ProjectHandle, guid))
				{
					guid = e.ObjectGuid;
					IMetaObject val2 = null;
					try
					{
						val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(e.ProjectHandle, deviceGuid);
						IDriverInfo driverInfo2 = ((IDeviceObject2)(val2.Object as IDeviceObjectBase)).DriverInfo;
						((IDriverInfo2)((driverInfo2 is IDriverInfo2) ? driverInfo2 : null)).IoApplication=(guid);
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
				if (guid == e.ObjectGuid)
				{
					try
					{
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(e.ProjectHandle, deviceGuid);
						SuppressUpdateObjects(bUpdateObjectSuppressed: true);
						UpdateLanguageModel(e.ProjectHandle, objectToRead.SubObjectGuids);
					}
					finally
					{
						SuppressUpdateObjects(bUpdateObjectSuppressed: false);
					}
				}
				if (deviceObjectBase is DeviceObject)
				{
					DeviceObject.CreateTasks((IIoProvider)((deviceObjectBase is IIoProvider) ? deviceObjectBase : null));
					foreach (IConnector item in (IEnumerable)((IDeviceObject)deviceObjectBase).Connectors)
					{
						object obj2 = (object)item;
						DeviceObject.CreateTasks((IIoProvider)((obj2 is IIoProvider) ? obj2 : null));
					}
				}
				((IEngine2)APEnvironment.Engine).UpdateLanguageModel(e.ProjectHandle, deviceGuid);
			}
			else
			{
				if (!typeof(IDeviceObjectBase).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(ExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return;
				}
				IMetaObject val3 = null;
				try
				{
					val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(e.ProjectHandle, e.ObjectGuid);
					bool flag = false;
					IObjectProperty[] properties = val3.Properties;
					foreach (IObjectProperty val4 in properties)
					{
						if ((val4 is IDeviceProperty && val3.Object is IDeviceObjectBase) || (val4 is IExplicitConProperty && val3.Object is ExplicitConnector))
						{
							flag = true;
						}
					}
					if (val3.Object is IDeviceObjectBase)
					{
						if (!flag)
						{
							IDeviceProperty val5 = (IDeviceProperty)(object)new DeviceProperty(((IDeviceObject5)(val3.Object as IDeviceObjectBase)).DeviceIdentificationNoSimulation);
							val3.AddProperty((IObjectProperty)(object)val5);
						}
						((IDeviceObjectBase)val3.Object).OnAfterAdded();
					}
					else if (val3.Object is ExplicitConnector)
					{
						if (!flag)
						{
							IObject @object = val3.Object;
							IExplicitConProperty val6 = (IExplicitConProperty)(object)new ModuleType(((IConnector)((@object is IExplicitConnector) ? @object : null)).ModuleType);
							val3.AddProperty((IObjectProperty)(object)val6);
						}
						((ExplicitConnector)(object)val3.Object).OnAfterAdded();
					}
					IObject object2 = val3.Object;
					AutoSetActivePathForChildPlc((IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null), Guid.Empty, null);
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
					if ((!(val3.Object is SlotDeviceObject) || ((SlotDeviceObject)(object)val3.Object).HasDevice) && !CheckUniqueIdentifier(e.ProjectHandle, val3.Name, e.ObjectGuid, GetHostStub(e.ProjectHandle, e.ObjectGuid), bCheckAll: false) && val3.Object is IDeviceObjectBase)
					{
						bool flag2 = bRenameInRecursion;
						try
						{
							bRenameInRecursion = true;
							string text = CreateUniqueIdentifier(e.ProjectHandle, val3.Name, e.ObjectGuid, GetHostStub(e.ProjectHandle, e.ObjectGuid));
							((IObjectManager)APEnvironment.ObjectMgr).RenameObject(e.ProjectHandle, e.ObjectGuid, text);
						}
						finally
						{
							bRenameInRecursion = flag2;
						}
					}
				}
				catch
				{
					if (val3 != null && val3.IsToModify)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, false, (object)null);
					}
					throw;
				}
				AddListLanguageModel(val3.ProjectHandle, val3.ObjectGuid, bIsTaskLanguageModel: false);
				if (!AddListLanguageModel(val3.ProjectHandle, val3.ObjectGuid, bIsTaskLanguageModel: true))
				{
					try
					{
						DeviceObjectHelper_TaskConfigChanged(sender, new CompileEventArgs(val3.ObjectGuid));
					}
					catch
					{
					}
				}
			}
		}

		internal static bool AutoSetActivePathForChildPlc(IDeviceObject childToModify, Guid gatewayGuid, IDeviceAddress parentAddress)
		{
			if (childToModify == null || ((IObject)childToModify).MetaObject.ParentObjectGuid == Guid.Empty || !((IObject)childToModify).MetaObject.IsToModify || !childToModify.AllowsDirectCommunication)
			{
				return false;
			}
			try
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)childToModify).MetaObject.ProjectHandle, ((IObject)childToModify).MetaObject.ParentObjectGuid);
				while (metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
				}
				IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid).Object;
				IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				if (val != null)
				{
					ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentification);
					if (targetSettingsById != null)
					{
						string stringValue = LocalTargetSettings.SetActivePathForChild.GetStringValue(targetSettingsById);
						if (parentAddress == null)
						{
							parentAddress = val.CommunicationSettings.Address;
						}
						if (stringValue != string.Empty && parentAddress != null)
						{
							object obj = ((ICloneable)parentAddress).Clone();
							IDeviceAddress val2 = (IDeviceAddress)((obj is IDeviceAddress) ? obj : null);
							val2.SetAddress(parentAddress.ToString() + "." + stringValue);
							if (childToModify.CommunicationSettings.Address != null && childToModify.CommunicationSettings.Gateway != Guid.Empty && val2.ToString() == childToModify.CommunicationSettings.Address.ToString() && childToModify.CommunicationSettings.Gateway == val.CommunicationSettings.Gateway)
							{
								return true;
							}
							if (gatewayGuid == Guid.Empty)
							{
								childToModify.CommunicationSettings.Gateway=(val.CommunicationSettings.Gateway);
							}
							else
							{
								childToModify.CommunicationSettings.Gateway=(gatewayGuid);
							}
							childToModify.CommunicationSettings.Address=(val2);
							return true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to set active path for child device: " + ex.Message);
			}
			return false;
		}

		internal static void AutoInsertLogicalDevice(ObjectEventArgs e)
		{
			AutoInsertLogicalDevice(e, bAutoInsert: true);
		}

		internal static void AutoInsertLogicalDevice(ObjectEventArgs e, bool bAutoInsert)
		{
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Expected O, but got Unknown
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Expected O, but got Unknown
			if (!GenerateCodeForLogicalDevices)
			{
				return;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ObjectGuid);
			if (!(objectToRead.Object is ILogicalDevice) || objectToRead.Object is LogicalIODevice)
			{
				return;
			}
			IObject @object = objectToRead.Object;
			ILogicalDevice val = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
			if (!val.IsPhysical || val.MappedDevices == null || ((ICollection)val.MappedDevices).Count <= 0)
			{
				return;
			}
			foreach (LogicalMappedDevice item in (IEnumerable)val.MappedDevices)
			{
				if (!item.IsMapped)
				{
					continue;
				}
				Guid getMappedDevice = item.GetMappedDevice;
				bool flag = false;
				foreach (Guid deviceGuid in PhysicalDevices.DeviceGuids)
				{
					if (deviceGuid != objectToRead.ObjectGuid && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.ProjectHandle, deviceGuid))
					{
						IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, deviceGuid).Object;
						ILogicalDevice val2 = (ILogicalDevice)(object)((object2 is ILogicalDevice) ? object2 : null);
						if (val2 != null)
						{
							foreach (LogicalMappedDevice item2 in (IEnumerable)val2.MappedDevices)
							{
								if (item2.GetMappedDevice == getMappedDevice)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			bool flag2 = true;
			int num = 0;
			foreach (LogicalMappedDevice item3 in (IEnumerable)val.MappedDevices)
			{
				if (item3.InsertAutoIndex >= 0)
				{
					string text = objectToRead.Name;
					if (num > 0)
					{
						text = text + "_L" + num;
					}
					flag2 &= MapLogicalDevice(objectToRead, text, num);
					num++;
				}
			}
			if (flag2)
			{
				return;
			}
			IMetaObject val3 = null;
			try
			{
				val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(objectToRead);
				IMetaObjectStub hostStub = GetHostStub(e.ProjectHandle, val3.ObjectGuid);
				num = 0;
				IObject object3 = val3.Object;
				Guid guid3 = default(Guid);
				byte[] array = default(byte[]);
				byte[] array2 = default(byte[]);
				Guid guid6 = default(Guid);
				Guid guid7 = default(Guid);
				IObject val9 = default(IObject);
				string text3 = default(string);
				IObjectProperty[] array3 = default(IObjectProperty[]);
				foreach (LogicalMappedDevice item4 in (IEnumerable)((ILogicalDevice)((object3 is ILogicalDevice) ? object3 : null)).MappedDevices)
				{
					bool flag3 = true;
					Guid guid = Guid.Empty;
					IDeviceIdentification val4 = null;
					if (item4.InsertAutoIndex >= 0)
					{
						val4 = item4.MatchingLogicalDevices[item4.InsertAutoIndex].DeviceIdentification;
						Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(e.ProjectHandle);
						foreach (Guid guid2 in allObjects)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, guid2);
							if (!typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								continue;
							}
							IMetaObjectStub hostStub2 = GetHostStub(e.ProjectHandle, guid2);
							if (hostStub2 != null && hostStub2.ObjectGuid == hostStub.ObjectGuid && !CheckLogicalIoDisableSynchronization(e.ProjectHandle, guid2))
							{
								if (guid == Guid.Empty)
								{
									guid = guid2;
								}
								else
								{
									flag3 = false;
								}
							}
						}
					}
					if (flag3 && guid != Guid.Empty && (item4.IsMapped || bAutoInsert))
					{
						try
						{
							IDeviceCollection allDevices = ((IDeviceRepository)APEnvironment.DeviceRepository).GetAllDevices();
							IDeviceDescription val5 = null;
							foreach (IDeviceDescription item5 in (IEnumerable)allDevices)
							{
								IDeviceDescription val6 = item5;
								if (((object)val6.DeviceIdentification).Equals((object)val4))
								{
									val5 = val6;
									break;
								}
							}
							if (val5 == null)
							{
								foreach (IDeviceDescription item6 in (IEnumerable)allDevices)
								{
									IDeviceDescription val7 = item6;
									if (LogicalMappedDevice.CheckMatching(val7.DeviceIdentification, val4))
									{
										val5 = val7;
										break;
									}
								}
							}
							if (val5 != null)
							{
								string text2 = val3.Name;
								if (num > 0)
								{
									text2 = text2 + "_L" + num;
								}
								text2 = ((IObjectManager)APEnvironment.ObjectMgr).GetUniqueName(e.ProjectHandle, guid, DeviceObject.GUID_DEVICENAMESPACE, text2, ref guid3);
								Guid guid4 = Guid.Empty;
								if (item4.IsMapped && e is ObjectAddedEventArgs)
								{
									IPastedObject pastedObject = ((ObjectAddedEventArgs)((e is ObjectAddedEventArgs) ? e : null)).PastedObject;
									IPastedObject2 val8 = (IPastedObject2)(object)((pastedObject is IPastedObject2) ? pastedObject : null);
									if (val8 != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.ProjectHandle, val8.SrcParentSVNodeGuid))
									{
										IMetaObjectStub hostStub3 = GetHostStub(e.ProjectHandle, val8.SrcParentSVNodeGuid);
										Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(e.ProjectHandle, item4.MappedDevice);
										foreach (Guid guid5 in allObjects)
										{
											IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, guid5);
											if (typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub2.ObjectType))
											{
												IMetaObjectStub hostStub4 = GetHostStub(e.ProjectHandle, guid5);
												IObject object4 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, guid5).Object;
												if (((ILogicalDevice)((object4 is ILogicalDevice) ? object4 : null)).IsLogical && hostStub4.ObjectGuid == hostStub3.ObjectGuid)
												{
													guid4 = guid5;
												}
											}
										}
									}
									if (guid4 == Guid.Empty)
									{
										guid4 = item4.GetMappedDevice;
									}
								}
								if (guid4 != Guid.Empty)
								{
									if (bAutoInsert)
									{
										((IObjectManager)APEnvironment.ObjectMgr).ExportObject(e.ProjectHandle, guid4, ref array, ref array2);
										((IObjectManager)APEnvironment.ObjectMgr).ImportObject(array, array2, ref guid6, ref guid7, ref val9, ref text3, ref array3);
										((IObjectManager)APEnvironment.ObjectMgr).AddObject(e.ProjectHandle, guid, Guid.NewGuid(), val9, text2, -1);
										item4.MappedDevice = string.Empty;
										MapLogicalDevice(val3, text2, num);
									}
								}
								else
								{
									LogicalIODevice logicalIODevice = new LogicalIODevice(new DeviceIdentification(val5.DeviceIdentification.Type, val5.DeviceIdentification.Id, val5.DeviceIdentification.Version, string.Empty));
									((IObjectManager)APEnvironment.ObjectMgr).AddObject(e.ProjectHandle, guid, Guid.NewGuid(), (IObject)(object)logicalIODevice, text2, -1);
									MapLogicalDevice(val3, text2, num);
								}
							}
						}
						catch
						{
						}
					}
					else
					{
						item4.MappedDevice = string.Empty;
					}
					num++;
				}
			}
			catch
			{
			}
			finally
			{
				if (val3 != null && val3.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
				}
			}
		}

		private void ObjectMgr_ObjectMoving(object sender, ObjectMovingEventArgs e)
		{
			if (DeviceManager.DoNotSkipEvents || (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && e.ProjectHandle == ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle))
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
				if (typeof(IDeviceObjectBase).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					((IDeviceObjectBase)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ObjectGuid).Object).PreparePaste(bOnlyChildConnectors: true);
				}
			}
		}

		private void ObjectMgr_ObjectMoved(object sender, ObjectMovedEventArgs e)
		{
			AddListLanguageModel(e.ProjectHandle, e.ObjectGuid, bIsTaskLanguageModel: false);
		}

		private void ObjectMgr_ObjectModifying(object sender, ObjectEventArgs e)
		{
			if (!_bProjectRecursion)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
				if (typeof(DeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(ExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					_lastObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ObjectGuid).Object;
				}
			}
		}

		private void ObjectMgr_ObjectModified(object sender, ObjectModifiedEventArgs e)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected O, but got Unknown
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Expected O, but got Unknown
			if (e.Editor is IProjectDiffTreeRoot)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
				if ((metaObjectStub != null && metaObjectStub.SubObjectGuids.Length == 0 && typeof(DeviceObjectBase).IsAssignableFrom(metaObjectStub.ObjectType)) || typeof(ExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject val = null;
					try
					{
						val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(e.ProjectHandle, e.ObjectGuid);
						if (val.Object is DeviceObjectBase)
						{
							foreach (IConnector item in (IEnumerable)(val.Object as DeviceObjectBase).Connectors)
							{
								IConnector val2 = item;
								if ((int)val2.ConnectorRole != 0)
								{
									continue;
								}
								foreach (IAdapter item2 in (IEnumerable)val2.Adapters)
								{
									IAdapter val3 = item2;
									if (val3 is IAdapterBase)
									{
										(val3 as IAdapterBase).ClearModules();
									}
								}
							}
						}
						if (val.Object is ExplicitConnector)
						{
							ExplicitConnector explicitConnector = val.Object as ExplicitConnector;
							if ((int)explicitConnector.ConnectorRole == 0)
							{
								foreach (IAdapter item3 in (IEnumerable)explicitConnector.Adapters)
								{
									IAdapter val4 = item3;
									if (val4 is IAdapterBase)
									{
										(val4 as IAdapterBase).ClearModules();
									}
								}
							}
						}
					}
					catch
					{
					}
					finally
					{
						if (val != null && val.IsToModify)
						{
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
						}
					}
				}
			}
			AddListLanguageModel(e.ProjectHandle, e.ObjectGuid, bIsTaskLanguageModel: false);
		}

		internal static void DeleteUnusedTasks(PSChangeAction action)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Invalid comparison between Unknown and I4
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Expected O, but got Unknown
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			if (_bInDeleteUnusedTasks)
			{
				return;
			}
			try
			{
				_bInDeleteUnusedTasks = true;
				if (RemovedTasks.Count <= 0)
				{
					return;
				}
				Enumerator<RequiredTask, RemoveTaskData> enumerator = _dictRemovedTasks.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<RequiredTask, RemoveTaskData> current = enumerator.Current;
						Guid appGuid = current.Value.appGuid;
						int num = -1;
						if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
						{
							break;
						}
						num = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
						if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(num, appGuid))
						{
							continue;
						}
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(num, appGuid);
						if (objectToRead == null)
						{
							continue;
						}
						IMetaObject taskConfig = GetTaskConfig(objectToRead);
						if (taskConfig == null || ((int)action == 2 && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(num, current.Value.ObjectGuid) && GetHostStub(num, current.Value.ObjectGuid).ObjectGuid == current.Value.HostGuid))
						{
							continue;
						}
						RequiredTask key = current.Key;
						string text = key.TaskName;
						if (text.Contains("$(DeviceName)"))
						{
							text = text.Replace("$(DeviceName)", current.Value.stMetaObjectName);
						}
						IMetaObject val = GetTask(taskConfig, text);
						if (val == null)
						{
							continue;
						}
						IObject @object = val.Object;
						ITaskObject val2 = (ITaskObject)(object)((@object is ITaskObject) ? @object : null);
						LList<string> val3 = new LList<string>();
						for (int i = 0; i < key.TaskPou.Count; i++)
						{
							string text2 = (string)key.TaskPou[i];
							if (text2.Contains("$(DeviceName)"))
							{
								text2 = text2.Replace("$(DeviceName)", current.Value.stMetaObjectName);
							}
							if (val2.POUs == null)
							{
								continue;
							}
							foreach (IPouObject item in (IEnumerable)val2.POUs)
							{
								if (string.Compare(item.Name, text2, StringComparison.InvariantCultureIgnoreCase) == 0)
								{
									val3.Add(text2);
								}
							}
						}
						if (((val2 != null) ? val2.POUs : null) == null)
						{
							continue;
						}
						if (((ICollection)val2.POUs).Count > val3.Count)
						{
							try
							{
								val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val);
								if (val == null)
								{
									continue;
								}
								IObject object2 = val.Object;
								val2 = (ITaskObject)(object)((object2 is ITaskObject) ? object2 : null);
								foreach (string item2 in val3)
								{
									foreach (IPouObject item3 in (IEnumerable)val2.POUs)
									{
										IPouObject val4 = item3;
										if (string.Compare(val4.Name, item2, StringComparison.InvariantCultureIgnoreCase) == 0)
										{
											val2.POUs.Remove(val4);
											break;
										}
									}
								}
							}
							catch
							{
							}
							finally
							{
								if (val != null && val.IsToModify)
								{
									((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
								}
							}
							continue;
						}
						bool flag = false;
						if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(num, current.Value.HostGuid))
						{
							IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(num, current.Value.HostGuid);
							IObject obj2 = ((objectToRead2 != null) ? objectToRead2.Object : null);
							IIoProvider val5 = (IIoProvider)(object)((obj2 is IIoProvider) ? obj2 : null);
							if (val5 != null)
							{
								LDictionary<RequiredTask, string> val6 = new LDictionary<RequiredTask, string>();
								CollectRequiredTasks(val5, val6);
								Enumerator<RequiredTask, string> enumerator4 = val6.GetEnumerator();
								try
								{
									while (enumerator4.MoveNext())
									{
										KeyValuePair<RequiredTask, string> current3 = enumerator4.Current;
										string text3 = current3.Key.TaskName;
										if (text3.Contains("$(DeviceName)"))
										{
											text3 = text3.Replace("$(DeviceName)", current3.Value);
										}
										if (string.Compare(text, text3, StringComparison.InvariantCultureIgnoreCase) == 0)
										{
											flag = true;
											break;
										}
									}
								}
								finally
								{
									((IDisposable)enumerator4).Dispose();
								}
							}
						}
						if (!flag)
						{
							try
							{
								((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(val.ProjectHandle, val.ObjectGuid);
							}
							catch
							{
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				RemovedTasks.Clear();
			}
			finally
			{
				_bInDeleteUnusedTasks = false;
			}
		}

		internal static void CollectRequiredTasks(IIoProvider ioprovider, LDictionary<RequiredTask, string> dictRequiredTasks)
		{
			if (ioprovider == null)
			{
				return;
			}
			if (((DriverInfo)(object)((ioprovider != null) ? ioprovider.DriverInfo : null))?.RequiredTasks != null)
			{
				string name = ioprovider.GetMetaObject().Name;
				RequiredTask[] requiredTasks = ((DriverInfo)(object)ioprovider.DriverInfo).RequiredTasks;
				foreach (RequiredTask requiredTask in requiredTasks)
				{
					dictRequiredTasks[requiredTask]= name;
				}
			}
			IIoProvider[] children = ioprovider.Children;
			for (int i = 0; i < children.Length; i++)
			{
				CollectRequiredTasks(children[i], dictRequiredTasks);
			}
		}

		internal static void CollectTasksToRemove(IIoProvider ioprovider, IMetaObject mo)
		{
			if (((DriverInfo)(object)((ioprovider != null) ? ioprovider.DriverInfo : null))?.RequiredTasks == null)
			{
				return;
			}
			IMetaObjectStub hostStub = GetHostStub(mo.ProjectHandle, mo.ParentObjectGuid);
			if (hostStub != null)
			{
				Guid ioApplication = GetIoApplication(hostStub.ProjectHandle, hostStub.ObjectGuid);
				RemoveTaskData removeTaskData = new RemoveTaskData();
				removeTaskData.stMetaObjectName = mo.Name;
				removeTaskData.appGuid = ioApplication;
				removeTaskData.HostGuid = hostStub.ObjectGuid;
				removeTaskData.ObjectGuid = mo.ObjectGuid;
				RequiredTask[] requiredTasks = ((DriverInfo)(object)ioprovider.DriverInfo).RequiredTasks;
				foreach (RequiredTask requiredTask in requiredTasks)
				{
					_dictRemovedTasks.Add(requiredTask, removeTaskData);
				}
			}
		}

		private void OnObjectRemoved(object sender, ObjectRemovedEventArgs e)
		{
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Expected O, but got Unknown
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Expected O, but got Unknown
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Expected O, but got Unknown
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Expected O, but got Unknown
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Expected O, but got Unknown
			bool flag = false;
			if (e is ObjectRemovedEventArgs2 && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((ObjectRemovedEventArgs2)((e is ObjectRemovedEventArgs2) ? e : null)).RootProjectHandle, ((ObjectRemovedEventArgs2)((e is ObjectRemovedEventArgs2) ? e : null)).RootObjectGuid))
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((ObjectRemovedEventArgs2)((e is ObjectRemovedEventArgs2) ? e : null)).RootProjectHandle, ((ObjectRemovedEventArgs2)((e is ObjectRemovedEventArgs2) ? e : null)).RootObjectGuid);
				if (metaObjectStub != null && metaObjectStub.ParentObjectGuid == Guid.Empty)
				{
					flag = true;
				}
			}
			LList<Guid> val = default(LList<Guid>);
			if (_dictLogicalNames.TryGetValue(e.MetaObject.Name, ref val))
			{
				val.Remove(e.MetaObject.ObjectGuid);
				if (val.Count == 0)
				{
					_dictLogicalNames.Remove(e.MetaObject.Name);
				}
			}
			if (_dictHosts.ContainsKey(e.MetaObject.ObjectGuid))
			{
				_dictHosts.Remove(e.MetaObject.ObjectGuid);
			}
			if (e.MetaObject.Object is ILogicalDevice && _dictMappedDevices.TryGetValue(e.MetaObject.Name, ref val))
			{
				val.Remove(e.MetaObject.ObjectGuid);
			}
			if (e.MetaObject.Object is IApplicationObject)
			{
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject != null && primaryProject.Handle == e.MetaObject.ProjectHandle)
				{
					Guid deviceGuid = ((IOnlineApplicationObject)(IApplicationObject)e.MetaObject.Object).DeviceGuid;
					if (deviceGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.MetaObject.ProjectHandle, deviceGuid).Object is IDeviceObjectBase)
					{
						((IEngine2)APEnvironment.Engine).UpdateLanguageModel(e.MetaObject.ProjectHandle, deviceGuid);
					}
				}
			}
			if (!flag)
			{
				if (e.MetaObject.Object is ExplicitConnector)
				{
					ExplicitConnector explicitConnector = e.MetaObject.Object as ExplicitConnector;
					CollectTasksToRemove((IIoProvider)(object)explicitConnector, e.MetaObject);
					object obj;
					if (explicitConnector == null)
					{
						obj = null;
					}
					else
					{
						IDriverInfo driverInfo = explicitConnector.DriverInfo;
						obj = ((driverInfo != null) ? driverInfo.RequiredLibs : null);
					}
					if (obj != null)
					{
						foreach (IRequiredLib item in (IEnumerable)explicitConnector.DriverInfo.RequiredLibs)
						{
							IRequiredLib val2 = item;
							_dictRemovedLibs.Add(val2, (IIoProvider)(object)explicitConnector);
						}
					}
				}
				if (e.MetaObject.Object is DeviceObject || e.MetaObject.Object is SlotDeviceObject)
				{
					DeviceObject deviceObject = e.MetaObject.Object as DeviceObject;
					if (e.MetaObject.Object is SlotDeviceObject)
					{
						deviceObject = (e.MetaObject.Object as SlotDeviceObject).GetDevice();
					}
					if (deviceObject != null)
					{
						CollectTasksToRemove((IIoProvider)(object)deviceObject, e.MetaObject);
						object obj2;
						if (deviceObject == null)
						{
							obj2 = null;
						}
						else
						{
							IDriverInfo driverInfo2 = deviceObject.DriverInfo;
							obj2 = ((driverInfo2 != null) ? driverInfo2.RequiredLibs : null);
						}
						if (obj2 != null)
						{
							foreach (IRequiredLib item2 in (IEnumerable)deviceObject.DriverInfo.RequiredLibs)
							{
								IRequiredLib val3 = item2;
								_dictRemovedLibs.Add(val3, (IIoProvider)(object)deviceObject);
							}
						}
						foreach (Connector item3 in (IEnumerable)deviceObject.Connectors)
						{
							CollectTasksToRemove((IIoProvider)(object)item3, e.MetaObject);
							object obj3;
							if (item3 == null)
							{
								obj3 = null;
							}
							else
							{
								IDriverInfo driverInfo3 = item3.DriverInfo;
								obj3 = ((driverInfo3 != null) ? driverInfo3.RequiredLibs : null);
							}
							if (obj3 == null)
							{
								continue;
							}
							foreach (IRequiredLib item4 in (IEnumerable)item3.DriverInfo.RequiredLibs)
							{
								IRequiredLib val4 = item4;
								_dictRemovedLibs.Add(val4, (IIoProvider)(object)item3);
							}
						}
					}
				}
			}
			if (e.MetaObject.Object is IDeviceObject)
			{
				AddListLanguageModel(e.MetaObject.ProjectHandle, e.MetaObject.ParentObjectGuid, bIsTaskLanguageModel: false);
				if (!AddListLanguageModel(e.MetaObject.ProjectHandle, e.MetaObject.ParentObjectGuid, bIsTaskLanguageModel: true))
				{
					try
					{
						DeviceObjectHelper_TaskConfigChanged(sender, new CompileEventArgs(e.MetaObject.ParentObjectGuid));
					}
					catch
					{
					}
				}
			}
			if (e.MetaObject.Object is DeviceObject || e.MetaObject.Object is SlotDeviceObject)
			{
				DeviceObject deviceObject2 = e.MetaObject.Object as DeviceObject;
				if (e.MetaObject.Object is SlotDeviceObject)
				{
					deviceObject2 = (e.MetaObject.Object as SlotDeviceObject).GetDevice();
				}
				if (deviceObject2 != null)
				{
					DeviceObject deviceObject3 = deviceObject2.GetHostDeviceObject() as DeviceObject;
					if (deviceObject3 != null && deviceObject3.GlobalDataTypes.Count > 0)
					{
						Enumerator<string, LDictionary<IDataElement, Guid>> enumerator3 = deviceObject3.GlobalDataTypes.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<string, LDictionary<IDataElement, Guid>> current = enumerator3.Current;
								Enumerator<IDataElement, Guid> enumerator4 = current.Value.GetEnumerator();
								try
								{
									while (enumerator4.MoveNext())
									{
										KeyValuePair<IDataElement, Guid> current2 = enumerator4.Current;
										if (current2.Value == e.MetaObject.ObjectGuid)
										{
											ClearGlobalDataTypes(e.MetaObject.ProjectHandle, current2.Key);
											current.Value.Remove(current2.Key);
											break;
										}
									}
								}
								finally
								{
									((IDisposable)enumerator4).Dispose();
								}
							}
						}
						finally
						{
							((IDisposable)enumerator3).Dispose();
						}
					}
				}
			}
			if (!_physicalDeviceBuffer.DeviceGuids.Contains(e.MetaObject.ObjectGuid))
			{
				return;
			}
			IMetaObjectStub hostStub = GetHostStub(e.MetaObject.ProjectHandle, e.MetaObject.ParentObjectGuid);
			IObject @object = e.MetaObject.Object;
			ILogicalDevice val5 = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
			if (val5 == null)
			{
				return;
			}
			LList<Guid> val7 = default(LList<Guid>);
			foreach (IMappedDevice item5 in (IEnumerable)val5.MappedDevices)
			{
				IMappedDevice val6 = item5;
				if (!LogicalNames.TryGetValue(val6.MappedDevice, ref val7))
				{
					continue;
				}
				foreach (Guid item6 in val7)
				{
					Guid empty = Guid.Empty;
					if (HostsForLogicalDevices.TryGetValue(item6, ref empty) && empty == hostStub.ObjectGuid && !CheckLogicalIoDisableSynchronization(e.MetaObject.ProjectHandle, item6))
					{
						_liLogicalDevicesToRemove.Add(item6);
					}
				}
			}
		}

		internal static bool CheckLogicalIoDisableSynchronization(int nProjectHandle, Guid logicalDeviceGuid)
		{
			if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, logicalDeviceGuid))
			{
				Guid guid = logicalDeviceGuid;
				do
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
					if (typeof(ILogicalObject2).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						break;
					}
					guid = metaObjectStub.ParentObjectGuid;
				}
				while (guid != Guid.Empty);
				if (guid != Guid.Empty)
				{
					IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid).Object;
					ILogicalObject2 val = (ILogicalObject2)(object)((@object is ILogicalObject2) ? @object : null);
					if (val != null && val.DisableSynchronization)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static void ClearGlobalDataTypes(int nProjectHandle, IDataElement dataElement)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
			{
				IDataElement dataElement2 = item;
				ClearGlobalDataTypes(nProjectHandle, dataElement2);
			}
			if (dataElement is DataElementStructType)
			{
				try
				{
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(nProjectHandle, (dataElement as DataElementStructType).LmStructType);
				}
				catch
				{
				}
			}
		}

		private bool CheckUniquePOUIdentifier(int nProjecthandle, Guid objectGuid, string stName)
		{
			IMetaObjectStub hostStub = GetHostStub(nProjecthandle, objectGuid);
			if (hostStub == null)
			{
				return true;
			}
			Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(nProjecthandle, stName);
			foreach (Guid guid in allObjects)
			{
				if (!(guid != objectGuid))
				{
					continue;
				}
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjecthandle, guid);
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObjectStub hostStub2 = GetHostStub(nProjecthandle, guid);
					if (hostStub2 != null && hostStub2.ObjectGuid == hostStub.ObjectGuid)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void OnObjectRenamed(object sender, ObjectRenamedEventArgs e)
		{
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Expected O, but got Unknown
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Expected O, but got Unknown
			//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0f: Expected O, but got Unknown
			if (bRenameInRecursion)
			{
				return;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
			if (metaObjectStub != null && !typeof(IDeviceObjectBase).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(ConnectorBase).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return;
			}
			IMetaObject val = null;
			if (s_undoMgr == null || (s_undoMgr != null && !((IUndoManager)s_undoMgr).InRedo && !((IUndoManager)s_undoMgr).InUndo))
			{
				IMetaObjectStub host = GetHostStub(e.ProjectHandle, e.ObjectGuid);
				Guid[] subObjectGuids = metaObjectStub.SubObjectGuids;
				foreach (Guid guid in subObjectGuids)
				{
					IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, guid);
					if (metaObjectStub2 != null && typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub2.ObjectType))
					{
						host = null;
					}
				}
				if (!CheckUniqueIdentifier(e.ProjectHandle, e.NewName, e.ObjectGuid, host, bCheckAll: false, bCheckLogical: false, out var existingObjectGuid))
				{
					throw new ObjectNameNotUniqueException2(e.ProjectHandle, e.ObjectGuid, existingObjectGuid, e.NewName);
				}
			}
			AddListLanguageModel(e.ProjectHandle, e.ObjectGuid, bIsTaskLanguageModel: false);
			if (typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub.ObjectType) && LogicalDevices != null)
			{
				bool flag = false;
				try
				{
					APEnvironment.DeviceMgr.RaiseLogicalDeviceRenaming(e.ProjectHandle, e.ObjectGuid);
				}
				catch
				{
					flag = true;
				}
				if (!flag && !_bRefactoring)
				{
					Guid[] array = new Guid[LogicalDevices.DeviceGuids.Count];
					LogicalDevices.DeviceGuids.CopyTo(array, 0);
					int num = 0;
					Guid[] subObjectGuids = array;
					foreach (Guid guid2 in subObjectGuids)
					{
						if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.ProjectHandle, guid2) || CheckLogicalIoDisableSynchronization(e.ProjectHandle, guid2) || GetHostStub(e.ProjectHandle, guid2) != GetHostStub(e.ProjectHandle, e.ObjectGuid))
						{
							continue;
						}
						string text = e.NewName;
						if (num > 0)
						{
							text = text + "_L" + num;
						}
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, guid2);
						string name = objectToRead.Name;
						if (!(name != text))
						{
							continue;
						}
						IObject @object = objectToRead.Object;
						foreach (IMappedDevice item in (IEnumerable)((ILogicalDevice)((@object is ILogicalDevice) ? @object : null)).MappedDevices)
						{
							if (!(item.GetMappedDevice == e.ObjectGuid))
							{
								continue;
							}
							try
							{
								bRenameInRecursion = true;
								((IObjectManager)APEnvironment.ObjectMgr).RenameObject(e.ProjectHandle, guid2, text);
							}
							finally
							{
								bRenameInRecursion = false;
							}
							IMetaObject val2 = null;
							try
							{
								val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(e.ProjectHandle, e.ObjectGuid);
								IObject object2 = val2.Object;
								foreach (IMappedDevice item2 in (IEnumerable)((ILogicalDevice)((object2 is ILogicalDevice) ? object2 : null)).MappedDevices)
								{
									IMappedDevice val3 = item2;
									if (val3.MappedDevice == name)
									{
										val3.MappedDevice=(text);
									}
								}
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
							num++;
						}
					}
				}
			}
			if (typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType) && PhysicalDevices != null)
			{
				Guid[] array2 = new Guid[PhysicalDevices.DeviceGuids.Count];
				PhysicalDevices.DeviceGuids.CopyTo(array2, 0);
				Guid[] subObjectGuids = array2;
				foreach (Guid guid3 in subObjectGuids)
				{
					if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.ProjectHandle, guid3) || GetHostStub(e.ProjectHandle, guid3) != GetHostStub(e.ProjectHandle, e.ObjectGuid))
					{
						continue;
					}
					IMetaObject val4 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, guid3);
					if (val4 == null || !(val4.Object is ILogicalDevice))
					{
						continue;
					}
					IObject object3 = val4.Object;
					foreach (IMappedDevice item3 in (IEnumerable)((ILogicalDevice)((object3 is ILogicalDevice) ? object3 : null)).MappedDevices)
					{
						IMappedDevice val5 = item3;
						if (!(val5.MappedDevice == e.OldName))
						{
							continue;
						}
						try
						{
							bool flag2 = false;
							try
							{
								APEnvironment.DeviceMgr.RaiseLogicalDeviceRenaming(e.ProjectHandle, guid3);
							}
							catch
							{
								flag2 = true;
							}
							if (!flag2 && !CheckLogicalIoDisableSynchronization(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid))
							{
								bRenameInRecursion = true;
								((IObjectManager)APEnvironment.ObjectMgr).RenameObject(e.ProjectHandle, guid3, e.NewName);
								if (val4.Object is IDeviceObjectBase || val4.Object is ConnectorBase)
								{
									try
									{
										val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val4);
										val4 = null;
										if (val.Object is IDeviceObjectBase)
										{
											((IDeviceObjectBase)val.Object).OnRenamed(e.OldName);
										}
										else
										{
											((ConnectorBase)(object)val.Object).OnDeviceRenamed(e.OldName);
										}
									}
									finally
									{
										if (val != null && val.IsToModify)
										{
											((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
										}
									}
								}
							}
						}
						catch
						{
							throw;
						}
						finally
						{
							bRenameInRecursion = false;
						}
						try
						{
							val4 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(e.ProjectHandle, guid3);
							IObject object4 = val4.Object;
							((ILogicalDevice)((object4 is ILogicalDevice) ? object4 : null)).MappedDevices.get_Item(val5.Index).MappedDevice=(e.NewName);
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
				}
			}
			try
			{
				if (typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IObject object5 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ObjectGuid).Object;
					foreach (LogicalMappedDevice item4 in (IEnumerable)((ILogicalDevice)((object5 is ILogicalDevice) ? object5 : null)).MappedDevices)
					{
						item4.UpdateNavigator();
					}
				}
				if (!typeof(IDeviceObjectBase).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(ConnectorBase).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return;
				}
				IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.ObjectGuid);
				if (!(objectToRead2.Object is IDeviceObjectBase) && !(objectToRead2.Object is ConnectorBase))
				{
					return;
				}
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(objectToRead2);
				objectToRead2 = null;
				if (val.Object is IDeviceObjectBase)
				{
					((IDeviceObjectBase)val.Object).OnRenamed(e.OldName);
				}
				else
				{
					((ConnectorBase)(object)val.Object).OnDeviceRenamed(e.OldName);
				}
				((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
				if (val.Object is DeviceObject || val.Object is SlotDeviceObject)
				{
					bool flag3 = false;
					IMetaObject val6 = null;
					DeviceObject deviceObject = null;
					if (val.Object is DeviceObject)
					{
						deviceObject = val.Object as DeviceObject;
					}
					if (val.Object is SlotDeviceObject)
					{
						deviceObject = (val.Object as SlotDeviceObject).GetDevice();
					}
					if (deviceObject != null)
					{
						if (deviceObject.DriverInfo != null && deviceObject.DriverInfo.RequiredLibs != null)
						{
							foreach (RequiredLib item5 in (IEnumerable)deviceObject.DriverInfo.RequiredLibs)
							{
								if (item5.IsDiagnosisLib || item5.FbInstances == null)
								{
									continue;
								}
								foreach (FBInstance item6 in (IEnumerable)item5.FbInstances)
								{
									if (((ICollection)item6.CyclicCalls).Count > 0)
									{
										flag3 = true;
										break;
									}
								}
								if (flag3)
								{
									break;
								}
							}
						}
						if (flag3)
						{
							val6 = deviceObject.GetApplication();
						}
						if (val6 == null)
						{
							foreach (Connector item7 in (IEnumerable)deviceObject.Connectors)
							{
								if (item7.DriverInfo != null && item7.DriverInfo.RequiredLibs != null)
								{
									foreach (RequiredLib item8 in (IEnumerable)item7.DriverInfo.RequiredLibs)
									{
										if (item8.IsDiagnosisLib || item8.FbInstances == null)
										{
											continue;
										}
										foreach (FBInstance item9 in (IEnumerable)item8.FbInstances)
										{
											if (((ICollection)item9.CyclicCalls).Count > 0)
											{
												flag3 = true;
												break;
											}
										}
										if (flag3)
										{
											break;
										}
									}
								}
								if (flag3)
								{
									val6 = item7.GetApplication();
									if (val6 != null)
									{
										break;
									}
								}
							}
						}
						if (flag3 && val6 != null && !AddListLanguageModel(val6.ProjectHandle, val6.ObjectGuid, bIsTaskLanguageModel: true))
						{
							DeviceObjectHelper_TaskConfigChanged(sender, new CompileEventArgs(val6.ObjectGuid));
						}
					}
				}
				val = null;
			}
			catch
			{
				if (val != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
				}
			}
		}

		internal static bool CreateBitChannels(int nProjectHandle, Guid objectGuid)
		{
			if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, objectGuid))
			{
				for (IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid); metaObjectStub != null; metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid))
				{
					if (typeof(DeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						DeviceObject deviceObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, metaObjectStub.ObjectGuid).Object as DeviceObject;
						if (deviceObject != null && deviceObject.CreateBitChannels)
						{
							return true;
						}
					}
					if (!(metaObjectStub.ParentObjectGuid != Guid.Empty))
					{
						break;
					}
				}
			}
			return false;
		}

		public static void DeviceObjectHelper_BeforeClearAll(object sender, EventArgs e)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return;
			}
			int handle = primaryProject.Handle;
			Guid guid = new Guid("{D9B2B2CC-EA99-4c3b-AA42-1E5C49E65B84}");
			IStructuredView structuredView = ((IObjectManager)APEnvironment.ObjectMgr).GetStructuredView(handle, guid);
			ResetPackMode();
			ResetStrategy();
			ISVNode[] children = structuredView.Children;
			foreach (ISVNode val in children)
			{
				if (!typeof(DeviceObject).IsAssignableFrom(val.ObjectType))
				{
					continue;
				}
				IMetaObject objectToRead = val.GetObjectToRead();
				if (objectToRead.Object is DeviceObject)
				{
					UpdateAddresses((IIoProvider)(object)(objectToRead.Object as DeviceObject));
					if ((objectToRead.Object as DeviceObject).GlobalDataTypes.Count > 0)
					{
						(objectToRead.Object as DeviceObject).GlobalDataTypes.Clear();
					}
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)0) && !_liPLCRenew.Contains(objectToRead.ObjectGuid))
					{
						_liPLCRenew.Add(objectToRead.ObjectGuid);
					}
					ReloadEditors(handle, new Guid[1] { objectToRead.ObjectGuid });
				}
			}
		}

		internal static void ReloadEditors(int nProjectHandle, Guid[] objectGuids)
		{
			foreach (Guid guid in objectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(ISlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(nProjectHandle, guid);
					if (editors != null && editors.Length != 0)
					{
						IEditor[] array = editors;
						for (int j = 0; j < array.Length; j++)
						{
							array[j].Reload();
						}
					}
				}
				ReloadEditors(nProjectHandle, metaObjectStub.SubObjectGuids);
			}
		}

		public static void DeviceObjectHelper_TaskConfigChanged(object sender, CompileEventArgs e)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return;
			}
			int handle = primaryProject.Handle;
			int num = default(int);
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, e.ApplicationGuid) || !((IObjectManager)APEnvironment.ObjectMgr).IsLoadProjectFinished(handle, ref num))
			{
				return;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, e.ApplicationGuid);
			Debug.Assert(metaObjectStub != null);
			while (metaObjectStub.ParentObjectGuid != Guid.Empty)
			{
				do
				{
					if (metaObjectStub.ParentObjectGuid == Guid.Empty)
					{
						return;
					}
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, metaObjectStub.ParentObjectGuid);
				}
				while (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType));
			}
			if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
			if (objectToRead.Object is DeviceObject)
			{
				DeviceObject deviceObject = (DeviceObject)(object)objectToRead.Object;
				try
				{
					deviceObject.SetTaskLanguage(bEnable: true);
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)deviceObject, true);
				}
				finally
				{
					deviceObject.SetTaskLanguage(bEnable: false);
				}
			}
		}

		private void OnLanguageModelMgr_AddLateLanguageModel(object sender, AddLanguageModelEventArgs e)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return;
			}
			int handle = primaryProject.Handle;
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, e.ApplicationGuid);
			Debug.Assert(metaObjectStub != null);
			if (typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub.ObjectType) || !typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return;
			}
			do
			{
				if (metaObjectStub.ParentObjectGuid == Guid.Empty)
				{
					return;
				}
				metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, metaObjectStub.ParentObjectGuid);
			}
			while (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType));
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
			if (objectToRead.Object is DeviceObject)
			{
				IDeviceObjectBase obj = (IDeviceObjectBase)objectToRead.Object;
				DateTime now = DateTime.Now;
				obj.AddLateLanguageModel(handle, e);
				TimeSpan timeSpan = DateTime.Now - now;
				Debug.WriteLine("--- Performance of AddLateLanguageModel: ---");
				Debug.WriteLine("Overall: " + timeSpan.ToString());
			}
		}

		private void OnSVNodesRemovingInterception(object sender, SVNodesRemovingEventArgs e)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Expected O, but got Unknown
			for (int num = e.SVNodesToDelete.Count - 1; num >= 0; num--)
			{
				ISVNode val = (ISVNode)e.SVNodesToDelete[num];
				if (!typeof(SlotDeviceObject).IsAssignableFrom(val.ObjectType))
				{
					continue;
				}
				IMetaObject val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(val.ProjectHandle, val.ObjectGuid);
				SlotDeviceObject slotDeviceObject = (SlotDeviceObject)(object)val2.Object;
				if (!slotDeviceObject.AllowEmpty)
				{
					if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("DeviceObject", "HideDeleteEmptySlotMessage") && ((IEngine3)APEnvironment.Engine).OEMCustomization.GetBoolValue("DeviceObject", "HideDeleteEmptySlotMessage"))
					{
						continue;
					}
					bool flag = false;
					foreach (ISVNode item in e.SVNodesToDelete)
					{
						ISVNode val3 = item;
						if (val.Parent != null && val.Parent.ObjectGuid == val3.ObjectGuid)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						string fullName = ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(val.ProjectHandle, val.ObjectGuid);
						string text = ((val.Parent != null) ? ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(val.Parent.ProjectHandle, val.Parent.ObjectGuid) : "<root>");
						throw new DeleteObjectConstraintException(val.ProjectHandle, val.ObjectGuid, fullName, text);
					}
				}
				LDictionary<Guid, string> val4 = new LDictionary<Guid, string>();
				CollectObjectGuids(val4, val2.ProjectHandle, val2.SubObjectGuids, typeof(IDeviceObject), bRecursive: true);
				CollectObjectGuids(val4, val2.ProjectHandle, val2.SubObjectGuids, typeof(IExplicitConnector), bRecursive: true);
				for (int num2 = e.SVNodesToDelete.Count - 1; num2 >= 0; num2--)
				{
					ISVNode val5 = (ISVNode)e.SVNodesToDelete[num2];
					if (((IEnumerable<Guid>)val4.Keys).Contains(val5.ObjectGuid))
					{
						e.SVNodesToDelete.RemoveAt(num2);
					}
				}
				DeviceCommandHelper.UnplugDeviceFromSlot(val.ProjectHandle, val.ObjectGuid, bCheckBeforeUnplug: true);
				if (slotDeviceObject.AllowEmpty)
				{
					try
					{
						val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val2);
						LList<Guid> val6 = new LList<Guid>();
						IObjectProperty[] properties = val2.Properties;
						for (int i = 0; i < properties.Length; i++)
						{
							Guid typeGuid = Common.GetTypeGuid(((object)properties[i]).GetType());
							if (typeGuid != Guid.Empty && typeGuid != DeviceProperty.Guid)
							{
								val6.Add(typeGuid);
							}
						}
						foreach (Guid item2 in val6)
						{
							val2.RemoveProperty(item2);
						}
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
				e.SVNodesToDelete.RemoveAt(num);
			}
		}

		internal IDeviceIdentification LoadDragData(byte[] byData, out string stDeviceNameForDrag, out AddDeviceContext context)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			stDeviceNameForDrag = null;
			context = (AddDeviceContext)0;
			if (byData != null && byData.Length != 0)
			{
				try
				{
					ChunkedMemoryStream val = new ChunkedMemoryStream(byData);
					try
					{
						XmlTextReader xmlTextReader = new XmlTextReader((Stream)(object)val);
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.NodeType != XmlNodeType.Element)
							{
								continue;
							}
							if (xmlTextReader.Name.Equals("DeviceCatalogue"))
							{
								string attribute = xmlTextReader.GetAttribute("Type");
								string attribute2 = xmlTextReader.GetAttribute("Id");
								string attribute3 = xmlTextReader.GetAttribute("Version");
								string attribute4 = xmlTextReader.GetAttribute("ModuleId");
								stDeviceNameForDrag = xmlTextReader.GetAttribute("DeviceNameForDrag");
								string attribute5 = xmlTextReader.GetAttribute("AddDeviceContext");
								if (!string.IsNullOrEmpty(attribute5) && int.TryParse(attribute5, out var result))
								{
									context = (AddDeviceContext)result;
								}
								IDeviceIdentification val2 = null;
								if (!string.IsNullOrEmpty(attribute) && int.TryParse(attribute, out var result2))
								{
									if (!string.IsNullOrEmpty(attribute4))
									{
										val2 = (IDeviceIdentification)(object)new ModuleIdentification();
										(val2 as ModuleIdentification).ModuleId = attribute4;
									}
									else
									{
										val2 = (IDeviceIdentification)(object)new DeviceIdentification();
									}
									(val2 as DeviceIdentification).Type = result2;
									(val2 as DeviceIdentification).Id = attribute2;
									(val2 as DeviceIdentification).Version = attribute3;
								}
								return val2;
							}
							xmlTextReader.Skip();
						}
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				catch
				{
				}
			}
			return null;
		}

		private void OnSVNodesDragOverInterception(object sender, SVNodesDragOverEventArgs e)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Expected O, but got Unknown
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Invalid comparison between Unknown and I4
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Expected O, but got Unknown
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Expected O, but got Unknown
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Expected O, but got Unknown
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Expected I4, but got Unknown
			//IL_070c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Expected O, but got Unknown
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Expected O, but got Unknown
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Invalid comparison between Unknown and I4
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Expected O, but got Unknown
			//IL_0887: Unknown result type (might be due to invalid IL or missing references)
			//IL_088d: Expected O, but got Unknown
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Expected O, but got Unknown
			if (e.DataObject != null && e.DataObject.GetDataPresent("NavigatorDragData"))
			{
				if (e.DestinationGuid == Guid.Empty || ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.DestinationGuid) == null)
				{
					return;
				}
				byte[] array = e.DataObject.GetData("NavigatorDragData") as byte[];
				if (array != null && array.Length != 0)
				{
					ChunkedMemoryStream val = new ChunkedMemoryStream(array);
					try
					{
						IArchiveReader obj = APEnvironment.CreateBinaryArchiveReader();
						obj.Initialize((Stream)(object)val);
						object obj2 = obj.Load();
						if (obj2 is INavigatorDragData)
						{
							INavigatorDragData val2 = (INavigatorDragData)((obj2 is INavigatorDragData) ? obj2 : null);
							LList<Guid> val3 = new LList<Guid>();
							Guid[] objectGuids = val2.ObjectGuids;
							foreach (Guid guid in objectGuids)
							{
								if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(val2.SourceProjectHandle, guid))
								{
									IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val2.SourceProjectHandle, guid);
									if (!val2.ObjectGuids.Contains(metaObjectStub.ParentObjectGuid))
									{
										val3.Add(guid);
									}
								}
							}
							foreach (Guid item in val3)
							{
								if (item == e.DestinationGuid)
								{
									e.Handled=(true);
									e.Effects=(DragDropEffects.None);
									break;
								}
								if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(val2.SourceProjectHandle, item))
								{
									IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val2.SourceProjectHandle, item);
									if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType))
									{
										e.Handled=(true);
										e.Effects=(DragDropEffects.None);
										IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(val2.SourceProjectHandle, item);
										IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.DestinationGuid);
										bool flag = false;
										bool flag2;
										do
										{
											flag2 = false;
											if (!(objectToRead.Object is IDeviceObject))
											{
												continue;
											}
											IObject @object = objectToRead.Object;
											IDeviceObject val4 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
											if (objectToRead2.Object is ILogicalObject && val4 is ILogicalDevice && ((ILogicalDevice)((val4 is ILogicalDevice) ? val4 : null)).IsLogical)
											{
												if ((e.KeyState & 8) == 8)
												{
													e.Effects=(DragDropEffects.Copy);
												}
												else
												{
													e.Effects=(DragDropEffects.Move);
												}
												break;
											}
											if (objectToRead2.Object is IFolderObject)
											{
												IObject object2 = objectToRead2.Object;
												IFolderObject val5 = (IFolderObject)(object)((object2 is IFolderObject) ? object2 : null);
												if (val5 != null)
												{
													ISVNode node = ((IObjectManager)APEnvironment.ObjectMgr).GetStructuredView(e.ProjectHandle, val5.StructuredViewGuid).GetNode(objectToRead2.ObjectGuid);
													if (node != null)
													{
														objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, node.Parent.ObjectGuid);
														flag = true;
														flag2 = true;
													}
												}
											}
											if (objectToRead2.Object is IDeviceObject)
											{
												IObject object3 = objectToRead2.Object;
												IDeviceObject val6 = (IDeviceObject)(object)((object3 is IDeviceObject) ? object3 : null);
												foreach (IConnector item2 in (IEnumerable)val6.Connectors)
												{
													IConnector val7 = item2;
													if ((int)val7.ConnectorRole == 1 && !(objectToRead2.Object is ISlotDeviceObject))
													{
														continue;
													}
													foreach (IConnector7 item3 in (IEnumerable)val4.Connectors)
													{
														IConnector7 val8 = item3;
														if (!DeviceManager.CheckMatchInterface((IConnector7)(object)((val7 is IConnector7) ? val7 : null), val8) || val7.IsExplicit || ((IConnector)val8).IsExplicit || (val7.ConnectorRole != ((IConnector)val8).ConnectorRole && (int)val7.ConnectorRole != 0))
														{
															continue;
														}
														if ((int)val7.ConnectorRole == 0)
														{
															bool flag3 = objectToRead2.Object is ISlotDeviceObject;
															foreach (IAdapter item4 in (IEnumerable)val7.Adapters)
															{
																IAdapter val9 = item4;
																if (val9 is VarAdapter)
																{
																	if ((val9 as VarAdapter).ModulesCount != (val9 as VarAdapter).MaxDevices || (flag && objectToRead.ParentObjectGuid == objectToRead2.ObjectGuid && (e.KeyState & 8) != 8))
																	{
																		flag3 = true;
																	}
																	break;
																}
															}
															if (!flag3)
															{
																continue;
															}
														}
														if ((e.KeyState & 8) == 8)
														{
															e.Effects=(DragDropEffects.Copy);
														}
														else
														{
															e.Effects=(DragDropEffects.Move);
														}
														break;
													}
												}
												if (e.Effects == DragDropEffects.None && !flag && !(val6 is ISlotDeviceObject))
												{
													objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, objectToRead2.ParentObjectGuid);
													flag = true;
													flag2 = true;
												}
											}
											if (!(objectToRead2.Object is IExplicitConnector))
											{
												continue;
											}
											IObject object4 = objectToRead2.Object;
											IExplicitConnector val10 = (IExplicitConnector)(object)((object4 is IExplicitConnector) ? object4 : null);
											foreach (IConnector7 item5 in (IEnumerable)val4.Connectors)
											{
												IConnector7 childConnector = item5;
												if (DeviceManager.CheckMatchInterface((IConnector7)(object)((val10 is IConnector7) ? val10 : null), childConnector))
												{
													if ((e.KeyState & 8) == 8)
													{
														e.Effects=(DragDropEffects.Copy);
													}
													else
													{
														e.Effects=(DragDropEffects.Move);
													}
													break;
												}
											}
											if (e.Effects == DragDropEffects.None && !flag)
											{
												objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, objectToRead2.ParentObjectGuid);
												flag = true;
												flag2 = true;
											}
										}
										while (flag2 && e.Effects == DragDropEffects.None);
									}
									if (typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub2.ObjectType))
									{
										e.Handled=(true);
										e.Effects=(DragDropEffects.None);
									}
								}
								if (e.Handled && e.Effects == DragDropEffects.None)
								{
									break;
								}
							}
						}
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
			}
			if (e.DataObject == null || !e.DataObject.GetDataPresent("DeviceCatalogue"))
			{
				return;
			}
			byte[] byData = e.DataObject.GetData("DeviceCatalogue") as byte[];
			string stDeviceNameForDrag;
			AddDeviceContext context;
			IDeviceIdentification val11 = LoadDragData(byData, out stDeviceNameForDrag, out context);
			if (val11 == null)
			{
				return;
			}
			IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(val11);
			if (device == null)
			{
				return;
			}
			e.Handled=(true);
			e.Effects=(DragDropEffects.None);
			if (e.DestinationGuid == Guid.Empty)
			{
				if (device.AllowTopLevel && e.NodeSelected)
				{
					e.Effects=(DragDropEffects.Copy);
				}
				return;
			}
			try
			{
				IMetaObject val12 = null;
				switch (context - 1)
				{
				case 0:
				case 3:
					val12 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.DestinationGuid);
					break;
				case 1:
				case 2:
				{
					IMetaObjectStub metaObjectStub3 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.DestinationGuid);
					val12 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, metaObjectStub3.ParentObjectGuid);
					break;
				}
				}
				if (val12 == null)
				{
					return;
				}
				if (val12.Object is IDeviceObject)
				{
					IObject object5 = val12.Object;
					IDeviceObject val13 = (IDeviceObject)(object)((object5 is IDeviceObject) ? object5 : null);
					foreach (IConnector item6 in (IEnumerable)device.Connectors)
					{
						IConnector val14 = item6;
						foreach (IConnector7 item7 in (IEnumerable)val13.Connectors)
						{
							IConnector7 val15 = item7;
							if (!DeviceManager.CheckMatchInterface((IConnector7)(object)((val14 is IConnector7) ? val14 : null), val15) || ((IConnector)val15).IsExplicit || (!(val12.Object is ISlotDeviceObject) && ((int)val14.ConnectorRole != 1 || (int)((IConnector)val15).ConnectorRole != 0)))
							{
								continue;
							}
							bool flag4 = true;
							foreach (IAdapter item8 in (IEnumerable)((IConnector)val15).Adapters)
							{
								IAdapter val16 = item8;
								if (val16 is VarAdapter && (val16 as VarAdapter).ModulesCount == (val16 as VarAdapter).MaxDevices)
								{
									flag4 = false;
									break;
								}
							}
							if (flag4 && (val15 as Connector).CheckConstraints(val11, bRecursion: false, e.DestinationGuid, bCheck: true))
							{
								e.Effects=(DragDropEffects.Copy);
							}
							break;
						}
					}
				}
				if (!(val12.Object is IExplicitConnector))
				{
					return;
				}
				IObject object6 = val12.Object;
				IExplicitConnector val17 = (IExplicitConnector)(object)((object6 is IExplicitConnector) ? object6 : null);
				foreach (IConnector item9 in (IEnumerable)device.Connectors)
				{
					object obj3 = (object)item9;
					if (!DeviceManager.CheckMatchInterface((IConnector7)((obj3 is IConnector7) ? obj3 : null), (IConnector7)(object)((val17 is IConnector7) ? val17 : null)))
					{
						continue;
					}
					bool flag5 = true;
					foreach (IAdapter item10 in (IEnumerable)((IConnector)val17).Adapters)
					{
						IAdapter val18 = item10;
						if (val18 is VarAdapter && (val18 as VarAdapter).ModulesCount == (val18 as VarAdapter).MaxDevices)
						{
							flag5 = false;
							break;
						}
					}
					if (flag5 && (val17 as ExplicitConnector).CheckConstraints(val11, bRecursion: false, e.DestinationGuid, bCheck: true))
					{
						e.Effects=(DragDropEffects.Copy);
					}
					break;
				}
			}
			catch
			{
			}
		}

		private void OnSVNodesDragDropInterception(object sender, SVNodesDragDropEventArgs e)
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Expected I4, but got Unknown
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Expected I4, but got Unknown
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Expected O, but got Unknown
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Expected O, but got Unknown
			if (e.DataObject == null || !e.DataObject.GetDataPresent("DeviceCatalogue"))
			{
				return;
			}
			byte[] byData = e.DataObject.GetData("DeviceCatalogue") as byte[];
			string stDeviceNameForDrag;
			AddDeviceContext context;
			IDeviceIdentification val = LoadDragData(byData, out stDeviceNameForDrag, out context);
			if (val == null)
			{
				return;
			}
			string text = stDeviceNameForDrag;
			IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(val);
			if (device == null)
			{
				return;
			}
			IMetaObjectStub val2 = null;
			if (!device.AllowTopLevel)
			{
				if (e.DestinationGuid == Guid.Empty)
				{
					return;
				}
				val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.DestinationGuid);
				if (val2 == null)
				{
					return;
				}
			}
			e.Effects=(DragDropEffects.None);
			if (val2 != null && typeof(ISlotDeviceObject).IsAssignableFrom(val2.ObjectType))
			{
				switch (context - 1)
				{
				case 3:
					if (string.IsNullOrEmpty(text))
					{
						string stBaseName = CreateInstanceNameBase(device.DeviceInfo);
						text = CreateUniqueIdentifier(e.ProjectHandle, stBaseName, GetHostStub(e.ProjectHandle, e.DestinationGuid));
					}
					DeviceCommandHelper.PlugDeviceIntoSlot(e.ProjectHandle, e.DestinationGuid, device.DeviceIdentification, text);
					break;
				case 1:
				{
					IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(e.ProjectHandle);
					undoManager.BeginCompoundAction(ReplaceDeviceAction.REPLACEACTION);
					bool flag = false;
					try
					{
						ReplaceDeviceAction replaceDeviceAction = new ReplaceDeviceAction(e.ProjectHandle, e.DestinationGuid, device.DeviceIdentification, null);
						undoManager.AddAction((IUndoableAction)(object)replaceDeviceAction);
						replaceDeviceAction.Redo();
						flag = true;
					}
					catch (Exception ex)
					{
						ArrayList arrayList = new ArrayList();
						arrayList.Add(e.DestinationGuid);
						if (APEnvironment.DeviceMgr.RaiseDeviceShowExecption(e.ProjectHandle, arrayList, ex.Message))
						{
							APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
						}
					}
					finally
					{
						undoManager.EndCompoundAction();
						if (!flag)
						{
							undoManager.Undo();
						}
					}
					break;
				}
				case 0:
				case 2:
					break;
				}
				return;
			}
			IDeviceIdentification deviceIdentification = device.DeviceIdentification;
			int projectHandle = e.ProjectHandle;
			Guid guid = Guid.Empty;
			int num = -1;
			switch (context - 1)
			{
			case 0:
				guid = e.DestinationGuid;
				break;
			case 2:
				num = val2.Index;
				guid = val2.ParentObjectGuid;
				break;
			case 1:
			{
				IUndoManager undoManager2 = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(e.ProjectHandle);
				undoManager2.BeginCompoundAction(ReplaceDeviceAction.REPLACEACTION);
				bool flag2 = false;
				try
				{
					ReplaceDeviceAction replaceDeviceAction2 = new ReplaceDeviceAction(projectHandle, e.DestinationGuid, deviceIdentification, null);
					undoManager2.AddAction((IUndoableAction)(object)replaceDeviceAction2);
					replaceDeviceAction2.Redo();
					flag2 = true;
				}
				catch
				{
				}
				finally
				{
					undoManager2.EndCompoundAction();
					if (!flag2)
					{
						undoManager2.Undo();
					}
				}
				break;
			}
			}
			if (!(guid != Guid.Empty) && !device.AllowTopLevel)
			{
				return;
			}
			IObjectFactory val3 = APEnvironment.CreateDeviceObjectFactory();
			IDeviceObject val4;
			if (deviceIdentification is IModuleIdentification)
			{
				string moduleId = ((IModuleIdentification)deviceIdentification).ModuleId;
				val4 = (IDeviceObject)val3.Create(new string[7]
				{
					deviceIdentification.Type.ToString(),
					deviceIdentification.Id,
					deviceIdentification.Version,
					moduleId,
					"1",
					projectHandle.ToString(),
					guid.ToString()
				});
			}
			else
			{
				val4 = (IDeviceObject)val3.Create(new string[7]
				{
					deviceIdentification.Type.ToString(),
					deviceIdentification.Id,
					deviceIdentification.Version,
					"",
					"0",
					projectHandle.ToString(),
					guid.ToString()
				});
			}
			IUndoManager undoManager3 = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(projectHandle);
			bool flag3 = false;
			try
			{
				undoManager3.BeginCompoundAction("DragDrop");
				if (string.IsNullOrEmpty(text))
				{
					string stBaseName2 = CreateInstanceNameBase(device.DeviceInfo);
					text = CreateUniqueIdentifier(e.ProjectHandle, stBaseName2, GetHostStub(e.ProjectHandle, e.DestinationGuid));
				}
				Guid guid2 = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(projectHandle, guid, Guid.NewGuid(), (IObject)(object)val4, text, num);
				val3.ObjectCreated(projectHandle, guid2);
				flag3 = true;
			}
			catch (Exception ex2)
			{
				ArrayList arrayList2 = new ArrayList();
				arrayList2.Add(e.DestinationGuid);
				if (APEnvironment.DeviceMgr.RaiseDeviceShowExecption(e.ProjectHandle, arrayList2, ex2.Message))
				{
					APEnvironment.MessageService.Error(ex2.Message, "Exception", Array.Empty<object>());
				}
			}
			finally
			{
				undoManager3.EndCompoundAction();
				if (!flag3)
				{
					try
					{
						undoManager3.Undo();
					}
					catch
					{
					}
				}
			}
		}

		internal static bool IsDeviceWithPlcLogic(IDeviceDescription10 devdesc)
		{
			bool result = false;
			if (devdesc != null)
			{
				if (!devdesc.FuncChildObjects.ContainsKey(GUID_PLCLOGICOBJECTFACTORY))
				{
					{
						foreach (Guid key in devdesc.FuncChildObjects.Keys)
						{
							IObjectFactory val = APEnvironment.TryCreateObjectFactory(key);
							if (val != null && typeof(IPlcLogicObject).IsAssignableFrom(val.ObjectType))
							{
								result = true;
							}
						}
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		private void OnSVNodesPastingInterception(object sender, SVNodesPastingEventArgs e)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Expected O, but got Unknown
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Expected O, but got Unknown
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Expected O, but got Unknown
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Expected O, but got Unknown
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Invalid comparison between Unknown and I4
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Expected O, but got Unknown
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Expected O, but got Unknown
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b4: Expected O, but got Unknown
			//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b00: Expected O, but got Unknown
			try
			{
				LDictionary<Guid, IPastedObject> val = new LDictionary<Guid, IPastedObject>();
				LDictionary<Guid, IPastedObject> val2 = new LDictionary<Guid, IPastedObject>();
				LDictionary<string, string> val3 = new LDictionary<string, string>();
				foreach (IPastedObject item in e.ObjectsToPaste)
				{
					IPastedObject val4 = item;
					val.set_Item(val4.ObjectGuid, val4);
					val2.set_Item(val4.OldObjectGuid, val4);
				}
				LDictionary<Guid, IPastedObject> val5 = new LDictionary<Guid, IPastedObject>();
				LList<IPastedObject> val6 = new LList<IPastedObject>();
				IMetaObjectStub hostStub = GetHostStub(e.ProjectHandle, e.SelectedNodeGuid);
				if (hostStub != null)
				{
					foreach (IPastedObject item2 in e.ObjectsToPaste)
					{
						IPastedObject val7 = item2;
						if ((!(val7.Object is IDeviceObjectBase) && !(val7.Object is LogicalIODevice)) || !(val7 is IPastedObject2))
						{
							continue;
						}
						IMetaObjectStub host = hostStub;
						if (val7.Object is SlotDeviceObject)
						{
							if (!(val7.Object as SlotDeviceObject).HasDevice)
							{
								continue;
							}
							IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice((val7.Object as SlotDeviceObject).DeviceIdentificationNoSimulation);
							if (IsDeviceWithPlcLogic((IDeviceDescription10)(object)((device is IDeviceDescription10) ? device : null)))
							{
								host = null;
							}
						}
						if (val7.Name.StartsWith("<"))
						{
							continue;
						}
						bool flag = val7.Object is LogicalIODevice;
						if (!CheckUniqueIdentifier(e.ProjectHandle, val7.Name, Guid.Empty, host, bCheckAll: false, flag, out var _))
						{
							string text = CreateUniqueIdentifier(e.ProjectHandle, val7.Name, Guid.Empty, host, bCheckAll: false, flag);
							if (flag)
							{
								val3.set_Item(val7.Name, text);
							}
							((IPastedObject2)((val7 is IPastedObject2) ? val7 : null)).ChangeName(text);
						}
					}
				}
				string mappedDevice = default(string);
				IPastedObject val11 = default(IPastedObject);
				IPastedObject val16 = default(IPastedObject);
				foreach (IPastedObject item3 in e.ObjectsToPaste)
				{
					IPastedObject val8 = item3;
					if (val8.Object is LogicalIODevice)
					{
						continue;
					}
					if (val8.Object is IDeviceObjectBase)
					{
						((IDeviceObjectBase)val8.Object).PreparePaste();
					}
					else if (val8.Object is ExplicitConnector)
					{
						((ExplicitConnector)(object)val8.Object).PreparePaste();
					}
					if (val8.Object is ILogicalDevice && ((ILogicalDevice)/*isinst with value type is only supported in some contexts*/).IsPhysical)
					{
						IObject @object = val8.Object;
						ILogicalDevice val9 = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
						if (val9.MappedDevices != null && ((ICollection)val9.MappedDevices).Count > 0)
						{
							foreach (IMappedDevice item4 in (IEnumerable)val9.MappedDevices)
							{
								IMappedDevice val10 = item4;
								if (val3.TryGetValue(val10.MappedDevice, ref mappedDevice))
								{
									val10.MappedDevice=(mappedDevice);
								}
							}
						}
					}
					val.TryGetValue(val8.ParentSVNodeGuid, ref val11);
					if (val11 == null)
					{
						if (val8.Object is SlotDeviceObject)
						{
							SlotDeviceObject slotDeviceObject = (SlotDeviceObject)(object)val8.Object;
							if (slotDeviceObject.HasDevice)
							{
								if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.ProjectHandle, e.SelectedNodeGuid))
								{
									IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.SelectedNodeGuid);
									if (!(objectToRead.Object is SlotDeviceObject))
									{
										bool flag2 = false;
										if (objectToRead.Object is DeviceObject)
										{
											foreach (IConnector item5 in (IEnumerable)(objectToRead.Object as DeviceObject).Connectors)
											{
												IConnector val12 = item5;
												if ((int)val12.ConnectorRole == 1)
												{
													continue;
												}
												foreach (IAdapter item6 in (IEnumerable)val12.Adapters)
												{
													IAdapter val13 = item6;
													if (!(val13 is SlotAdapter))
													{
														continue;
													}
													SlotAdapter slotAdapter = val13 as SlotAdapter;
													for (int i = 0; i < slotAdapter.ModulesCount; i++)
													{
														if (slotAdapter.Modules[i] == Guid.Empty)
														{
															flag2 = true;
														}
													}
												}
											}
										}
										if (objectToRead.Object is ExplicitConnector)
										{
											foreach (IAdapter item7 in (IEnumerable)(objectToRead.Object as ExplicitConnector).Adapters)
											{
												IAdapter val14 = item7;
												if (!(val14 is SlotAdapter))
												{
													continue;
												}
												SlotAdapter slotAdapter2 = val14 as SlotAdapter;
												for (int j = 0; j < slotAdapter2.ModulesCount; j++)
												{
													if (slotAdapter2.Modules[j] == Guid.Empty)
													{
														flag2 = true;
													}
												}
											}
										}
										if (!flag2)
										{
											val6.Add(val8);
										}
										continue;
									}
								}
								val8.Object=((IObject)(object)slotDeviceObject.GetDevice());
								val6.Add(val8);
							}
							else
							{
								val5.Add(val8.ObjectGuid, (IPastedObject)null);
							}
						}
						else if (val8.Object is IDeviceObject)
						{
							val6.Add(val8);
						}
					}
					else if (val5.ContainsKey(val8.ParentSVNodeGuid))
					{
						val5.Add(val8.ObjectGuid, (IPastedObject)null);
					}
					else if (val11.Object is IDeviceObjectBase)
					{
						((IDeviceObjectBase)val11.Object).UpdatePasteModuleGuid(val8.OldObjectGuid, val8.ObjectGuid);
					}
					else if (val11.Object is ExplicitConnector)
					{
						((ExplicitConnector)(object)val11.Object).UpdatePasteModuleGuid(val8.OldObjectGuid, val8.ObjectGuid);
					}
					if (!(val8.Object is IDeviceObjectBase) || !(val8.OldParentSVNodeGuid != val8.ParentSVNodeGuid))
					{
						continue;
					}
					IDeviceObjectBase obj = (IDeviceObjectBase)val8.Object;
					obj.UpdatePasteModuleGuid(val8.OldParentSVNodeGuid, val8.ParentSVNodeGuid);
					IDriverInfo driverInfo = ((IDeviceObject2)obj).DriverInfo;
					IDriverInfo2 val15 = (IDriverInfo2)(object)((driverInfo is IDriverInfo2) ? driverInfo : null);
					if (val15 != null && val15.IoApplication != Guid.Empty)
					{
						val2.TryGetValue(val15.IoApplication, ref val16);
						if (val16 != null && val16.Object is IApplicationObject)
						{
							val15.IoApplication=(val16.ObjectGuid);
						}
					}
				}
				for (int num = val6.Count - 1; num >= 0; num--)
				{
					if (val6[num].PastePosition >= 0)
					{
						val6.RemoveAt(num);
					}
				}
				if (val6.Count <= 0)
				{
					goto IL_0ad2;
				}
				IMetaObject val17 = null;
				IMetaObject val18 = null;
				if (e.SelectedNodeGuid != Guid.Empty)
				{
					val17 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, e.SelectedNodeGuid);
					if (val17.ParentObjectGuid != Guid.Empty)
					{
						val18 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(e.ProjectHandle, val17.ParentObjectGuid);
					}
				}
				SortDevicesByPastePosition(val6, val17, val18, out var alDevicesToAppend, out var alDevicesToInsert, out var alDevicesAmbigous);
				PastePosition pastePosition;
				if (alDevicesToAppend.Count > 0)
				{
					if (alDevicesToInsert.Count > 0 || alDevicesAmbigous.Count > 0)
					{
						throw new DeviceObjectException((DeviceObjectExeptionReason)2, Strings.InconsistentPastePosition);
					}
					pastePosition = PastePosition.AppendBelow;
					goto IL_07f0;
				}
				if (alDevicesToInsert.Count > 0)
				{
					if (alDevicesAmbigous.Count > 0)
					{
						throw new DeviceObjectException((DeviceObjectExeptionReason)2, Strings.InconsistentPastePosition);
					}
					pastePosition = PastePosition.InsertBefore;
					goto IL_07f0;
				}
				if (((IEngine)APEnvironment.Engine).Frame != null)
				{
					SelectPastePositionForm selectPastePositionForm = new SelectPastePositionForm();
					selectPastePositionForm.TargetElement = val17.Name;
					selectPastePositionForm.PasteObjects = val6;
					if (selectPastePositionForm.ShowDialog((IWin32Window)APEnvironment.FrameForm) == DialogResult.OK)
					{
						pastePosition = selectPastePositionForm.PastePosition;
						goto IL_07f0;
					}
					e.ObjectsToPaste.Clear();
					return;
				}
				pastePosition = PastePosition.AppendBelow;
				goto IL_07f0;
				IL_0ad2:
				if (val5.Count <= 0)
				{
					return;
				}
				for (int num2 = e.ObjectsToPaste.Count - 1; num2 >= 0; num2--)
				{
					IPastedObject val19 = (IPastedObject)e.ObjectsToPaste[num2];
					if (val5.ContainsKey(val19.ObjectGuid))
					{
						e.ObjectsToPaste.RemoveAt(num2);
					}
				}
				goto end_IL_0000;
				IL_07f0:
				if (pastePosition == PastePosition.InsertBefore && val6.Count == 1 && val17.Object is SlotDeviceObject)
				{
					foreach (IPastedObject item8 in val6)
					{
						string stName = CreateUniqueIdentifier(e.ProjectHandle, item8.Name, GetHostStub(e.ProjectHandle, val17.ObjectGuid));
						DeviceCommandHelper.PlugDeviceIntoSlot(e.ProjectHandle, val17.ObjectGuid, item8.Object as DeviceObject, stName);
						try
						{
							val17 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val17);
							LList<Guid> val20 = new LList<Guid>();
							IObjectProperty[] properties = val17.Properties;
							for (int k = 0; k < properties.Length; k++)
							{
								Guid typeGuid = Common.GetTypeGuid(((object)properties[k]).GetType());
								if (typeGuid != Guid.Empty)
								{
									val20.Add(typeGuid);
								}
							}
							foreach (Guid item9 in val20)
							{
								val17.RemoveProperty(item9);
							}
							properties = item8.Properties;
							foreach (IObjectProperty val21 in properties)
							{
								_003F val22 = val17;
								object obj2 = ((ICloneable)val21).Clone();
								((IMetaObject)val22).AddProperty((IObjectProperty)((obj2 is IObjectProperty) ? obj2 : null));
							}
						}
						catch
						{
						}
						finally
						{
							if (val17 != null && val17.IsToModify)
							{
								((IObjectManager)APEnvironment.ObjectMgr).SetObject(val17, true, (object)null);
							}
						}
						Guid[] subObjectGuids = val17.SubObjectGuids;
						foreach (Guid guid in subObjectGuids)
						{
							try
							{
								((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(e.ProjectHandle, guid);
							}
							catch
							{
							}
						}
						foreach (IPastedObject item10 in e.ObjectsToPaste)
						{
							IPastedObject val23 = item10;
							if (val23.ParentSVNodeGuid == item8.ObjectGuid)
							{
								val23.ParentSVNodeGuid=(val17.ObjectGuid);
							}
						}
						e.ObjectsToPaste.Remove(item8);
					}
					return;
				}
				switch (pastePosition)
				{
				case PastePosition.AppendBelow:
					foreach (IPastedObject item11 in val6)
					{
						item11.ParentSVNodeGuid=(e.SelectedNodeGuid);
					}
					break;
				case PastePosition.InsertBefore:
				{
					int num3 = ((IOrderedSubObjects)val18.Object).GetChildIndex(val17.ObjectGuid);
					if (num3 < 0)
					{
						throw new InvalidOperationException("Invalid insert position");
					}
					foreach (IPastedObject item12 in val6)
					{
						item12.ParentSVNodeGuid=(val18.ObjectGuid);
						item12.PastePosition=(num3);
						num3++;
					}
					break;
				}
				}
				goto IL_0ad2;
				end_IL_0000:;
			}
			catch (Exception ex)
			{
				if (APEnvironment.DeviceMgr.RaiseDeviceShowExecption(e.ProjectHandle, e.ObjectsToPaste, ex.Message))
				{
					APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
				}
				e.ObjectsToPaste.Clear();
			}
		}

		private void SortDevicesByPastePosition(LList<IPastedObject> alTopLevelDevices, IMetaObject moTarget, IMetaObject moTargetParent, out LList<IDeviceObjectBase> alDevicesToAppend, out LList<IDeviceObjectBase> alDevicesToInsert, out LList<IPastedObject> alDevicesAmbigous)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Expected O, but got Unknown
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Expected O, but got Unknown
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			string[] parentInterfaces = new string[0];
			alDevicesToInsert = new LList<IDeviceObjectBase>();
			alDevicesAmbigous = new LList<IPastedObject>();
			alDevicesToAppend = new LList<IDeviceObjectBase>();
			if (moTarget == null)
			{
				foreach (IPastedObject alTopLevelDevice in alTopLevelDevices)
				{
					if (alTopLevelDevice.Object is IDeviceObjectBase)
					{
						alDevicesToAppend.Add(alTopLevelDevice.Object as IDeviceObjectBase);
					}
				}
				return;
			}
			if (moTargetParent != null)
			{
				if (moTargetParent.Object is IExplicitConnector)
				{
					int childIndex = ((IOrderedSubObjects)moTargetParent.Object).GetChildIndex(moTarget.ObjectGuid);
					parentInterfaces = ((IExplicitConnector)moTargetParent.Object).GetPossibleInterfacesForInsert(childIndex);
				}
				else if (moTargetParent.Object is IDeviceObjectBase)
				{
					int childIndex2 = ((IOrderedSubObjects)moTargetParent.Object).GetChildIndex(moTarget.ObjectGuid);
					parentInterfaces = ((IDeviceObject)(IDeviceObjectBase)moTargetParent.Object).GetPossibleInterfacesForInsert(childIndex2);
				}
			}
			string[] parentInterfaces2 = ((moTarget.Object is IExplicitConnector) ? ((IExplicitConnector)moTarget.Object).GetPossibleInterfacesForInsert(moTarget.SubObjectGuids.Length) : ((!(moTarget.Object is IDeviceObjectBase)) ? new string[0] : ((IDeviceObject)(IDeviceObjectBase)moTarget.Object).GetPossibleInterfacesForInsert(moTarget.SubObjectGuids.Length)));
			if (moTarget.Object is SlotDeviceObject && alTopLevelDevices.Count == 1)
			{
				parentInterfaces = ((SlotDeviceObject)(object)moTarget.Object).GetPossibleInterfacesForPlug();
			}
			foreach (IPastedObject alTopLevelDevice2 in alTopLevelDevices)
			{
				if (!(alTopLevelDevice2.Object is IDeviceObjectBase))
				{
					continue;
				}
				IDeviceObjectBase deviceObjectBase = (IDeviceObjectBase)alTopLevelDevice2.Object;
				if (HasMatchingChildConnector(deviceObjectBase, parentInterfaces))
				{
					if (HasMatchingChildConnector(deviceObjectBase, parentInterfaces2))
					{
						alDevicesAmbigous.Add(alTopLevelDevice2);
					}
					else
					{
						alDevicesToInsert.Add(deviceObjectBase);
					}
					continue;
				}
				if (HasMatchingChildConnector(deviceObjectBase, parentInterfaces2))
				{
					alDevicesToAppend.Add(deviceObjectBase);
					continue;
				}
				if (moTarget.Object is IExplicitConnector)
				{
					foreach (IAdapter item in (IEnumerable)((IConnector)(IExplicitConnector)moTarget.Object).Adapters)
					{
						object obj = (object)item;
						IVarAdapter val = (IVarAdapter)((obj is IVarAdapter) ? obj : null);
						if (val != null && ((IAdapter)val).ModulesCount >= val.MaxDevices)
						{
							throw new DeviceObjectException((DeviceObjectExeptionReason)3, Strings.Error_Constraint_Reached);
						}
					}
				}
				if (moTarget.Object is IDeviceObjectBase)
				{
					foreach (IConnector item2 in (IEnumerable)((IDeviceObject)(IDeviceObjectBase)moTarget.Object).Connectors)
					{
						foreach (IAdapter item3 in (IEnumerable)item2.Adapters)
						{
							object obj2 = (object)item3;
							IVarAdapter val2 = (IVarAdapter)((obj2 is IVarAdapter) ? obj2 : null);
							if (val2 != null && ((IAdapter)val2).ModulesCount >= val2.MaxDevices)
							{
								throw new DeviceObjectException((DeviceObjectExeptionReason)3, Strings.Error_Constraint_Reached);
							}
						}
					}
				}
				throw new DeviceObjectException((DeviceObjectExeptionReason)3, string.Format(Strings.InvalidPastePosition, alTopLevelDevice2.Name, ((IDeviceObject)deviceObjectBase).DeviceInfo.Name));
			}
		}

		private bool HasMatchingChildConnector(IDeviceObjectBase device, string[] parentInterfaces)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Invalid comparison between Unknown and I4
			if (parentInterfaces == null)
			{
				return false;
			}
			foreach (IConnector item in (IEnumerable)((IDeviceObject)device).Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole != 1)
				{
					continue;
				}
				if (Array.IndexOf(parentInterfaces, val.Interface) >= 0)
				{
					return true;
				}
				if (!(val is IConnector7))
				{
					continue;
				}
				foreach (string additionalInterface in ((IConnector7)((val is IConnector7) ? val : null)).AdditionalInterfaces)
				{
					if (Array.IndexOf(parentInterfaces, additionalInterface) >= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static bool AddListLanguageModel(int nProjectHandle, Guid objectGuid, bool bIsTaskLanguageModel)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected O, but got Unknown
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(nProjectHandle);
			IUndoManager2 val = (IUndoManager2)(object)((undoManager is IUndoManager2) ? undoManager : null);
			if (val != null && (((IUndoManager)val).InCompoundAction || ((IUndoManager)val).InUndo || ((IUndoManager)val).InRedo))
			{
				if (_dictLanguageModelValues.Count == 0 && s_undoMgr == null)
				{
					if (val is IUndoManager4)
					{
						((IUndoManager4)((val is IUndoManager4) ? val : null)).AfterEndCompoundAction2+=((EventHandler)_undoMgr_AfterEndCompoundAction);
					}
					else
					{
						val.AfterEndCompoundAction+=((EventHandler)_undoMgr_AfterEndCompoundAction);
					}
					((IUndoManager)val).ActionUndone+=(new UndoEventHandler(_undoMgr_AfterEndCompoundAction));
					((IUndoManager)val).ActionRedone+=(new UndoEventHandler(_undoMgr_AfterEndCompoundAction));
					s_undoMgr = val;
				}
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, objectGuid))
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid);
					if (metaObjectStub != null)
					{
						if (APEnvironment.Engine.IsUpdateLanguageModelSuppressed && typeof(ITaskObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							LanguageModelData languageModelData = new LanguageModelData(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid, bIsTaskLanguageModel: true, bIsPlc: false);
							string text = objectGuid.ToString() + "_" + languageModelData.IsTaskLanguageModel;
							if (!_dictLanguageModelValues.ContainsKey(text))
							{
								_dictLanguageModelValues.Add(text, languageModelData);
							}
						}
						if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							_htIecAddresses.Clear();
							if (typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
								if (objectToRead != null)
								{
									foreach (IMappedDevice item in (IEnumerable)(objectToRead.Object as LogicalIODevice).MappedDevices)
									{
										Guid getMappedDevice = item.GetMappedDevice;
										if (getMappedDevice != Guid.Empty)
										{
											LanguageModelData languageModelData2 = new LanguageModelData(metaObjectStub.ProjectHandle, getMappedDevice, bIsTaskLanguageModel, bIsPlc: false);
											string text2 = languageModelData2.ObjectGuid.ToString() + "_" + languageModelData2.IsTaskLanguageModel;
											if (!_dictLanguageModelValues.ContainsKey(text2))
											{
												_dictLanguageModelValues.Add(text2, languageModelData2);
											}
										}
									}
								}
							}
							else
							{
								LanguageModelData languageModelData2 = new LanguageModelData(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid, bIsTaskLanguageModel, metaObjectStub.ParentObjectGuid == Guid.Empty);
								string text2 = objectGuid.ToString() + "_" + languageModelData2.IsTaskLanguageModel;
								if (!_dictLanguageModelValues.ContainsKey(text2))
								{
									_dictLanguageModelValues.Add(text2, languageModelData2);
								}
							}
							IMetaObjectStub plcDevice = APEnvironment.DeviceMgr.GetPlcDevice(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
							if (plcDevice != null)
							{
								LanguageModelData languageModelData2 = new LanguageModelData(plcDevice.ProjectHandle, plcDevice.ObjectGuid, bIsTaskLanguageModel, bIsPlc: true);
								string text2 = languageModelData2.ObjectGuid.ToString() + "_" + languageModelData2.IsTaskLanguageModel;
								if (!_dictLanguageModelValues.ContainsKey(text2))
								{
									_dictLanguageModelValues.Add(text2, languageModelData2);
								}
								if (!bIsTaskLanguageModel)
								{
									IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(plcDevice.ProjectHandle, plcDevice.ObjectGuid).Object;
									DeviceObject deviceObject = null;
									deviceObject = ((!(@object is SlotDeviceObject)) ? (@object as DeviceObject) : (@object as SlotDeviceObject).GetDevice());
									if (deviceObject != null && deviceObject.GlobalDataTypes.Count > 0)
									{
										Enumerator<string, LDictionary<IDataElement, Guid>> enumerator2 = deviceObject.GlobalDataTypes.GetEnumerator();
										try
										{
											while (enumerator2.MoveNext())
											{
												KeyValuePair<string, LDictionary<IDataElement, Guid>> current = enumerator2.Current;
												if (!current.Value.ContainsValue(objectGuid))
												{
													continue;
												}
												LList<IDataElement> val2 = new LList<IDataElement>();
												Enumerator<IDataElement, Guid> enumerator3 = current.Value.GetEnumerator();
												try
												{
													while (enumerator3.MoveNext())
													{
														KeyValuePair<IDataElement, Guid> current2 = enumerator3.Current;
														if (current2.Value == objectGuid)
														{
															val2.Add(current2.Key);
														}
													}
												}
												finally
												{
													((IDisposable)enumerator3).Dispose();
												}
												if (val2.Count <= 0)
												{
													continue;
												}
												try
												{
													((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(nProjectHandle, (val2[0] as DataElementStructType).LmStructType);
												}
												catch
												{
												}
												foreach (IDataElement item2 in val2)
												{
													current.Value.Remove(item2);
												}
											}
										}
										finally
										{
											((IDisposable)enumerator2).Dispose();
										}
									}
								}
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		internal static bool IsInListLanguageModel(Guid objectGuid)
		{
			string text = objectGuid.ToString() + "_False";
			if (_dictLanguageModelValues.ContainsKey(text))
			{
				return true;
			}
			return false;
		}

		internal static void _undoMgr_AfterEndCompoundAction(object sender, EventArgs e)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			if (s_undoMgr == null)
			{
				return;
			}
			if (s_undoMgr is IUndoManager4)
			{
				IUndoManager2 obj = s_undoMgr;
				((IUndoManager4)((obj is IUndoManager4) ? obj : null)).AfterEndCompoundAction2-=((EventHandler)_undoMgr_AfterEndCompoundAction);
			}
			else
			{
				s_undoMgr.AfterEndCompoundAction-=((EventHandler)_undoMgr_AfterEndCompoundAction);
			}
			((IUndoManager)s_undoMgr).ActionUndone-=(new UndoEventHandler(_undoMgr_AfterEndCompoundAction));
			((IUndoManager)s_undoMgr).ActionRedone-=(new UndoEventHandler(_undoMgr_AfterEndCompoundAction));
			s_undoMgr = null;
			if (_dictLanguageModelValues.Count <= 0)
			{
				return;
			}
			LList<LanguageModelData> val = new LList<LanguageModelData>();
			bool flag = false;
			for (int i = 0; i < 2; i++)
			{
				Enumerator<string, LanguageModelData> enumerator = _dictLanguageModelValues.Values.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						LanguageModelData current = enumerator.Current;
						if (current.IsPlc == flag)
						{
							val.Add(current);
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				flag = true;
			}
			_dictLanguageModelValues.Clear();
			foreach (LanguageModelData item in val)
			{
				try
				{
					DateTime now = DateTime.Now;
					if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(item.ProjectHandle, item.ObjectGuid))
					{
						goto IL_0310;
					}
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(item.ProjectHandle, item.ObjectGuid);
					if (objectToRead.Object is ExplicitConnector)
					{
						ExplicitConnector explicitConnector = (ExplicitConnector)(object)objectToRead.Object;
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)explicitConnector, true);
					}
					if (objectToRead.Object is SlotDeviceObject)
					{
						SlotDeviceObject slotDeviceObject = objectToRead.Object as SlotDeviceObject;
						Debug.WriteLine("--- GenerateLanguageModel : --- " + slotDeviceObject.MetaObject.Name);
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)slotDeviceObject, true);
					}
					if (!(objectToRead.Object is DeviceObject))
					{
						goto IL_02c6;
					}
					DeviceObject deviceObject = (DeviceObject)(object)objectToRead.Object;
					Debug.WriteLine("--- GenerateLanguageModel : --- " + deviceObject.MetaObject.Name);
					if (item.IsTaskLanguageModel)
					{
						if (!(objectToRead.ParentObjectGuid == Guid.Empty))
						{
							goto IL_0260;
						}
						bool flag2 = false;
						foreach (LanguageModelData item2 in val)
						{
							if (item2.ObjectGuid == objectToRead.ObjectGuid && !item2.IsTaskLanguageModel)
							{
								flag2 = true;
							}
						}
						if (!flag2)
						{
							goto IL_0260;
						}
						continue;
					}
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)deviceObject, true);
					if (objectToRead.ParentObjectGuid == Guid.Empty)
					{
						try
						{
							deviceObject.SetTaskLanguage(bEnable: true);
							((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)deviceObject, true);
						}
						catch
						{
						}
						finally
						{
							deviceObject.SetTaskLanguage(bEnable: false);
						}
					}
					goto IL_02c6;
					IL_0310:
					Debug.WriteLine("Overall: " + (DateTime.Now - now).ToString());
					goto end_IL_0105;
					IL_02c6:
					if (objectToRead.Object is LogicalIOObject)
					{
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)(objectToRead.Object as LogicalIOObject), true);
					}
					if (objectToRead.Object is ITaskConfigObject)
					{
						_003F val2 = APEnvironment.LanguageModelMgr;
						IObject @object = objectToRead.Object;
						((ILanguageModelManager)val2).PutLanguageModel((ILanguageModelProvider)(object)((@object is ILanguageModelProvider) ? @object : null), true);
					}
					goto IL_0310;
					IL_0260:
					try
					{
						deviceObject.SetTaskLanguage(bEnable: true);
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)deviceObject, true);
					}
					finally
					{
						deviceObject.SetTaskLanguage(bEnable: false);
					}
					goto IL_02c6;
					end_IL_0105:;
				}
				catch
				{
				}
			}
		}

		internal static LList<object> GetParameterUpdateFactories(IDeviceObject device, int nConnectorID)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected O, but got Unknown
			if (device == null || ((IObject)device).MetaObject == null)
			{
				return new LList<object>();
			}
			string text = ((IObject)device).MetaObject.ObjectGuid.ToString() + "_" + nConnectorID;
			if (_htUpdateDeviceFactories.ContainsKey(text))
			{
				return _htUpdateDeviceFactories[text];
			}
			LList<object> val = new LList<object>();
			foreach (IUpdateDeviceParametersFactory updateDeviceParametersFactory in APEnvironment.UpdateDeviceParametersFactories)
			{
				foreach (IConnector item in (IEnumerable)device.Connectors)
				{
					IConnector val2 = item;
					if (val2.ConnectorId == nConnectorID && updateDeviceParametersFactory.GetMatch(device, val2))
					{
						val.Add((object)updateDeviceParametersFactory);
					}
				}
			}
			_htUpdateDeviceFactories.Add(text, val);
			return val;
		}

		internal static string EscapeString(string stName)
		{
			char[] array = new char[1];
			StringBuilder stringBuilder = new StringBuilder(stName.Length * 2);
			foreach (char c in stName)
			{
				if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || "_-. ".IndexOf(c) >= 0)
				{
					stringBuilder.Append(c);
					continue;
				}
				array[0] = c;
				byte[] bytes = Encoding.UTF8.GetBytes(array);
				for (int j = 0; j < bytes.Length; j++)
				{
					stringBuilder.AppendFormat("%{0:X2}", bytes[j]);
				}
			}
			return stringBuilder.ToString();
		}

		internal static long GetPackMode(Guid guid)
		{
			if (s_htPackModes.ContainsKey(guid))
			{
				return (long)s_htPackModes[guid];
			}
			return 0L;
		}

		internal static void StorePackMode(Guid guid, long lPackMode)
		{
			if (Guid.Empty != guid)
			{
				s_htPackModes[guid] = lPackMode;
			}
		}

		internal static void ResetPackMode()
		{
			s_htPackModes.Clear();
		}

		internal static long GetPackMode(IDeviceObject host)
		{
			if (host != null && ((IObject)host).MetaObject != null)
			{
				if (s_htPackModes.ContainsKey(((IObject)host).MetaObject.ObjectGuid))
				{
					return (long)s_htPackModes[((IObject)host).MetaObject.ObjectGuid];
				}
				IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((host is IDeviceObject5) ? host : null)).DeviceIdentificationNoSimulation;
				ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceIdentificationNoSimulation);
				long num = LocalTargetSettings.PackMode.GetIntValue(targetSettingsById) * 8;
				s_htPackModes[((IObject)host).MetaObject.ObjectGuid] = num;
				return num;
			}
			return 64L;
		}

		internal static IAddressAssignmentStrategy GetStrategy(Guid hostGuid)
		{
			IAddressAssignmentStrategy result = null;
			_liStrategies.TryGetValue(hostGuid, ref result);
			return result;
		}

		internal static void SaveStrategy(Guid hostGuid, IAddressAssignmentStrategy strategy)
		{
			_liStrategies[hostGuid]= strategy;
		}

		internal static void ResetStrategy()
		{
			_liStrategies.Clear();
		}

		internal static void UpdateAddresses(IIoProvider ioprovider)
		{
			if (ioprovider.ParameterSet is ParameterSet)
			{
				(ioprovider.ParameterSet as ParameterSet).NumParams = 0u;
			}
			if (ioprovider is ConnectorBase)
			{
				(ioprovider as ConnectorBase).Strategy = null;
			}
			ioprovider.Strategy.UpdateAddresses(ioprovider);
			IIoProvider[] children = ioprovider.Children;
			for (int i = 0; i < children.Length; i++)
			{
				UpdateAddresses(children[i]);
			}
		}

		public static void FeatureChanged()
		{
			_bFeatureRead = false;
		}

		public static LList<IIoProvider> GetMappedIoProvider(IIoProvider ioProvider, bool bCheckForLogical)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			LList<IIoProvider> val = new LList<IIoProvider>();
			if (ioProvider != null)
			{
				IMetaObject metaObject = ioProvider.GetMetaObject();
				if (metaObject != null && metaObject.Object is ILogicalDevice)
				{
					IObject @object = metaObject.Object;
					ILogicalDevice val2 = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
					if ((val2.IsLogical && bCheckForLogical) || (val2.IsPhysical && !bCheckForLogical))
					{
						if (val2.MappedDevices == null)
						{
							return null;
						}
						{
							foreach (IMappedDevice item in (IEnumerable)val2.MappedDevices)
							{
								Guid getMappedDevice = item.GetMappedDevice;
								if (!(getMappedDevice != Guid.Empty))
								{
									continue;
								}
								IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObject.ProjectHandle, getMappedDevice);
								if (objectToRead.Object is LogicalExchangeGVLObject)
								{
									LogicalExchangeGVLObject logicalExchangeGVLObject = objectToRead.Object as LogicalExchangeGVLObject;
									if (ioProvider is Connector && logicalExchangeGVLObject.Device != null)
									{
										foreach (Connector item2 in (IEnumerable)logicalExchangeGVLObject.Device.Connectors)
										{
											if (item2.ConnectorId == (ioProvider as Connector).ConnectorId)
											{
												val.Add((IIoProvider)(object)item2);
											}
										}
									}
								}
								if (objectToRead.Object is DeviceObject)
								{
									DeviceObject deviceObject = objectToRead.Object as DeviceObject;
									if (ioProvider is Connector)
									{
										foreach (Connector item3 in (IEnumerable)deviceObject.Connectors)
										{
											if (item3.ConnectorId == (ioProvider as Connector).ConnectorId)
											{
												val.Add((IIoProvider)(object)item3);
											}
										}
									}
									if (ioProvider is DeviceObject)
									{
										val.Add((IIoProvider)(object)deviceObject);
									}
								}
								if (!(objectToRead.Object is SlotDeviceObject))
								{
									continue;
								}
								SlotDeviceObject slotDeviceObject = objectToRead.Object as SlotDeviceObject;
								if (ioProvider is Connector)
								{
									foreach (Connector item4 in (IEnumerable)slotDeviceObject.Connectors)
									{
										if (item4.ConnectorId == (ioProvider as Connector).ConnectorId)
										{
											val.Add((IIoProvider)(object)item4);
										}
									}
								}
								if (ioProvider is DeviceObject)
								{
									val.Add((IIoProvider)(object)slotDeviceObject.GetDevice());
								}
							}
							return val;
						}
					}
				}
			}
			return val;
		}

		internal static void SuppressUpdateObjects(bool bUpdateObjectSuppressed)
		{
			_bUpdateObjectSuppressed = bUpdateObjectSuppressed;
		}

		internal static Guid ConfigModeApplication(Guid plcGuid)
		{
			if (_htConfigModeApplication.ContainsKey(plcGuid))
			{
				return (Guid)_htConfigModeApplication[plcGuid];
			}
			return Guid.Empty;
		}

		internal static void ConfigModeApplication(Guid plcGuid, Guid appGuid)
		{
			_htConfigModeApplication[plcGuid] = appGuid;
		}

		internal static bool CheckLanguageModelGuids(LList<Guid> guids)
		{
			bool result = false;
			if (guids.Count > 0)
			{
				IPreCompileContext[] precompileContexts = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).PrecompileContexts;
				for (int i = 0; i < precompileContexts.Length; i++)
				{
					ISignature[] allSignatures = ((ICompileContextCommon)precompileContexts[i]).AllSignatures;
					foreach (ISignature val in allSignatures)
					{
						if (guids.IndexOf(val.ObjectGuid) != -1)
						{
							return true;
						}
					}
				}
			}
			return result;
		}

		public static bool EnableAdditionalParameters(IDeviceObject hostDevice)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Invalid comparison between Unknown and I4
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Invalid comparison between Unknown and I4
			bool result = false;
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject5)((hostDevice is IDeviceObject5) ? hostDevice : null)).DeviceIdentificationNoSimulation);
			if (targetSettingsById != null)
			{
				IDriverInfo driverInfo = ((IDeviceObject2)((hostDevice is IDeviceObject5) ? hostDevice : null)).DriverInfo;
				AdditionalParameterEnum additionalParameterSetting = ((IDriverInfo9)((driverInfo is IDriverInfo9) ? driverInfo : null)).AdditionalParameterSetting;
				if ((int)additionalParameterSetting == 0)
				{
					result = LocalTargetSettings.EnableAdditionalParameters.GetBoolValue(targetSettingsById);
				}
				else if ((int)additionalParameterSetting == 2 || (int)additionalParameterSetting == 3)
				{
					result = true;
				}
			}
			return result;
		}

		public static bool SkipAdditionalParametersForEmptyConnectors(IDeviceObject hostDevice)
		{
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject5)((hostDevice is IDeviceObject5) ? hostDevice : null)).DeviceIdentificationNoSimulation);
			return LocalTargetSettings.SkipAdditionalParametersForEmptyConnectors.GetBoolValue(targetSettingsById);
		}

		public static bool MotorolaBitfields(IDeviceObject hostDevice)
		{
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject5)((hostDevice is IDeviceObject5) ? hostDevice : null)).DeviceIdentificationNoSimulation);
			return LocalTargetSettings.MotorolaBitfields.GetBoolValue(targetSettingsById);
		}

		internal static ICollection SortedParameterSet(IIoProvider ioprovider)
		{
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)0))
			{
				bool flag = false;
				if (typeof(IDeviceObject9).IsAssignableFrom(((object)ioprovider).GetType()))
				{
					flag = ((IDeviceObject9)((ioprovider is IDeviceObject9) ? ioprovider : null)).ShowParamsInDevDescOrder;
				}
				else
				{
					IMetaObject metaObject = ioprovider.GetMetaObject();
					if (metaObject != null && typeof(IDeviceObject9).IsAssignableFrom(((object)metaObject.Object).GetType()))
					{
						IObject @object = metaObject.Object;
						flag = ((IDeviceObject9)((@object is IDeviceObject9) ? @object : null)).ShowParamsInDevDescOrder;
					}
				}
				if (flag)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.AddRange((ICollection)ioprovider.ParameterSet);
					arrayList.Sort(ParameterByDevDescComparer.Instance);
					return arrayList;
				}
			}
			return (ICollection)ioprovider.ParameterSet;
		}

		internal static void FillIecAddresstable(IDeviceObject host)
		{
			try
			{
				_bIsInLateLanguageModel = true;
				_dictChildrens.Clear();
				_htIecAddresses.Clear();
				FillIecAddresstable((IIoProvider)(object)((host is IIoProvider) ? host : null));
			}
			catch
			{
			}
			finally
			{
				_bIsInLateLanguageModel = false;
			}
		}

		internal static void FillIecAddresstable(IIoProvider ioProvider)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Invalid comparison between Unknown and I4
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Invalid comparison between Unknown and I4
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Invalid comparison between Unknown and I4
			bool flag = false;
			if (ioProvider == null)
			{
				return;
			}
			if (GenerateCodeForLogicalDevices && GetMappedIoProvider(ioProvider, bCheckForLogical: false).Count > 0)
			{
				IMetaObject metaObject = ioProvider.GetMetaObject();
				if (!(metaObject.Object is ILogicalDevice2) || !((ILogicalDevice2)/*isinst with value type is only supported in some contexts*/).MappingPossible)
				{
					flag = true;
				}
			}
			if (!flag && ((ICollection)ioProvider.ParameterSet).Count > 0)
			{
				bool flag2 = false;
				bool flag3 = false;
				foreach (Parameter item in SortedParameterSet(ioProvider))
				{
					if ((int)item.ChannelType == 0)
					{
						continue;
					}
					if (!flag2 && (int)item.ChannelType == 1)
					{
						flag2 = true;
						LateLanguageStartAddresses lateLanguageStartAddresses;
						if (_htIecAddresses.ContainsKey(ioProvider))
						{
							lateLanguageStartAddresses = _htIecAddresses[ioProvider] as LateLanguageStartAddresses;
						}
						else
						{
							lateLanguageStartAddresses = new LateLanguageStartAddresses();
							_htIecAddresses.Add(ioProvider, lateLanguageStartAddresses);
						}
						lateLanguageStartAddresses.startInAddress = (item.IoMapping as IoMapping).GetIecAddress(_htIecAddresses);
						HashIecAddresses[ioProvider] = lateLanguageStartAddresses;
					}
					if (!flag3 && ((int)item.ChannelType == 2 || (int)item.ChannelType == 3))
					{
						flag3 = true;
						LateLanguageStartAddresses lateLanguageStartAddresses2;
						if (_htIecAddresses.ContainsKey(ioProvider))
						{
							lateLanguageStartAddresses2 = _htIecAddresses[ioProvider] as LateLanguageStartAddresses;
						}
						else
						{
							lateLanguageStartAddresses2 = new LateLanguageStartAddresses();
							_htIecAddresses.Add(ioProvider, lateLanguageStartAddresses2);
						}
						lateLanguageStartAddresses2.startOutAddress = (item.IoMapping as IoMapping).GetIecAddress(_htIecAddresses);
						HashIecAddresses[ioProvider] = lateLanguageStartAddresses2;
					}
					if (flag2 && flag3)
					{
						break;
					}
				}
			}
			IIoProvider[] children = ioProvider.Children;
			for (int i = 0; i < children.Length; i++)
			{
				FillIecAddresstable(children[i]);
			}
		}

		internal static bool HasPlcLogicObject(int nProjectHandle, Guid objectGuid)
		{
			Guid GuidPlcLogic;
			return HasPlcLogicObject(nProjectHandle, objectGuid, out GuidPlcLogic);
		}

		internal static bool HasPlcLogicObject(int nProjectHandle, Guid objectGuid, out Guid GuidPlcLogic)
		{
			Guid[] subObjectGuids = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid).SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					GuidPlcLogic = guid;
					return true;
				}
			}
			GuidPlcLogic = Guid.Empty;
			return false;
		}

		internal static IDeviceObject GetPlcDevice(int nProjectHandle, Guid objectGuid)
		{
			Guid guid = Guid.Empty;
			if (HasPlcLogicObject(nProjectHandle, objectGuid))
			{
				guid = objectGuid;
			}
			else
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid);
				while (metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					guid = metaObjectStub.ParentObjectGuid;
					if (HasPlcLogicObject(nProjectHandle, metaObjectStub.ParentObjectGuid))
					{
						break;
					}
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
				}
			}
			if (Guid.Empty != guid)
			{
				IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid).Object;
				return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
			}
			return null;
		}

		internal static Guid GetIoApplication(int nProjectHandle, Guid guidDeviceObject)
		{
			Guid result = Guid.Empty;
			if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, guidDeviceObject))
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidDeviceObject);
				IObject @object = objectToRead.Object;
				IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				if (objectToRead.ParentObjectGuid != Guid.Empty && val != null && val.Attributes["IoApplicationProvider"] != null)
				{
					Guid guid = Guid.Empty;
					if (val.Attributes["IoApplicationProvider"].ToUpperInvariant() == "PARENT")
					{
						guid = objectToRead.ParentObjectGuid;
					}
					else if (val.Attributes["IoApplicationProvider"].ToUpperInvariant() == "TOPLEVEL")
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectToRead.ParentObjectGuid);
						while (metaObjectStub.ParentObjectGuid != Guid.Empty)
						{
							metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
						}
						guid = metaObjectStub.ObjectGuid;
					}
					if (guid != Guid.Empty)
					{
						objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
					}
				}
				DeviceObject deviceObject = null;
				IMetaObject val2 = null;
				if (objectToRead.Object is ExplicitConnector)
				{
					val2 = (objectToRead.Object as ExplicitConnector).GetApplication();
				}
				else
				{
					deviceObject = ((!(objectToRead.Object is SlotDeviceObject)) ? ((DeviceObject)(object)objectToRead.Object) : ((SlotDeviceObject)(object)objectToRead.Object).GetDevice());
					if (deviceObject != null)
					{
						val2 = deviceObject.GetApplication();
					}
				}
				if (val2 != null)
				{
					result = val2.ObjectGuid;
				}
			}
			return result;
		}

		internal static IDeviceObject GetParentDeviceObject(IDeviceObject device)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Invalid comparison between Unknown and I4
			IDeviceObject result = null;
			if (device != null)
			{
				foreach (IConnector item in (IEnumerable)device.Connectors)
				{
					IConnector val = item;
					if ((int)val.ConnectorRole == 1 && CheckChildConnectorUsed(val))
					{
						IIoProvider parent = ((IIoProvider)((val is IIoProvider) ? val : null)).Parent;
						IConnector val2 = (IConnector)(object)((parent is IConnector) ? parent : null);
						if (val2 != null)
						{
							return val2.GetDeviceObject();
						}
					}
				}
				return result;
			}
			return result;
		}

		internal static IConnector GetParentConnector(IConnector connector)
		{
			IIoModuleIterator val = ((IConnector5)((connector is IConnector9) ? connector : null)).CreateModuleIterator();
			try
			{
				if (val.MoveToParent() && val.Current.IsConnector)
				{
					IIoModuleEditorHelper val2 = val.Current.CreateEditorHelper();
					try
					{
						return val2.GetConnector(false);
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return null;
		}

		internal static bool CheckChildConnectorUsed(IConnector con)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (IAdapter item in (IEnumerable)con.Adapters)
			{
				IAdapter val = item;
				if (val is ISlotAdapter && val.Modules.Length != 0 && val.Modules[0] != Guid.Empty)
				{
					return true;
				}
			}
			return false;
		}

		internal static void CheckNameIdentifier(ILanguageModelBuilder3 lmBuilder3, ISequenceStatement seq, DataElementBase elem)
		{
			IToken val = default(IToken);
			if (elem == null || ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(elem.Identifier, false, false, false, false).Match((TokenType)13, true, ref val) > 0)
			{
				return;
			}
			string text = string.Format(Strings.ErrorInvalidIdenfier, elem.Identifier);
			if (elem.Parent != null && elem.Parent.IoProvider != null)
			{
				IMetaObject metaObject = elem.Parent.IoProvider.GetMetaObject();
				if (metaObject.Object is IDeviceObject5)
				{
					IObject @object = metaObject.Object;
					IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((@object is IDeviceObject5) ? @object : null)).DeviceIdentificationNoSimulation;
					if (deviceIdentificationNoSimulation != null)
					{
						text = text + " Type: " + deviceIdentificationNoSimulation.Type + " ID: " + deviceIdentificationNoSimulation.Id + " Version: " + deviceIdentificationNoSimulation.Version;
					}
				}
			}
			seq.AddStatement(lmBuilder3.CreatePragmaStatement2((IExprementPosition)null, string.Format("error '{0}'", text.Replace("'", "$'"))));
		}

		internal static void CollectObjectGuids(LDictionary<Guid, string> dictObjects, int nProjectHandle, Guid[] subGuids, Type objectType, bool bRecursive)
		{
			foreach (Guid guid in subGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (objectType.IsAssignableFrom(metaObjectStub.ObjectType))
				{
					dictObjects[guid]= metaObjectStub.Name;
				}
				if (bRecursive)
				{
					CollectObjectGuids(dictObjects, nProjectHandle, metaObjectStub.SubObjectGuids, objectType, bRecursive);
				}
			}
		}
	}
}
