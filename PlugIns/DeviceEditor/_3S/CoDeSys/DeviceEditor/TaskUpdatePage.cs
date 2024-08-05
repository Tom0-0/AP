#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_task_deployment.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/task_deployment.htm")]
	public class TaskUpdatePage : UserControl, IEditorPage3, IEditorPage2, IEditorPage, IEditorPageAppearance2, IEditorPageAppearance
	{
		public class ParameterByDevDescComparer : IComparer
		{
			private static ParameterByDevDescComparer _instance;

			internal static ParameterByDevDescComparer Instance
			{
				get
				{
					if (_instance == null)
					{
						_instance = new ParameterByDevDescComparer();
					}
					return _instance;
				}
			}

			private ParameterByDevDescComparer()
			{
			}

			public int Compare(object x, object y)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected O, but got Unknown
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Expected O, but got Unknown
				IParameter6 val = (IParameter6)x;
				IParameter6 val2 = (IParameter6)y;
				if (val.IndexInDevDesc == -1 || val2.IndexInDevDesc == -1)
				{
					return ((IParameter)val).Id.CompareTo(((IParameter)val2).Id);
				}
				return val.IndexInDevDesc.CompareTo(val2.IndexInDevDesc);
			}
		}

		public static readonly Guid GUID_EDITSELECTALL = new Guid("{1C36CA5E-E26D-4edc-9AB7-C7D87690C328}");

		private DeviceEditor _editor;

		private TaskUpdateModel _model;

		private LList<TaskInfo> _liTaskInfos = new LList<TaskInfo>();

		private LDictionary<Guid, LDictionary<long, LList<IIOTaskUsage>>> _dictTasks;

		private int _iShowOnlyColumn;

		private IContainer components;

		private Label _lblTitel;

		private TreetableViewWithColumnStorage _treeTableView;

		private Label label1;

		private PictureBox pictureBox1;

		public string PageName => Strings.TaskUpdatePageName;

		public Icon Icon => null;

		public Control Control => this;

		public bool HasOnlineMode => true;

		public string PageIdentifier => "TaskUpdatePage";

		public DeviceEditor DeviceEditor
		{
			get
			{
				return _editor;
			}
			set
			{
				_editor = value;
			}
		}

		public TaskUpdatePage()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CodeChanged-=(new CompileEventHandler(CodeChanged));
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AfterCompile-=(new CompileEventHandler(CodeChanged));
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).TaskConfigChanged-=(new CompileEventHandler(CodeChanged));
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AfterLazyLibraryLoad-=((EventHandler)LazyLoad);
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			base.OnHandleCreated(e);
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CodeChanged+=(new CompileEventHandler(CodeChanged));
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AfterCompile+=(new CompileEventHandler(CodeChanged));
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).TaskConfigChanged+=(new CompileEventHandler(CodeChanged));
			((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AfterLazyLibraryLoad+=((EventHandler)LazyLoad);
			Reload();
		}

		public void CodeChanged(object sender, CompileEventArgs e)
		{
			Reload();
		}

		public void LazyLoad(object sender, EventArgs e)
		{
			Reload();
		}

		public void Reload()
		{
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_liTaskInfos.Clear();
				_dictTasks = new LDictionary<Guid, LDictionary<long, LList<IIOTaskUsage>>>();
				IDeviceObject deviceObject = _editor.GetDeviceObject(bToModify: false);
				IDeviceObject10 val = (IDeviceObject10)(object)((deviceObject is IDeviceObject10) ? deviceObject : null);
				if (val != null && ((IDeviceObject2)val).DriverInfo is IDriverInfo2)
				{
					LDictionary<long, LList<IIOTaskUsage>> val2 = default(LDictionary<long, LList<IIOTaskUsage>>);
					LList<IIOTaskUsage> val3 = default(LList<IIOTaskUsage>);
					foreach (Guid application in DeviceHelper.GetApplications(((IObject)val).MetaObject, bWithHidden: false))
					{
						if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_editor.ProjectHandle, application))
						{
							continue;
						}
						ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(application);
						if (compileContext == null)
						{
							continue;
						}
						try
						{
							List<IIOTaskUsage> taskMappings = val.GetTaskMappings(application);
							if (taskMappings == null)
							{
								continue;
							}
							foreach (IIOTaskUsage item in taskMappings)
							{
								if (!_dictTasks.TryGetValue(item.ObjectGuid, out val2))
								{
									val2 = new LDictionary<long, LList<IIOTaskUsage>>();
									_dictTasks[item.ObjectGuid]= val2;
								}
								if (!val2.TryGetValue(item.Parameter.Id, out val3))
								{
									val3 = new LList<IIOTaskUsage>();
									val3.Add(item);
									val2.Add(item.Parameter.Id, val3);
								}
								val3.Add(item);
							}
							ITaskInfo[] allTasks = compileContext.AllTasks;
							if (allTasks == null)
							{
								continue;
							}
							ITaskInfo[] array = allTasks;
							foreach (ITaskInfo val4 in array)
							{
								if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_editor.ProjectHandle, val4.TaskGuid))
								{
									IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_editor.ProjectHandle, val4.TaskGuid);
									if (objectToRead != null && objectToRead.Object is ITaskObject)
									{
										int.TryParse(((ITaskObject)objectToRead.Object).Priority, out var result);
										TaskInfo taskInfo = new TaskInfo();
										taskInfo.iPriority = result;
										taskInfo.taskInfo = val4;
										_liTaskInfos.Add(taskInfo);
									}
								}
							}
							_liTaskInfos.Sort((Comparison<TaskInfo>)CompareTask);
						}
						catch
						{
						}
					}
				}
				_model = new TaskUpdateModel((TreeTableView)(object)_treeTableView, _liTaskInfos);
				((TreeTableView)_treeTableView).Model=((ITreeTableModel)(object)_model);
				_treeTableView.ObjectGuid = _editor.ObjectGuid;
				_treeTableView.IdentificationGuid = new Guid("{5DCA182F-525A-4FB5-A4FE-C361BDFCBAD9}");
				FillModel();
			}
			catch
			{
			}
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITSELECTALL)
			{
				return true;
			}
			return false;
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITSELECTALL)
			{
				for (TreeTableViewNode val = ((TreeTableView)_treeTableView).TopNode; val != null; val = val.NextVisibleNode)
				{
					val.Selected=(true);
				}
			}
		}

		internal IDataElement SearchMapping(IDataElement dataElement, IIOTaskUsage taskUsage, LDictionary<IDataElement, string> dictAddresses)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			if (dataElement != null && dataElement.IoMapping != null)
			{
				string iecAddress = default(string);
				if (!dictAddresses.TryGetValue(dataElement, out iecAddress))
				{
					iecAddress = dataElement.IoMapping.IecAddress;
					dictAddresses[dataElement]= iecAddress;
				}
				if (taskUsage.MappedAddress != iecAddress && dataElement.HasSubElements)
				{
					foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
					{
						IDataElement val = item;
						if (SearchMapping(val, taskUsage, dictAddresses) != null)
						{
							return val;
						}
					}
				}
				else if (taskUsage.MappedAddress == iecAddress)
				{
					return dataElement;
				}
			}
			return null;
		}

		private void FillDataElements(IMetaObjectStub mos, IDataElement dataElement, IParameter parameter, ITaskNode parentNode, Guid parentBusTaskGuid, LList<Guid> liTaskGuidsParent, Guid filterTaskGuid, LDictionary<IDataElement, string> dictAddresses)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Expected O, but got Unknown
			bool flag = false;
			Guid guid = parentBusTaskGuid;
			LList<Guid> val = new LList<Guid>();
			foreach (IVariableMapping item in (IEnumerable)dataElement.IoMapping.VariableMappings)
			{
				if (!item.CreateVariable)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				val.AddRange((IEnumerable<Guid>)liTaskGuidsParent);
			}
			LDictionary<long, LList<IIOTaskUsage>> val2 = default(LDictionary<long, LList<IIOTaskUsage>>);
			if (_dictTasks.TryGetValue(mos.ObjectGuid, out val2))
			{
				long id = parameter.Id;
				LList<IIOTaskUsage> val3 = default(LList<IIOTaskUsage>);
				if (val2.TryGetValue(id, out val3))
				{
					string iecAddress = default(string);
					foreach (IIOTaskUsage item2 in val3)
					{
						bool flag2 = false;
						if (item2 is IIOTaskUsage2 && dataElement is IDataElement4)
						{
							ITaskMapping taskmapping = ((IIOTaskUsage2)((item2 is IIOTaskUsage2) ? item2 : null)).Taskmapping;
							int parameterBitOffset = taskmapping.ParameterBitOffset;
							int bitSize = taskmapping.BitSize;
							IDataElement obj = ((dataElement is IDataElement4) ? dataElement : null);
							long bitOffset = ((IDataElement4)obj).GetBitOffset();
							long bitSize2 = obj.GetBitSize();
							if (parameterBitOffset <= bitOffset && bitSize + parameterBitOffset >= bitSize2 + bitOffset)
							{
								flag2 = true;
							}
						}
						if (!dictAddresses.TryGetValue(dataElement, out iecAddress))
						{
							iecAddress = dataElement.IoMapping.IecAddress;
							dictAddresses[dataElement]= iecAddress;
						}
						if (!flag2 && dataElement != item2.DataElement && !(iecAddress == item2.MappedAddress))
						{
							continue;
						}
						bool flag3 = true;
						if (dataElement.HasSubElements && ((IDataElement2)((dataElement is IDataElement2) ? dataElement : null)).HasBaseType && iecAddress != item2.MappedAddress && SearchMapping(dataElement, item2, dictAddresses) != null)
						{
							flag3 = false;
						}
						if (!flag3)
						{
							continue;
						}
						if (item2.BusTask != null)
						{
							guid = item2.BusTask.TaskGuid;
						}
						foreach (ITaskInfo usedTask in item2.UsedTasks)
						{
							if (!val.Contains(usedTask.TaskGuid))
							{
								val.Add(usedTask.TaskGuid);
							}
						}
					}
				}
			}
			ITaskNode parentNode2 = parentNode;
			if (_iShowOnlyColumn == 0 || val.Contains(filterTaskGuid))
			{
				parentNode2 = _model.AddNode(mos.ObjectGuid, guid, val, dataElement, parameter.ChannelType, parentNode);
				if (flag)
				{
					val.Clear();
					val.AddRange((IEnumerable<Guid>)liTaskGuidsParent);
					guid = parentBusTaskGuid;
				}
			}
			if (!dataElement.HasSubElements)
			{
				return;
			}
			foreach (IDataElement item3 in (IEnumerable)dataElement.SubElements)
			{
				IDataElement dataElement2 = item3;
				FillDataElements(mos, dataElement2, parameter, parentNode2, guid, val, filterTaskGuid, dictAddresses);
			}
		}

		private void FillIoProviders(IIoProvider ioProvider, LDictionary<IMetaObjectStub, DescriptionNode> rootNodes)
		{
			bool flag = false;
			foreach (IParameter item in (IEnumerable)ioProvider.ParameterSet)
			{
				if ((int)item.ChannelType != 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_editor.ProjectHandle, ioProvider.GetMetaObject().ObjectGuid);
				DescriptionNode descriptionNode = default(DescriptionNode);
				if (!rootNodes.TryGetValue(metaObjectStub, out descriptionNode))
				{
					Icon smallIcon = ((IHasDynamicIcons)new DeviceEditorFactory()).GetSmallIcon(metaObjectStub);
					descriptionNode = _model.AddRootNode(metaObjectStub.Name, smallIcon);
					rootNodes.Add(metaObjectStub, descriptionNode);
				}
				Guid filterTaskGuid = Guid.Empty;
				if (_iShowOnlyColumn > 0)
				{
					TaskInfo columnTaskInfo = _model.GetColumnTaskInfo(_iShowOnlyColumn);
					if (columnTaskInfo != null)
					{
						filterTaskGuid = columnTaskInfo.taskInfo.TaskGuid;
					}
				}
				foreach (IParameter item2 in SortedParameterSet(ioProvider))
				{
					IParameter val = item2;
					if ((int)val.ChannelType != 0)
					{
						FillDataElements(metaObjectStub, (IDataElement)(object)val, val, descriptionNode, Guid.Empty, new LList<Guid>(), filterTaskGuid, new LDictionary<IDataElement, string>());
					}
				}
			}
			IIoProvider[] children = ioProvider.Children;
			foreach (IIoProvider ioProvider2 in children)
			{
				FillIoProviders(ioProvider2, rootNodes);
			}
		}

		internal static ICollection SortedParameterSet(IIoProvider ioprovider)
		{
			bool flag = false;
			if (typeof(IDeviceObject9).IsAssignableFrom(((object)ioprovider).GetType()))
			{
				flag = ((IDeviceObject9)((ioprovider is IDeviceObject9) ? ioprovider : null)).ShowParamsInDevDescOrder;
			}
			else
			{
				IMetaObject metaObject = ioprovider.GetMetaObject();
				if (metaObject != null && typeof(IDeviceObject9).IsAssignableFrom(((object)metaObject.Object).GetType()))
				{
					IObject @object = metaObject.Object;
					flag = ((IDeviceObject9)((@object is IDeviceObject9) ? @object : null)).ShowParamsInDevDescOrder;
				}
			}
			if (flag)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange((ICollection)ioprovider.ParameterSet);
				arrayList.Sort(ParameterByDevDescComparer.Instance);
				return arrayList;
			}
			return (ICollection)ioprovider.ParameterSet;
		}

		private void FillModel()
		{
			try
			{
				((TreeTableView)_treeTableView).BeginUpdate();
				_model.Clear();
				if (_liTaskInfos == null || _liTaskInfos.Count == 0)
				{
					_model.AddRootNode(Strings.TaskUpdateCompileNecessary, null);
					return;
				}
				LDictionary<IMetaObjectStub, DescriptionNode> rootNodes = new LDictionary<IMetaObjectStub, DescriptionNode>();
				IDeviceObject deviceObject = _editor.GetDeviceObject(bToModify: false);
				IIoProvider val = (IIoProvider)(object)((deviceObject is IIoProvider) ? deviceObject : null);
				if (val != null)
				{
					FillIoProviders(val, rootNodes);
				}
				bool isInRestore = _treeTableView.IsInRestore;
				try
				{
					_treeTableView.IsInRestore = true;
					((TreeTableView)_treeTableView).ExpandAll();
				}
				finally
				{
					_treeTableView.IsInRestore = isInRestore;
					_treeTableView.StoreExpandedNodes();
				}
			}
			catch
			{
			}
			finally
			{
				AdjustColumnWidths();
				((TreeTableView)_treeTableView).EndUpdate();
			}
		}

		private static int CompareTask(TaskInfo x, TaskInfo y)
		{
			if (x == null || y == null)
			{
				return 0;
			}
			return x.iPriority.CompareTo(y.iPriority);
		}

		private void AdjustColumnWidths()
		{
			for (int i = 0; i < ((TreeTableView)_treeTableView).Columns.Count; i++)
			{
				_treeTableView.AdjustColumnWidth(i, bConsiderHeaderText: true);
			}
		}

		private void _treeTableView_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				if (((TreeTableViewNodeCollection)((TreeTableView)_treeTableView).SelectedNodes).Count == 0)
				{
					return;
				}
				TreeTableViewNode val = ((TreeTableViewNodeCollection)((TreeTableView)_treeTableView).SelectedNodes)[0];
				TaskUpdateNode taskUpdateNode = ((TreeTableView)_treeTableView).GetModelNode(val) as TaskUpdateNode;
				if (taskUpdateNode != null)
				{
					int projectHandle = _editor.ProjectHandle;
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, taskUpdateNode.ObjectGuid);
					Debug.Assert(metaObjectStub != null);
					Type objectType = metaObjectStub.ObjectType;
					Type[] embeddedObjectTypes = metaObjectStub.EmbeddedObjectTypes;
					Guid defaultEditorViewFactory = ((IEngine)APEnvironment.Engine).Frame.ViewFactoryManager.GetDefaultEditorViewFactory(objectType, embeddedObjectTypes);
					IEditorView val2 = ((IEngine)APEnvironment.Engine).Frame.OpenEditorView(metaObjectStub, defaultEditorViewFactory, (string)null);
					if (val2 != null)
					{
						IDataElement dataElement = taskUpdateNode.DataElement;
						long languageModelPositionId = ((IDataElement2)((dataElement is IDataElement2) ? dataElement : null)).LanguageModelPositionId;
						val2.Select(languageModelPositionId, 0);
					}
				}
			}
			catch
			{
			}
		}

		private void _treeTableView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			_iShowOnlyColumn = e.Column;
			FillModel();
		}

		public void Mark(long nPosition, int nLength, object tag)
		{
		}

		public void UnmarkAll(object tag)
		{
		}

		public void Select(long nPosition, int nLength)
		{
		}

		public void GetSelection(out long lPosition, out short sOffset, out int nLength)
		{
			lPosition = 0L;
			sOffset = 0;
			nLength = 0;
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.TaskUpdatePage));
			_lblTitel = new System.Windows.Forms.Label();
			_treeTableView = new _3S.CoDeSys.DeviceEditor.TreetableViewWithColumnStorage();
			label1 = new System.Windows.Forms.Label();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			resources.ApplyResources(_lblTitel, "_lblTitel");
			_lblTitel.Name = "_lblTitel";
			((TreeTableView)_treeTableView).AllowColumnReorder=(false);
			resources.ApplyResources(_treeTableView, "_treeTableView");
			((TreeTableView)_treeTableView).AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_treeTableView).BackColor = System.Drawing.SystemColors.Window;
			((TreeTableView)_treeTableView).BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			((TreeTableView)_treeTableView).DoNotShrinkColumnsAutomatically=(false);
			((TreeTableView)_treeTableView).ForceFocusOnClick=(false);
			((TreeTableView)_treeTableView).GridLines=(true);
			((TreeTableView)_treeTableView).HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Clickable);
			((TreeTableView)_treeTableView).HideSelection=(false);
			_treeTableView.IdentificationGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
			((TreeTableView)_treeTableView).ImmediateEdit=(false);
			((TreeTableView)_treeTableView).Indent=(20);
			((TreeTableView)_treeTableView).KeepColumnWidthsAdjusted=(false);
			((TreeTableView)_treeTableView).Model=((ITreeTableModel)null);
			((TreeTableView)_treeTableView).MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_treeTableView).Name = "_treeTableView";
			((TreeTableView)_treeTableView).NoSearchStrings=(false);
			_treeTableView.ObjectGuid = new System.Guid("00000000-0000-0000-0000-000000000000");
			((TreeTableView)_treeTableView).OnlyWhenFocused=(false);
			((TreeTableView)_treeTableView).OpenEditOnDblClk=(true);
			((TreeTableView)_treeTableView).ReadOnly=(false);
			((TreeTableView)_treeTableView).Scrollable=(true);
			((TreeTableView)_treeTableView).ShowLines=(true);
			((TreeTableView)_treeTableView).ShowPlusMinus=(true);
			((TreeTableView)_treeTableView).ShowRootLines=(true);
			((TreeTableView)_treeTableView).ToggleOnDblClk=(false);
			((TreeTableView)_treeTableView).ColumnClick+=(new System.Windows.Forms.ColumnClickEventHandler(_treeTableView_ColumnClick));
			((System.Windows.Forms.Control)(object)_treeTableView).DoubleClick += new System.EventHandler(_treeTableView_DoubleClick);
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			resources.ApplyResources(pictureBox1, "pictureBox1");
			pictureBox1.Image = _3S.CoDeSys.DeviceEditor.Strings.Busycle;
			pictureBox1.Name = "pictureBox1";
			pictureBox1.TabStop = false;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			base.Controls.Add(pictureBox1);
			base.Controls.Add(label1);
			base.Controls.Add((System.Windows.Forms.Control)(object)_treeTableView);
			base.Controls.Add(_lblTitel);
			base.Name = "TaskUpdatePage";
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
