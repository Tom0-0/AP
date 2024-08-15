#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.Utilities;
using _3S.CoDeSys.VarDeclObject;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{7BB2E02B-39DD-46f5-B184-3DF239F2F856}")]
	[StorageVersion("3.4.2.0")]
	internal class LogicalExchangeGVLObject : GenericObject2, ILogicalGVLObject2, ILogicalGVLObject, IObject, IGenericObject, IArchivable, ICloneable, IComparable, ILogicalDevice, IFindReplace2, IFindReplace, ILanguageModelProvider, ILanguageModelProviderWithDependencies
	{
		public const int GVLOBJECTTYPE = 152;

		private IMetaObject _metaObject;

		private LocalUniqueIdGenerator _idGenerator = new LocalUniqueIdGenerator();

		internal static readonly Guid GUID_POUNAMESPACE = new Guid("{E5B60C93-5445-4e40-ADA9-CD9C005549B4}");

		private IVarDeclObject _interface;

		[DefaultSerialization("DeviceObject")]
		[StorageVersion("3.4.4.60")]
		[DefaultDuplication((DuplicationMethod)1)]
		[StorageDefaultValue(null)]
		private IDeviceObject _deviceObject;

		[DefaultSerialization("LogicalDeviceList")]
		[StorageVersion("3.4.2.0")]
		[DefaultDuplication((DuplicationMethod)1)]
		protected LogicalDeviceList _logicalDevices = new LogicalDeviceList();

		[DefaultDuplication((DuplicationMethod)1)]
		[DefaultSerialization("LogicalLanguageModelPositionId")]
		[StorageVersion("3.4.2.0")]
		protected long _lLogicalLanguageModelPositionId = -1L;

		[DefaultDuplication((DuplicationMethod)1)]
		[DefaultSerialization("UseCombinedType")]
		[StorageVersion("3.5.8.0")]
		[StorageDefaultValue(false)]
		protected bool _bUseCombinedType;

		public static Guid TypeGuid => ((TypeGuidAttribute)typeof(LogicalExchangeGVLObject).GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;

		public bool CanRename => true;

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
				foreach (LogicalMappedDevice logicalDevice in _logicalDevices)
				{
					logicalDevice.MetaObject = _metaObject;
				}
				if (_metaObject != null && _metaObject.IsToModify)
				{
					UpdateMappingDevice();
				}
				if (_metaObject != null)
				{
					if (_deviceObject != null)
					{
						((IObject)_deviceObject).MetaObject=(_metaObject);
					}
					if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, _metaObject.ObjectGuid))
					{
						APEnvironment.DeviceMgr.UpdateObject(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
					}
				}
			}
		}

		public Guid Namespace => GUID_POUNAMESPACE;

		public IUniqueIdGenerator UniqueIdGenerator => (IUniqueIdGenerator)(object)_idGenerator;

		[DefaultSerialization("Interface")]
		[StorageVersion("3.4.2.0")]
		[DefaultDuplication((DuplicationMethod)1)]
		public IVarDeclObject Interface
		{
			get
			{
				return _interface;
			}
			set
			{
				_interface = value;
				((IEmbeddedObject)_interface).OwnerObject=((IObject)(object)this);
			}
		}

		public bool IsPhysical => true;

		public bool IsLogical => false;

		public IMappedDeviceList MappedDevices => (IMappedDeviceList)(object)_logicalDevices;

		public long LanguageModelPositionId => _lLogicalLanguageModelPositionId;

		internal IDeviceObject Device => _deviceObject;

		public bool UseCombinedType
		{
			get
			{
				return _bUseCombinedType;
			}
			set
			{
				_bUseCombinedType = value;
			}
		}

		public bool CanUseCombinedType
		{
			get
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Expected O, but got Unknown
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Expected O, but got Unknown
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Expected O, but got Unknown
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Invalid comparison between Unknown and I4
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Invalid comparison between Unknown and I4
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Invalid comparison between Unknown and I4
				Guid guid = Guid.Empty;
				if (_logicalDevices.Count > 0)
				{
					foreach (IMappedDevice logicalDevice in _logicalDevices)
					{
						IMappedDevice val = logicalDevice;
						if (val.IsMapped)
						{
							guid = val.GetMappedDevice;
						}
					}
				}
				if (_metaObject != null && guid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, guid))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, guid);
					if (objectToRead != null && objectToRead.Object is IDeviceObject)
					{
						IObject @object = objectToRead.Object;
						foreach (IConnector item in (IEnumerable)((IDeviceObject)((@object is IDeviceObject) ? @object : null)).Connectors)
						{
							IConnector val2 = item;
							LList<IParameter> val3 = new LList<IParameter>();
							LList<IParameter> val4 = new LList<IParameter>();
							foreach (IParameter item2 in (IEnumerable)val2.HostParameterSet)
							{
								IParameter val5 = item2;
								if ((int)val5.ChannelType == 1)
								{
									val3.Add(val5);
								}
								if ((int)val5.ChannelType == 2 || (int)val5.ChannelType == 3)
								{
									val4.Add(val5);
								}
							}
							if (CheckCompactParams(val2, val3, out var stCompactedType) || CheckCompactParams(val2, val4, out stCompactedType))
							{
								return true;
							}
						}
					}
				}
				return false;
			}
		}

		public string GetDeclaration => Declaration(bMapped: false, bLanguageModel: false);

		public Guid[] ObjectsToUpdate
		{
			get
			{
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				ArrayList arrayList = new ArrayList();
				if (_metaObject != null)
				{
					int num = default(int);
					bool flag = ((IObjectManager)APEnvironment.ObjectMgr).IsLoadProjectFinished(_metaObject.ProjectHandle, out num);
					if (flag)
					{
						foreach (IMappedDevice item in (IEnumerable)MappedDevices)
						{
							Guid getMappedDevice = item.GetMappedDevice;
							if (!(getMappedDevice != Guid.Empty) || !((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, getMappedDevice))
							{
								continue;
							}
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, getMappedDevice);
							arrayList.Add(getMappedDevice);
							do
							{
								metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, metaObjectStub.ParentObjectGuid);
								if (typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
								{
									arrayList.Add(metaObjectStub.ObjectGuid);
								}
							}
							while (metaObjectStub.ParentObjectGuid != Guid.Empty);
						}
					}
					IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
					if (hostStub != null)
					{
						if (!flag)
						{
							DeviceObjectHelper.AddObjectsToUpdate(hostStub.ProjectHandle, hostStub.ObjectGuid);
						}
						else
						{
							arrayList.Add(hostStub.ObjectGuid);
						}
					}
				}
				Guid[] array = new Guid[arrayList.Count];
				arrayList.CopyTo(array);
				return array;
			}
		}

		public LogicalExchangeGVLObject()
			: base()
		{
			Interface = (IVarDeclObject)(object)APEnvironment.CreateVarDeclObject();
			_logicalDevices.Add(new LogicalMappedDevice());
		}

		public bool AcceptsChildObject(Type childObjectType)
		{
			return false;
		}

		public bool AcceptsParentObject(IObject parentObject)
		{
			if (parentObject == null)
			{
				return false;
			}
			if (parentObject is IDeviceApplication)
			{
				return true;
			}
			if (parentObject is IApplicationObject && ((IOnlineApplicationObject)((parentObject is IApplicationObject) ? parentObject : null)).ParentApplicationGuid == Guid.Empty)
			{
				return true;
			}
			return false;
		}

		public bool CheckName(string stName)
		{
			return DeviceObject.IsIdentifier(stName);
		}

		public int CheckRelationships(IObject parentObject, IObject[] childObjects)
		{
			return 0;
		}

		public string GetContentString(ref long nPosition, ref int nLength, bool bWord)
		{
			string result = null;
			if (_interface != null)
			{
				result = ((IEmbeddedObject)_interface).GetContentString(ref nPosition, ref nLength, bWord);
			}
			return result;
		}

		public string GetPositionText(long nPosition)
		{
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out num2);
			IVarDeclObject @interface = _interface;
			ITextVarDeclObject val = (ITextVarDeclObject)(object)((@interface is ITextVarDeclObject) ? @interface : null);
			if (val != null)
			{
				int num3 = val.TextDocument.FindLineById(num);
				if (num3 >= 0)
				{
					return string.Format(Strings.Position, num3 + 1);
				}
			}
			return null;
		}

		public void HandleRenamed()
		{
		}

		public SearchableTextBlock[] GetSearchableTextBlocks(long nPosition, int nLength)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			SearchableTextBlock[] result = null;
			if (_interface != null && _interface is IFindReplace2)
			{
				result = ((IFindReplace2)_interface).GetSearchableTextBlocks(nPosition, nLength);
			}
			else if (_interface != null && _interface is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)_interface).IsFunctionAvailable("GetSearchableTextBlocks"))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
				xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("nPosition"));
				xmlDocument.DocumentElement["nPosition"].InnerText = nPosition.ToString();
				xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("nLength"));
				xmlDocument.DocumentElement["nLength"].InnerText = nLength.ToString();
				XmlDocument xmlDocument2 = ((IGenericInterfaceExtensionProvider)_interface).CallFunction("GetSearchableTextBlocks", xmlDocument);
				List<SearchableTextBlock> list = new List<SearchableTextBlock>();
				foreach (XmlNode childNode in xmlDocument2.DocumentElement.ChildNodes)
				{
					if (!(childNode is XmlElement) || childNode.Name != "SearchableTextBlock")
					{
						continue;
					}
					SearchableTextBlock item = default(SearchableTextBlock);
					item.Text = ((((XmlElement)childNode)["Text"] != null) ? ((XmlElement)childNode)["Text"].InnerText : null);
					if (((XmlElement)childNode)["PositionIds"] != null)
					{
						List<long> list2 = new List<long>();
						foreach (XmlNode childNode2 in ((XmlElement)childNode)["PositionIds"].ChildNodes)
						{
							if (childNode2 is XmlElement && !(childNode2.Name != "PositionId") && long.TryParse(((XmlElement)childNode2).InnerText, out var result2))
							{
								list2.Add(result2);
							}
						}
						item.PositionIds = list2.ToArray();
					}
					else
					{
						item.PositionIds = null;
					}
					if (((XmlElement)childNode)["PositionOffsets"] != null)
					{
						List<int> list3 = new List<int>();
						foreach (XmlNode childNode3 in ((XmlElement)childNode)["PositionOffsets"].ChildNodes)
						{
							if (childNode3 is XmlElement && !(childNode3.Name != "PositionOffset") && int.TryParse(((XmlElement)childNode3).InnerText, out var result3))
							{
								list3.Add(result3);
							}
						}
						item.PositionOffsets = list3.ToArray();
					}
					else
					{
						item.PositionOffsets = null;
					}
					list.Add(item);
				}
				result = list.ToArray();
			}
			return result;
		}

		public SearchableTextBlock[] GetSearchableTextBlocks()
		{
			IVarDeclObject @interface = _interface;
			IFindReplace val = (IFindReplace)(object)((@interface is IFindReplace) ? @interface : null);
			if (val == null)
			{
				return null;
			}
			return val.GetSearchableTextBlocks();
		}

		public void Replace(long nPosition, int nLength, string stReplacement)
		{
		}

		public string GetLanguageModel()
		{
			try
			{
				if (_metaObject == null)
				{
					throw new InvalidOperationException("This object does not belong to a meta object.");
				}
				Guid guid = Guid.Empty;
				Guid guid2 = Guid.Empty;
				if (_metaObject.ParentObjectGuid != Guid.Empty)
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ParentObjectGuid);
					Debug.Assert(objectToRead != null);
					if (objectToRead.Object is IApplicationObject)
					{
						guid = objectToRead.ObjectGuid;
					}
					else if (objectToRead.Object is IPlcLogicObject)
					{
						guid2 = objectToRead.ObjectGuid;
					}
				}
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement("language-model");
				if (guid != Guid.Empty)
				{
					xmlTextWriter.WriteAttributeString("application-id", guid.ToString());
				}
				if (guid2 != Guid.Empty)
				{
					xmlTextWriter.WriteAttributeString("plclogic-id", guid2.ToString());
				}
				IProject projectByHandle = ((IEngine)APEnvironment.Engine).Projects.GetProjectByHandle(_metaObject.ProjectHandle);
				Debug.Assert(projectByHandle != null);
				if (projectByHandle.Library)
				{
					xmlTextWriter.WriteAttributeString("library-id", projectByHandle.Id);
				}
				xmlTextWriter.WriteStartElement("global-interface");
				xmlTextWriter.WriteAttributeString("id", XmlConvert.ToString(_metaObject.ObjectGuid));
				xmlTextWriter.WriteAttributeString("name", _metaObject.Name);
				StringBuilder stringBuilder = new StringBuilder();
				string format = "attribute '{0}' := '{1}'";
				string value = "{" + string.Format(format, CompileAttributes.ATTRIBUTE_SIGNATURE_FLAG, 1073741824) + "}\r\n";
				stringBuilder.Append(value);
				stringBuilder.Append(Declaration(bMapped: false, bLanguageModel: true));
				if (stringBuilder.Length > 0)
				{
					xmlTextWriter.WriteElementString("interface", stringBuilder.ToString());
				}
				xmlTextWriter.WriteEndElement();
				if (_bUseCombinedType && _deviceObject != null)
				{
					foreach (Connector item in (IEnumerable)_deviceObject.Connectors)
					{
						if (!item.IsExplicit)
						{
							LanguageModelHelper.GetLanguageModelForParameterSet(null, null, (ParameterSet)(object)item.HostParameterSet, item.GetParamsListName(), xmlTextWriter, _metaObject.ObjectGuid, string.Empty, null, bCreateAdditionalParams: false, item.DownloadParamsDevDescOrder, guid, bUpdateIoInStop: false);
						}
					}
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
			}
		}

		internal void CompactParams(IConnector con, LList<IParameter> liParams)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			if (liParams.Count <= 0)
			{
				return;
			}
			string stCompactedType = string.Empty;
			if (CheckCompactParams(con, liParams, out stCompactedType))
			{
				if (!_bUseCombinedType)
				{
					return;
				}
				foreach (IParameter liParam in liParams)
				{
					con.HostParameterSet.RemoveParameter(liParam.Id);
				}
				((IDataElement)con.HostParameterSet.AddParameter(liParams[0].Id, ((IDataElement)liParams[0]).VisibleName, liParams[0].GetAccessRight(true), liParams[0].GetAccessRight(false), liParams[0].ChannelType, "std:" + stCompactedType)).IoMapping.VariableMappings.AddMapping(_metaObject.Name, true);
			}
			else
			{
				_bUseCombinedType = false;
			}
		}

		internal bool CheckCompactParams(IConnector con, LList<IParameter> liParams, out string stCompactedType)
		{
			stCompactedType = string.Empty;
			if (liParams.Count > 0)
			{
				long num = 0L;
				foreach (IParameter liParam in liParams)
				{
					if (((IDataElement)liParam).GetBitSize() == 1)
					{
						num++;
						continue;
					}
					num = 0L;
					break;
				}
				if (num > 0)
				{
					switch (num)
					{
					case 8L:
						stCompactedType = "BYTE";
						break;
					case 16L:
						stCompactedType = "WORD";
						break;
					case 32L:
						stCompactedType = "DWORD";
						break;
					case 64L:
						stCompactedType = "LWORD";
						break;
					}
					if (!string.IsNullOrEmpty(stCompactedType))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void UpdateMappingDevice()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Expected O, but got Unknown
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Expected O, but got Unknown
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Invalid comparison between Unknown and I4
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Invalid comparison between Unknown and I4
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Invalid comparison between Unknown and I4
			Guid guid = Guid.Empty;
			_deviceObject = null;
			if (_logicalDevices.Count > 0)
			{
				foreach (IMappedDevice logicalDevice in _logicalDevices)
				{
					IMappedDevice val = logicalDevice;
					if (val.IsMapped)
					{
						guid = val.GetMappedDevice;
					}
				}
			}
			if (_metaObject == null || !(guid != Guid.Empty) || !((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, guid))
			{
				return;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, guid);
			if (objectToRead == null || !(objectToRead.Object is IDeviceObjectBase))
			{
				return;
			}
			IDeviceObjectBase deviceObjectBase = objectToRead.Object as IDeviceObjectBase;
			_deviceObject = (IDeviceObject)(deviceObjectBase.Clone() as IDeviceObjectBase);
			((IObject)_deviceObject).MetaObject=(objectToRead);
			(_deviceObject as IDeviceObjectBase).UpdateLanguageModelGuids(bUpgrade: false);
			foreach (IConnector item in (IEnumerable)_deviceObject.Connectors)
			{
				IConnector val2 = item;
				LList<IParameter> val3 = new LList<IParameter>();
				LList<IParameter> val4 = new LList<IParameter>();
				foreach (IParameter item2 in (IEnumerable)val2.HostParameterSet)
				{
					IParameter val5 = item2;
					if ((int)val5.ChannelType == 1)
					{
						val3.Add(val5);
					}
					if ((int)val5.ChannelType == 2 || (int)val5.ChannelType == 3)
					{
						val4.Add(val5);
					}
				}
				CompactParams(val2, val3);
				CompactParams(val2, val4);
			}
			_interface = (IVarDeclObject)(object)APEnvironment.CreateVarDeclObject();
			IVarDeclObject @interface = _interface;
			ITextVarDeclObject val6 = (ITextVarDeclObject)(object)((@interface is ITextVarDeclObject) ? @interface : null);
			if (val6 != null)
			{
				((DefaultUniqueIdGenerator)_idGenerator).Reset();
				val6.TextDocument.SetUniqueIdGenerator((IUniqueIdGenerator)(object)_idGenerator);
				val6.TextDocument.Text=(GetDeclaration);
			}
		}

		internal string Declaration(bool bMapped, bool bLanguageModel)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Expected O, but got Unknown
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("VAR_GLOBAL");
			Guid guid = Guid.Empty;
			if (_logicalDevices.Count > 0)
			{
				foreach (IMappedDevice logicalDevice in _logicalDevices)
				{
					IMappedDevice val = logicalDevice;
					if (val.IsMapped)
					{
						guid = val.GetMappedDevice;
					}
				}
			}
			if (_deviceObject != null && _metaObject != null && guid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, guid))
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, guid);
				IDeviceObject val2 = _deviceObject;
				if (bMapped)
				{
					IObject @object = objectToRead.Object;
					val2 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				}
				if (objectToRead != null && objectToRead.Object is IDeviceObject)
				{
					ChannelType val3 = (ChannelType)2;
					if (bLanguageModel && objectToRead.Object is LogicalIODevice)
					{
						IMetaObject application = (objectToRead.Object as LogicalIODevice).GetApplication();
						if (application != null && application.ParentObjectGuid != Guid.Empty)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(application.ProjectHandle, application.ParentObjectGuid);
							if (metaObjectStub != null && metaObjectStub.ParentObjectGuid != Guid.Empty)
							{
								IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
								if (objectToRead2 != null && objectToRead2.Object is DeviceObject)
								{
									val3 = (objectToRead2.Object as DeviceObject).LogicalGVLAssignmentErrorType;
								}
							}
						}
					}
					foreach (IConnector item in (IEnumerable)val2.Connectors)
					{
						int num = 1;
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)15, (ushort)0))
						{
							num = 3;
						}
						foreach (IParameter item2 in (IEnumerable)item.HostParameterSet)
						{
							IParameter val5 = item2;
							if ((int)val5.ChannelType != 0 && val5 is IDataElement2 && ((IDataElement2)((val5 is IDataElement2) ? val5 : null)).HasBaseType && ((IDataElement)val5).IoMapping != null && ((IDataElement)val5).IoMapping.VariableMappings != null && ((ICollection)((IDataElement)val5).IoMapping.VariableMappings).Count > 0 && !string.IsNullOrEmpty(((IDataElement)val5).IoMapping.VariableMappings[0]
								.Variable))
							{
								if (bLanguageModel && (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)5, (ushort)0) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0))) && val5.ChannelType == val3)
								{
									stringBuilder.AppendLine("{attribute 'read_only'}");
									stringBuilder.AppendLine("{warning disable C0228}");
									stringBuilder.AppendLine("{attribute 'const_non_replaced'}");
								}
								if (bLanguageModel && (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)20) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)12, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)0))))
								{
									stringBuilder.AppendFormat("{{p{0}}}", num++);
								}
								stringBuilder.AppendLine("//" + objectToRead.Name + " : " + ((IDataElement)val5).Description);
								string text = ((IDataElement)val5).BaseType;
								if (text.ToLowerInvariant() == "bit")
								{
									text = "BOOL";
								}
								if (bLanguageModel && (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)20) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)12, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)0))))
								{
									stringBuilder.AppendFormat("{{p{0}}}", num++);
								}
								stringBuilder.AppendFormat("{0} : {1};\r\n", ((IDataElement)val5).IoMapping.VariableMappings[0]
									.Variable
									.Trim(), text);
							}
						}
					}
				}
			}
			stringBuilder.AppendLine("END_VAR");
			return stringBuilder.ToString();
		}
	}
}
