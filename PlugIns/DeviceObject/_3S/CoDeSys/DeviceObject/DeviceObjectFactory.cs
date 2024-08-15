#define DEBUG
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{84d12aa5-3225-473b-9df6-18af40889bdf}")]
	public class DeviceObjectFactory : IObjectFactory
	{
		public Guid Namespace => DeviceObject.GUID_DEVICENAMESPACE;

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DeviceObjectName");

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DeviceObjectDescription");

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectFactory), "_3S.CoDeSys.DeviceObject.Resources.DeviceIconSmall.ico");

		public Icon LargeIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectFactory), "_3S.CoDeSys.DeviceObject.Resources.DeviceIconSmall.ico");

		public Control WizardPage => null;

		public Control ObjectNameControl => null;

		public string ObjectBaseName => "";

		public Type ObjectType => typeof(DeviceObject);

		public Type[] EmbeddedObjectTypes => null;

		public IObject Create()
		{
			return null;
		}

		public IObject Create(string[] batchArguments)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			if (batchArguments.Length < 3)
			{
				throw new BatchTooFewArgumentsException(new string[0], batchArguments.Length, 3);
			}
			if (batchArguments.Length > 7)
			{
				throw new BatchTooManyArgumentsException(new string[0], batchArguments.Length, 7);
			}
			bool bCreateBitChannels = false;
			int type;
			try
			{
				type = int.Parse(batchArguments[0]);
			}
			catch
			{
				throw new BatchWrongArgumentTypeException(new string[0], 0, "int");
			}
			CreateDeviceFlags createDeviceFlags = CreateDeviceFlags.None;
			if (batchArguments.Length == 3)
			{
				createDeviceFlags = CreateDeviceFlags.None;
			}
			else if (batchArguments.Length == 4)
			{
				createDeviceFlags = CreateDeviceFlags.UseModuleId;
			}
			else if (batchArguments.Length >= 5)
			{
				try
				{
					createDeviceFlags = (CreateDeviceFlags)uint.Parse(batchArguments[4]);
				}
				catch
				{
					throw new BatchWrongArgumentTypeException(new string[0], 4, "uint");
				}
			}
			if (batchArguments.Length == 7)
			{
				int.TryParse(batchArguments[5], out var result);
				bCreateBitChannels = DeviceObjectHelper.CreateBitChannels(objectGuid: new Guid(batchArguments[6]), nProjectHandle: result);
			}
			DeviceIdentification deviceIdentification = ((batchArguments.Length != 3 && !(batchArguments[3] == "") && (createDeviceFlags & CreateDeviceFlags.UseModuleId) != 0) ? new ModuleIdentification
			{
				ModuleId = batchArguments[3]
			} : new DeviceIdentification());
			deviceIdentification.Type = type;
			deviceIdentification.Id = batchArguments[1];
			deviceIdentification.Version = batchArguments[2];
			return (IObject)(object)CreateDeviceObject(deviceIdentification, createDeviceFlags, bCreateBitChannels);
		}

		private DeviceObject CreateDeviceObject(DeviceIdentification id, CreateDeviceFlags flags, bool bCreateBitChannels)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if ((flags & (CreateDeviceFlags.HiddenDevice | CreateDeviceFlags.TransientDevice)) == 0)
			{
				return new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)id);
			}
			if ((flags & CreateDeviceFlags.HiddenDevice) != 0 && (flags & CreateDeviceFlags.TransientDevice) != 0)
			{
				return new HiddenAndTransientDeviceObject(bCreateBitChannels, id);
			}
			if ((flags & CreateDeviceFlags.HiddenDevice) != 0)
			{
				return new HiddenDeviceObject(bCreateBitChannels, id);
			}
			if ((flags & CreateDeviceFlags.TransientDevice) != 0)
			{
				return new TransientDeviceObject(bCreateBitChannels, id);
			}
			Debug.Assert(condition: false, "Denkfehler");
			throw new DeviceObjectException((DeviceObjectExeptionReason)9, "Unknown combination of CreateDeviceFlags");
		}

		public void ObjectCreated(int nProjectHandle, Guid guidObject)
		{
			ObjectCreatedStatic(nProjectHandle, guidObject);
		}

		public static void ObjectCreatedStatic(int nProjectHandle, Guid guidObject)
		{
			IDeviceObjectBase deviceObjectBase = null;
			IMetaObject val = null;
			try
			{
				DeviceObjectHelper.BeginCreateDevice(guidObject);
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidObject);
				if (val != null)
				{
					deviceObjectBase = val.Object as IDeviceObjectBase;
				}
				if (deviceObjectBase != null)
				{
					DeviceObject.UpdateChildObjects(deviceObjectBase, bUpdate: false, bVersionUpgrade: false);
				}
			}
			catch
			{
			}
			finally
			{
				DeviceObjectHelper.EndCreateDevice(guidObject);
			}
		}

		public bool AcceptsParentObject(IObject parentObject)
		{
			if (parentObject == null)
			{
				return true;
			}
			if (parentObject is IDeviceObject)
			{
				return true;
			}
			if (parentObject is IConnector)
			{
				return true;
			}
			return false;
		}
	}
}
