#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{5728F0BB-F39A-4892-A23A-5EE1B76AE095}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_add_watch.htm")]
	public class AddWatchCommand : IStandardCommand, ICommand
	{
		private class ExpressionSelection
		{
			public readonly string Expression;

			public readonly Guid ObjectGuid;

			public readonly IPreCompileContext Context;

			public ExpressionSelection(string expression)
				: this(expression, Guid.Empty, null)
			{
			}

			public ExpressionSelection(string expression, Guid guid, IPreCompileContext ctx)
			{
				Expression = expression;
				ObjectGuid = guid;
				Context = ctx;
			}

			public bool IsValid(int projectHandle)
			{
				if (string.IsNullOrWhiteSpace(Expression))
				{
					return false;
				}
				if (Context == null)
				{
					return ObjectGuid == Guid.Empty;
				}
				return APEnvironment.ObjectMgr.ExistsObject(projectHandle, ObjectGuid);
			}
		}

		private static readonly Guid GUID_ONLINECOMMANDCATEGORY = new Guid("{8DDBE3C7-2966-4ba9-A27B-7DB46265241D}");

		private static readonly Guid GUID_GLOBALWATCHLISTVIEWFACTORY1 = new Guid("{2EFCE11C-EFB8-4282-B233-0644A415088D}");

		private static readonly string[] BATCH_COMMAND = new string[2] { "debug", "addwatch" };

		public Guid Category => GUID_ONLINECOMMANDCATEGORY;

		public string Name => Strings.AddWatchCommandName;

		public string Description => Strings.AddWatchCommandDescription;

		public string ToolTipText => Name;

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.WatchList.Resources.AddWatchSmall.ico");

		public Icon LargeIcon => SmallIcon;

		public bool Enabled => true;

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
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			if (!bContextMenu)
			{
				return false;
			}
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || ((IEngine)APEnvironment.Engine).Frame == null || ((IEngine)APEnvironment.Engine).Frame.ActiveView != ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront || ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront == null || ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront.Editor == null)
			{
				return false;
			}
			IEnumerable<ExpressionSelection> selection = GetSelection();
			if (selection == null || !selection.Any())
			{
				return false;
			}
			int nProjectHandle = ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront.Editor
				.ProjectHandle;
			if (selection.Any((ExpressionSelection s) => !s.IsValid(nProjectHandle)))
			{
				return false;
			}
			IEditorView editorViewInFront = ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront;
			IHasOnlineMode val = (IHasOnlineMode)(object)((editorViewInFront is IHasOnlineMode) ? editorViewInFront : null);
			if (val == null || val.OnlineState.InstancePath == null)
			{
				return false;
			}
			return true;
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (arguments.Length != 0)
			{
				throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 0);
			}
			if (APEnvironment.FrameForm == null)
			{
				throw new BatchInteractiveException(BatchCommand);
			}
			IEnumerable<ExpressionSelection> selection = GetSelection();
			if (selection == null || !selection.Any())
			{
				return;
			}
			int nProjectHandle = ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront.Editor
				.ProjectHandle;
			if (selection.Any((ExpressionSelection s) => !s.IsValid(nProjectHandle)))
			{
				return;
			}
			IEditorView editorViewInFront = ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront;
			IHasOnlineMode val = (IHasOnlineMode)(object)((editorViewInFront is IHasOnlineMode) ? editorViewInFront : null);
			if (val == null || val.OnlineState.InstancePath == null)
			{
				return;
			}
			WatchListView watchListView = FindTargetWatchListView();
			if (watchListView == null)
			{
				return;
			}
			Common.AddToWatch(from s in selection
				where s.Context == null
				select s.Expression, watchListView);
			foreach (ExpressionSelection item in selection.Where((ExpressionSelection s) => s.Context != null))
			{
				Common.AddToWatch(val.OnlineState, item.Context, nProjectHandle, item.ObjectGuid, item.Expression, watchListView);
			}
		}

		private static WatchListView FindTargetWatchListView()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			IFrame frame = ((IEngine)APEnvironment.Engine).Frame;
			IFrame9 val = (IFrame9)(object)((frame is IFrame9) ? frame : null);
			IView[] views;
			if (val != null)
			{
				views = ((IFrame)val).GetViews();
				for (int i = 0; i < views.Length; i++)
				{
					WatchListView watchListView = views[i] as WatchListView;
					if (watchListView != null && (int)val.GetVisibility((IView)(object)watchListView) == 0)
					{
						return watchListView;
					}
				}
			}
			views = ((IEngine)APEnvironment.Engine).Frame.GetViews();
			for (int i = 0; i < views.Length; i++)
			{
				WatchListView watchListView2 = views[i] as WatchListView;
				if (watchListView2 != null)
				{
					return watchListView2;
				}
			}
			return ((IEngine)APEnvironment.Engine).Frame.OpenView(GUID_GLOBALWATCHLISTVIEWFACTORY1, (string)null) as WatchListView;
		}

		public static IEnumerable<T> GetAllSubControlsOfType<T>(Control control)
		{
			IEnumerable<Control> source = control.Controls.Cast<Control>();
			IEnumerable<T> second = source.OfType<T>();
			return source.SelectMany((Control ctrl) => AddWatchCommand.GetAllSubControlsOfType<T>(ctrl)).Concat(second);
		}

		private static IEnumerable<WatchListView> GetFocusedEditorWatchListView(IEditorView editorView)
		{
			Control control = editorView as Control;
			if (control == null)
			{
				return System.Linq.Enumerable.Empty<WatchListView>();
			}
			return from w in GetAllSubControlsOfType<WatchListView>(control)
				where w.ContainsFocus
				select w;
		}

		private IEnumerable<ExpressionSelection> GetSelection()
		{
			IEditorView editorViewInFront = ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront;
			Debug.Assert(editorViewInFront != null);
			List<ExpressionSelection> list = new List<ExpressionSelection>();
			foreach (WatchListView item in GetFocusedEditorWatchListView(editorViewInFront))
			{
				IEnumerable<string> selectedExpressions_FullPath = item.GetSelectedExpressions_FullPath();
				list.AddRange(selectedExpressions_FullPath.Select((string str) => new ExpressionSelection(str)));
			}
			if (list.Count > 0)
			{
				return list;
			}
			IMetaObject objectToRead = editorViewInFront.Editor.GetObjectToRead();
			Debug.Assert(objectToRead != null);
			IObject @object = objectToRead.Object;
			Debug.Assert(@object != null);
			Guid objectGuid = objectToRead.ObjectGuid;
			long num = default(long);
			int num2 = default(int);
			editorViewInFront.GetSelection(out num, out num2);
			long num3 = default(long);
			short num4 = default(short);
			if (num2 < 0)
			{
				num2 = -num2;
				PositionHelper.SplitPosition(num, out num3, out num4);
				num4 = (short)(num4 - num2);
				num = PositionHelper.CombinePosition(num3, num4);
			}
			PositionHelper.SplitPosition(num, out num3, out num4);
			ISourcePosition val = (ISourcePosition)(object)new SourcePosition(objectToRead.ProjectHandle, objectToRead.ObjectGuid, num3, num4, (short)num2);
			IPreCompileContext ctx = default(IPreCompileContext);
			IExprement val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindExpressionAtSourcePosition(val, (WhatToFind)1, out ctx);
			IWatchListView4 firstFocusedUiElement = UIHelper.GetFirstFocusedUiElement<IWatchListView4>();
			string expression;
			if (firstFocusedUiElement == null)
			{
				expression = ((val2 == null) ? @object.GetContentString(ref num, ref num2, true) : val2.ToString());
			}
			else
			{
				bool flag = false;
				expression = firstFocusedUiElement.GetSelectedExpression(false, out flag);
			}
			return new ExpressionSelection[1]
			{
				new ExpressionSelection(expression, objectGuid, ctx)
			};
		}
	}
}
