using System;
using System.Drawing;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.NavigatorControl;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	[TypeGuid("{778BE21D-A863-415B-A19B-CB8C432980DB}")]
	internal class SimpleMappingEditorCommand : IStandardCommand, ICommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "device", "simplemapping" };

		public static readonly Guid GUID_DEVICECMDCATEGORY = new Guid("61ACA6D6-F298-47be-972E-99601E7E9410");

		internal int _iSafetyPLCCategory = 4098;

		public Guid Category => GUID_DEVICECMDCATEGORY;

		public string Name => Strings.SimpleMappingName;

		public string Description => Strings.SimpleMappingDescription;

		public string ToolTipText => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public bool Enabled
		{
			get
			{
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject == null || !IsProjectLoaded(primaryProject))
				{
					return false;
				}
				ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
				if (selectedSVNodes.Length != 0)
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(selectedSVNodes[0].ProjectHandle, selectedSVNodes[0].ObjectGuid);
					if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						return false;
					}
					IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid).Object;
					IDeviceObject5 val = (IDeviceObject5)(object)((@object is IDeviceObject5) ? @object : null);
					if (val != null)
					{
						bool flag = false;
						if (val.DeviceIdentificationNoSimulation.Type == _iSafetyPLCCategory)
						{
							flag = true;
						}
						if (((IDeviceObject)val).DeviceInfo.Categories.Length != 0)
						{
							int[] categories = ((IDeviceObject)val).DeviceInfo.Categories;
							for (int i = 0; i < categories.Length; i++)
							{
								if (categories[i] == _iSafetyPLCCategory)
								{
									flag = true;
								}
							}
						}
						if (!string.IsNullOrEmpty(((IDeviceObject)val).DeviceInfo.Custom))
						{
							try
							{
								XmlDocument xmlDocument = new XmlDocument();
								xmlDocument.LoadXml(((IDeviceObject)val).DeviceInfo.Custom);
								if (xmlDocument != null && xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.Name == "safety_config")
								{
									flag = true;
								}
							}
							catch
							{
							}
						}
						if (flag)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public string[] BatchCommand => BATCH_COMMAND;

		public string[] CreateBatchArguments()
		{
			return new string[0];
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (bContextMenu && Enabled)
			{
				return ((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl;
			}
			return false;
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (arguments.Length != 0)
			{
				throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 0);
			}
			if (((IEngine)APEnvironment.Engine).Frame == null)
			{
				throw new BatchInteractiveException(BatchCommand);
			}
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return;
			}
			IView val = ((IEngine)APEnvironment.Engine).Frame.OpenView(MappingEditorViewFactory.TypeGuid, (string)null);
			if (val != null)
			{
				ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
				if (selectedSVNodes.Length != 0)
				{
					(val as SimpleMappingView).StartNode(selectedSVNodes[0]);
				}
				else
				{
					(val as SimpleMappingView).StartNode(null);
				}
				((IEngine)APEnvironment.Engine).Frame.ActiveView=(val);
			}
		}

		public static bool IsProjectLoaded(IProject proj)
		{
			if (proj == null)
			{
				return false;
			}
			int num = default(int);
			return ((IObjectManager)APEnvironment.ObjectMgr).IsLoadProjectFinished(proj.Handle, out num);
		}
	}
}
