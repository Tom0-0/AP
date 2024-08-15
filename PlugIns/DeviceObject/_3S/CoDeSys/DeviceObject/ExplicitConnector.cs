#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{e9159722-55bc-49e5-8034-fbd278ef718f}")]
	[StorageVersion("3.3.0.0")]
	public class ExplicitConnector : ConnectorBase, IExplicitConnector, IObject, IGenericObject, IArchivable, ICloneable, IComparable, IConnector, IOrderedSubObjects, ILanguageModelProvider3, ILanguageModelProvider2, ILanguageModelProvider, IStructuredLanguageModelProvider, ILanguageModelProviderBuildPropertiesControl, IKnowMyOrderedSubObjectsInAdvance, IHasLocalizedDisplayName
	{
		private IMetaObject _metaObject;

		private bool _bPastePrepared;

		public override bool IsExplicit => true;

		public override bool Disabled
		{
			get
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Expected O, but got Unknown
				bool result = false;
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
				{
					IDeviceObject13 val = (IDeviceObject13)InternalGetDeviceObject();
					if (val != null)
					{
						result = val.InheritedDisable;
					}
				}
				return result;
			}
		}

		public override bool Excluded => false;

		public IMetaObject MetaObject
		{
			get
			{
				return _metaObject;
			}
			set
			{
				if (_metaObject != null && value == null)
				{
					_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
				}
				else
				{
					_metaObject = value;
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)0) && _metaObject != null)
				{
					ParameterSet hostParameterSet = _hostParameterSet;
					IDeviceObject obj = InternalGetDeviceObject();
					hostParameterSet.Device = (IDeviceObject2)(object)((obj is IDeviceObject2) ? obj : null);
				}
			}
		}

		public Guid Namespace => Guid.Empty;

		public bool CanRename => false;

		public IEmbeddedObject[] EmbeddedObjects => null;

		public IUniqueIdGenerator UniqueIdGenerator => null;

		public bool NeedsContextForLanguageModelProvision => false;

		public bool ShowPropertiesDialog => false;

		public bool ExcludeFromBuildEnabled => false;

		public bool ExternalEnabled => false;

		public bool EnableSystemCallEnabled => false;

		public bool LinkAlwaysEnabled => false;

		public bool CompilerDefinesEnabled => false;

		public string LocalizedDisplayName => base.VisibleInterfaceName;

		public ExplicitConnector()
		{
		}

		public ExplicitConnector(IConnector connector)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			Connector connector2 = connector as Connector;
			_usModuleType = (ushort)connector.ModuleType;
			_stInterface = connector.Interface;
			_nConnectorId = connector.ConnectorId;
			_nHostpath = connector.HostPath;
			_role = connector.ConnectorRole;
			_adapters = connector.Adapters as AdapterList;
			_hostParameterSet = connector.HostParameterSet as ParameterSet;
			if (connector2 != null)
			{
				_stVisibleInterfaceName = connector2.VisibleInterfaceNameAsStringRef;
				_driverInfo = (DriverInfo)((GenericObject)(DriverInfo)(object)connector2.DriverInfo).Clone();
				_guidBusCycleTask = connector2.BusCycleTaskGuid;
				_customItems = (CustomItemList)((GenericObject)(CustomItemList)(object)connector2.CustomItems).Clone();
				_nModuleId = connector2.ModuleId;
				_stAllowedPages = connector2.AllowedPages;
				_bAllwaysMapping = connector2.AlwaysMapping;
				_stIoUpdateTask = connector2.IoUpdateTask;
				_stClient = connector2.Client;
				_nMaxOutputSize = connector2.MaxOutputSize;
				_nMaxInputSize = connector2.MaxInputSize;
				_nMaxInOutputSize = connector2.MaxInOutputSize;
				_nClientConnectorId = connector2.ClientConnectorId;
				_stClientConnectorInterface = connector2.ClientConnectorInterface;
				_guidClientTypeGuid = connector2.ClientTypeGuid;
				_alAdditionalInterfaces = connector2.AdditionalInterfaces;
				_alConstraints = connector2.Contraints;
				_bHideInStatusPage = connector2.HideInStatusPage;
				_bUpdateAllowed = connector2.UpdateAllowed;
				_stFixedInputAddress = connector2.FixedInputAddress;
				_stFixedOutputAddress = connector2.FixedOutputAddress;
				_bDownloadParamsDevDescOrder = connector2.DownloadParamsDevDescOrder;
				base.AllowOnlyDevices = connector2.AllowOnlyDevices;
				_bAllwaysMappingDisabled = connector2.AlwaysMappingDisabled;
				_uiInitialStatusFlag = connector2.InitialStatusFlag;
				_bUseBlobInitConst = connector2.UseBlobInitConst;
				_alwaysMappingMode = connector2.AlwaysMappingMode;
				_uiConnectorGroup = connector2.ConnectorGroup;
			}
		}

		private ExplicitConnector(ExplicitConnector original)
			: base(original)
		{
		}

		public override object Clone()
		{
			ExplicitConnector explicitConnector = new ExplicitConnector(this);
			((GenericObject)explicitConnector).AfterClone();
			if (_metaObject != null)
			{
				ParameterSet hostParameterSet = explicitConnector._hostParameterSet;
				IDeviceObject obj = InternalGetDeviceObject();
				hostParameterSet.Device = (IDeviceObject2)(object)((obj is IDeviceObject2) ? obj : null);
			}
			return explicitConnector;
		}

		public void UpdateConnector(ExplicitConnector newConnector, LList<Guid> droppedDevices, bool bVersionUpgrade)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Expected O, but got Unknown
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Expected O, but got Unknown
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Expected O, but got Unknown
			_usModuleType = (ushort)newConnector.ModuleType;
			_stInterface = newConnector.Interface;
			_nConnectorId = newConnector.ConnectorId;
			_nHostpath = newConnector.HostPath;
			_role = newConnector.ConnectorRole;
			LList<object> val = new LList<object>();
			LList<object> val2 = new LList<object>();
			LList<object> val3 = new LList<object>();
			GetDevices(_adapters, val, val2, val3);
			foreach (IAdapterBase item in (IEnumerable)newConnector.Adapters)
			{
				if (item is FixedAdapter)
				{
					item.UpdateModules(val, (IConnector7)(object)this);
					if (val2.Count > 0)
					{
						item.UpdateModules(val2, (IConnector7)(object)this);
					}
				}
				else if (item is SlotAdapter)
				{
					item.UpdateModules(val2, (IConnector7)(object)this);
					if (val.Count > 0)
					{
						item.UpdateModules(val, (IConnector7)(object)this);
					}
				}
				else if (item is VarAdapter)
				{
					(item as VarAdapter).ClearModules();
					item.UpdateModules(val3, (IConnector7)(object)this);
				}
			}
			_adapters = newConnector.Adapters as AdapterList;
			foreach (IDeviceObject item2 in val)
			{
				IDeviceObject val4 = item2;
				droppedDevices.Add(((IObject)val4).MetaObject.ObjectGuid);
			}
			foreach (IDeviceObject item3 in val2)
			{
				IDeviceObject val5 = item3;
				droppedDevices.Add(((IObject)val5).MetaObject.ObjectGuid);
			}
			foreach (IDeviceObject item4 in val3)
			{
				IDeviceObject val6 = item4;
				droppedDevices.Add(((IObject)val6).MetaObject.ObjectGuid);
			}
			ParameterSet hostParameterSet = _hostParameterSet;
			IDeviceObject deviceObject = GetDeviceObject();
			hostParameterSet.Device = (IDeviceObject2)(object)((deviceObject is IDeviceObject2) ? deviceObject : null);
			if (base.ModuleType < 32768 || bVersionUpgrade)
			{
				_hostParameterSet.UpdateParameters((IParameterSet)(object)newConnector._hostParameterSet, bWithAddParameter: true);
			}
			else
			{
				_hostParameterSet = (ParameterSet)((GenericObject)newConnector._hostParameterSet).Clone();
				ParameterSet hostParameterSet2 = _hostParameterSet;
				IDeviceObject deviceObject2 = GetDeviceObject();
				hostParameterSet2.Device = (IDeviceObject2)(object)((deviceObject2 is IDeviceObject2) ? deviceObject2 : null);
			}
			_driverInfo = (DriverInfo)((GenericObject)(DriverInfo)(object)newConnector.DriverInfo).Clone();
			_stVisibleInterfaceName = newConnector._stVisibleInterfaceName;
			_alAdditionalInterfaces = newConnector._alAdditionalInterfaces;
			_alConstraints = newConnector._alConstraints;
			_customItems = newConnector._customItems;
			_stAllowedPages = newConnector._stAllowedPages;
			_alAllowOnlyDevices = (ArrayList)newConnector._alAllowOnlyDevices.Clone();
		}

		public override IDeviceObject GetDeviceObject()
		{
			IDeviceObject val = InternalGetDeviceObject();
			if (val == null)
			{
				throw new InvalidOperationException("Not a child of a device object");
			}
			return val;
		}

		protected IDeviceObject InternalGetDeviceObject()
		{
			if (_metaObject == null)
			{
				return null;
			}
			if (_metaObject.ParentObjectGuid == Guid.Empty)
			{
				return null;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ParentObjectGuid);
			if (objectToRead == null)
			{
				return null;
			}
			IObject @object = objectToRead.Object;
			return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
		}

		internal override void PreparePaste()
		{
			if (!_bPastePrepared)
			{
				base.PreparePaste();
				_bPastePrepared = true;
			}
		}

		public void OnAfterAdded()
		{
			if (_metaObject != null)
			{
				UpdateAddresses();
				LList<Guid> val = new LList<Guid>();
				val.Add(_hostParameterSet.LanguageModelGuid);
				if (val.IndexOf(Guid.Empty) != -1 || DeviceObjectHelper.CheckLanguageModelGuids(val))
				{
					_hostParameterSet.UpdateLanguageModelGuids(bUpgrade: false);
				}
			}
			_bPastePrepared = false;
		}

		public string[] GetPossibleInterfacesForInsert(int nInsertPosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			if ((int)_role == 1)
			{
				return null;
			}
			foreach (IAdapter item in (IEnumerable)base.Adapters)
			{
				IAdapter val = item;
				if (val is VarAdapter)
				{
					VarAdapter varAdapter = (VarAdapter)(object)val;
					if (varAdapter.ModulesCount < varAdapter.MaxDevices)
					{
						if (_alAdditionalInterfaces.Count > 0)
						{
							string[] array = new string[_alAdditionalInterfaces.Count + 1];
							int num = 0;
							array[num] = base.Interface;
							foreach (string alAdditionalInterface in _alAdditionalInterfaces)
							{
								num++;
								array[num] = alAdditionalInterface;
							}
							return array;
						}
						return new string[1] { base.Interface };
					}
				}
				nInsertPosition -= val.ModulesCount;
				if (nInsertPosition < 0)
				{
					break;
				}
			}
			return null;
		}

		public override IIoProvider GetIoProviderParent()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Invalid comparison between Unknown and I4
			IDeviceObject val = InternalGetDeviceObject();
			if (val != null)
			{
				if (base.HostPath == -1)
				{
					return (IIoProvider)(object)((val is IIoProvider) ? val : null);
				}
				if (val is DeviceObject)
				{
					Connector connectorById = (val as DeviceObject).GetConnectorById(base.HostPath);
					Debug.Assert((int)((IConnector)connectorById).ConnectorRole == 1);
					return (IIoProvider)(object)((connectorById is IIoProvider) ? connectorById : null);
				}
				if (val is SlotDeviceObject)
				{
					foreach (Connector item in (IEnumerable)val.Connectors)
					{
						if (item.ConnectorId == base.HostPath)
						{
							return (IIoProvider)(object)item;
						}
					}
				}
			}
			return null;
		}

		public override LList<IIoProvider> GetIoProviderChildren()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			LList<IIoProvider> val = new LList<IIoProvider>();
			foreach (IAdapter item in (IEnumerable)base.Adapters)
			{
				Guid[] modules = item.Modules;
				foreach (Guid guid in modules)
				{
					if (!(guid != Guid.Empty) || !((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(MetaObject.ProjectHandle, guid))
					{
						continue;
					}
					IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(MetaObject.ProjectHandle, guid).Object;
					IDeviceObject val2 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
					if (val2 != null)
					{
						IConnector val3 = FindConnectedChildConnector(val2);
						if (val3 != null)
						{
							val.Add((IIoProvider)(object)((val3 is IIoProvider) ? val3 : null));
						}
					}
				}
			}
			return val;
		}

		public override bool IoProviderEquals(IIoProvider provider)
		{
			ExplicitConnector explicitConnector = provider as ExplicitConnector;
			if (explicitConnector == null)
			{
				return false;
			}
			if (MetaObject == null)
			{
				return false;
			}
			if (explicitConnector.MetaObject == null)
			{
				return false;
			}
			if (explicitConnector.MetaObject.ObjectGuid.Equals(MetaObject.ObjectGuid))
			{
				return explicitConnector.MetaObject.ProjectHandle == MetaObject.ProjectHandle;
			}
			return false;
		}

		public override string GetIoBaseName()
		{
			return "Io_" + MetaObject.ObjectGuid.ToString().Replace('-', '_');
		}

		public override IMetaObject GetMetaObject()
		{
			return MetaObject;
		}

		public override IIoModuleIterator CreateModuleIterator()
		{
			IMetaObject metaObject = GetMetaObject();
			return (IIoModuleIterator)(object)new IoModuleIterator(new IoModuleExplicitParentConnectorReference(metaObject.ObjectGuid, metaObject.ProjectHandle, base.ConnectorId));
		}

		public bool CheckName(string stName)
		{
			return true;
		}

		public void HandleRenamed()
		{
		}

		public string GetPositionText(long nPosition)
		{
			return "";
		}

		public string GetContentString(ref long nPosition, ref int nLength, bool bWord)
		{
			return "";
		}

		public bool AcceptsParentObject(IObject parentObject)
		{
			return true;
		}

		public bool AcceptsChildObject(Type childObjectType)
		{
			return true;
		}

		public int CheckRelationships(IObject parentObject, IObject[] childObjects)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Expected O, but got Unknown
			if (!AcceptsParentObject(parentObject))
			{
				return -1;
			}
			Hashtable hashtable = new Hashtable();
			foreach (IObject val in childObjects)
			{
				hashtable.Add(val.MetaObject.ObjectGuid, val);
			}
			int num = 0;
			foreach (IAdapter item in (IEnumerable)base.Adapters)
			{
				IAdapter val2 = item;
				if (val2 is FixedAdapter || val2 is SlotAdapter)
				{
					Guid[] modules = val2.Modules;
					foreach (Guid guid in modules)
					{
						num++;
						if (guid != Guid.Empty)
						{
							if (!hashtable.ContainsKey(guid) || !(hashtable[guid] is IDeviceObject))
							{
								return num;
							}
							hashtable.Remove(guid);
						}
					}
				}
				else if (val2 is VarAdapter)
				{
					Guid[] modules = val2.Modules;
					foreach (Guid guid2 in modules)
					{
						num++;
						hashtable.Remove(guid2);
					}
				}
			}
			foreach (IObject value in hashtable.Values)
			{
				IObject val3 = value;
				if (val3 is IDeviceObject || val3 is IExplicitConnector || val3 is SlotDeviceObject)
				{
					return num;
				}
			}
			return 0;
		}

		protected IConnector FindConnectedChildConnector(IDeviceObject child)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Invalid comparison between Unknown and I4
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (MetaObject == null)
			{
				return null;
			}
			Guid objectGuid = MetaObject.ObjectGuid;
			foreach (IConnector item in (IEnumerable)child.Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole != 1)
				{
					continue;
				}
				foreach (IAdapter item2 in (IEnumerable)val.Adapters)
				{
					if (Array.IndexOf(item2.Modules, objectGuid) >= 0)
					{
						return val;
					}
				}
			}
			return null;
		}

		bool IOrderedSubObjects.AcceptsChildObject(Type childObjectType, int nIndex)
		{
			return true;
		}

		public void AddChild(Guid subObjectGuid, int nIndex)
		{
			if ((DeviceManager.DoNotSkipEvents || (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && _metaObject.ProjectHandle == ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle)) && !SetExpectedModule(subObjectGuid))
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, subObjectGuid);
				InsertChild(nIndex, objectToRead.Object, objectToRead.ObjectGuid);
			}
		}

		public override bool InsertChild(int nIndex, IObject childDevice, Guid newChildGuid)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			if (childDevice.MetaObject == null && IsExpectedModule(newChildGuid))
			{
				return true;
			}
			if (nIndex < 0)
			{
				nIndex = SubObjectsCount;
			}
			IIoModuleIterator val = CreateModuleIterator();
			if (val.MoveToParent() && val.Current.IsConnector)
			{
				IIoModuleEditorHelper obj = val.Current.CreateEditorHelper();
				(obj.GetConnector(false) as Connector)?.CheckConstraints(childDevice);
				((IDisposable)obj).Dispose();
			}
			if (!base.InsertChild(nIndex, childDevice, newChildGuid))
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)6, Strings.Error_Device_Insert);
			}
			return true;
		}

		public int GetChildIndex(Guid subObjectGuid)
		{
			int nNumSubmoduls;
			return GetChildIndex(subObjectGuid, out nNumSubmoduls);
		}

		public void RemoveChild(IMetaObject moRemoved)
		{
			((ConnectorBase)this).RemoveChild(moRemoved.ObjectGuid);
		}

		public string GetLanguageModel()
		{
			return GetLanguageModelDevice(null, null, null);
		}

		public string GetLanguageModel2(out List<List<string>> codeTables)
		{
			codeTables = null;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)2, (ushort)0))
			{
				codeTables = new List<List<string>>();
			}
			return GetLanguageModelDevice(null, null, codeTables);
		}

		public ILanguageModel GetStructuredLanguageModel(ILanguageModelBuilder lmbuilder)
		{
			List<List<string>> codeTables = null;
			ILanguageModel val = lmbuilder.CreateLanguageModel(Guid.Empty, Guid.Empty, Guid.Empty, string.Empty);
			string text;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)))
			{
				codeTables = new List<List<string>>();
				text = GetLanguageModelDevice((ILanguageModelBuilder3)(object)((lmbuilder is ILanguageModelBuilder3) ? lmbuilder : null), val, codeTables);
			}
			else
			{
				text = GetLanguageModel2(out codeTables);
			}
			ILanguageModel val2 = lmbuilder.CreateLanguageModelOfXml(text, codeTables);
			if (val2 != null)
			{
				ILMPOU[] pous = val.Pous;
				foreach (ILMPOU val3 in pous)
				{
					val2.AddPou(val3);
				}
				ILMGlobVarlist[] globalVariableLists = val.GlobalVariableLists;
				foreach (ILMGlobVarlist val4 in globalVariableLists)
				{
					val2.AddGlobalVariableList(val4);
				}
				ILMDataType[] dataTypes = val.DataTypes;
				foreach (ILMDataType val5 in dataTypes)
				{
					val2.AddDataType(val5);
				}
			}
			return val2;
		}

		private string GetLanguageModelDevice(ILanguageModelBuilder3 lmb, ILanguageModel lmnew, List<List<string>> codeTables)
		{
			IProject projectByHandle = ((IEngine)APEnvironment.Engine).Projects.GetProjectByHandle(_metaObject.ProjectHandle);
			if (projectByHandle == null)
			{
				return string.Empty;
			}
			if (projectByHandle.Library)
			{
				return string.Empty;
			}
			try
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement("language-model");
				bool flag = false;
				bool flag2 = false;
				DeviceObject deviceObject = GetDeviceObject() as DeviceObject;
				Guid appGuid = Guid.Empty;
				if (deviceObject != null)
				{
					IDeviceObject hostDeviceObject = deviceObject.GetHostDeviceObject();
					if (hostDeviceObject != null)
					{
						if (hostDeviceObject is DeviceObject)
						{
							IPlcLogicObject plcLogicObject = (hostDeviceObject as DeviceObject).GetPlcLogicObject();
							if (plcLogicObject != null)
							{
								IApplicationObject val = (hostDeviceObject as DeviceObject).GetDeviceApplicationObject(plcLogicObject);
								if (val == null)
								{
									val = (hostDeviceObject as DeviceObject).GetApplicationObject(plcLogicObject);
								}
								if (val != null)
								{
									if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0))
									{
										xmlTextWriter.WriteAttributeString("application-id", ((IObject)val).MetaObject.ObjectGuid.ToString());
										xmlTextWriter.WriteAttributeString("plclogic-id", ((IObject)plcLogicObject).MetaObject.ObjectGuid.ToString());
										xmlTextWriter.WriteAttributeString("object-id", _metaObject.ObjectGuid.ToString());
									}
									appGuid = ((IObject)val).MetaObject.ObjectGuid;
								}
							}
						}
						flag = DeviceObjectHelper.EnableAdditionalParameters(hostDeviceObject);
						flag2 = DeviceObjectHelper.SkipAdditionalParametersForEmptyConnectors(hostDeviceObject);
					}
				}
				bool bCreateAdditionalParams = flag;
				if (flag2 && _hostParameterSet.Count == 0)
				{
					bCreateAdditionalParams = false;
				}
				LanguageModelHelper.GetLanguageModelForParameterSet(lmb, lmnew, _hostParameterSet, GetParamsListName(), xmlTextWriter, MetaObject.ObjectGuid, string.Empty, codeTables, bCreateAdditionalParams, _bDownloadParamsDevDescOrder, appGuid, bUpdateIoInStop: false);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Close();
				return stringWriter.ToString();
			}
			finally
			{
				int num = default(int);
				if (((IObjectManager)APEnvironment.ObjectMgr).IsLoadProjectFinished(_metaObject.ProjectHandle, out num))
				{
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
				}
			}
		}

		public int GetEnvisionedIndexOf(int nProjectHandle, Guid objectGuid)
		{
			return GetChildIndex(objectGuid);
		}
	}
}
