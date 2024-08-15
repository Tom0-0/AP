using System.Collections;
using System.Text;
using System.Xml;
using _3S.CoDeSys.DeviceObject.DevDesc;

namespace _3S.CoDeSys.DeviceObject.ExportDevice
{
	internal class ExportParameterSetHelper
	{
		internal ParameterExport exportRoot = new ParameterExport();

		public object[] ExportParameterSet(ParameterSet paramset, DeviceDescription devdesc)
		{
			exportRoot = new ParameterExport();
			GenerateSections(exportRoot, ((IParameterSet2)paramset).GetTopLevelSections());
			foreach (Parameter item in paramset)
			{
				if (item.Section != null)
				{
					FindSection(exportRoot, item.Section as ParameterSection)?.liParams.Add(ConvertParameter(item, devdesc));
				}
				else
				{
					exportRoot.liParams.Add(ConvertParameter(item, devdesc));
				}
			}
			CreateObjects(exportRoot);
			return exportRoot.objects;
		}

		internal void CreateObjects(ParameterExport export)
		{
			object[] array = new object[export.liParams.Count + export.liSubs.Count];
			int num = 0;
			foreach (ParameterExport liSub in export.liSubs)
			{
				array[num++] = liSub.sectionType;
				CreateObjects(liSub);
				liSub.sectionType.Items = liSub.objects;
			}
			foreach (ParameterType liParam in export.liParams)
			{
				array[num++] = liParam;
			}
			export.objects = array;
		}

		private void GenerateSections(ParameterExport export, IParameterSection2[] sections)
		{
			if (sections == null)
			{
				return;
			}
			for (int i = 0; i < sections.Length; i++)
			{
				ParameterSection parameterSection = (ParameterSection)(object)sections[i];
				ParameterExport parameterExport = new ParameterExport();
				parameterExport.section = parameterSection;
				parameterExport.sectionType = ConvertSection(parameterSection);
				export.liSubs.Add(parameterExport);
				if (parameterSection.SubSections != null && parameterSection.SubSections.Length != 0)
				{
					GenerateSections(parameterExport, parameterSection.SubSections);
				}
			}
		}

		private ParameterExport FindSection(ParameterExport export, ParameterSection section)
		{
			if (export.section == section)
			{
				return export;
			}
			foreach (ParameterExport liSub in export.liSubs)
			{
				ParameterExport parameterExport = FindSection(liSub, section);
				if (parameterExport != null)
				{
					return parameterExport;
				}
			}
			return null;
		}

