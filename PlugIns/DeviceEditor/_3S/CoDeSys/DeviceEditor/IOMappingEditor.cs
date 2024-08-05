using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;

namespace _3S.CoDeSys.DeviceEditor
{
	public abstract class IOMappingEditor
	{
		private FbInstanceTreeTableModel _modelInstances;

		private ParameterTreeTableModel _modelChannels;

		private IoMappingEditorPage _page;

		private IECObjectsPage _objectsPage;

		private Guid _guidObject;

		private int _nConnectors;

		private IUndoManager _undoMgr;

		private bool _bLocalizationActive;

		public IoMappingEditorPage MappingPage => _page;

		internal abstract string PageName { get; }

		internal abstract IHasOnlineMode HasOnlineMode { get; }

		internal bool IsApplicationOnline
		{
			get
			{
				try
				{
					IDeviceObject host = GetHost();
					if (host != null && ((IObject)host).MetaObject != null)
					{
						foreach (Guid application2 in DeviceHelper.GetApplications(((IObject)host).MetaObject, bWithHidden: true))
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)host).MetaObject.ProjectHandle, application2);
							if (!typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(application2);
								if (application != null && application.IsLoggedIn)
								{
									return true;
								}
							}
						}
					}
				}
				catch
				{
				}
				return false;
			}
		}

		internal OnlineState OnlineState
		{
			get
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				IHasOnlineMode hasOnlineMode = HasOnlineMode;
				if (hasOnlineMode != null && hasOnlineMode.OnlineState.OnlineApplication == Guid.Empty)
				{
					try
					{
						if (DeviceHelper.GetOnlineDevice(GetHost(), hasOnlineMode))
						{
							return hasOnlineMode.OnlineState;
						}
					}
					catch
					{
					}
				}
				if (hasOnlineMode == null)
				{
					return default(OnlineState);
				}
				return hasOnlineMode.OnlineState;
			}
		}

		internal bool LogicalDeviceIsMapped
		{
			get
			{
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				if (EnableLogicalDevices)
				{
					IDeviceObject deviceObject = GetDeviceObject(bToModify: false);
					if (deviceObject != null && deviceObject is ILogicalDevice)
					{
						ILogicalDevice val = (ILogicalDevice)(object)((deviceObject is ILogicalDevice) ? deviceObject : null);
						if (deviceObject is ILogicalDevice2 && ((ILogicalDevice2)((deviceObject is ILogicalDevice2) ? deviceObject : null)).MappingPossible)
						{
							return false;
						}
						if (val.IsPhysical)
						{
							foreach (IMappedDevice item in (IEnumerable)val.MappedDevices)
							{
								if (item.IsMapped)
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			}
		}

		public IEditorPage[] Pages
		{
			get
			{
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Invalid comparison between Unknown and I4
				List<IEditorPage> list = new List<IEditorPage>();
				if (_page.HasLogicalDevices && _nConnectors > 1 && _page != null && _page.Editor is ConnectorIoMappingEditor)
				{
					int connectorId = (_page.Editor as ConnectorIoMappingEditor).ConnectorId;
					IConnectorEditorFrame frame = GetFrame();
					if (frame != null)
					{
						IConnector connector = frame.GetConnector(connectorId, false);
						if (connector != null && (int)connector.ConnectorRole == 1)
						{
							list.Add((IEditorPage)(object)_page);
						}
					}
				}
				if ((_page.CycleTaskEnable || (_modelChannels != null && ((AbstractTreeTableModel)_modelChannels).Sentinel.HasChildren) || (_page.HasLogicalDevices && _nConnectors <= 1)) && !list.Contains((IEditorPage)(object)_page))
				{
					list.Add((IEditorPage)(object)_page);
				}
				if (_objectsPage != null && _modelInstances != null && ((DefaultTreeTableModel)_modelInstances).Sentinel.HasChildren)
				{
					list.Add((IEditorPage)(object)_objectsPage);
				}
				IEditorPage[] array = (IEditorPage[])(object)new IEditorPage[list.Count];
				list.CopyTo(array);
				return array;
			}
		}

		public bool HideGenericEditor => true;

		internal IUndoManager EditorUndoMgr => _undoMgr;

		internal virtual Guid ObjectGuid
		{
			get
			{
				return _guidObject;
			}
			set
			{
				_guidObject = value;
			}
		}

		internal IoMappingEditorPage Page => _page;

		internal ParameterTreeTableModel ChannelsModel => _modelChannels;

		internal FbInstanceTreeTableModel InstancesModel => _modelInstances;

		public static bool EnableLogicalDevices
		{
			get
			{
				IFeatureSettingsManager featureSettingsMgrOrNull = APEnvironment.FeatureSettingsMgrOrNull;
				if (featureSettingsMgrOrNull != null)
				{
					return featureSettingsMgrOrNull.GetFeatureSettingValue("device-management", "enable-logicaldevices", false);
				}
				return true;
			}
		}

		public bool LocalizationActive => _bLocalizationActive;

		internal abstract IParameterSetProvider GetParameterSetProvider();

		internal abstract IDriverInfo GetDriverInfo(bool bToModify);

		internal abstract IDeviceObject GetHost();

		internal abstract IConnector Connector(bool bModify);

		internal abstract IDeviceObject GetDeviceObject(bool bToModify);

		protected abstract void OnParameterAdded(object sender, ParameterEventArgs e);

		protected abstract void OnParameterRemoved(object sender, ParameterEventArgs e);

		protected abstract void OnParameterChanged(object sender, ParameterChangedEventArgs e);

		protected abstract void OnParameterSectionChanged(object sender, ParameterSectionChangedEventArgs e);

		protected abstract void OnParameterMoved(object sender, ParameterMovedEventArgs e);

		public bool HideParameter(int nParameterId, string[] componentPath)
		{
			return true;
		}

		public void Reload()
		{
			IConnectorEditorFrame frame = GetFrame();
			if (frame != null)
			{
				if (((IEditorView)frame).Editor is DeviceEditor && !(((IEditorView)frame).Editor as DeviceEditor).OpenView)
				{
					if (_page == null)
					{
						_page = new IoMappingEditorPage(this);
					}
					if (_objectsPage == null)
					{
						_objectsPage = new IECObjectsPage(this);
					}
					return;
				}
				_undoMgr = (frame as DeviceEditor).EditorUndoMgr;
				IDeviceObject associatedDeviceObject = frame.GetAssociatedDeviceObject(false);
				if (associatedDeviceObject != null)
				{
					_nConnectors = ((ICollection)associatedDeviceObject.Connectors).Count;
					_guidObject = ((IObject)associatedDeviceObject).MetaObject.ObjectGuid;
				}
			}
			if (_modelChannels == null)
			{
				_modelChannels = new ParameterTreeTableModel(GetParameterSetProvider(), ParameterTreeTableModelView.IOMappingsOnline, null);
			}
			else
			{
				_modelChannels.Refill();
			}
			if (_modelInstances == null)
			{
				_modelInstances = new FbInstanceTreeTableModel(new FbInstanceProvider(this));
				_modelInstances.LocalizationActive = _bLocalizationActive;
			}
			if (_page == null)
			{
				_page = new IoMappingEditorPage(this);
			}
			if (_objectsPage == null)
			{
				_objectsPage = new IECObjectsPage(this);
			}
			Page.ChannelPanelVisible = ((AbstractTreeTableModel)_modelChannels).Sentinel.ChildCount > 0;
			_modelChannels.UndoManager = _page.UndoMgr;
			_modelInstances.UndoManager = _page.UndoMgr;
		}

		public void Save(bool bCommit)
		{
			if (!bCommit || _page == null)
			{
				return;
			}
			TreeTableView mappingTreeTableViewCellValue = _page.MappingTreeTableViewCellValue;
			if (mappingTreeTableViewCellValue != null && mappingTreeTableViewCellValue.FocusedNode != null)
			{
				TreeTableViewNode focusedNode = mappingTreeTableViewCellValue.FocusedNode;
				int num = default(int);
				if (focusedNode != null && focusedNode.IsFocused(out num) && focusedNode.IsEditing(num) && num != 3)
				{
					focusedNode.EndEdit(num, false);
				}
			}
		}

		internal abstract IConnectorEditorFrame GetFrame();

		protected void OnModuleStatusChanged(object sender, IModuleStatusEventArgs e)
		{
			if (e.ObjectGuid == _guidObject && _page != null)
			{
				_page.OnModuleStatusChanged(sender, e);
			}
		}

		protected void OnOnlineStateChanged(object sender, EventArgs e)
		{
			if (_page != null)
			{
				_page.OnOnlineStateChanged(sender, e);
			}
		}

		internal bool TrySelect(long lPosition, int nOffset, int nLength)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			if (_page != null && _page.TrySelect(lPosition, nOffset, nLength))
			{
				return true;
			}
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)((TreeTableView)_page.MappingTreeTableView).SelectedNodes)
			{
				item.Selected=(false);
			}
			bool flag = false;
			if ((lPosition & 0x80000000u) != 0L)
			{
				flag = true;
				lPosition &= 0x7FFFFFFF;
			}
			DataElementNode nodeByPosition = _modelChannels.GetNodeByPosition(lPosition);
			if (nodeByPosition == null)
			{
				return false;
			}
			_page.MappingTreeTableView.RestoreExpandedNodes();
			TreeTableViewNode viewNode = ((TreeTableView)_page.MappingTreeTableView).GetViewNode((ITreeTableNode)(object)nodeByPosition);
			if (viewNode == null)
			{
				bool isInRestore = _page.MappingTreeTableView.IsInRestore;
				try
				{
					_page.MappingTreeTableView.IsInRestore = true;
					((TreeTableView)_page.MappingTreeTableView).ExpandAll();
					viewNode = ((TreeTableView)_page.MappingTreeTableView).GetViewNode((ITreeTableNode)(object)nodeByPosition);
					_page.MappingTreeTableView.RestoreExpandedNodes();
				}
				finally
				{
					_page.MappingTreeTableView.IsInRestore = isInRestore;
				}
			}
			if (viewNode != null)
			{
				((Control)(object)_page.MappingTreeTableView).Focus();
				int num = (flag ? _modelChannels.GetIndexOfColumn(3) : _modelChannels.GetIndexOfColumn(0));
				viewNode.EnsureVisible(num);
				viewNode.Selected=(true);
				viewNode.Focus(num);
			}
			return true;
		}

		public IEditorPage Select(long lPosition, int nOffset, int nLength)
		{
			if (_page != null && TrySelect(lPosition, nOffset, nLength))
			{
				return (IEditorPage)(object)_page;
			}
			if (_objectsPage != null && _objectsPage.TrySelect(lPosition, nOffset, nLength))
			{
				return (IEditorPage)(object)_objectsPage;
			}
			return null;
		}

		public int ComparePositions(long nPosition1, int nOffset1, long nPosition2, int nOffset2)
		{
			DataElementNode nodeByPosition = ChannelsModel.GetNodeByPosition(nPosition1);
			DataElementNode nodeByPosition2 = ChannelsModel.GetNodeByPosition(nPosition2);
			if (nodeByPosition == null)
			{
				if (nodeByPosition2 != null)
				{
					return -11;
				}
				return 0;
			}
			if (nodeByPosition2 == null)
			{
				return 1;
			}
			return ChannelsModel.CompareNodesByIndex(nodeByPosition, nodeByPosition2);
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			if (_page != null && _page != null)
			{
				return ((IEditorBasedFindReplace)_page).UndoableReplace(nPosition, nLength, stReplacement);
			}
			return false;
		}

		public void SetLocalizedObject(IMetaObject obj, bool bLocalizationActive)
		{
			_bLocalizationActive = bLocalizationActive;
			if (_page != null)
			{
				_page.SetLocalizedObject(obj, bLocalizationActive);
			}
		}

		public bool IsComment(long nPositionCombined, string stText)
		{
			if (_page != null && _page.IsComment(nPositionCombined, stText))
			{
				return true;
			}
			return false;
		}
	}
}
