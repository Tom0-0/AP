using System;
using System.Collections;
using System.IO;
using System.Text;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public class ImportExportCSV
	{
		private LDictionary<string, int> _dictElementsCounter = new LDictionary<string, int>();

		private LDictionary<string, IDataElement> _dictElements = new LDictionary<string, IDataElement>();

		private bool _bMultipleMappableAllowed;

		private bool _bUnionRootEditable = true;

		private bool _bBaseTypeMappable = true;

		private bool _bBitfieldMappable = true;

		public void ExportAsCSV(string stFileName, int nProjectHandle, Guid ObjectGuid)
		{
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Expected O, but got Unknown
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Invalid comparison between Unknown and I4
			bool bMotorolaBitfields = false;
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, ObjectGuid);
			IIoProvider val = null;
			if (objectToRead != null && objectToRead.Object is SlotDeviceObject)
			{
				val = (IIoProvider)(object)(objectToRead.Object as SlotDeviceObject).GetDevice();
			}
			if (objectToRead != null && objectToRead.Object is IIoProvider)
			{
				IObject @object = objectToRead.Object;
				val = (IIoProvider)(object)((@object is IIoProvider) ? @object : null);
			}
			if (val == null)
			{
				return;
			}
			IDeviceObject val2 = null;
			val2 = (IDeviceObject)((!(val is DeviceObject)) ? (val.GetHost() as IDeviceObject5) : ((object)(val as DeviceObject).GetHostDeviceObject()));
			if (val2 != null)
			{
				bMotorolaBitfields = DeviceObjectHelper.MotorolaBitfields(val2);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("//CoDeSys Mapping Export V1.2");
			stringBuilder.AppendLine();
			stringBuilder.Append("//Mapped variable;");
			stringBuilder.Append("//Parameter name @ counter in device;");
			stringBuilder.Append("//Unit;");
			stringBuilder.Append("//Description;");
			stringBuilder.Append("//IEC address;");
			stringBuilder.Append("//Device name;");
			stringBuilder.AppendLine();
			stringBuilder.Append("//Important: change only first, third or fourth column in Excel or add variable name before first ;");
			stringBuilder.AppendLine();
			if (objectToRead.ParentObjectGuid == Guid.Empty)
			{
				IObject object2 = objectToRead.Object;
				IIoProvider ioprovider = (IIoProvider)(object)((object2 is IIoProvider) ? object2 : null);
				CollectMappings(ioprovider, stringBuilder, bMotorolaBitfields);
			}
			else
			{
				if (objectToRead.Object is IDeviceObject)
				{
					IObject object3 = objectToRead.Object;
					CollectMappings((IIoProvider)(object)((object3 is IIoProvider) ? object3 : null), stringBuilder, bMotorolaBitfields);
					IObject object4 = objectToRead.Object;
					foreach (IConnector item in (IEnumerable)((IDeviceObject)((object4 is IDeviceObject) ? object4 : null)).Connectors)
					{
						IConnector val3 = item;
						if ((int)val3.ConnectorRole == 1 && DeviceObjectHelper.CheckChildConnectorUsed(val3))
						{
							CollectMappings((IIoProvider)(object)((val3 is IIoProvider) ? val3 : null), stringBuilder, bMotorolaBitfields);
						}
					}
				}
				if (objectToRead.Object is IExplicitConnector)
				{
					IObject object5 = objectToRead.Object;
					CollectMappings((IIoProvider)(object)((object5 is IIoProvider) ? object5 : null), stringBuilder, bMotorolaBitfields);
				}
			}
			try
			{
				AuthFile.WriteAllText(stFileName, stringBuilder.ToString());
			}
			catch (Exception ex)
			{
				APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
			}
		}

		private void CollectMappings(IIoProvider ioprovider, StringBuilder sb, bool bMotorolaBitfields)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			if (ioprovider == null)
			{
				return;
			}
			foreach (IParameter item in (IEnumerable)ioprovider.ParameterSet)
			{
				IParameter val = item;
				if ((int)val.ChannelType == 2 || (int)val.ChannelType == 3 || (int)val.ChannelType == 1)
				{
					ExportDataElement(ioprovider, null, (IDataElement)(object)val, sb, ((IDataElement)val).Identifier, bMotorolaBitfields);
				}
			}
			IIoProvider[] children = ioprovider.Children;
			foreach (IIoProvider ioprovider2 in children)
			{
				CollectMappings(ioprovider2, sb, bMotorolaBitfields);
			}
		}

		private void CollectDataElements(IIoProvider ioProvider)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Invalid comparison between Unknown and I4
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Invalid comparison between Unknown and I4
			if (ioProvider == null)
			{
				return;
			}
			foreach (IParameter item in (IEnumerable)ioProvider.ParameterSet)
			{
				IParameter val = item;
				if ((int)val.ChannelType == 2 || (int)val.ChannelType == 3 || (int)val.ChannelType == 1)
				{
					CollectDataElement(ioProvider, (IDataElement)(object)val);
				}
			}
		}

		internal static string GetIecAddress(IDataElement dataElement, IDataElement parentDateElement, bool bMotorolaBitFields)
		{
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				string text = ((dataElement.IoMapping != null) ? dataElement.IoMapping.IecAddress : string.Empty);
				if (!string.IsNullOrEmpty(text) && bMotorolaBitFields && dataElement is IDataElement2 && ((IDataElement2)((dataElement is IDataElement2) ? dataElement : null)).HasBaseType && dataElement.GetBitSize() == 1 && parentDateElement != null)
				{
					long bitSize = parentDateElement.GetBitSize();
					if (bitSize == 16 || bitSize == 32 || bitSize == 64)
					{
						IScanner val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(text, false, false, false, false);
						IToken val2 = default(IToken);
						if (val.Match((TokenType)7, true, out val2) > 0)
						{
							DirectVariableLocation location = default(DirectVariableLocation);
							DirectVariableSize size = default(DirectVariableSize);
							int[] array = default(int[]);
							bool flag = default(bool);
							val.GetDirectVariable(val2, out location, out size, out array, out flag);
							int num = (int)(bitSize / 8);
							array[0] += num - 1 - array[0] % num * 2;
							text = DirectVariableToString(location, size, array);
						}
					}
				}
				return text;
			}
			catch
			{
				return string.Empty;
			}
		}

		internal static string GetSizeString(DirectVariableSize size)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected I4, but got Unknown
			return ((int)size - 1) switch
			{
				0 => "X", 
				1 => "B", 
				2 => "W", 
				3 => "D", 
				4 => "L", 
				_ => "?", 
			};
		}

		internal static string DirectVariableToString(DirectVariableLocation location, DirectVariableSize size, int[] components)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected I4, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			LStringBuilder val = new LStringBuilder("%");
			switch ((int)location - 1)
			{
			case 0:
				val.Append("I");
				break;
			case 1:
				val.Append("Q");
				break;
			case 2:
				val.Append("M");
				break;
			default:
				return "";
			}
			val.Append(GetSizeString(size));
			for (int i = 0; i < components.Length; i++)
			{
				if (i > 0)
				{
					val.Append('.');
				}
				val.Append(components[i]);
			}
			return ((object)val).ToString();
		}

		private string GetDataElementName(IIoProvider ioprovider, IDataElement dataelement)
		{
			string text = ioprovider.GetMetaObject().Name + "@" + dataelement.VisibleName;
			if (_dictElementsCounter.ContainsKey(text))
			{
				LDictionary<string, int> dictElementsCounter = _dictElementsCounter;
				string text2 = text;
				int num = dictElementsCounter[text2];
				dictElementsCounter[text2]= num + 1;
				_dictElements.Add(text + "@" + _dictElementsCounter[text], dataelement);
				return dataelement.VisibleName + "@" + _dictElementsCounter[text];
			}
			_dictElementsCounter.Add(text, 0);
			_dictElements.Add(text, dataelement);
			return dataelement.VisibleName;
		}

		private void CollectDataElement(IIoProvider ioprovider, IDataElement dataelement)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			GetDataElementName(ioprovider, dataelement);
			if (!dataelement.HasSubElements)
			{
				return;
			}
			foreach (IDataElement item in (IEnumerable)dataelement.SubElements)
			{
				IDataElement dataelement2 = item;
				CollectDataElement(ioprovider, dataelement2);
			}
		}

		private void ExportDataElement(IIoProvider ioprovider, IDataElement parentElement, IDataElement dataelement, StringBuilder sb, string stName, bool bMotorolaBitfields)
		{
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Expected O, but got Unknown
			if (dataelement.IoMapping != null && dataelement.IoMapping.VariableMappings != null && ((ICollection)dataelement.IoMapping.VariableMappings).Count > 0)
			{
				sb.Append(dataelement.IoMapping.VariableMappings[0]
					.Variable);
			}
			sb.Append(";");
			sb.Append(GetDataElementName(ioprovider, dataelement));
			sb.Append(";");
			sb.Append(dataelement.Unit);
			sb.Append(";");
			sb.Append(dataelement.Description);
			sb.Append(";");
			sb.Append(GetIecAddress(dataelement, parentElement, bMotorolaBitfields));
			sb.Append(";");
			sb.Append(ioprovider.GetMetaObject().Name);
			sb.AppendLine();
			if (!dataelement.HasSubElements)
			{
				return;
			}
			foreach (IDataElement item in (IEnumerable)dataelement.SubElements)
			{
				IDataElement val = item;
				ExportDataElement(ioprovider, dataelement, val, sb, stName + "." + val.Identifier, bMotorolaBitfields);
			}
		}

		public void ImportFromCSV(string stFileName, int nProjectHandle, Guid hostDeviceGuid)
		{
			IMetaObject val = null;
			try
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, hostDeviceGuid);
				if (objectToRead.Object is IDeviceObject5)
				{
					ITargetSettingsList val2 = APEnvironment.TargetSettingsMgr.Settings;
					IObject @object = objectToRead.Object;
					ITargetSettings targetSettingsById = ((ITargetSettingsList)val2).GetTargetSettingsById(((IDeviceObject5)((@object is IDeviceObject5) ? @object : null)).DeviceIdentificationNoSimulation);
					_bMultipleMappableAllowed = LocalTargetSettings.MultipleMappableAllowed.GetBoolValue(targetSettingsById);
					_bBaseTypeMappable = LocalTargetSettings.BasetypeMappable.GetBoolValue(targetSettingsById);
					_bBitfieldMappable = LocalTargetSettings.BitfieldMappable.GetBoolValue(targetSettingsById);
					_bUnionRootEditable = LocalTargetSettings.UnionRootEditable.GetBoolValue(targetSettingsById);
				}
				AuthFileStream val3 = AuthFile.OpenRead(stFileName);
				try
				{
					using StreamReader streamReader = new StreamReader((Stream)(object)val3);
					IDataElement val9 = default(IDataElement);
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine();
						if (text.StartsWith("//"))
						{
							continue;
						}
						string[] array = text.Split(';');
						if (array.Length != 7 && array.Length != 9 && array.Length != 6)
						{
							continue;
						}
						int num = ((array.Length == 7) ? 3 : 5);
						long result = -1L;
						int result2 = -1;
						Guid empty = Guid.Empty;
						bool flag = true;
						if (array.Length > num + 2)
						{
							string text2 = array[num + 2];
							if (text2.Contains("."))
							{
								text2 = text2.Substring(0, text2.IndexOf('.'));
							}
							flag &= long.TryParse(text2, out result);
							if (!string.IsNullOrEmpty(array[num + 1]))
							{
								flag &= int.TryParse(array[num + 1], out result2);
							}
						}
						empty = Guid.Empty;
						Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(nProjectHandle, array[num]);
						foreach (Guid guid in allObjects)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
							if ((typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType)) && DeviceObjectHelper.GetHostStub(nProjectHandle, guid).ObjectGuid == hostDeviceGuid)
							{
								empty = guid;
								break;
							}
						}
						if (empty == Guid.Empty)
						{
							DeviceMessage deviceMessage = new DeviceMessage(string.Format(Strings.ErrorImportHostWrong, array[num]), (Severity)2);
							APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
							flag = false;
						}
						if (!flag || (string.IsNullOrEmpty(array[0]) && string.IsNullOrEmpty(array[2]) && string.IsNullOrEmpty(array[3])))
						{
							continue;
						}
						if (val == null || empty != val.ObjectGuid)
						{
							if (val != null && val.IsToModify)
							{
								((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
								val = null;
							}
							if (DeviceObjectHelper.GetHostStub(nProjectHandle, empty).ObjectGuid != hostDeviceGuid)
							{
								DeviceMessage deviceMessage2 = new DeviceMessage(string.Format(Strings.ErrorImportHostWrong, array[num]), (Severity)2);
								APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage2);
								break;
							}
							if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, empty))
							{
								try
								{
									val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, empty);
									_dictElements.Clear();
									_dictElementsCounter.Clear();
									if (val.Object is IDeviceObject)
									{
										IObject object2 = val.Object;
										CollectDataElements((IIoProvider)(object)((object2 is IIoProvider) ? object2 : null));
										IObject object3 = val.Object;
										foreach (IConnector item in (IEnumerable)((IDeviceObject)((object3 is IDeviceObject) ? object3 : null)).Connectors)
										{
											IConnector val4 = item;
											if ((int)val4.ConnectorRole == 1)
											{
												if (DeviceObjectHelper.CheckChildConnectorUsed(val4))
												{
													CollectDataElements((IIoProvider)(object)((val4 is IIoProvider) ? val4 : null));
												}
												continue;
											}
											IConnector parentConnector = DeviceObjectHelper.GetParentConnector(val4);
											if (parentConnector != null && DeviceObjectHelper.CheckChildConnectorUsed(parentConnector))
											{
												CollectDataElements((IIoProvider)(object)((val4 is IIoProvider) ? val4 : null));
											}
										}
									}
									if (val.Object is IExplicitConnector)
									{
										IObject object4 = val.Object;
										CollectDataElements((IIoProvider)(object)((object4 is IIoProvider) ? object4 : null));
									}
								}
								catch
								{
									val = null;
								}
							}
							else
							{
								val = null;
							}
						}
						if (val == null)
						{
							DeviceMessage deviceMessage3 = new DeviceMessage(string.Format(Strings.ErrorImportDevice, array[num]), (Severity)2);
							APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage3);
						}
						if (val == null)
						{
							continue;
						}
						IDataElement val5 = null;
						if (result >= 0)
						{
							if (val.Object is IDeviceObject)
							{
								IObject object5 = val.Object;
								IDeviceObject val6 = (IDeviceObject)(object)((object5 is IDeviceObject) ? object5 : null);
								if (result2 == -1)
								{
									if (val6.DeviceParameterSet.Contains(result))
									{
										val5 = (IDataElement)(object)val6.DeviceParameterSet.GetParameter(result);
									}
								}
								else
								{
									foreach (IConnector item2 in (IEnumerable)val6.Connectors)
									{
										IConnector val7 = item2;
										if (val7.ConnectorId == result2 && val7.HostParameterSet.Contains(result))
										{
											val5 = (IDataElement)(object)val7.HostParameterSet.GetParameter(result);
										}
									}
								}
							}
							if (val.Object is IExplicitConnector)
							{
								IObject object6 = val.Object;
								IExplicitConnector val8 = (IExplicitConnector)(object)((object6 is IExplicitConnector) ? object6 : null);
								if (((IConnector)val8).HostParameterSet.Contains(result))
								{
									val5 = (IDataElement)(object)((IConnector)val8).HostParameterSet.GetParameter(result);
								}
							}
							string stUnit = ((array.Length == 9 || array.Length == 6) ? array[2] : null);
							string stDescription = ((array.Length == 9 || array.Length == 6) ? array[3] : null);
							if (val5 != null && !ImportParameter(val5, array[num + 2], array[0], stUnit, stDescription))
							{
								DeviceMessage deviceMessage4 = new DeviceMessage(string.Format(Strings.ErrorImportParameter, array[1]), (Severity)2);
								APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage4);
							}
							continue;
						}
						string text3 = val.Name + "@" + array[1];
						if (!_dictElements.ContainsKey(text3))
						{
							int num2 = text3.LastIndexOf('@');
							if (num2 > 0)
							{
								string text4 = text3.Substring(0, num2);
								if (_dictElements.ContainsKey(text4))
								{
									text3 = text4;
								}
							}
						}
						if (_dictElements.TryGetValue(text3, out val9))
						{
							string text5 = ((array.Length == 9 || array.Length == 6) ? array[2] : null);
							string text6 = ((array.Length == 9 || array.Length == 6) ? array[3] : null);
							string text7 = array[0];
							if (!string.IsNullOrEmpty(text5))
							{
								if (val9 is DataElementBase)
								{
									(val9 as DataElementBase).SetUnit((IStringRef)(object)new StringRef("", "", text5));
								}
								if (val9 is IParameter12)
								{
									((IParameter12)((val9 is IParameter12) ? val9 : null)).SetUnit((IStringRef)(object)new StringRef("", "", text5));
								}
							}
							if (!string.IsNullOrEmpty(text6) && val9 is IDataElement4)
							{
								((IDataElement4)((val9 is IDataElement4) ? val9 : null)).SetDescription((IStringRef)(object)new StringRef("", "", text6));
							}
							if (string.IsNullOrEmpty(text7) || val9.IoMapping == null || val9.IoMapping.VariableMappings == null)
							{
								continue;
							}
							bool flag2 = false;
							if (((ICollection)val9.IoMapping.VariableMappings).Count > 0)
							{
								val9.IoMapping.VariableMappings.RemoveAt(0);
							}
							if (!_bMultipleMappableAllowed)
							{
								IParameter val10 = null;
								if (val9 is DataElementBase)
								{
									val10 = (IParameter)(object)(val9 as DataElementBase).Parent.GetParameter();
								}
								if (val9 is IParameter)
								{
									val10 = (IParameter)(object)((val9 is IParameter) ? val9 : null);
								}
								if (((val10 != null && (int)val10.ChannelType == 2) || (int)val10.ChannelType == 3) && !CheckForMultipleMapping(val10, (IDataElement2)(object)((val9 is IDataElement2) ? val9 : null)))
								{
									DeviceMessage deviceMessage5 = new DeviceMessage(string.Format(Strings.ImportCSVSkippingMultipleOutputs, array[0]), (Severity)8);
									APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage5);
									flag2 = true;
								}
							}
							text7 = text7.Trim('"', ' ');
							if (!flag2 && !string.IsNullOrEmpty(text7))
							{
								val9.IoMapping.VariableMappings.AddMapping(text7, !text7.Contains("."));
							}
						}
						else
						{
							DeviceMessage deviceMessage6 = new DeviceMessage(string.Format(Strings.ErrorImportParameter, array[1]), (Severity)2);
							APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage6);
						}
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
			}
			finally
			{
				if (val != null && val.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
				}
			}
		}

		internal bool CheckForMultipleMapping(IParameter parameter, IDataElement2 dataElement)
		{
			if (dataElement != null)
			{
				if (!_bBaseTypeMappable && !dataElement.HasBaseType)
				{
					return false;
				}
				if (!_bBitfieldMappable && ((IDataElement)dataElement).HasSubElements && dataElement.HasBaseType)
				{
					return false;
				}
				if (((IDataElement)dataElement).HasSubElements && !CheckSubMapping((IDataElement)(object)dataElement, bOnlyCreateVariable: false))
				{
					object obj;
					if (dataElement == null)
					{
						obj = null;
					}
					else
					{
						IIoMapping ioMapping = ((IDataElement)dataElement).IoMapping;
						obj = ((ioMapping != null) ? ioMapping.VariableMappings : null);
					}
					if (obj == null || ((ICollection)((IDataElement)dataElement).IoMapping.VariableMappings).Count <= 0)
					{
						return false;
					}
				}
				if (dataElement is DataElementBase)
				{
					object obj2 = dataElement;
					bool flag = true;
					while (obj2 != null)
					{
						IDataElement val = null;
						if (obj2 is IDataElement)
						{
							val = (IDataElement)((obj2 is IDataElement) ? obj2 : null);
						}
						else if (val is IDataElementParent)
						{
							val = (obj2 as IDataElementParent)?.DataElement;
						}
						if (val is IDataElement5 && ((IDataElement5)((val is IDataElement5) ? val : null)).IsUnion)
						{
							if (flag && !_bUnionRootEditable)
							{
								return false;
							}
							if (!flag)
							{
								object obj3;
								if (val == null)
								{
									obj3 = null;
								}
								else
								{
									IIoMapping ioMapping2 = val.IoMapping;
									obj3 = ((ioMapping2 != null) ? ioMapping2.VariableMappings : null);
								}
								if (obj3 != null && ((ICollection)val.IoMapping.VariableMappings).Count > 0 && !val.IoMapping.VariableMappings[0]
									.CreateVariable)
								{
									return false;
								}
							}
							return true;
						}
						obj2 = ((!(obj2 is DataElementBase)) ? ((!(obj2 is DataElementCollection)) ? null : (obj2 as DataElementCollection).Parent) : (obj2 as DataElementBase).Parent);
						flag = false;
					}
					for (IDataElementParent dataElementParent = (dataElement as DataElementBase).Parent; dataElementParent != null; dataElementParent = ((!(dataElementParent is DataElementBase)) ? ((!(dataElementParent is DataElementCollection)) ? null : (dataElementParent as DataElementCollection).Parent) : (dataElementParent as DataElementBase).Parent))
					{
						IDataElement val2 = dataElementParent?.DataElement;
						object obj4;
						if (val2 == null)
						{
							obj4 = null;
						}
						else
						{
							IIoMapping ioMapping3 = val2.IoMapping;
							obj4 = ((ioMapping3 != null) ? ioMapping3.VariableMappings : null);
						}
						if (obj4 != null && ((ICollection)val2.IoMapping.VariableMappings).Count > 0)
						{
							object obj5;
							if (dataElement == null)
							{
								obj5 = null;
							}
							else
							{
								IIoMapping ioMapping4 = ((IDataElement)dataElement).IoMapping;
								obj5 = ((ioMapping4 != null) ? ioMapping4.VariableMappings : null);
							}
							if (obj5 == null || ((ICollection)((IDataElement)dataElement).IoMapping.VariableMappings).Count <= 0)
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		internal static bool CheckSubMapping(IDataElement datalement, bool bOnlyCreateVariable)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			if (datalement.HasSubElements)
			{
				foreach (IDataElement item in (IEnumerable)datalement.SubElements)
				{
					IDataElement val = item;
					object obj;
					if (val == null)
					{
						obj = null;
					}
					else
					{
						IIoMapping ioMapping = val.IoMapping;
						obj = ((ioMapping != null) ? ioMapping.VariableMappings : null);
					}
					if (obj != null && ((ICollection)val.IoMapping.VariableMappings).Count > 0)
					{
						foreach (IVariableMapping item2 in (IEnumerable)val.IoMapping.VariableMappings)
						{
							IVariableMapping val2 = item2;
							if (!bOnlyCreateVariable)
							{
								return false;
							}
							if (!val2.CreateVariable)
							{
								return false;
							}
						}
					}
					if (!CheckSubMapping(val, bOnlyCreateVariable))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool ImportParameter(IDataElement element, string stIdentifier, string stVariable, string stUnit, string stDescription)
		{
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Expected O, but got Unknown
			string text = stIdentifier;
			if (!string.IsNullOrEmpty(stIdentifier) && stIdentifier.Contains("."))
			{
				text = stIdentifier.Substring(0, stIdentifier.IndexOf('.'));
			}
			if (text == element.Identifier)
			{
				if (stIdentifier == element.Identifier)
				{
					if (!string.IsNullOrEmpty(stUnit))
					{
						if (element is DataElementBase)
						{
							(element as DataElementBase).SetUnit((IStringRef)(object)new StringRef("", "", stUnit));
						}
						if (element is IParameter12)
						{
							((IParameter12)((element is IParameter12) ? element : null)).SetUnit((IStringRef)(object)new StringRef("", "", stUnit));
						}
					}
					if (!string.IsNullOrEmpty(stDescription) && element is IDataElement4)
					{
						((IDataElement4)((element is IDataElement4) ? element : null)).SetDescription((IStringRef)(object)new StringRef("", "", stDescription));
					}
					if (!string.IsNullOrEmpty(stVariable) && element.IoMapping != null && element.IoMapping.VariableMappings != null)
					{
						if (((ICollection)element.IoMapping.VariableMappings).Count > 0)
						{
							element.IoMapping.VariableMappings.RemoveAt(0);
						}
						element.IoMapping.VariableMappings.AddMapping(stVariable, !stVariable.Contains("."));
					}
					return true;
				}
				stIdentifier = stIdentifier.Remove(0, element.Identifier.Length + 1);
				if (element.IoMapping != null && element.IoMapping.VariableMappings != null && ((ICollection)element.IoMapping.VariableMappings).Count > 0 && !_bMultipleMappableAllowed && element.IoMapping.IecAddress.Contains("%Q"))
				{
					return true;
				}
				if (element.HasSubElements)
				{
					foreach (IDataElement item in (IEnumerable)element.SubElements)
					{
						IDataElement element2 = item;
						if (ImportParameter(element2, stIdentifier, stVariable, stUnit, stDescription))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
