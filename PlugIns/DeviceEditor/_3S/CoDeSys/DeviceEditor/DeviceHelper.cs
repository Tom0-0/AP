using System;
using System.Linq;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DeviceHelper
	{
		internal static void CollectObjectGuids(LList<Guid> liGuids, int nProjectHandle, Guid[] subGuids, Type objectType, bool bRecursive, bool bWithHidden)
		{
			foreach (Guid guid in subGuids)
			{
				IMetaObjectStub mosSub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (bWithHidden || (!typeof(IHiddenObject).IsAssignableFrom(mosSub.ObjectType) && !APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mosSub))))
				{
					if (objectType.IsAssignableFrom(mosSub.ObjectType))
					{
						liGuids.Add(guid);
					}
					if (bRecursive)
					{
						CollectObjectGuids(liGuids, nProjectHandle, mosSub.SubObjectGuids, objectType, bRecursive, bWithHidden);
					}
				}
			}
		}

		internal static string GetIecAddress(IDataElement dataElement, IDataElement parentDataElement, bool bMotorolaBitFields)
		{
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				string text = ((dataElement.IoMapping != null) ? dataElement.IoMapping.IecAddress : string.Empty);
				if (!string.IsNullOrEmpty(text) && bMotorolaBitFields && dataElement is IDataElement2 && ((IDataElement2)((dataElement is IDataElement2) ? dataElement : null)).HasBaseType && dataElement.GetBitSize() == 1 && parentDataElement != null && parentDataElement is IDataElement2 && ((IDataElement2)((parentDataElement is IDataElement2) ? parentDataElement : null)).HasBaseType)
				{
					long bitSize = parentDataElement.GetBitSize();
					if (bitSize == 16 || bitSize == 32 || bitSize == 64)
					{
						IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text, false, false, false, false);
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

		internal static string DirectVariableToString(DirectVariableLocation location, DirectVariableSize size, int[] components)
		{
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

		internal static string GetSizeString(DirectVariableSize size)
		{
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

		internal static LList<Guid> GetApplications(IMetaObject metaHost, bool bWithHidden)
		{
			LList<Guid> val = new LList<Guid>();
			if (metaHost != null)
			{
				LList<Guid> val2 = new LList<Guid>();
				LList<Guid> val3 = new LList<Guid>();
				CollectObjectGuids(val2, metaHost.ProjectHandle, metaHost.SubObjectGuids, typeof(IPlcLogicObject), bRecursive: false, bWithHidden: false);
				if (val2.Count == 1)
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaHost.ProjectHandle, val2[0]);
					CollectObjectGuids(val3, metaHost.ProjectHandle, metaObjectStub.SubObjectGuids, typeof(IDeviceApplication), bRecursive: false, bWithHidden: false);
					if (val3.Count == 1)
					{
						IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaHost.ProjectHandle, val3[0]);
						CollectObjectGuids(val, metaHost.ProjectHandle, metaObjectStub2.SubObjectGuids, typeof(IApplicationObject), bRecursive: true, bWithHidden);
						val.Add(val3[0]);
					}
					else if (metaHost.Object is IDeviceObject2)
					{
						IObject @object = metaHost.Object;
						IDriverInfo driverInfo = ((IDeviceObject2)((@object is IDeviceObject2) ? @object : null)).DriverInfo;
						Guid ioApplication = ((IDriverInfo2)((driverInfo is IDriverInfo2) ? driverInfo : null)).IoApplication;
						LList<Guid> obj = new LList<Guid>();
						CollectObjectGuids(obj, metaHost.ProjectHandle, metaObjectStub.SubObjectGuids, typeof(IApplicationObject), bRecursive: false, bWithHidden);
						if (obj.Contains(ioApplication))
						{
							val.Add(ioApplication);
						}
					}
				}
			}
			return val;
		}

		internal static bool GetOnlineDevice(IDeviceObject hostdevice, IHasOnlineMode hom)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			if (hostdevice != null && ((IObject)hostdevice).MetaObject != null)
			{
				foreach (Guid application2 in GetApplications(((IObject)hostdevice).MetaObject, bWithHidden: true))
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)hostdevice).MetaObject.ProjectHandle, application2);
					if (!typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(application2);
						if (application != null && application.IsLoggedIn)
						{
							OnlineState onlineState = default(OnlineState);
							onlineState.OnlineApplication = application2;
							hom.OnlineState=(onlineState);
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
