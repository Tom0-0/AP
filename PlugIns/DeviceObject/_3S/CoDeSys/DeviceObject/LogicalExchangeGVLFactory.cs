using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject.LogicalDevice;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{06DF114A-33CA-4528-B7A6-6CE873209479}")]
	public class LogicalExchangeGVLFactory : IObjectFactory
	{
		private GVLWizardPage _wizardPage = new GVLWizardPage();

		public string Description => LogicalIOStrings.LogicalExchangeGVLDescription;

		public Type[] EmbeddedObjectTypes => new Type[0];

		public Icon LargeIcon => SmallIcon;

		public string Name => LogicalIOStrings.LogicalExchangeGVLName;

		public Guid Namespace => LogicalExchangeGVLObject.GUID_POUNAMESPACE;

		public string ObjectBaseName => "Logical_GVL";

		public Control ObjectNameControl => _wizardPage.ObjectNameControl;

		public Type ObjectType => typeof(LogicalExchangeGVLObject);

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceObject.Resources.GVLSmall.ico");

		public Control WizardPage => _wizardPage;

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
				_=parentObject.MetaObject.SubObjectGuids;
				if (ReflectionHelper.CheckImplementsInterface((object)parentObject, typeof(IApplicationObject).FullName))
				{
					if (APEnvironment.ExistsSafety || APEnvironment.ExistsSafetyEL6900)
					{
						if (parentObject is IDeviceApplication)
						{
							return true;
						}
						if (parentObject is IApplicationObject && ((IOnlineApplicationObject)((parentObject is IApplicationObject) ? parentObject : null)).ParentApplicationGuid == Guid.Empty)
						{
							return true;
						}
						return false;
					}
					if (DeviceObjectHelper.LogicalDevices != null && DeviceObjectHelper.LogicalDevices.DeviceGuids != null && DeviceObjectHelper.LogicalDevices.DeviceGuids.Count > 0)
					{
						Type typeFromHandle = typeof(LogicalExchangeGVLObject);
						if (((IEngine)APEnvironment.Engine).Frame.ViewFactoryManager.GetDefaultEditorViewFactory(typeFromHandle, (Type[])null) != Guid.Empty)
						{
							return true;
						}
					}
				}
				ReflectionHelper.CheckImplementsInterface((object)parentObject, typeof(IPlcLogicObject).FullName);
				return false;
			}
			return false;
		}

		public IObject Create(string[] stBatchArguments)
		{
			return (IObject)(object)new LogicalExchangeGVLObject();
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
