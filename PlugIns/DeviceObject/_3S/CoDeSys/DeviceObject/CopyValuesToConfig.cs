using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{F34ABCF8-4944-41FC-A57E-D9A5EFC1D00E}")]
	public class CopyValuesToConfig : IStandardCommand, ICommand
	{
		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string Name => Strings.IoConfigParCmdName;

		public string Description => Strings.IoConfigParCmdDescription;

		public string ToolTipText => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public bool Enabled
		{
			get
			{
				IMetaObjectStub selectedStub = DeviceCommandHelper.GetSelectedStub();
				if (selectedStub != null && selectedStub.ParentObjectGuid == Guid.Empty)
				{
					return true;
				}
				return false;
			}
		}

		public string[] BatchCommand => new string[0];

		public string[] CreateBatchArguments()
		{
			return new string[0];
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (bContextMenu)
			{
				if (Enabled)
				{
					return ((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl;
				}
				return false;
			}
			return false;
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Expected O, but got Unknown
			DeviceObject deviceObject = DeviceCommandHelper.GetSelectedDevice() as DeviceObject;
			if (deviceObject == null)
			{
				return;
			}
			IMetaObject application = deviceObject.GetApplication();
			if (application == null)
			{
				return;
			}
			APEnvironment.MessageStorage.ClearMessages((IMessageCategory)(object)DeviceMessageCategory.Instance);
			if (deviceObject == null || !UploadIoConfig((IDeviceObject)(object)deviceObject, out var memStream))
			{
				return;
			}
			LDictionary<ulong, IIoProvider> val = new LDictionary<ulong, IIoProvider>();
			CollectIoProviders((IIoProvider)(object)deviceObject, val);
			try
			{
				IMetaObject val2 = null;
				StreamReader streamReader = new StreamReader(memStream);
				LList<string> val3 = new LList<string>();
				memStream.Seek(0L, SeekOrigin.Begin);
				while (!streamReader.EndOfStream)
				{
					string text = streamReader.ReadLine();
					if (!string.IsNullOrEmpty(text) && text.Split(',').Length == 6)
					{
						val3.Add(text);
					}
				}
				val3.Sort();
				streamReader.Close();
				foreach (string item in val3)
				{
					if (string.IsNullOrEmpty(item))
					{
						continue;
					}
					string[] array = item.Split(',');
					if (array.Length != 6 || (1u & (uint.TryParse(array[0], out var result) ? 1u : 0u) & (uint.TryParse(array[1], out var result2) ? 1u : 0u) & (uint.TryParse(array[2], out var result3) ? 1u : 0u) & (uint.TryParse(array[3], out var result4) ? 1u : 0u) & (uint.TryParse(array[4], out var result5) ? 1u : 0u)) == 0)
					{
						continue;
					}
					ulong num = ((ulong)result << 32) | result2;
					if (!val.ContainsKey(num))
					{
						continue;
					}
					IIoProvider val4 = val[num];
					IMetaObject metaObject = val4.GetMetaObject();
					if (val2 == null || metaObject.ObjectGuid != val2.ObjectGuid)
					{
						if (val2 != null && val2.IsToModify)
						{
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val2, true, (object)null);
							val2 = null;
						}
						if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaObject.ProjectHandle, metaObject.ObjectGuid))
						{
							try
							{
								DeviceMessage deviceMessage = new DeviceMessage(string.Format(Strings.IoConfigParReadingParameter, metaObject.Name), (Severity)8);
								APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
								val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(metaObject.ProjectHandle, metaObject.ObjectGuid);
							}
							catch
							{
								val2 = null;
							}
						}
						else
						{
							val2 = null;
						}
					}
					int num2 = -1;
					if (val4 is IConnector)
					{
						num2 = ((IConnector)((val4 is IConnector) ? val4 : null)).ConnectorId;
					}
					if (val2 == null)
					{
						continue;
					}
					IDataElement val5 = null;
					if (val2.Object is IDeviceObject)
					{
						IObject @object = val2.Object;
						IDeviceObject val6 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
						if (num2 == -1)
						{
							if (val6.DeviceParameterSet.Contains((long)result3))
							{
								val5 = (IDataElement)(object)deviceObject.DeviceParameterSet.GetParameter((long)result3);
							}
						}
						else
						{
							foreach (IConnector item2 in (IEnumerable)val6.Connectors)
							{
								IConnector val7 = item2;
								if (val7.ConnectorId == num2 && val7.HostParameterSet.Contains((long)result3))
								{
									val5 = (IDataElement)(object)val7.HostParameterSet.GetParameter((long)result3);
								}
							}
						}
					}
					if (val2.Object is IExplicitConnector)
					{
						IObject object2 = val2.Object;
						IExplicitConnector val8 = (IExplicitConnector)(object)((object2 is IExplicitConnector) ? object2 : null);
						if (((IConnector)val8).HostParameterSet.Contains((long)result3))
						{
							val5 = (IDataElement)(object)((IConnector)val8).HostParameterSet.GetParameter((long)result3);
						}
					}
					if (val5 != null)
					{
						SetValue((IDataElement4)(object)((val5 is IDataElement4) ? val5 : null), result5, result4, array[5], application.ObjectGuid);
					}
				}
				if (val2 != null && val2.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val2, true, (object)null);
				}
			}
			finally
			{
				memStream.Close();
			}
		}

		private bool SetValue(IDataElement4 dataElement, uint uiBitOffset, uint uiBitSize, string stValue, Guid applicationGuid)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Invalid comparison between Unknown and I4
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Invalid comparison between Unknown and I4
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Invalid comparison between Unknown and I4
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Expected O, but got Unknown
			if (dataElement.GetBitOffset() == uiBitOffset && ((IDataElement)dataElement).GetBitSize() == uiBitSize && ((IDataElement2)dataElement).HasBaseType)
			{
				try
				{
					byte[] array = HexStringToBytes(stValue.Trim());
					ICompiledType val = Types.ParseType(((IDataElement)dataElement).BaseType);
					ByteOrder val2 = (ByteOrder)0;
					ICompileContext referenceContextIfAvailable = ((ILanguageModelManager2)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(applicationGuid);
					if (referenceContextIfAvailable != null && referenceContextIfAvailable.Codegenerator != null)
					{
						val2 = (ByteOrder)(referenceContextIfAvailable.Codegenerator.MotorolaByteOrder ? 1 : 0);
					}
					object obj = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).ConvertRaw(array, (IType)(object)val, applicationGuid, val2);
					IConverterToIEC converterToIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)1);
					if ((int)((IType)val).Class == 26)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("[");
						object[] array2 = obj as object[];
						if (array2 != null)
						{
							for (int i = 0; i < array2.Length; i++)
							{
								if (i > 0)
								{
									stringBuilder.Append(",");
								}
								stringBuilder.Append(converterToIEC.GetLiteralText(array2[i], ((IType)val.BaseType).Class));
							}
						}
						stringBuilder.Append("]");
						((IDataElement)dataElement).Value=(stringBuilder.ToString());
						return true;
					}
					if (((IDataElement)dataElement).IsEnumeration && dataElement is IEnumerationDataElement)
					{
						int num = default(int);
						IEnumerationValue enumerationValue = ((IEnumerationDataElement)dataElement).GetEnumerationValue(obj, out num);
						if (enumerationValue != null)
						{
							((IDataElement)dataElement).EnumerationValue=(enumerationValue);
						}
					}
					else if ((int)((IType)val).Class == 29)
					{
						((IDataElement)dataElement).Value=(obj.ToString());
					}
					else if ((int)((IType)val).Class == 24)
					{
						((IDataElement)dataElement).Value=(converterToIEC.GetLiteralText(obj, ((IType)val.BaseType).Class));
					}
					else
					{
						((IDataElement)dataElement).Value=(converterToIEC.GetLiteralText(obj, ((IType)val).Class));
					}
					return true;
				}
				catch
				{
					return false;
				}
			}
			if (((IDataElement)dataElement).HasSubElements)
			{
				foreach (IDataElement4 item in (IEnumerable)((IDataElement)dataElement).SubElements)
				{
					IDataElement4 dataElement2 = item;
					if (SetValue(dataElement2, uiBitOffset, uiBitSize, stValue, applicationGuid))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static byte[] HexStringToBytes(string hex)
		{
			byte[] array = new byte[(hex.Length + 1) / 2];
			try
			{
				int num = 0;
				for (int i = 0; i < hex.Length; i += 2)
				{
					if (i < hex.Length - 1)
					{
						array[array.Length - num - 1] = Convert.ToByte(hex.Substring(i, 2), 16);
					}
					else
					{
						array[array.Length - num - 1] = Convert.ToByte(hex.Substring(i), 16);
					}
					num++;
				}
				return array;
			}
			catch
			{
				return array;
			}
		}

		private void CollectIoProviders(IIoProvider ioProvider, LDictionary<ulong, IIoProvider> dictIoproviders)
		{
			if (ioProvider != null)
			{
				uint num = default(uint);
				uint num2 = default(uint);
				ioProvider.GetModuleAddress(out num, out num2);
				dictIoproviders.Add(((ulong)num << 32) | num2, ioProvider);
				IIoProvider[] children = ioProvider.Children;
				foreach (IIoProvider ioProvider2 in children)
				{
					CollectIoProviders(ioProvider2, dictIoproviders);
				}
			}
		}

		private bool UploadIoConfig(IDeviceObject deviceObject, out MemoryStream memStream)
		{
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected I4, but got Unknown
			memStream = null;
			IAsyncResult asyncResult = null;
			long num = 0L;
			IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)deviceObject).MetaObject.ObjectGuid);
			IOnlineDevice3 val = (IOnlineDevice3)(object)((onlineDevice is IOnlineDevice3) ? onlineDevice : null);
			object obj = new object();
			try
			{
				val.SharedConnect(obj);
			}
			catch (Exception ex)
			{
				APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
			}
			try
			{
				if (((IOnlineDevice)val).IsConnected)
				{
					string text = "IoConfig.par";
					memStream = new MemoryStream();
					long num2 = 0L;
					long num3 = -1L;
					IFileUpload obj2 = ((IOnlineDevice2)val).CreateFileUpload((Stream)memStream, text);
					IFileUpload2 val2 = (IFileUpload2)(object)((obj2 is IFileUpload2) ? obj2 : null);
					if (val2 is IFileUpload3)
					{
						((IFileUpload3)((val2 is IFileUpload3) ? val2 : null)).SetHostPath(Path.GetTempFileName());
					}
					asyncResult = ((IFileUpload)val2).BeginUpload(true, (AsyncCallback)null, (object)null);
					try
					{
						UploadProgress val3 = default(UploadProgress);
						while (!asyncResult.AsyncWaitHandle.WaitOne(10, exitContext: false))
						{
							val2.GetProgress(asyncResult, out val3, out num2, out num3);
							switch ((int)val3)
							{
							case 2:
								num = num2;
								break;
							}
							Application.DoEvents();
						}
					}
					catch (Exception ex2)
					{
						asyncResult = null;
						throw ex2;
					}
					((IFileUpload)val2).EndUpload(asyncResult);
					return true;
				}
			}
			catch (Exception ex3)
			{
				if (num > 0)
				{
					APEnvironment.MessageService.Error(ex3.Message, "Exception", Array.Empty<object>());
				}
				else
				{
					APEnvironment.MessageService.Warning(Strings.IoConfigParWarningFile, "IoConfigParWarningFile", Array.Empty<object>());
				}
			}
			finally
			{
				val.SharedDisconnect(obj);
			}
			return false;
		}
	}
}
