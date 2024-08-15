using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject.LogicalDevice;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{0446980F-8351-46bc-9759-D59411343104}")]
	[SystemInterface("_3S.CoDeSys.DeviceObject.ILogicalIONotification")]
	public class LogicalIONotification : ILogicalIONotification, ILanguageModelProvider, ISystemInstanceRequiresInitialization
	{
		private static IDeviceManagerBuffer _logicalMappingApp;

		private static IDeviceManagerBuffer _logicalExchangeGVL;

		private string _stLogicalIoErrors = string.Empty;

		private Guid _applicationGuid = Guid.Empty;

		private Guid _LogicalIoErrorGuid = new Guid("{6A213A57-C259-4513-9485-13A74AF49472}");

		internal IUndoManager4 _undoMgr;

		private LDictionary<int, Guid> _dictAppsToRemove = new LDictionary<int, Guid>();

		public const long LOGICALDEVICEIDENT = 1879048216L;

		public const long LOGICALDEVICEIDENTCOUNT = 2130706456L;

		public const long FIRSTLOGICALDEVICEIDENT = 2130706457L;

		internal static IDeviceManagerBuffer LogicalMappingApps => _logicalMappingApp;

		internal static IDeviceManagerBuffer LogicalExchangeGVL => _logicalExchangeGVL;

		public IDeviceManagerBuffer LogicalExchangeGVLs => _logicalExchangeGVL;

		public void OnAllSystemInstancesAvailable()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded+=(new ObjectEventHandler(OnObjectAdded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved+=(new ObjectRemovedEventHandler(ObjectMgr_ObjectRemoved));
			((ILanguageModelManager)APEnvironment.LanguageModelMgr).BeforeCompile+=(new CompileEventHandler(OnCompile));
			_logicalMappingApp = APEnvironment.DeviceMgr.CreateDeviceBuffer(IsLogicalMappingApp);
			_logicalExchangeGVL = APEnvironment.DeviceMgr.CreateDeviceBuffer(IsLogicalExchangeGVL);
		}

		private bool IsLogicalMappingApp(IDeviceManagerInfo deviceInfo)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(deviceInfo.ProjectHandle, deviceInfo.ObjectGuid);
			if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				IObjectProperty[] properties = metaObjectStub.Properties;
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i] is ILogicalApplicationProperty)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsLogicalExchangeGVL(IDeviceManagerInfo deviceInfo)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(deviceInfo.ProjectHandle, deviceInfo.ObjectGuid);
			if (typeof(ILogicalGVLObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return true;
			}
			return false;
		}

		private void OnCompile(object sender, CompileEventArgs e)
		{
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Expected O, but got Unknown
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Expected O, but got Unknown
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0681: Invalid comparison between Unknown and I4
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Invalid comparison between Unknown and I4
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Invalid comparison between Unknown and I4
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0700: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Expected O, but got Unknown
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Invalid comparison between Unknown and I4
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Invalid comparison between Unknown and I4
			//IL_0729: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Invalid comparison between Unknown and I4
			if (!DeviceObjectHelper.GenerateCodeForLogicalDevices)
			{
				return;
			}
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null || ((DeviceObjectHelper.PhysicalDevices == null || DeviceObjectHelper.PhysicalDevices.DeviceGuids.Count == 0) && (DeviceObjectHelper.AdditionalModules == null || DeviceObjectHelper.AdditionalModules.DeviceGuids.Count == 0)))
			{
				return;
			}
			_applicationGuid = e.ApplicationGuid;
			int handle = primaryProject.Handle;
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			StringBuilder stringBuilder = new StringBuilder();
			IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(handle, _applicationGuid);
			if (hostStub != null)
			{
				IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, hostStub.ObjectGuid).Object;
				IDeviceObject5 val = (IDeviceObject5)(object)((@object is IDeviceObject5) ? @object : null);
				if (val != null)
				{
					ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentificationNoSimulation);
					num = LocalTargetSettings.MaxLogicalInputBitSize.GetIntValue(targetSettingsById);
					num2 = LocalTargetSettings.MaxLogicalOutputBitSize.GetIntValue(targetSettingsById);
				}
			}
			if (hostStub != null)
			{
				SortedList<Guid, string> logicalApplications = LogicalIOHelper.GetLogicalApplications(handle);
				LogicalIOHelper.RemoveUnusedLogicalApps(handle);
				foreach (KeyValuePair<Guid, string> item in logicalApplications)
				{
					LogicalIOHelper.CreateLogicalApp(handle, item.Key, item.Value);
				}
				if (DeviceObjectHelper.LogicalDevices != null)
				{
					foreach (Guid deviceGuid in DeviceObjectHelper.LogicalDevices.DeviceGuids)
					{
						IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(handle, deviceGuid);
						if (hostStub2 == null || !(hostStub.ObjectGuid == hostStub2.ObjectGuid))
						{
							continue;
						}
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, deviceGuid);
						if (!typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							continue;
						}
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, deviceGuid);
						IObject object2 = objectToRead.Object;
						ILogicalDevice val2 = (ILogicalDevice)(object)((object2 is ILogicalDevice) ? object2 : null);
						foreach (IMappedDevice item2 in (IEnumerable)val2.MappedDevices)
						{
							if (!item2.IsMapped)
							{
								stringBuilder.AppendFormat("{{messageguid '{0}'}}", deviceGuid);
								if (val2.LanguageModelPositionId != -1)
								{
									stringBuilder.AppendFormat("{{p {0} }}", val2.LanguageModelPositionId);
								}
								string arg = string.Format(LogicalIOStrings.ErrorLogicalDeviceNotMapped, objectToRead.Name);
								stringBuilder.AppendFormat("{{error '{0}'}}", arg);
							}
						}
					}
				}
				LList<Guid> val3 = new LList<Guid>();
				IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(handle);
				try
				{
					if (undoManager != null)
					{
						undoManager.BeginCompoundAction("Update LanguageModel for logical devices");
					}
					foreach (Guid deviceGuid2 in DeviceObjectHelper.PhysicalDevices.DeviceGuids)
					{
						IMetaObjectStub hostStub3 = DeviceObjectHelper.GetHostStub(handle, deviceGuid2);
						if (hostStub3 == null || !(hostStub.ObjectGuid == hostStub3.ObjectGuid))
						{
							continue;
						}
						IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, deviceGuid2);
						if (!typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub2.ObjectType))
						{
							continue;
						}
						IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, deviceGuid2);
						IObject object3 = objectToRead2.Object;
						ILogicalDevice val4 = (ILogicalDevice)(object)((object3 is ILogicalDevice) ? object3 : null);
						foreach (IMappedDevice item3 in (IEnumerable)val4.MappedDevices)
						{
							IMappedDevice val5 = item3;
							if (val5.IsMapped)
							{
								if (val4.IsPhysical)
								{
									((IEngine2)APEnvironment.Engine).UpdateLanguageModel(handle, deviceGuid2);
								}
								if (objectToRead2.Object is LogicalExchangeGVLObject)
								{
									LogicalExchangeGVLObject logicalExchangeGVLObject = objectToRead2.Object as LogicalExchangeGVLObject;
									if ((!logicalExchangeGVLObject.UseCombinedType || !logicalExchangeGVLObject.CanUseCombinedType) && logicalExchangeGVLObject.Declaration(bMapped: false, bLanguageModel: true).ToLowerInvariant() != logicalExchangeGVLObject.Declaration(bMapped: true, bLanguageModel: true).ToLowerInvariant())
									{
										stringBuilder.AppendFormat("{{messageguid '{0}'}}", deviceGuid2);
										if (val4.LanguageModelPositionId != -1)
										{
											stringBuilder.AppendFormat("{{p {0} }}", val4.LanguageModelPositionId);
										}
										string arg2 = string.Format(LogicalIOStrings.ErrorLogicalDeviceNotCompatible, val5.MappedDevice);
										stringBuilder.AppendFormat("{{error '{0}'}}", arg2);
									}
								}
								Guid getMappedDevice = val5.GetMappedDevice;
								if (getMappedDevice == Guid.Empty)
								{
									stringBuilder.AppendFormat("{{messageguid '{0}'}}", deviceGuid2);
									if (val4.LanguageModelPositionId != -1)
									{
										stringBuilder.AppendFormat("{{p {0} }}", val4.LanguageModelPositionId);
									}
									string arg3 = string.Format(LogicalIOStrings.ErrorMappingNotValid, val5.MappedDevice);
									stringBuilder.AppendFormat("{{error '{0}'}}", arg3);
								}
								else
								{
									IMetaObject objectToRead3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, getMappedDevice);
									if (objectToRead3 != null && objectToRead3.Object is IDeviceObject5)
									{
										IObject object4 = objectToRead3.Object;
										IDeviceObject5 val6 = (IDeviceObject5)(object)((object4 is IDeviceObject5) ? object4 : null);
										bool flag = false;
										if (val6.DeviceIdentificationNoSimulation.Type == 152 && val4 is LogicalExchangeGVLObject)
										{
											flag = true;
										}
										if (!flag)
										{
											foreach (MatchingLogicalDevice matchingLogicalDevice in (val5 as LogicalMappedDevice).MatchingLogicalDevices)
											{
												if (LogicalMappedDevice.CheckMatching(matchingLogicalDevice.DeviceIdentification, val6.DeviceIdentificationNoSimulation))
												{
													flag = true;
													break;
												}
											}
										}
										if (!flag)
										{
											stringBuilder.AppendFormat("{{messageguid '{0}'}}", deviceGuid2);
											if (val4.LanguageModelPositionId != -1)
											{
												stringBuilder.AppendFormat("{{p {0} }}", val4.LanguageModelPositionId);
											}
											string arg4 = string.Format(LogicalIOStrings.ErrorLogicalDeviceNotCompatible, val5.MappedDevice);
											stringBuilder.AppendFormat("{{error '{0}'}}", arg4);
										}
									}
									if (val3.Contains(getMappedDevice))
									{
										stringBuilder.AppendFormat("{{messageguid '{0}'}}", deviceGuid2);
										if (val4.LanguageModelPositionId != -1)
										{
											stringBuilder.AppendFormat("{{p {0} }}", val4.LanguageModelPositionId);
										}
										string arg5 = string.Format(LogicalIOStrings.ErrorLogicalDeviceMultipleMapped, val5.MappedDevice);
										stringBuilder.AppendFormat("{{error '{0}'}}", arg5);
										stringBuilder.AppendFormat("{{info '{0}'}}", LogicalIOStrings.InfoMultipleMapping);
									}
									else
									{
										val3.Add(getMappedDevice);
									}
								}
							}
							if (!val4.IsLogical || (num <= 0 && num2 <= 0))
							{
								continue;
							}
							IObject object5 = objectToRead2.Object;
							IDeviceObject val7 = (IDeviceObject)(object)((object5 is IDeviceObject) ? object5 : null);
							if (val7 == null)
							{
								continue;
							}
							foreach (IParameter item4 in (IEnumerable)val7.DeviceParameterSet)
							{
								IParameter val8 = item4;
								if ((int)val8.ChannelType == 1)
								{
									num3 += ((IDataElement)val8).GetBitSize();
								}
								if ((int)val8.ChannelType == 2 || (int)val8.ChannelType == 3)
								{
									num4 += ((IDataElement)val8).GetBitSize();
								}
							}
							foreach (IConnector item5 in (IEnumerable)val7.Connectors)
							{
								foreach (IParameter item6 in (IEnumerable)item5.HostParameterSet)
								{
									IParameter val9 = item6;
									if ((int)val9.ChannelType == 1)
									{
										num3 += ((IDataElement)val9).GetBitSize();
									}
									if ((int)val9.ChannelType == 2 || (int)val9.ChannelType == 3)
									{
										num4 += ((IDataElement)val9).GetBitSize();
									}
								}
							}
						}
					}
				}
				finally
				{
					if (undoManager != null)
					{
						undoManager.EndCompoundAction();
					}
				}
			}
			if (num > 0 || num2 > 0)
			{
				if (num3 > num && num > 0)
				{
					string arg6 = string.Format(LogicalIOStrings.ErrorInputSizeExceeded, (num3 + 7) / 8, (num + 7) / 8);
					stringBuilder.AppendFormat("{{error '{0}'}}", arg6);
				}
				if (num4 > num2 && num2 > 0)
				{
					string arg7 = string.Format(LogicalIOStrings.ErrorOutputSizeExceeded, (num4 + 7) / 8, (num2 + 7) / 8);
					stringBuilder.AppendFormat("{{error '{0}'}}", arg7);
				}
			}
			_stLogicalIoErrors = stringBuilder.ToString();
			if (_stLogicalIoErrors == string.Empty)
			{
				((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(handle, _LogicalIoErrorGuid);
			}
			else
			{
				((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)this, true);
			}
		}

		private void OnObjectAdded(object sender, ObjectEventArgs e)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
			if (typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				string stName;
				Guid logicalAppForDevice = LogicalIOHelper.GetLogicalAppForDevice(e.ProjectHandle, e.ObjectGuid, out stName);
				if (logicalAppForDevice != Guid.Empty)
				{
					LogicalIOHelper.CreateLogicalApp(e.ProjectHandle, logicalAppForDevice, stName);
				}
			}
			if (typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				AddLogicalIdentification(e.ProjectHandle, e.ObjectGuid);
			}
		}

		internal void undoMgr_AfterEndCompoundAction2(object sender, EventArgs e)
		{
			this._undoMgr.AfterEndCompoundAction2 -= this.undoMgr_AfterEndCompoundAction2;
			foreach (KeyValuePair<int, Guid> keyValuePair in this._dictAppsToRemove)
			{
				if (APEnvironment.ObjectMgr.ExistsObject(keyValuePair.Key, keyValuePair.Value))
				{
					try
					{
						APEnvironment.ObjectMgr.RemoveObjectWithoutParentCheck(keyValuePair.Key, keyValuePair.Value);
					}
					catch
					{
					}
				}
			}
			this._dictAppsToRemove.Clear();
		}

		private void ObjectMgr_ObjectRemoved(object sender, ObjectRemovedEventArgs e)
		{
			if (!DeviceObjectHelper.GenerateCodeForLogicalDevices || !(e.MetaObject.Object is ILogicalObject))
			{
				return;
			}
			Guid[] array = new Guid[LogicalMappingApps.DeviceGuids.Count];
			LogicalMappingApps.DeviceGuids.CopyTo(array, 0);
			Guid[] array2 = array;
			foreach (Guid guid in array2)
			{
				if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.MetaObject.ProjectHandle, guid))
				{
					continue;
				}
				IObjectProperty[] properties = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.MetaObject.ProjectHandle, guid).Properties;
				foreach (IObjectProperty val in properties)
				{
					if (!(val is ILogicalApplicationProperty))
					{
						continue;
					}
					Guid logicalApplication = (val as ILogicalApplicationProperty).LogicalApplication;
					if (!(logicalApplication == e.MetaObject.ParentObjectGuid) && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.MetaObject.ProjectHandle, logicalApplication))
					{
						continue;
					}
					ref IUndoManager4 undoMgr = ref _undoMgr;
					IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(e.MetaObject.ProjectHandle);
					undoMgr = (IUndoManager4)(object)((undoManager is IUndoManager4) ? undoManager : null);
					if (_undoMgr != null && ((IUndoManager)_undoMgr).InCompoundAction && !((IUndoManager)_undoMgr).InRedo && !((IUndoManager)_undoMgr).InUndo)
					{
						_undoMgr.AfterEndCompoundAction2+=((EventHandler)undoMgr_AfterEndCompoundAction2);
						_dictAppsToRemove.Add(e.MetaObject.ProjectHandle, guid);
						continue;
					}
					try
					{
						((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(e.MetaObject.ProjectHandle, guid);
					}
					catch
					{
					}
				}
			}
		}

		internal static void AddIdentification(string stParamValue, List<uint> liIdentifications)
		{
			if (!string.IsNullOrEmpty(stParamValue))
			{
				uint result = 0u;
				string[] array = stParamValue.Trim(' ', '[', ']').Split(',');
				if (array.Length != 0 && uint.TryParse(array[0], out result) && !liIdentifications.Contains(result))
				{
					liIdentifications.Add(result);
				}
			}
		}

		internal static void CollectIdentifications(int nProjectHandle, ICollection<Guid> guids, List<uint> liIdentifications, Guid guidToIgnore)
		{
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(nProjectHandle, guidToIgnore);
			if (hostStub == null)
			{
				return;
			}
			foreach (Guid guid in guids)
			{
				if (guid == guidToIgnore)
				{
					continue;
				}
				IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(nProjectHandle, guid);
				if (hostStub2 != null && hostStub2.ObjectGuid != hostStub.ObjectGuid)
				{
					continue;
				}
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					LogicalIODevice logicalIODevice = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid).Object as LogicalIODevice;
					if (logicalIODevice != null)
					{
						foreach (IConnector item in (IEnumerable)logicalIODevice.Connectors)
						{
							IConnector val = item;
							if (val.HostParameterSet.Contains(1879048216L))
							{
								IParameter parameter = val.HostParameterSet.GetParameter(1879048216L);
								if (parameter != null)
								{
									AddIdentification(((IDataElement)parameter).Value, liIdentifications);
								}
							}
							if (!val.HostParameterSet.Contains(2130706456L))
							{
								continue;
							}
							IParameter parameter2 = val.HostParameterSet.GetParameter(2130706456L);
							if (parameter2 == null || string.IsNullOrEmpty(((IDataElement)parameter2).Value) || !uint.TryParse(((IDataElement)parameter2).Value, out var result))
							{
								continue;
							}
							for (uint num = 0u; num < result; num++)
							{
								parameter2 = val.HostParameterSet.GetParameter(2130706457L + (long)num);
								if (parameter2 != null)
								{
									AddIdentification(((IDataElement)parameter2).Value, liIdentifications);
								}
							}
						}
					}
				}
				if (!typeof(IAdditionalModules).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					continue;
				}
				IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid).Object;
				IAdditionalModules val2 = (IAdditionalModules)(object)((@object is IAdditionalModules) ? @object : null);
				if (val2 == null)
				{
					continue;
				}
				foreach (IAdditionalModuleData additionalModule in val2.AdditionalModules)
				{
					uint result2 = 0u;
					foreach (IAdditionalModuleParameter parameter3 in additionalModule.ParameterList)
					{
						if (parameter3.ParameterId == 1879048216)
						{
							AddIdentification(parameter3.Value, liIdentifications);
						}
						if (parameter3.ParameterId == 2130706456)
						{
							uint.TryParse(parameter3.Value, out result2);
						}
						if (parameter3.ParameterId >= 2130706457 && parameter3.ParameterId < 2130706457L + (long)result2)
						{
							AddIdentification(parameter3.Value, liIdentifications);
						}
					}
				}
			}
		}

		private void AddLogicalIdentification(int nProjectHandle, Guid objectGuid)
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, objectGuid))
			{
				return;
			}
			List<uint> list = new List<uint>();
			if (DeviceObjectHelper.LogicalDevices == null)
			{
				return;
			}
			if (DeviceObjectHelper.AdditionalModules != null && DeviceObjectHelper.AdditionalModules.DeviceGuids != null)
			{
				CollectIdentifications(nProjectHandle, DeviceObjectHelper.AdditionalModules.DeviceGuids, list, objectGuid);
			}
			CollectIdentifications(nProjectHandle, DeviceObjectHelper.LogicalDevices.DeviceGuids, list, objectGuid);
			IMetaObject val = null;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, objectGuid);
				if (val == null)
				{
					return;
				}
				string text = "std:ARRAY[0..7] OF DWORD";
				foreach (IConnector item in (IEnumerable)(val.Object as LogicalIODevice).Connectors)
				{
					IConnector val2 = item;
					Parameter parameter = null;
					if (!val2.HostParameterSet.Contains(1879048216L))
					{
						parameter = val2.HostParameterSet.AddParameter(1879048216L, "LogicalDeviceIdentification", (AccessRight)1, (AccessRight)1, (ChannelType)0, text) as Parameter;
						if (parameter != null)
						{
							parameter.LogicalParameter = true;
						}
					}
					else
					{
						parameter = val2.HostParameterSet.GetParameter(1879048216L) as Parameter;
					}
					if (parameter == null)
					{
						continue;
					}
					if (parameter.ParamType != text)
					{
						break;
					}
					string text2 = parameter.Value.Replace("[", "").Replace("]", "");
					string[] array = text2.Split(',');
					if (text2.Length > 0)
					{
						uint.TryParse(array[0], out var result);
						if (!list.Contains(result))
						{
							break;
						}
					}
					for (uint num = 1u; num < uint.MaxValue; num++)
					{
						if (list.Contains(num))
						{
							continue;
						}
						array[0] = num.ToString();
						StringBuilder stringBuilder = new StringBuilder();
						bool flag = true;
						string[] array2 = array;
						foreach (string value in array2)
						{
							if (!flag)
							{
								stringBuilder.Append(",");
							}
							stringBuilder.Append(value);
							flag = false;
						}
						parameter.Value = "[" + stringBuilder.ToString() + "]";
						break;
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

		public string GetLanguageModel()
		{
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.WriteStartElement("language-model");
			xmlTextWriter.WriteAttributeString("application-id", XmlConvert.ToString(_applicationGuid));
			xmlTextWriter.WriteStartElement("pou");
			xmlTextWriter.WriteAttributeString("id", XmlConvert.ToString(_LogicalIoErrorGuid));
			string text = "LogicalIO_Errors";
			xmlTextWriter.WriteAttributeString("name", text);
			string format = "\r\n{0}\r\n";
			string value = $"\r\n{{attribute 'signature_flag' := '1073741824'}}\r\n{{implicit}}\r\nPROGRAM {text}\r\nVAR\r\nEND_VAR\r\n";
			string value2 = string.Format(format, _stLogicalIoErrors);
			xmlTextWriter.WriteElementString("interface", value);
			xmlTextWriter.WriteElementString("body", value2);
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Close();
			return stringWriter.ToString();
		}
	}
}
