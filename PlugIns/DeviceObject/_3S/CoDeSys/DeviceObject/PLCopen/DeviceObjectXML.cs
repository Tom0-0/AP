using System;
using System.Collections;
using System.Collections.Generic;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class DeviceObjectXML
	{
		private Converter c = new Converter();

		private DeviceUtil util;

		private Device deviceDescType;

		private DeviceObject device;

		private string _typesNamespace;

		internal DeviceObjectXML(DeviceObject deviceObject)
		{
			device = deviceObject;
			util = new DeviceUtil(((IEngine)APEnvironment.Engine).Projects.PrimaryProject);
		}

		public void Init(Device deviceDescType)
		{
			this.deviceDescType = deviceDescType;
			DeviceType deviceType2 = (deviceDescType.DeviceType = new DeviceType());
			_typesNamespace = GetTypesNamespace();
			deviceDescType.Types = CreateTypes();
			deviceType2.Item = CreateIdentification();
			deviceType2.Connector = CreateConnectors();
			DeviceParameterListType deviceParameterListType2 = (deviceDescType.DeviceType.DeviceParameterSet = new DeviceParameterListType());
			DeviceParameterListType deviceParameterListType3 = deviceParameterListType2;
			IParameterSet parameterSet = device.ParameterSet;
			deviceParameterListType3.Items = CreateParameterSet((IParameterSet4)(object)((parameterSet is IParameterSet4) ? parameterSet : null));
			deviceType2.ConnectorGroup = null;
			deviceType2.DeviceParameterSet = deviceParameterListType3;
			deviceType2.Disable = device.Disable;
			deviceType2.Exclude = device.Exclude;
			deviceType2.ExcludeFromBuild = APEnvironment.LanguageModelMgr.IsExcludedFromBuild(device.MetaObject.ProjectHandle, device.MetaObject.ObjectGuid);
		}

		private DeviceIdentificationType CreateIdentification()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			DeviceIdentificationType deviceIdentificationType = null;
			deviceIdentificationType = ((!(device.DeviceIdentification is IModuleIdentification)) ? new DeviceIdentificationType() : new ModuleIdentificationType
			{
				ModuleId = ((IModuleIdentification)device.DeviceIdentification).ModuleId
			});
			deviceIdentificationType.Id = device.DeviceIdentification.Id;
			deviceIdentificationType.Type = device.DeviceIdentification.Type;
			deviceIdentificationType.Version = device.DeviceIdentification.Version;
			return deviceIdentificationType;
		}

		private string GetTypesNamespace()
		{
			foreach (KeyValuePair<string, ITypeDefinition> keyValuePair in this.device.Types.TypeMap)
			{
				string key = keyValuePair.Key;
				if (key.Contains(":"))
				{
					return key.Split(new char[]
					{
						':'
					}, StringSplitOptions.None)[0];
				}
			}
			return string.Empty;
		}

		private DeviceTypes CreateTypes()
		{
			DeviceTypes deviceTypes = new DeviceTypes();
			List<TypedefType> list = new List<TypedefType>();
			List<ItemsChoiceType> list2 = new List<ItemsChoiceType>();
			foreach (KeyValuePair<string, ITypeDefinition> keyValuePair in this.device.Types.TypeMap)
			{
				if (keyValuePair.Value.CreatedType)
				{
					string key = keyValuePair.Key;
					if (key.Contains(":"))
					{
						this._typesNamespace = key.Split(new char[]
						{
							':'
						}, StringSplitOptions.None)[0];
						if (!string.IsNullOrEmpty(this._typesNamespace) && string.IsNullOrEmpty(deviceTypes.@namespace))
						{
							deviceTypes.@namespace = this._typesNamespace;
						}
					}
					TypeDefinition typeDefinition = keyValuePair.Value as TypeDefinition;
					if (typeDefinition is StructType)
					{
						StructType structType = typeDefinition as StructType;
						StructdefType structdefType = new StructdefType();
						structdefType.iecType = structType.IecType;
						structdefType.iecTypeLib = structType.IecTypeLib;
						structdefType.name = structType.Name;
						List<StructComponentType> list3 = this.CreateStructComponents(structType.Components);
						structdefType.Component = list3.ToArray();
						list.Add(structdefType);
						list2.Add(ItemsChoiceType.StructType);
					}
					else if (typeDefinition is RangeType)
					{
						RangeType rangeType = typeDefinition as RangeType;
						list.Add(new RangedefType
						{
							basetype = rangeType.BaseType,
							Default = rangeType.Default,
							Max = rangeType.Max,
							Min = rangeType.Min,
							name = rangeType.Name
						});
						list2.Add(ItemsChoiceType.RangeType);
					}
					else if (typeDefinition is BitfieldType)
					{
						BitfieldType bitfieldType = typeDefinition as BitfieldType;
						BitfielddefType bitfielddefType = new BitfielddefType();
						bitfielddefType.name = bitfieldType.Name;
						bitfielddefType.basetype = "std:" + bitfieldType.BaseType;
						List<StructComponentType> list4 = this.CreateStructComponents(bitfieldType.Components);
						bitfielddefType.Component = list4.ToArray();
						list.Add(bitfielddefType);
						list2.Add(ItemsChoiceType.BitfieldType);
					}
					else if (typeDefinition is EnumType)
					{
						EnumType enumType = typeDefinition as EnumType;
						list.Add(new EnumdefType
						{
							basetype = enumType.BaseType,
							name = enumType.Name,
							Enum = this.CreateEnumComponents(enumType.Values).ToArray()
						});
						list2.Add(ItemsChoiceType.EnumType);
					}
					else if (typeDefinition is ArrayType)
					{
						ArrayType arrayType = typeDefinition as ArrayType;
						list.Add(new ArraydefType
						{
							FirstDimension = TypeConv.CreateDimensionType(arrayType.FirstDimension),
							SecondDimension = TypeConv.CreateDimensionType(arrayType.SecondDimension),
							ThirdDimension = TypeConv.CreateDimensionType(arrayType.ThirdDimension),
							name = arrayType.Name,
							basetype = arrayType.BaseType
						});
						list2.Add(ItemsChoiceType.ArrayType);
					}
					else if (typeDefinition is UnionType)
					{
						UnionType unionType = typeDefinition as UnionType;
						StructdefType structdefType2 = new StructdefType();
						structdefType2.iecType = unionType.IecType;
						structdefType2.iecTypeLib = unionType.IecTypeLib;
						structdefType2.name = unionType.Name;
						List<StructComponentType> list5 = this.CreateStructComponents(unionType.Components);
						structdefType2.Component = list5.ToArray();
						list.Add(structdefType2);
						list2.Add(ItemsChoiceType.UnionType);
					}
				}
			}
			if (list.Count > 0)
			{
				deviceTypes.Items = list.ToArray();
				deviceTypes.ItemsElementName = list2.ToArray();
				return deviceTypes;
			}
			return null;
		}

		private List<StructComponentType> CreateStructComponents(IList compList)
		{
			List<StructComponentType> list = new List<StructComponentType>();
			foreach (StructComponent comp in compList)
			{
				StructComponentType structComponentType = new StructComponentType();
				if (!string.IsNullOrEmpty(comp.DefaultValue))
				{
					structComponentType.Default = new ValueType();
					structComponentType.Default.Text = new string[1];
					structComponentType.Default.Text[0] = comp.DefaultValue;
				}
				structComponentType.Description = c.CreateString((IStringRef)(object)comp.Description);
				structComponentType.identifier = comp.Identifier;
				structComponentType.type = comp.Type;
				structComponentType.Unit = c.CreateString((IStringRef)(object)comp.Unit);
				structComponentType.VisibleName = c.CreateString((IStringRef)(object)comp.VisibleName);
				list.Add(structComponentType);
			}
			return list;
		}

		private List<EnumdefTypeEnum> CreateEnumComponents(IList valueList)
		{
			List<EnumdefTypeEnum> list = new List<EnumdefTypeEnum>();
			foreach (EnumTypeValue value in valueList)
			{
				EnumdefTypeEnum enumdefTypeEnum = new EnumdefTypeEnum();
				enumdefTypeEnum.Description = c.CreateString((IStringRef)(object)value.Description);
				enumdefTypeEnum.identifier = value.Identifier;
				enumdefTypeEnum.VisibleName = c.CreateString((IStringRef)(object)value.Name);
				enumdefTypeEnum.Value = new ValueType();
				enumdefTypeEnum.Value.Text = new string[1] { value.Value };
				list.Add(enumdefTypeEnum);
			}
			return list;
		}

		private DeviceTypeConnector[] CreateConnectors()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			LList<DeviceTypeConnector> val = new LList<DeviceTypeConnector>();
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val2 = item;
				DeviceTypeConnector deviceTypeConnector = CreateConnector((IConnector7)(object)((val2 is IConnector7) ? val2 : null));
				val.Add(deviceTypeConnector);
			}
			return val.ToArray();
		}

		private DeviceTypeConnector CreateConnector(IConnector7 con)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			DeviceTypeConnector deviceTypeConnector = new DeviceTypeConnector();
			deviceTypeConnector.connectorId = ((IConnector)con).ConnectorId;
			IParameterSet hostParameterSet = ((IConnector)con).HostParameterSet;
			deviceTypeConnector.HostParameterSet = CreateParameterSet((IParameterSet4)(object)((hostParameterSet is IParameterSet4) ? hostParameterSet : null));
			deviceTypeConnector.hostpath = ((IConnector)con).HostPath;
			deviceTypeConnector.@interface = ((IConnector)con).Interface;
			deviceTypeConnector.moduleType = ((IConnector)con).ModuleType;
			deviceTypeConnector.role = EnumConv.CreateConnectorRoleType(((IConnector)con).ConnectorRole);
			LList<CustomNode> val = new LList<CustomNode>();
			foreach (ICustomItem item in (IEnumerable)((IConnector)con).CustomItems)
			{
				CustomNode customNode = new CustomNode(item.Data);
				val.Add(customNode);
			}
			if (val.Count > 0)
			{
				CustomType customType = new CustomType();
				customType.Any = val.ToArray();
				deviceTypeConnector.Custom = customType;
			}
			return deviceTypeConnector;
		}

		public TOutput[] ConvertAll<TInput, TOutput>(ICollection list, Converter<TInput, TOutput> converter)
		{
			LList<TOutput> val = new LList<TOutput>();
			foreach (TInput item in list)
			{
				TOutput val2 = converter(item);
				val.Add(val2);
			}
			return val.ToArray();
		}

		private object[] CreateParameterSet(IParameterSet4 pset)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			LList<object> val = new LList<object>();
			LDictionary<IParameterSection, ParameterSectionType> sectionMapping = new LDictionary<IParameterSection, ParameterSectionType>();
			IParameterSection2[] topLevelSections = ((IParameterSet2)pset).GetTopLevelSections();
			for (int i = 0; i < topLevelSections.Length; i++)
			{
				IParameterSection val2 = (IParameterSection)(object)topLevelSections[i];
				ParameterSectionType parameterSectionType = CreateSection((IParameterSection3)(object)((val2 is IParameterSection3) ? val2 : null), ref sectionMapping);
				val.Add((object)parameterSectionType);
			}
			foreach (IParameter item in (IEnumerable)pset)
			{
				IParameter val3 = item;
				ParameterType parameterType = CreateParameter((IParameter6)(object)((val3 is IParameter6) ? val3 : null));
				if (val3.Section != null)
				{
					ParameterSectionType parameterSectionType2 = sectionMapping[val3.Section];
					LList<object> val4 = new LList<object>((IEnumerable<object>)parameterSectionType2.Items);
					val4.Add((object)parameterType);
					parameterSectionType2.Items = val4.ToArray();
				}
				else
				{
					val.Add((object)parameterType);
				}
			}
			return val.ToArray();
		}

		private ParameterSectionType CreateSection(IParameterSection3 section, ref LDictionary<IParameterSection, ParameterSectionType> sectionMapping)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			ParameterSectionType parameterSectionType = new ParameterSectionType();
			parameterSectionType.Description = ((IParameterSection)section).Description;
			parameterSectionType.Name = ((IParameterSection)section).Name;
			sectionMapping.Add((IParameterSection)(object)section, parameterSectionType);
			LList<CustomNode> val = new LList<CustomNode>();
			foreach (ICustomItem item in (IEnumerable)((IParameterSection2)section).CustomItems)
			{
				CustomNode customNode = new CustomNode(item.Data);
				val.Add(customNode);
			}
			if (val.Count > 0)
			{
				CustomType customType = new CustomType();
				customType.Any = val.ToArray();
				parameterSectionType.Custom = customType;
			}
			LList<ParameterSectionType> val2 = new LList<ParameterSectionType>();
			IParameterSection2[] subSections = ((IParameterSection2)section).SubSections;
			for (int i = 0; i < subSections.Length; i++)
			{
				IParameterSection val3 = (IParameterSection)(object)subSections[i];
				ParameterSectionType parameterSectionType2 = CreateSection((IParameterSection3)(object)((val3 is IParameterSection3) ? val3 : null), ref sectionMapping);
				val2.Add(parameterSectionType2);
			}
			parameterSectionType.Items = val2.ToArray();
			return parameterSectionType;
		}

		private ParameterType CreateParameter(IParameter6 ip)
		{
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			ParameterType parameterType = new ParameterType();
			Parameter parameter = ip as Parameter;
			parameterType.Attributes = TypeConv.CreateAttributes(ip as Parameter);
			if (!string.IsNullOrEmpty(parameter.Description))
			{
				parameterType.Description = parameter.Description;
			}
			parameterType.Name = parameter.VisibleName;
			parameterType.ParameterId = (uint)parameter.Id;
			if (parameter.IndexInDevDesc != -1)
			{
				parameterType.IndexInDevDesc = parameter.IndexInDevDesc;
				parameterType.IndexInDevDescSpecified = true;
			}
			if (!parameter.IoMapping.AutomaticIecAddress)
			{
				parameterType.FixedAddress = parameter.IoMapping.IecAddress;
			}
			if (!string.IsNullOrEmpty(parameter.ParamType))
			{
				parameterType.type = parameter.ParamType;
			}
			else if (parameter.DataElementBase is DataElementSimpleType)
			{
				parameterType.type = "std:" + parameter.DataElementBase.BaseType;
			}
			else if (parameter.DataElementBase is DataElementStructType)
			{
				DataElementStructType dataElementStructType = parameter.DataElementBase as DataElementStructType;
				parameterType.type = _typesNamespace + ":" + dataElementStructType.TypeName;
			}
			else if (parameter.DataElementBase is DataElementUnionType)
			{
				DataElementUnionType dataElementUnionType = parameter.DataElementBase as DataElementUnionType;
				parameterType.type = _typesNamespace + ":" + dataElementUnionType.TypeName;
			}
			else if (parameter.DataElementBase is DataElementRangeType)
			{
				DataElementRangeType range = parameter.DataElementBase as DataElementRangeType;
				RangeType rangeType = new TypeFinder().FindRangeType(device, range);
				parameterType.type = _typesNamespace + ":" + rangeType.Name;
			}
			else if (parameter.DataElementBase is DataElementBitFieldType)
			{
				DataElementBitFieldType bitfield = parameter.DataElementBase as DataElementBitFieldType;
				BitfieldType bitfieldType = new TypeFinder().FindBitType(device, bitfield);
				if (bitfieldType == null && (int)parameter.ChannelType != 0)
				{
					parameterType.type = "std:" + parameter.DataElementBase.BaseType;
				}
				else
				{
					parameterType.type = _typesNamespace + ":" + bitfieldType.Name;
				}
			}
			else if (parameter.DataElementBase is DataElementEnumType)
			{
				DataElementEnumType en = parameter.DataElementBase as DataElementEnumType;
				EnumType enumType = new TypeFinder().FindEnumType(device, en);
				parameterType.type = _typesNamespace + ":" + enumType.Name;
			}
			else if (parameter.DataElementBase is DataElementArrayType)
			{
				DataElementArrayType array = parameter.DataElementBase as DataElementArrayType;
				ArrayType arrayType = new TypeFinder().FindArrayType(device, array);
				parameterType.type = _typesNamespace + ":" + arrayType.Name;
			}
			parameterType.Unit = c.CreateString(parameter.Unit);
			ValueType valueType = null;
			if (parameter.DataElementBase is DataElementArrayType)
			{
				parameterType.Value = CreateArrayValueTypes((IDataElement)(object)parameter);
			}
			else
			{
				valueType = CreateValueType((IDataElement)(object)parameter);
				parameterType.Value = new ValueType[1] { valueType };
			}
			ValueType valueType2 = CreateMappingType((IDataElement)(object)parameter);
			parameterType.Mapping = new ValueType[1] { valueType2 };
			LList<CustomNode> val = new LList<CustomNode>();
			foreach (ICustomItem item in (IEnumerable)parameter.CustomItems)
			{
				CustomNode customNode = new CustomNode(item.Data);
				val.Add(customNode);
			}
			if (val.Count > 0)
			{
				CustomType customType = new CustomType();
				customType.Any = val.ToArray();
				parameterType.Custom = customType;
			}
			return parameterType;
		}

		private ValueType[] CreateArrayValueTypes(IDataElement dataElement)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			LList<ValueType> val = new LList<ValueType>();
			foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
			{
				ValueType valueType = CreateValueType(item);
				val.Add(valueType);
			}
			return val.ToArray();
		}

		private static ValueType CreateValueType(IDataElement dataElement)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			ValueType valueType = new ValueType();
			valueType.name = dataElement.Identifier;
			if (!string.IsNullOrEmpty(dataElement.VisibleName))
			{
				valueType.visiblename = dataElement.VisibleName;
			}
			if (!string.IsNullOrEmpty(dataElement.Description))
			{
				valueType.desc = dataElement.Description;
			}
			IDataElement6 val = (IDataElement6)(object)((dataElement is IDataElement6) ? dataElement : null);
			if (val != null)
			{
				valueType.onlineaccess = EnumConv.CreateAccessRightType(val.GetAccessRight(true));
				valueType.offlineaccess = EnumConv.CreateAccessRightType(val.GetAccessRight(false));
			}
			if (dataElement.HasSubElements)
			{
				LList<ValueTypeElement> val2 = new LList<ValueTypeElement>();
				foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
				{
					ValueTypeElement valueTypeElement = CreateValueTypeElement(item);
					val2.Add(valueTypeElement);
				}
				valueType.Element = val2.ToArray();
			}
			else
			{
				valueType.Text = new string[1] { dataElement.Value };
			}
			LList<CustomNode> val3 = new LList<CustomNode>();
			foreach (ICustomItem item2 in (IEnumerable)dataElement.CustomItems)
			{
				CustomNode customNode = new CustomNode(item2.Data);
				val3.Add(customNode);
			}
			if (val3.Count > 0)
			{
				CustomType customType = new CustomType();
				customType.Any = val3.ToArray();
				valueType.Custom = customType;
			}
			return valueType;
		}

		private static ValueTypeElement CreateValueTypeElement(IDataElement el)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Expected O, but got Unknown
			ValueTypeElement valueTypeElement = new ValueTypeElement();
			valueTypeElement.name = el.Identifier;
			if (!string.IsNullOrEmpty(el.VisibleName))
			{
				valueTypeElement.visiblename = el.VisibleName;
			}
			if (!string.IsNullOrEmpty(el.Description))
			{
				valueTypeElement.desc = el.Description;
			}
			IDataElement6 val = (IDataElement6)(object)((el is IDataElement6) ? el : null);
			if (val != null)
			{
				valueTypeElement.onlineaccess = EnumConv.CreateAccessRightType(val.GetAccessRight(true));
				valueTypeElement.offlineaccess = EnumConv.CreateAccessRightType(val.GetAccessRight(false));
			}
			LList<CustomNode> val2 = new LList<CustomNode>();
			foreach (ICustomItem item in (IEnumerable)el.CustomItems)
			{
				CustomNode customNode = new CustomNode(item.Data);
				val2.Add(customNode);
			}
			if (val2.Count > 0)
			{
				CustomType customType = new CustomType();
				customType.Any = val2.ToArray();
				valueTypeElement.Custom = customType;
			}
			if (el.HasSubElements)
			{
				LList<ValueTypeElement> val3 = new LList<ValueTypeElement>();
				foreach (IDataElement item2 in (IEnumerable)el.SubElements)
				{
					ValueTypeElement valueTypeElement2 = CreateValueTypeElement(item2);
					val3.Add(valueTypeElement2);
				}
				valueTypeElement.Element = val3.ToArray();
			}
			else
			{
				valueTypeElement.Text = new string[1] { el.Value };
			}
			return valueTypeElement;
		}

		private static ValueType CreateMappingType(IDataElement dataElement)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			ValueType valueType = new ValueType();
			if (dataElement.HasSubElements)
			{
				LList<ValueTypeElement> val = new LList<ValueTypeElement>();
				foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
				{
					ValueTypeElement valueTypeElement = CreateMappingTypeElement(item);
					if (valueTypeElement != null)
					{
						val.Add(valueTypeElement);
					}
				}
				if (val.Count > 0)
				{
					valueType.Element = val.ToArray();
				}
			}
			foreach (IVariableMapping item2 in (IEnumerable)dataElement.IoMapping.VariableMappings)
			{
				IVariableMapping val2 = item2;
				valueType.Text = new string[1] { val2.Variable };
			}
			if (valueType.Element != null || valueType.Text != null)
			{
				return valueType;
			}
			return null;
		}

		private static ValueTypeElement CreateMappingTypeElement(IDataElement el)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			ValueTypeElement valueTypeElement = new ValueTypeElement();
			valueTypeElement.name = el.Identifier;
			if (el.HasSubElements)
			{
				LList<ValueTypeElement> val = new LList<ValueTypeElement>();
				foreach (IDataElement item in (IEnumerable)el.SubElements)
				{
					ValueTypeElement valueTypeElement2 = CreateMappingTypeElement(item);
					if (valueTypeElement2 != null)
					{
						val.Add(valueTypeElement2);
					}
				}
				if (val.Count > 0)
				{
					valueTypeElement.Element = val.ToArray();
				}
			}
			foreach (IVariableMapping item2 in (IEnumerable)el.IoMapping.VariableMappings)
			{
				IVariableMapping val2 = item2;
				valueTypeElement.Text = new string[1] { val2.Variable };
			}
			if (valueTypeElement.Element != null || valueTypeElement.Text != null)
			{
				return valueTypeElement;
			}
			return null;
		}
	}
}
