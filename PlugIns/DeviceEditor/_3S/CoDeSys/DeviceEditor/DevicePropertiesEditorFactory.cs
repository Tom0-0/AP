using System;
using System.Drawing;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{5F7DC621-5615-4996-9E5F-028D9D6612DC}")]
	public class DevicePropertiesEditorFactory : IPropertiesEditorViewFactory, IEditorViewFactory
	{
		public string Name => Strings.PropertyEditorName;

		public string Description => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => null;

		public IPropertiesEditorView Create(IMetaObjectStub mos)
		{
			if (mos != null && typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType))
			{
				try
				{
					IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mos.ProjectHandle, mos.ObjectGuid).Object;
					if (((IDeviceObject)((@object is IDeviceObject) ? @object : null)).AllowsDirectCommunication)
					{
						return (IPropertiesEditorView)(object)new DevicePropertiesEditor();
					}
				}
				catch
				{
				}
			}
			return null;
		}

		public bool AcceptsObjectType(Type objectType, Type[] embeddedObjectTypes)
		{
			return typeof(IDeviceObject).IsAssignableFrom(objectType);
		}
	}
}
