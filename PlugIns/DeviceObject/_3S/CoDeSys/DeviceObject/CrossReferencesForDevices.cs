using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{6D757418-AD2B-4F6F-A470-2D395686EE5A}")]
	public class CrossReferencesForDevices : IAdditionalCrossReferenceProvider
	{
		private class VarVisitor : IVariableVisitor
		{
			private List<VarRefInfo> m_refs;

			public IEnumerable<VarRefInfo> Refs => m_refs;

			public VarVisitor()
			{
				m_refs = new List<VarRefInfo>();
			}

			public void visit(IVariableExpression varExp, AccessFlag access, IPrecompileScope5 scope)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				IVariableExpression2 val = (IVariableExpression2)varExp;
				IVariable variable = val.GetVariable((IPrecompileScope)(object)scope);
				ISignature signature = val.GetSignature((IPrecompileScope)(object)scope);
				m_refs.Add(new VarRefInfo(varExp, access, signature, variable));
			}
		}

		private struct VarRefInfo
		{
			public readonly IVariableExpression varExp;

			public readonly AccessFlag access;

			public readonly ISignature sig;

			public readonly IVariable variable;

			public VarRefInfo(IVariableExpression varExp_, AccessFlag access_, ISignature sig_, IVariable variable_)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				varExp = varExp_;
				access = access_;
				sig = sig_;
				variable = variable_;
			}
		}

		private ICrossReferenceService2 m_Service2;

		public ICrossReferenceService2 CrossReferenceService
		{
			get
			{
				if (m_Service2 == null)
				{
					m_Service2 = APEnvironment.CreateCrossReferenceService();
				}
				return m_Service2;
			}
		}

		private static IPreCompileUtilities6 PreCompUtils
		{
			get
			{
				IPreCompileUtilities preCompileUtils = APEnvironment.LanguageModelUtilities.PreCompileUtils;
				return (IPreCompileUtilities6)(object)((preCompileUtils is IPreCompileUtilities6) ? preCompileUtils : null);
			}
		}

		private void CollectCrossNodes(IIoProvider ioProvder, LList<ICrossReferenceNode> nodes, Guid guidApplication, Predicate<string> matcher)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject metaObject = ioProvder.GetMetaObject();
			if (metaObject == null)
			{
				return;
			}
			bool flag = false;
			if (metaObject != null && metaObject.Object is DeviceObject)
			{
				flag = (metaObject.Object as DeviceObject).NoIoDownload;
			}
			if (!flag)
			{
				foreach (IParameter item in (IEnumerable)ioProvder.ParameterSet)
				{
					IParameter val = item;
					GetAllMappedVariables(metaObject, guidApplication, val.ChannelType, val, (IDataElement2)(object)((val is IDataElement2) ? val : null), nodes, matcher);
				}
			}
			IIoProvider[] children = ioProvder.Children;
			foreach (IIoProvider ioProvder2 in children)
			{
				CollectCrossNodes(ioProvder2, nodes, guidApplication, matcher);
			}
			foreach (RequiredLib item2 in (IEnumerable)ioProvder.DriverInfo.RequiredLibs)
			{
				foreach (FBInstance item3 in (IEnumerable)item2.FbInstances)
				{
					if (!string.IsNullOrEmpty(item3.FbName) && matcher(item3.FbName))
					{
						AddCrossRefNodeSignature(metaObject, nodes, item3.FbName, item3.LanguageModelPositionId, guidApplication, (AccessFlag)128);
					}
					if (!string.IsNullOrEmpty(item3.FbNameDiag) && matcher(item3.FbNameDiag))
					{
						AddCrossRefNodeSignature(metaObject, nodes, item3.FbNameDiag, item3.LanguageModelPositionId, guidApplication, (AccessFlag)128);
					}
				}
			}
		}

		private void GetAllMappedVariables(IMetaObject mo, Guid guidApplication, ChannelType channeltype, IParameter parameter, IDataElement2 dataelement, LList<ICrossReferenceNode> nodes, Predicate<string> matcher)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Invalid comparison between Unknown and I4
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Invalid comparison between Unknown and I4
			if (((IDataElement)dataelement).HasSubElements)
			{
				foreach (IDataElement2 item in (IEnumerable)((IDataElement)dataelement).SubElements)
				{
					IDataElement2 dataelement2 = item;
					GetAllMappedVariables(mo, guidApplication, channeltype, parameter, dataelement2, nodes, matcher);
				}
			}
			if ((int)channeltype != 0)
			{
				string iecAddress = ((IDataElement)dataelement).IoMapping.IecAddress;
				if (!string.IsNullOrEmpty(iecAddress) && matcher(iecAddress))
				{
					AddCrossRefNode(mo, nodes, iecAddress, dataelement.LanguageModelPositionId, guidApplication, (AccessFlag)(((int)channeltype != 1) ? 1 : 2));
				}
				if (((IDataElement)dataelement).IoMapping == null || ((IDataElement)dataelement).IoMapping.VariableMappings == null)
				{
					return;
				}
				foreach (VariableMapping item2 in (IEnumerable)((IDataElement)dataelement).IoMapping.VariableMappings)
				{
					if (item2.CreateVariable)
					{
						continue;
					}
					string text = item2.Variable;
					if (string.IsNullOrEmpty(text) || !matcher(text))
					{
						continue;
					}
					if (text.Contains("$(DeviceName)"))
					{
						text = text.Replace("$(DeviceName)", mo.Name);
					}
					if (text.Contains("."))
					{
						int startIndex = text.IndexOf('.') + 1;
						text = text.Substring(startIndex);
					}
					string strB = matcher.Target.ToString();
					string[] array = text.Split('.');
					for (int i = 0; i < array.Length; i++)
					{
						if (string.Compare(array[i], strB, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							AddCrossRefNode(mo, nodes, text, dataelement.LanguageModelPositionId, guidApplication, (AccessFlag)(((int)channeltype != 1) ? 1 : 2));
							break;
						}
					}
				}
			}
			else if ((parameter as Parameter).UseRefactoring && dataelement.HasBaseType && !string.IsNullOrEmpty(((IDataElement)dataelement).Value) && matcher(((IDataElement)dataelement).Value))
			{
				string stVariable = ((IDataElement)dataelement).Value.Trim('\'', '"');
				long lPosition = (dataelement as Parameter).LanguageModelPositionId | DeviceObjectBase._lValueFlag;
				AddCrossRefNode(mo, nodes, stVariable, lPosition, guidApplication, (AccessFlag)1);
			}
		}

		private void AddCrossRefNodeSignature(IMetaObject mo, LList<ICrossReferenceNode> nodes, string stVariable, long lPosition, Guid guidApplication, AccessFlag access)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			ISourcePosition val = ((ICrossReferenceService)CrossReferenceService).CreateSourcePosition(mo.ProjectHandle, mo.ObjectGuid, lPosition, (short?)(short)0, (short)stVariable.Length);
			IAccessInfo2 val2 = ((ICrossReferenceService)CrossReferenceService).CreateAccessInfo(val, access, guidApplication, mo.ObjectGuid);
			IPreCompileContext2 val3 = (IPreCompileContext2)((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetPrecompileContext(guidApplication);
			if (val3 != null)
			{
				ISignature[] array = ((IPreCompileContext)val3).FindSignature(stVariable);
				if (array.Length != 0)
				{
					ICrossReferenceNode val4 = CrossReferenceService.CreateNode(stVariable, (IAccessInfo)(object)val2, (IVariable)null, array[0], (CrossRefSearchType)1, (CrossRefOccurence)2, (CrossReferenceMatchType)1);
					nodes.Add(val4);
				}
			}
		}

		private void AddCrossRefNode(IMetaObject mo, LList<ICrossReferenceNode> nodes, string stVariable, long lPosition, Guid guidApplication, AccessFlag access)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			ISourcePosition val = ((ICrossReferenceService)CrossReferenceService).CreateSourcePosition(mo.ProjectHandle, mo.ObjectGuid, lPosition, (short?)(short)0, (short)stVariable.Length);
			IAccessInfo2 val2 = ((ICrossReferenceService)CrossReferenceService).CreateAccessInfo(val, access, guidApplication, Guid.Empty);
			bool flag = false;
			foreach (VarRefInfo item in Lookup(guidApplication, Guid.Empty, stVariable))
			{
				if (item.variable != null && item.sig != null)
				{
					ICrossReferenceNode val3 = CrossReferenceService.CreateNode(stVariable, (IAccessInfo)(object)val2, item.variable, item.sig, (CrossRefSearchType)2, (CrossRefOccurence)2, (CrossReferenceMatchType)1);
					nodes.Add(val3);
					flag = true;
				}
			}
			if (!flag)
			{
				ICrossReferenceNode val4 = ((ICrossReferenceService)CrossReferenceService).CreateNode(stVariable, (IAccessInfo)(object)val2, (CrossRefSearchType)2, (CrossRefOccurence)2, (CrossReferenceMatchType)1);
				nodes.Add(val4);
			}
		}

		public static IExpression ParseExpression(string stExp)
		{
			IScanner val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(stExp, false, false, false, false);
			IParser obj = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateParser(val);
			return ((IParser2)((obj is IParser2) ? obj : null)).ParseExpression();
		}

		private static IEnumerable<VarRefInfo> Lookup(Guid gdApp, Guid gdSig, string stExp)
		{
			IPreCompileContext precompileContext = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetPrecompileContext(gdApp);
			IPreCompileContext9 val = (IPreCompileContext9)(object)((precompileContext is IPreCompileContext9) ? precompileContext : null);
			if (val == null)
			{
				return new VarRefInfo[0];
			}
			IPreCompileUtilities6 preCompUtils = PreCompUtils;
			if (preCompUtils == null)
			{
				return new VarRefInfo[0];
			}
			try
			{
				IExpression val2 = ParseExpression(stExp);
				if (val2 == null)
				{
					return new VarRefInfo[0];
				}
				ISignature signature = ((ICompileContextCommon)val).GetSignature(gdSig);
				ISignature4 val3 = (ISignature4)(object)((signature is ISignature4) ? signature : null);
				VarVisitor varVisitor = new VarVisitor();
				preCompUtils.VisitAllVariables((IVariableVisitor)(object)varVisitor, val3, val, (IExprement)(object)val2);
				return varVisitor.Refs;
			}
			catch (Exception)
			{
				return new VarRefInfo[0];
			}
		}

		public IEnumerable<ICrossReferenceNode> GetAdditionalCrossReferences(string stName, Guid guidObject, CrossRefSearchType searchType, CrossRefOccurence occurence, CrossReferenceMatchType matchType, Guid guidApplication = default(Guid))
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Regex @object = new Regex(Regex.Escape(stName), RegexOptions.IgnoreCase);
			return GetAdditionalCrossReferences(@object.IsMatch, guidObject, searchType, occurence, matchType, guidApplication);
		}

		public IEnumerable<ICrossReferenceNode> GetAdditionalCrossReferences(Regex regex, Guid guidObject, CrossRefSearchType searchType, CrossRefOccurence occurence, CrossReferenceMatchType matchType, Guid guidApplication = default(Guid))
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return GetAdditionalCrossReferences(regex.IsMatch, guidObject, searchType, occurence, matchType, guidApplication);
		}

		private IEnumerable<ICrossReferenceNode> GetAdditionalCrossReferences(Predicate<string> matcher, Guid guidObject, CrossRefSearchType searchType, CrossRefOccurence occurence, CrossReferenceMatchType matchType, Guid guidApplication)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			LList<ICrossReferenceNode> val = new LList<ICrossReferenceNode>();
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return (IEnumerable<ICrossReferenceNode>)val;
			}
			if (!((Enum)occurence).HasFlag((Enum)(object)(CrossRefOccurence)2))
			{
				return (IEnumerable<ICrossReferenceNode>)val;
			}
			if (!((Enum)matchType).HasFlag((Enum)(object)(CrossReferenceMatchType)1))
			{
				return (IEnumerable<ICrossReferenceNode>)val;
			}
			if (!((Enum)searchType).HasFlag((Enum)(object)(CrossRefSearchType)2) && !((Enum)searchType).HasFlag((Enum)(object)(CrossRefSearchType)1))
			{
				return (IEnumerable<ICrossReferenceNode>)val;
			}
			if (DeviceObjectHelper.RootDevices == null || DeviceObjectHelper.RootDevices.DeviceGuids == null)
			{
				return (IEnumerable<ICrossReferenceNode>)val;
			}
			try
			{
				foreach (Guid deviceGuid in DeviceObjectHelper.RootDevices.DeviceGuids)
				{
					if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(primaryProject.Handle, deviceGuid))
					{
						continue;
					}
					if (guidApplication != Guid.Empty)
					{
						IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(primaryProject.Handle, guidApplication);
						if (hostStub != null && hostStub.ObjectGuid != deviceGuid)
						{
							continue;
						}
					}
					if (guidObject != Guid.Empty)
					{
						IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(primaryProject.Handle, guidObject);
						if (hostStub2 != null && hostStub2.ObjectGuid != deviceGuid && typeof(IDeviceObject).IsAssignableFrom(hostStub2.ObjectType))
						{
							continue;
						}
					}
					IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, deviceGuid).Object;
					IIoProvider val2 = (IIoProvider)(object)((@object is IIoProvider) ? @object : null);
					if (val2 != null)
					{
						IDriverInfo driverInfo = val2.DriverInfo;
						CollectCrossNodes(val2, val, ((IDriverInfo2)((driverInfo is IDriverInfo2) ? driverInfo : null)).IoApplication, matcher);
					}
				}
				return (IEnumerable<ICrossReferenceNode>)val;
			}
			catch
			{
				return (IEnumerable<ICrossReferenceNode>)val;
			}
		}
	}
}
