#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	public abstract class Common
	{
		internal const string ATTRIBUTE_UNQUALIFIED_ACCESS = "unqualified-access";

		public static Icon GetIcon(IIdentifierInfo iii)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (iii == null)
			{
				throw new ArgumentNullException("iii");
			}
			if (((int)iii.Flags & 1) != 0)
			{
				return GetIcon(iii.Variable);
			}
			if (((int)iii.Flags & 2) != 0)
			{
				return GetIcon(iii.Signature);
			}
			return null;
		}

		internal static IVariable GetVariable(Guid objectGuid, string expression)
		{
			IPreCompileContext preCompileContext = null;
			APEnvironment.LanguageModelMgr.FindSignature(objectGuid, out preCompileContext);
			IIdentifierInfo[] identifierInfo = preCompileContext.GetIdentifierInfo(objectGuid, expression);
			if (identifierInfo != null && identifierInfo.Length != 0)
			{
				return identifierInfo[0].Variable;
			}
			return null;
		}

		public static void GetArrayDimensionSize(IArrayType arrayType, out int min, out int max)
		{
			int num = 0;
			int num2 = 1;
			for (int i = 0; i < arrayType.Dimensions.Length; i++)
			{
				int arrayDimensionSize = Common.GetArrayDimensionSize(arrayType.Dimensions[i], out min, out max);
				num2 *= arrayDimensionSize;
				if (i == 0)
				{
					num = min;
				}
			}
			min = num;
			max = num + num2 - 1;
		}

		public static int GetArrayDimensionSize(IArrayDimension arrayDimension, out int min, out int max)
		{
			int result = 0;
			int num = 0;
			int num2 = 0;
			if (arrayDimension.LowerBorder != null && arrayDimension.UpperBorder != null)
			{
				IPrecompileScope4 scope = (APEnvironment.LanguageModelMgr.GetPrecompileContext(Common.GetApplicationGuid()) as IPreCompileContext4).CreatePrecompileScope(Guid.Empty) as IPrecompileScope4;
				ILiteralValue literalValue = (arrayDimension.LowerBorder as IExpression2).Literal(scope);
				ILiteralValue literalValue2 = (arrayDimension.UpperBorder as IExpression2).Literal(scope);
				literalValue.GetInt(out num);
				literalValue2.GetInt(out num2);
				result = num2 - num + 1;
				min = num;
				max = num2;
				return result;
			}
			min = 0;
			max = 0;
			return result;
		}

		internal static IIdentifierInfo[] GetSubElements(IPreCompileContext pcc, Guid signatureGuid, IType type)
		{
			string stAccessPathOrType = type.ToString();
			if (type is IArrayType)
			{
				IArrayType2 arrayType = type as IArrayType2;
				int num;
				int num2;
				Common.GetArrayDimensionSize(arrayType, out num, out num2);
				stAccessPathOrType = string.Format("ARRAY [{0}..{1}] OF ", num, num2) + arrayType.Base.ToString();
			}
			bool flag;
			IIdentifierInfo[] result = pcc.FindSubelements(signatureGuid, stAccessPathOrType, FindSubelementsFlags.IncludeLocalVars | FindSubelementsFlags.IncludeArrayElements | FindSubelementsFlags.SearchForType | FindSubelementsFlags.ExcludeSubSignatures | FindSubelementsFlags.ExcludeProperties, out flag);
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static Guid GetApplicationGuid()
		{
			Guid empty = Guid.Empty;
			IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
			if (primaryProject != null)
			{
				foreach (Guid guid in SystemInstances.ObjectMgr.GetAllObjects(primaryProject.Handle))
				{
					IMetaObjectStub metaObjectStub = SystemInstances.ObjectMgr.GetMetaObjectStub(primaryProject.Handle, guid);
					if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						return guid;
					}
				}
			}
			return empty;
		}

		internal static int[] GetIndexOfArray(string operandExpr, out string varName)
		{
			string text = operandExpr;
			string text2 = string.Empty;
			if (operandExpr.Contains('.'))
			{
				text2 = operandExpr.Substring(0, operandExpr.LastIndexOf('.'));
				text = operandExpr.Substring(operandExpr.LastIndexOf('.') + 1);
			}
			int num = text.IndexOf("[");
			if (num == -1)
			{
				varName = operandExpr;
				return null;
			}
			varName = text.Substring(0, num);
			if (!string.IsNullOrEmpty(text2))
			{
				varName = text2 + "." + varName;
			}
			char obj = '[';
			char obj2 = ']';
			int num2 = -1;
			List<string> list = new List<string>();
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c.Equals(obj))
				{
					num2 = i;
				}
				else if (c.Equals(obj2))
				{
					int num3 = i;
					string item = text.Substring(num2 + 1, num3 - num2 - 1);
					list.Add(item);
					num2 = -1;
				}
			}
			List<int> list2 = new List<int>();
			int item2 = 0;
			for (int j = 0; j < list.Count; j++)
			{
				string[] array = list[j].Split(new char[]
				{
					','
				});
				for (int k = 0; k < array.Length; k++)
				{
					if (int.TryParse(array[k].Trim(), out item2))
					{
						list2.Add(item2);
					}
				}
			}
			return list2.ToArray();
		}

		private static Icon GetIcon(ISignature signature)
		{
			if (signature == null)
			{
				return null;
			}
			IPreCompileContext val = default(IPreCompileContext);
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindSignature(signature.ObjectGuid, out val);
			if (val == null)
			{
				return null;
			}
			int num = -1;
			string libraryPath = val.LibraryPath;
			if (libraryPath == null || libraryPath.Length == 0)
			{
				num = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
			}
			else
			{
				IProject[] projects = ((IEngine)APEnvironment.Engine).Projects.Projects;
				Debug.Assert(projects != null);
				IProject[] array = projects;
				foreach (IProject val2 in array)
				{
					if (string.Compare(val2.Id, libraryPath, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num = val2.Handle;
						break;
					}
				}
			}
			try
			{
				if (APEnvironment.ObjectMgr.ExistsObject(num, signature.ObjectGuid))
				{
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(num, signature.ObjectGuid);
					Debug.Assert(metaObjectStub != null);
					Icon iconForObject = ((IEngine)APEnvironment.Engine).Frame.GetIconForObject(metaObjectStub, false);
					if (iconForObject == null)
					{
						iconForObject = ((IEngine)APEnvironment.Engine).Frame.GetIconForObject(metaObjectStub, true);
					}
					return iconForObject;
				}
			}
			catch
			{
			}
			return null;
		}

		private static Icon GetIcon(IVariable variable)
		{
			if (variable == null)
			{
				return null;
			}
			if (variable.GetFlag((VarFlag)2))
			{
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(Common), "_3S.CoDeSys.WatchList.Resources.VarInputSmall.ico");
			}
			if (variable.GetFlag((VarFlag)4))
			{
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(Common), "_3S.CoDeSys.WatchList.Resources.VarOutputSmall.ico");
			}
			if (variable.GetFlag((VarFlag)8))
			{
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(Common), "_3S.CoDeSys.WatchList.Resources.VarInOutSmall.ico");
			}
			if (variable.GetFlag((VarFlag)134217728))
			{
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(Common), "_3S.CoDeSys.WatchList.Resources.VarStatSmall.ico");
			}
			if (variable.GetFlag((VarFlag)2097152))
			{
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(Common), "_3S.CoDeSys.WatchList.Resources.VarTempSmall.ico");
			}
			if (variable.GetFlag((VarFlag)128))
			{
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(Common), "_3S.CoDeSys.WatchList.Resources.EnumSmall.ico");
			}
			return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(Common), "_3S.CoDeSys.WatchList.Resources.VarSmall.ico");
		}

		internal static ICompileContext GetReferenceContext(Guid guidApplication)
		{
			return APEnvironment.MonitoringUtilities.GetReferenceContext(guidApplication);
		}

		internal static bool LoadEnumValuesToComboBoxControl(ComboBox control, ICompiledType ctype, Guid application)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (control != null && ctype != null)
			{
				try
				{
					ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(application);
					if (compileContext != null)
					{
						IScope val = compileContext.CreateGlobalIScope();
						if (val != null)
						{
							ISignature val2 = null;
							if (typeof(IEnumType2).IsAssignableFrom(((object)ctype).GetType()))
							{
								val2 = val[((IEnumType2)ctype).SignatureId];
							}
							else
							{
								ISignature[] array = val.FindSignature(((IEnumType)ctype).Name);
								if (array != null && array.Length == 1)
								{
									val2 = array[0];
								}
							}
							if (val2 != null)
							{
								IVariable[] constant = val2.Constant;
								foreach (IVariable val3 in constant)
								{
									if (!VisibilityUtil.IsHiddenVariable(val2, val3, (GUIHidingFlags)5764607523034234881L))
									{
										if (!string.IsNullOrEmpty(val3.OrgName))
										{
											control.Items.Add(val3.OrgName);
										}
										else
										{
											control.Items.Add(val3.Name);
										}
									}
								}
								return true;
							}
						}
					}
				}
				catch
				{
				}
			}
			return false;
		}

		private static bool MustAddWithPOUName(string expression, IPreCompileContext pcc, Guid objectGuid)
		{
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Invalid comparison between Unknown and I4
			if (string.IsNullOrEmpty(expression))
			{
				return true;
			}
			string[] array = expression.Split('.', '[', '^');
			if (array.Length == 0)
			{
				return true;
			}
			IIdentifierInfo[] identifierInfo = pcc.GetIdentifierInfo(objectGuid, array[0]);
			if (identifierInfo == null || identifierInfo.Length == 0)
			{
				return true;
			}
			if (identifierInfo[0].Variable != null && identifierInfo[0].Variable.GetFlag((VarFlag)8192))
			{
				if (identifierInfo[0].Signature != null && identifierInfo[0].Signature.HasAttribute(CompileAttributes.ATTRIBUTE_QUALIFIED_ONLY))
				{
					return true;
				}
				return false;
			}
			if (identifierInfo[0].Variable == null && identifierInfo[0].Signature != null && (int)identifierInfo[0].Signature.POUType == 109)
			{
				return false;
			}
			return true;
		}

		internal static bool AddToWatch(OnlineState onlineState, IPreCompileContext pcc, int nProjectHandle, Guid objectGuid, string expression, WatchListView view)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			string qualifiedExpression = string.Empty;
			bool qualifiedExpression2 = GetQualifiedExpression(onlineState.InstancePath, pcc, nProjectHandle, objectGuid, expression, out qualifiedExpression);
			if (qualifiedExpression2)
			{
				((IEngine)APEnvironment.Engine).Frame.ActiveView=((IView)(object)view);
				view.Control.Focus();
				if (view.SelectExpression(qualifiedExpression))
				{
					return true;
				}
				string[] expressions = view.GetExpressions();
				int nIndex = ((expressions != null) ? expressions.Length : 0);
				view.InsertExpression(nIndex, qualifiedExpression);
				view.SelectExpression(qualifiedExpression);
			}
			return qualifiedExpression2;
		}

		public static bool GetQualifiedExpression(string stInstancePath, IPreCompileContext pcc, int nProjectHandle, Guid objectGuid, string expression, out string qualifiedExpression)
		{
			qualifiedExpression = string.Empty;
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (stInstancePath != null)
			{
				empty = stInstancePath;
				empty2 = string.Join(".", empty.Split('.'), 0, 2);
			}
			else
			{
				try
				{
					string name = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, objectGuid).Name;
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((ICompileContextCommon)pcc).ApplicationGuid);
					string name2 = metaObjectStub.Name;
					while (metaObjectStub.ParentObjectGuid != Guid.Empty)
					{
						metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
					}
					empty = metaObjectStub.Name + "." + name2 + "." + name;
					empty2 = metaObjectStub.Name + "." + name2;
				}
				catch
				{
					return false;
				}
			}
			IIdentifierInfo[] identifierInfo = pcc.GetIdentifierInfo(objectGuid, expression);
			if (identifierInfo == null || identifierInfo.Length == 0 || identifierInfo[0].Signature == null)
			{
				return false;
			}
			ISignature signature = identifierInfo[0].Signature;
			if (!string.IsNullOrEmpty(identifierInfo[0].Name))
			{
				if (MustAddWithPOUName(expression, pcc, objectGuid))
				{
					ISignature signature2 = ((ICompileContextCommon)pcc).GetSignature(objectGuid);
					if (signature2 != null && !((object)signature2).Equals((object)signature) && empty.ToUpperInvariant().EndsWith("." + signature2.Name))
					{
						int length = empty.LastIndexOf(".");
						empty = empty.Substring(0, length);
					}
					qualifiedExpression = $"{empty}.{identifierInfo[0].Name}";
				}
				else
				{
					qualifiedExpression = $"{empty2}.{identifierInfo[0].Name}";
				}
			}
			else
			{
				qualifiedExpression = $"{empty2}.{identifierInfo[0].Signature.OrgName}";
			}
			return true;
		}

		internal static bool AddToWatch(IEnumerable<string> expressions, WatchListView view)
		{
			if (expressions == null)
			{
				return false;
			}
			bool result = false;
			bool flag = true;
			foreach (string expression in expressions)
			{
				if (!string.IsNullOrWhiteSpace(expression))
				{
					if (flag)
					{
						((IEngine)APEnvironment.Engine).Frame.ActiveView=((IView)(object)view);
						view.Control.Focus();
					}
					if (!view.SelectExpression(expression, flag))
					{
						string[] expressions2 = view.GetExpressions();
						int nIndex = ((expressions2 != null) ? expressions2.Length : 0);
						view.InsertExpression(nIndex, expression);
						view.SelectExpression(expression, flag);
					}
					result = true;
					flag = false;
				}
			}
			return result;
		}

		internal static Guid GetApplicationGuid(string stDevice, string stApplication)
		{
			return APEnvironment.MonitoringUtilities.GetApplicationGuid(stDevice, stApplication);
		}

		public static string GetApplicationName(Guid appGuid)
		{
			IMetaObjectStub val = ((!(appGuid == Guid.Empty)) ? APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, appGuid) : APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication));
			IMetaObjectStub val2 = val;
			while (val2.ParentObjectGuid != Guid.Empty)
			{
				val2 = APEnvironment.ObjectMgr.GetMetaObjectStub(val2.ProjectHandle, val2.ParentObjectGuid);
				if (typeof(IDeviceObject).IsAssignableFrom(val2.ObjectType))
				{
					return val2.Name + "." + val.Name;
				}
			}
			return null;
		}

		internal static void SplitExpression(string stExpression, out string stResource, out string stApplication, out string stSignature)
		{
			Debug.Assert(stExpression != null);
			string[] array = stExpression.Split('.');
			stResource = ((array.Length != 0) ? array[0] : string.Empty);
			stApplication = ((array.Length > 1) ? array[1] : string.Empty);
			stSignature = string.Empty;
			for (int i = 2; i < array.Length; i++)
			{
				if (i > 2)
				{
					stSignature += ".";
				}
				stSignature += array[i];
			}
		}

		internal static bool TryToGetEnumValue(IVarRef varref, int nSignatureID, string stEnumValue, out string stValue)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Invalid comparison between Unknown and I4
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			stValue = string.Empty;
			bool result = false;
			try
			{
				IScope val = GetReferenceContext(varref.ApplicationGuid).CreateGlobalIScope();
				if ((int)((IType)varref.WatchExpression.Type.BaseType
					.DeRefType).Class != 0)
				{
					if (varref.WatchExpression.Type.BaseType is IEnumType2)
					{
						IntegerUnion val2 = default(IntegerUnion);
						if (stEnumValue.Contains("2#"))
						{
							val2.m_long = Convert.ToInt64(stEnumValue.Substring(2), 2);
						}
						else if (stEnumValue.StartsWith("16#"))
						{
							val2.m_long = Convert.ToInt64(stEnumValue.Substring(3), 16);
						}
						else
						{
							val2.m_long = long.Parse(stEnumValue);
						}
						ISignature val3 = val[((IEnumType2)varref.WatchExpression.Type.BaseType).SignatureId];
						if (val3 != null)
						{
							IVariable[] all = val3.All;
							foreach (IVariable val4 in all)
							{
								if ((int)val4.Type.Class == 25 && val4.Initial != null && val4.Initial.Literal(val) != null)
								{
									ILiteralValue val5 = val4.Initial.Literal(val);
									if ((int)val5.KindOf == 0 && val2.m_long == val5.SignedLong)
									{
										stValue = val3.OrgName + "." + val4.OrgName;
										break;
									}
								}
							}
							if (stValue != null)
							{
								result = true;
								return result;
							}
							return result;
						}
						return result;
					}
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		internal static string[] GetAllDevices()
		{
			Hashtable hashtable = new Hashtable();
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				if (handle >= 0)
				{
					Guid[] allObjects = APEnvironment.ObjectMgr.GetAllObjects(handle);
					Debug.Assert(allObjects != null);
					Guid[] array = allObjects;
					foreach (Guid guid in array)
					{
						IMetaObjectStub mos = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, guid);
						Debug.Assert(mos != null);
						if (!typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType) || typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) || APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)))
						{
							continue;
						}
						IMetaObjectStub val = null;
						while (mos.ParentObjectGuid != Guid.Empty)
						{
							mos = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, mos.ParentObjectGuid);
							Debug.Assert(mos != null);
							if (typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType) && !typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) && !APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)))
							{
								val = mos;
								break;
							}
						}
						if (val != null)
						{
							hashtable[val.ObjectGuid] = val.Name;
						}
					}
				}
			}
			string[] array2 = new string[hashtable.Count];
			hashtable.Values.CopyTo(array2, 0);
			return array2;
		}

		internal static IList<string> LookForAllDeviceAppPrefixes()
		{
			IList<string> list = (IList<string>)new LList<string>();
			string[] allDevices = GetAllDevices();
			if (allDevices != null && allDevices.Length != 0)
			{
				string[] array = allDevices;
				foreach (string text in array)
				{
					string[] allApplications = GetAllApplications(text);
					if (allApplications == null || allApplications.Length == 0)
					{
						continue;
					}
					string[] array2 = allApplications;
					foreach (string text2 in array2)
					{
						string item = text + "." + text2;
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		internal static string LookForActiveDeviceAppPrefix()
		{
			string result = string.Empty;
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication != Guid.Empty && APEnvironment.ObjectMgr.ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication))
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication);
				result = metaObjectStub.Name;
				while (metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
				}
				result = metaObjectStub.Name + "." + result;
			}
			return result;
		}

		internal static string[] GetAllApplications(string stDevice)
		{
			Hashtable hashtable = new Hashtable();
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				if (handle >= 0)
				{
					Guid[] allObjects = APEnvironment.ObjectMgr.GetAllObjects(handle);
					Debug.Assert(allObjects != null);
					Guid[] array = allObjects;
					foreach (Guid guid in array)
					{
						IMetaObjectStub mos = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, guid);
						Debug.Assert(mos != null);
						string name = mos.Name;
						if (!typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType) || typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) || APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)))
						{
							continue;
						}
						while (mos.ParentObjectGuid != Guid.Empty)
						{
							mos = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, mos.ParentObjectGuid);
							Debug.Assert(mos != null);
							if (typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType) && !typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) && !APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)))
							{
								if (string.Compare(mos.Name, stDevice, StringComparison.OrdinalIgnoreCase) == 0)
								{
									hashtable[guid] = name;
								}
								break;
							}
						}
					}
				}
			}
			string[] array2 = new string[hashtable.Count];
			hashtable.Values.CopyTo(array2, 0);
			return array2;
		}

		internal static object ConvertToSmallestPointer(ulong ulAddress)
		{
			if ((ulAddress & 0xFFFFFFFF00000000uL) == 0L)
			{
				return (uint)ulAddress;
			}
			return ulAddress;
		}
	}
}
