#define DEBUG
using System;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DataElementOnlineVarRefManager
	{
		private LDictionary<Guid, DataElementOnlineVarRefCollection> _htDeviceGuidToVarRefCollection = new LDictionary<Guid, DataElementOnlineVarRefCollection>();

		public DataElementOnlineVarRefManager()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			((IObjectManager)APEnvironment.ObjectMgr).ProjectClosed+=(new ProjectClosedEventHandler(OnProjectClosed));
			((IOnlineManager2)APEnvironment.OnlineMgr).AfterApplicationDownload+=(new AfterApplicationDownloadEventHandler(OnAfterApplicationDownload));
		}

		public void Add(DataElementOnlineVarRef ovr, Guid guidDevice)
		{
			DataElementOnlineVarRefCollection dataElementOnlineVarRefCollection = default(DataElementOnlineVarRefCollection);
			_htDeviceGuidToVarRefCollection.TryGetValue(guidDevice, out dataElementOnlineVarRefCollection);
			if (dataElementOnlineVarRefCollection == null)
			{
				dataElementOnlineVarRefCollection = new DataElementOnlineVarRefCollection();
				_htDeviceGuidToVarRefCollection[guidDevice]= dataElementOnlineVarRefCollection;
			}
			dataElementOnlineVarRefCollection.Add(ovr);
		}

		public void Remove(DataElementOnlineVarRef ovr, Guid guidDevice)
		{
			DataElementOnlineVarRefCollection dataElementOnlineVarRefCollection = default(DataElementOnlineVarRefCollection);
			_htDeviceGuidToVarRefCollection.TryGetValue(guidDevice, out dataElementOnlineVarRefCollection);
			dataElementOnlineVarRefCollection?.Remove(ovr);
		}

		private void OnProjectClosed(object sender, ProjectClosedEventArgs e)
		{
			_htDeviceGuidToVarRefCollection.Clear();
		}

		private void OnAfterApplicationDownload(object sender, OnlineEventArgs e)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (e is OnlineDownloadEventArgs2 && ((OnlineDownloadEventArgs2)e).DownloadException != null)
			{
				return;
			}
			try
			{
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject == null)
				{
					return;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, e.GuidObject);
				Guid deviceGuid = ((IOnlineApplicationObject)objectToRead.Object).DeviceGuid;
				Debug.Assert(deviceGuid != Guid.Empty);
				IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, deviceGuid).Object;
				IIoProvider val = (IIoProvider)(object)((@object is IIoProvider) ? @object : null);
				if (!((object)objectToRead).Equals((object)val.GetApplication()))
				{
					return;
				}
				DataElementOnlineVarRefCollection dataElementOnlineVarRefCollection = default(DataElementOnlineVarRefCollection);
				_htDeviceGuidToVarRefCollection.TryGetValue(deviceGuid, out dataElementOnlineVarRefCollection);
				if (dataElementOnlineVarRefCollection == null)
				{
					return;
				}
				foreach (DataElementOnlineVarRef item in dataElementOnlineVarRefCollection)
				{
					item.UpdateOnlineVarRef();
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}
	}
}
