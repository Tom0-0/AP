#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FBIoChannels
	{
		internal static readonly string AttributeIoFunctionBlock = "io_function_block";

		internal static readonly string AttributeIoFunctionMapping = "io_function_block_mapping";

		internal const string DEVICENAME = "$(DeviceName)";

		internal static LDictionary<string, ICompiledType> _dictCompiledTypes = new LDictionary<string, ICompiledType>();

		private static string[] _numericTypes = new string[12]
		{
			"BYTE", "WORD", "DWORD", "LWORD", "SINT", "INT", "DINT", "LINT", "USINT", "UINT",
			"UDINT", "ULINT"
		};

		internal static void RemoveIECObject(IParameterSetProvider paramProvider, string stFBName)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			IIoProvider val = paramProvider?.GetIoProvider(bToModify: true);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				IDriverInfo driverInfo = val.DriverInfo;
				obj = ((driverInfo != null) ? driverInfo.RequiredLibs : null);
			}
			IRequiredLibsList2 val2 = (IRequiredLibsList2)((obj is IRequiredLibsList2) ? obj : null);
			if (val2 == null)
			{
				return;
			}
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				IMetaObject metaObject = val.GetMetaObject();
				obj2 = ((metaObject != null) ? metaObject.Name : null);
			}
			string text = (string)obj2;
			if (!string.IsNullOrEmpty(text))
			{
				stFBName = stFBName.Replace("$(DeviceName)", text);
			}
			foreach (IRequiredLib4 item in (IEnumerable)val2)
			{
				IRequiredLib4 val3 = item;
				foreach (IFbInstance item2 in (IEnumerable)((IRequiredLib)val3).FbInstances)
				{
					IFbInstance val4 = item2;
					if (string.Compare(val4.Instance.Variable, stFBName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						IFbInstanceList fbInstances = ((IRequiredLib)val3).FbInstances;
						((IFbInstanceList2)((fbInstances is IFbInstanceList3) ? fbInstances : null)).RemoveFbInstance(val4);
						if (((ICollection)((IRequiredLib)val3).FbInstances).Count == 0)
						{
							val2.RemoveRequiredLib((IRequiredLib)(object)val3);
						}
						return;
					}
				}
			}
		}

		internal static void AddIECObject(IParameterSetProvider paramProvider, int nHandle, Guid guidApp, string stLibName, ref string stFBName, string stFBType, bool bIsOutput)
		{
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Expected O, but got Unknown
			if (string.IsNullOrEmpty(stFBName))
			{
				return;
			}
			IList<ILibManItem> toplevelLibs = LibH.GetToplevelLibs(nHandle, guidApp);
			IIoProvider val = paramProvider?.GetIoProvider(bToModify: true);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				IDriverInfo driverInfo = val.DriverInfo;
				obj = ((driverInfo != null) ? driverInfo.RequiredLibs : null);
			}
			IRequiredLibsList2 val2 = (IRequiredLibsList2)((obj is IRequiredLibsList2) ? obj : null);
			if (val2 == null || val == null)
			{
				return;
			}
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				IMetaObject metaObject = val.GetMetaObject();
				obj2 = ((metaObject != null) ? metaObject.Name : null);
			}
			string text = (string)obj2;
			if (!string.IsNullOrEmpty(text))
			{
				stFBName = stFBName.Replace(text, "$(DeviceName)");
			}
			string text2 = string.Empty;
			if (!string.IsNullOrEmpty(stLibName) && toplevelLibs != null)
			{
				foreach (ILibManItem item in toplevelLibs)
				{
					IManagedLibrary managedLibFromLibManItem = LibH.GetManagedLibFromLibManItem(item);
					if (managedLibFromLibManItem != null && string.Compare(managedLibFromLibManItem.DisplayName, stLibName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						string text3 = string.Empty;
						if (item is IPlaceholderLibManItem)
						{
							text3 = ((IPlaceholderLibManItem)((item is IPlaceholderLibManItem) ? item : null)).PlaceholderName;
						}
						text2 = managedLibFromLibManItem.DefaultNamespace;
						val2.AddRequiredLib(managedLibFromLibManItem.Title, managedLibFromLibManItem.Company, managedLibFromLibManItem.Version.ToString(), text2, text3);
						break;
					}
				}
			}
			else
			{
				val2.AddRequiredLib(string.Empty, string.Empty, string.Empty, text2, string.Empty);
			}
			if (val2 != null)
			{
				int num = 1;
				string text4 = stFBName;
				bool flag = false;
				do
				{
					flag = false;
					foreach (IRequiredLib4 item2 in (IEnumerable)val2)
					{
						IFbInstanceList fbInstances = ((IRequiredLib)item2).FbInstances;
						IFbInstanceList3 val3 = (IFbInstanceList3)(object)((fbInstances is IFbInstanceList3) ? fbInstances : null);
						if (val3 == null)
						{
							continue;
						}
						foreach (IFbInstance5 item3 in (IEnumerable)val3)
						{
							if (string.Compare(item3.BaseName, stFBName, StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								stFBName = text4 + "_" + num++;
								flag = true;
							}
						}
					}
				}
				while (flag);
			}
			foreach (IRequiredLib4 item4 in (IEnumerable)val2)
			{
				IRequiredLib4 val4 = item4;
				if (string.Compare(val4.Identifier, text2, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					continue;
				}
				IFbInstanceList fbInstances2 = ((IRequiredLib)val4).FbInstances;
				IFbInstanceList3 val5 = (IFbInstanceList3)(object)((fbInstances2 is IFbInstanceList3) ? fbInstances2 : null);
				if (val5 == null)
				{
					break;
				}
				IFbInstance val6;
				for (val6 = null; val6 == null; val6 = val5.AddFbInstance(stFBType, stFBName, val))
				{
				}
				if (val6 != null)
				{
					if (!bIsOutput)
					{
						ICyclicCallsList cyclicCalls = val6.CyclicCalls;
						((ICyclicCallsList2)((cyclicCalls is ICyclicCallsList2) ? cyclicCalls : null)).AddCyclicCall(string.Empty, "afterReadInputs", "#buscycletask");
					}
					else
					{
						ICyclicCallsList cyclicCalls2 = val6.CyclicCalls;
						((ICyclicCallsList2)((cyclicCalls2 is ICyclicCallsList2) ? cyclicCalls2 : null)).AddCyclicCall(string.Empty, "beforeWriteOutputs", "#buscycletask");
					}
				}
				break;
			}
		}

		internal static ISignature FindIoFbs(int nProjectHandle, Guid gdApp, string stFB, out string stType, out bool bIsOutput)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			stType = string.Empty;
			bIsOutput = false;
			new LDictionary<ISignature, IVariable>();
			IProject projectFromHandle = LibH.GetProjectFromHandle(nProjectHandle);
			foreach (IProject item in Fun.Join<IProject>(new IEnumerable<IProject>[2]
			{
				EnumMaybe<IProject>(projectFromHandle),
				LibH.GetVisibleLibraries(nProjectHandle, gdApp)
			}))
			{
				IEnumerable<ISignature2> enumerable = FindAllFBs(item, gdApp);
				if (Fun.IsEmpty<ISignature2>(enumerable))
				{
					continue;
				}
				foreach (ISignature2 item2 in enumerable)
				{
					if (string.Compare(((ISignature)item2).OrgName, stFB, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						continue;
					}
					IVariable[] inputs = ((ISignature)item2).Inputs;
					TypeClass @class;
					foreach (IVariable val in inputs)
					{
						if (val.HasAttribute(AttributeIoFunctionMapping))
						{
							@class = val.Type.Class;
							stType = @class.ToString();
							break;
						}
					}
					inputs = ((ISignature)item2).Outputs;
					foreach (IVariable val2 in inputs)
					{
						if (val2.HasAttribute(AttributeIoFunctionMapping))
						{
							bIsOutput = true;
							@class = val2.Type.Class;
							stType = @class.ToString();
							break;
						}
					}
					return (ISignature)(object)item2;
				}
			}
			return null;
		}

		internal static void CheckIoFunctionBlock(LDictionary<ISignature, IVariable> diResult, IEnumerable<ISignature> sigs, string stType, bool bIsOutput)
		{
			if (Fun.IsEmpty<ISignature>(sigs))
			{
				return;
			}
			foreach (ISignature sig in sigs)
			{
				bool flag = false;
				IVariable val = null;
				IVariable[] array = null;
				if (!string.IsNullOrEmpty(stType))
				{
					array = ((!bIsOutput) ? sig.Inputs : sig.Outputs);
				}
				else
				{
					array = (IVariable[])(object)new IVariable[sig.Outputs.Length + sig.Inputs.Length];
					sig.Outputs.CopyTo(array, 0);
					sig.Inputs.CopyTo(array, sig.Outputs.Length);
				}
				if (array != null)
				{
					IVariable[] array2 = array;
					foreach (IVariable val2 in array2)
					{
						if (val2.HasAttribute(AttributeIoFunctionMapping) && (string.IsNullOrEmpty(stType) || CheckForCompatibleType(((object)val2.Type).ToString(), stType)))
						{
							val = val2;
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					diResult.Add(sig, val);
				}
			}
		}

		internal static LDictionary<ISignature, IVariable> GetIoFbs(int nProjectHandle, Guid gdApp, string stType, bool bIsOutput)
		{
			LDictionary<ISignature, IVariable> val = new LDictionary<ISignature, IVariable>();
			IProject projectFromHandle = LibH.GetProjectFromHandle(nProjectHandle);
			IEnumerable<IProject> enumerable = Fun.Join<IProject>(new IEnumerable<IProject>[2]
			{
				EnumMaybe<IProject>(projectFromHandle),
				LibH.GetVisibleLibraries(nProjectHandle, gdApp)
			});
			try
			{
				foreach (IProject item in enumerable)
				{
					IEnumerable<ISignature2> sigs = FindAllFBs(item, gdApp);
					CheckIoFunctionBlock(val, (IEnumerable<ISignature>)sigs, stType, bIsOutput);
					if (item.Primary)
					{
						IEnumerable<ISignature2> sigs2 = FindAllFBs(item, Guid.Empty);
						CheckIoFunctionBlock(val, (IEnumerable<ISignature>)sigs2, stType, bIsOutput);
					}
				}
				return val;
			}
			catch
			{
				val.Clear();
				return val;
			}
		}

		private static IEnumerable<T> EnumMaybe<T>(T t) where T : class
		{
			if (t != null)
			{
				yield return t;
			}
		}

		private static IEnumerable<ISignature2> FindAllFBs(IProject proj, Guid gdApp)
		{
			IPreCompileContext val = (proj.Primary ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(gdApp) : APEnvironment.LanguageModelMgr.GetLibraryPrecompileContext(proj.Id));
			Debug.Assert(val != null);
			ISignature[] allSignatures = ((ICompileContextCommon)val).AllSignatures;
			foreach (ISignature val2 in allSignatures)
			{
				if (IsFBSignature(val2))
				{
					yield return (ISignature2)val2;
				}
			}
		}

		public static bool IsFBSignature(ISignature sig)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			ISignature2 val = (ISignature2)(object)((sig is ISignature2) ? sig : null);
			if (val != null && (int)((ISignature)val).POUType == 88)
			{
				return ((ISignature)val).HasAttribute(AttributeIoFunctionBlock);
			}
			return false;
		}

		private static ICompiledType ParseType(string stType)
		{
			ICompiledType val = null;
			if (!_dictCompiledTypes.TryGetValue(stType, out val))
			{
				IScanner val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stType, false, false, false, false);
				val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val2).ParseTypeDeclaration();
				_dictCompiledTypes[stType]= val;
			}
			return val;
		}

		private static bool CheckForCompatibleType(string stType, string stVarType)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Invalid comparison between Unknown and I4
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Invalid comparison between Unknown and I4
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Invalid comparison between Unknown and I4
			if (string.Compare(stVarType, "BIT", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				stVarType = "BOOL";
			}
			ICompiledType val = ParseType(stType);
			if (val != null && val.BaseType != null && ((int)((IType)val).Class == 26 || (int)((IType)val).Class == 24))
			{
				stType = ((object)val.BaseType).ToString();
			}
			ICompiledType val2 = ParseType(stVarType);
			if (val2 != null && val2.BaseType != null && ((int)((IType)val2).Class == 26 || (int)((IType)val2).Class == 24))
			{
				stVarType = ((object)val2.BaseType).ToString();
			}
			if (val != null && val.BaseType != null && val2 != null && val2.BaseType != null && val.BaseType == val2.BaseType)
			{
				return true;
			}
			stType = stType.ToUpperInvariant();
			stVarType = stVarType.ToUpperInvariant();
			if (Array.IndexOf(_numericTypes, stType) >= 0 && Array.IndexOf(_numericTypes, stVarType) >= 0)
			{
				return true;
			}
			bool flag = stVarType.StartsWith("SAFE");
			bool flag2 = stType.StartsWith("SAFE");
			if (flag || flag2)
			{
				if (flag)
				{
					stVarType = stVarType.Substring(4);
				}
				if (flag2)
				{
					stType = stType.Substring(4);
				}
				if (flag && flag2 && Array.IndexOf(_numericTypes, stType) >= 0 && Array.IndexOf(_numericTypes, stVarType) >= 0)
				{
					return true;
				}
				if (Array.IndexOf(_numericTypes, stType) >= 0)
				{
					Array.IndexOf(_numericTypes, stVarType);
					_ = 0;
					return false;
				}
			}
			return false;
		}
	}
}
