#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MappingVariableEditor : ITreeTableViewEditor
	{
		private int _nProjectHandle;

		private Guid _guidHost;

		private DataElementNode _dataNodeEdit;

		internal IInputAssistantCategory[] InputAssistantCategories
		{
			get
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Expected O, but got Unknown
				IInputAssistantCategory[] array = new IInputAssistantCategory[1];
				IVariablesInputAssistantCategory val = APEnvironment.CreateVariablesInputAssistantCategory();
				val.CallbackVariable=(new AcceptVariableEventHandler(OnInputAssistantCallback));
				array[0] = (IInputAssistantCategory)val;
				return (IInputAssistantCategory[])(object)array;
			}
		}

		private bool OnInputAssistantCallback(IPreCompileContext pcc, ISignature signature, IVariable variable, ISignature signTopLevel, ISignature signCurrent)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			if (_dataNodeEdit != null && (int)_dataNodeEdit.Parameter.ChannelType == 1 && variable.GetFlag((VarFlag)64))
			{
				return false;
			}
			if (variable.GetFlag((VarFlag)2097152) || variable.GetFlag((VarFlag)128))
			{
				return false;
			}
			if (variable.HasAttribute(CompileAttributes.ATTRIBUTE_PROPERTY))
			{
				return false;
			}
			return true;
		}

		internal MappingVariableEditor()
		{
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Invalid comparison between Unknown and I4
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Expected O, but got Unknown
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Expected O, but got Unknown
			if (cImmediate == '+' || cImmediate == '-' || cImmediate == '*' || cImmediate == '/')
			{
				return null;
			}
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			TreeTableView view = node.View;
			if (view == null)
			{
				throw new ArgumentException("The node is not associated with a view.");
			}
			object obj = node.CellValues[nColumnIndex];
			IconLabelTreeTableViewCellData val = (IconLabelTreeTableViewCellData)((obj is IconLabelTreeTableViewCellData) ? obj : null);
			string text = null;
			if (val != null)
			{
				text = val.Label as string;
			}
			MappingVariableEditorPanel mappingVariableEditorPanel = new MappingVariableEditorPanel();
			mappingVariableEditorPanel.CellData = val;
			if (view.Columns.Count > 0)
			{
				mappingVariableEditorPanel.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			}
			mappingVariableEditorPanel.GotFocus += OnPanelGotFocus;
			Button button = new Button();
			button.FlatStyle = FlatStyle.System;
			button.Text = "...";
			button.Size = new Size(24, mappingVariableEditorPanel.Bounds.Height);
			button.Location = new Point(mappingVariableEditorPanel.Bounds.Width - button.Size.Width, 0);
			button.Font = ((Control)(object)view).Font;
			button.Click += OnButtonClick;
			DataElementNode dataElementNode = (_dataNodeEdit = node.View.GetModelNode(node) as DataElementNode);
			IDeviceObject host = dataElementNode.ParameterSetProvider.GetHost();
			_nProjectHandle = -1;
			_guidHost = Guid.Empty;
			if (host != null)
			{
				_nProjectHandle = ((IObject)host).MetaObject.ProjectHandle;
				_guidHost = ((IObject)host).MetaObject.ObjectGuid;
			}
			Guid inputAssistantGuid = Guid.Empty;
			try
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, _guidHost);
				if (objectToRead != null && objectToRead.Object is IDeviceObject2)
				{
					IObject @object = objectToRead.Object;
					inputAssistantGuid = ((IDriverInfo2)((IDeviceObject2)((@object is IDeviceObject2) ? @object : null)).DriverInfo).IoApplication;
				}
			}
			catch
			{
			}
			ISingleLineIECTextEditor5 val2 = APEnvironment.CreateSingleLineIECTextEditor();
			Debug.Assert(val2 != null);
			((ISingleLineIECTextEditor)val2).InputAssistantCategories=(InputAssistantCategories);
			val2.InputAssistantGuid=(inputAssistantGuid);
			((ISingleLineIECTextEditor)val2).BackColor=(((Control)(object)view).BackColor);
			((ISingleLineIECTextEditor)val2).Bounds=(new Rectangle(0, 0, mappingVariableEditorPanel.Bounds.Width - button.Bounds.Width, mappingVariableEditorPanel.Bounds.Height));
			((ISingleLineIECTextEditor)val2).Font=(((Control)(object)view).Font);
			((ISingleLineIECTextEditor)val2).ForeColor=(((Control)(object)view).ForeColor);
			((ISingleLineIECTextEditor)val2).Text=((cImmediate != 0) ? cImmediate.ToString() : ((text != null) ? text : string.Empty));
			bool flag = dataElementNode.PlcNode.TreeModel.GetIndexOfColumn(1) != -1 && ((view.Columns.Count <= 0) ? (!dataElementNode.IsEditable(1)) : (!node.View.IsColumnEditable(1)));
			if (dataElementNode != null)
			{
				flag |= (int)dataElementNode.Parameter.ChannelType == 3;
			}
			if (dataElementNode.Parameter is IParameter9)
			{
				bool num = flag;
				IParameter parameter = dataElementNode.Parameter;
				flag = num | ((IParameter9)((parameter is IParameter9) ? parameter : null)).MapOnlyNew;
			}
			if (!DataElementNode.CheckUnionMapping(dataElementNode))
			{
				flag = true;
			}
			if (!flag)
			{
				((ISingleLineIECTextEditor)val2).SetIntelliSenseItemsCallback(new IntelliSenseItemsCallback(EditorIntelliSenseItemsCallback));
			}
			else
			{
				((ISingleLineIECTextEditor)val2).SetUserInputCallback(new UserInputCallback(EditorUserInputCallback));
			}
			if (cImmediate == '\0')
			{
				((ISingleLineIECTextEditor)val2).SelectAll();
			}
			else
			{
				((ISingleLineIECTextEditor)val2).Select(1, 0);
			}
			mappingVariableEditorPanel.Tag = val2;
			button.Tag = val2;
			mappingVariableEditorPanel.Controls.Add(((ISingleLineIECTextEditor)val2).Control);
			if (!flag)
			{
				mappingVariableEditorPanel.Controls.Add(button);
			}
			return mappingVariableEditorPanel;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Expected O, but got Unknown
			_dataNodeEdit = null;
			MappingVariableEditorPanel mappingVariableEditorPanel = control as MappingVariableEditorPanel;
			object tag = control.Tag;
			ISingleLineIECTextEditor val = (ISingleLineIECTextEditor)((tag is ISingleLineIECTextEditor) ? tag : null);
			if (val != null && mappingVariableEditorPanel != null && mappingVariableEditorPanel.CellData != null)
			{
				string text = val.Text;
				if (!string.IsNullOrEmpty(text) && text.Contains("."))
				{
					if (text.StartsWith("."))
					{
						text = text.Substring(1);
					}
					string[] allApplications = GetAllApplications(_nProjectHandle, _guidHost);
					if (allApplications.Length == 1)
					{
						if (!text.StartsWith(allApplications[0]))
						{
							text = allApplications[0] + "." + text;
						}
					}
					else
					{
						bool flag = false;
						string[] array = allApplications;
						foreach (string value in array)
						{
							if (text.StartsWith(value))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Guid guid = Guid.Empty;
							try
							{
								IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, _guidHost);
								if (objectToRead != null && objectToRead.Object is IDeviceObject2)
								{
									IObject @object = objectToRead.Object;
									IDriverInfo driverInfo = ((IDeviceObject2)((@object is IDeviceObject2) ? @object : null)).DriverInfo;
									IDriverInfo2 val2 = (IDriverInfo2)(object)((driverInfo is IDriverInfo2) ? driverInfo : null);
									if (val2 != null)
									{
										guid = val2.IoApplication;
									}
								}
							}
							catch
							{
							}
							if (guid != Guid.Empty)
							{
								IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, guid);
								if (metaObjectStub != null && !text.StartsWith(metaObjectStub.Name))
								{
									text = metaObjectStub.Name + "." + text;
								}
							}
						}
					}
				}
				return (object)new IconLabelTreeTableViewCellData(mappingVariableEditorPanel.CellData.Image, (object)text);
			}
			return null;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			object tag = ((Control)sender).Tag;
			ISingleLineIECTextEditor val = (ISingleLineIECTextEditor)((tag is ISingleLineIECTextEditor) ? tag : null);
			if (val == null || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return;
			}
			Guid guid = Guid.Empty;
			try
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, _guidHost);
				if (objectToRead != null && objectToRead.Object is IDeviceObject2)
				{
					IObject @object = objectToRead.Object;
					guid = ((IDriverInfo2)((IDeviceObject2)((@object is IDeviceObject2) ? @object : null)).DriverInfo).IoApplication;
				}
			}
			catch
			{
			}
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guid))
			{
				guid = Guid.Empty;
			}
			Guid empty = Guid.Empty;
			GetAllApplications(_nProjectHandle, _guidHost, out var liApplicationGuids);
			IInputAssistantService2 obj2 = APEnvironment.CreateInputAssistantService();
			Debug.Assert(obj2 != null);
			string text = obj2.Invoke(InputAssistantCategories, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guid, (IList<Guid>)liApplicationGuids, (IWin32Window)(Control)sender, out empty);
			if (text != null)
			{
				int num = text.IndexOf("IoConfig_Globals");
				if (num >= 0)
				{
					text = text.Substring(num + 17);
				}
				if (empty != Guid.Empty && liApplicationGuids.Contains(empty))
				{
					IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, empty);
					if (objectToRead2 != null)
					{
						val.Text=(objectToRead2.Name + "." + text);
					}
				}
				else
				{
					string[] allApplications = GetAllApplications(_nProjectHandle, _guidHost);
					if (allApplications.Length == 1)
					{
						val.Text=(allApplications[0] + "." + text);
					}
					else
					{
						val.Text=(text);
						if (guid != Guid.Empty)
						{
							IMetaObject objectToRead3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guid);
							if (objectToRead3 != null)
							{
								val.Text=(objectToRead3.Name + "." + text);
							}
						}
					}
				}
			}
			val.Control.Focus();
		}

		private void OnPanelGotFocus(object sender, EventArgs e)
		{
			object tag = ((Control)sender).Tag;
			ISingleLineIECTextEditor val = (ISingleLineIECTextEditor)((tag is ISingleLineIECTextEditor) ? tag : null);
			if (val != null)
			{
				val.Control.Focus();
			}
		}

		private void EditorIntelliSenseItemsCallback(string stAccessPath, out Icon[] icons, out string[] texts, out string[] descriptions, out bool bIncludeKeywordsIfAppropriate)
		{
			string[] array = ((stAccessPath != null && stAccessPath.Length > 0) ? stAccessPath.Split('.') : new string[0]);
			Debug.Assert(array != null);
			texts = new string[0];
			icons = new Icon[0];
			descriptions = new string[0];
			bIncludeKeywordsIfAppropriate = false;
			if (array.Length == 0)
			{
				texts = GetAllApplications(_nProjectHandle, _guidHost);
				Debug.Assert(texts != null);
				icons = new Icon[texts.Length];
				descriptions = new string[texts.Length];
				for (int i = 0; i < texts.Length; i++)
				{
					icons[i] = null;
					descriptions[i] = string.Empty;
				}
				return;
			}
			try
			{
				string text = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, _guidHost).Name + "." + array[0];
				string text2;
				if (array.Length > 1)
				{
					text2 = string.Empty;
					for (int j = 1; j < array.Length; j++)
					{
						if (j > 1)
						{
							text2 += ".";
						}
						text2 += array[j];
					}
				}
				else
				{
					text2 = ".";
				}
				Guid applicationGuidByName = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetApplicationGuidByName(text);
				IPreCompileContext val = ((applicationGuidByName != Guid.Empty) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(applicationGuidByName) : null);
				if (val == null)
				{
					return;
				}
				bool flag = default(bool);
				IIdentifierInfo[] array2 = val.FindSubelements(Guid.Empty, text2, (FindSubelementsFlags)17, out flag);
				if (!flag && array2 != null && array2.Length != 0)
				{
					icons = new Icon[array2.Length];
					texts = new string[array2.Length];
					descriptions = new string[array2.Length];
					for (int k = 0; k < array2.Length; k++)
					{
						icons[k] = null;
						texts[k] = array2[k].Name;
						descriptions[k] = array2[k].Comment;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		public bool EditorUserInputCallback(char c)
		{
			if (c == '.')
			{
				return true;
			}
			return false;
		}

		private static string[] GetAllApplications(int nProjectHandle, Guid guidHost)
		{
			LList<Guid> liApplicationGuids;
			return GetAllApplications(nProjectHandle, guidHost, out liApplicationGuids);
		}

		private static string[] GetAllApplications(int nProjectHandle, Guid guidHost, out LList<Guid> liApplicationGuids)
		{
			LDictionary<Guid, string> val = new LDictionary<Guid, string>();
			liApplicationGuids = new LList<Guid>();
			try
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidHost);
				if (objectToRead != null && objectToRead.Object is IDeviceObject2)
				{
					foreach (Guid application in DeviceHelper.GetApplications(objectToRead, bWithHidden: false))
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, application);
						val[application]= metaObjectStub.Name;
					}
					string[] array = new string[val.Count];
					val.Values.CopyTo(array, 0);
					liApplicationGuids.AddRange((IEnumerable<Guid>)val.Keys);
					return array;
				}
			}
			catch
			{
			}
			if (nProjectHandle >= 0)
			{
				Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(nProjectHandle);
				Debug.Assert(allObjects != null);
				Guid[] array2 = allObjects;
				foreach (Guid guid in array2)
				{
					IMetaObjectStub mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
					Debug.Assert(mos != null);
					string name = mos.Name;
					if (!typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType) || typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) || APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)))
					{
						continue;
					}
					while (mos.ParentObjectGuid != Guid.Empty)
					{
						mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, mos.ParentObjectGuid);
						Debug.Assert(mos != null);
						if (typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType))
						{
							break;
						}
						if (typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType) && !typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) && !APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)))
						{
							if (mos.ObjectGuid == guidHost)
							{
								val[guid]= name;
							}
							break;
						}
					}
				}
			}
			string[] array3 = new string[val.Count];
			val.Values.CopyTo(array3, 0);
			liApplicationGuids.AddRange((IEnumerable<Guid>)val.Keys);
			return array3;
		}
	}
}
