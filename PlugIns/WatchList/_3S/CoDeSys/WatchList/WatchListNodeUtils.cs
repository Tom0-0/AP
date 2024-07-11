using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.WatchList
{
	public static class WatchListNodeUtils
	{
		internal static readonly Image IMAGE_VAR = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(WatchListNode), "_3S.CoDeSys.WatchList.Resources.VarSmall.ico").ToBitmap();

		private static readonly Image IMAGE_VAR_INPUT = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(WatchListNode), "_3S.CoDeSys.WatchList.Resources.VarInputSmall.ico").ToBitmap();

		private static readonly Image IMAGE_VAR_OUTPUT = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(WatchListNode), "_3S.CoDeSys.WatchList.Resources.VarOutputSmall.ico").ToBitmap();

		private static readonly Image IMAGE_VAR_IN_OUT = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(WatchListNode), "_3S.CoDeSys.WatchList.Resources.VarInOutSmall.ico").ToBitmap();

		private static readonly Image IMAGE_VAR_STAT = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(WatchListNode), "_3S.CoDeSys.WatchList.Resources.VarStatSmall.ico").ToBitmap();

		private static readonly Image IMAGE_VAR_TEMP = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(WatchListNode), "_3S.CoDeSys.WatchList.Resources.VarTempSmall.ico").ToBitmap();

		private static readonly Image IMAGE_VAR_GLOBAL = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(WatchListNode), "_3S.CoDeSys.WatchList.Resources.VarGlobalSmall.ico").ToBitmap();

		internal static IVariable FindVariableSimple(string stExpression)
		{
			IVariable result = null;
			try
			{
				result = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVariable(stExpression, true);
				return result;
			}
			catch
			{
				return result;
			}
		}

		internal static IVariable FindVariableAndOptionalDirectAddress(string stExpression, bool bLookForDirectAddressedStructMembers, string stParentDirectAddress, out string stDirectAddress)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Invalid comparison between Unknown and I4
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Invalid comparison between Unknown and I4
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Invalid comparison between Unknown and I4
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Invalid comparison between Unknown and I4
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Invalid comparison between Unknown and I4
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Invalid comparison between Unknown and I4
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Invalid comparison between Unknown and I4
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Invalid comparison between Unknown and I4
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Invalid comparison between Unknown and I4
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Invalid comparison between Unknown and I4
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Invalid comparison between Unknown and I4
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Invalid comparison between Unknown and I4
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Invalid comparison between Unknown and I4
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Invalid comparison between Unknown and I4
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Invalid comparison between Unknown and I4
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			stDirectAddress = string.Empty;
			int num = 0;
			IToken val = null;
			IToken val2 = null;
			IToken val3 = null;
			IToken val4 = null;
			IVariable val5 = null;
			IVariable val6 = null;
			try
			{
				IScanner val7 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stExpression, false, false, false, false);
				val7.AllowMultipleUnderlines=(true);
				if ((int)val7.GetNext(out val) != 13)
				{
					return null;
				}
				string identifier = val7.GetIdentifier(val);
				string stApplication = string.Empty;
				if ((int)val7.GetNext(out val2) == 15 && (int)val7.GetOperator(val2) == 162 && (int)val7.GetNext(out val) == 13)
				{
					stApplication = val7.GetIdentifier(val);
				}
				else
				{
					val7.SetPosition(val2);
				}
				if ((int)val7.GetNext(out val) != 15 || (int)val7.GetOperator(val) != 162)
				{
					return null;
				}
				Guid applicationGuid = Common.GetApplicationGuid(identifier, stApplication);
				ICompileContext referenceContext = Common.GetReferenceContext(applicationGuid);
				if (referenceContext == null)
				{
					return null;
				}
				IScope val8 = referenceContext.CreateGlobalIScope();
				Guid guid = Guid.Empty;
				if ((int)val7.GetNext(out val4) == 13 && (int)val7.GetNext(out val) == 15 && (int)val7.GetOperator(val) == 162)
				{
					string identifier2 = val7.GetIdentifier(val4);
					ISignature val9 = ((ICompileContextCommon)referenceContext).GetSignature(identifier2);
					if (val9 == null)
					{
						val7.SetPosition(val4);
					}
					else
					{
						guid = val9.ObjectGuid;
						val7.GetNext(out val2);
						if ((int)val2.Type != 21)
						{
							val7.GetNext(out val);
						}
						while (val2 != null && (int)val2.Type == 13 && (int)val.Type == 15 && (int)val7.GetOperator(val) == 162 && val9 != null)
						{
							val4 = val2;
							string identifier3 = val7.GetIdentifier(val2);
							if (val9.GetSubSignature(identifier3) != null)
							{
								guid = val9.GetSubSignature(identifier3).ObjectGuid;
								break;
							}
							if (val9[identifier3] != null)
							{
								val5 = val9[identifier3];
								if (val5 != null && val5.OriginalType is IUserdefType)
								{
									val9 = val8[((IUserdefType)(IUserdefType2)val5.OriginalType).SignatureId];
									if (val9 != null)
									{
										guid = val9.ObjectGuid;
										val3 = val2;
									}
								}
							}
							val7.GetNext(out val2);
							val7.GetNext(out val);
							if ((int)val.Type == 21 && val5 != null)
							{
								val2 = val3;
							}
						}
						if (val2 != null)
						{
							val7.SetPosition(val2);
						}
					}
				}
				else
				{
					val7.SetPosition(val4);
				}
				StringBuilder stringBuilder = new StringBuilder();
				while ((int)val7.GetNext(out val) != 21)
				{
					stringBuilder.Append(val7.GetTokenText(val));
				}
				stExpression = stringBuilder.ToString();
				if (!bLookForDirectAddressedStructMembers && stExpression.Contains("."))
				{
					stExpression = stExpression.Substring(stExpression.LastIndexOf(".") + 1);
				}
				ISignature signature = ((ICompileContextCommon)referenceContext).GetSignature(guid);
				if (signature == null)
				{
					return null;
				}
				val6 = signature[stExpression];
				if (val6 != null || !bLookForDirectAddressedStructMembers)
				{
					return val6;
				}
				ISignature[] array = null;
				if (guid == Guid.Empty)
				{
					if (val8 != null)
					{
						if (stExpression.EndsWith("]"))
						{
							IVariable[] array2 = null;
							string text = stExpression;
							int num2 = text.LastIndexOf("[");
							if (num2 > -1)
							{
								string text2 = text.Substring(num2 + 1);
								string s = text2.Remove(text2.Length - 1);
								int result = -1;
								if (int.TryParse(s, out result))
								{
									text = text.Remove(num2);
									ISignature[] array3 = null;
									array2 = val8.FindVariable(text, out array3);
									if (array3 != null && array3.Length == 1 && array3[0].OrgName.ToUpperInvariant() == "IOCONFIG_GLOBALS_MAPPING" && array2 != null && array2.Length == 1 && array2[0].Address != null)
									{
										int multiplicationFactor = GetMultiplicationFactor(array2[0].Address.Size, applicationGuid);
										if (array2[0].CompiledType.BaseType is IUserdefType2)
										{
											array = ((IScope2)((val8 is IScope3) ? val8 : null)).FindSignature(((IUserdefType2)array2[0].CompiledType.BaseType).NameExpression);
										}
										num = ((array == null || array.Length != 1) ? (array2[0].Address.Components[0] * multiplicationFactor + result * array2[0].CompiledType.BaseType.Size(val8)) : (array2[0].Address.Components[0] * multiplicationFactor + result * array[0].Size));
									}
									if (array2[0].Address.Incomplete)
									{
										stDirectAddress = "%" + GetVariableLocationPrefix(array2[0].Address.Location) + "B*";
									}
									else
									{
										stDirectAddress = "%" + GetVariableLocationPrefix(array2[0].Address.Location) + "B" + num;
									}
								}
							}
							return array2[0];
						}
						IVariable[] array4 = val8.FindVariable(stExpression, out array);
						if (array4 == null || array == null || array4.Length != array.Length)
						{
							return null;
						}
						for (int i = 0; i < array4.Length; i++)
						{
							if (array[i].OrgName.ToUpperInvariant() == "IOCONFIG_GLOBALS_MAPPING")
							{
								return array4[i];
							}
						}
					}
				}
				else if (stExpression.EndsWith("]"))
				{
					int num3 = stExpression.LastIndexOf("[");
					if (num3 > -1)
					{
						string text3 = stExpression.Substring(num3 + 1);
						string s2 = text3.Remove(text3.Length - 1);
						int result2 = -1;
						if (int.TryParse(s2, out result2))
						{
							stExpression = stExpression.Remove(num3);
							IVariable val10 = signature[stExpression];
							if (val10 != null && val10.Address != null && val10.OriginalType != null)
							{
								int result3 = 0;
								string text4 = ((object)val10.OriginalType).ToString().Substring(7);
								text4 = text4.Substring(0, text4.IndexOf("."));
								if (int.TryParse(text4, out result3))
								{
									result2 -= result3;
								}
								int multiplicationFactor2 = GetMultiplicationFactor(val10.Address.Size, applicationGuid);
								if (val10.CompiledType.BaseType is IUserdefType2)
								{
									array = ((IScope2)((val8 is IScope3) ? val8 : null)).FindSignature(((IUserdefType2)val10.CompiledType.BaseType).NameExpression);
								}
								num = ((array == null || array.Length != 1) ? (val10.Address.Components[0] * multiplicationFactor2 + result2 * val10.CompiledType.BaseType.Size(val8)) : (val10.Address.Components[0] * multiplicationFactor2 + result2 * array[0].Size));
								if (val10.Address.Incomplete)
								{
									stDirectAddress = "%" + GetVariableLocationPrefix(val10.Address.Location) + "B*";
								}
								else
								{
									stDirectAddress = "%" + GetVariableLocationPrefix(val10.Address.Location) + "B" + num;
								}
							}
							return val10;
						}
					}
				}
				else
				{
					int num4 = stExpression.LastIndexOf(".");
					if (num4 > -1 && val8 != null)
					{
						string text5 = stExpression.Substring(num4 + 1);
						if (val5 != null && val8 != null && val8 is IScope3 && val5.CompiledType is IUserdefType2 && ((IUserdefType2)val5.CompiledType).NameExpression is IVariableExpression)
						{
							array = ((IScope2)((val8 is IScope3) ? val8 : null)).FindSignature(((IUserdefType2)val5.CompiledType).NameExpression);
							if (array != null && array.Length == 1)
							{
								IVariable val11 = array[0][text5];
								if (val11 != null)
								{
									if (val11.Address == null && val11.DataLocation.IsRelativ)
									{
										int result4 = 0;
										if (val5.Address != null)
										{
											stDirectAddress = "%" + GetVariableLocationPrefix(val5.Address.Location);
											if (val5.Address.Incomplete)
											{
												stDirectAddress += "B*";
											}
											else
											{
												bool num5 = IsDeviceUsingBitWordAddressing(applicationGuid) && val11.DataLocation.IsBitLocation;
												int multiplicationFactor3 = GetMultiplicationFactor(val5.Address.Size, applicationGuid);
												int num6 = val11.DataLocation.Offset;
												byte b = val11.DataLocation.BitNr;
												if (num5)
												{
													num6 /= 2;
													b = ((val11.DataLocation.Offset % 2 == 0) ? b : ((byte)(b + 8)));
												}
												result4 = val5.Address.Components[0] * multiplicationFactor3 + num6;
												if (val11.DataLocation.IsBitLocation)
												{
													stDirectAddress = stDirectAddress + "X" + result4 + "." + b;
												}
												else
												{
													stDirectAddress = stDirectAddress + "B" + result4;
												}
											}
										}
										else if (stParentDirectAddress != string.Empty && int.TryParse(stParentDirectAddress.Substring(3), out result4))
										{
											result4 += val11.DataLocation.Offset;
											stDirectAddress = stParentDirectAddress.Substring(0, 2);
											if (val11.DataLocation.IsBitLocation)
											{
												stDirectAddress = stDirectAddress + "X" + result4 + "." + val11.DataLocation.BitNr;
											}
											else
											{
												stDirectAddress = stDirectAddress + "B" + result4;
											}
										}
									}
									return val11;
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			return null;
		}

		internal static string GetVariableLocationPrefix(DirectVariableLocation location)
		{
			switch (location)
			{
				case DirectVariableLocation.Input:
					return "I";
				case DirectVariableLocation.Output:
					return "Q";
				case DirectVariableLocation.Memory:
					return "M";
				default:
					return "?";
			}
		}

		private static int GetMultiplicationFactor(DirectVariableSize size, Guid guidApplication)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected I4, but got Unknown
			int num = 0;
			if (IsDeviceUsingByteAddressing(guidApplication))
			{
				return 1;
			}
			return (int)size switch
			{
				4 => 4, 
				5 => 8, 
				3 => 2, 
				_ => 1, 
			};
		}

		private static bool IsDeviceUsingByteAddressing(Guid guidApplication)
		{
			bool result = false;
			if (guidApplication != Guid.Empty)
			{
				IDeviceObject deviceObject = GetDeviceObject(guidApplication);
				if (deviceObject != null && APEnvironment.TargetSettingsMgr.Settings != null)
				{
					IDeviceIdentification val = deviceObject.DeviceIdentification;
					if (deviceObject is IDeviceObject5)
					{
						val = ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation;
					}
					ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val);
					result = LocalTargetSettings.ByteAddressing.GetBoolValue(targetSettingsById);
				}
			}
			return result;
		}

		private static bool IsDeviceUsingBitWordAddressing(Guid guidApplication)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			if (guidApplication != Guid.Empty)
			{
				result = ((IMemorySettings3)((ILMPouSet)APEnvironment.LMServiceProvider.PreCompileService.GetPreCompileSet(guidApplication)).MemorySettings).BitWordAddressing;
			}
			return result;
		}

		private static IDeviceObject GetDeviceObject(Guid guidApplication)
		{
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && guidApplication != Guid.Empty)
			{
				try
				{
					int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, guidApplication);
					while (metaObjectStub.ParentObjectGuid != Guid.Empty)
					{
						metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, metaObjectStub.ParentObjectGuid);
					}
					if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						IObject @object = APEnvironment.ObjectMgr.GetObjectToRead(handle, metaObjectStub.ObjectGuid).Object;
						return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
					}
				}
				catch
				{
				}
			}
			return null;
		}

		internal static ApplicationPrefixItem CreateAppPrefixItem(string stExpression, bool bIgnoreActiveApp, out string stDisplayExpression)
		{
			stDisplayExpression = stExpression;
			string text = string.Empty;
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication != Guid.Empty && APEnvironment.ObjectMgr.ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication))
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication);
				text = metaObjectStub.Name;
				while (metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
				}
				text = metaObjectStub.Name + "." + text;
			}
			if (string.IsNullOrEmpty(stExpression))
			{
				return new ApplicationPrefixItem(text);
			}
			if (stExpression.StartsWith(text + "."))
			{
				stDisplayExpression = stExpression.Replace(text + ".", "");
				return new ApplicationPrefixItem(text);
			}
			IList<string> list = Common.LookForAllDeviceAppPrefixes();
			if (list != null && list.Count > 0)
			{
				foreach (string item in list)
				{
					if (stExpression.StartsWith(item + "."))
					{
						stDisplayExpression = stExpression.Replace(item + ".", "");
						return new ApplicationPrefixItem(item);
					}
				}
			}
			if (!bIgnoreActiveApp && !string.IsNullOrEmpty(text))
			{
				return new ApplicationPrefixItem(text);
			}
			return new ApplicationPrefixItem(string.Empty);
		}

		internal static bool IsValidExpression(string stExpression, Guid explicitApplicationGuid, ref Guid explicitEditorGuid)
		{
			if (stExpression == null)
			{
				return false;
			}
			IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stExpression, false, false, false, false);
			val.AllowMultipleUnderlines=(true);
			IToken val2 = default(IToken);
			if ((int)val.GetNext(out val2) != 13)
			{
				return false;
			}
			string identifier = val.GetIdentifier(val2);
			string stApplication = string.Empty;
			IToken val3 = default(IToken);
			if ((int)val.GetNext(out val3) == 15 && (int)val.GetOperator(val3) == 162 && (int)val.GetNext(out val2) == 13)
			{
				stApplication = val.GetIdentifier(val2);
			}
			else
			{
				val.SetPosition(val3);
			}
			if ((int)val.GetNext(out val2) != 15 || (int)val.GetOperator(val2) != 162)
			{
				return false;
			}
			Guid guidApplication = Common.GetApplicationGuid(identifier, stApplication);
			if (explicitApplicationGuid != Guid.Empty)
			{
				guidApplication = explicitApplicationGuid;
			}
			ICompileContext referenceContext = Common.GetReferenceContext(guidApplication);
			if (referenceContext == null)
			{
				return false;
			}
			_ = Guid.Empty;
			IToken val4 = default(IToken);
			if ((int)val.GetNext(out val4) == 13 && (int)val.GetNext(out val2) == 15 && (int)val.GetOperator(val2) == 162)
			{
				string identifier2 = val.GetIdentifier(val4);
				ISignature signature = ((ICompileContextCommon)referenceContext).GetSignature(identifier2);
				if (signature == null)
				{
					val.SetPosition(val4);
				}
				else
				{
					if (signature.Name.ToUpperInvariant() == identifier2.ToUpperInvariant() && signature.FPDataLocation == null && (int)signature.POUType == 88 && stExpression.EndsWith(identifier2))
					{
						return false;
					}
					explicitEditorGuid = signature.ObjectGuid;
					if ((int)val.GetNext(out val3) == 13 && (int)val.GetNext(out val2) == 15 && (int)val.GetOperator(val2) == 162)
					{
						string identifier3 = val.GetIdentifier(val3);
						if (signature.GetSubSignature(identifier3) == null)
						{
							val.SetPosition(val3);
						}
						else
						{
							Guid objectGuid = signature.GetSubSignature(identifier3).ObjectGuid;
						}
					}
					else
					{
						val.SetPosition(val3);
					}
				}
			}
			else
			{
				val.SetPosition(val4);
			}
			IParser obj = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val);
			if (((IParser2)((obj is IParser2) ? obj : null)).ParseExpression() == null)
			{
				return false;
			}
			if (explicitEditorGuid == Guid.Empty)
			{
				val.SetPosition(val4);
				StringBuilder stringBuilder = new StringBuilder();
				if ((int)val.GetNext(out val2) != 21)
				{
					stringBuilder.Append(val.GetTokenText(val2));
				}
				string text = stringBuilder.ToString();
				IScope obj2 = referenceContext.CreateGlobalIScope();
				ISignature val5 = null;
				IVariable[] array = default(IVariable[]);
				ISignature[] array2 = default(ISignature[]);
				IScope val6 = default(IScope);
				obj2.FindDeclaration(text, out array, out array2, out val6);
				if (array2 != null && array2.Length != 0)
				{
					val5 = array2[0];
				}
				if (val5 != null)
				{
					explicitEditorGuid = val5.ObjectGuid;
				}
				return true;
			}
			IToken val7 = default(IToken);
			if ((int)val.GetNext(out val7) != 21)
			{
				return false;
			}
			return true;
		}

		internal static bool IsValidCastType(IExpression watchExpression)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Invalid comparison between I4 and Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			ICastExpression val = (ICastExpression)(object)((watchExpression is ICastExpression) ? watchExpression : null);
			if (val != null)
			{
				ICompiledType explicitelySpecifiedType = val.ExplicitelySpecifiedType;
				IPointerType val2 = (IPointerType)(object)((explicitelySpecifiedType is IPointerType) ? explicitelySpecifiedType : null);
				if (val2 != null && val2.Base != null && 28 == (int)val2.Base.Class)
				{
					IUserdefType val3 = (IUserdefType)val2.Base;
					return -1 != val3.SignatureId;
				}
			}
			return true;
		}

		internal static bool TryToConvertBinHexValue(string stValue, IOnlineVarRef onlineVarRef, TypeClass typeClass, out string stConvertedValue)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected I4, but got Unknown
			object obj = null;
			stConvertedValue = string.Empty;
			IConverterFromIEC converterFromIEC = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
			try
			{
				switch ((int)typeClass - 6)
				{
				case 1:
				{
					converterFromIEC.GetInteger(stValue, out obj, out typeClass);
					int num3 = BitConverter.ToInt16(BitConverter.GetBytes((ulong)obj), 0);
					stConvertedValue = num3.ToString();
					if (onlineVarRef != null)
					{
						onlineVarRef.PreparedValue=((object)num3);
					}
					break;
				}
				case 2:
				{
					converterFromIEC.GetInteger(stValue, out obj, out typeClass);
					int num = BitConverter.ToInt32(BitConverter.GetBytes((ulong)obj), 0);
					stConvertedValue = num.ToString();
					if (onlineVarRef != null)
					{
						onlineVarRef.PreparedValue=((object)num);
					}
					break;
				}
				case 3:
				{
					converterFromIEC.GetInteger(stValue, out obj, out typeClass);
					long num2 = BitConverter.ToInt64(BitConverter.GetBytes((ulong)obj), 0);
					if (onlineVarRef != null)
					{
						onlineVarRef.PreparedValue=((object)num2);
					}
					stConvertedValue = num2.ToString();
					break;
				}
				case 0:
				{
					converterFromIEC.GetInteger(stValue, out obj, out typeClass);
					sbyte b = (sbyte)BitConverter.GetBytes((ulong)obj)[0];
					stConvertedValue = b.ToString();
					if (onlineVarRef != null)
					{
						onlineVarRef.PreparedValue=((object)b);
					}
					break;
				}
				default:
					return false;
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		internal static Guid DetermineApplicationGuidIfNecessary(Guid gdApplication)
		{
			if (Guid.Empty == gdApplication && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication != Guid.Empty && APEnvironment.ObjectMgr.ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication))
			{
				gdApplication = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication).ObjectGuid;
			}
			return gdApplication;
		}

		internal static Image DetermineImageForExpression(string stExpression)
		{
			Image result = null;
			try
			{
				IVariable variable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVariable(stExpression, true);
				if (variable != null)
				{
					if (variable.GetFlag((VarFlag)2))
					{
						return IMAGE_VAR_INPUT;
					}
					if (variable.GetFlag((VarFlag)4))
					{
						return IMAGE_VAR_OUTPUT;
					}
					if (variable.GetFlag((VarFlag)8))
					{
						return IMAGE_VAR_IN_OUT;
					}
					if (variable.GetFlag((VarFlag)134217728))
					{
						return IMAGE_VAR_STAT;
					}
					if (variable.GetFlag((VarFlag)2097152))
					{
						return IMAGE_VAR_TEMP;
					}
					if (variable.GetFlag((VarFlag)8192))
					{
						return IMAGE_VAR_GLOBAL;
					}
					return IMAGE_VAR;
				}
				return result;
			}
			catch
			{
				return IMAGE_VAR;
			}
		}

		internal static IConverterToIEC GetEffectiveConverter(IConverterToIEC globalConverter, IVarRef varRef)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Invalid comparison between Unknown and I4
			try
			{
				if (varRef == null)
				{
					return globalConverter;
				}
				IExpression watchExpression = varRef.WatchExpression;
				IExpression3 val = (IExpression3)(object)((watchExpression is IExpression3) ? watchExpression : null);
				if (val == null)
				{
					return globalConverter;
				}
				ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(varRef.ApplicationGuid);
				if (compileContext == null)
				{
					return globalConverter;
				}
				IScope val2 = compileContext.CreateGlobalIScope();
				if (val2 == null)
				{
					return globalConverter;
				}
				IVariable variable = val.GetVariable(val2);
				if (variable == null)
				{
					return globalConverter;
				}
				string attributeValue = variable.GetAttributeValue("displaymode");
				if (attributeValue == null && variable.OriginalType != null && (int)((IType)variable.OriginalType).Class == 28)
				{
					ICompiledType originalType = variable.OriginalType;
					IUserdefType val3 = (IUserdefType)(object)((originalType is IUserdefType) ? originalType : null);
					ISignature val4 = val2[val3.SignatureId];
					if (val4 != null && val4.GetFlag((SignatureFlag)4))
					{
						attributeValue = val4.GetAttributeValue("displaymode");
					}
				}
				if (attributeValue == null)
				{
					return globalConverter;
				}
				switch (attributeValue.ToLowerInvariant())
				{
				case "bin":
				case "binary":
					return WatchListModel.s_binaryConverter;
				case "dec":
				case "decimal":
					return WatchListModel.s_decimalConverter;
				case "hex":
				case "hexadecimal":
					return WatchListModel.s_hexadecimalConverter;
				default:
					return globalConverter;
				}
			}
			catch
			{
				return globalConverter;
			}
		}

		private static bool ContainsControlCharacters(string stText)
		{
			if (stText == null)
			{
				return false;
			}
			char[] array = stText.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (char.IsControl(array[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static string GetTextToRender(string stText, TypeClass typeClass)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between I4 and Unknown
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between I4 and Unknown
			if (stText == null)
			{
				return null;
			}
			if (16 != (int)typeClass && 17 != (int)typeClass)
			{
				return stText;
			}
			if (!ContainsControlCharacters(stText))
			{
				return stText;
			}
			char[] array = stText.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			char[] array2 = array;
			foreach (char c in array2)
			{
				if (char.IsControl(c))
				{
					stringBuilder.AppendFormat("${0:X00}", (int)c);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			stText = stringBuilder.ToString();
			return stText;
		}
	}
}
