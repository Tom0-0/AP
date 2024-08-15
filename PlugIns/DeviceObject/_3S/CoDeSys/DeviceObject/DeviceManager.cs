using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{72F09C2B-744B-428c-A640-27566E156F5E}")]
	[SystemInterface("_3S.CoDeSys.DeviceObject.IDeviceManager")]
	internal class DeviceManager : DeviceObjectManager, IDeviceManager11, IDeviceManager10, IDeviceManager9, IDeviceManager8, IDeviceManager7, IDeviceManager6, IDeviceManager5, IDeviceManager4, IDeviceManager3, IDeviceManager2, IDeviceManager, IDeviceManagerMessageStorage, IGenericInterfaceExtensionProvider
	{
		private static LDictionary<Guid, DeviceInfo> listeners = new LDictionary<Guid, DeviceInfo>();

		private LList<DeviceBuffer> buffers = new LList<DeviceBuffer>();

		private static bool _bDoNotSkipEvents = false;

		private static bool _bIsPLCOpenXMLImport = false;

		private static LDictionary<Guid, LDictionary<Guid, LDictionary<Guid, LList<IMessage>>>> _dictMessages = new LDictionary<Guid, LDictionary<Guid, LDictionary<Guid, LList<IMessage>>>>();

		private WeakMulticastDelegate _ehMessagesCleared;

		private WeakMulticastDelegate _ehMessageAdded;

		private static bool _bCheckOemCustomization = false;

		private WeakMulticastDelegate _ehDeviceUpdating;

		private WeakMulticastDelegate _ehDeviceUpdated;

		private WeakMulticastDelegate _ehDevicePlugging;

		private WeakMulticastDelegate _ehDevicePlugged;

		private WeakMulticastDelegate _ehDeviceShowException;

		private WeakMulticastDelegate _ehLogicalDeviceRenaming;

		internal static bool _bInterfaceIgnoreCaseForUpdateChecked = false;

		internal static Comparison<string> _comparison = null;

		private IGenericInterfaceExtensionProvider _baseGenericInterfaceExtensionProvider;

		private static WeakMulticastDelegate _ehDeviceAddressChanged = null;

		private static WeakMulticastDelegate _ehUpdateDeviceRemoving = null;

		internal static IDeviceManagerBuffer _PlcBuffer;

		public bool IsPLCOpenXMLImport
		{
			get
			{
				return _bIsPLCOpenXMLImport;
			}
			set
			{
				_bIsPLCOpenXMLImport = value;
			}
		}

		internal static bool IsDuringPLCOpenImport => _bIsPLCOpenXMLImport;

		internal static bool DoNotSkipEvents
		{
			get
			{
				if (!_bCheckOemCustomization)
				{
					_bCheckOemCustomization = true;
					if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("DeviceObject", "SkipObjectEventsForNonPrimaryProjects"))
					{
						_bDoNotSkipEvents = !((IEngine3)APEnvironment.Engine).OEMCustomization.GetBoolValue("DeviceObject", "SkipObjectEventsForNonPrimaryProjects");
					}
				}
				return _bDoNotSkipEvents;
			}
		}

		public IGenericInterfaceExtensionProvider GenericInterfaceExtensionProvider => _baseGenericInterfaceExtensionProvider;

		public bool SkipObjectEventsForNonPrimaryProjects
		{
			get
			{
				return !_bDoNotSkipEvents;
			}
			set
			{
				_bDoNotSkipEvents = !value;
			}
		}

		public event DeviceManagerMessageClearEventHandler MessagesCleared
		{
			add
			{
				_ehMessagesCleared = WeakMulticastDelegate.CombineUnique(_ehMessagesCleared, (Delegate)(object)value);
			}
			remove
			{
				_ehMessagesCleared = WeakMulticastDelegate.Remove(_ehMessagesCleared, (Delegate)(object)value);
			}
		}

		public event DeviceManagerMessageEventHandler MessageAdded
		{
			add
			{
				_ehMessageAdded = WeakMulticastDelegate.CombineUnique(_ehMessageAdded, (Delegate)(object)value);
			}
			remove
			{
				_ehMessageAdded = WeakMulticastDelegate.Remove(_ehMessageAdded, (Delegate)(object)value);
			}
		}

		public event DeviceCancelEventHandler DeviceUpdating
		{
			add
			{
				_ehDeviceUpdating = WeakMulticastDelegate.CombineUnique(_ehDeviceUpdating, (Delegate)(object)value);
			}
			remove
			{
				_ehDeviceUpdating = WeakMulticastDelegate.Remove(_ehDeviceUpdating, (Delegate)(object)value);
			}
		}

		public event DeviceEventHandler DeviceUpdated
		{
			add
			{
				_ehDeviceUpdated = WeakMulticastDelegate.CombineUnique(_ehDeviceUpdated, (Delegate)(object)value);
			}
			remove
			{
				_ehDeviceUpdated = WeakMulticastDelegate.Remove(_ehDeviceUpdated, (Delegate)(object)value);
			}
		}

		public event DeviceCancelEventHandler DevicePlugging
		{
			add
			{
				_ehDevicePlugging = WeakMulticastDelegate.CombineUnique(_ehDevicePlugging, (Delegate)(object)value);
			}
			remove
			{
				_ehDevicePlugging = WeakMulticastDelegate.Remove(_ehDevicePlugging, (Delegate)(object)value);
			}
		}

		public event DeviceEventHandler DevicePlugged
		{
			add
			{
				_ehDevicePlugged = WeakMulticastDelegate.CombineUnique(_ehDevicePlugged, (Delegate)(object)value);
			}
			remove
			{
				_ehDevicePlugged = WeakMulticastDelegate.Remove(_ehDevicePlugged, (Delegate)(object)value);
			}
		}

		public event DeviceShowExeptionHandler DeviceShowException
		{
			add
			{
				_ehDeviceShowException = WeakMulticastDelegate.CombineUnique(_ehDeviceShowException, (Delegate)(object)value);
			}
			remove
			{
				_ehDeviceShowException = WeakMulticastDelegate.Remove(_ehDeviceShowException, (Delegate)(object)value);
			}
		}

		public event EventHandler<LogicalDeviceCancelEventArgs> LogicalDeviceRenaming
		{
			add
			{
				_ehLogicalDeviceRenaming = WeakMulticastDelegate.CombineUnique(_ehLogicalDeviceRenaming, (Delegate)value);
			}
			remove
			{
				_ehLogicalDeviceRenaming = WeakMulticastDelegate.Remove(_ehLogicalDeviceRenaming, (Delegate)value);
			}
		}

		public event DeviceAddressChangingEventHandler DeviceAddressChanging
		{
			add
			{
				_ehDeviceAddressChanged = WeakMulticastDelegate.CombineUnique(_ehDeviceAddressChanged, (Delegate)(object)value);
			}
			remove
			{
				_ehDeviceAddressChanged = WeakMulticastDelegate.Remove(_ehDeviceAddressChanged, (Delegate)(object)value);
			}
		}

		public event EventHandler<DeviceCancelEventArgs> UpdateDeviceRemoving
		{
			add
			{
				_ehUpdateDeviceRemoving = WeakMulticastDelegate.CombineUnique(_ehUpdateDeviceRemoving, (Delegate)value);
			}
			remove
			{
				_ehUpdateDeviceRemoving = WeakMulticastDelegate.Remove(_ehUpdateDeviceRemoving, (Delegate)value);
			}
		}

		public DeviceManager()
		{
			_baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		public IDeviceManagerBuffer CreateDeviceBuffer(Predicate<IDeviceManagerInfo> match)
		{
			DeviceBuffer deviceBuffer = new DeviceBuffer(match);
			buffers.Add(deviceBuffer);
			return (IDeviceManagerBuffer)(object)deviceBuffer;
		}

		protected override void RegisterObjectEvents()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Expected O, but got Unknown
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Expected O, but got Unknown
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded+=(new ObjectEventHandler(OnObjectAdded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectLoaded+=(new ObjectEventHandler(OnObjectLoaded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectModified+=(new ObjectModifiedEventHandler(OnObjectModified));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved+=(new ObjectRemovedEventHandler(OnObjectRemoved));
			if (((IObjectManager)APEnvironment.ObjectMgr).ProjectConverterFactoryManager is IProjectConverterFactoryManager2)
			{
				((IProjectConverterFactoryManager2)((IObjectManager)APEnvironment.ObjectMgr).ProjectConverterFactoryManager).BeforeProjectConversion+=(new ProjectBeforeImportEventHandler(OnBeforeProjectConversion));
				((IProjectConverterFactoryManager2)((IObjectManager)APEnvironment.ObjectMgr).ProjectConverterFactoryManager).AfterProjectConversion+=(new ProjectAfterImportEventHandler(OnAfterProjectConversion));
			}
			((IEngine)APEnvironment.Engine).Projects.AfterPrimaryProjectSwitched+=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
			((ILanguageModelManager)APEnvironment.LanguageModelMgr).AfterCompile+=(new CompileEventHandler(LanguageModelMgr_AfterCompile));
			_PlcBuffer = CreateDeviceBuffer(IsPLC);
		}

		private void LanguageModelMgr_AfterCompile(object sender, CompileEventArgs e)
		{
			int handle = APEnvironment.Engine.Projects.PrimaryProject.Handle;
			IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(handle, e.ApplicationGuid);
			if (hostStub != null)
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, e.ApplicationGuid);
				if (!typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IDeviceObjectBase deviceObjectBase = APEnvironment.ObjectMgr.GetObjectToRead(handle, hostStub.ObjectGuid).Object as IDeviceObjectBase;
					if (deviceObjectBase != null && e.ApplicationGuid != (deviceObjectBase.DriverInfo as IDriverInfo2).IoApplication)
					{
						return;
					}
				}
			}
			LDictionary<Guid, LDictionary<Guid, LList<IMessage>>> ldictionary;
			if (DeviceManager._dictMessages.TryGetValue(e.ApplicationGuid, out ldictionary))
			{
				foreach (KeyValuePair<Guid, LDictionary<Guid, LList<IMessage>>> keyValuePair in ldictionary)
				{
					if (APEnvironment.ObjectMgr.ExistsObject(handle, keyValuePair.Key))
					{
						foreach (KeyValuePair<Guid, LList<IMessage>> keyValuePair2 in keyValuePair.Value)
						{
							foreach (IMessage message in keyValuePair2.Value)
							{
								APEnvironment.MessageStorage.AddMessage(APEnvironment.CompilerMessageCategory, message);
							}
						}
					}
				}
			}
		}

		public void AddMessage(Guid applicationGuid, Guid objectGuid, Guid providerGuid, IMessage message)
		{
			LDictionary<Guid, LDictionary<Guid, LList<IMessage>>> val = default(LDictionary<Guid, LDictionary<Guid, LList<IMessage>>>);
			if (!_dictMessages.TryGetValue(applicationGuid, out val))
			{
				val = new LDictionary<Guid, LDictionary<Guid, LList<IMessage>>>();
				_dictMessages.Add(applicationGuid, val);
			}
			LDictionary<Guid, LList<IMessage>> val2 = default(LDictionary<Guid, LList<IMessage>>);
			if (!val.TryGetValue(objectGuid, out val2))
			{
				val2 = new LDictionary<Guid, LList<IMessage>>();
				val.Add(objectGuid, val2);
			}
			LList<IMessage> val3 = default(LList<IMessage>);
			if (!val2.TryGetValue(providerGuid, out val3))
			{
				val3 = new LList<IMessage>();
				val2.Add(providerGuid, val3);
			}
			val3.Add(message);
			FireMessageAdded(applicationGuid, objectGuid, message);
		}

		public void ClearMessages(Guid applicationGuid, Guid objectGuid, Guid providerGuid)
		{
			LDictionary<Guid, LDictionary<Guid, LList<IMessage>>> ldictionary;
			if (DeviceManager._dictMessages.TryGetValue(applicationGuid, out ldictionary))
			{
				if (objectGuid != Guid.Empty)
				{
					LDictionary<Guid, LList<IMessage>> ldictionary2;
					LList<IMessage> llist;
					if (ldictionary.TryGetValue(objectGuid, out ldictionary2) && ldictionary2.TryGetValue(providerGuid, out llist))
					{
						llist.Clear();
						this.FireMessagesCleared(applicationGuid, objectGuid);
						return;
					}
				}
				else
				{
					foreach (KeyValuePair<Guid, LDictionary<Guid, LList<IMessage>>> keyValuePair in ldictionary)
					{
						LList<IMessage> llist2;
						if (keyValuePair.Value.TryGetValue(providerGuid, out llist2))
						{
							llist2.Clear();
							this.FireMessagesCleared(applicationGuid, keyValuePair.Key);
						}
					}
				}
			}
		}

		public void ClearMessages(Guid applicationGuid, Guid providerGuid)
		{
			ClearMessages(applicationGuid, Guid.Empty, providerGuid);
		}

		public IMessage[] GetMessages(Guid applicationGuid, Guid providerGuid)
		{
			return GetMessages(applicationGuid, providerGuid, (Severity?)null);
		}

		public IMessage[] GetMessages(Guid applicationGuid, Guid providerGuid, Severity severity)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			return GetMessages(applicationGuid, providerGuid, severity);
		}

		internal IMessage[] GetMessages(Guid applicationGuid, Guid providerGuid, Severity? severity)
		{
			LList<IMessage> llist = new LList<IMessage>();
			LDictionary<Guid, LDictionary<Guid, LList<IMessage>>> ldictionary;
			if (DeviceManager._dictMessages.TryGetValue(applicationGuid, out ldictionary))
			{
				using (LDictionary<Guid, LDictionary<Guid, LList<IMessage>>>.ValueCollection.Enumerator enumerator = ldictionary.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						LList<IMessage> llist2;
						if (enumerator.Current.TryGetValue(providerGuid, out llist2))
						{
							if (severity != null)
							{
								using (IEnumerator<IMessage> enumerator2 = llist2.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										IMessage message = enumerator2.Current;
										if (message.Severity == severity)
										{
											llist.Add(message);
										}
									}
									continue;
								}
							}
							llist.AddRange(llist2);
						}
					}
				}
			}
			if (llist.Count > 0)
			{
				IMessage[] array = (IMessage[])(object)new IMessage[llist.Count];
				llist.CopyTo(array);
				return array;
			}
			return null;
		}

		private void FireMessagesCleared(Guid applicationGuid, Guid objectGuid)
		{
			if (_ehMessagesCleared != null)
			{
				_ehMessagesCleared.Invoke(new object[2] { applicationGuid, objectGuid });
			}
		}

		private void FireMessageAdded(Guid applicationGuid, Guid objectGuid, IMessage message)
		{
			if (_ehMessageAdded != null)
			{
				_ehMessageAdded.Invoke(new object[3] { applicationGuid, objectGuid, message });
			}
		}

		private void OnObjectAdded(object sender, ObjectEventArgs e)
		{
			UpdateObject(e.ProjectHandle, e.ObjectGuid);
		}

		private void OnObjectLoaded(object sender, ObjectEventArgs e)
		{
			UpdateObject(e.ProjectHandle, e.ObjectGuid);
		}

		private void OnObjectRemoved(object sender, ObjectRemovedEventArgs e)
		{
			if (DeviceManager._dictMessages.ContainsKey(e.MetaObject.ObjectGuid))
			{
				DeviceManager._dictMessages.Remove(e.MetaObject.ObjectGuid);
			}
			foreach (LDictionary<Guid, LDictionary<Guid, LList<IMessage>>> ldictionary in DeviceManager._dictMessages.Values)
			{
				if (ldictionary.ContainsKey(e.MetaObject.ObjectGuid))
				{
					ldictionary.Remove(e.MetaObject.ObjectGuid);
				}
			}
			foreach (DeviceBuffer deviceBuffer in this.buffers)
			{
				deviceBuffer.NofifyRemoved(e.MetaObject.ObjectGuid);
			}
		}

		private void OnObjectModified(object sender, ObjectModifiedEventArgs e)
		{
			UpdateObject(e.ProjectHandle, e.ObjectGuid);
		}

		private void OnBeforeProjectConversion(object sender, ProjectBeforeImportEventArgs e)
		{
			_bDoNotSkipEvents = true;
		}

		private void OnAfterProjectConversion(object sender, ProjectAfterImportEventArgs e)
		{
			if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("DeviceObject", "SkipObjectEventsForNonPrimaryProjects"))
			{
				_bDoNotSkipEvents = !((IEngine3)APEnvironment.Engine).OEMCustomization.GetBoolValue("DeviceObject", "SkipObjectEventsForNonPrimaryProjects");
			}
			else
			{
				_bDoNotSkipEvents = false;
			}
		}

		private void OnPrimaryProjectSwitched(IProject oldProject, IProject newProject)
		{
			_dictMessages.Clear();
			if (newProject != null)
			{
				foreach (DeviceBuffer buffer in buffers)
				{
					buffer.NofifyProjectSwitch(newProject.Handle);
				}
				return;
			}
			foreach (DeviceBuffer buffer2 in buffers)
			{
				buffer2.NofifyClear();
			}
		}

		internal void UpdateObject(int projectHandle, Guid guid)
		{
			DeviceManagerInfo deviceManagerInfo = LoadInfoIntern(projectHandle, guid);
			if (deviceManagerInfo == null)
			{
				return;
			}
			foreach (DeviceBuffer buffer in buffers)
			{
				buffer.NofifyUpdate(deviceManagerInfo, projectHandle);
			}
		}

		public IDeviceManagerInfo CreateInfo(int projectHandle, Guid guid)
		{
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, guid);
			if (objectToRead != null)
			{
				return (IDeviceManagerInfo)(object)new DeviceManagerInfo(objectToRead.Object);
			}
			return null;
		}

		internal static DeviceManagerInfo LoadInfoIntern(int projectHandle, Guid guid)
		{
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, guid);
			if (objectToRead != null)
			{
				return new DeviceManagerInfo(objectToRead.Object);
			}
			return null;
		}

		public IEnumerable<IDeviceManagerInfo> GetChildDevices(IConnector parentCon_)
		{
			if ((int)parentCon_.ConnectorRole != 0)
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)0, "Not a parent connector !");
			}
			IConnector5 val = (IConnector5)(object)((parentCon_ is IConnector5) ? parentCon_ : null);
			IIoModuleIterator iterator = val.CreateModuleIterator();
			try
			{
				if (!iterator.MoveToFirstChild())
				{
					yield break;
				}
				do
				{
					if (iterator.Current.IsConnector)
					{
						IIoModuleEditorHelper editorhelper = iterator.Current.CreateEditorHelper();
						try
						{
							IConnector connector = editorhelper.GetConnector(false);
							yield return (IDeviceManagerInfo)(object)new DeviceManagerInfo((IObject)(object)connector.GetDeviceObject());
						}
						finally
						{
							((IDisposable)editorhelper)?.Dispose();
						}
					}
				}
				while (iterator.MoveToNextSibling());
			}
			finally
			{
				((IDisposable)iterator)?.Dispose();
			}
		}

		public IDeviceManagerInfo GetParent(IConnector connector)
		{
			IIoModuleIterator val = ((IConnector5)((connector is IConnector5) ? connector : null)).CreateModuleIterator();
			try
			{
				if (val.MoveToParent() && val.Current.IsConnector)
				{
					IIoModuleEditorHelper val2 = val.Current.CreateEditorHelper();
					try
					{
						return (IDeviceManagerInfo)(object)new DeviceManagerInfo((IObject)(object)val2.GetConnector(false).GetDeviceObject());
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return null;
		}

		public IDeviceManagerInfo CreateInfo(IObject obj)
		{
			return (IDeviceManagerInfo)(object)new DeviceManagerInfo(obj);
		}

		internal void RaiseLogicalDeviceRenaming(int nProjectHandle, Guid objectGuid)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			LogicalDeviceCancelEventArgs val = new LogicalDeviceCancelEventArgs(nProjectHandle, objectGuid);
			OnLogicalDeviceRenaming(val);
			if (val.Exception != null)
			{
				throw val.Exception;
			}
		}

		protected virtual void OnLogicalDeviceRenaming(LogicalDeviceCancelEventArgs e)
		{
			if (_ehLogicalDeviceRenaming != null)
			{
				_ehLogicalDeviceRenaming.Invoke(new object[2] { this, e });
			}
		}

		internal void RaiseDeviceUpdating(int nProjectHandle, Guid objectGuid, IDeviceIdentification devId)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Expected O, but got Unknown
			DeviceCancelEventArgs val = new DeviceCancelEventArgs(nProjectHandle, objectGuid, devId);
			OnDeviceUpdating(val);
			if (val.Exception != null)
			{
				throw val.Exception;
			}
		}

		internal void RaiseDeviceUpdated(int nProjectHandle, Guid objectGuid, IDeviceIdentification devIdNew, IDeviceIdentification devIdOld)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			DeviceEventArgs2 e = new DeviceEventArgs2(nProjectHandle, objectGuid, devIdNew, devIdOld);
			OnDeviceUpdated((DeviceEventArgs)(object)e);
		}

		internal void RaiseDeviceUpdated(DeviceEventArgs2 e)
		{
			OnDeviceUpdated((DeviceEventArgs)(object)e);
		}

		internal void RaiseDevicePlugging(int nProjectHandle, Guid objectGuid, IDeviceIdentification devId)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Expected O, but got Unknown
			DeviceCancelEventArgs val = new DeviceCancelEventArgs(nProjectHandle, objectGuid, devId);
			OnDevicePlugging(val);
			if (val.Exception != null)
			{
				throw val.Exception;
			}
		}

		internal void RaiseDevicePlugged(int nProjectHandle, Guid objectGuid, IDeviceIdentification devId)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Expected O, but got Unknown
			DeviceEventArgs e = new DeviceEventArgs(nProjectHandle, objectGuid, devId);
			OnDevicePlugged(e);
		}

		protected virtual void OnDeviceUpdating(DeviceCancelEventArgs e)
		{
			if (_ehDeviceUpdating != null)
			{
				_ehDeviceUpdating.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnDeviceUpdated(DeviceEventArgs e)
		{
			if (_ehDeviceUpdated != null)
			{
				_ehDeviceUpdated.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnDevicePlugging(DeviceCancelEventArgs e)
		{
			if (_ehDevicePlugging != null)
			{
				_ehDevicePlugging.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnDevicePlugged(DeviceEventArgs e)
		{
			if (_ehDevicePlugged != null)
			{
				_ehDevicePlugged.Invoke(new object[2] { this, e });
			}
		}

		internal bool RaiseDeviceShowExecption(int nProjectHandle, ArrayList objects, string stMessage)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Expected O, but got Unknown
			DeviceShowExeptionArgs val = new DeviceShowExeptionArgs(nProjectHandle, objects, stMessage);
			OnShowExecption(val);
			if (val.Cancel)
			{
				return false;
			}
			return true;
		}

		protected virtual void OnShowExecption(DeviceShowExeptionArgs e)
		{
			if (_ehDeviceShowException != null)
			{
				_ehDeviceShowException.Invoke(new object[2] { this, e });
			}
		}

		public ICreateDeviceDialog GetCreateDeviceDialog(IMetaObject target, IDeviceCatalogueFilter customFilter)
		{
			ScanImportDeviceForm scanImportDeviceForm = new ScanImportDeviceForm();
			scanImportDeviceForm.CustomFilter = customFilter;
			if (target != null)
			{
				scanImportDeviceForm.ChangeTarget(target);
			}
			return (ICreateDeviceDialog)(object)scanImportDeviceForm;
		}

		public void ChangeActiveChildConnector(int projectHandle, Guid deviceGuid, int connectorID)
		{
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(projectHandle);
			bool flag = undoManager.InCompoundAction || undoManager.InRedo || undoManager.InUndo;
			if (!flag)
			{
				undoManager.BeginCompoundAction("DeviceManager.ChangeActiveChildConnector");
			}
			try
			{
				HostpathUpdate.ChangeActiveChildConnector(projectHandle, deviceGuid, connectorID);
			}
			finally
			{
				if (!flag)
				{
					undoManager.EndCompoundAction();
				}
			}
		}

		internal static Guid[] GetOrderedSubGuids(IDeviceObject device)
		{
			Guid[] subObjectGuids = ((IObject)device).MetaObject.SubObjectGuids;
			Guid[] array = new Guid[subObjectGuids.Length];
			IOrderedSubObjects val = (IOrderedSubObjects)(object)((device is IOrderedSubObjects) ? device : null);
			if (val != null)
			{
				Guid[] array2 = subObjectGuids;
				foreach (Guid guid in array2)
				{
					int childIndex = val.GetChildIndex(guid);
					array[childIndex] = guid;
				}
			}
			return array;
		}

		internal static Guid[] GetOrderedSubGuids(IMetaObject meta)
		{
			Guid[] subObjectGuids = meta.SubObjectGuids;
			Guid[] array = new Guid[subObjectGuids.Length];
			IObject @object = meta.Object;
			IOrderedSubObjects val = (IOrderedSubObjects)(object)((@object is IOrderedSubObjects) ? @object : null);
			if (val != null)
			{
				Guid[] array2 = subObjectGuids;
				foreach (Guid guid in array2)
				{
					int childIndex = val.GetChildIndex(guid);
					array[childIndex] = guid;
				}
			}
			return array;
		}

		internal static bool CompareInterfaces(string stInterface1, string stInterface2)
		{
			if (stInterface1 == stInterface2)
			{
				return true;
			}
			if (!_bInterfaceIgnoreCaseForUpdateChecked && ((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("DeviceObject", "InterfaceCompareForUpdate"))
			{
				_bInterfaceIgnoreCaseForUpdateChecked = true;
				_comparison = ((IEngine3)APEnvironment.Engine).OEMCustomization.GetValue("DeviceObject", "InterfaceCompareForUpdate") as Comparison<string>;
			}
			if (_comparison != null && _comparison(stInterface1, stInterface2) == 0)
			{
				return true;
			}
			return false;
		}

		internal static bool CheckMatchInterface(IConnector7 parentConnector, IConnector7 childConnector)
		{
			if (CompareInterfaces(((IConnector)parentConnector).Interface, ((IConnector)childConnector).Interface))
			{
				return true;
			}
			foreach (string additionalInterface in parentConnector.AdditionalInterfaces)
			{
				if (CompareInterfaces(additionalInterface, ((IConnector)childConnector).Interface))
				{
					return true;
				}
			}
			LDictionary<string, string> val = new LDictionary<string, string>();
			foreach (string additionalInterface2 in childConnector.AdditionalInterfaces)
			{
				if (CompareInterfaces(additionalInterface2, ((IConnector)parentConnector).Interface))
				{
					return true;
				}
				val[additionalInterface2]= string.Empty;
			}
			foreach (string additionalInterface3 in parentConnector.AdditionalInterfaces)
			{
				if (val.ContainsKey(additionalInterface3))
				{
					return true;
				}
			}
			return false;
		}

		public IConnector ActiveChildConnector(IDeviceObject device)
		{
			return (IConnector)(object)GetActiveChildConnector(device);
		}

		internal static Connector GetActiveChildConnector(IDeviceObject device)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Invalid comparison between Unknown and I4
			foreach (Connector item in (IEnumerable)device.Connectors)
			{
				if ((int)item.ConnectorRole == 1 && GetParentDeviceGuid(item) != Guid.Empty)
				{
					return item;
				}
			}
			return null;
		}

		internal static Guid GetParentDeviceGuid(Connector connector)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			if ((int)connector.ConnectorRole == 1)
			{
				foreach (IAdapter item in (IEnumerable)connector.Adapters)
				{
					IAdapter val = item;
					if (val is ISlotAdapter && val.Modules.Length != 0 && val.Modules[0] != Guid.Empty)
					{
						return val.Modules[0];
					}
				}
				return Guid.Empty;
			}
			throw new DeviceObjectException((DeviceObjectExeptionReason)1, "Not a child connector !)");
		}

		private IDirectVariable ParseDirectAddress(string st)
		{
			IScanner val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner("", false, false, false, false);
			IParser obj = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateParser(val);
			val.Initialize(st);
			IExpression val2 = obj.ParseOperand();
			if (val2 != null && val2 is IAddressExpression)
			{
				return ((IAddressExpression)((val2 is IAddressExpression) ? val2 : null)).DirectAddress;
			}
			return null;
		}

		public bool CheckIOChannelIsUpdated(int nProjectHandle, Guid appGuid, string stChannelVariable)
		{
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			bool result = true;
			if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, appGuid))
			{
				ICompileContext referenceContextIfAvailable = ((ILanguageModelManager2)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(appGuid);
				if (referenceContextIfAvailable != null)
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, appGuid);
					string text = $"IoConfig_{metaObjectStub.Name}_Mappings";
					ISignature signature = ((ICompileContextCommon)referenceContextIfAvailable).GetSignature(text);
					if (signature != null)
					{
						IDataLocation val = null;
						IDirectVariable val2 = null;
						if (stChannelVariable.Contains("%"))
						{
							val2 = ParseDirectAddress(stChannelVariable);
							bool flag = default(bool);
							val = referenceContextIfAvailable.LocateAddress(out flag, val2);
						}
						bool flag2 = false;
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)0))
						{
							result = false;
						}
						IVariable[] all = signature.All;
						for (int i = 0; i < all.Length; i++)
						{
							string[] attributes = all[i].Attributes;
							foreach (string text2 in attributes)
							{
								if (!text2.StartsWith("MapInformation"))
								{
									continue;
								}
								result = false;
								string[] array = text2.Split(new char[2] { ',', ' ' }, 4);
								if (array.Length < 4)
								{
									continue;
								}
								int.TryParse(array[1], out var result2);
								int.TryParse(array[2], out var result3);
								if (val != null && val2 != null && array[3].StartsWith("%"))
								{
									IDirectVariable val3 = ParseDirectAddress(array[3]);
									int num = val.Offset * 8;
									if (val.IsBitLocation)
									{
										num += val.BitNr;
									}
									if (num >= result2 && num < result2 + result3 && val3.Location == val2.Location)
									{
										result = true;
										flag2 = true;
										break;
									}
								}
								if (array[3] == stChannelVariable)
								{
									result = true;
									flag2 = true;
									break;
								}
							}
							if (flag2)
							{
								break;
							}
						}
					}
				}
			}
			return result;
		}

		internal bool TryGetTargetDeviceID(int projectHandle, Guid objectGuid, IDeviceIdentification defaultDeviceID, out IDeviceIdentification deviceID)
		{
			deviceID = (from provider in APEnvironment.CreateUpdateDeviceIDProviders()
				select provider.GetTargetDeviceID(projectHandle, objectGuid, defaultDeviceID)).FirstOrDefault((IDeviceIdentification id) => id != null);
			return deviceID != null;
		}

		public void AttachToEvent(string stEvent, GenericEventDelegate callback)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.AttachToEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		public void DetachFromEvent(string stEvent, GenericEventDelegate callback)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.DetachFromEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		public void RaiseEvent(string stEvent, XmlDocument eventData)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.RaiseEvent(stEvent, eventData);
				return;
			}
			throw new NotImplementedException();
		}

		public bool IsFunctionAvailable(string stFunction)
		{
			if (stFunction == null)
			{
				throw new ArgumentNullException("stFunction");
			}
			return false;
		}

		public XmlDocument CallFunction(string stFunction, XmlDocument functionData)
		{
			throw new NotImplementedException();
		}

		internal static void NotifyNewDeviceAddress(object sender, Guid objectGuid, IDeviceAddress newAddress)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (_ehDeviceAddressChanged != null)
			{
				_ehDeviceAddressChanged.Invoke(new object[2]
				{
					sender,
					(object)new DeviceAddressChangingEventArgs(objectGuid, newAddress)
				});
			}
		}

		public ICollection<uint> GetLogicalIdentifications(int nProjectHandle, Guid guidToIgnore)
		{
			List<uint> list = new List<uint>();
			if (DeviceObjectHelper.AdditionalModules != null && DeviceObjectHelper.AdditionalModules.DeviceGuids != null)
			{
				LogicalIONotification.CollectIdentifications(nProjectHandle, DeviceObjectHelper.AdditionalModules.DeviceGuids, list, guidToIgnore);
			}
			if (DeviceObjectHelper.LogicalDevices != null)
			{
				LogicalIONotification.CollectIdentifications(nProjectHandle, DeviceObjectHelper.LogicalDevices.DeviceGuids, list, guidToIgnore);
			}
			return list;
		}

		internal static void OnUpdateDeviceRemoving(object sender, int nProjectHandle, Guid objectGuid, IDeviceIdentification devId)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			if (_ehUpdateDeviceRemoving != null)
			{
				DeviceCancelEventArgs val = new DeviceCancelEventArgs(nProjectHandle, objectGuid, devId);
				_ehUpdateDeviceRemoving.Invoke(new object[2] { sender, val });
				if (val.Exception != null)
				{
					throw val.Exception;
				}
			}
		}

		internal bool IsPLC(IDeviceManagerInfo deviceInfo)
		{
			if (deviceInfo.IsDevice)
			{
				IObject @object = deviceInfo.Object;
				return ((IDeviceObject)((@object is IDeviceObject) ? @object : null)).AllowsDirectCommunication;
			}
			return false;
		}

		public IMetaObjectStub GetPlcDevice(int nProjectHandle, Guid ObjectGuid)
		{
			if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, ObjectGuid))
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, ObjectGuid);
				while (metaObjectStub.ParentObjectGuid != Guid.Empty)
				{
					IDeviceManagerBuffer plcBuffer = _PlcBuffer;
					if (((plcBuffer != null) ? plcBuffer.DeviceGuids : null) != null && _PlcBuffer.DeviceGuids.Contains(metaObjectStub.ObjectGuid))
					{
						return metaObjectStub;
					}
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
				}
				IDeviceManagerBuffer plcBuffer2 = _PlcBuffer;
				if (((plcBuffer2 != null) ? plcBuffer2.DeviceGuids : null) != null && _PlcBuffer.DeviceGuids.Contains(metaObjectStub.ObjectGuid))
				{
					return metaObjectStub;
				}
			}
			return null;
		}
	}
}
