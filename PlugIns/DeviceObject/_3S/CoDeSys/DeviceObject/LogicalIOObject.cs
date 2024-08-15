using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject.LogicalDevice;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{EF290667-72A3-4c4d-82F3-6CC077B4978D}")]
	[StorageVersion("3.4.1.0")]
	internal class LogicalIOObject : GenericObject2, ILogicalObject2, ILogicalObject, IObject, IGenericObject, IArchivable, ICloneable, IComparable, ILanguageModelProvider3, ILanguageModelProvider2, ILanguageModelProvider, IHasLocalizedDisplayName
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("DisableSynchronization")]
		[StorageVersion("3.5.16.0")]
		[StorageDefaultValue(false)]
		private bool _bDisableSynchronization;

		private IMetaObject _metaObject;

		private LocalUniqueIdGenerator _idGenerator = new LocalUniqueIdGenerator();

		internal static readonly Guid GUID_LOGCICALNAMESPACE = new Guid("{9FAAE977-68F9-4261-A4C3-61942F8CAF4E}");

		private static string GVL_IOCONFIGREMOTE_GLOBALS = "IoConfigRemote_Globals";

		public bool DisableSynchronization
		{
			get
			{
				return _bDisableSynchronization;
			}
			set
			{
				_bDisableSynchronization = value;
			}
		}

		public static Guid TypeGuid => ((TypeGuidAttribute)typeof(LogicalIOObject).GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;

		public bool CanRename => false;

		public IEmbeddedObject[] EmbeddedObjects => null;

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
			}
		}

		public Guid Namespace => GUID_LOGCICALNAMESPACE;

		public IUniqueIdGenerator UniqueIdGenerator => (IUniqueIdGenerator)(object)_idGenerator;

		public bool NeedsContextForLanguageModelProvision => false;

		public string LocalizedDisplayName => LogicalIOStrings.LogicalIOObjectName;

		public bool AcceptsChildObject(Type childObjectType)
		{
			if (childObjectType == null)
			{
				return false;
			}
			if (!typeof(LogicalIOObject).IsAssignableFrom(childObjectType))
			{
				return typeof(LogicalIODevice).IsAssignableFrom(childObjectType);
			}
			return true;
		}

		public bool AcceptsParentObject(IObject parentObject)
		{
			if (parentObject == null)
			{
				return false;
			}
			Guid[] subObjectGuids = parentObject.MetaObject.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(parentObject.MetaObject.ProjectHandle, guid);
				if (typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return false;
				}
			}
			if (!(parentObject is IApplicationObject))
			{
				return parentObject is ILogicalObject;
			}
			return true;
		}

		public bool CheckName(string stName)
		{
			return true;
		}

		public int CheckRelationships(IObject parentObject, IObject[] childObjects)
		{
			return 0;
		}

		public string GetContentString(ref long nPosition, ref int nLength, bool bWord)
		{
			return string.Empty;
		}

		public string GetPositionText(long nPosition)
		{
			return string.Empty;
		}

		public void HandleRenamed()
		{
		}

		public string GetLanguageModel2(out List<List<string>> codeTables)
		{
			codeTables = new List<List<string>>();
			return GetLanguageModelDevice(codeTables);
		}

		public string GetLanguageModel()
		{
			return GetLanguageModelDevice(null);
		}

		private string GetLanguageModelDevice(List<List<string>> codeTables)
		{
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Expected O, but got Unknown
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Expected O, but got Unknown
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Expected O, but got Unknown
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (_metaObject == null || primaryProject == null || primaryProject.Handle != _metaObject.ProjectHandle)
			{
				return string.Empty;
			}
			if (!DeviceObjectHelper.BeginGetLanguageModel(_metaObject.ObjectGuid))
			{
				return string.Empty;
			}
			try
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement("language-model-list");
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ParentObjectGuid);
				if (objectToRead != null)
				{
					if (LogicalIONotification.LogicalMappingApps != null)
					{
						Guid[] array = new Guid[LogicalIONotification.LogicalMappingApps.DeviceGuids.Count];
						LogicalIONotification.LogicalMappingApps.DeviceGuids.CopyTo(array, 0);
						Guid[] array2 = array;
						foreach (Guid guid in array2)
						{
							if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, guid))
							{
								continue;
							}
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, guid);
							if (metaObjectStub == null)
							{
								continue;
							}
							IObjectProperty[] properties = metaObjectStub.Properties;
							foreach (IObjectProperty val in properties)
							{
								if (!(val is ILogicalApplicationProperty) || !((val as ILogicalApplicationProperty).LogicalApplication == objectToRead.ObjectGuid))
								{
									continue;
								}
								Guid[] subObjectGuids = metaObjectStub.SubObjectGuids;
								foreach (Guid guid2 in subObjectGuids)
								{
									IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, guid2);
									if (typeof(ITaskConfigObject).IsAssignableFrom(metaObjectStub2.ObjectType))
									{
										IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, metaObjectStub2.ObjectGuid);
										if (objectToRead2 != null && objectToRead2.Object is ITaskConfigObject)
										{
											ITaskConfigObject val2 = (ITaskConfigObject)objectToRead2.Object;
											((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)val2, true);
										}
									}
								}
							}
						}
					}
					xmlTextWriter.WriteStartElement("language-model");
					xmlTextWriter.WriteAttributeString("application-id", objectToRead.ObjectGuid.ToString());
					xmlTextWriter.WriteAttributeString("object-id", _metaObject.ObjectGuid.ToString());
					Hashtable htStartAddresses = new Hashtable();
					bool bMotorolaBitfield = false;
					bool bAdditionalParams = false;
					bool bSkipAdditionalParamsForZeroParams = false;
					bool flag = false;
					IDeviceObject val3 = null;
					IMetaObject objectToRead3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, objectToRead.ParentObjectGuid);
					if (objectToRead3 != null)
					{
						IMetaObject objectToRead4 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, objectToRead3.ParentObjectGuid);
						if (objectToRead4 != null && objectToRead4.Object is DeviceObject)
						{
							IObject @object = objectToRead4.Object;
							val3 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
							flag = (objectToRead4.Object as DeviceObject).SkipInsertLibrary;
						}
					}
					IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
					if (hostStub != null)
					{
						IMetaObject objectToRead5 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(hostStub.ProjectHandle, hostStub.ObjectGuid);
						if (objectToRead5 != null)
						{
							IObject object2 = objectToRead5.Object;
							IDeviceObject5 val4 = (IDeviceObject5)(object)((object2 is IDeviceObject5) ? object2 : null);
							if (val4 != null)
							{
								bAdditionalParams = DeviceObjectHelper.EnableAdditionalParameters((IDeviceObject)(object)val4);
								bSkipAdditionalParamsForZeroParams = DeviceObjectHelper.SkipAdditionalParametersForEmptyConnectors((IDeviceObject)(object)val4);
								bMotorolaBitfield = DeviceObjectHelper.MotorolaBitfields((IDeviceObject)(object)val4);
							}
						}
					}
					int nNumMappingsModules = 0;
					ConnectorMapList connectorMapList = new ConnectorMapList();
					LanguageModelContainer languageModelContainer = new LanguageModelContainer();
					LDictionary<IRequiredLib, IIoProvider> val5 = new LDictionary<IRequiredLib, IIoProvider>();
					if (_metaObject.Object is LogicalIOObject && val3 != null)
					{
						foreach (IRequiredLib item2 in (IEnumerable)((IDeviceObject2)((val3 is IDeviceObject2) ? val3 : null)).DriverInfo.RequiredLibs)
						{
							IRequiredLib val6 = item2;
							if (!(val6 as RequiredLib).IsDiagnosisLib)
							{
								val5.Add(val6, (IIoProvider)(object)((val3 is IIoProvider) ? val3 : null));
							}
						}
					}
					CollectLanguageModel(_metaObject.ProjectHandle, _metaObject.SubObjectGuids, connectorMapList, languageModelContainer, objectToRead, ref nNumMappingsModules, htStartAddresses, bMotorolaBitfield, bAdditionalParams, val5, bSkipAdditionalParamsForZeroParams);
					if (!flag)
					{
						int num = default(int);
						bool bLoadFinished = ((IObjectManager)APEnvironment.ObjectMgr).IsLoadProjectFinished(_metaObject.ProjectHandle, out num);
						LanguageModelHelper.AddLibraries(((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(objectToRead.ProjectHandle, objectToRead.ParentObjectGuid).ParentObjectGuid, val5, objectToRead, languageModelContainer, bIsLogical: true, bLoadFinished);
					}
					string stErrorPragmas;
					string stAttributes;
					List<string> mappedVariableDeclarations = connectorMapList.GetMappedVariableDeclarations(objectToRead.Name, out stAttributes, out stErrorPragmas, bCreateForLogicalIos: true);
					string item = $"{{nobp}}{{messageguid '{objectToRead.ObjectGuid.ToString()}'}}\n";
					stAttributes = "{attribute 'global_init_slot' := '60100'}" + stAttributes;
					List<string> list = new List<string>();
					list.AddRange(languageModelContainer.sbValues.StringList);
					list.Add(languageModelContainer.sbMessages.ToString());
					list.AddRange(mappedVariableDeclarations);
					list.Add(item);
					Guid guid3 = Guid.Empty;
					IPreCompileContext2 val7 = (IPreCompileContext2)((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetPrecompileContext(objectToRead.ObjectGuid);
					if (val7 != null)
					{
						ISignature[] array3 = ((IPreCompileContext)val7).FindSignature(GVL_IOCONFIGREMOTE_GLOBALS);
						if (array3.Length != 0)
						{
							guid3 = array3[0].ObjectGuid;
						}
					}
					if (guid3 == Guid.Empty)
					{
						guid3 = LanguageModelHelper.CreateDeterministicGuid(objectToRead.ObjectGuid, GVL_IOCONFIGREMOTE_GLOBALS);
					}
					LanguageModelHelper.AddGlobalVarListWithRetains(list, languageModelContainer.sbRetains.ToString(), guid3, GVL_IOCONFIGREMOTE_GLOBALS, xmlTextWriter, _metaObject.ObjectGuid, bAllowOnlineChange: false, stAttributes, codeTables);
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Close();
				return stringWriter.ToString();
			}
			finally
			{
				if (_metaObject != null && _metaObject.ParentObjectGuid != Guid.Empty)
				{
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
				}
				DeviceObjectHelper.EndGetLanguageModel(MetaObject.ObjectGuid);
			}
		}

		internal void CollectLanguageModel(int nProjectHandle, Guid[] objectGuids, ConnectorMapList cml, LanguageModelContainer lmcontainer, IMetaObject metaApp, ref int nNumMappingsModules, Hashtable htStartAddresses, bool bMotorolaBitfield, bool bAdditionalParams, LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs, bool bSkipAdditionalParamsForZeroParams)
		{
			foreach (Guid guid in objectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
					if (objectToRead.Object is IIoProvider && objectToRead.Object is DeviceObject)
					{
						int nNumModules = 0;
						DeviceObject deviceObject = objectToRead.Object as DeviceObject;
						foreach (Connector item in (IEnumerable)deviceObject.Connectors)
						{
							item.UpdateAddresses();
						}
						bool bHasManualAddress = false;
						IObject @object = objectToRead.Object;
						deviceObject.CollectMappings((IIoProvider)(object)((@object is IIoProvider) ? @object : null), cml, ref nNumMappingsModules, bPlcAlwaysMapping: false, (AlwaysMappingMode)0, htStartAddresses, bMotorolaBitfield, null, ref bHasManualAddress);
						IObject object2 = objectToRead.Object;
						LanguageModelHelper.AddModuleIoLanguageModel((IIoProvider)(object)((object2 is IIoProvider) ? object2 : null), null, "0", lmcontainer, metaApp, ref nNumModules, bAdditionalParams, bIsLogical: true, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
					}
				}
				CollectLanguageModel(nProjectHandle, metaObjectStub.SubObjectGuids, cml, lmcontainer, metaApp, ref nNumMappingsModules, htStartAddresses, bMotorolaBitfield, bAdditionalParams, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
			}
		}

		public LogicalIOObject()
			: base()
		{
		}
	}
}
