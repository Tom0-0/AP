#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.LanguageModelUtilities;

namespace _3S.CoDeSys.WatchList
{
	internal class ExpressionEditor : ITreeTableViewEditor
	{
		private static readonly ExpressionEditor s_singleton = new ExpressionEditor();

		private static readonly Icon ICON_DEVICE = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ExpressionEditor), "_3S.CoDeSys.WatchList.Resources.DeviceSmall.ico");

		private static readonly Icon ICON_APPLICATION = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ExpressionEditor), "_3S.CoDeSys.WatchList.Resources.ApplicationSmall.ico");

		public static ITreeTableViewEditor Singleton => (ITreeTableViewEditor)(object)s_singleton;

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Expected O, but got Unknown
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			TreeTableView view = node.View;
			if (view == null)
			{
				throw new ArgumentException("The node is not associated with a view.");
			}
			ExpressionData expressionData = node.CellValues[nColumnIndex] as ExpressionData;
			Panel panel = new Panel();
			panel.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			panel.GotFocus += OnPanelGotFocus;
			Button button = new Button();
			button.FlatStyle = FlatStyle.Standard;
			button.Text = "...";
			button.Size = new Size(24, panel.Bounds.Height);
			button.Location = new Point(panel.Bounds.Width - button.Size.Width, 0);
			button.Font = ((Control)(object)view).Font;
			button.Click += OnButtonClick;
			ISingleLineIECTextEditor val = APEnvironment.CreateSingleLineIECTextEditor();
			Debug.Assert(val != null);
			IInputAssistantCategory val2 = APEnvironment.CreateWatchVariablesInputAssistantCategory();
			Debug.Assert(val2 != null);
			val.InputAssistantCategories=((IInputAssistantCategory[])(object)new IInputAssistantCategory[1] { val2 });
			val.BackColor=(((Control)(object)view).BackColor);
			val.Bounds=(new Rectangle(0, 0, panel.Bounds.Width - button.Bounds.Width, panel.Bounds.Height));
			val.Font=(((Control)(object)view).Font);
			val.ForeColor=(((Control)(object)view).ForeColor);
			string text = string.Empty;
			if (expressionData != null)
			{
				text = CastExpressionFormatter.Instance.GetCastExpressionDisplayString(expressionData.DisplayExpression);
			}
			val.Text=((cImmediate == '\0') ? text : cImmediate.ToString());
			val.SetIntelliSenseItemsCallback(new IntelliSenseItemsCallback(EditorIntelliSenseItemsCallback));
			if (cImmediate == '\0')
			{
				val.SelectAll();
			}
			else
			{
				val.Select(1, 0);
			}
			Control control = val.Control;
			panel.Tag = val;
			button.Tag = val;
			panel.Controls.Add(control);
			panel.Controls.Add(button);
			control.Select();
			if (!control.Focus())
			{
				Timer timer = new Timer();
				timer.Tag = control;
				timer.Interval = 20;
				timer.Tick += OnUpdateFocus;
				timer.Enabled = true;
			}
			return panel;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			object tag = control.Tag;
			ISingleLineIECTextEditor val = (ISingleLineIECTextEditor)((tag is ISingleLineIECTextEditor) ? tag : null);
			string text = val.Text;
			string stFullExpression = string.Empty;
			if (text.Trim().Length > 0)
			{
				if (node.View.Model is WatchListModel && ((WatchListModel)(object)node.View.Model).COLUMN_APPLICATION_PREFIX > -1)
				{
					stFullExpression = text;
				}
				else
				{
					IPreCompileUtilities preCompileUtils = ((ILanguageModelUtilities)APEnvironment.LanguageModelUtilities).PreCompileUtils;
					IPreCompileUtilities obj = ((preCompileUtils is IPreCompileUtilities7) ? preCompileUtils : null);
					Debug.Assert(obj != null);
					text = ((IPreCompileUtilities7)obj).AddDeviceApplicationPrefix(text);
				}
			}
			if (val == null)
			{
				return null;
			}
			ExpressionData result = null;
			try
			{
				result = new ExpressionData(stFullExpression, text, null, bTryToConvertToCastExpression: true);
				return result;
			}
			catch (InvalidExpressionSyntaxException ex)
			{
				APEnvironment.MessageService.Error(ex.Message, ex.Message, Array.Empty<object>());
				return result;
			}
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			object tag = ((Control)sender).Tag;
			ISingleLineIECTextEditor val = (ISingleLineIECTextEditor)((tag is ISingleLineIECTextEditor) ? tag : null);
			if (val != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				IInputAssistantService obj = APEnvironment.CreateInputAssistantService();
				Debug.Assert(obj != null);
				IInputAssistantCategory val2 = APEnvironment.CreateWatchVariablesInputAssistantCategory();
				Debug.Assert(val2 != null);
				string text = obj.Invoke((IInputAssistantCategory[])(object)new IInputAssistantCategory[1] { val2 }, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, Guid.Empty, (IWin32Window)(Control)sender);
				if (text != null)
				{
					val.Text=(text);
				}
				val.Control.Focus();
			}
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
			string[] array = ((!string.IsNullOrEmpty(stAccessPath)) ? stAccessPath.Split('.') : new string[0]);
			texts = new string[0];
			icons = new Icon[0];
			descriptions = new string[0];
			bIncludeKeywordsIfAppropriate = false;
			List<string> list = new List<string>();
			List<Icon> list2 = new List<Icon>();
			List<string> list3 = new List<string>();
			try
			{
				if (array.Length == 0)
				{
					string[] allDevices = Common.GetAllDevices();
					Debug.Assert(allDevices != null);
					string[] array2 = allDevices;
					foreach (string item in array2)
					{
						list.Add(item);
						list2.Add(ICON_DEVICE);
						list3.Add(string.Empty);
					}
					if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || !(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication != Guid.Empty))
					{
						goto IL_02c6;
					}
					IPreCompileContext precompileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication);
					if (precompileContext == null)
					{
						goto IL_02c6;
					}
					bool flag = default(bool);
					IIdentifierInfo[] array3 = precompileContext.FindSubelements(Guid.Empty, stAccessPath, (FindSubelementsFlags)1, out flag);
					if (array3 == null || array3.Length == 0)
					{
						return;
					}
					CreateFilteredIntelliSenseItems(array3, list, list2, list3);
					goto IL_02c6;
				}
				string[] allApplications = Common.GetAllApplications(array[0]);
				if (allApplications.Length != 0)
				{
					if (array.Length == 1)
					{
						texts = allApplications;
						icons = new Icon[allApplications.Length];
						descriptions = new string[allApplications.Length];
						for (int j = 0; j < allApplications.Length; j++)
						{
							icons[j] = ICON_APPLICATION;
							descriptions[j] = string.Empty;
						}
						goto IL_02c6;
					}
					string text = array[0] + "." + array[1];
					string text2;
					if (array.Length > 2)
					{
						text2 = string.Empty;
						for (int k = 2; k < array.Length; k++)
						{
							if (k > 2)
							{
								text2 += ".";
							}
							text2 += array[k];
						}
					}
					else
					{
						text2 = ".";
					}
					Guid applicationGuidByName = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetApplicationGuidByName(text);
					IPreCompileContext val = ((applicationGuidByName != Guid.Empty) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(applicationGuidByName) : null);
					if (val != null)
					{
						bool flag2 = default(bool);
						IIdentifierInfo[] array4 = val.FindSubelements(Guid.Empty, text2, (FindSubelementsFlags)1, out flag2);
						if (array4 != null && array4.Length != 0)
						{
							CreateFilteredIntelliSenseItems(array4, list, list2, list3);
							goto IL_02c6;
						}
					}
				}
				else
				{
					if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || !(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication != Guid.Empty))
					{
						goto IL_02c6;
					}
					IPreCompileContext precompileContext2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication);
					if (precompileContext2 == null)
					{
						goto IL_02c6;
					}
					bool flag3 = default(bool);
					IIdentifierInfo[] array5 = precompileContext2.FindSubelements(Guid.Empty, stAccessPath, (FindSubelementsFlags)1, out flag3);
					if (array5 != null && array5.Length != 0)
					{
						CreateFilteredIntelliSenseItems(array5, list, list2, list3);
						goto IL_02c6;
					}
				}
				goto end_IL_0051;
				IL_02c6:
				texts = list.ToArray();
				icons = list2.ToArray();
				descriptions = list3.ToArray();
				end_IL_0051:;
			}
			catch
			{
			}
		}

		private static void CreateFilteredIntelliSenseItems(IIdentifierInfo[] iiis, List<string> textList, List<Icon> iconList, List<string> descriptionList)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Invalid comparison between Unknown and I4
			foreach (IIdentifierInfo val in iiis)
			{
				if (val.Signature != null)
				{
					if ((int)val.Signature.POUType == 103 || (int)val.Signature.POUType == 88 || VisibilityUtil.IsHiddenSignature(val.Signature, GUIHidingFlags.AllChecks))
					{
						continue;
					}
				}
				else if (val.Variable != null)
				{
					IIdentifierInfo obj = ((val is IIdentifierInfo2) ? val : null);
					if (VisibilityUtil.IsHiddenVariable((obj != null) ? ((IIdentifierInfo2)obj).ContainingSignature : null, val.Variable, GUIHidingFlags.AllChecks))
					{
						continue;
					}
				}
				textList.Add(val.Name);
				iconList.Add(Common.GetIcon(val));
				descriptionList.Add(val.Comment);
			}
		}

		private void OnUpdateFocus(object sender, EventArgs args)
		{
			Timer obj = sender as Timer;
			obj.Tick -= OnUpdateFocus;
			(obj.Tag as Control)?.Focus();
		}
	}
}
