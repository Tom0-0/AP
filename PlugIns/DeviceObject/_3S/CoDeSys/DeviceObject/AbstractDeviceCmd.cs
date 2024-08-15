#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public abstract class AbstractDeviceCmd
	{
		private static readonly string[] s_requiredArgumentType = new string[7] { "int", "GUID", "int", "int", "string", "string", "string" };

		private Guid deviceGuid;

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Name");

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Description");

		public string ToolTipText => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		internal Guid DeviceGuid
		{
			get
			{
				return deviceGuid;
			}
			set
			{
				deviceGuid = value;
			}
		}

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public abstract string[] BatchCommand { get; }

		protected virtual void ExecuteBatchNonInteractive(string[] arguments)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Expected O, but got Unknown
			DateTime now = DateTime.Now;
			ParseArguments(arguments, out var nProjectHandle, out var guidParent, out var nInsertPosition, out var deviceId, out var stName);
			DeviceObjectFactory deviceObjectFactory = new DeviceObjectFactory();
			IDeviceObject val;
			if (deviceId is IModuleIdentification)
			{
				string moduleId = ((IModuleIdentification)deviceId).ModuleId;
				val = (IDeviceObject)deviceObjectFactory.Create(new string[7]
				{
					deviceId.Type.ToString(),
					deviceId.Id,
					deviceId.Version,
					moduleId,
					"1",
					nProjectHandle.ToString(),
					guidParent.ToString()
				});
			}
			else
			{
				val = (IDeviceObject)deviceObjectFactory.Create(new string[7]
				{
					deviceId.Type.ToString(),
					deviceId.Id,
					deviceId.Version,
					"",
					"0",
					nProjectHandle.ToString(),
					guidParent.ToString()
				});
			}
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(nProjectHandle);
			bool flag = false;
			DateTime now2;
			try
			{
				undoManager.BeginCompoundAction(Name);
				Guid guid = (deviceGuid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidParent, Guid.NewGuid(), (IObject)(object)val, stName, nInsertPosition));
				now2 = DateTime.Now;
				deviceObjectFactory.ObjectCreated(nProjectHandle, deviceGuid);
				flag = true;
			}
			finally
			{
				undoManager.EndCompoundAction();
				if (!flag)
				{
					try
					{
						undoManager.Undo();
					}
					catch
					{
					}
				}
			}
			DateTime now3 = DateTime.Now;
			TimeSpan timeSpan = now2 - now;
			TimeSpan timeSpan2 = now3 - now2;
			TimeSpan timeSpan3 = now3 - now;
			Debug.WriteLine("--- Performance of AddDevice: ---");
			Debug.WriteLine("Creation and adding: " + timeSpan.ToString());
			Debug.WriteLine("AfterCreated: " + timeSpan2.ToString());
			Debug.WriteLine("Overall: " + timeSpan3.ToString());
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		private void ParseArguments(string[] arguments, out int nProjectHandle, out Guid guidParent, out int nInsertPosition, out IDeviceIdentification deviceId, out string stName)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = s_requiredArgumentType.Length;
			if (arguments.Length < num2)
			{
				throw new BatchTooFewArgumentsException(BatchCommand, arguments.Length, num2);
			}
			if (arguments.Length > num2 + 1)
			{
				throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, num2 + 1);
			}
			try
			{
				nProjectHandle = int.Parse(arguments[num]);
				num++;
				if (arguments[num] == string.Empty)
				{
					guidParent = Guid.Empty;
				}
				else
				{
					guidParent = new Guid(arguments[num]);
				}
				num++;
				nInsertPosition = int.Parse(arguments[num]);
				num++;
				int num3 = int.Parse(arguments[num]);
				num++;
				string text = arguments[num];
				num++;
				string text2 = arguments[num];
				num++;
				if (arguments.Length == num2)
				{
					deviceId = ((IDeviceRepository)APEnvironment.DeviceRepository).CreateDeviceIdentification(num3, text, text2);
				}
				else
				{
					string text3 = arguments[num];
					num++;
					deviceId = (IDeviceIdentification)(object)((IDeviceRepository)APEnvironment.DeviceRepository).CreateModuleIdentification(num3, text, text2, text3);
				}
				stName = arguments[num];
				num++;
				Debug.Assert(num >= num2);
			}
			catch (Exception)
			{
				throw new BatchWrongArgumentTypeException(BatchCommand, num, s_requiredArgumentType[num]);
			}
		}

		internal static bool IsSelectable(ICommand command)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			Guid guid = TypeGuidAttribute.FromObject((object)command).Guid;
			if (((IEngine)APEnvironment.Engine).CommandManager is ICommandManager3)
			{
				return ((ICommandManager3)((IEngine)APEnvironment.Engine).CommandManager).GetCommandEnabled(guid);
			}
			return command.Enabled;
		}

		internal static string[] CheckConstraints(IMetaObject meta, string[] filter, bool bCheckParent, bool bUpdate)
		{
			bool flag = false;
			if (meta.Object is Connector && (meta.Object as Connector).Contraints.Count > 0)
			{
				flag = true;
			}
			if (meta.Object is ExplicitConnector && (meta.Object as ExplicitConnector).Contraints.Count > 0)
			{
				flag = true;
			}
			if (meta.Object is DeviceObject)
			{
				foreach (Connector item in (IEnumerable)(meta.Object as DeviceObject).Connectors)
				{
					if (item.Contraints.Count > 0)
					{
						flag = true;
					}
				}
			}
			if (meta.Object is SlotDeviceObject)
			{
				foreach (Connector item2 in (IEnumerable)(meta.Object as SlotDeviceObject).Connectors)
				{
					if (item2.Contraints.Count > 0)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				string[] array = new string[filter.Length + 1];
				filter.CopyTo(array, 0);
				if (bUpdate)
				{
					array[filter.Length] = UpdateConstraintCatalogueFilter.ConstraintFilter;
				}
				else if (!bCheckParent)
				{
					array[filter.Length] = ConstraintCatalogueFilter.ConstraintFilter;
				}
				else
				{
					array[filter.Length] = ParentConstraintCatalogueFilter.ConstraintFilter;
				}
				return array;
			}
			return filter;
		}

		public void OverridableExecuteBatch(string[] batchArgs)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (((IEngine)APEnvironment.Engine).CommandManager is ICommandManager2)
			{
				((ICommandManager2)((IEngine)APEnvironment.Engine).CommandManager).ExecuteCommand(TypeGuidAttribute.FromObject((object)this).Guid, batchArgs);
			}
			else
			{
				ExecuteBatchNonInteractive(batchArgs);
			}
		}
	}
}