		private ParameterType ConvertParameter(Parameter param, DeviceDescription devdesc)
		{
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Invalid comparison between Unknown and I4
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Expected I4, but got Unknown
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_0729: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Expected I4, but got Unknown
			//IL_078c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Expected I4, but got Unknown
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0802: Unknown result type (might be due to invalid IL or missing references)
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Invalid comparison between Unknown and I4
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Invalid comparison between Unknown and I4
			ParameterType parameterType = new ParameterType();
			parameterType.ParameterId = (uint)param.Id;
			DataElementBase dataElementBase = param.DataElementBase;
			if (dataElementBase.GetVisibleName != null && !string.IsNullOrEmpty(dataElementBase.GetVisibleName.Default))
			{
				StringRef getVisibleName = dataElementBase.GetVisibleName;
				parameterType.Name = new StringRefType();
				parameterType.Name.Value = getVisibleName.Default;
				if (!string.IsNullOrEmpty(getVisibleName.Namespace) && !string.IsNullOrEmpty(getVisibleName.Identifier))
				{
					parameterType.Name.name = getVisibleName.Namespace + ":" + getVisibleName.Identifier;
				}
				else
				{
					parameterType.Name.name = "localTypes:NONE";
				}
			}
			if (dataElementBase.GetDescription != null && !string.IsNullOrEmpty(dataElementBase.GetDescription.Default))
			{
				StringRef getDescription = dataElementBase.GetDescription;
				parameterType.Description = new StringRefType();
				parameterType.Description.Value = getDescription.Default;
				if (!string.IsNullOrEmpty(getDescription.Namespace) && !string.IsNullOrEmpty(getDescription.Identifier))
				{
					parameterType.Description.name = getDescription.Namespace + ":" + getDescription.Identifier;
				}
				else
				{
					parameterType.Description.name = "localTypes:NONE";
				}
			}
			if (dataElementBase.GetUnit != null && !string.IsNullOrEmpty(dataElementBase.GetUnit.Default))
			{
				StringRef getUnit = dataElementBase.GetUnit;
				parameterType.Unit = new StringRefType();
				parameterType.Unit.Value = getUnit.Default;
				if (!string.IsNullOrEmpty(getUnit.Namespace) && !string.IsNullOrEmpty(getUnit.Identifier))
				{
					parameterType.Unit.name = getUnit.Namespace + ":" + getUnit.Identifier;
				}
				else
				{
					parameterType.Unit.name = "localTypes:NONE";
				}
			}
			if (param.FilterFlags != null && param.FilterFlags.Length != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string[] filterFlags = param.FilterFlags;
				foreach (string value in filterFlags)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(',');
				}
				parameterType.FilterFlags = stringBuilder.ToString();
			}
			if (!string.IsNullOrEmpty(param.ParamType))
			{
				parameterType.type = param.ParamType;
			}
			else
			{
				if (param.DataElementBase is DataElementSimpleType)
				{
					parameterType.type = "std:" + param.BaseType;
				}
				else if (devdesc.Types != null)
				{
					for (int j = 0; j < devdesc.Types.Items.Length; j++)
					{
						switch (devdesc.Types.ItemsElementName[j])
						{
						case ItemsChoiceType.BitfieldType:
						{
							if (!(param.DataElementBase is DataElementBitFieldType))
							{
								break;
							}
							BitfielddefType bitfielddefType = devdesc.Types.Items[j] as BitfielddefType;
							if (bitfielddefType.Component.Length != ((ICollection)param.SubElements).Count)
							{
								break;
							}
							bool flag3 = true;
							for (int m = 0; m < ((ICollection)param.SubElements).Count; m++)
							{
								if (param.SubElements[m].Identifier != bitfielddefType.Component[m].identifier)
								{
									flag3 = false;
								}
							}
							if (flag3)
							{
								parameterType.type = devdesc.Types.@namespace + ":" + bitfielddefType.name;
							}
							break;
						}
						case ItemsChoiceType.EnumType:
						{
							if (!(param.DataElementBase is DataElementEnumType))
							{
								break;
							}
							DataElementEnumType dataElementEnumType = param.DataElementBase as DataElementEnumType;
							EnumdefType enumdefType = devdesc.Types.Items[j] as EnumdefType;
							if (!(enumdefType.basetype.ToLowerInvariant() == "std:" + dataElementEnumType.BaseType.ToLowerInvariant()) || enumdefType.Enum.Length != dataElementEnumType.EnumerationValues.Length)
							{
								break;
							}
							bool flag2 = true;
							for (int l = 0; l < dataElementEnumType.EnumerationValues.Length; l++)
							{
								if (enumdefType.Enum[l].identifier != dataElementEnumType.EnumerationValues[l].Identifier)
								{
									flag2 = false;
								}
							}
							if (flag2)
							{
								parameterType.type = devdesc.Types.@namespace + ":" + enumdefType.name;
							}
							break;
						}
						case ItemsChoiceType.RangeType:
							if (param.DataElementBase is DataElementRangeType)
							{
								RangedefType rangedefType = devdesc.Types.Items[j] as RangedefType;
								DataElementRangeType dataElementRangeType = param.DataElementBase as DataElementRangeType;
								if (rangedefType.Max == dataElementRangeType.MaxValue && rangedefType.Min == dataElementRangeType.MinValue && rangedefType.basetype.ToLowerInvariant() == "std:" + dataElementRangeType.BaseType.ToLowerInvariant())
								{
									parameterType.type = devdesc.Types.@namespace + ":" + rangedefType.name;
								}
							}
							break;
						case ItemsChoiceType.StructType:
						case ItemsChoiceType.UnionType:
						{
							if (!(param.DataElementBase is DataElementStructType) && !(param.DataElementBase is DataElementUnionType))
							{
								break;
							}
							StructdefType structdefType = devdesc.Types.Items[j] as StructdefType;
							if (structdefType.Component.Length != ((ICollection)param.SubElements).Count)
							{
								break;
							}
							bool flag = true;
							for (int k = 0; k < ((ICollection)param.SubElements).Count; k++)
							{
								if (param.SubElements[k].Identifier != structdefType.Component[k].identifier)
								{
									flag = false;
								}
							}
							if (flag)
							{
								parameterType.type = devdesc.Types.@namespace + ":" + structdefType.name;
							}
							break;
						}
						}
					}
				}
				if (string.IsNullOrEmpty(parameterType.type))
				{
					throw new DeviceObjectException((DeviceObjectExeptionReason)15, "found no parameter type -> no file possible!");
				}
			}
			ExportDefault(param.DataElementBase, parameterType);
			if (param.CustomItems != null && ((ICollection)param.CustomItems).Count > 0)
			{
				parameterType.Custom = new CustomType();
				parameterType.Custom.Any = new XmlElement[((ICollection)param.CustomItems).Count];
				for (int n = 0; n < ((ICollection)param.CustomItems).Count; n++)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(param.CustomItems[n].Data);
					parameterType.Custom.Any[n] = xmlDocument.DocumentElement;
				}
			}
			parameterType.Attributes = new ParameterTypeAttributes();
			parameterType.Attributes.alwaysmapping = param.AlwaysMapping;
			AlwaysMappingMode alwaysMappingMode = param.AlwaysMappingMode;
			if ((int)alwaysMappingMode != 0)
			{
				if ((int)alwaysMappingMode == 1)
				{
					parameterType.Attributes.alwaysmappingMode = ParameterTypeAttributesAlwaysmappingMode.AlwaysInBusCycle;
				}
			}
			else
			{
				parameterType.Attributes.alwaysmappingMode = ParameterTypeAttributesAlwaysmappingMode.OnlyIfUnused;
			}
			AccessRight accessRight = param.GetAccessRight(bOnline: false);
			switch ((int)accessRight)
			{
			case 0:
				parameterType.Attributes.offlineaccess = ParameterTypeAttributesOfflineaccess.none;
				break;
			case 1:
				parameterType.Attributes.offlineaccess = ParameterTypeAttributesOfflineaccess.read;
				break;
			case 3:
				parameterType.Attributes.offlineaccess = ParameterTypeAttributesOfflineaccess.readwrite;
				break;
			case 2:
				parameterType.Attributes.offlineaccess = ParameterTypeAttributesOfflineaccess.write;
				break;
			}
			accessRight = param.GetAccessRight(bOnline: true);
			switch ((int)accessRight)
			{
			case 0:
				parameterType.Attributes.onlineaccess = ParameterTypeAttributesOnlineaccess.none;
				break;
			case 1:
				parameterType.Attributes.onlineaccess = ParameterTypeAttributesOnlineaccess.read;
				break;
			case 3:
				parameterType.Attributes.onlineaccess = ParameterTypeAttributesOnlineaccess.readwrite;
				break;
			case 2:
				parameterType.Attributes.onlineaccess = ParameterTypeAttributesOnlineaccess.write;
				break;
			}
			parameterType.Attributes.download = param.Download;
			ChannelType channelType = param.ChannelType;
			switch ((int)channelType - 1)
			{
			case 0:
				parameterType.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.input;
				parameterType.Attributes.channelSpecified = true;
				break;
			case 1:
			case 2:
				parameterType.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.output;
				parameterType.Attributes.channelSpecified = true;
				break;
			}
			if (param.IsFileType)
			{
				parameterType.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.file;
				parameterType.Attributes.channelSpecified = true;
			}
			DiagType diagType = param.DiagType;
			if ((int)diagType != 1)
			{
				if ((int)diagType == 2)
				{
					parameterType.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.diagAck;
					parameterType.Attributes.channelSpecified = true;
				}
			}
			else
			{
				parameterType.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.diag;
				parameterType.Attributes.channelSpecified = true;
			}
			parameterType.Attributes.createDownloadStructure = param.CreateDownloadStructure;
			parameterType.Attributes.createInHostConnector = param.CreateInHostConnector;
			parameterType.Attributes.createInChildConnector = param.CreateInChildConnector;
			parameterType.Attributes.onlineparameter = param.OnlineParameter;
			parameterType.Attributes.noManualAddress = param.NoManualAddress;
			parameterType.Attributes.preparedValueAccess = param.PreparedValueAccess;
			parameterType.Attributes.useRefactoring = param.UseRefactoring;
			parameterType.Attributes.disableMapping = param.DisableMapping;
			parameterType.Attributes.bidirectionaloutput = param.BidirectionalOutput;
			return parameterType;
		}

		private void ExportDefault(DataElementBase dataElement, ValueType[] valueElement, bool bIsArray)
		{
			int num = 0;
			foreach (DataElementBase item in (IEnumerable)dataElement.SubElements)
			{
				if (bIsArray)
				{
					valueElement[num] = new ValueType();
				}
				else
				{
					valueElement[num] = new ValueTypeElement();
					(valueElement[num] as ValueTypeElement).name = item.Identifier;
				}
				if (item.HasSubElements)
				{
					valueElement[num].Element = new ValueTypeElement[((ICollection)item.SubElements).Count];
					ExportDefault(item, valueElement[num].Element, bIsArray: false);
				}
				else if (item.HasBaseType && !string.IsNullOrEmpty(item.Value))
				{
					valueElement[num].Text = new string[1] { item.Value };
				}
				num++;
			}
		}

		private void ExportDefault(DataElementBase dataElement, ParameterType paramtype)
		{
			if (dataElement == null)
			{
				return;
			}
			if (dataElement.HasSubElements)
			{
				ValueType[] array = null;
				bool bIsArray = false;
				if (dataElement is DataElementArrayType)
				{
					paramtype.Default = new ValueType[((ICollection)dataElement.SubElements).Count];
					array = paramtype.Default;
					bIsArray = true;
				}
				else
				{
					paramtype.Default = new ValueType[1];
					paramtype.Default[0] = new ValueType();
					paramtype.Default[0].Element = new ValueTypeElement[((ICollection)dataElement.SubElements).Count];
					array = paramtype.Default[0].Element;
				}
				ExportDefault(dataElement, array, bIsArray);
			}
			else if (dataElement.HasBaseType && !string.IsNullOrEmpty(dataElement.Value))
			{
				paramtype.Default = new ValueType[1];
				paramtype.Default[0] = new ValueType();
				paramtype.Default[0].Text = new string[1] { dataElement.Value };
			}
		}

		private ParameterSectionType ConvertSection(ParameterSection section)
		{
			if (section != null)
			{
				ParameterSectionType parameterSectionType = new ParameterSectionType();
				StringRef name = section.GetName();
				if (name != null && !string.IsNullOrEmpty(name.Default))
				{
					parameterSectionType.Name = new StringRefType();
					parameterSectionType.Name.Value = name.Default;
					if (!string.IsNullOrEmpty(name.Namespace) && !string.IsNullOrEmpty(name.Identifier))
					{
						parameterSectionType.Name.name = name.Namespace + ":" + name.Identifier;
					}
					else
					{
						parameterSectionType.Name.name = "localTypes:NONE";
					}
				}
				StringRef getDescription = section.GetDescription;
				if (getDescription != null && !string.IsNullOrEmpty(getDescription.Default))
				{
					parameterSectionType.Description = new StringRefType();
					parameterSectionType.Description.Value = getDescription.Default;
					if (!string.IsNullOrEmpty(getDescription.Namespace) && !string.IsNullOrEmpty(getDescription.Identifier))
					{
						parameterSectionType.Description.name = getDescription.Namespace + ":" + getDescription.Identifier;
					}
					else
					{
						parameterSectionType.Description.name = "localTypes:NONE";
					}
				}
				return parameterSectionType;
			}
			return null;
		}
	}
}
