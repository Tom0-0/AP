using System;
using System.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;

namespace _3S.CoDeSys.DeviceEditor
{
	internal abstract class ParameterEditor
	{
		private ParameterTreeTableModel _model;

		private ParameterEditorPage _page;

		protected HideParameterDelegate _paramFilter;

		private bool _bOpenView;

		public HideParameterDelegate ParameterFilter => _paramFilter;

		internal abstract string PageName { get; }

		public IEditorPage[] Pages
		{
			get
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Expected O, but got Unknown
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				IEditorPage val = null;
				if (_bOpenView)
				{
					val = (IEditorPage)(object)EditorPage;
				}
				else
				{
					IParameterSetProvider parameterSetProvider = GetParameterSetProvider();
					if (parameterSetProvider != null)
					{
						foreach (IParameter item in (IEnumerable)parameterSetProvider.GetParameterSet(bToModify: false))
						{
							IParameter val2 = item;
							if ((_paramFilter == null || !_paramFilter.Invoke((int)val2.Id, (string[])null)) && (int)val2.ChannelType == 0 && (int)val2.GetAccessRight(false) != 0)
							{
								val = (IEditorPage)(object)_page;
							}
						}
					}
				}
				if (val != null)
				{
					return (IEditorPage[])(object)new IEditorPage[1] { val };
				}
				return (IEditorPage[])(object)new IEditorPage[0];
			}
		}

		protected ParameterEditorPage EditorPage
		{
			get
			{
				if (_model != null && ((AbstractTreeTableModel)_model).Sentinel.HasChildren)
				{
					if (_page == null)
					{
						_page = new ParameterEditorPage(this);
						_model.UndoManager = _page.UndoMgr;
					}
				}
				else
				{
					_page = null;
				}
				return _page;
			}
		}

		internal ParameterTreeTableModel Model => _model;

		internal bool LogicalDeviceIsMapped
		{
			get
			{
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				if (IOMappingEditor.EnableLogicalDevices)
				{
					IDeviceObject deviceObject = GetDeviceObject(bToModify: false);
					if (deviceObject != null && deviceObject is ILogicalDevice)
					{
						ILogicalDevice val = (ILogicalDevice)(object)((deviceObject is ILogicalDevice) ? deviceObject : null);
						if (val.IsPhysical)
						{
							if (val is ILogicalDevice2 && ((ILogicalDevice2)((val is ILogicalDevice2) ? val : null)).MappingPossible)
							{
								return false;
							}
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

		internal abstract IHasOnlineMode HasOnlineMode { get; }

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

		internal abstract IParameterSetProvider GetParameterSetProvider();

		internal abstract IDeviceObject GetHost(bool bToModify);

		internal abstract IDeviceObject GetDeviceObject(bool bToModify);

		protected abstract void OnOnlineStateChanged(object sender, EventArgs e);

		protected abstract void OnParameterAdded(object sender, ParameterEventArgs e);

		protected abstract void OnParameterRemoved(object sender, ParameterEventArgs e);

		protected abstract void OnParameterChanged(object sender, ParameterChangedEventArgs e);

		protected abstract void OnParameterSectionChanged(object sender, ParameterSectionChangedEventArgs e);

		protected ParameterEditor(HideParameterDelegate paramFilter)
		{
			_paramFilter = paramFilter;
		}

		public void Reload()
		{
			if (this is ConnectorParameterEditor && !((this as ConnectorParameterEditor).ConnectorEditorFrame as DeviceEditor).OpenView)
			{
				if (_page == null)
				{
					_page = new ParameterEditorPage(this);
				}
			}
			else if (this is DeviceParameterEditor && !((this as DeviceParameterEditor)._deviceEditor as DeviceEditor).OpenView)
			{
				if (_page == null)
				{
					_page = new ParameterEditorPage(this);
				}
			}
			else
			{
				_bOpenView = true;
				_page = null;
				_model = new ParameterTreeTableModel(GetParameterSetProvider(), ParameterTreeTableModelView.Parameters, _paramFilter);
			}
		}

		public void Save(bool bCommit)
		{
			if (bCommit && _page != null)
			{
				_page.CheckFocuseNode(bCancel: false);
			}
		}

		public int ComparePositions(long nPosition1, int nOffset1, long nPosition2, int nOffset2)
		{
			DataElementNode nodeByPosition = Model.GetNodeByPosition(nPosition1);
			DataElementNode nodeByPosition2 = Model.GetNodeByPosition(nPosition2);
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
			return Model.CompareNodesByIndex(nodeByPosition, nodeByPosition2);
		}

		public IEditorPage Select(long lPosition, int nOffset, int nLength)
		{
			if (EditorPage != null && EditorPage.TrySelect(lPosition, nOffset, nLength))
			{
				return (IEditorPage)(object)EditorPage;
			}
			return null;
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			if (EditorPage != null)
			{
				return EditorPage.UndoableReplace(nPosition, nLength, stReplacement);
			}
			return false;
		}

		internal abstract IDeviceObject GetHost();
	}
}
