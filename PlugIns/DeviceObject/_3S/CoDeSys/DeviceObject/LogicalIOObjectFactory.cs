using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject.LogicalDevice;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{006C004A-6473-4aa7-BEE4-1A7C84CEF829}")]
	public class LogicalIOObjectFactory : IObjectFactory
	{
		public string Description => LogicalIOStrings.LogicalIOObjectDescription;

		public Type[] EmbeddedObjectTypes => new Type[0];

		public Icon LargeIcon => SmallIcon;

		public string Name => LogicalIOStrings.LogicalIOObjectName;

		public Guid Namespace => LogicalIOObject.GUID_LOGCICALNAMESPACE;

		public string ObjectBaseName => "LogicalIOObject";

		public Control ObjectNameControl => new TextBox
		{
			ReadOnly = true,
			Text = LogicalIOStrings.LogicalIOObjectName
		};

		public Type ObjectType => typeof(LogicalIOObject);

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceObject.Resources.LogicalIconSmall.ico");

		public Control WizardPage => new Control();

		public bool AcceptsParentObject(IObject parentObject)
		{
			bool flag = false;
			if (parentObject is ILogicalObjectRequired)
			{
				flag = ((ILogicalObjectRequired)((parentObject is ILogicalObjectRequired) ? parentObject : null)).LogicalObjectRequired;
			}
			if (!flag && !DeviceObjectHelper.EnableLogicalDevices)
			{
				return false;
			}
			if (parentObject != null)
			{
				if (ReflectionHelper.CheckImplementsInterface((object)parentObject, typeof(IApplicationObject).FullName))
				{
					Guid[] subObjectGuids = parentObject.MetaObject.SubObjectGuids;
					foreach (Guid guid in subObjectGuids)
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(parentObject.MetaObject.ProjectHandle, guid);
						if (typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							return false;
						}
					}
					if (APEnvironment.ExistsSafety || APEnvironment.ExistsSafetyEL6900)
					{
						return true;
					}
					if (DeviceObjectHelper.PhysicalDevices != null && DeviceObjectHelper.PhysicalDevices.DeviceGuids != null && DeviceObjectHelper.PhysicalDevices.DeviceGuids.Count > 0)
					{
						return true;
					}
				}
				ReflectionHelper.CheckImplementsInterface((object)parentObject, typeof(IPlcLogicObject).FullName);
				return false;
			}
			return false;
		}

		public IObject Create(string[] stBatchArguments)
		{
			return (IObject)(object)new LogicalIOObject();
		}

		public IObject Create()
		{
			return Create(new string[0]);
		}

		public void ObjectCreated(int nProjectHandle, Guid guidObject)
		{
		}
	}
}
