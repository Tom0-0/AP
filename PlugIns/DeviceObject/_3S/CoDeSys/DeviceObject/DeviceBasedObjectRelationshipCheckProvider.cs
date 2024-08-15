#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{31766A80-2FDE-4B95-A1BF-5ECC900D454B}")]
	public class DeviceBasedObjectRelationshipCheckProvider : IObjectRelationshipCheckProvider
	{
		private static LDictionary<IDeviceIdentification, Type[]> s_objectTypeRestrictions = new LDictionary<IDeviceIdentification, Type[]>();

		private static readonly Guid GUID_DEVICEPROPERTY = new Guid("{CDADFC2B-8598-4621-AD56-5B1DF7DB910F}");

		public void CheckParentObjectAcceptance(int projectHandle, Type childObjectType, Type[] embeddedChildObjectTypes, IObject parentObject, int index, out bool handled, out bool acceptable)
		{
			acceptable = IsSupported(parentObject, childObjectType);
			handled = true;
		}

		public void CheckParentObjectAcceptance(int projectHandle, IObject obj, IObject parentObject, out bool handled, out bool acceptable)
		{
			acceptable = IsSupported(parentObject, ((object)obj).GetType());
			handled = true;
		}

		public void CheckRelationships(IObject obj, IObject parentObject, IObject[] childObjects, out bool handled, out int result)
		{
			result = 0;
			handled = false;
		}

		internal static void UpdateDevice(int projectHandle, Guid objectGuid, IDeviceIdentification deviceId, Action<string> messageCallback)
		{
			if (deviceId != null)
			{
				s_objectTypeRestrictions.Remove(deviceId);
				Type[] objectTypeRestrictions = GetObjectTypeRestrictions(deviceId);
				if (objectTypeRestrictions != null && messageCallback != null)
				{
					CheckRelationships(projectHandle, objectGuid, objectTypeRestrictions, messageCallback);
				}
			}
		}

		private static void CheckRelationships(int projectHandle, Guid objectGuid, Type[] objectTypeRestrictions, Action<string> messageCallback)
		{
			if (objectTypeRestrictions == null || messageCallback == null)
			{
				return;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, objectGuid);
			Debug.Assert(metaObjectStub != null);
			Guid[] subObjectGuids = metaObjectStub.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, guid);
				Debug.Assert(metaObjectStub2 != null);
				bool flag = false;
				foreach (Type type in objectTypeRestrictions)
				{
					if (type.IsInterface && type.IsAssignableFrom(metaObjectStub2.ObjectType))
					{
						flag = true;
					}
					else if (!type.IsInterface && type == metaObjectStub2.ObjectType)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					string obj = string.Format(Strings.ObjectNotSupported, ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(projectHandle, guid));
					messageCallback(obj);
				}
				CheckRelationships(projectHandle, guid, objectTypeRestrictions, messageCallback);
			}
		}

		private static bool IsSupported(IObject parentObject, Type childObjectType)
		{
			int num = ((parentObject != null && parentObject.MetaObject != null) ? parentObject.MetaObject.ProjectHandle : (-1));
			Guid guid = ((parentObject != null && parentObject.MetaObject != null) ? parentObject.MetaObject.ObjectGuid : Guid.Empty);
			if (num < 0 || guid == Guid.Empty)
			{
				return true;
			}
			while (guid != Guid.Empty)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(num, guid);
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IObjectProperty property = metaObjectStub.GetProperty(GUID_DEVICEPROPERTY);
					IDeviceProperty val = (IDeviceProperty)(object)((property is IDeviceProperty) ? property : null);
					Type[] objectTypeRestrictions = GetObjectTypeRestrictions((val != null) ? val.DeviceIdentification : null);
					if (objectTypeRestrictions != null)
					{
						Type[] array = objectTypeRestrictions;
						foreach (Type type in array)
						{
							if (type.IsInterface && type.IsAssignableFrom(childObjectType))
							{
								return true;
							}
							if (!type.IsInterface && type == childObjectType)
							{
								return true;
							}
						}
						return false;
					}
				}
				guid = metaObjectStub.ParentObjectGuid;
			}
			return true;
		}

		private static Type[] GetObjectTypeRestrictions(IDeviceIdentification deviceId)
		{
			if (deviceId != null)
			{
				Type[] array = default(Type[]);
				s_objectTypeRestrictions.TryGetValue(deviceId, out array);
				if (array != null)
				{
					return array;
				}
				ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceId);
				ITargetSection val = ((targetSettingsById != null && targetSettingsById.Sections != null) ? targetSettingsById.Sections["object-type-restrictions"] : null);
				if (val != null)
				{
					LList<Type> val2 = new LList<Type>();
					for (int i = 0; i < ((ICollection)val.Settings).Count; i++)
					{
						if (val.Settings[i].Name == "type-guid")
						{
							try
							{
								string g = val.Settings[i].DefaultValue
									.ToString();
								Type type = APEnvironment.TryGetType(new Guid(g));
								if (type != null)
								{
									val2.Add(type);
								}
							}
							catch
							{
							}
						}
						else
						{
							if (!(val.Settings[i].Name == "interface"))
							{
								continue;
							}
							try
							{
								Type type2 = Type.GetType(val.Settings[i].DefaultValue
									.ToString());
								if (type2 != null)
								{
									val2.Add(type2);
								}
							}
							catch
							{
							}
						}
					}
					array = val2.ToArray();
					s_objectTypeRestrictions[deviceId]= array;
					return array;
				}
				s_objectTypeRestrictions[deviceId]= (Type[])null;
				return null;
			}
			return null;
		}
	}
}
