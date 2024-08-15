#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{4f958885-8d0a-4b58-83b6-29cfd1c9cfca}")]
	[StorageVersion("3.3.0.0")]
	public class DriverInfo : GenericObject2, IDriverInfo14, IDriverInfo13, IDriverInfo12, IDriverInfo11, IDriverInfo10, IDriverInfo9, IDriverInfo8, IDriverInfo7, IDriverInfo6, IDriverInfo5, IDriverInfo4, IDriverInfo3, IDriverInfo2, IDriverInfo, IGenericInterfaceExtensionProvider
	{
		[DefaultSerialization("NeedsBusCycle")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bNeedsBusCycle;

		[DefaultSerialization("BusCycleTask")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stBusCycleTask = "";

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected ArrayList _alRequiredLibs = new ArrayList();

		[DefaultSerialization("IoApp")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private Guid _guidIoApplication = Guid.Empty;

		[DefaultSerialization("IoAppSet")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bIoApplicationSet;

		[DefaultSerialization("PositionIds")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private long[] _positionIds = new long[0];

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected RequiredTask[] _requiredTasks;

		private IGenericInterfaceExtensionProvider _baseGenericInterfaceExtensionProvider;

		[DefaultSerialization("UseSlowestTask")]
		[StorageVersion("3.3.0.20")]
		[DefaultDuplication(0)]
		[StorageDefaultValue(false)]
		private bool _bUseSlowestTask;

		[DefaultSerialization("NeedsBusCycleBeforeRead")]
		[StorageVersion("3.3.2.30")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		private bool _bNeedsBusCycleBeforeRead;

		[DefaultSerialization("GenerateForceVariables")]
		[StorageVersion("3.4.1.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		private bool _bGenerateForceVariables;

		[DefaultSerialization("EnableDiagnosis")]
		[StorageVersion("3.5.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		private bool _bEnableDiagnosis;

		[DefaultSerialization("AdditionalParameterSetting")]
		[StorageVersion("3.5.3.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(0u)]
		private uint _AdditionalParameterSetting;

		[DefaultSerialization("ShowWarningsAsErrors")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		private bool _showWarningsAsErrors;

		[DefaultSerialization("DiagnosisCheckboxMode")]
		[StorageVersion("3.5.7.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(DiagnosisCheckboxEnum.Enabled)]
		private DiagnosisCheckboxEnum _diagnosisCheckboxMode;

		[DefaultSerialization("SkipOverlapCheck")]
		[StorageVersion("3.5.15.20")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[StorageDefaultValue(false)]
		private bool _bSkipOverlapCheck;

		private IIoProvider _owner;

		[DefaultSerialization("DeviceScanSupported")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public bool _bDeviceScanSupported;

		[DefaultSerialization("DeviceNominateSupported")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public bool _bDeviceNominateSupported;

		[DefaultSerialization("DeviceIdentifySupported")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public bool _bDeviceIdentifySupported;

		[DefaultSerialization("DeviceUploadDescriptionSupported")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public bool _bDeviceUploadDescriptionSupported;

		[DefaultSerialization("UpdateIOsInStop")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public bool _bUpdateIOsInStop;

		[DefaultSerialization("StopResetBehaviourSetting")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public StopResetBehaviour _StopResetBehaviourSetting;

		[DefaultSerialization("StopResetBehaviourUserProgram")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public string _StopResetBehaviourUserProgram = string.Empty;

		[DefaultSerialization("BusCycleTaskGuid")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public Guid _BusCycleTaskGuid = Guid.Empty;

		[DefaultSerialization("PlcAlwaysMapping")]
		[StorageVersion("3.3.0.10")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[StorageDefaultValue(false)]
		private bool _bPlcAlwaysMapping;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("PlCAlwaysMappingMode")]
		[StorageVersion("3.5.5.0")]
		[StorageDefaultValue(AlwaysMappingMode.OnlyIfUnused)]
		private AlwaysMappingMode _plcAlwaysMappingMode;

		[DefaultSerialization("RequiredLibs")]
		[StorageVersion("3.3.0.0")]
		protected ArrayList RequiredLibsSerialization
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				foreach (RequiredLib alRequiredLib in _alRequiredLibs)
				{
					if (!alRequiredLib.IsDiagnosisLib)
					{
						arrayList.Add(alRequiredLib);
					}
				}
				return arrayList;
			}
			set
			{
				_alRequiredLibs = value;
			}
		}

		[DefaultSerialization("RequiredTasks")]
		[StorageVersion("3.3.0.0")]
		protected ArrayList RequiredTasksSerialization
		{
			get
			{
				if (_requiredTasks == null)
				{
					return null;
				}
				ArrayList arrayList = new ArrayList(_requiredTasks.Length);
				RequiredTask[] requiredTasks = _requiredTasks;
				foreach (RequiredTask requiredTask in requiredTasks)
				{
					Hashtable hashtable = new Hashtable();
					requiredTask.SerializeToHashtable(hashtable);
					hashtable["TaskType"] = requiredTask.TypeOfTask;
					arrayList.Add(hashtable);
				}
				return arrayList;
			}
			set
			{
				if (value == null)
				{
					_requiredTasks = null;
					return;
				}
				_requiredTasks = new RequiredTask[value.Count];
				int num = 0;
				foreach (Hashtable item in value)
				{
					HashtableReader hashtableReader = new HashtableReader(item);
					string text = (string)hashtableReader.Read("TaskType");
					RequiredTask requiredTask;
					if (!(text == "Cyclic"))
					{
						if (text == "ExternalEvent")
						{
							requiredTask = new RequiredExternalEventTask();
							requiredTask.DeserializeFromHashtable(hashtableReader);
						}
						else
						{
							requiredTask = new UnknownRequiredTask(text);
							requiredTask.DeserializeFromHashtable(hashtableReader);
						}
					}
					else
					{
						requiredTask = new RequiredCyclicTask();
						requiredTask.DeserializeFromHashtable(hashtableReader);
					}
					_requiredTasks[num] = requiredTask;
					num++;
				}
			}
		}

		public bool NeedsBusCycle
		{
			get
			{
				return _bNeedsBusCycle;
			}
			set
			{
				_bNeedsBusCycle = value;
			}
		}

		public bool NeedsBusCycleBeforeRead
		{
			get
			{
				return _bNeedsBusCycleBeforeRead;
			}
			set
			{
				_bNeedsBusCycleBeforeRead = value;
			}
		}

		public bool SkipOverlapCheck
		{
			get
			{
				return _bSkipOverlapCheck;
			}
			set
			{
				_bSkipOverlapCheck = value;
			}
		}

		public string BusCycleTask
		{
			get
			{
				if (Guid.Empty != _BusCycleTaskGuid && _owner != null)
				{
					IMetaObject metaObject = _owner.GetMetaObject();
					if (metaObject != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaObject.ProjectHandle, _BusCycleTaskGuid))
					{
						return ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObject.ProjectHandle, _BusCycleTaskGuid).Name;
					}
				}
				if (_owner != null && !string.IsNullOrEmpty(_stBusCycleTask) && _stBusCycleTask.Contains("$(DeviceName)"))
				{
					IMetaObject metaObject2 = _owner.GetMetaObject();
					return _stBusCycleTask.Replace("$(DeviceName)", metaObject2.Name);
				}
				return _stBusCycleTask;
			}
			set
			{
				if (value == null)
				{
					_stBusCycleTask = "";
				}
				else
				{
					_stBusCycleTask = value;
				}
			}
		}

		public bool UseSlowestTask => _bUseSlowestTask;

		public Guid IoApplication
		{
			get
			{
				if (_owner != null && _owner is DeviceObject && (_owner as DeviceObject).MetaObject != null)
				{
					Guid guid = DeviceObjectHelper.ConfigModeApplication((_owner as DeviceObject).MetaObject.ObjectGuid);
					if (guid != Guid.Empty && guid != DeviceObjectHelper.ParamModeGuid)
					{
						return guid;
					}
				}
				return _guidIoApplication;
			}
			set
			{
				_guidIoApplication = value;
				_bIoApplicationSet = true;
			}
		}

		public bool CanSetIoApplication
		{
			get
			{
				if (_owner == null)
				{
					return false;
				}
				if (_owner is IDeviceObject)
				{
					return _owner.GetPlcLogic() != null;
				}
				return false;
			}
		}

		public bool IoApplicationSet => _bIoApplicationSet;

		public RequiredTask[] RequiredTasks => _requiredTasks;

		public IRequiredLibsList RequiredLibs
		{
			get
			{
				return (IRequiredLibsList)(object)new RequiredLibsList(_alRequiredLibs);
			}
			set
			{
				_alRequiredLibs = new ArrayList((RequiredLibsList)(object)value);
			}
		}

		internal ArrayList AlRequiredLibs => _alRequiredLibs;

		public bool DeviceScanSupported => _bDeviceScanSupported;

		public bool DeviceNominateSupported => _bDeviceNominateSupported;

		public bool DeviceIdentifySupported => _bDeviceIdentifySupported;

		public bool DeviceUploadDescriptionSupported => _bDeviceUploadDescriptionSupported;

		public bool UpdateIOsInStop
		{
			get
			{
				return _bUpdateIOsInStop;
			}
			set
			{
				_bUpdateIOsInStop = value;
			}
		}

		public StopResetBehaviour StopResetBehaviourSetting
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _StopResetBehaviourSetting;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_StopResetBehaviourSetting = value;
			}
		}

		public string StopResetBehaviourUserProgram
		{
			get
			{
				return _StopResetBehaviourUserProgram;
			}
			set
			{
				_StopResetBehaviourUserProgram = value;
			}
		}

		public Guid BusCycleTaskGuid
		{
			get
			{
				return _BusCycleTaskGuid;
			}
			set
			{
				_BusCycleTaskGuid = value;
			}
		}

		public bool PlcAlwaysMapping
		{
			get
			{
				if (_owner != null && _owner is DeviceObject && (_owner as DeviceObject).MetaObject != null)
				{
					Guid guid = DeviceObjectHelper.ConfigModeApplication((_owner as DeviceObject).MetaObject.ObjectGuid);
					if (guid != Guid.Empty && guid != DeviceObjectHelper.ParamModeGuid)
					{
						return true;
					}
				}
				return _bPlcAlwaysMapping;
			}
			set
			{
				_bPlcAlwaysMapping = value;
			}
		}

		public AlwaysMappingMode PlcAlwaysMappingMode
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _plcAlwaysMappingMode;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_plcAlwaysMappingMode = value;
			}
		}

		public bool CreateForceVariables
		{
			get
			{
				return _bGenerateForceVariables;
			}
			set
			{
				_bGenerateForceVariables = value;
			}
		}

		public bool EnableDiagnosis
		{
			get
			{
				return _bEnableDiagnosis;
			}
			set
			{
				_bEnableDiagnosis = value;
			}
		}

		public AdditionalParameterEnum AdditionalParameterSetting
		{
			get
			{
				return (AdditionalParameterEnum)_AdditionalParameterSetting;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected I4, but got Unknown
				_AdditionalParameterSetting = (uint)(int)value;
			}
		}

		public bool PlcCreateWarningsAsErros
		{
			get
			{
				return _showWarningsAsErrors;
			}
			set
			{
				_showWarningsAsErrors = value;
			}
		}

		public DiagnosisCheckboxEnum DiagnosisCheckboxMode
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _diagnosisCheckboxMode;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_diagnosisCheckboxMode = value;
			}
		}

		public IEnumerable<string> RequiredTaskNames
		{
			get
			{
				if (RequiredTasks != null)
				{
					RequiredTask[] requiredTasks = RequiredTasks;
					foreach (RequiredTask requiredTask in requiredTasks)
					{
						yield return requiredTask.TaskName;
					}
				}
			}
		}

		private DriverInfo(DriverInfo original)
			: this()
		{
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			_bNeedsBusCycle = original._bNeedsBusCycle;
			_bNeedsBusCycleBeforeRead = original._bNeedsBusCycleBeforeRead;
			_stBusCycleTask = original._stBusCycleTask;
			_guidIoApplication = original._guidIoApplication;
			_bIoApplicationSet = original._bIoApplicationSet;
			_positionIds = original._positionIds;
			_alRequiredLibs = new ArrayList(original._alRequiredLibs.Count);
			foreach (RequiredLib alRequiredLib in original._alRequiredLibs)
			{
				_alRequiredLibs.Add(((GenericObject)alRequiredLib).Clone());
			}
			if (original._requiredTasks != null)
			{
				_requiredTasks = new RequiredTask[original._requiredTasks.Length];
				for (int i = 0; i < original._requiredTasks.Length; i++)
				{
					_requiredTasks[i] = (RequiredTask)((GenericObject)original._requiredTasks[i]).Clone();
				}
			}
			else
			{
				_requiredTasks = null;
			}
			_bDeviceScanSupported = original._bDeviceScanSupported;
			_bDeviceNominateSupported = original._bDeviceNominateSupported;
			_bDeviceIdentifySupported = original._bDeviceIdentifySupported;
			_bDeviceUploadDescriptionSupported = original._bDeviceUploadDescriptionSupported;
			_bUpdateIOsInStop = original._bUpdateIOsInStop;
			_StopResetBehaviourSetting = original._StopResetBehaviourSetting;
			_StopResetBehaviourUserProgram = original._StopResetBehaviourUserProgram;
			_BusCycleTaskGuid = original._BusCycleTaskGuid;
			_bPlcAlwaysMapping = original._bPlcAlwaysMapping;
			_bUseSlowestTask = original._bUseSlowestTask;
			_bGenerateForceVariables = original._bGenerateForceVariables;
			_bEnableDiagnosis = original._bEnableDiagnosis;
			_AdditionalParameterSetting = original._AdditionalParameterSetting;
			_plcAlwaysMappingMode = original._plcAlwaysMappingMode;
			_showWarningsAsErrors = original._showWarningsAsErrors;
			_diagnosisCheckboxMode = original._diagnosisCheckboxMode;
			_bSkipOverlapCheck = original._bSkipOverlapCheck;
		}

		public DriverInfo()
			: base()
		{
			_baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		public DriverInfo(XmlNode node)
			: this()
		{
			Import(node, bUpdate: false);
		}

		public void Import(XmlNode node, bool bUpdate)
		{
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			Debug.Assert(node is XmlElement);
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			XmlElement xmlElement = (XmlElement)node;
			_bNeedsBusCycle = DeviceObjectHelper.ParseBool(xmlElement.GetAttribute("needsBusCycle"), bDefault: false);
			_bNeedsBusCycleBeforeRead = DeviceObjectHelper.ParseBool(xmlElement.GetAttribute("needsBusCycleBeforeReadInputs"), bDefault: false);
			_bUseSlowestTask = DeviceObjectHelper.ParseBool(xmlElement.GetAttribute("useSlowestTask"), bDefault: false);
			_bEnableDiagnosis = DeviceObjectHelper.ParseBool(xmlElement.GetAttribute("enableDiagnosis"), _bEnableDiagnosis);
			_bSkipOverlapCheck = DeviceObjectHelper.ParseBool(xmlElement.GetAttribute("skipOverlapCheck"), bDefault: false);
			string attribute = xmlElement.GetAttribute("createAdditionalParameters");
			if (!string.IsNullOrEmpty(attribute))
			{
				switch (attribute.ToLowerInvariant())
				{
				case "hidden":
					_AdditionalParameterSetting = 0u;
					break;
				case "enabled":
					_AdditionalParameterSetting = 2u;
					break;
				case "disabled":
					_AdditionalParameterSetting = 1u;
					break;
				case "enabledfixed":
					_AdditionalParameterSetting = 3u;
					break;
				}
			}
			attribute = xmlElement.GetAttribute("diagnosisCheckbox");
			if (!string.IsNullOrEmpty(attribute))
			{
				switch (attribute.ToLowerInvariant())
				{
				case "hidden":
					_diagnosisCheckboxMode = (DiagnosisCheckboxEnum)2;
					break;
				case "enabled":
					_diagnosisCheckboxMode = (DiagnosisCheckboxEnum)0;
					break;
				case "readonly":
					_diagnosisCheckboxMode = (DiagnosisCheckboxEnum)1;
					break;
				}
			}
			if (!bUpdate)
			{
				_bUpdateIOsInStop = DeviceObjectHelper.ParseBool(xmlElement.GetAttribute("UpdateIosInStop"), bDefault: false);
				string attribute2 = xmlElement.GetAttribute("StopResetBehaviour");
				if (attribute2 != null && attribute2 != string.Empty)
				{
					if (attribute2 == "KeepCurrentValues")
					{
						_StopResetBehaviourSetting = (StopResetBehaviour)0;
					}
					if (attribute2 == "SetToDefault")
					{
						_StopResetBehaviourSetting = (StopResetBehaviour)1;
					}
					if (attribute2 == "ExecuteProgram")
					{
						_StopResetBehaviourSetting = (StopResetBehaviour)2;
						_StopResetBehaviourUserProgram = xmlElement.GetAttribute("StopResetBehaviourUserProgram");
					}
				}
			}
			string attribute3 = xmlElement.GetAttribute("defaultBusCycleTask");
			if (attribute3 != null && attribute3 != string.Empty)
			{
				_stBusCycleTask = attribute3;
			}
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				switch (childNode.Name)
				{
				case "RequiredCyclicTask":
					arrayList.Add(new RequiredCyclicTask((XmlElement)childNode));
					break;
				case "RequiredExternalEventTask":
					arrayList.Add(new RequiredExternalEventTask((XmlElement)childNode));
					break;
				case "RequiredLib":
				case "RequiredLibEx":
				{
					RequiredLib requiredLib = new RequiredLib((XmlElement)childNode);
					RequiredLib requiredLib2 = null;
					if (bUpdate)
					{
						requiredLib2 = FindMatchingLib(requiredLib);
						if (requiredLib2 != null)
						{
							requiredLib2.Update((XmlElement)childNode);
							requiredLib = requiredLib2;
							requiredLib.AddedByAP = false;
						}
					}
					arrayList2.Add(requiredLib);
					break;
				}
				case "Scan":
				{
					XmlElement xmlElement2 = (XmlElement)childNode;
					_bDeviceScanSupported = DeviceObjectHelper.ParseBool(xmlElement2.GetAttribute("supported"), bDefault: false);
					_bDeviceNominateSupported = DeviceObjectHelper.ParseBool(xmlElement2.GetAttribute("nominate"), bDefault: false);
					_bDeviceIdentifySupported = DeviceObjectHelper.ParseBool(xmlElement2.GetAttribute("identify"), bDefault: false);
					_bDeviceUploadDescriptionSupported = DeviceObjectHelper.ParseBool(xmlElement2.GetAttribute("uploadDescription"), bDefault: false);
					break;
				}
				}
			}
			if (bUpdate)
			{
				foreach (RequiredLib alRequiredLib in _alRequiredLibs)
				{
					if (alRequiredLib.AddedByAP)
					{
						arrayList2.Add(alRequiredLib);
					}
				}
			}
			_alRequiredLibs = arrayList2;
			if (arrayList.Count > 0)
			{
				_requiredTasks = new RequiredTask[arrayList.Count];
				for (int i = 0; i < arrayList.Count; i++)
				{
					_requiredTasks[i] = (RequiredTask)arrayList[i];
				}
			}
		}

		public override object Clone()
		{
			DriverInfo driverInfo = new DriverInfo(this);
			((GenericObject)driverInfo).AfterClone();
			return driverInfo;
		}

		internal void SetPositionIds(LocalUniqueIdGenerator idGenerator)
		{
			_positionIds = new long[3];
			_positionIds[0] = ((DefaultUniqueIdGenerator)idGenerator).GetNext(true);
			_positionIds[1] = ((DefaultUniqueIdGenerator)idGenerator).GetNext(true);
			_positionIds[2] = ((DefaultUniqueIdGenerator)idGenerator).GetNext(true);
			foreach (RequiredLib item in (IEnumerable)RequiredLibs)
			{
				foreach (FBInstance item2 in (IEnumerable)item.FbInstances)
				{
					item2.LanguageModelPositionId = ((DefaultUniqueIdGenerator)idGenerator).GetNext(true);
				}
			}
		}

		public long GetPositionId(int nField)
		{
			if (nField < 0)
			{
				throw new ArgumentOutOfRangeException("The field identifier must not be negative");
			}
			if (_positionIds == null || _positionIds.Length <= nField)
			{
				return -1L;
			}
			return _positionIds[nField];
		}

		internal void SetIoProvider(IIoProvider ioProvider)
		{
			_owner = ioProvider;
		}

		internal void OnDeviceRenamed(IMetaObject moApp, IMetaObject moDevice, string stOldName)
		{
			if (_requiredTasks != null)
			{
				RequiredTask[] requiredTasks = _requiredTasks;
				for (int i = 0; i < requiredTasks.Length; i++)
				{
					string taskName = requiredTasks[i].TaskName;
					if (!taskName.Contains("$(DeviceName)"))
					{
						continue;
					}
					string text = taskName.Replace("$(DeviceName)", stOldName);
					string text2 = taskName.Replace("$(DeviceName)", moDevice.Name);
					Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(moDevice.ProjectHandle, text);
					foreach (Guid guid in allObjects)
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(moDevice.ProjectHandle, guid);
						if (typeof(ITaskObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(moDevice.ProjectHandle, guid);
							IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(moDevice.ProjectHandle, moDevice.ObjectGuid);
							if (hostStub != null && hostStub2 != null && hostStub2.ObjectGuid == hostStub.ObjectGuid)
							{
								((IObjectManager)APEnvironment.ObjectMgr).RenameObject(moDevice.ProjectHandle, guid, text2);
							}
						}
					}
				}
			}
			foreach (RequiredLib alRequiredLib in _alRequiredLibs)
			{
				foreach (FBInstance item in (IEnumerable)alRequiredLib.FbInstances)
				{
					item.ResetInstanceName(moDevice, moApp, RequiredLibs);
				}
			}
		}

		private RequiredLib FindMatchingLib(RequiredLib libSpec)
		{
			foreach (RequiredLib alRequiredLib in _alRequiredLibs)
			{
				if (alRequiredLib.LibName == libSpec.LibName && alRequiredLib.Vendor == libSpec.Vendor)
				{
					return alRequiredLib;
				}
			}
			return null;
		}

		public void AttachToEvent(string stEvent, GenericEventDelegate callback)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.AttachToEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		public void DetachFromEvent(string stEvent, GenericEventDelegate callback)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.DetachFromEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		public void RaiseEvent(string stEvent, XmlDocument eventData)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.RaiseEvent(stEvent, eventData);
				return;
			}
			throw new NotImplementedException();
		}

		public bool IsFunctionAvailable(string stFunction)
		{
			if (stFunction == null)
			{
				throw new ArgumentNullException("stFunction");
			}
			if (stFunction == "GetPlcAlwaysMapping" || stFunction == "SetPlcAlwaysMapping")
			{
				return true;
			}
			return false;
		}

		public XmlDocument CallFunction(string stFunction, XmlDocument functionData)
		{
			if (stFunction == null)
			{
				throw new ArgumentNullException("stFunction");
			}
			if (functionData == null)
			{
				throw new ArgumentNullException("functionData");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateElement("Output"));
			if (!(stFunction == "GetPlcAlwaysMapping"))
			{
				if (stFunction == "SetPlcAlwaysMapping")
				{
					PlcAlwaysMapping = XmlConvert.ToBoolean(functionData.DocumentElement["value"].InnerText);
				}
			}
			else
			{
				xmlDocument.DocumentElement.InnerText = XmlConvert.ToString(PlcAlwaysMapping);
			}
			return xmlDocument;
		}
	}
}
