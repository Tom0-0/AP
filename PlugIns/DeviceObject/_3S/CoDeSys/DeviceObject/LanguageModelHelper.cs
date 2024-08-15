#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Compression.Checksums;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject.LogicalDevice;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.Utilities;
using static _3S.CoDeSys.Utilities.GuidHelper;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class LanguageModelHelper
	{
		internal class DiagnosisInstance
		{
			private FBInstance _Instance;

			private int _iModuleIndex;

			private Guid _objectGuid;

			private Guid _parentGuid;

			private IIoProvider _ioprovider;

			private RequiredLib _reqLib;

			public RequiredLib RequiredLib => _reqLib;

			public int ModuleIndex => _iModuleIndex;

			public Guid ObjectGuid => _objectGuid;

			public Guid ParentGuid => _parentGuid;

			public FBInstance Instance
			{
				get
				{
					return _Instance;
				}
				set
				{
					_Instance = value;
				}
			}

			public IIoProvider IoProvider
			{
				get
				{
					return _ioprovider;
				}
				set
				{
					_ioprovider = value;
				}
			}

			public DiagnosisInstance(int iModuleIndex, Guid objectGuid, Guid parentGuid, IIoProvider ioprovider)
			{
				_iModuleIndex = iModuleIndex;
				_objectGuid = objectGuid;
				_parentGuid = parentGuid;
				_Instance = new FBInstance();
				_ioprovider = ioprovider;
			}

			public void SetLibrary(ILibManItem item)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Expected O, but got Unknown
				string text = string.Empty;
				string placeHolderLib = string.Empty;
				if (item is IPlaceholderLibManItem)
				{
					IPlaceholderLibManItem val = (IPlaceholderLibManItem)item;
					if (val.EffectiveResolution != null)
					{
						text = val.EffectiveResolution.DisplayName;
						placeHolderLib = val.PlaceholderName;
					}
				}
				else
				{
					text = item.Name;
				}
				string libName = default(string);
				string version = default(string);
				string vendor = default(string);
				if (((ILibraryLoader3)APEnvironment.LibraryLoader).ParseDisplayName(text, out libName, out version, out vendor))
				{
					RequiredLib requiredLib = new RequiredLib();
					requiredLib.LibName = libName;
					requiredLib.Version = version;
					requiredLib.Identifier = item.Namespace;
					requiredLib.Vendor = vendor;
					requiredLib.PlaceHolderLib = placeHolderLib;
					requiredLib.LoadAsSystemLibrary = true;
					requiredLib.IsDiagnosisLib = true;
					_reqLib = requiredLib;
				}
			}
		}

		public const string IOLIB_NAME = "IoStandard";

		public const string SYSMEMLIB_NAME = "SysMem";

		public static string PRAGMA_ATTRIBUTE_HIDE = "{attribute 'hide'}";

		private const string SUFFIX_BUSCYLCE_EXTERNAL_EVENT = "_BusCycleExternalEvent";

		private const string BUSCYCLE_EXTERNAL_EVENT_FB_TYPE = "BusCycleExternalEventTrigger";

		private const string INSTANCE_SUFFIX = "_INSTANCE";

		private static long _lAdditionaParam1 = 1879048208L;

		private static long _lAdditionaParam2 = 1879048209L;

		private static long _lAdditionaParam3 = 1879048210L;

		private static readonly Guid namespaceGuid = new Guid("{845148E3-DE23-41A1-B2CE-5BD01A179712}");

		internal static LDictionary<Guid, DiagnosisInstance> _dictDiagnosisInstances = new LDictionary<Guid, DiagnosisInstance>();

		private static bool _bLastDiagnosis = true;

		internal static readonly string ATTRIBUTEIGNORE = "skip-warning-read-access";

		public static string GetExternalEventName(string taskName)
		{
			return string.Format("{0}{1}", taskName, "_BusCycleExternalEvent").ToUpperInvariant();
		}

		public static string GetExternalEventFBInstanceName(string taskName, out string eventName)
		{
			eventName = GetExternalEventName(taskName);
			return string.Format("{0}{1}", eventName, "_INSTANCE");
		}

		public static string GetExternalFBInstanceDeclaration(string taskName)
		{
			string eventName;
			string externalEventFBInstanceName = GetExternalEventFBInstanceName(taskName, out eventName);
			return string.Format("{0} : {1}('{2}');\r\n", externalEventFBInstanceName, "BusCycleExternalEventTrigger", eventName);
		}

		public static string GetIoBaseName(IDeviceObject device)
		{
			return "Io_" + ((IObject)device).MetaObject.ObjectGuid.ToString()
				.Replace('-', '_');
		}

		internal static Guid FindGuidStructure(string stTypeName, Guid appGuid)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			IPreCompileContext2 val = null;
			if (appGuid == Guid.Empty)
			{
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject != null)
				{
					val = (IPreCompileContext2)((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetPrecompileContext(primaryProject.ActiveApplication);
				}
			}
			else
			{
				val = (IPreCompileContext2)((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetPrecompileContext(appGuid);
			}
			if (val != null)
			{
				try
				{
					ISignature[] array = ((IPreCompileContext)val).FindSignature(stTypeName.ToUpperInvariant());
					if (array.Length != 0)
					{
						return array[0].ObjectGuid;
					}
				}
				catch
				{
				}
			}
			return CreateDeterministicGuid(appGuid, stTypeName);
		}

		private static void AddIndexToParameterSet(ParameterSet parameterSet, long lParamId, ref uint nNumParams)
		{
			parameterSet.ParamIdToIndex[lParamId]= nNumParams;
			nNumParams++;
		}

		private static void AddAdditionalParameter1(ParameterSet parameterSet, LanguageModelContainer localContainer, ref uint nNumParams, string stStructBase)
		{
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			if (parameterSet.Contains(_lAdditionaParam1) || parameterSet.Device == null)
			{
				return;
			}
			string name = ((IObject)parameterSet.Device).MetaObject.Name;
			string text = "p" + nNumParams.ToString("x");
			ILanguageModelBuilder lmBuilder = localContainer.lmBuilder;
			ILanguageModelBuilder3 val = (ILanguageModelBuilder3)(object)((lmBuilder is ILanguageModelBuilder3) ? lmBuilder : null);
			TypeClass val13;
			if (localContainer.lmNew != null && val != null)
			{
				IVariableExpression val2 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, text);
				ILiteralExpression val3 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, name);
				IAssignmentExpression item = ((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val2, (IExpression)(object)val3);
				localContainer.assInitValues.Add(item);
				IVariableExpression2 val4 = ((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, stStructBase);
				IVariableExpression2 val5 = ((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, text);
				ICompoAccessExpression val6 = ((ILanguageModelBuilder)val).CreateCompoAccessExpression((IExprementPosition)null, (IExpression)(object)val4, val5);
				List<IAssignmentExpression> list = new List<IAssignmentExpression>();
				IVariableExpression val7 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "dwParameterId");
				IVariableExpression val8 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "dwValue");
				IVariableExpression val9 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "wType");
				IVariableExpression val10 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "wLen");
				IVariableExpression val11 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "dwFlags");
				ILiteralExpression val12 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, (ulong)_lAdditionaParam1, (TypeClass)4);
				val13 = (TypeClass)16;
				ILiteralExpression val14 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, (ulong)Types.GetTypeId(val13.ToString(), ushort.MaxValue), (TypeClass)3);
				val13 = (TypeClass)16;
				ILiteralExpression val15 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, (ulong)Types.GetBitSize(val13.ToString()), (TypeClass)3);
				ILiteralExpression val16 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, 50uL, (TypeClass)4);
				IOperatorExpression val17 = ((ILanguageModelBuilder)val).CreateOperatorExpression((IExprementPosition)null, (Operator)33, (IExpression)(object)val6);
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val7, (IExpression)(object)val12));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val8, (IExpression)(object)val17));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val9, (IExpression)(object)val14));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val10, (IExpression)(object)val15));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val11, (IExpression)(object)val16));
				localContainer.struinitParameters.Add(((ILanguageModelBuilder)val).CreateStructureInitialisation((IExprementPosition)null, list));
				localContainer.seqParamStruct.AddStatement(val.CreatePragmaStatement2((IExprementPosition)null, "attribute 'noinit'"));
				ICompiledType val18 = ((ILanguageModelBuilder)val).CreateSimpleType((TypeClass)16);
				IVariableDeclarationStatement val19 = ((ILanguageModelBuilder)val).CreateSimpleVariableDeclarationStatement((IExprementPosition)null, text, val18);
				localContainer.seqParamStruct.AddStatement((IStatement)(object)val19);
			}
			else
			{
				if (localContainer.sbInitValues.Length > 0)
				{
					localContainer.sbInitValues.Append(",\n");
				}
				localContainer.sbInitValues.AppendFormat("{0}:='{1}'", text, name);
				string text2 = stStructBase + "." + text;
				if (localContainer.sbParameters.Length > 0)
				{
					localContainer.sbParameters.Append(",\n");
				}
				StringBuilder sbParameters = localContainer.sbParameters;
				object[] obj = new object[4] { _lAdditionaParam1, text2, null, null };
				val13 = (TypeClass)16;
				obj[2] = Types.GetTypeId(val13.ToString(), ushort.MaxValue);
				val13 = (TypeClass)16;
				obj[3] = Types.GetBitSize(val13.ToString());
				sbParameters.AppendFormat("(dwParameterId:={0}, dwValue:=ADR({1}), wType:={2}, wLen := {3}, dwFlags:=50 )", obj);
				localContainer.sbParamStruct.AppendFormat("{{attribute 'noinit'}}{0}:{1};\n", text, "STRING");
			}
			AddIndexToParameterSet(parameterSet, _lAdditionaParam1, ref nNumParams);
		}

		private static void AddAdditionalParameter2(ParameterSet parameterSet, LanguageModelContainer localContainer, ref uint nNumParams, string stStructBase)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			if (parameterSet.Contains(_lAdditionaParam2) || parameterSet.Device == null)
			{
				return;
			}
			int num = 0;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)0))
			{
				Crc32 val = new Crc32();
				Encoding unicode = Encoding.Unicode;
				IDeviceObject2 device = parameterSet.Device;
				IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((device is IDeviceObject5) ? device : null)).DeviceIdentificationNoSimulation;
				if (deviceIdentificationNoSimulation.Id != null)
				{
					val.Update(deviceIdentificationNoSimulation.Type);
					byte[] bytes = unicode.GetBytes(deviceIdentificationNoSimulation.Id);
					val.Update(bytes);
				}
				if (deviceIdentificationNoSimulation is ModuleIdentification && (deviceIdentificationNoSimulation as ModuleIdentification).ModuleId != null)
				{
					byte[] bytes2 = unicode.GetBytes((deviceIdentificationNoSimulation as ModuleIdentification).ModuleId);
					val.Update(bytes2);
				}
				num = (int)val.Value;
			}
			else
			{
				IDeviceObject2 device2 = parameterSet.Device;
				IDeviceIdentification deviceIdentificationNoSimulation2 = ((IDeviceObject5)((device2 is IDeviceObject5) ? device2 : null)).DeviceIdentificationNoSimulation;
				if (deviceIdentificationNoSimulation2.Id != null)
				{
					num = deviceIdentificationNoSimulation2.Type.GetHashCode() ^ GetHashStringCode(deviceIdentificationNoSimulation2.Id);
				}
				if (deviceIdentificationNoSimulation2 is ModuleIdentification && (deviceIdentificationNoSimulation2 as ModuleIdentification).ModuleId != null)
				{
					num ^= GetHashStringCode((deviceIdentificationNoSimulation2 as ModuleIdentification).ModuleId);
				}
			}
			string text = "p" + nNumParams.ToString("x");
			ILanguageModelBuilder lmBuilder = localContainer.lmBuilder;
			ILanguageModelBuilder3 val2 = (ILanguageModelBuilder3)(object)((lmBuilder is ILanguageModelBuilder3) ? lmBuilder : null);
			TypeClass val14;
			if (localContainer.lmNew != null && val2 != null)
			{
				IVariableExpression val3 = (IVariableExpression)(object)((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, text);
				ILiteralExpression val4 = ((ILanguageModelBuilder)val2).CreateLiteralExpression((IExprementPosition)null, (long)num, (TypeClass)8);
				IAssignmentExpression item = ((ILanguageModelBuilder)val2).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val3, (IExpression)(object)val4);
				localContainer.assInitValues.Add(item);
				IVariableExpression2 val5 = ((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, stStructBase);
				IVariableExpression2 val6 = ((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, text);
				ICompoAccessExpression val7 = ((ILanguageModelBuilder)val2).CreateCompoAccessExpression((IExprementPosition)null, (IExpression)(object)val5, val6);
				List<IAssignmentExpression> list = new List<IAssignmentExpression>();
				IVariableExpression val8 = (IVariableExpression)(object)((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, "dwParameterId");
				IVariableExpression val9 = (IVariableExpression)(object)((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, "dwValue");
				IVariableExpression val10 = (IVariableExpression)(object)((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, "wType");
				IVariableExpression val11 = (IVariableExpression)(object)((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, "wLen");
				IVariableExpression val12 = (IVariableExpression)(object)((ILanguageModelBuilder)val2).CreateVariableExpression((IExprementPosition)null, "dwFlags");
				ILiteralExpression val13 = ((ILanguageModelBuilder)val2).CreateLiteralExpression((IExprementPosition)null, (ulong)_lAdditionaParam2, (TypeClass)4);
				val14 = (TypeClass)8;
				ILiteralExpression val15 = ((ILanguageModelBuilder)val2).CreateLiteralExpression((IExprementPosition)null, (ulong)Types.GetTypeId(val14.ToString(), ushort.MaxValue), (TypeClass)3);
				val14 = (TypeClass)8;
				ILiteralExpression val16 = ((ILanguageModelBuilder)val2).CreateLiteralExpression((IExprementPosition)null, (ulong)Types.GetBitSize(val14.ToString()), (TypeClass)3);
				ILiteralExpression val17 = ((ILanguageModelBuilder)val2).CreateLiteralExpression((IExprementPosition)null, 50uL, (TypeClass)4);
				IOperatorExpression val18 = ((ILanguageModelBuilder)val2).CreateOperatorExpression((IExprementPosition)null, (Operator)33, (IExpression)(object)val7);
				list.Add(((ILanguageModelBuilder)val2).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val8, (IExpression)(object)val13));
				list.Add(((ILanguageModelBuilder)val2).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val9, (IExpression)(object)val18));
				list.Add(((ILanguageModelBuilder)val2).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val10, (IExpression)(object)val15));
				list.Add(((ILanguageModelBuilder)val2).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val11, (IExpression)(object)val16));
				list.Add(((ILanguageModelBuilder)val2).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val12, (IExpression)(object)val17));
				localContainer.struinitParameters.Add(((ILanguageModelBuilder)val2).CreateStructureInitialisation((IExprementPosition)null, list));
				localContainer.seqParamStruct.AddStatement(val2.CreatePragmaStatement2((IExprementPosition)null, "attribute 'noinit'"));
				ICompiledType val19 = ((ILanguageModelBuilder)val2).CreateSimpleType((TypeClass)8);
				IVariableDeclarationStatement val20 = ((ILanguageModelBuilder)val2).CreateSimpleVariableDeclarationStatement((IExprementPosition)null, text, val19);
				localContainer.seqParamStruct.AddStatement((IStatement)(object)val20);
			}
			else
			{
				if (localContainer.sbInitValues.Length > 0)
				{
					localContainer.sbInitValues.Append(",\n");
				}
				localContainer.sbInitValues.AppendFormat("{0}:={1}", text, num.ToString());
				string text2 = stStructBase + "." + text;
				if (localContainer.sbParameters.Length > 0)
				{
					localContainer.sbParameters.Append(",\n");
				}
				StringBuilder sbParameters = localContainer.sbParameters;
				object[] obj = new object[4] { _lAdditionaParam2, text2, null, null };
				val14 = (TypeClass)8;
				obj[2] = Types.GetTypeId(val14.ToString(), ushort.MaxValue);
				val14 = (TypeClass)8;
				obj[3] = Types.GetBitSize(val14.ToString());
				sbParameters.AppendFormat("(dwParameterId:={0}, dwValue:=ADR({1}), wType:={2}, wLen := {3}, dwFlags:=50 )", obj);
				localContainer.sbParamStruct.AppendFormat("{{attribute 'noinit'}}{0}:{1};\n", text, "DINT");
			}
			AddIndexToParameterSet(parameterSet, _lAdditionaParam2, ref nNumParams);
		}

		private static void AddAdditionalParameter3(ParameterSet parameterSet, LanguageModelContainer localContainer, ref uint nNumParams, string stStructBase, bool bUpdateIoInStop)
		{
			if (parameterSet.Contains(_lAdditionaParam3))
			{
				return;
			}
			string text = "p" + nNumParams.ToString("x");
			ILanguageModelBuilder lmBuilder = localContainer.lmBuilder;
			ILanguageModelBuilder3 val = (ILanguageModelBuilder3)(object)((lmBuilder is ILanguageModelBuilder3) ? lmBuilder : null);
			TypeClass val10;
			if (val != null)
			{
				IVariableExpression val2 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, text);
				ILiteralExpression val3 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, bUpdateIoInStop);
				IAssignmentExpression item = ((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val2, (IExpression)(object)val3);
				localContainer.assInitValues.Add(item);
				List<IAssignmentExpression> list = new List<IAssignmentExpression>();
				IVariableExpression val4 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "dwParameterId");
				IVariableExpression val5 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "dwValue");
				IVariableExpression val6 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "wType");
				IVariableExpression val7 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "wLen");
				IVariableExpression val8 = (IVariableExpression)(object)((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, "dwFlags");
				ILiteralExpression val9 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, (ulong)_lAdditionaParam2, (TypeClass)4);
				val10 = (TypeClass)0;
				ILiteralExpression val11 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, (ulong)Types.GetTypeId(val10.ToString(), ushort.MaxValue), (TypeClass)3);
				val10 = (TypeClass)0;
				ILiteralExpression val12 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, (ulong)Types.GetBitSize(val10.ToString()), (TypeClass)3);
				ILiteralExpression val13 = ((ILanguageModelBuilder)val).CreateLiteralExpression((IExprementPosition)null, 50uL, (TypeClass)4);
				IVariableExpression2 val14 = ((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, stStructBase);
				IVariableExpression2 val15 = ((ILanguageModelBuilder)val).CreateVariableExpression((IExprementPosition)null, text);
				ICompoAccessExpression val16 = ((ILanguageModelBuilder)val).CreateCompoAccessExpression((IExprementPosition)null, (IExpression)(object)val14, val15);
				IOperatorExpression val17 = ((ILanguageModelBuilder)val).CreateOperatorExpression((IExprementPosition)null, (Operator)33, (IExpression)(object)val16);
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val4, (IExpression)(object)val9));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val5, (IExpression)(object)val17));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val6, (IExpression)(object)val11));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val7, (IExpression)(object)val12));
				list.Add(((ILanguageModelBuilder)val).CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val8, (IExpression)(object)val13));
				localContainer.struinitParameters.Add(((ILanguageModelBuilder)val).CreateStructureInitialisation((IExprementPosition)null, list));
				localContainer.seqParamStruct.AddStatement(val.CreatePragmaStatement2((IExprementPosition)null, "attribute 'noinit'"));
				ICompiledType val18 = ((ILanguageModelBuilder)val).CreateSimpleType((TypeClass)0);
				IVariableDeclarationStatement val19 = ((ILanguageModelBuilder)val).CreateSimpleVariableDeclarationStatement((IExprementPosition)null, text, val18);
				localContainer.seqParamStruct.AddStatement((IStatement)(object)val19);
			}
			else
			{
				if (localContainer.sbInitValues.Length > 0)
				{
					localContainer.sbInitValues.Append(",\n");
				}
				localContainer.sbInitValues.AppendFormat("{0}:={1}", text, bUpdateIoInStop.ToString());
				string text2 = stStructBase + "." + text;
				if (localContainer.sbParameters.Length > 0)
				{
					localContainer.sbParameters.Append(",\n");
				}
				StringBuilder sbParameters = localContainer.sbParameters;
				object[] obj = new object[4] { _lAdditionaParam3, text2, null, null };
				val10 = (TypeClass)0;
				obj[2] = Types.GetTypeId(val10.ToString(), ushort.MaxValue);
				val10 = (TypeClass)0;
				obj[3] = Types.GetBitSize(val10.ToString());
				sbParameters.AppendFormat("(dwParameterId:={0}, dwValue:=ADR({1}), wType:={2}, wLen := {3}, dwFlags:=50 )", obj);
				localContainer.sbParamStruct.AppendFormat("{{attribute 'noinit'}}{0}:{1};\n", text, "BOOL");
			}
			AddIndexToParameterSet(parameterSet, _lAdditionaParam3, ref nNumParams);
		}

		internal static bool ConvertValue(string stValueToCheck, ArrayList objectList, string stBaseType)
		{
			if (stValueToCheck.Length == 0)
			{
				return true;
			}
			IConverterToIEC converterToIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterToIEC(false, false, (DisplayMode)1);
			IConverterFromIEC converterFromIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
			IScanner val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(stBaseType, false, false, false, false);
			try
			{
				ICompiledType val2 = Types.ParseType(stBaseType);
				if (val2 == null)
				{
					return false;
				}
				TypeClass @class = ((IType)val2).Class;
				ArrayList arrayList = new ArrayList();
				IToken val3 = default(IToken);
				ulong num = default(ulong);
				bool flag = default(bool);
				Operator val4 = default(Operator);
				bool flag2 = default(bool);
				if ((int)@class == 26)
				{
					val.Initialize(stValueToCheck);
					val.GetNext(out val3);
					DateTime dateTime = default(DateTime);
					DateTime dateTime2 = default(DateTime);
					do
					{
						if ((int)val3.Type == 1)
						{
							arrayList.Add(val.GetBoolean(val3).ToString());
						}
						if ((int)val3.Type == 14)
						{
							val.GetInteger(val3, out num, out flag, out val4, out flag2);
							if (flag2)
							{
								return false;
							}
							arrayList.Add(num.ToString());
						}
						if ((int)val3.Type == 16)
						{
							double num2 = 0.0;
							val.GetReal(val3, out num2, out val4, out flag2);
							if (flag2)
							{
								return false;
							}
							arrayList.Add(converterToIEC.GetReal((object)num2, ((IType)val2.BaseType).Class));
						}
						if ((int)val3.Type == 17)
						{
							arrayList.Add("'" + val.GetSingleByteString(val3) + "'");
						}
						if ((int)val3.Type == 9)
						{
							arrayList.Add("\"" + val.GetDoubleByteString(val3) + "\"");
						}
						if ((int)val3.Type == 10)
						{
							uint num3 = 0u;
							val.GetDuration(val3, out num3, out flag2);
							if (flag2)
							{
								return false;
							}
							arrayList.Add(converterToIEC.GetDuration((long)num3));
						}
						if ((int)val3.Type == 11)
						{
							ulong num4 = 0uL;
							val.GetLDuration(val3, out num4, out flag2);
							if (flag2)
							{
								return false;
							}
							if (converterToIEC is IConverterToIEC2)
							{
								arrayList.Add(((IConverterToIEC2)((converterToIEC is IConverterToIEC2) ? converterToIEC : null)).GetLDuration((long)num4));
							}
						}
						if ((int)val3.Type == 6)
						{
							val.GetDateAndTime(val3, out dateTime, out flag2);
							if (flag2)
							{
								return false;
							}
							arrayList.Add(converterToIEC.GetDateAndTime(dateTime));
						}
						if ((int)val3.Type == 18)
						{
							val.GetTimeOfDay(val3, out dateTime2, out flag2);
							if (flag2)
							{
								return false;
							}
							arrayList.Add(converterToIEC.GetTimeOfDay(dateTime2));
						}
						val.GetNext(out val3);
					}
					while ((int)val3.Type != 21);
					@class = ((IType)val2.BaseType).Class;
					stBaseType = ((object)val2.BaseType).ToString();
				}
				else
				{
					arrayList.Add(stValueToCheck);
				}
				foreach (string item in arrayList)
				{
					object obj = null;
					switch ((int)@class)
					{
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
						val.Initialize(stBaseType + "#" + item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type == 14)
							{
								val.GetInteger(val3, out num, out flag, out val4, out flag2);
								if (flag2)
								{
									return false;
								}
								obj = num;
								val.GetNext(out val3);
								continue;
							}
							return false;
						}
						while ((int)val3.Type != 21);
						break;
					case 1:
						val.Initialize(item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type == 14)
							{
								val.GetInteger(val3, out num, out flag, out val4, out flag2);
								if (flag2 || (num != 0L && num != 1))
								{
									return false;
								}
								obj = num;
							}
							else
							{
								if ((int)val3.Type != 1)
								{
									return false;
								}
								obj = converterFromIEC.GetBoolean(item);
							}
							val.GetNext(out val3);
						}
						while ((int)val3.Type != 21);
						break;
					case 0:
					{
						string text2 = item;
						if (item == "0" || item == "1")
						{
							text2 = "BOOL#" + item;
						}
						val.Initialize(text2);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type != 1)
							{
								return false;
							}
							obj = converterFromIEC.GetBoolean(text2);
							val.GetNext(out val3);
						}
						while ((int)val3.Type != 21);
						break;
					}
					case 16:
						val.Initialize(item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type != 17)
							{
								return false;
							}
							obj = converterFromIEC.GetSingleByteString(item);
							val.GetNext(out val3);
						}
						while ((int)val3.Type != 21);
						break;
					case 17:
						val.Initialize(item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type != 9)
							{
								return false;
							}
							obj = converterFromIEC.GetDoubleByteString(item);
							val.GetNext(out val3);
						}
						while ((int)val3.Type != 21);
						break;
					case 14:
					case 15:
						val.Initialize(stBaseType + "#" + item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type == 16)
							{
								double num6 = 0.0;
								val.GetReal(val3, out num6, out val4, out flag2);
								if (flag2)
								{
									return false;
								}
								obj = num6;
								val.GetNext(out val3);
								continue;
							}
							return false;
						}
						while ((int)val3.Type != 21);
						break;
					case 18:
						val.Initialize(item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type == 10)
							{
								uint num5 = 0u;
								val.GetDuration(val3, out num5, out flag2);
								if (flag2)
								{
									return false;
								}
								obj = (long)num5;
								val.GetNext(out val3);
								continue;
							}
							return false;
						}
						while ((int)val3.Type != 21);
						break;
					case 37:
						val.Initialize(item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type == 11)
							{
								val.GetLDuration(val3, out num, out flag2);
								if (flag2)
								{
									return false;
								}
								obj = num;
								val.GetNext(out val3);
								continue;
							}
							return false;
						}
						while ((int)val3.Type != 21);
						break;
					case 20:
						val.Initialize(item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type != 6)
							{
								return false;
							}
							obj = converterFromIEC.GetDateAndTime(item);
							val.GetNext(out val3);
						}
						while ((int)val3.Type != 21);
						break;
					case 21:
						val.Initialize(item);
						val.GetNext(out val3);
						do
						{
							if ((int)val3.Type != 18)
							{
								return false;
							}
							obj = converterFromIEC.GetTimeOfDay(item);
							val.GetNext(out val3);
						}
						while ((int)val3.Type != 21);
						break;
					default:
						return false;
					}
					if (obj != null)
					{
						objectList.Add(obj);
					}
				}
			}
			catch
			{
			}
			return true;
		}

		internal static void MergeDataElements(IDataElement2 dataElementPhys, IDataElement2 dataElementLog, ByteOrder bo, Guid guidApplication, uint uiBitStartOffset, uint uiBitSize, ref string stError)
		{
			foreach (IDataElement2 item in (IEnumerable)((IDataElement)dataElementPhys).SubElements)
			{
				IDataElement2 val = item;
				if (((IDataElement)dataElementLog).SubElements.Contains(((IDataElement)val).Identifier))
				{
					IDataElement obj = ((IDataElement)dataElementLog).SubElements[((IDataElement)val).Identifier];
					MergeDataElements(val, (IDataElement2)(object)((obj is IDataElement2) ? obj : null), bo, guidApplication, uiBitStartOffset, uiBitSize, ref stError);
				}
			}
			if (!dataElementPhys.HasBaseType)
			{
				return;
			}
			try
			{
				ICompiledType val2 = Types.ParseType(((IDataElement)dataElementPhys).BaseType);
				ArrayList arrayList = new ArrayList();
				ConvertValue(((IDataElement)dataElementPhys).Value, arrayList, ((IDataElement)dataElementPhys).BaseType);
				ArrayList arrayList2 = new ArrayList();
				ConvertValue(((IDataElement)dataElementLog).Value, arrayList2, ((IDataElement)dataElementLog).BaseType);
				byte[] array = null;
				byte[] array2 = null;
				if ((int)((IType)val2).Class == 26)
				{
					object[] array3 = new object[arrayList.Count];
					arrayList.CopyTo(array3);
					array2 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).ConvertToRaw((object)array3, (IType)(object)val2, guidApplication, bo);
					object[] array4 = new object[arrayList2.Count];
					arrayList2.CopyTo(array4);
					array = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).ConvertToRaw((object)array4, (IType)(object)val2, guidApplication, bo);
				}
				else
				{
					array2 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).ConvertToRaw(arrayList[0], (IType)(object)val2, guidApplication, bo);
					array = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).ConvertToRaw(arrayList2[0], (IType)(object)val2, guidApplication, bo);
				}
				if (uiBitStartOffset != 0 && uiBitSize != 0)
				{
					uint num = (uint)((IDataElement4)((dataElementPhys is IDataElement4) ? dataElementPhys : null)).GetBitOffset();
					uint num2 = (uint)((IDataElement)((dataElementPhys is IDataElement4) ? dataElementPhys : null)).GetBitSize();
					uint num3 = uiBitStartOffset;
					uint num4 = uiBitStartOffset + uiBitSize;
					num3 -= num;
					num4 -= num;
					if (num3 < 0)
					{
						num3 = 0u;
					}
					if (num4 > num2)
					{
						num4 = num2;
					}
					if (num4 == 0 || num3 >= num4)
					{
						return;
					}
					if (num3 % 8u != 0)
					{
						uint num5 = num3 / 8u;
						if (num5 < array2.Length && num5 < array.Length)
						{
							byte b = (byte)(255 << (int)num3);
							array2[num5] = (byte)((array2[num5] & ~b) | (array[num5] & b));
							num3 = (num5 + 1) * 8;
						}
					}
					for (; num3 + 8 < num4; num3 += 8)
					{
						uint num6 = num3 / 8u;
						if (num6 < array2.Length && num6 < array.Length)
						{
							array2[num6] = array[num6];
						}
					}
					if (num3 < num4)
					{
						uint num7 = num3 / 8u;
						if (num7 < array2.Length && num7 < array.Length)
						{
							byte b2 = (byte)(255 << (int)(num4 - num3));
							array2[num7] = (byte)((array2[num7] & b2) | (array[num7] & ~b2));
						}
					}
					goto IL_0281;
				}
				array2 = array;
				goto IL_0281;
				IL_0281:
				object obj2 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).ConvertRaw(array2, (IType)(object)val2, guidApplication, bo);
				if ((int)((IType)val2).Class == 26)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("[");
					bool flag = true;
					object[] array5 = obj2 as object[];
					foreach (object obj3 in array5)
					{
						if (!flag)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(obj3.ToString());
						flag = false;
					}
					stringBuilder.Append("]");
					if (dataElementPhys is Parameter)
					{
						(dataElementPhys as Parameter).DataElementBase.SetValue(stringBuilder.ToString());
					}
					else if (dataElementPhys is DataElementBase)
					{
						(dataElementPhys as DataElementBase).SetValue(stringBuilder.ToString());
					}
					else
					{
						((IDataElement)dataElementPhys).Value=(stringBuilder.ToString());
					}
				}
				else if (dataElementPhys is Parameter)
				{
					(dataElementPhys as Parameter).DataElementBase.SetValue(obj2.ToString());
				}
				else if (dataElementPhys is DataElementBase)
				{
					(dataElementPhys as DataElementBase).SetValue(obj2.ToString());
				}
				else
				{
					((IDataElement)dataElementPhys).Value=(obj2.ToString());
				}
			}
			catch (Exception ex)
			{
				string arg = string.Format(LogicalIOStrings.ErrorMergingParams, ex.Message);
				stError += $"\r\n{{error '{arg}'}}";
			}
		}

		internal static void MergeParameters(Parameter param, SortedList parameters, long lIndex, Guid guidApplication, ref string stError)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			Parameter obj = parameters[lIndex] as Parameter;
			ByteOrder bo = (ByteOrder)0;
			ICompileContext referenceContextIfAvailable = ((ILanguageModelManager2)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(guidApplication);
			if (referenceContextIfAvailable != null && referenceContextIfAvailable.Codegenerator != null)
			{
				bo = (ByteOrder)(referenceContextIfAvailable.Codegenerator.MotorolaByteOrder ? 1 : 0);
			}
			else
			{
				guidApplication = Guid.Empty;
			}
			Parameter parameter = ((GenericObject)obj).Clone() as Parameter;
			MergeDataElements((IDataElement2)(object)parameter, (IDataElement2)(object)param, bo, guidApplication, param.LogicalMapOffset, param.LogicalMapSize, ref stError);
			parameters[lIndex] = parameter;
		}

		internal static void FillParametersList(Parameter param, bool bDevDescOrderDownload, SortedList parameters, bool bIsLogical, Guid guidApplication, ref string stError)
		{
			if ((!param.Download && !string.IsNullOrEmpty(param.FbInstanceVariable)) || (bIsLogical && !param.LogicalParameter))
			{
				return;
			}
			if (bDevDescOrderDownload)
			{
				if (bIsLogical && parameters.ContainsKey(param.IndexInDevDesc))
				{
					MergeParameters(param, parameters, param.IndexInDevDesc, guidApplication, ref stError);
				}
				else if (param.IndexInDevDesc == -1 || parameters.ContainsKey(param.IndexInDevDesc))
				{
					long num = 4294967295L;
					while (parameters.ContainsKey(num))
					{
						num--;
					}
					parameters.Add(num, param);
				}
				else
				{
					parameters.Add(param.IndexInDevDesc, param);
				}
			}
			else if (bIsLogical && parameters.Contains(param.Id))
			{
				MergeParameters(param, parameters, param.Id, guidApplication, ref stError);
			}
			else
			{
				parameters.Add(param.Id, param);
			}
		}

		public static bool FindChangedParametersInSet(IIoProvider ioprovider, out List<IParameter> changedParameters)
		{
			changedParameters = new List<IParameter>();
			DeviceObject deviceObject = ioprovider.GetHost() as DeviceObject;
			if (deviceObject == null)
			{
				return false;
			}
			IApplicationObject val = null;
			DeviceObject deviceObject2 = deviceObject.GetHostDeviceObject() as DeviceObject;
			if (deviceObject2 != null)
			{
				IPlcLogicObject plcLogicObject = deviceObject2.GetPlcLogicObject();
				if (plcLogicObject != null)
				{
					val = deviceObject2.GetApplicationObject(plcLogicObject);
				}
			}
			if (val == null)
			{
				return false;
			}
			string text = string.Empty;
			if (ioprovider is Connector)
			{
				text = (ioprovider as Connector).GetParamsListName();
			}
			if (ioprovider is DeviceObject)
			{
				text = (ioprovider as DeviceObject).GetParamsListName();
			}
			string text2 = "S_" + text;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)0, (ushort)4))
			{
				string text3 = text.Substring(3, 8) + text.Substring(text.LastIndexOf("PS") - 1);
				text2 = "S" + text3;
			}
			ICompileContext referenceContextIfAvailable = ((ILanguageModelManager2)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(((IOnlineApplicationObject)val).ApplicationGuid);
			ICompileContext compileContext = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetCompileContext(((IOnlineApplicationObject)val).ApplicationGuid);
			bool result = false;
			if (referenceContextIfAvailable != null && compileContext != null)
			{
				string text4 = "GVL_" + text;
				ISignature signature = ((ICompileContextCommon)referenceContextIfAvailable).GetSignature(text4);
				ISignature signature2 = ((ICompileContextCommon)compileContext).GetSignature(text4);
				if (signature != null && signature2 != null)
				{
					_ = string.Empty;
					IVariable val2 = signature[text];
					IVariable val3 = signature2[text];
					if (val3 != null && val2 != null && val2.Initial is IArrayInitialization && val3.Initial is IArrayInitialization)
					{
						IExpression initial = val2.Initial;
						IExpression obj = ((initial is IArrayInitialization) ? initial : null);
						IExpression initial2 = val3.Initial;
						IArrayInitialization val4 = (IArrayInitialization)(object)((initial2 is IArrayInitialization) ? initial2 : null);
						if (((IArrayInitialization)obj).InitValues.Length != val4.InitValues.Length)
						{
							return false;
						}
					}
					IVariable val5 = signature[text2];
					IVariable val6 = signature2[text2];
					if (val5 != null && val5.Initial is IStructureInitialization && val6 != null && val6.Initial is IStructureInitialization)
					{
						IExpression initial3 = val5.Initial;
						IExpression obj2 = ((initial3 is IStructureInitialization) ? initial3 : null);
						IExpression initial4 = val6.Initial;
						IStructureInitialization val7 = (IStructureInitialization)(object)((initial4 is IStructureInitialization) ? initial4 : null);
						IAssignmentExpression[] compoInits = ((IStructureInitialization)obj2).CompoInits;
						foreach (IAssignmentExpression val8 in compoInits)
						{
							IAssignmentExpression[] compoInits2 = val7.CompoInits;
							foreach (IAssignmentExpression val9 in compoInits2)
							{
								if (!((ILanguageModelManager13)APEnvironment.LanguageModelMgr).ExpressionsEqual((IExprement)(object)val8.LValue, (IExprement)(object)val9.LValue))
								{
									continue;
								}
								result = true;
								if (((ILanguageModelManager13)APEnvironment.LanguageModelMgr).ExpressionsEqual((IExprement)(object)val8.RValue, (IExprement)(object)val9.RValue))
								{
									continue;
								}
								foreach (Parameter item in (IEnumerable)ioprovider.ParameterSet)
								{
									if (string.Equals(item.ParamName, ((IExprement)val8.LValue).ToString(), StringComparison.InvariantCultureIgnoreCase))
									{
										item.DownloadedValue = ((IExprement)val8.RValue).ToString();
										changedParameters.Add((IParameter)(object)item);
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		internal static int CalcDataElementHashCode(IDataElement dataElement)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0))
			{
				Crc32 val = new Crc32();
				CalcDataElementCrc(dataElement, val);
				return (int)val.Value;
			}
			return CalcDataElementOldHashCode(dataElement);
		}

		internal static int CalcDataElementCrc(IDataElement dataElement, Crc32 crc)
		{
			int result = 0;
			if (dataElement.HasSubElements)
			{
				foreach (DataElementBase item in (IEnumerable)dataElement.SubElements)
				{
					Encoding unicode = Encoding.Unicode;
					byte[] bytes = unicode.GetBytes(item.GetTypeName(string.Empty));
					crc.Update(bytes);
					bytes = unicode.GetBytes(item.Identifier);
					crc.Update(bytes);
					CalcDataElementCrc((IDataElement)(object)item, crc);
				}
				return result;
			}
			return result;
		}

		internal static int CalcDataElementOldHashCode(IDataElement dataElement)
		{
			int num = 0;
			if (dataElement.HasSubElements)
			{
				foreach (DataElementBase item in (IEnumerable)dataElement.SubElements)
				{
					num ^= GetHashStringCode(item.GetTypeName(string.Empty));
					num ^= GetHashStringCode(item.Identifier);
					num ^= CalcDataElementOldHashCode((IDataElement)(object)item);
				}
				return num;
			}
			return num;
		}

		public static int GetHashStringCode(string stSource)
		{
			Encoding unicode = Encoding.Unicode;
			byte[] array = new byte[stSource.Length * 2 + 8];
			unicode.GetBytes(stSource).CopyTo(array, 0);
			int num = 352654597;
			int num2 = num;
			int num3 = 0;
			int num4 = stSource.Length;
			while (num4 > 2)
			{
				int num5 = BitConverter.ToInt32(array, num3);
				int num6 = BitConverter.ToInt32(array, num3 + 4);
				num = ((num << 5) + num + (num >> 27)) ^ num5;
				num2 = ((num2 << 5) + num2 + (num2 >> 27)) ^ num6;
				num4 -= 4;
				num3 += 8;
			}
			if (num4 > 0)
			{
				int num7 = BitConverter.ToInt32(array, num3);
				num = ((num << 5) + num + (num >> 27)) ^ num7;
			}
			return num + num2 * 1566083941;
		}

		public static void GetLanguageModelForParameterSet(ILanguageModelBuilder3 lmbuilder, ILanguageModel lmnew, ParameterSet parameterSet, string stBaseName, XmlWriter xmlWriter, Guid guidLanguageModelParent, string stErrors, List<List<string>> codeTables, bool bCreateAdditionalParams, bool bDevDescOrderDownload, Guid appGuid, bool bUpdateIoInStop)
		{
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Expected I4, but got Unknown
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060e: Expected O, but got Unknown
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0784: Invalid comparison between Unknown and I4
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Invalid comparison between Unknown and I4
			uint nNumParams = 0u;
			LanguageModelContainer languageModelContainer = new LanguageModelContainer();
			languageModelContainer.lmBuilder = (ILanguageModelBuilder)(object)lmbuilder;
			languageModelContainer.lmNew = lmnew;
			if (lmbuilder != null)
			{
				languageModelContainer.assInitValues = new List<IAssignmentExpression>();
				languageModelContainer.assInitValuesNoBlobInit = new List<IAssignmentExpression>();
				languageModelContainer.struinitParameters = new List<IStructureInitialization>();
				languageModelContainer.seqParamStruct = (ISequenceStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
				languageModelContainer.seqParamVarDeclNoBlobInit = (ISequenceStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
			}
			string text = "";
			string text2 = "S_" + stBaseName;
			string text3 = "T_" + stBaseName;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)0, (ushort)4))
			{
				string text4 = stBaseName.Substring(3, 8) + stBaseName.Substring(stBaseName.LastIndexOf("PS") - 1);
				text2 = "S" + text4;
				text3 = "T" + text4;
			}
			string text5 = text2 + "NoBlob";
			string text6 = text3 + "NoBlob";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)0, (ushort)2))
			{
				flag2 = true;
			}
			SortedList sortedList = new SortedList();
			foreach (Parameter item in parameterSet)
			{
				FillParametersList(item, bDevDescOrderDownload, sortedList, bIsLogical: false, appGuid, ref stErrors);
			}
			if (DeviceObjectHelper.GenerateCodeForLogicalDevices && parameterSet.Device is ILogicalDevice)
			{
				IDeviceObject2 device = parameterSet.Device;
				ILogicalDevice val = (ILogicalDevice)(object)((device is ILogicalDevice) ? device : null);
				if (val.IsPhysical)
				{
					LList<IIoProvider> mappedIoProvider = DeviceObjectHelper.GetMappedIoProvider(parameterSet.IoProvider, bCheckForLogical: false);
					if (mappedIoProvider.Count > 0)
					{
						long num = 0L;
						long num2 = 0L;
						bool flag6 = val is ILogicalDevice2 && ((ILogicalDevice2)((val is ILogicalDevice2) ? val : null)).MappingPossible;
						foreach (IIoProvider item2 in mappedIoProvider)
						{
							foreach (Parameter item3 in (IEnumerable)item2.ParameterSet)
							{
								if (item3.Id == 1879048216)
								{
									Parameter parameter2 = new Parameter(item3);
									Parameter parameter3 = new Parameter(item3);
									if (flag6 || mappedIoProvider.Count > 1)
									{
										string[] array = parameter2.Value.Replace("[", "").Replace("]", "").Split(',');
										if (array != null && array.Length >= 4)
										{
											long num3 = 0L;
											long num4 = 0L;
											long num5 = -1L;
											long num6 = -1L;
											long num7 = 0L;
											long num8 = 0L;
											long.TryParse(array[2], out var result);
											long.TryParse(array[3], out var result2);
											foreach (Parameter value in sortedList.Values)
											{
												ChannelType channelType = value.ChannelType;
												switch ((int)channelType - 1)
												{
												case 0:
												{
													long bitSize = value.GetBitSize();
													if ((value.LogicalParameter || !flag6) && num <= num3)
													{
														if (num6 < 0)
														{
															num6 = num3;
														}
														if (num7 < result)
														{
															num7 += bitSize;
															num = num3 + bitSize;
														}
													}
													num3 += bitSize;
													break;
												}
												case 1:
												case 2:
												{
													long bitSize = value.GetBitSize();
													if ((value.LogicalParameter || !flag6) && num2 <= num4)
													{
														if (num5 < 0)
														{
															num5 = num4;
														}
														if (num8 < result2)
														{
															num8 += bitSize;
															num2 = num4 + bitSize;
														}
													}
													num4 += bitSize;
													break;
												}
												}
											}
											if (num6 > 0 || num5 > 0)
											{
												LDictionary<int, string> val2 = new LDictionary<int, string>();
												for (int i = 0; i < array.Length; i++)
												{
													val2[i]= array[i];
												}
												while (val2.Count < 6)
												{
													val2[val2.Count]= "0";
												}
												if (num6 > 0)
												{
													val2[6]= num6.ToString();
												}
												else
												{
													val2[6]= "0";
												}
												if (num5 > 0)
												{
													val2[7]= num5.ToString();
												}
												else
												{
													val2[7]= "0";
												}
												StringBuilder stringBuilder = new StringBuilder();
												foreach (string value in val2.Values)
												{
													if (stringBuilder.Length == 0)
													{
														stringBuilder.Append("[");
													}
													else
													{
														stringBuilder.Append(",");
													}
													stringBuilder.Append(value);
												}
												stringBuilder.Append("]");
												parameter2.DataElementBase.SetValue(stringBuilder.ToString());
												parameter3.DataElementBase.SetValue(stringBuilder.ToString());
											}
										}
									}
									if (sortedList.Contains(2130706456L))
									{
										Parameter parameter5 = sortedList[2130706456L] as Parameter;
										if (parameter5 != null)
										{
											uint.TryParse(parameter5.Value, out var result3);
											parameter5.Value = (result3 + 1).ToString();
											parameter2.Id = 2130706457L + (long)result3;
											sortedList.Add(parameter2.Id, parameter2);
										}
									}
									else
									{
										Parameter parameter6 = new Parameter(2130706456L, null, null, (AccessRight)3, (AccessRight)3, (ChannelType)0, "std:USINT", null, item2.ParameterSet as ParameterSet, bCreateBitChannels: false);
										parameter6.Value = "1";
										sortedList.Add(parameter6.Id, parameter6);
										parameter2.Id = 2130706457L;
										sortedList.Add(parameter2.Id, parameter2);
									}
									FillParametersList(parameter3, bDevDescOrderDownload, sortedList, bIsLogical: true, appGuid, ref stErrors);
								}
								else
								{
									FillParametersList(item3, bDevDescOrderDownload, sortedList, bIsLogical: true, appGuid, ref stErrors);
								}
							}
						}
					}
				}
			}
			Crc32 val3 = new Crc32();
			parameterSet.ParamIdToIndex.Clear();
			uint num9 = 0u;
			LDictionary<IDataElement, Guid> val16 = default(LDictionary<IDataElement, Guid>);
			foreach (Parameter value2 in sortedList.Values)
			{
				if (!value2.Download || !string.IsNullOrEmpty(value2.FbInstanceVariable))
				{
					continue;
				}
				if (!value2.OnlineChangeEnabled && value2.Value != null)
				{
					byte[] bytes = Encoding.GetEncoding(1252).GetBytes(value2.Value);
					val3.Update(bytes);
				}
				if (bCreateAdditionalParams && !flag3 && value2.Id > _lAdditionaParam1)
				{
					AddAdditionalParameter1(parameterSet, languageModelContainer, ref nNumParams, text2);
					flag3 = true;
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)2, (ushort)0) && bCreateAdditionalParams && !flag4 && value2.Id > _lAdditionaParam2)
				{
					AddAdditionalParameter2(parameterSet, languageModelContainer, ref nNumParams, text2);
					flag4 = true;
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)0) && value2.Id > _lAdditionaParam3 && !flag5 && bUpdateIoInStop)
				{
					AddAdditionalParameter3(parameterSet, languageModelContainer, ref nNumParams, text2, bUpdateIoInStop);
					flag5 = true;
				}
				bool bHide = true;
				if ((int)value2.ChannelType != 0 && ((ICollection)value2.IoMapping.VariableMappings).Count > 0 && value2.IoMapping.VariableMappings[0].CreateVariable)
				{
					bHide = false;
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0))
				{
					if (!value2.IsConstantValue)
					{
						flag2 = false;
					}
				}
				else
				{
					AccessRight accessRight = value2.GetAccessRight(bOnline: true);
					if ((int)accessRight == 3 || (int)accessRight == 2 || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)30))
					{
						flag2 = false;
					}
				}
				bool flag7 = false;
				if (value2.CustomItems != null && ((ICollection)value2.CustomItems).Count > 0)
				{
					foreach (CustomItem item4 in (IEnumerable)value2.CustomItems)
					{
						if (!(item4.Name == "POINTER_TO_IOCONFIGADDRESS"))
						{
							continue;
						}
						parameterSet.HasIoConfigAddress = true;
						flag7 = true;
						long result4 = 0L;
						if (!long.TryParse(value2.Value, out result4))
						{
							continue;
						}
						foreach (Parameter item5 in parameterSet)
						{
							if (item5.Id == result4)
							{
								string empty = string.Empty;
								if (item5.IoMapping.VariableMappings != null && ((ICollection)item5.IoMapping.VariableMappings).Count > 0 && !item5.IoMapping.VariableMappings[0].CreateVariable)
								{
									IVariableMapping val4 = item5.IoMapping.VariableMappings[0];
									empty = val4.Variable.Substring(val4.Variable.IndexOf('.') + 1);
								}
								else
								{
									empty = item5.IoMapping.IecAddress;
								}
								if (lmbuilder != null)
								{
									List<IAssignmentExpression> list = new List<IAssignmentExpression>();
									IVariableExpression val5 = (IVariableExpression)(object)languageModelContainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwParameterId");
									IVariableExpression val6 = (IVariableExpression)(object)languageModelContainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwValue");
									IVariableExpression val7 = (IVariableExpression)(object)languageModelContainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "wType");
									IVariableExpression val8 = (IVariableExpression)(object)languageModelContainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "wLen");
									IVariableExpression val9 = (IVariableExpression)(object)languageModelContainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwFlags");
									ILiteralExpression val10 = languageModelContainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)value2.Id, (TypeClass)4);
									ILiteralExpression val11 = languageModelContainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)Types.GetTypeId(value2.BaseType, ushort.MaxValue), (TypeClass)3);
									ILiteralExpression val12 = languageModelContainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)value2.GetBitSize(), (TypeClass)3);
									ILiteralExpression val13 = languageModelContainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 50uL, (TypeClass)4);
									IExpression val14 = languageModelContainer.lmBuilder.ParseExpression(empty);
									IOperatorExpression val15 = languageModelContainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)33, val14);
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val5, (IExpression)(object)val10));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val6, (IExpression)(object)val15));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val7, (IExpression)(object)val11));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val8, (IExpression)(object)val12));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val9, (IExpression)(object)val13));
									languageModelContainer.struinitParameters.Add(languageModelContainer.lmBuilder.CreateStructureInitialisation((IExprementPosition)null, list));
								}
								else
								{
									languageModelContainer.sbParameters.Append(text);
									languageModelContainer.sbParameters.AppendFormat("(dwParameterId:={0}, dwValue:=ADR({1}), wType:={2}, wLen := {3}, dwFlags:=50 )", value2.Id, empty, Types.GetTypeId(value2.BaseType, ushort.MaxValue), value2.GetBitSize());
									text = ",\n\t";
								}
								AddIndexToParameterSet(parameterSet, value2.Id, ref nNumParams);
								flag = true;
							}
						}
					}
				}
				if (flag7)
				{
					continue;
				}
				string stParamName = CreateParamName(stBaseName, value2.Id);
				bool isGlobalType = false;
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)0) && value2.HasSubElements)
				{
					DeviceObject deviceObject = null;
					if (value2.DataElementBase is DataElementStructType && !(value2.DataElementBase as DataElementStructType).HasIecType)
					{
						deviceObject = parameterSet.IoProvider.GetHost() as DeviceObject;
					}
					if (value2.DataElementBase is DataElementUnionType && !(value2.DataElementBase as DataElementUnionType).HasIecType)
					{
						deviceObject = parameterSet.IoProvider.GetHost() as DeviceObject;
					}
					if (deviceObject != null && parameterSet.Device != null)
					{
						IDeviceObject2 device2 = parameterSet.Device;
						IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((device2 is IDeviceObject5) ? device2 : null)).DeviceIdentificationNoSimulation;
						string text7 = CalcDataElementHashCode((IDataElement)(object)value2.DataElementBase).ToString("X");
						string text8 = "param_" + deviceIdentificationNoSimulation.Type + "_" + text7 + "_";
						string typeName = value2.DataElementBase.GetTypeName("T" + text8 + value2.Identifier);
						if (!deviceObject.GlobalDataTypes.TryGetValue(typeName, out val16))
						{
							val16 = new LDictionary<IDataElement, Guid>();
							deviceObject.GlobalDataTypes[typeName]= val16;
						}
						stParamName = text8 + value2.DataElementBase.Identifier;
						isGlobalType = true;
						val16[(IDataElement)(object)value2.DataElementBase]= guidLanguageModelParent;
					}
				}
				if (value2.AddToLanguageModel(languageModelContainer, stParamName, text2, text5, text, bHide, isGlobalType, nNumParams))
				{
					AddIndexToParameterSet(parameterSet, value2.Id, ref nNumParams);
					if (!value2.CreateDownloadStructure)
					{
						num9++;
					}
					text = ",\n\t";
				}
			}
			if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) && (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)60) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)))
			{
				parameterSet.NumParams = nNumParams;
			}
			if (bCreateAdditionalParams && !flag3)
			{
				AddAdditionalParameter1(parameterSet, languageModelContainer, ref nNumParams, text2);
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)2, (ushort)0) && bCreateAdditionalParams && !flag4)
			{
				AddAdditionalParameter2(parameterSet, languageModelContainer, ref nNumParams, text2);
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)0) && bUpdateIoInStop && !flag5)
			{
				AddAdditionalParameter3(parameterSet, languageModelContainer, ref nNumParams, text2, bUpdateIoInStop);
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)10) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)40) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0)))
			{
				nNumParams -= num9;
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)))
			{
				parameterSet.NumParams = nNumParams;
			}
			if (lmbuilder != null)
			{
				Debug.Assert(languageModelContainer.sbInitValues.Length == 0);
				Debug.Assert(languageModelContainer.sbParameters.Length == 0);
				Debug.Assert(languageModelContainer.sbParamStruct.Length == 0);
				ISequenceStatement2 val17 = ((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
				if (nNumParams != 0)
				{
					((ISequenceStatement)val17).AddStatement((IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateMessageGuidPragmaStatement(guidLanguageModelParent));
					if (parameterSet.IoProvider is IConnector10 && (parameterSet.IoProvider as IConnector10).UseBlobInitConst)
					{
						((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'blobinitconst'"));
					}
					else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)30))
					{
						((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'blobinit'"));
					}
					else
					{
						((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'blobinitconst'"));
					}
					((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
					string text9 = ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)0)) ? $"ARRAY[0..{nNumParams}] OF IoConfigParameter" : $"ARRAY[0..{nNumParams - 1}] OF IoConfigParameter");
					ICompiledType val18 = ((ILanguageModelBuilder)lmbuilder).ParseType(text9);
					List<IExpression> list2 = new List<IExpression>();
					foreach (IStructureInitialization struinitParameter in languageModelContainer.struinitParameters)
					{
						list2.Add((IExpression)(object)struinitParameter);
					}
					IArrayInitialization val19 = ((ILanguageModelBuilder)lmbuilder).CreateArrayInitialisation((IExprementPosition)null, list2);
					IVariableDeclarationStatement val20 = ((ILanguageModelBuilder)lmbuilder).CreateVariableDeclarationStatement((IExprementPosition)null, stBaseName, val18, (IExpression)(object)val19, (IDirectVariable)null);
					((ISequenceStatement)val17).AddStatement((IStatement)(object)val20);
					if (languageModelContainer.seqParamStruct.Statements.Length != 0)
					{
						string text10 = $"attribute 'parametercrc={val3.Value.ToString()}'";
						((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, text10));
						if (flag2)
						{
							((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'blobinitconst'"));
						}
						else
						{
							((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'blobinit'"));
						}
						((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
						IStructureInitialization val21 = ((ILanguageModelBuilder)lmbuilder).CreateStructureInitialisation((IExprementPosition)null, languageModelContainer.assInitValues);
						ICompiledType val22 = ((ILanguageModelBuilder)lmbuilder).ParseType(text3);
						IVariableDeclarationStatement val23 = ((ILanguageModelBuilder)lmbuilder).CreateVariableDeclarationStatement((IExprementPosition)null, text2, val22, (IExpression)(object)val21, (IDirectVariable)null);
						((ISequenceStatement)val17).AddStatement((IStatement)(object)val23);
						Guid guid = FindGuidStructure(text3, appGuid);
						ILMDataType val24 = ((ILanguageModelBuilder)lmbuilder).CreateDataType(text3, guid);
						ISequenceStatement val25 = (ISequenceStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0))
						{
							val25.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'symbol' := 'none'"));
						}
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)30))
						{
							val25.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
						}
						val25.AddStatement((IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateStructDeclaration((IExprementPosition)null, text3, (IExpression)null, (IExpression)null, languageModelContainer.seqParamStruct, (SignatureFlag)0));
						val24.Interface=(val25);
						lmnew.AddDataType(val24);
					}
					if (languageModelContainer.seqParamVarDeclNoBlobInit.Statements.Length != 0)
					{
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
						{
							IStatement[] statements = languageModelContainer.seqParamVarDeclNoBlobInit.Statements;
							foreach (IStatement val26 in statements)
							{
								string text11 = $"attribute 'parametercrc={val3.Value.ToString()}'";
								((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, text11));
								((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
								((ISequenceStatement)val17).AddStatement(val26);
							}
						}
						else
						{
							string text12 = $"attribute 'parametercrc={val3.Value.ToString()}'";
							((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, text12));
							((ISequenceStatement)val17).AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
							IStructureInitialization val27 = ((ILanguageModelBuilder)lmbuilder).CreateStructureInitialisation((IExprementPosition)null, languageModelContainer.assInitValuesNoBlobInit);
							ICompiledType val28 = ((ILanguageModelBuilder)lmbuilder).ParseType(text6);
							IVariableDeclarationStatement val29 = ((ILanguageModelBuilder)lmbuilder).CreateVariableDeclarationStatement((IExprementPosition)null, text5, val28, (IExpression)(object)val27, (IDirectVariable)null);
							((ISequenceStatement)val17).AddStatement((IStatement)(object)val29);
							Guid guid2 = FindGuidStructure(text6, appGuid);
							ILMDataType val30 = ((ILanguageModelBuilder)lmbuilder).CreateDataType(text6, guid2);
							ISequenceStatement val31 = (ISequenceStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0))
							{
								val31.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'symbol' := 'none'"));
							}
							val31.AddStatement((IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateStructDeclaration((IExprementPosition)null, text6, (IExpression)null, (IExpression)null, languageModelContainer.seqParamVarDeclNoBlobInit, (SignatureFlag)0));
							val30.Interface=(val31);
							lmnew.AddDataType(val30);
						}
					}
				}
				languageModelContainer.GetParameterInitializationStatements(stBaseName, (ISequenceStatement)(object)val17);
				if (stErrors != null && stErrors != "")
				{
					((ISequenceStatement)val17).AddStatement((IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateMessageGuidPragmaStatement(guidLanguageModelParent));
					ISequenceStatement val32 = (ISequenceStatement)(object)((ILanguageModelBuilder)lmbuilder).ParseSTSnippet(stErrors);
					((ISequenceStatement)val17).AddStatement((IStatement)(object)val32);
				}
				Guid empty2 = Guid.Empty;
				if (DeviceObjectHelper.LanguageModelGuids.TryGetValue(parameterSet.LanguageModelGuid, out empty2) && empty2 != guidLanguageModelParent)
				{
					parameterSet.UpdateLanguageModelGuids(bUpgrade: false);
				}
				DeviceObjectHelper.LanguageModelGuids[parameterSet.LanguageModelGuid]= guidLanguageModelParent;
				if (((ISequenceStatement)val17).Statements.Length != 0)
				{
					ILMGlobVarlist val33 = ((ILanguageModelBuilder)lmbuilder).CreateGlobVarlist("GVL_" + stBaseName, parameterSet.LanguageModelGuid);
					IVariableDeclarationListStatement val34 = ((ILanguageModelBuilder)lmbuilder).CreateVariableDeclarationListStatement((IExprementPosition)null, (VarFlag)8192, (ISequenceStatement)(object)val17);
					ISequenceStatement val35 = (ISequenceStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
					string text13 = "attribute '" + CompileAttributes.ATTRIBUTE_ALLOW_INITIAL_VALUE_CHANGES + "'";
					val35.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, text13));
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0))
					{
						val35.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
					}
					if (!string.IsNullOrEmpty(stErrors) && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)0))
					{
						val35.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'force_precompile_checks'"));
					}
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)40) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)5, (ushort)0) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)))
					{
						val35.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'qualified_only'"));
					}
					if (appGuid != Guid.Empty)
					{
						string text14 = "attribute '" + CompileAttributes.ATTRIBUTE_SIGNATURE_FLAG + "' := '" + 1073741824 + "'";
						val35.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, text14));
					}
					val35.AddStatement((IStatement)(object)val34);
					val33.Interface=(val35);
					val33.ObjectGuid=(guidLanguageModelParent);
					val33.InhibitOnlineChange=(true);
					lmnew.AddGlobalVariableList(val33);
				}
				AddTypeDefs(languageModelContainer.StructTypes, xmlWriter, guidLanguageModelParent, codeTables);
				return;
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			if (nNumParams != 0)
			{
				stringBuilder2.AppendFormat("{{messageguid '{0}'}}\n", guidLanguageModelParent);
				if (!flag)
				{
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)0, (ushort)2))
					{
						stringBuilder2.AppendLine("{attribute 'blobinitconst'}");
					}
					else
					{
						stringBuilder2.AppendLine("{attribute 'blobinit'}");
					}
				}
				stringBuilder2.AppendFormat("{0}\n{1} : ARRAY[0..{2}] OF IoConfigParameter := [\n", PRAGMA_ATTRIBUTE_HIDE, stBaseName, nNumParams);
				stringBuilder2.Append(languageModelContainer.sbParameters.ToString());
				stringBuilder2.Append("\n];\n");
				if (languageModelContainer.sbParamStruct.Length > 0)
				{
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)3, (ushort)10))
					{
						stringBuilder2.AppendFormat("{{attribute 'parametercrc={0}'}}\n", val3.Value.ToString());
					}
					if (flag2)
					{
						stringBuilder2.AppendLine("{attribute 'blobinitconst'}");
					}
					else
					{
						stringBuilder2.AppendLine("{attribute 'blobinit'}");
					}
					stringBuilder2.AppendLine(PRAGMA_ATTRIBUTE_HIDE);
					stringBuilder2.AppendFormat("{0}: {1} := ({2});\n", text2, text3, languageModelContainer.sbInitValues.ToString());
					StringBuilder stringBuilder3 = new StringBuilder();
					stringBuilder3.AppendFormat("TYPE {0}:\nSTRUCT\n{1}", text3, languageModelContainer.sbParamStruct.ToString());
					stringBuilder3.Append("END_STRUCT\nEND_TYPE\n");
					Guid guidStructDef = FindGuidStructure(text3, appGuid);
					languageModelContainer.AddStructType(text3, stringBuilder3.ToString(), guidStructDef, bHide: true);
				}
			}
			string parameterInitializations = languageModelContainer.GetParameterInitializations(stBaseName);
			if (parameterInitializations != string.Empty || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)1, (ushort)1))
			{
				stringBuilder2.AppendLine(parameterInitializations);
			}
			if (stErrors != null && stErrors != "")
			{
				stringBuilder2.AppendFormat("{{messageguid '{0}'}}\n", guidLanguageModelParent);
				stringBuilder2.AppendLine(stErrors);
			}
			Guid empty3 = Guid.Empty;
			if (DeviceObjectHelper.LanguageModelGuids.TryGetValue(parameterSet.LanguageModelGuid, out empty3) && empty3 != guidLanguageModelParent)
			{
				parameterSet.UpdateLanguageModelGuids(bUpgrade: false);
			}
			DeviceObjectHelper.LanguageModelGuids[parameterSet.LanguageModelGuid]= guidLanguageModelParent;
			if (stringBuilder2.Length > 0)
			{
				List<string> list3 = new List<string>();
				list3.Add(stringBuilder2.ToString());
				string text15 = string.Empty;
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)3, (ushort)10))
				{
					text15 = "{attribute '" + CompileAttributes.ATTRIBUTE_ALLOW_INITIAL_VALUE_CHANGES + "'}";
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
				{
					text15 = text15 + "\r\n" + PRAGMA_ATTRIBUTE_HIDE;
				}
				if (!string.IsNullOrEmpty(stErrors) && stErrors != "(* *)" && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)0, (ushort)0))
				{
					AddGlobalVarListWithRetains(list3, string.Empty, parameterSet.LanguageModelGuid, "GVL_" + stBaseName, xmlWriter, guidLanguageModelParent, bAllowOnlineChange: false, text15, null, appGuid == Guid.Empty);
				}
				else
				{
					AddGlobalVarListWithRetains(list3, string.Empty, parameterSet.LanguageModelGuid, "GVL_" + stBaseName, xmlWriter, guidLanguageModelParent, bAllowOnlineChange: false, text15, codeTables, appGuid == Guid.Empty);
				}
			}
			AddTypeDefs(languageModelContainer.StructTypes, xmlWriter, guidLanguageModelParent, codeTables);
		}

		public static uint GetNumDownloadParams(IParameterSet parameterSet, out bool bHasPointerParams)
		{
			bHasPointerParams = false;
			Hashtable hashtable = new Hashtable();
			if (DeviceObjectHelper.GenerateCodeForLogicalDevices && parameterSet is ParameterSet && (parameterSet as ParameterSet).Device is ILogicalDevice && ((parameterSet as ParameterSet).Device as ILogicalDevice).IsPhysical)
			{
				LList<IIoProvider> mappedIoProvider = DeviceObjectHelper.GetMappedIoProvider((parameterSet as ParameterSet).IoProvider, bCheckForLogical: false);
				if (mappedIoProvider.Count > 0)
				{
					foreach (IIoProvider item in mappedIoProvider)
					{
						foreach (Parameter item2 in (IEnumerable)item.ParameterSet)
						{
							if (item2.Download && item2.CreateDownloadStructure && item2.LogicalParameter && !hashtable.ContainsKey(item2.Id))
							{
								hashtable.Add(item2.Id, item2);
							}
						}
					}
				}
			}
			foreach (Parameter item3 in (IEnumerable)parameterSet)
			{
				bool flag = false;
				if (item3.CustomItems != null && ((ICollection)item3.CustomItems).Count > 0)
				{
					foreach (CustomItem item4 in (IEnumerable)item3.CustomItems)
					{
						if (!(item4.Name == "POINTER_TO_IOCONFIGADDRESS"))
						{
							continue;
						}
						bHasPointerParams = true;
						if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)20) && (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)70) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0)))
						{
							continue;
						}
						long result = 0L;
						flag = true;
						if (!long.TryParse(item3.Value, out result))
						{
							continue;
						}
						foreach (Parameter item5 in (IEnumerable)parameterSet)
						{
							if (item5.Id == result)
							{
								flag = false;
							}
						}
					}
				}
				if (!flag && item3.Download && item3.CreateDownloadStructure && string.IsNullOrEmpty(item3.FbInstanceVariable) && !hashtable.ContainsKey(item3.Id))
				{
					hashtable.Add(item3.Id, item3);
				}
			}
			return (uint)hashtable.Count;
		}

		public static void AddGlobalVarList(List<string> liData, Guid guidGvl, string stName, XmlWriter xmlWriter, Guid guidLanguageModelParent, bool bAllowOnlineChange, string stAttributes, List<List<string>> codeTables)
		{
			AddGlobalVarListWithRetains(liData, "", guidGvl, stName, xmlWriter, guidLanguageModelParent, bAllowOnlineChange, stAttributes, codeTables, bLanguageInPool: false);
		}

		public static void AddGlobalVarListWithRetains(List<string> liData, string stRetains, Guid guidGvl, string stName, XmlWriter xmlWriter, Guid guidLanguageModelParent, bool bAllowOnlineChange, string stAttributes, List<List<string>> codeTables)
		{
			AddGlobalVarListWithRetains(liData, stRetains, guidGvl, stName, xmlWriter, guidLanguageModelParent, bAllowOnlineChange, stAttributes, codeTables, bLanguageInPool: false);
		}

		internal static Guid CreateDeterministicGuid(Guid baseGuid, string referenceName)
		{
			return GuidHelper.CreateNameBasedGuid(namespaceGuid, baseGuid, referenceName, (UUIDVersion)5);
		}

		public static void AddDeviceApplicationExternalEventFb(XmlWriter xmlWriter, Guid applicationGuid)
		{
			string value = CreateDeterministicGuid(applicationGuid, "BusCycleExternalEventTrigger").ToString();
			xmlWriter.WriteStartElement("pou");
			xmlWriter.WriteAttributeString("id", value);
			xmlWriter.WriteAttributeString("name", "BusCycleExternalEventTrigger");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("FUNCTION_BLOCK {0}\n", "BusCycleExternalEventTrigger");
			stringBuilder.AppendLine("VAR CONSTANT");
			stringBuilder.AppendLine("  INVALID_HANDLE : POINTER TO WORD := -1;");
			stringBuilder.AppendLine("  RESULT_OK : UDINT := 0;");
			stringBuilder.AppendLine("END_VAR");
			stringBuilder.AppendLine("VAR");
			stringBuilder.AppendLine("  hExtHandle : POINTER TO WORD := INVALID_HANDLE;");
			stringBuilder.AppendLine("  szEventName : STRING;");
			stringBuilder.AppendFormat("END_VAR");
			xmlWriter.WriteElementString("interface", stringBuilder.ToString());
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("{ST_IMPLEMENTATION}");
			stringBuilder2.AppendLine("{IF defined(pou:CmpSchedule.SchedPostExternalEvent)}");
			stringBuilder2.AppendLine("  IF THIS^.hExtHandle <> INVALID_HANDLE THEN");
			stringBuilder2.AppendLine("    CmpSchedule.SchedPostExternalEvent(THIS^.hExtHandle);");
			stringBuilder2.AppendLine("  END_IF");
			stringBuilder2.AppendLine("{END_IF}");
			xmlWriter.WriteElementString("body", stringBuilder2.ToString());
			xmlWriter.WriteEndElement();
			string referenceName = string.Format("{0}.{1}", "BusCycleExternalEventTrigger", "FB_INIT");
			string value2 = CreateDeterministicGuid(applicationGuid, referenceName).ToString();
			xmlWriter.WriteStartElement("method");
			xmlWriter.WriteAttributeString("id", value2);
			xmlWriter.WriteAttributeString("pou-id", value);
			xmlWriter.WriteAttributeString("name", "FB_INIT");
			stringBuilder = new StringBuilder();
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)16, (ushort)0))
			{
				stringBuilder.AppendFormat("METHOD {0} : BOOL\n", "FB_INIT");
			}
			else
			{
				stringBuilder.AppendFormat("METHOD {0}\n", "FB_INIT");
			}
			stringBuilder.AppendLine("VAR_INPUT");
			stringBuilder.AppendLine("  bInitRetains : BOOL;");
			stringBuilder.AppendLine("  bInCopyCode : BOOL;");
			stringBuilder.AppendLine("  szEventName: STRING;");
			stringBuilder.AppendLine("END_VAR");
			stringBuilder.AppendLine("VAR");
			stringBuilder.AppendLine("  result : UDINT;");
			stringBuilder.AppendFormat("END_VAR");
			xmlWriter.WriteElementString("interface", stringBuilder.ToString());
			stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("{ST_IMPLEMENTATION}");
			stringBuilder2.AppendLine("THIS^.szEventName := szEventName;");
			stringBuilder2.AppendLine("{IF defined(pou:CmpSchedule.SchedRegisterExternalEvent)}");
			stringBuilder2.AppendLine("  THIS^.hExtHandle := CmpSchedule.SchedRegisterExternalEvent(THIS^.szEventName, ADR(result));");
			stringBuilder2.AppendLine("  IF result <> RESULT_OK THEN");
			stringBuilder2.AppendLine("    THIS^.hExtHandle := INVALID_HANDLE;");
			stringBuilder2.AppendLine("  END_IF");
			stringBuilder2.AppendLine("{END_IF}");
			xmlWriter.WriteElementString("body", stringBuilder2.ToString());
			xmlWriter.WriteEndElement();
			string referenceName2 = string.Format("{0}.{1}", "BusCycleExternalEventTrigger", "FB_EXIT");
			string value3 = CreateDeterministicGuid(applicationGuid, referenceName2).ToString();
			xmlWriter.WriteStartElement("method");
			xmlWriter.WriteAttributeString("id", value3);
			xmlWriter.WriteAttributeString("pou-id", value);
			xmlWriter.WriteAttributeString("name", "FB_EXIT");
			stringBuilder = new StringBuilder();
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)16, (ushort)0))
			{
				stringBuilder.AppendFormat("METHOD {0} : BOOL\n", "FB_EXIT");
			}
			else
			{
				stringBuilder.AppendFormat("METHOD {0}\n", "FB_EXIT");
			}
			stringBuilder.AppendLine("VAR_INPUT");
			stringBuilder.AppendLine("  bInCopyCode : BOOL;");
			stringBuilder.AppendFormat("END_VAR");
			xmlWriter.WriteElementString("interface", stringBuilder.ToString());
			stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("{ST_IMPLEMENTATION}");
			stringBuilder2.AppendLine("{IF defined(pou:CmpSchedule.SchedUnregisterExternalEvent)}");
			stringBuilder2.AppendLine("  IF THIS^.hExtHandle <> INVALID_HANDLE THEN");
			stringBuilder2.AppendLine("    CmpSchedule.SchedUnregisterExternalEvent(THIS^.hExtHandle);");
			stringBuilder2.AppendLine("  END_IF");
			stringBuilder2.AppendLine("{END_IF}");
			xmlWriter.WriteElementString("body", stringBuilder2.ToString());
			xmlWriter.WriteEndElement();
		}

		public static void AddGlobalVarListWithRetains(List<string> liData, string stRetains, Guid guidGvl, string stName, XmlWriter xmlWriter, Guid guidLanguageModelParent, bool bAllowOnlineChange, string stAttributes, List<List<string>> codeTables, bool bLanguageInPool)
		{
			if (guidGvl == Guid.Empty)
			{
				Debug.Fail("The Guid for a GVL must not be null");
				throw new ArgumentException("The Guid for a GVL must not be null", "guidGvl");
			}
			xmlWriter.WriteStartElement("global-interface");
			xmlWriter.WriteAttributeString("id", XmlConvert.ToString(guidGvl));
			xmlWriter.WriteAttributeString("name", stName);
			if (guidLanguageModelParent != Guid.Empty)
			{
				xmlWriter.WriteAttributeString("object-id", guidLanguageModelParent.ToString());
			}
			if (!bAllowOnlineChange)
			{
				xmlWriter.WriteAttributeString("inhibit-online-change", XmlConvert.ToString(value: true));
			}
			if (codeTables == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (!bLanguageInPool)
				{
					stringBuilder.Append("{attribute '" + CompileAttributes.ATTRIBUTE_SIGNATURE_FLAG + "' := '" + 1073741824 + "'}");
				}
				stringBuilder.Append(stAttributes);
				stringBuilder.Append("VAR_GLOBAL");
				foreach (string liDatum in liData)
				{
					stringBuilder.Append(liDatum);
				}
				stringBuilder.Append("\nEND_VAR");
				if (!string.IsNullOrEmpty(stRetains))
				{
					stringBuilder.Append("VAR_GLOBAL RETAIN\n");
					stringBuilder.Append(stRetains);
					stringBuilder.Append("\nEND_VAR");
				}
				xmlWriter.WriteElementString("interface", stringBuilder.ToString());
				xmlWriter.WriteEndElement();
				return;
			}
			List<string> list = new List<string>();
			StringBuilder stringBuilder2 = new StringBuilder();
			if (!bLanguageInPool)
			{
				stringBuilder2.Append("{attribute '" + CompileAttributes.ATTRIBUTE_SIGNATURE_FLAG + "' := '" + 1073741824 + "'}");
			}
			if (!string.IsNullOrEmpty(stAttributes))
			{
				stringBuilder2.Append(stAttributes);
			}
			stringBuilder2.Append("VAR_GLOBAL");
			list.Add(stringBuilder2.ToString());
			list.AddRange(liData);
			list.Add("\nEND_VAR");
			if (!string.IsNullOrEmpty(stRetains))
			{
				stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("VAR_GLOBAL RETAIN\n");
				stringBuilder2.Append(stRetains);
				stringBuilder2.Append("\nEND_VAR");
				list.Add(stringBuilder2.ToString());
			}
			xmlWriter.WriteStartElement("interface");
			xmlWriter.WriteAttributeString("string-table-reference", codeTables.Count.ToString());
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			codeTables.Add(list);
		}

		public static void AddTypeDefs(IList structureDefinitions, XmlWriter xmlWriter, Guid guidLanguageModelParent, List<List<string>> codeTables)
		{
			foreach (StructDefinition structureDefinition in structureDefinitions)
			{
				xmlWriter.WriteStartElement("data-type");
				xmlWriter.WriteAttributeString("id", XmlConvert.ToString(structureDefinition._guidStructDef));
				xmlWriter.WriteAttributeString("name", structureDefinition._stName);
				xmlWriter.WriteAttributeString("object-id", guidLanguageModelParent.ToString());
				if (codeTables == null)
				{
					xmlWriter.WriteElementString("interface", structureDefinition._stDefinition);
				}
				else
				{
					xmlWriter.WriteStartElement("interface");
					List<string> list = new List<string>();
					list.Add(structureDefinition._stDefinition);
					xmlWriter.WriteAttributeString("string-table-reference", codeTables.Count.ToString());
					codeTables.Add(list);
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal static bool MoveIoIteratorInConnectorListOrder(IIoModuleIterator it)
		{
			if (it.MoveToFirstChild())
			{
				return true;
			}
			do
			{
				if (it.MoveToNextSibling())
				{
					return true;
				}
			}
			while (it.MoveToParent());
			return false;
		}

		internal static bool IsLibraryInLibMan(IMetaObject moApplication, LibsToAddItem libitem, ILibManItem[] libitems, bool bRemoveLibs, out ILibManItem foundLib, bool bLoadFinished)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			foundLib = null;
			if (libitems == null)
			{
				return false;
			}
			string text2 = default(string);
			string text3 = default(string);
			string text4 = default(string);
			IManagedLibraryRepository val3 = default(IManagedLibraryRepository);
			foreach (ILibManItem val in libitems)
			{
				string text = string.Empty;
				bool flag = false;
				if (val is IPlaceholderLibManItem)
				{
					IPlaceholderLibManItem val2 = (IPlaceholderLibManItem)val;
					if (val2.EffectiveResolution != null)
					{
						text = val2.EffectiveResolution.DisplayName;
						flag = true;
					}
					else if (val2.DefaultResolution != null)
					{
						text = val2.DefaultResolution;
						flag = true;
					}
				}
				else
				{
					text = val.Name;
				}
				if (string.IsNullOrEmpty(text) || !((ILibraryLoader3)APEnvironment.LibraryLoader).ParseDisplayName(text, out text2, out text3, out text4) || !text2.Equals(libitem.stLibName, StringComparison.OrdinalIgnoreCase) || !text4.Equals(libitem.stLibVendor, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				foundLib = val;
				if (bRemoveLibs)
				{
					try
					{
						if (bLoadFinished)
						{
							string text5 = "0.0.0.0";
							if (text3 == "*" || libitem.stLibVersion == "*")
							{
								IManagedLibrary val4 = APEnvironment.ManagedLibraryMgr.FindLibrary(text, out val3);
								if (val4 != null)
								{
									text5 = val4.Version.ToString();
								}
							}
							bool flag2 = false;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)0))
							{
								if ((libitem.bLoadAsPlaceHolder | !string.IsNullOrEmpty(libitem.stPlaceholder)) != flag)
								{
									flag2 = true;
								}
								if (!libitem.bLoadAsPlaceHolder)
								{
									flag = false;
								}
							}
							else if (libitem.bLoadAsPlaceHolder != flag)
							{
								flag2 = true;
							}
							Version version = new Version((text3 == "*") ? text5 : text3);
							Version version2 = new Version((libitem.stLibVersion == "*") ? text5 : libitem.stLibVersion);
							if (text2.Equals(libitem.stLibName, StringComparison.OrdinalIgnoreCase) && text4.Equals(libitem.stLibVendor, StringComparison.OrdinalIgnoreCase) && ((version != version2 && !flag) || flag2))
							{
								((ILibraryLoader)APEnvironment.LibraryLoader).UnloadSystemLibrary(val.Name, moApplication.ProjectHandle, moApplication.ObjectGuid);
								return false;
							}
						}
					}
					catch
					{
					}
				}
				return true;
			}
			return false;
		}

		public static ILibManItem[] GetLibItems(IMetaObject moApplication)
		{
			IMetaObjectStub libManOfApp = GetLibManOfApp(moApplication);
			if (libManOfApp != null)
			{
				IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(moApplication.ProjectHandle, libManOfApp.ObjectGuid).Object;
				return ((ILibManObject)((@object is ILibManObject) ? @object : null)).GetAllLibraries(false);
			}
			return null;
		}

		private static IMetaObjectStub GetLibManOfApp(IMetaObject moApplication)
		{
			IMetaObjectStub result = null;
			Guid[] subObjectGuids = moApplication.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(moApplication.ProjectHandle, guid);
				if (typeof(ILibManObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					result = metaObjectStub;
					break;
				}
			}
			return result;
		}

		public static void CollectLibsForLogicalIO(int nProjectHandle, LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs, Guid[] objectGuids)
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Expected O, but got Unknown
			foreach (Guid guid in objectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
					if (objectToRead.Object is IIoProvider && objectToRead.Object is DeviceObject)
					{
						IObject @object = objectToRead.Object;
						IIoProvider val = (IIoProvider)(object)((@object is IIoProvider) ? @object : null);
						foreach (IRequiredLib item in (IEnumerable)val.DriverInfo.RequiredLibs)
						{
							IRequiredLib val2 = item;
							if (!(val2 as RequiredLib).IsDiagnosisLib)
							{
								dictRequiredLibs.Add(val2, val);
							}
						}
						IIoProvider[] children = val.Children;
						foreach (IIoProvider val3 in children)
						{
							if (val3.Excluded)
							{
								continue;
							}
							foreach (IRequiredLib item2 in (IEnumerable)val3.DriverInfo.RequiredLibs)
							{
								IRequiredLib val4 = item2;
								if (!(val4 as RequiredLib).IsDiagnosisLib)
								{
									dictRequiredLibs.Add(val4, val);
								}
							}
						}
					}
				}
				CollectLibsForLogicalIO(nProjectHandle, dictRequiredLibs, metaObjectStub.SubObjectGuids);
			}
		}

		public static void AddLibraries(Guid hostGuid, LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs, IMetaObject moApplication, LanguageModelContainer lmcontainer, bool bIsLogical, bool bLoadFinished)
		{
			AddLibraries(hostGuid, dictRequiredLibs, moApplication, lmcontainer, bIsLogical, out var _, bEnableDiagnosis: false, null, bHideFbInstances: false, bLoadFinished);
		}

		internal static DiagnosisInstance GetDiagnosisInstance(Guid objectGuid)
		{
			DiagnosisInstance result = default(DiagnosisInstance);
			if (_dictDiagnosisInstances.TryGetValue(objectGuid, out result))
			{
				return result;
			}
			return null;
		}

		internal static void AddDiagnosisInstance(DiagnosisInstance instance)
		{
			_dictDiagnosisInstances.Add(instance.ObjectGuid, instance);
		}

		internal static void ClearDiagnosisInstance()
		{
			_dictDiagnosisInstances.Clear();
		}

		internal static Guid GetParentGuid(IMetaObject meta, LDictionary<Guid, DiagnosisInstance> liDiagLibs)
		{
			if (Guid.Empty != meta.ParentObjectGuid && !liDiagLibs.ContainsKey(meta.ParentObjectGuid))
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(meta.ProjectHandle, meta.ParentObjectGuid);
				if (typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return metaObjectStub.ParentObjectGuid;
				}
			}
			return meta.ParentObjectGuid;
		}

		internal static void CollectDiagnosisInstance(LDictionary<Guid, DiagnosisInstance> liDiagLibs, IIoProvider ioProvider, IMetaObject moApplication, int iModuleIndex)
		{
			if (ioProvider == null)
			{
				return;
			}
			IMetaObject metaObject = ioProvider.GetMetaObject();
			ILibManItem library = default(ILibManItem);
			foreach (RequiredLib item in (IEnumerable)ioProvider.DriverInfo.RequiredLibs)
			{
				if (item.IsDiagnosisLib)
				{
					continue;
				}
				string text = string.Empty;
				if (typeof(IRequiredLib2).IsAssignableFrom(((object)item).GetType()))
				{
					text = ((IRequiredLib2)item).Client;
				}
				if (!(string.Empty == text))
				{
					continue;
				}
				foreach (FBInstance item2 in (IEnumerable)item.FbInstances)
				{
					if (!string.IsNullOrEmpty(item2.FbNameDiag))
					{
						DiagnosisInstance diagnosisInstance = new DiagnosisInstance(iModuleIndex, metaObject.ObjectGuid, GetParentGuid(metaObject, liDiagLibs), ioProvider);
						diagnosisInstance.Instance = item2;
						liDiagLibs[metaObject.ObjectGuid]= diagnosisInstance;
					}
					else
					{
						if (!(item2.BaseName == "$(DeviceName)"))
						{
							continue;
						}
						DiagnosisInstance diagnosisInstance2 = GetDiagnosisInstance(metaObject.ObjectGuid);
						if (diagnosisInstance2 == null)
						{
							diagnosisInstance2 = new DiagnosisInstance(iModuleIndex, metaObject.ObjectGuid, GetParentGuid(metaObject, liDiagLibs), ioProvider);
							diagnosisInstance2.Instance.BaseName += "_Diag";
							string fbName = "CAADiagDeviceDefault";
							foreach (IFbInstanceFactory fbInstanceFactory in APEnvironment.FbInstanceFactories)
							{
								string text2 = fbInstanceFactory.FBInstanceDiagnosis(metaObject, ioProvider, out library);
								if (!string.IsNullOrEmpty(text2))
								{
									diagnosisInstance2.SetLibrary(library);
									fbName = text2;
									AddDiagnosisInstance(diagnosisInstance2);
									break;
								}
							}
							diagnosisInstance2.Instance.SetFbName(fbName);
							string baseName = DeviceObjectHelper.GetBaseName(diagnosisInstance2.Instance.BaseName, metaObject.Name);
							diagnosisInstance2.Instance.Instance.Variable=(DeviceObjectHelper.BuildIecIdentifier(baseName));
							diagnosisInstance2.Instance.Instance.CreateVariable=(true);
						}
						liDiagLibs[metaObject.ObjectGuid]= diagnosisInstance2;
					}
				}
			}
			if (liDiagLibs.ContainsKey(metaObject.ObjectGuid) || metaObject.Object is IExplicitConnector || (metaObject.Object is SlotDeviceObject && !((SlotDeviceObject)(object)metaObject.Object).HasDevice))
			{
				return;
			}
			DiagnosisInstance diagnosisInstance3 = GetDiagnosisInstance(metaObject.ObjectGuid);
			if (diagnosisInstance3 == null)
			{
				diagnosisInstance3 = new DiagnosisInstance(iModuleIndex, metaObject.ObjectGuid, GetParentGuid(metaObject, liDiagLibs), ioProvider);
				string fbName2 = "CAADiagDeviceDefault";
				ILibManItem val = default(ILibManItem);
				foreach (IFbInstanceFactory fbInstanceFactory2 in APEnvironment.FbInstanceFactories)
				{
					string text3 = fbInstanceFactory2.FBInstanceDiagnosis(metaObject, ioProvider, out val);
					if (!string.IsNullOrEmpty(text3) && val != null)
					{
						diagnosisInstance3.SetLibrary(val);
						fbName2 = text3;
						AddDiagnosisInstance(diagnosisInstance3);
						break;
					}
				}
				diagnosisInstance3.Instance.SetFbName(fbName2);
				diagnosisInstance3.Instance.Instance.Variable=(DeviceObjectHelper.GetBaseName(diagnosisInstance3.Instance.BaseName, metaObject.Name));
				diagnosisInstance3.Instance.Instance.CreateVariable=(true);
			}
			liDiagLibs[metaObject.ObjectGuid]= diagnosisInstance3;
		}

		internal static void CollectDiagnosisInstances(LDictionary<Guid, DiagnosisInstance> liDiagLibs, IIoProvider ioProvider, IMetaObject moApplication)
		{
			Dictionary<IIoProvider, int> dictIndex = new Dictionary<IIoProvider, int>();
			int iModuleIndex = 0;
			CollectIoProviders(ioProvider, dictIndex, ref iModuleIndex);
			foreach (IFbInstanceFactory fbInstanceFactory in APEnvironment.FbInstanceFactories)
			{
				fbInstanceFactory.ReInit(moApplication.ProjectHandle, moApplication.ObjectGuid);
			}
			CollectDiagnosisInstances(liDiagLibs, ioProvider, moApplication, dictIndex);
			foreach (IFbInstanceFactory fbInstanceFactory2 in APEnvironment.FbInstanceFactories)
			{
				fbInstanceFactory2.CleanUp();
			}
		}

		internal static void CollectIoProviders(IIoProvider ioProvider, Dictionary<IIoProvider, int> dictIndex, ref int iModuleIndex)
		{
			if (ioProvider == null || DeviceObjectHelper.IsExcludedFromBuild(ioProvider.GetMetaObject()))
			{
				return;
			}
			if (!dictIndex.ContainsKey(ioProvider))
			{
				dictIndex.Add(ioProvider, iModuleIndex);
				iModuleIndex++;
			}
			else if (!DeviceObjectHelper.MessageShown)
			{
				DeviceObjectHelper.MessageShown = true;
				string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ErrorCorruptDeviceStructure");
				DeviceMessage deviceMessage = new DeviceMessage(@string, (Severity)1);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
				APEnvironment.MessageService.Error(@string, "ErrorCorruptDeviceStructure", Array.Empty<object>());
			}
			IIoProvider[] children = ioProvider.Children;
			foreach (IIoProvider val in children)
			{
				if (!val.Excluded)
				{
					CollectIoProviders(val, dictIndex, ref iModuleIndex);
				}
			}
		}

		internal static void CollectDiagnosisInstances(LDictionary<Guid, DiagnosisInstance> liDiagLibs, IIoProvider ioProvider, IMetaObject moApplication, Dictionary<IIoProvider, int> dictIndex)
		{
			if (ioProvider == null || !dictIndex.ContainsKey(ioProvider))
			{
				return;
			}
			int iModuleIndex = dictIndex[ioProvider];
			CollectDiagnosisInstance(liDiagLibs, ioProvider, moApplication, iModuleIndex);
			IIoProvider[] children = ioProvider.Children;
			foreach (IIoProvider val in children)
			{
				if (!dictIndex.ContainsKey(val))
				{
					continue;
				}
				iModuleIndex = dictIndex[val];
				CollectDiagnosisInstance(liDiagLibs, val, moApplication, iModuleIndex);
				if (val is IExplicitConnector && !liDiagLibs.ContainsKey(val.GetMetaObject().ObjectGuid))
				{
					IIoProvider[] children2 = val.Children;
					foreach (IIoProvider val2 in children2)
					{
						iModuleIndex = dictIndex[val2];
						CollectDiagnosisInstance(liDiagLibs, val2, moApplication, iModuleIndex);
					}
				}
			}
			children = ioProvider.Children;
			foreach (IIoProvider ioProvider2 in children)
			{
				CollectDiagnosisInstances(liDiagLibs, ioProvider2, moApplication, dictIndex);
			}
		}

		private static void ResolveLibs(LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs, List<LibsToAddItem> liLibs, Guid applicationGuid, bool bIsLogical, ref bool bIoStandardInserted)
		{
			foreach (KeyValuePair<IRequiredLib, IIoProvider> keyValuePair in dictRequiredLibs)
			{
				IIoProvider value = keyValuePair.Value;
				IRequiredLib key = keyValuePair.Key;
				IIoProvider ioProvider = value;
				string text = key.LibName;
				string text2 = key.Version;
				string text3 = key.Vendor;
				bool flag = false;
				string placeHolderLib = ((RequiredLib)key).PlaceHolderLib;
				if (placeHolderLib != string.Empty)
				{
					ILibraryPlaceholderResolution libraryPlaceholderResolution = APEnvironment.CreatePlaceholderResolution();
					while (ioProvider != null)
					{
						IMetaObject metaObject = ioProvider.GetMetaObject();
						if (metaObject != null && metaObject.Object is IDeviceObject)
						{
							IDeviceObject deviceObject = (IDeviceObject)metaObject.Object;
							IDeviceIdentification deviceIdentification;
							if (deviceObject is IDeviceObject5)
							{
								deviceIdentification = (deviceObject as IDeviceObject5).DeviceIdentificationNoSimulation;
							}
							else
							{
								deviceIdentification = deviceObject.DeviceIdentification;
							}
							if (!(deviceIdentification is IModuleIdentification))
							{
								ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceIdentification);
								if (targetSettingsById != null && targetSettingsById.Sections["library-management"] != null)
								{
									Guid empty = Guid.Empty;
									if (metaObject.ParentObjectGuid == Guid.Empty || DeviceObjectHelper.HasPlcLogicObject(metaObject.ProjectHandle, metaObject.ObjectGuid, out empty))
									{
										flag = true;
										if (empty != Guid.Empty && Array.IndexOf<Guid>(APEnvironment.ObjectMgr.GetMetaObjectStub(metaObject.ProjectHandle, empty).SubObjectGuids, applicationGuid) == -1)
										{
											flag = false;
										}
									}
									string text4 = null;
									if (libraryPlaceholderResolution != null)
									{
										try
										{
											DeviceObjectHelper.SkipPlaceholderResolution = true;
											string stDefaultLibrary = string.Format("{0}, {1} ({2})", text, text2, text3);
											if (libraryPlaceholderResolution is ILibraryPlaceholderResolution3)
											{
												string text5;
												string text6;
												bool flag2;
												text4 = (libraryPlaceholderResolution as ILibraryPlaceholderResolution3).ResolvePlaceholder(targetSettingsById, applicationGuid, placeHolderLib, stDefaultLibrary, true, out text5, out text6, out flag2);
												if (flag2)
												{
													flag = true;
												}
											}
											else
											{
												string text5;
												text4 = libraryPlaceholderResolution.ResolvePlaceholder(targetSettingsById, placeHolderLib, stDefaultLibrary, out text5);
											}
										}
										catch
										{
											text4 = null;
										}
										finally
										{
											DeviceObjectHelper.SkipPlaceholderResolution = false;
										}
									}
									if (string.IsNullOrEmpty(text4))
									{
										string stPath = string.Format("library-management\\placeholder-libraries\\{0}", placeHolderLib);
										text4 = targetSettingsById.GetStringValue(stPath, null);
									}
									if (!string.IsNullOrEmpty(text4))
									{
										IManagedLibraryRepository managedLibraryRepository;
										IManagedLibrary managedLibrary = APEnvironment.ManagedLibraryMgr.FindLibrary(text4, out managedLibraryRepository);
										if (managedLibrary != null)
										{
											text = managedLibrary.Title;
											text3 = managedLibrary.Company;
											text2 = managedLibrary.Version.ToString();
											break;
										}
									}
								}
							}
						}
						ioProvider = ioProvider.Parent;
						if (ioProvider == null && metaObject.Object is ILogicalDevice && (metaObject.Object as ILogicalDevice).IsLogical)
						{
							IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObject.ProjectHandle, metaObject.ParentObjectGuid);
							while (metaObjectStub.ParentObjectGuid != Guid.Empty && !typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
							}
							if (metaObjectStub != null)
							{
								IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
								if (objectToRead.Object is IDeviceObject)
								{
									if ((objectToRead.Object as IDeviceObject).Connectors.Count > 0)
									{
										ioProvider = ((objectToRead.Object as IDeviceObject).Connectors[0] as IIoProvider);
									}
									else
									{
										ioProvider = (objectToRead.Object as IIoProvider);
									}
								}
								if (objectToRead.Object is IExplicitConnector)
								{
									ioProvider = (objectToRead.Object as IIoProvider);
								}
							}
						}
					}
				}
				bool flag3 = true;
				ioProvider = value.Parent;
				while (ioProvider != null && flag3)
				{
					foreach (object obj in ioProvider.DriverInfo.RequiredLibs)
					{
						IRequiredLib requiredLib = (IRequiredLib)obj;
						if (requiredLib.LibName == text && requiredLib.Vendor == text3)
						{
							flag3 = false;
							break;
						}
					}
					ioProvider = ioProvider.Parent;
				}
				if (text == "IoStandard")
				{
					bIoStandardInserted = true;
				}
				string text7 = string.Empty;
				if (typeof(IRequiredLib2).IsAssignableFrom(key.GetType()))
				{
					text7 = ((IRequiredLib2)key).Client;
				}
				bool flag4 = bIsLogical && text7.ToLowerInvariant() == "onlyLocal";
				if (text7 == string.Empty || flag4 || string.Equals(text7, "3SLicense", StringComparison.OrdinalIgnoreCase))
				{
					bool bLoadAsSystemLibrary = true;
					if (key is RequiredLib)
					{
						bLoadAsSystemLibrary = (key as RequiredLib).LoadAsSystemLibrary;
					}
					bool bLoadAsPlaceHolder = false;
					if (flag && !string.IsNullOrEmpty(placeHolderLib))
					{
						bLoadAsPlaceHolder = true;
						if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 3, 0))
						{
							try
							{
								ILibManItem[] libItems = LanguageModelHelper.GetLibItems(APEnvironment.ObjectMgr.GetObjectToRead(value.GetMetaObject().ProjectHandle, applicationGuid));
								if (libItems != null)
								{
									foreach (ILibManItem libManItem in libItems)
									{
										string a;
										string a2;
										string a3;
										if (!(libManItem is IPlaceholderLibManItem) && !string.IsNullOrEmpty(text) && APEnvironment.LibraryLoader.ParseDisplayName(libManItem.Name, out a, out a2, out a3) && a == text && a3 == text3 && a2 == text2)
										{
											bLoadAsPlaceHolder = false;
										}
									}
								}
							}
							catch
							{
							}
						}
					}
					LibsToAddItem libsToAddItem = new LibsToAddItem();
					libsToAddItem.stLibName = text;
					libsToAddItem.stLibVendor = text3;
					libsToAddItem.stLibVersion = text2;
					libsToAddItem.ioProvider = value;
					libsToAddItem.bLoadAsPlaceHolder = bLoadAsPlaceHolder;
					libsToAddItem.bLoadAsSystemLibrary = bLoadAsSystemLibrary;
					libsToAddItem.stPlaceholder = placeHolderLib;
					libsToAddItem.bInsertLibrary = flag3;
					libsToAddItem.requiredLib = key;
					if (flag3)
					{
						foreach (LibsToAddItem libsToAddItem2 in liLibs)
						{
							if (libsToAddItem2.stLibName.Equals(text, StringComparison.OrdinalIgnoreCase) && libsToAddItem2.stLibVendor.Equals(text3, StringComparison.OrdinalIgnoreCase) && libsToAddItem2.bInsertLibrary)
							{
								libsToAddItem.bLoadAsPlaceHolder |= libsToAddItem2.bLoadAsPlaceHolder;
								libsToAddItem2.bLoadAsPlaceHolder |= libsToAddItem.bLoadAsPlaceHolder;
								libsToAddItem.bLoadAsSystemLibrary |= libsToAddItem2.bLoadAsSystemLibrary;
								libsToAddItem2.bLoadAsSystemLibrary |= libsToAddItem.bLoadAsSystemLibrary;
								if (libsToAddItem.stLibVersion == "*")
								{
									libsToAddItem2.bInsertLibrary = false;
								}
								else if (libsToAddItem2.stLibVersion == "*")
								{
									libsToAddItem.bInsertLibrary = false;
								}
								else
								{
									try
									{
										Version v = new Version(libsToAddItem.stLibVersion);
										Version v2 = new Version(libsToAddItem2.stLibVersion);
										if (!(v >= v2))
										{
											libsToAddItem2.bInsertLibrary = true;
											libsToAddItem.bInsertLibrary = false;
											break;
										}
										libsToAddItem2.bInsertLibrary = false;
										libsToAddItem.bInsertLibrary = true;
									}
									catch
									{
									}
								}
							}
						}
					}
					liLibs.Add(libsToAddItem);
				}
			}
		}

		public static void AddLibraries(Guid hostGuid, LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs, IMetaObject moApplication, LanguageModelContainer lmcontainer, bool bIsLogical, out bool bIoStandardInserted, bool bEnableDiagnosis, IIoProvider ioprovider, bool bHideFbInstances, bool bLoadFinished)
		{
			if (bEnableDiagnosis || bEnableDiagnosis != _bLastDiagnosis)
			{
				LDictionary<Guid, DiagnosisInstance> val = new LDictionary<Guid, DiagnosisInstance>();
				CollectDiagnosisInstances(val, ioprovider, moApplication);
				if (val.Count > 0)
				{
					RequiredLib requiredLib = new RequiredLib();
					requiredLib.LibName = "CAA Device Diagnosis";
					requiredLib.Version = "3.5.0.0";
					requiredLib.Identifier = "DED";
					requiredLib.Vendor = "CAA Technical Workgroup";
					requiredLib.PlaceHolderLib = "CAA Device Diagnosis";
					requiredLib.LoadAsSystemLibrary = true;
					requiredLib.IsDiagnosisLib = true;
					if (bEnableDiagnosis)
					{
						dictRequiredLibs.Add((IRequiredLib)(object)requiredLib, ioprovider);
					}
					foreach (LanguageModelHelper.DiagnosisInstance diagnosisInstance in val.Values)
					{
						if (diagnosisInstance.IoProvider.DriverInfo != null)
						{
							DriverInfo driverInfo = diagnosisInstance.IoProvider.DriverInfo as DriverInfo;
							if (driverInfo != null)
							{
								foreach (object obj in driverInfo.AlRequiredLibs)
								{
									RequiredLib requiredLib2 = (RequiredLib)obj;
									if (requiredLib2.LibName == requiredLib.LibName || requiredLib2.IsDiagnosisLib)
									{
										driverInfo.AlRequiredLibs.Remove(requiredLib2);
										break;
									}
								}
								if (bEnableDiagnosis)
								{
									if (diagnosisInstance.Instance.FbName == "CAADiagDeviceDefault")
									{
										RequiredLib requiredLib3 = new RequiredLib(requiredLib);
										requiredLib3.AlFbInstances.Add(diagnosisInstance.Instance);
										driverInfo.AlRequiredLibs.Add(requiredLib3);
									}
									else if (diagnosisInstance.RequiredLib != null)
									{
										RequiredLib requiredLib4 = new RequiredLib(diagnosisInstance.RequiredLib);
										requiredLib4.AlFbInstances.Add(diagnosisInstance.Instance);
										driverInfo.AlRequiredLibs.Add(requiredLib4);
									}
								}
							}
						}
						if (bEnableDiagnosis && string.IsNullOrEmpty(diagnosisInstance.Instance.FbNameDiag))
						{
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 2, 0))
							{
								if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 6, 40))
								{
									lmcontainer.sbValues.Append(string.Format("{{messageguid '{0}'}}\n", diagnosisInstance.ObjectGuid.ToString()));
								}
								if (diagnosisInstance.RequiredLib != null)
								{
									lmcontainer.sbValues.Append(string.Format("\t{0} : {1};\n", diagnosisInstance.Instance.Instance.Variable, diagnosisInstance.RequiredLib.Identifier + "." + diagnosisInstance.Instance.FbName));
								}
								else
								{
									lmcontainer.sbValues.Append(string.Format("\t{0} : {1};\n", diagnosisInstance.Instance.Instance.Variable, requiredLib.Identifier + "." + diagnosisInstance.Instance.FbName));
								}
							}
							else
							{
								lmcontainer.sbValues.Append(string.Format("\t{0} : {1};\n", diagnosisInstance.Instance.Instance.Variable, diagnosisInstance.Instance.FbName));
							}
						}
					}
					if (bEnableDiagnosis)
					{
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)6, (ushort)40))
						{
							lmcontainer.sbValues.Append($"DeviceNodes : ARRAY[0..{val.Count - 1}] OF DED.INode;\n");
							lmcontainer.sbValues.Append("CAADEDTemp : POINTER TO BYTE := ADR(DED.g_itfRootINodeInst);\n");
						}
						else
						{
							lmcontainer.sbValues.Append($"DeviceNodes : ARRAY[0..{val.Count - 1}] OF DED.INode;\n");
							lmcontainer.sbValues.Append("CAADEDTemp : POINTER TO BYTE := ADR(g_itfRootINodeInst);\n");
						}
					}
				}
				_bLastDiagnosis = bEnableDiagnosis;
			}
			List<LibsToAddItem> list = new List<LibsToAddItem>();
			List<IRequiredLib3> list2 = new List<IRequiredLib3>();
			bIoStandardInserted = false;
			if (DeviceObjectHelper.RemovedLibs.Count > 0)
			{
				foreach (KeyValuePair<IRequiredLib, IIoProvider> keyValuePair in DeviceObjectHelper.RemovedLibs)
				{
					bool flag = true;
					foreach (KeyValuePair<IRequiredLib, IIoProvider> keyValuePair2 in dictRequiredLibs)
					{
						if (keyValuePair.Key.LibName == keyValuePair2.Key.LibName && keyValuePair.Key.Vendor == keyValuePair2.Key.Vendor && keyValuePair.Key.Version == keyValuePair2.Key.Version)
						{
							flag = false;
							break;
						}
						if (!string.IsNullOrEmpty((keyValuePair.Key as IRequiredLib3).PlaceHolderLib) && (keyValuePair.Key as IRequiredLib3).PlaceHolderLib == (keyValuePair2.Key as IRequiredLib3).PlaceHolderLib)
						{
							flag = false;
							break;
						}
					}
					foreach (IRequiredLib3 requiredLib5 in list2)
					{
						if (requiredLib5.LibName == keyValuePair.Key.LibName && requiredLib5.Vendor == keyValuePair.Key.Vendor && requiredLib5.Version == keyValuePair.Key.Version)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						list2.Add(keyValuePair.Key as IRequiredLib3);
					}
				}
				DeviceObjectHelper.RemovedLibs.Clear();
			}
			ILibManItem[] libItems = GetLibItems(moApplication);
			if (libItems != null)
			{
				string text3 = default(string);
				string text4 = default(string);
				string text5 = default(string);
				foreach (IRequiredLib3 item2 in list2)
				{
					ILibManItem[] array = libItems;
					foreach (ILibManItem val2 in array)
					{
						string text = string.Empty;
						string text2 = string.Empty;
						if (val2 is IPlaceholderLibManItem)
						{
							IPlaceholderLibManItem val3 = (IPlaceholderLibManItem)val2;
							if (val3.EffectiveResolution != null)
							{
								text2 = val3.PlaceholderName;
								text = val3.EffectiveResolution.DisplayName;
							}
							else if (val3.DefaultResolution != null)
							{
								text = val3.DefaultResolution;
							}
						}
						else
						{
							text = val2.Name;
						}
						if (!string.IsNullOrEmpty(text) && ((ILibraryLoader3)APEnvironment.LibraryLoader).ParseDisplayName(text, out text3, out text4, out text5) && ((!string.IsNullOrEmpty(text2) && text2 == item2.PlaceHolderLib) || (((text3 == ((IRequiredLib)item2).LibName) & (text4 == ((IRequiredLib)item2).Version)) && text5 == ((IRequiredLib)item2).Vendor)))
						{
							try
							{
								((ILibraryLoader)APEnvironment.LibraryLoader).UnloadSystemLibrary(val2.Name, moApplication.ProjectHandle, moApplication.ObjectGuid);
							}
							catch
							{
							}
						}
					}
				}
			}
			LDictionary<string, string> val4 = new LDictionary<string, string>();
			foreach (KeyValuePair<IRequiredLib, IIoProvider> keyValuePair3 in dictRequiredLibs)
			{
				IMetaObject metaObject = keyValuePair3.Value.GetMetaObject();
				if (metaObject != null && metaObject.Object is IDeviceObject && metaObject.ParentObjectGuid != Guid.Empty)
				{
					IDeviceObject deviceObject = (IDeviceObject)metaObject.Object;
					IDeviceIdentification deviceIdentification;
					if (deviceObject is IDeviceObject5)
					{
						deviceIdentification = (deviceObject as IDeviceObject5).DeviceIdentificationNoSimulation;
					}
					else
					{
						deviceIdentification = deviceObject.DeviceIdentification;
					}
					if (!(deviceIdentification is IModuleIdentification))
					{
						ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceIdentification);
						if (targetSettingsById != null && targetSettingsById.Sections["library-management"] != null)
						{
							ITargetSection targetSection = targetSettingsById.Sections["library-management"].SubSections["placeholder-libraries"];
							if (targetSection != null)
							{
								foreach (object obj2 in targetSection.Settings)
								{
									ITargetSetting targetSetting = (ITargetSetting)obj2;
									string text3 = null;
									if (!val4.TryGetValue(targetSetting.Name, out text3))
									{
										val4.Add(targetSetting.Name, targetSetting.DefaultValue.ToString());
									}
									else if (!string.IsNullOrEmpty(text3))
									{
										string a4;
										string version;
										string a5;
										APEnvironment.LibraryLoader.ParseDisplayName(text3, out a4, out version, out a5);
										string b;
										string version2;
										string b2;
										APEnvironment.LibraryLoader.ParseDisplayName(targetSetting.DefaultValue.ToString(), out b, out version2, out b2);
										if (a4 != b || a5 != b2)
										{
											lmcontainer.AddCompilerMessage("warning", "", 0L, hostGuid, string.Format(Strings.ErrorPlaceholderMismatch, text3, targetSetting.DefaultValue.ToString()));
											val4[targetSetting.Name] = null;
										}
										else
										{
											try
											{
												Version v = new Version(version);
												if (new Version(version2) > v)
												{
													val4[targetSetting.Name] = targetSetting.DefaultValue.ToString();
												}
											}
											catch
											{
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (hostGuid != Guid.Empty && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)0) && bLoadFinished)
			{
				DeviceObject deviceObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(moApplication.ProjectHandle, hostGuid).Object as DeviceObject;
				if (deviceObject != null)
				{
					bool flag2 = (deviceObject.PlaceholderResolutions == null) & (val4.Count > 0);
					if (!flag2 && deviceObject.PlaceholderResolutions != null)
					{
						foreach (KeyValuePair<string, string> keyValuePair4 in deviceObject.PlaceholderResolutions)
						{
							string a6 = null;
							if (!val4.TryGetValue(keyValuePair4.Key, out a6) || a6 != keyValuePair4.Value)
							{
								flag2 = true;
							}
						}
						foreach (KeyValuePair<string, string> keyValuePair5 in val4)
						{
							if (!string.IsNullOrEmpty(keyValuePair5.Value))
							{
								string a7 = null;
								if (!deviceObject.PlaceholderResolutions.TryGetValue(keyValuePair5.Key, out a7) || a7 != keyValuePair5.Value)
								{
									flag2 = true;
									break;
								}
							}
						}
					}
					if (flag2)
					{
						Guid[] subObjectGuids = moApplication.SubObjectGuids;
						foreach (Guid guid in subObjectGuids)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(moApplication.ProjectHandle, guid);
							if (!typeof(ILibManObject).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								continue;
							}
							LList<string> val9 = new LList<string>();
							IMetaObject val10 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
							IObject @object = val10.Object;
							ILibManObject5 val11 = (ILibManObject5)(object)((@object is ILibManObject5) ? @object : null);
							ILibManItem[] allLibraries = ((ILibManObject)val11).GetAllLibraries(true);
							if (allLibraries == null)
							{
								break;
							}
							LList<string> val12 = new LList<string>();
							ILibManItem[] array = allLibraries;
							foreach (ILibManItem val13 in array)
							{
								if (val13 is IPlaceholderLibManItem4)
								{
									IPlaceholderLibManItem4 val14 = (IPlaceholderLibManItem4)(object)((val13 is IPlaceholderLibManItem4) ? val13 : null);
									val12.Add(((IPlaceholderLibManItem)val14).PlaceholderName);
									if (val14.RedirectedResolution && val14.EffectiveResolutionIgnoreRedirection == null && val4.ContainsKey(((IPlaceholderLibManItem)val14).PlaceholderName))
									{
										val9.Add(((IPlaceholderLibManItem)val14).PlaceholderName);
									}
								}
							}
							if (val4.Count > 0)
							{
								foreach (string item in val4.Keys)
								{
									if (!val12.Contains(item))
									{
										val12.Add(item);
									}
								}
							}
							if (val9.Count <= 0)
							{
								break;
							}
							try
							{
								val10 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
								IObject object2 = val10.Object;
								val11 = (ILibManObject5)(object)((object2 is ILibManObject5) ? object2 : null);
								foreach (string item4 in val9)
								{
									val11.PlaceholderRedirection.SetRedirectedPlaceholderResolution(item4, (string)null);
								}
							}
							catch
							{
							}
							finally
							{
								if (val10 != null && val10.IsToModify)
								{
									((IObjectManager)APEnvironment.ObjectMgr).SetObject(val10, true, (object)null);
								}
							}
							break;
						}
						IMetaObject val15 = null;
						try
						{
							val15 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(moApplication.ProjectHandle, hostGuid);
							DeviceObject deviceObject2 = val15.Object as DeviceObject;
							deviceObject2.PlaceholderResolutions = new LDictionary<string, string>();
							foreach (KeyValuePair<string, string> keyValuePair6 in val4)
							{
								if (!string.IsNullOrEmpty(keyValuePair6.Value))
								{
									deviceObject2.PlaceholderResolutions[keyValuePair6.Key] = keyValuePair6.Value;
								}
							}
						}
						catch
						{
						}
						finally
						{
							if (val15 != null && val15.IsToModify)
							{
								((IObjectManager)APEnvironment.ObjectMgr).SetObject(val15, true, (object)null);
							}
						}
					}
				}
			}
			ResolveLibs(dictRequiredLibs, list, moApplication.ObjectGuid, bIsLogical, ref bIoStandardInserted);
			foreach (LibsToAddItem item5 in list)
			{
				if (item5.bInsertLibrary && !string.IsNullOrEmpty(item5.stLibName))
				{
					string stDisplayName = string.Empty;
					string stError = string.Empty;
					if (!IsLibraryInLibMan(moApplication, item5, libItems, bRemoveLibs: true, out var _, bLoadFinished) && !TryInsertLib(moApplication, item5.stLibName, item5.stLibVersion, item5.stLibVendor, out stDisplayName, out stError, item5.bLoadAsSystemLibrary, item5.bLoadAsPlaceHolder, item5.stPlaceholder))
					{
						lmcontainer.AddCompilerMessage("error", "", 0L, item5.ioProvider.GetMetaObject().ObjectGuid, string.Format(Strings.ErrorNoLib, stDisplayName, stError));
					}
				}
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)0))
			{
				IMetaObjectStub libManOfApp = GetLibManOfApp(moApplication);
				if (libManOfApp != null)
				{
					((IEngine2)APEnvironment.Engine).UpdateLanguageModel(moApplication.ProjectHandle, libManOfApp.ObjectGuid);
				}
			}
			libItems = GetLibItems(moApplication);
			if (libItems == null)
			{
				return;
			}
			LList<IIoProvider> val16 = new LList<IIoProvider>();
			foreach (LibsToAddItem item6 in list)
			{
				IIoProvider ioProvider = item6.ioProvider;
				IRequiredLib requiredLib5 = item6.requiredLib;
				ILibManItem foundLib2 = null;
				if (!string.IsNullOrEmpty(item6.stLibName) && !IsLibraryInLibMan(moApplication, item6, libItems, bRemoveLibs: false, out foundLib2, bLoadFinished))
				{
					continue;
				}
				foreach (FBInstance item7 in (IEnumerable)requiredLib5.FbInstances)
				{
					if (!(item7.Instance.Variable != ""))
					{
						continue;
					}
					InstanceLocation instanceLocation = InstanceLocation.GVL;
					if (item7 != null)
					{
						instanceLocation = item7.Location;
					}
					IMetaObject metaObject2 = ioProvider.GetMetaObject();
					if (metaObject2 != null)
					{
						if (bHideFbInstances)
						{
							lmcontainer.sbValues.Append("{attribute 'hide'}");
						}
						lmcontainer.sbValues.Append($"{{messageguid '{metaObject2.ObjectGuid.ToString()}'}}\n");
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0) && item7.LanguageModelPositionId != -1)
						{
							lmcontainer.sbValues.Append($"{{p {item7.LanguageModelPositionId} }}\n");
						}
						else
						{
							lmcontainer.sbValues.Append($"{{p {0} }}\n");
						}
					}
					string text13 = ((bEnableDiagnosis && !string.IsNullOrEmpty(item7.FbNameDiag)) ? item7.FbNameDiag : item7.FbName);
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0) && foundLib2 != null)
					{
						text13 = foundLib2.Namespace + "." + text13;
					}
					if (string.IsNullOrEmpty(text13))
					{
						continue;
					}
					if ((ioProvider.ParameterSet as ParameterSet).HasInstanceVariable)
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (Parameter item8 in (IEnumerable)ioProvider.ParameterSet)
						{
							if (string.IsNullOrEmpty(item8.FbInstanceVariable))
							{
								continue;
							}
							string fbInstanceVariable = item8.FbInstanceVariable;
							if (fbInstanceVariable.Contains("$(DeviceName)") && fbInstanceVariable.Contains("."))
							{
								int num = fbInstanceVariable.IndexOf('.');
								string value = fbInstanceVariable.Substring(0, num);
								if (!item7.BaseName.Equals(value, StringComparison.InvariantCultureIgnoreCase))
								{
									continue;
								}
								item8.WatchFbInstanceVariable = item7.Instance.Variable + fbInstanceVariable.Substring(num);
							}
							else if (!val16.Contains(ioProvider))
							{
								item8.WatchFbInstanceVariable = item7.Instance.Variable + "." + fbInstanceVariable;
							}
							else
							{
								string stMessage = "attribute instanceVarialbe has no $(DeviceName) but multiple fb instances used";
								lmcontainer.AddCompilerMessage("error", "", 0L, ioProvider.GetMetaObject().ObjectGuid, stMessage);
							}
							bool bDefault;
							string text14 = item8.DataElementBase.GetInitialization(out bDefault, ((int)item8.ChannelType == 2) | ((int)item8.ChannelType == 3), bCreateDefaultValue: false).Trim();
							if (text14.StartsWith("("))
							{
								text14 = text14.Substring(1);
							}
							if (text14.EndsWith(")"))
							{
								text14 = text14.Substring(0, text14.Length - 1);
							}
							if (string.IsNullOrEmpty(text14))
							{
								continue;
							}
							int num2 = 0;
							if (stringBuilder.Length > 0)
							{
								stringBuilder.Append(",");
							}
							else
							{
								stringBuilder.Append(":= (");
							}
							if (fbInstanceVariable.Contains("."))
							{
								string[] array2 = fbInstanceVariable.Split('.');
								string[] array3 = array2;
								foreach (string text15 in array3)
								{
									if (!text15.Contains("$(DeviceName)"))
									{
										stringBuilder.Append(text15);
										if (text15 == array2[array2.Length - 1] && text14.StartsWith("["))
										{
											stringBuilder.Append(":=");
											continue;
										}
										stringBuilder.Append(":=(");
										num2++;
									}
								}
							}
							else
							{
								stringBuilder.Append(fbInstanceVariable);
								stringBuilder.Append(":=(");
								num2 = 1;
							}
							stringBuilder.Append(text14);
							while (num2 > 0)
							{
								stringBuilder.Append(")");
								num2--;
							}
						}
						val16.Add(ioProvider);
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(")");
							text13 += stringBuilder.ToString();
						}
					}
					switch (instanceLocation)
					{
					case InstanceLocation.GVL:
						lmcontainer.sbValues.Append($"\t{item7.Instance.Variable} : {text13};\n");
						break;
					case InstanceLocation.Retain:
						lmcontainer.sbRetains.AppendFormat("\t{0} : {1};\n", item7.Instance.Variable, text13);
						break;
					case InstanceLocation.Input:
						lmcontainer.sbValues.Append($"\t{item7.Instance.Variable} AT {GetModuleBaseAddress(ioProvider, (DirectVariableLocation)1)} : {text13};\n");
						break;
					case InstanceLocation.Output:
						lmcontainer.sbValues.Append($"\t{item7.Instance.Variable} AT {GetModuleBaseAddress(ioProvider, (DirectVariableLocation)2)} : {text13};\n");
						break;
					default:
					{
						string stMessage2 = "Don't know how to put instance '" + item7.Instance.Variable + "' at location " + instanceLocation;
						lmcontainer.AddCompilerMessage("error", "", 0L, ioProvider.GetMetaObject().ObjectGuid, stMessage2);
						break;
					}
					}
				}
			}
		}

		public static void AddModuleIoLanguageModel(IIoProvider ioProvider, string stParentPointer, LanguageModelContainer lmcontainer, IMetaObject moApplication, ref int nNumModules, bool bCreateAdditionalParams, LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs, bool bSkipAdditionalParamsForZeroParams)
		{
			AddModuleIoLanguageModel(ioProvider, null, stParentPointer, lmcontainer, moApplication, ref nNumModules, bCreateAdditionalParams, bIsLogical: false, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
		}

		public static void AddModuleIoLanguageModel(IIoProvider ioProvider, IIoProvider ioProviderParent, string stParentPointer, LanguageModelContainer lmcontainer, IMetaObject moApplication, ref int nNumModules, bool bCreateAdditionalParams, bool bIsLogical, LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs, bool bSkipAdditionalParamsForZeroParams)
		{
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Expected O, but got Unknown
			IIoProvider[] children;
			if (ioProvider is ErrorConnector)
			{
				children = ioProvider.Children;
				for (int i = 0; i < children.Length; i++)
				{
					IMetaObject metaObject = children[i].GetMetaObject();
					string stMessage = "Device " + metaObject.Name + " is not allowed here. Please delete the device or move it to a valid position.";
					lmcontainer.AddCompilerMessage("error", "", 0L, metaObject.ObjectGuid, stMessage);
				}
				return;
			}
			nNumModules++;
			uint num = 0u;
			bool bHasPointerParams = (ioProvider.ParameterSet as ParameterSet).HasIoConfigAddress;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)50) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)))
			{
				num = (ioProvider.ParameterSet as ParameterSet).NumParams;
				if (num == 0)
				{
					num = GetNumDownloadParams(ioProvider.ParameterSet, out bHasPointerParams);
				}
			}
			else
			{
				num = GetNumDownloadParams(ioProvider.ParameterSet, out bHasPointerParams);
			}
			if (bHasPointerParams && ioProvider is Connector)
			{
				DeviceObject deviceObject = (ioProvider as Connector).GetDeviceObject() as DeviceObject;
				if (deviceObject != null)
				{
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)deviceObject, true);
				}
			}
			if ((ioProvider.ParameterSet as ParameterSet).NumParams == 0 || (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) && (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)62) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0))))
			{
				bool flag = false;
				if (ioProvider is ExplicitConnector)
				{
					flag = true;
				}
				if (ioProvider is Connector)
				{
					IDeviceObject deviceObject2 = (ioProvider as Connector).GetDeviceObject();
					if (deviceObject2 is SlotDeviceObject)
					{
						flag = !(deviceObject2 as SlotDeviceObject).HasDevice;
					}
				}
				if (!flag && (!bSkipAdditionalParamsForZeroParams || num != 0))
				{
					if (bCreateAdditionalParams)
					{
						num += 2;
					}
					if (ioProvider is IDeviceObject && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)0) && (ioProvider.DriverInfo as IDriverInfo4).UpdateIOsInStop)
					{
						num++;
					}
				}
			}
			string text;
			if (num != 0)
			{
				text = "0";
				IMetaObject metaObject2 = ioProvider.GetMetaObject();
				if (metaObject2 != null && metaObject2.ParentObjectGuid != Guid.Empty && metaObject2.SubObjectGuids.Length != 0 && metaObject2 != null && metaObject2.Object is DeviceObject && !(metaObject2.Object as DeviceObject).UseParentPLC)
				{
					Guid[] subObjectGuids = metaObject2.SubObjectGuids;
					foreach (Guid guid in subObjectGuids)
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObject2.ProjectHandle, guid);
						if (typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							num = 0u;
						}
					}
				}
				if (num != 0)
				{
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)40) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)5, (ushort)0) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)))
					{
						string text2 = "GVL_" + ioProvider.GetParamsListName() + "." + ioProvider.GetParamsListName();
						text = "ADR(" + text2 + ")";
					}
					else
					{
						text = "ADR(" + ioProvider.GetParamsListName() + ")";
					}
					if (metaObject2 != null && metaObject2.Object is DeviceObject && (metaObject2.Object as DeviceObject).NoIoDownload)
					{
						text = "0";
						num = 0u;
					}
				}
			}
			else
			{
				text = "0";
			}
			if (text == "0")
			{
				num = 0u;
			}
			if (nNumModules > 1 && (lmcontainer.lmBuilder == null || lmcontainer.struinitModules == null))
			{
				lmcontainer.sbModuleList.Append(",\n");
			}
			uint num2 = 0u;
			if (ioProvider is ConnectorBase)
			{
				num2 = (ioProvider as ConnectorBase).InitialStatusFlag;
			}
			num2 = ((!ioProvider.Disabled && (ioProviderParent == null || !ioProviderParent.Disabled)) ? (num2 | 1u) : (num2 & 0xFFFFFFFEu));
			if (ioProvider is Connector)
			{
				IDeviceObject deviceObject3 = (ioProvider as Connector).GetDeviceObject();
				IDeviceObject13 val = (IDeviceObject13)(object)((deviceObject3 is IDeviceObject13) ? deviceObject3 : null);
				if (val != null && val.InheritedDisable && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
				{
					num2 &= 0xFFFFFFFEu;
				}
			}
			if (lmcontainer.lmBuilder != null && lmcontainer.struinitModules != null)
			{
				List<IAssignmentExpression> list = new List<IAssignmentExpression>();
				IVariableExpression val2 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "wType");
				IVariableExpression val3 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwFlags");
				IVariableExpression val4 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwNumOfParameters");
				IVariableExpression val5 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "pParameterList");
				IVariableExpression val6 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "pFather");
				ILiteralExpression val7 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)ioProvider.TypeId, (TypeClass)3);
				ILiteralExpression val8 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)num2, (TypeClass)4);
				ILiteralExpression val9 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)num, (TypeClass)4);
				IExpression val10 = lmcontainer.lmBuilder.ParseExpression(text);
				IExpression val11 = lmcontainer.lmBuilder.ParseExpression(stParentPointer);
				list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val2, (IExpression)(object)val7));
				list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val3, (IExpression)(object)val8));
				list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val4, (IExpression)(object)val9));
				list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val5, val10));
				list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val6, val11));
				lmcontainer.struinitModules.Add(lmcontainer.lmBuilder.CreateStructureInitialisation((IExprementPosition)null, list));
			}
			else
			{
				lmcontainer.sbModuleList.Append($"\t(wType:={ioProvider.TypeId}, dwFlags:={num2}, dwNumOfParameters:={num}, pParameterList:={text}, pFather:={stParentPointer})");
			}
			foreach (IRequiredLib item in (IEnumerable)ioProvider.DriverInfo.RequiredLibs)
			{
				IRequiredLib val12 = item;
				if (!(val12 as RequiredLib).IsDiagnosisLib && !dictRequiredLibs.ContainsKey(val12))
				{
					dictRequiredLibs.Add(val12, ioProvider);
				}
			}
			string stParentPointer2 = $"ADR(moduleList[{nNumModules - 1}])";
			children = ioProvider.Children;
			foreach (IIoProvider val13 in children)
			{
				if (!val13.Excluded)
				{
					AddModuleIoLanguageModel(val13, ioProvider, stParentPointer2, lmcontainer, moApplication, ref nNumModules, bCreateAdditionalParams, bIsLogical, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
				}
			}
		}

		private static string GetModuleBaseAddress(IIoProvider ioProvider, DirectVariableLocation location)
		{
			string userBaseAddress = ioProvider.GetUserBaseAddress(location);
			if (userBaseAddress != null && userBaseAddress != string.Empty)
			{
				return userBaseAddress;
			}
			ChannelType val;
			if ((int)location == 1)
			{
				val = (ChannelType)1;
			}
			else
			{
				if ((int)location != 2)
				{
					return null;
				}
				val = (ChannelType)2;
			}
			foreach (IParameter item in (IEnumerable)ioProvider.ParameterSet)
			{
				IParameter val2 = item;
				if (val2.ChannelType == val)
				{
					return ((IDataElement)val2).IoMapping.IecAddress;
				}
			}
			return ((object)ioProvider.Strategy.ResolveBaseAddress(location, ioProvider)).ToString();
		}

		public static void FillLanguageModel(LDictionary<Guid, string> dictAllTask, IIoProvider ioProvider, LateLanguageModel late, ref int nModuleIndex, IPreCompileContext2 comcon, bool UpdateIOsInStop)
		{
			FillLanguageModel(dictAllTask, ioProvider, late, ref nModuleIndex, comcon, new Hashtable(), UpdateIOsInStop);
		}

		private static void FillLanguageModel(LDictionary<Guid, string> dictAllTask, IIoProvider ioProvider, LateLanguageModel late, ref int nModuleIndex, IPreCompileContext2 comcon, Hashtable htInstanceCounter, bool UpdateIOsInStop)
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Expected O, but got Unknown
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Expected O, but got Unknown
			object obj = htInstanceCounter[ioProvider.TypeId];
			uint num = ((obj != null) ? ((uint)obj + 1) : 0u);
			htInstanceCounter[ioProvider.TypeId] = num;
			string paramsListName = ioProvider.GetParamsListName();
			IVariable val3 = default(IVariable);
			IPrecompileScope val4 = default(IPrecompileScope);
			foreach (RequiredLib item in (IEnumerable)ioProvider.DriverInfo.RequiredLibs)
			{
				if (item.IsDiagnosisLib)
				{
					continue;
				}
				string text = string.Empty;
				if (typeof(IRequiredLib2).IsAssignableFrom(((object)item).GetType()))
				{
					text = ((IRequiredLib2)item).Client;
				}
				if (!(string.Empty == text))
				{
					continue;
				}
				foreach (IFbInstance item2 in (IEnumerable)item.FbInstances)
				{
					IFbInstance val = item2;
					if (!(val.Instance.Variable != ""))
					{
						continue;
					}
					string text2 = string.Empty;
					if (((ICollection)val.CyclicCalls).Count > 0)
					{
						text2 = ioProvider.DriverInfo.BusCycleTask;
						if (text2 == string.Empty || !late.CyclicCalls.IsTaskValid(text2))
						{
							text2 = string.Empty;
							IIoProvider parent = ioProvider.Parent;
							while (parent != null && text2 == string.Empty)
							{
								if (parent.DriverInfo.BusCycleTask != string.Empty)
								{
									text2 = parent.DriverInfo.BusCycleTask;
									if (!late.CyclicCalls.IsTaskValid(text2))
									{
										text2 = string.Empty;
									}
								}
								parent = parent.Parent;
							}
						}
					}
					ISignature fbSignature = null;
					try
					{
						IPrecompileScope val2 = ((IPreCompileContext)comcon).CreatePrecompileScope(Guid.Empty);
						if (val2 != null)
						{
							val2.FindDeclaration(val.Instance.Variable, out val3, out fbSignature, out val4);
						}
					}
					catch
					{
					}
					foreach (ICyclicCall item3 in (IEnumerable)val.CyclicCalls)
					{
						ICyclicCall call = item3;
						int nBusCycleTask = -1;
						if (ioProvider.DriverInfo is DriverInfo && text2 == string.Empty && (ioProvider.DriverInfo as DriverInfo).UseSlowestTask)
						{
							int num2 = ((!DeviceObjectHelper.ConfigModeList.ContainsValue(((ICompileContextCommon)comcon).ApplicationGuid)) ? DeviceObject.GetBusTaskChild(dictAllTask, late.CyclicCalls.TaskInfos, ioProvider.DriverInfo as DriverInfo) : DeviceObject.GetBusTaskConfigMode(late.CyclicCalls.TaskInfos, ioProvider.DriverInfo as DriverInfo));
							if (num2 >= -1)
							{
								nBusCycleTask = num2;
							}
						}
						late.CyclicCalls.Add(val, call, UpdateIOsInStop, text2, nBusCycleTask, fbSignature);
					}
					if (val.InitMethodName != "")
					{
						late.AddFbInit(ioProvider.TypeId, num, nModuleIndex, val.Instance.Variable, val.InitMethodName);
					}
				}
			}
			nModuleIndex++;
			GetParameterInitializationListNames(paramsListName, out var stCountVariable, out var stIdsVariable, out var stPointersVariable, out var stSizeVariable);
			string stCode = string.Format("\r\n\t{{IF defined (variable:{0})}}\r\n\t\tFOR __i:=0 TO {0} DO\r\n\t\t\tIoMgrWriteParameter(dwModuleType:={1}, dwInstance:={2}, dwParameterId:={3}[__i], pData:={4}[__i], dwBitSize:={5}[__i], dwBitOffset:=0);\r\n\t\tEND_FOR\r\n\t{{END_IF}}\r\n", stCountVariable, ioProvider.TypeId, num, stIdsVariable, stPointersVariable, stSizeVariable);
			late.AddAfterUpdateConfigurationCode(stCode);
			nModuleIndex++;
			IIoProvider[] children = ioProvider.Children;
			foreach (IIoProvider ioProvider2 in children)
			{
				FillLanguageModel(dictAllTask, ioProvider2, late, ref nModuleIndex, comcon, htInstanceCounter, UpdateIOsInStop);
			}
		}

		public static void FillLateLanguageModel(IApplicationObject app, LDictionary<Guid, string> dictAllTask, IIoProvider ioProvider, LateLanguageModel late, ref int nModuleIndex, ICompileContext comcon, bool bPlcAlwaysMapping, AlwaysMappingMode PlcMappingMode, int nBusTask, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfields, bool bMappingForLogical, Guid logAppGuid, bool bMappingNoSafetyApp, bool bShowMultipleMappingsAsError)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			FillLateLanguageModel(app, dictAllTask, ioProvider, late, ref nModuleIndex, comcon, new Hashtable(), bPlcAlwaysMapping, PlcMappingMode, nBusTask, directVarCRefs, htStartAddresses, bMotorolaBitfields, bMappingForLogical, logAppGuid, bMappingNoSafetyApp, bShowMultipleMappingsAsError, bSkipCheckOverlap: false);
		}

		private static void FillLateLanguageModel(IApplicationObject app, LDictionary<Guid, string> dictAllTask, IIoProvider ioProvider, LateLanguageModel late, ref int nModuleIndex, ICompileContext comcon, Hashtable htInstanceCounter, bool bPlcAlwaysMapping, AlwaysMappingMode PlcMappingMode, int nBusTask, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield, bool bMappingForLogical, Guid logAppGuid, bool bMappingNoSafetyApp, bool bShowMultipleMappingsAsError, bool bSkipCheckOverlap)
		{
			bool flag = bMappingForLogical || bMappingNoSafetyApp;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = bPlcAlwaysMapping;
			if (bMappingForLogical || bMappingNoSafetyApp)
			{
				flag = true;
				IMetaObject metaObject = ioProvider.GetMetaObject();
				if (metaObject != null)
				{
					IObject @object = metaObject.Object;
					ILogicalDevice val = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
					if (val != null)
					{
						foreach (IMappedDevice item in (IEnumerable)val.MappedDevices)
						{
							IMappedDevice val2 = item;
							if (!val2.IsMapped)
							{
								continue;
							}
							string stName;
							Guid guid = ((!(val is LogicalIODevice)) ? LogicalIOHelper.GetLogicalAppForDevice(metaObject.ProjectHandle, val2.GetMappedDevice, out stName) : LogicalIOHelper.GetLogicalAppForDevice(metaObject.ProjectHandle, metaObject.ObjectGuid, out stName));
							if (val is ILogicalDevice2 && ((ILogicalDevice2)((val is ILogicalDevice2) ? val : null)).MappingPossible)
							{
								flag3 = true;
							}
							if (logAppGuid == guid)
							{
								if (!bMappingNoSafetyApp)
								{
									flag4 = true;
								}
								flag = false;
							}
						}
					}
				}
			}
			else if (DeviceObjectHelper.GenerateCodeForLogicalDevices)
			{
				IMetaObject metaObject2 = ioProvider.GetMetaObject();
				if (metaObject2 != null)
				{
					IObject object2 = metaObject2.Object;
					ILogicalDevice val3 = (ILogicalDevice)(object)((object2 is ILogicalDevice) ? object2 : null);
					if (val3 != null && val3.MappedDevices != null)
					{
						if (val3 is ILogicalDevice2 && ((ILogicalDevice2)((val3 is ILogicalDevice2) ? val3 : null)).MappingPossible)
						{
							flag3 = true;
						}
						else
						{
							foreach (IMappedDevice item2 in (IEnumerable)val3.MappedDevices)
							{
								if (item2.IsMapped)
								{
									flag4 = false;
									flag2 = true;
								}
							}
						}
					}
				}
			}
			uint num = 0u;
			string stBaseName = string.Empty;
			ConnectorMap connectorMap = null;
			if (!flag)
			{
				object obj = htInstanceCounter[ioProvider.TypeId];
				num = ((obj != null) ? ((uint)obj + 1) : 0u);
				htInstanceCounter[ioProvider.TypeId] = num;
				stBaseName = ioProvider.GetParamsListName();
				IMetaObject metaObject3 = ioProvider.GetMetaObject();
				if (ioProvider.DriverInfo is DriverInfo)
				{
					string busCycleTask = ioProvider.DriverInfo.BusCycleTask;
					if (!string.IsNullOrEmpty(busCycleTask) && !busCycleTask.StartsWith("<"))
					{
						int num2 = ((!DeviceObjectHelper.ConfigModeList.ContainsValue(((ICompileContextCommon)comcon).ApplicationGuid)) ? DeviceObject.GetBusTaskChild(dictAllTask, late.CyclicCalls.TaskInfos, ioProvider.DriverInfo as DriverInfo) : DeviceObject.GetBusTaskConfigMode(late.CyclicCalls.TaskInfos, ioProvider.DriverInfo as DriverInfo));
						if (num2 >= -1)
						{
							nBusTask = num2;
						}
					}
				}
				if (metaObject3.Object is IDeviceObject5)
				{
					ITargetSettingsList val4 = APEnvironment.TargetSettingsMgr.Settings;
					IObject object3 = metaObject3.Object;
					ITargetSettings targetSettingsById = ((ITargetSettingsList)val4).GetTargetSettingsById(((IDeviceObject5)((object3 is IDeviceObject5) ? object3 : null)).DeviceIdentificationNoSimulation);
					if (targetSettingsById != null)
					{
						bShowMultipleMappingsAsError = targetSettingsById.GetBoolValue(LocalTargetSettings.ShowMultipleTaskMappingsAsError.Path, bShowMultipleMappingsAsError);
					}
				}
				connectorMap = new ConnectorMap((ITaskMappingInfo)(object)new MappingInfo(ioProvider, nModuleIndex), ioProvider.ParameterSet, $"ADR(moduleList[{nModuleIndex}])", metaObject3.ProjectHandle, metaObject3.ObjectGuid, metaObject3.Name, nBusTask, bShowMultipleMappingsAsError);
				connectorMap.SkipOverlapCheck = bSkipCheckOverlap;
				AlwaysMappingMode mappingMode = PlcMappingMode;
				if (!flag2)
				{
					if (typeof(IConnector6).IsAssignableFrom(((object)ioProvider).GetType()))
					{
						IConnector6 val5 = (IConnector6)ioProvider;
						if (val5.AlwaysMapping)
						{
							flag4 |= val5.AlwaysMapping;
							mappingMode = ((IConnector11)((val5 is IConnector11) ? val5 : null)).AlwaysMappingMode;
						}
					}
					if (ioProvider.ParameterSet != null && ParameterSet4_GetAlwaysMapping(ioProvider.ParameterSet))
					{
						flag4 |= ParameterSet4_GetAlwaysMapping(ioProvider.ParameterSet);
						if (ioProvider.ParameterSet is IParameterSet5)
						{
							IParameterSet parameterSet = ioProvider.ParameterSet;
							mappingMode = ((IParameterSet5)((parameterSet is IParameterSet5) ? parameterSet : null)).AlwaysMappingMode;
						}
					}
				}
				bool flag5 = false;
				bool flag6 = false;
				foreach (Parameter item3 in DeviceObjectHelper.SortedParameterSet(ioProvider))
				{
					if (htStartAddresses != null)
					{
						if (!flag5 && (int)item3.ChannelType == 1)
						{
							flag5 = true;
							LateLanguageStartAddresses lateLanguageStartAddresses;
							if (htStartAddresses.ContainsKey(ioProvider))
							{
								lateLanguageStartAddresses = htStartAddresses[ioProvider] as LateLanguageStartAddresses;
							}
							else
							{
								lateLanguageStartAddresses = new LateLanguageStartAddresses();
								htStartAddresses.Add(ioProvider, lateLanguageStartAddresses);
							}
							lateLanguageStartAddresses.startInAddress = (item3.IoMapping as IoMapping).GetIecAddress(htStartAddresses);
						}
						if (!flag6 && ((int)item3.ChannelType == 2 || (int)item3.ChannelType == 3))
						{
							flag6 = true;
							LateLanguageStartAddresses lateLanguageStartAddresses2;
							if (htStartAddresses.ContainsKey(ioProvider))
							{
								lateLanguageStartAddresses2 = htStartAddresses[ioProvider] as LateLanguageStartAddresses;
							}
							else
							{
								lateLanguageStartAddresses2 = new LateLanguageStartAddresses();
								htStartAddresses.Add(ioProvider, lateLanguageStartAddresses2);
							}
							lateLanguageStartAddresses2.startOutAddress = (item3.IoMapping as IoMapping).GetIecAddress(htStartAddresses);
						}
					}
					try
					{
						if (!bMappingForLogical && !bMappingNoSafetyApp && (int)item3.ChannelType != 0)
						{
							item3.AddChannels(late.AddrToChannelMap, connectorMap, comcon, htStartAddresses, bSkipCheckOverlap);
						}
					}
					catch (ArgumentException ex)
					{
						IMetaObject metaObject4 = ioProvider.GetMetaObject();
						string arg = string.Empty;
						if (metaObject4 != null)
						{
							arg = metaObject4.Name;
						}
						string text = $"{{error '{arg}: {ex.Message}'}}\n";
						if (metaObject4 != null)
						{
							text = $"{{messageguid '{metaObject4.ObjectGuid}'}}\n" + text;
						}
						if (item3.LanguageModelPositionId != -1)
						{
							text = $"{{p {item3.LanguageModelPositionId} }}" + text;
						}
						late.AddAfterUpdateConfigurationCode(text);
					}
					if (flag3 && (int)item3.ChannelType != 0)
					{
						AccessRight accessRight = item3.GetAccessRight(bOnline: false);
						if (bMappingForLogical || bMappingNoSafetyApp)
						{
							if ((int)accessRight != 0)
							{
								continue;
							}
						}
						else if ((int)accessRight == 0)
						{
							continue;
						}
					}
					if (!flag3 || !item3.LogicalParameter || bMappingForLogical)
					{
						item3.AddMapping(connectorMap, stBaseName, flag4, mappingMode, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
						if (ioProvider.IoUpdateTask != null)
						{
							item3.AddFixedUpdates(connectorMap, late.FixedTaskUpdates, ioProvider.IoUpdateTask, htStartAddresses);
						}
					}
				}
				late.ConnectorMapList.Add(connectorMap);
			}
			bool flag7 = false;
			bool flag8 = false;
			DriverInfo driverInfo = ioProvider.DriverInfo as DriverInfo;
			if (driverInfo != null && driverInfo.NeedsBusCycleBeforeRead)
			{
				flag7 = true;
			}
			if (driverInfo != null && driverInfo.SkipOverlapCheck)
			{
				bSkipCheckOverlap = true;
				if (connectorMap != null)
				{
					connectorMap.SkipOverlapCheck = bSkipCheckOverlap;
				}
			}
			flag8 = ioProvider.DriverInfo.NeedsBusCycle;
			bool flag9 = !flag && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)10, (ushort)0) && app is IDeviceApplication;
			string text2 = ioProvider.DriverInfo.BusCycleTask;
			if (flag8 || flag7)
			{
				if (text2 == string.Empty || !late.CyclicCalls.IsTaskValid(text2))
				{
					text2 = string.Empty;
					IIoProvider parent = ioProvider.Parent;
					while (parent != null && text2 == string.Empty)
					{
						if (parent.DriverInfo.BusCycleTask != string.Empty)
						{
							text2 = parent.DriverInfo.BusCycleTask;
							if (!late.CyclicCalls.IsTaskValid(text2))
							{
								text2 = string.Empty;
							}
						}
						parent = parent.Parent;
					}
				}
				late.AddrToChannelMap.AddStartBusCycle(nModuleIndex, text2, flag7, flag8, flag9);
				late.AddrToChannelMap.AddWatchdogTrigger(nModuleIndex, text2);
			}
			if (!flag)
			{
				foreach (RequiredLib item4 in (IEnumerable)ioProvider.DriverInfo.RequiredLibs)
				{
					if (item4.IsDiagnosisLib)
					{
						continue;
					}
					string text3 = string.Empty;
					if (typeof(IRequiredLib2).IsAssignableFrom(((object)item4).GetType()))
					{
						text3 = ((IRequiredLib2)item4).Client;
					}
					if (!(string.Empty == text3))
					{
						continue;
					}
					foreach (IFbInstance item5 in (IEnumerable)item4.FbInstances)
					{
						IFbInstance val6 = item5;
						if (val6.Instance.Variable != "" && val6.InitMethodName != "")
						{
							late.AddFbInit(ioProvider.TypeId, num, nModuleIndex, val6.Instance.Variable, val6.InitMethodName);
						}
						if (!flag9 || ((ICollection)val6.CyclicCalls).Count <= 0)
						{
							continue;
						}
						foreach (ICyclicCall item6 in (IEnumerable)val6.CyclicCalls)
						{
							ICyclicCall val7 = item6;
							if (val7.Task == "#buscycletask" || val7.Task == "#userdeftask")
							{
								late.AddrToChannelMap.AddStartBusCycle(nModuleIndex, text2, bBeforeReadInputs: false, bAfterWriteOutputs: false, flag9);
							}
						}
					}
				}
				GetParameterInitializationListNames(stBaseName, out var stCountVariable, out var stIdsVariable, out var stPointersVariable, out var stSizeVariable);
				string stCode = string.Format("\r\n\t{{IF defined (variable:{0})}}\r\n\t\tFOR __i:=0 TO {0} DO\r\n\t\t\tIoMgrWriteParameter(dwModuleType:={1}, dwInstance:={2}, dwParameterId:={3}[__i], pData:={4}[__i], dwBitSize:={5}[__i], dwBitOffset:=0);\r\n\t\tEND_FOR\r\n\t{{END_IF}}\r\n", stCountVariable, ioProvider.TypeId, num, stIdsVariable, stPointersVariable, stSizeVariable);
				late.AddAfterUpdateConfigurationCode(stCode);
			}
			nModuleIndex++;
			if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)20))
			{
				flag4 = bPlcAlwaysMapping;
			}
			IIoProvider[] children = ioProvider.Children;
			foreach (IIoProvider ioProvider2 in children)
			{
				FillLateLanguageModel(app, dictAllTask, ioProvider2, late, ref nModuleIndex, comcon, htInstanceCounter, flag4, PlcMappingMode, nBusTask, directVarCRefs, htStartAddresses, bMotorolaBitfield, bMappingForLogical, logAppGuid, bMappingNoSafetyApp, bShowMultipleMappingsAsError, bSkipCheckOverlap);
			}
		}

		private static bool ParameterSet4_GetAlwaysMapping(IParameterSet parameterSet)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (parameterSet is IParameterSet4)
			{
				return ((IParameterSet4)parameterSet).AlwaysMapping;
			}
			if (parameterSet is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)parameterSet).IsFunctionAvailable("GetAlwaysMapping"))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
				return XmlConvert.ToBoolean(((IGenericInterfaceExtensionProvider)parameterSet).CallFunction("GetAlwaysMapping", xmlDocument).DocumentElement.InnerText);
			}
			return false;
		}

		public static void GetParameterInitializationListNames(string stBaseName, out string stCountVariable, out string stIdsVariable, out string stPointersVariable, out string stSizeVariable)
		{
			stCountVariable = stBaseName + "_ParamValuesCount";
			stIdsVariable = stBaseName + "_ParamValuesIds";
			stPointersVariable = stBaseName + "_ParamValuesPointers";
			stSizeVariable = stBaseName + "_ParamValuesSizes";
		}

		public static int GetMaxBitSize(IAddressCodePosition[] positions)
		{
			int num = -1;
			foreach (IAddressCodePosition val in positions)
			{
				num = Math.Max(num, val.TypeSize);
			}
			return num switch
			{
				-1 => 0, 
				0 => 1, 
				_ => num * 8, 
			};
		}

		internal static bool CheckSkipWarning(ISourcePosition sourcePosition, ISignature sign)
		{
			bool result = false;
			IPreCompileContext val = default(IPreCompileContext);
			IExprement val2 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).FindExpressionAtSourcePosition(sourcePosition, (WhatToFind)2, out val);
			string[] array = sign.GetAttributeValue(ATTRIBUTEIGNORE).Split(',');
			if (val2 is IExpressionStatement)
			{
				IExpression val3 = ((IExpressionStatement)((val2 is IExpressionStatement) ? val2 : null)).Expr;
				if (val3 is IAssignmentExpression)
				{
					val3 = ((IAssignmentExpression)((val3 is IAssignmentExpression) ? val3 : null)).RValue;
				}
				string text = string.Empty;
				if (val3 is IAddressExpression)
				{
					text = ((object)((IAddressExpression)((val3 is IAddressExpression) ? val3 : null)).DirectAddress).ToString();
				}
				if (val3 is IVariableExpression)
				{
					text = ((IVariableExpression)((val3 is IVariableExpression) ? val3 : null)).Name;
				}
				if (val3 is ICompoAccessExpression)
				{
					text = ((IExprement)((val3 is ICompoAccessExpression) ? val3 : null)).ToString();
				}
				if (!string.IsNullOrEmpty(text))
				{
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (string.Compare(array2[i].Trim(), text, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		internal static bool CheckSkipWarning(ICompileContext comcon, VariableCrossRef varRef)
		{
			bool result = false;
			if (varRef == null || varRef.CrossReference == null)
			{
				return result;
			}
			ISignature signatureById = comcon.GetSignatureById(varRef.CrossReference.CodeId);
			if (signatureById != null && signatureById.HasAttribute(ATTRIBUTEIGNORE) && varRef.Variable != null && comcon is ICompileContext10)
			{
				int num = -1;
				num = ((varRef.SignatureId != -1) ? varRef.SignatureId : signatureById.Id);
				{
					foreach (ICodePosition item in ((ICompileContext10)((comcon is ICompileContext10) ? comcon : null)).GetReferencePositionsOfPOUEx(varRef.CrossReference.CodeId, num, varRef.Variable.Id))
					{
						if (CheckSkipWarning((ISourcePosition)(object)new SourcePosition(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, signatureById.ObjectGuid, item.EditorPosition, item.PositionOffset, 0), signatureById))
						{
							return true;
						}
					}
					return result;
				}
			}
			return result;
		}

		internal static bool CheckSkipWarning(ICompileContext comcon, IAddressCrossReference cref)
		{
			bool result = false;
			ISignature signatureById = comcon.GetSignatureById(cref.CodeId);
			if (signatureById != null && signatureById.HasAttribute(ATTRIBUTEIGNORE))
			{
				IAddressCodePosition[] positions = cref.Positions;
				foreach (IAddressCodePosition val in positions)
				{
					if (CheckSkipWarning((ISourcePosition)(object)new SourcePosition(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, signatureById.ObjectGuid, val.EditorPosition, val.PositionOffset, 0), signatureById))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public static void GetTaskMappings(int iTaskNbr, AddrToChannelMap map, TaskMapList taskmaplist, DoubleAddressTaskChecker checker, LList<DirectVarCrossRef> directVarCRefList, VariableCrossRef[] variableCRefList, FixedTaskUpdate[] fixedTaskUpdates, ICompileContext comcon, Hashtable htStartAddresses, bool bAlwaysMapToNew, bool bShowAsError)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Invalid comparison between Unknown and I4
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Expected O, but got Unknown
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Invalid comparison between Unknown and I4
			LDictionary<IIoProvider, LList<IIoProvider>> val = new LDictionary<IIoProvider, LList<IIoProvider>>();
			LList<IIoProvider> mappedIoProvider = default(LList<IIoProvider>);
			foreach (DirectVarCrossRef directVarCRef in directVarCRefList)
			{
				if (directVarCRef.DataLocation == null || directVarCRef.BitDataLocation == null)
				{
					taskmaplist.AddErrorMsg(Strings.ErrorAddressWrong, directVarCRef, comcon, !bShowAsError);
					continue;
				}
				int i = map.FindFirstPossibleIntersection(directVarCRef.DirectVariable, directVarCRef.BitDataLocation);
				if (i < 0)
				{
					taskmaplist.AddErrorMsg(Strings.ErrorAddressOutsideConfig, directVarCRef, comcon, !bShowAsError);
					continue;
				}
				int maxBitSize = GetMaxBitSize(directVarCRef.CrossRef.Positions);
				IList<ChannelRef> channelRefList = map.GetChannelRefList(directVarCRef.DirectVariable.Location);
				bool flag = false;
				for (; i < channelRefList.Count; i++)
				{
					ChannelRef channelRef = channelRefList[i];
					channelRef.GetIntersection(directVarCRef.BitDataLocation, maxBitSize, out var nChannelStartBit, out var nLocationStartBit, out var nNumBits);
					if (nNumBits <= 0)
					{
						break;
					}
					int nParamBitOffset = channelRef.BitOffset + nChannelStartBit;
					int num = 0;
					if (directVarCRef.DataLocation.IsBitLocation)
					{
						num = directVarCRef.DataLocation.BitNr;
					}
					num += nLocationStartBit;
					if (DeviceObjectHelper.GenerateCodeForLogicalDevices && (int)directVarCRef.DirectVariable.Location == 2)
					{
						if (!val.TryGetValue(channelRef.MappingInfo.IoProvider, out mappedIoProvider))
						{
							mappedIoProvider = DeviceObjectHelper.GetMappedIoProvider(channelRef.MappingInfo.IoProvider, bCheckForLogical: false);
							val.Add(channelRef.MappingInfo.IoProvider, mappedIoProvider);
						}
						if (mappedIoProvider.Count > 0)
						{
							bool flag2 = true;
							IMetaObject metaObject = channelRef.MappingInfo.IoProvider.GetMetaObject();
							if (metaObject != null && metaObject.Object is ILogicalDevice2 && (metaObject.Object as ILogicalDevice2).MappingPossible)
							{
								Parameter parameter = channelRef.MappingInfo.IoProvider.ParameterSet.GetParameter((long)channelRef.ParamId) as Parameter;
								if (parameter == null || !parameter.LogicalParameter)
								{
									flag2 = false;
								}
							}
							if (flag2)
							{
								taskmaplist.AddErrorMsg(Strings.ErrorOutputMapped, directVarCRef, comcon, bWarning: false);
								continue;
							}
						}
					}
					IDataElement val2 = channelRef.DataElement;
					if (val2 is DataElementBitFieldType && ((object)(val2.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses)).ToString() != ((object)directVarCRef.DirectVariable).ToString())
					{
						foreach (IDataElement item in (IEnumerable)val2.SubElements)
						{
							IDataElement val3 = item;
							if (((object)(val3.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses)).ToString() == ((object)directVarCRef.DirectVariable).ToString())
							{
								val2 = val3;
							}
						}
					}
					bool flag3 = false;
					flag3 = CheckSkipWarning(comcon, directVarCRef.CrossRef);
					if (!taskmaplist.Add(iTaskNbr, channelRef.ConnectorMap, channelRef.ParamId, ((object)directVarCRef.DirectVariable).ToString(), nParamBitOffset, num, nNumBits, directVarCRef.BitDataLocation.BitOffset, channelRef.BaseType, directVarCRef.BitDataLocation.BitOffset, checker, val2, flag3))
					{
						taskmaplist.AddErrorMsg(Strings.ErrorChannelAlreadyUsed, directVarCRef, comcon, !channelRef.ConnectorMap.ShowMultipleMappingsAsError);
					}
					flag = true;
				}
				if (!flag)
				{
					taskmaplist.AddErrorMsg(Strings.ErrorAddressOutsideConfig, directVarCRef, comcon, !bShowAsError);
				}
			}
			foreach (FixedTaskUpdate fixedTaskUpdate in fixedTaskUpdates)
			{
				if (!taskmaplist.Add(iTaskNbr, fixedTaskUpdate.ConnectorMap, (uint)fixedTaskUpdate.ChannelMap.ParameterId, fixedTaskUpdate.Address, fixedTaskUpdate.ChannelMap.ParamBitoffset, fixedTaskUpdate.BitOffset, (int)fixedTaskUpdate.ChannelMap.BitSize, 0L, fixedTaskUpdate.ChannelMap.Type, fixedTaskUpdate.BitOffset, checker, fixedTaskUpdate.DataElement, bNoOutputCheck: false))
				{
					taskmaplist.AddErrorMsg(Strings.ErrorChannelAlreadyUsed, (DirectVarCrossRef)null, comcon, !fixedTaskUpdate.ConnectorMap.ShowMultipleMappingsAsError);
				}
			}
			bool flag5 = default(bool);
			LList<IIoProvider> mappedIoProvider2 = default(LList<IIoProvider>);
			bool flag10 = default(bool);
			foreach (VariableCrossRef variableCrossRef in variableCRefList)
			{
				VariableDeclaration variableDeclaration = variableCrossRef.VariableDeclaration;
				bool flag4 = variableDeclaration.Variable.Variable.StartsWith("%");
				if (flag4 || bAlwaysMapToNew)
				{
					IDataLocation val4 = comcon.LocateAddress(out flag5, variableDeclaration.Channel.IecAddress);
					if (flag5 || val4 == null)
					{
						continue;
					}
					BitDataLocation bitDataLocation = new BitDataLocation(val4);
					int k = map.FindFirstPossibleIntersection(variableDeclaration.Channel.IecAddress, bitDataLocation);
					if (k < 0)
					{
						continue;
					}
					int bitSize = (int)variableDeclaration.Channel.BitSize;
					IList<ChannelRef> list = null;
					string text = (flag4 ? variableDeclaration.Variable.Variable : ((object)variableDeclaration.Channel.IecAddress).ToString());
					switch (text.Substring(0, 2).ToLowerInvariant())
					{
					case "%i":
						list = map.GetChannelRefList((DirectVariableLocation)1);
						break;
					case "%q":
						list = map.GetChannelRefList((DirectVariableLocation)2);
						break;
					case "%m":
						list = map.GetChannelRefList((DirectVariableLocation)3);
						break;
					default:
						continue;
					}
					for (; k < list.Count; k++)
					{
						ChannelRef channelRef2 = list[k];
						channelRef2.GetIntersection(bitDataLocation, bitSize, out var nChannelStartBit2, out var nLocationStartBit2, out var nNumBits2);
						if (nNumBits2 <= 0)
						{
							break;
						}
						int nParamBitOffset2 = channelRef2.BitOffset + nChannelStartBit2;
						int num2 = 0;
						if (val4.IsBitLocation)
						{
							num2 = val4.BitNr;
						}
						num2 += nLocationStartBit2;
						if (DeviceObjectHelper.GenerateCodeForLogicalDevices && !variableDeclaration.Channel.IsInput)
						{
							if (!val.TryGetValue(channelRef2.MappingInfo.IoProvider, out mappedIoProvider2))
							{
								mappedIoProvider2 = DeviceObjectHelper.GetMappedIoProvider(channelRef2.MappingInfo.IoProvider, bCheckForLogical: false);
								val.Add(channelRef2.MappingInfo.IoProvider, mappedIoProvider2);
							}
							if (mappedIoProvider2.Count > 0)
							{
								bool flag6 = true;
								IMetaObject metaObject2 = channelRef2.MappingInfo.IoProvider.GetMetaObject();
								if (metaObject2 != null && metaObject2.Object is ILogicalDevice2 && (metaObject2.Object as ILogicalDevice2).MappingPossible)
								{
									Parameter parameter2 = channelRef2.MappingInfo.IoProvider.ParameterSet.GetParameter((long)channelRef2.ParamId) as Parameter;
									if (parameter2 == null || !parameter2.LogicalParameter)
									{
										flag6 = false;
									}
								}
								if (flag6)
								{
									List<long> list2 = new List<long>();
									list2.Add(variableCrossRef.VariableDeclaration.Channel.LanguageModelPositionId);
									taskmaplist.AddErrorMsg(Strings.ErrorOutputMapped, variableCrossRef.VariableDeclaration.Connector.ObjectGuid, list2, variableCrossRef.VariableDeclaration.Variable.Variable, bWarning: false);
									continue;
								}
							}
						}
						bool flag7 = false;
						flag7 = CheckSkipWarning(comcon, variableCrossRef);
						if (!taskmaplist.Add(iTaskNbr, variableDeclaration.Connector, (uint)variableDeclaration.Channel.ParameterId, text, nParamBitOffset2, num2, nNumBits2, bitDataLocation.BitOffset, channelRef2.BaseType, bitDataLocation.BitOffset, checker, variableDeclaration.Channel.DataElement, flag7))
						{
							List<long> list3 = new List<long>();
							list3.Add(variableCrossRef.VariableDeclaration.Channel.LanguageModelPositionId);
							taskmaplist.AddErrorMsg(Strings.ErrorChannelAlreadyUsed, variableCrossRef.VariableDeclaration.Connector.ObjectGuid, list3, variableCrossRef.VariableDeclaration.Variable.Variable, !variableDeclaration.Connector.ShowMultipleMappingsAsError);
						}
					}
					if (flag4)
					{
						continue;
					}
					string plainVariableName = variableDeclaration.Variable.GetPlainVariableName();
					DataElementBase dataElementBase = variableDeclaration.Variable.Parent as DataElementBase;
					bool flag8 = false;
					if (dataElementBase.HasBaseType)
					{
						ICompiledType val5 = Types.ParseType(dataElementBase.BaseType);
						if (val5 != null && (val5.IsInteger || (int)((IType)val5.BaseType).Class == 0 || (int)((IType)val5.BaseType).Class == 1))
						{
							flag8 = true;
						}
					}
					string text2 = ((!flag8 || (dataElementBase != null && !dataElementBase.HasBaseType)) ? ((!variableDeclaration.Channel.IsInput) ? string.Format("SysMemCpy(ADR({0}),ADR({1}),SIZEOF({1}));", text, plainVariableName) : string.Format("SysMemCpy(ADR({0}),ADR({1}),SIZEOF({0}));", plainVariableName, text)) : ((!variableDeclaration.Channel.IsInput) ? $"{text} := {plainVariableName};" : $"{plainVariableName} := {text};"));
					taskmaplist.MapToExisting.Add(text2);
				}
				else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)1, (ushort)20))
				{
					if (variableCrossRef != null && variableCrossRef.Variable != null)
					{
						try
						{
							IScope val6 = comcon.CreateGlobalIScope();
							int num3 = variableCrossRef.Variable.CompiledType.Size(val6) * 8;
							if (variableDeclaration.Channel.BitSize > num3)
							{
								List<long> list4 = new List<long>();
								if (variableDeclaration.Channel.LanguageModelPositionId != -1)
								{
									list4.Add(variableDeclaration.Channel.LanguageModelPositionId);
								}
								taskmaplist.AddErrorMsg(Strings.ErrorTypeMismatch, variableDeclaration.Connector.ObjectGuid, list4, variableDeclaration.Variable.Variable, bWarning: false);
								continue;
							}
						}
						catch
						{
						}
					}
					string stBaseType = variableDeclaration.Channel.Type;
					DataElementBase dataElementBase2 = variableDeclaration.Variable.Parent as DataElementBase;
					if (dataElementBase2 == null)
					{
						continue;
					}
					if (variableDeclaration.Channel.BitSize == 1 && variableDeclaration.Channel.ParentType != string.Empty)
					{
						stBaseType = variableDeclaration.Channel.ParentType;
					}
					bool flag9 = dataElementBase2.HasBaseType;
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)14, (ushort)0) && dataElementBase2 is DataElementArrayType && (dataElementBase2.SubElements[0] as IDataElement2).HasBaseType)
					{
						flag9 = true;
					}
					if (flag9)
					{
						if (!taskmaplist.Add(iTaskNbr, variableCrossRef, comcon, variableDeclaration, variableDeclaration.Channel.ParamBitoffset, variableDeclaration.Channel.IecBitoffset, (int)variableDeclaration.Channel.BitSize, stBaseType, checker, (IDataElement)(object)dataElementBase2))
						{
							taskmaplist.AddErrorMsg(Strings.ErrorChannelAlreadyUsed, variableCrossRef, comcon, !variableDeclaration.Connector.ShowMultipleMappingsAsError);
						}
						continue;
					}
					IDirectVariable iecAddress = ((IoMapping)(object)dataElementBase2.GetParameter().IoMapping).GetIecAddress(htStartAddresses);
					BitDataLocation bitDataLocation2 = new BitDataLocation(comcon.LocateAddress(out flag10, iecAddress));
					iecAddress = ((IoMapping)(object)dataElementBase2.IoMapping).GetIecAddress(htStartAddresses);
					BitDataLocation bitDataLocation3 = new BitDataLocation(comcon.LocateAddress(out flag10, iecAddress));
					int nSize = -1;
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0) && variableCrossRef != null && variableCrossRef.VariableDeclaration != null && variableCrossRef.VariableDeclaration.Variable != null)
					{
						string text3 = variableCrossRef.VariableDeclaration.Variable.Variable.Substring(variableCrossRef.VariableDeclaration.Variable.Variable.IndexOf('.') + 1);
						if (!string.IsNullOrEmpty(text3))
						{
							IScope val7 = comcon.CreateGlobalIScope();
							IExpressionTypifier obj2 = ((ILanguageModelManager9)APEnvironment.LanguageModelMgr).CreateTypifier(((ICompileContextCommon)comcon).ApplicationGuid, -1, false, false);
							IExpressionTypifier2 val8 = (IExpressionTypifier2)(object)((obj2 is IExpressionTypifier2) ? obj2 : null);
							IScanner val9 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(text3, false, false, false, false);
							IExpression val10 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateParser(val9).ParseOperand();
							if (val10 != null && val8 != null)
							{
								val8.TypifyExpression(val10);
								if (val10.Type != null)
								{
									nSize = val10.Type.Size(val7) * 8;
								}
							}
						}
					}
					AddTaskMapListForComplexTypes(iTaskNbr, variableCrossRef, dataElementBase2, taskmaplist, checker, variableDeclaration, comcon, bitDataLocation2.BitOffset, bitDataLocation3.BitOffset, nSize, htStartAddresses);
				}
				else if (!taskmaplist.Add(iTaskNbr, variableCrossRef, comcon, variableDeclaration, variableDeclaration.Channel.ParamBitoffset, variableDeclaration.Channel.IecBitoffset, (int)variableDeclaration.Channel.BitSize, string.Empty, checker, variableDeclaration.Channel.DataElement))
				{
					taskmaplist.AddErrorMsg(Strings.ErrorChannelAlreadyUsed, variableCrossRef, comcon, !variableDeclaration.Connector.ShowMultipleMappingsAsError);
				}
			}
		}

		internal static void AddTaskMapListForComplexTypes(int iTaskNbr, VariableCrossRef cref, DataElementBase dataelement, TaskMapList taskmaplist, DoubleAddressTaskChecker checker, VariableDeclaration vd, ICompileContext comcon, long lStartBit, long lIecStartBit, int nSize, Hashtable htStartAddresses)
		{
			if (!dataelement.HasSubElements)
			{
				return;
			}
			long num = 0L;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)70))
			{
				num = dataelement.GetBitOffset();
			}
			bool flag = default(bool);
			foreach (DataElementBase item in (IEnumerable)dataelement.SubElements)
			{
				if (nSize > 0 && item.GetBitOffset() - num >= nSize)
				{
					break;
				}
				if (item.HasBaseType)
				{
					string baseType = item.BaseType;
					long bitSize = item.GetBitSize();
					IDirectVariable iecAddress = ((IoMapping)(object)item.IoMapping).GetIecAddress(htStartAddresses);
					BitDataLocation bitDataLocation = new BitDataLocation(comcon.LocateAddress(out flag, iecAddress));
					long num2 = bitDataLocation.BitOffset - lStartBit;
					long num3 = bitDataLocation.BitOffset - lIecStartBit;
					if (!taskmaplist.Add(iTaskNbr, cref, comcon, vd, (int)num2, (int)num3, (int)bitSize, baseType, checker, (IDataElement)(object)item))
					{
						taskmaplist.AddErrorMsg(Strings.ErrorChannelAlreadyUsed, cref, comcon, !vd.Connector.ShowMultipleMappingsAsError);
					}
				}
				else
				{
					AddTaskMapListForComplexTypes(iTaskNbr, cref, item, taskmaplist, checker, vd, comcon, lStartBit, lIecStartBit, nSize, htStartAddresses);
				}
			}
		}

		public static bool TryInsertLib(IMetaObject moApplication, string stLibName, string stVersion, string stVendor, out string stDisplayName, out string stError, bool bSystemLibrary, bool bPlaceHolderLib, string stPlaceHolder)
		{
			bool flag = stLibName.Equals("IoStandard", StringComparison.OrdinalIgnoreCase);
			if (stLibName.Equals("SysMem", StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			if (string.IsNullOrEmpty(stVersion))
			{
				stVersion = "*";
			}
			stDisplayName = $"{stLibName}, {stVersion} ({stVendor})";
			try
			{
				IManagedLibraryRepository val = default(IManagedLibraryRepository);
				if (!bPlaceHolderLib && APEnvironment.ManagedLibraryMgr.FindLibrary(stDisplayName, out val) == null)
				{
					stError = "";
					return false;
				}
			}
			catch
			{
				stError = "";
				return false;
			}
			try
			{
				if (bPlaceHolderLib)
				{
					Guid guid = APEnvironment.PlaceholderResolutionTypeGuid;
					if (string.Equals(stPlaceHolder, "3SLicense", StringComparison.OrdinalIgnoreCase))
					{
						guid = APEnvironment.LicenseLibPlaceholderResolutionTypeGuid;
					}
					if (bSystemLibrary)
					{
						((ILibraryLoader9)APEnvironment.LibraryLoader).LoadSystemPlaceholderLibrary(stPlaceHolder, stDisplayName, moApplication.ProjectHandle, moApplication.ObjectGuid, guid, flag);
					}
					else
					{
						((ILibraryLoader6)APEnvironment.LibraryLoader).LoadPlaceholderLibrary(stPlaceHolder, stDisplayName, moApplication.ProjectHandle, moApplication.ObjectGuid, guid, false);
					}
					if (guid != APEnvironment.LicenseLibPlaceholderResolutionTypeGuid)
					{
						APEnvironment.LibraryLoader.ProposeUnboundPlaceholderResolution(moApplication.ProjectHandle, moApplication.ObjectGuid, stPlaceHolder, stDisplayName);
					}
				}
				else if (!string.IsNullOrEmpty(stPlaceHolder) && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)0))
				{
					Guid placeholderResolutionTypeGuid = APEnvironment.PlaceholderResolutionTypeGuid;
					if (bSystemLibrary)
					{
						((ILibraryLoader9)APEnvironment.LibraryLoader).LoadSystemPlaceholderLibrary(stPlaceHolder, stDisplayName, moApplication.ProjectHandle, moApplication.ObjectGuid, placeholderResolutionTypeGuid, flag);
					}
					else
					{
						((ILibraryLoader6)APEnvironment.LibraryLoader).LoadPlaceholderLibrary(stPlaceHolder, stDisplayName, moApplication.ProjectHandle, moApplication.ObjectGuid, placeholderResolutionTypeGuid, false);
					}
				}
				else if (bSystemLibrary)
				{
					((ILibraryLoader4)APEnvironment.LibraryLoader).LoadSystemLibrary(stDisplayName, moApplication.ProjectHandle, moApplication.ObjectGuid, flag);
				}
				else
				{
					((ILibraryLoader)APEnvironment.LibraryLoader).LoadLibrary(stDisplayName, moApplication.ProjectHandle, moApplication.ObjectGuid, false);
				}
				stError = "";
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				stError = ex.Message;
				return false;
			}
		}

		private static bool CheckForIoLibrary(ILibManObject libman)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			foreach (ILibManItem item in (IEnumerable)libman)
			{
				object obj = (object)item;
				IManagedLibManItem val = (IManagedLibManItem)((obj is IManagedLibManItem) ? obj : null);
				if (val != null && val.ManagedLibrary.Title == "IoStandard")
				{
					return true;
				}
			}
			return false;
		}

		private static void InsertIoLibrary(ILibManObject libman)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			IManagedLibrary[] allLibraries = APEnvironment.ManagedLibraryMgr.GetAllLibraries(true);
			ArrayList arrayList = new ArrayList();
			IManagedLibrary[] array = allLibraries;
			foreach (IManagedLibrary val in array)
			{
				if (val.Title == "IoStandard")
				{
					arrayList.Add(val);
				}
			}
			if (arrayList.Count == 0)
			{
				return;
			}
			int index = 0;
			Version value = new Version();
			for (int j = 0; j < arrayList.Count; j++)
			{
				if (((IManagedLibrary)arrayList[j]).Version.CompareTo(value) > 0)
				{
					index = j;
				}
			}
			ILibManItem val2 = libman.CreateManagedLibManItem((IManagedLibrary)arrayList[index]);
			if (val2 != null)
			{
				libman.Add(val2);
			}
		}

		private static string CreateParamName(string stBaseName, long lParamId)
		{
			return $"param_{stBaseName}_{lParamId}";
		}
	}
}
