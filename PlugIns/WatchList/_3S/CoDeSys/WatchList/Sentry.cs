#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	internal class Sentry
	{
		private readonly Guid WATCH1 = new Guid("{2EFCE11C-EFB8-4282-B233-0644A415088D}");

		private readonly Guid WATCH2 = new Guid("{88F734F5-B42A-4082-8FB4-178171CEA8F6}");

		private readonly Guid WATCH3 = new Guid("{E3EFC840-A4EF-4f85-92DF-581AF2875E63}");

		private readonly Guid WATCH4 = new Guid("{201E620C-9ABC-4cd2-89BE-CD83D992890D}");

		public Sentry()
		{
			if (APEnvironment.RefactoringServiceOrNull != null)
			{
				APEnvironment.RefactoringServiceOrNull.RefactoringCommitted+=((EventHandler<RefactoringCommittedEventArgs>)OnRefactoringCommitted);
			}
		}

		private void OnRefactoringCommitted(object sender, RefactoringCommittedEventArgs e)
		{
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || ((AbstractRefactoringEventArgs)e).SourceProject != ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle || ((AbstractRefactoringEventArgs)e).Operation == null)
			{
				return;
			}
			List<Guid> list = new List<Guid>(new Guid[4] { WATCH1, WATCH2, WATCH3, WATCH4 });
			if (((IEngine)APEnvironment.Engine).Frame == null)
			{
				return;
			}
			IView[] views = ((IEngine)APEnvironment.Engine).Frame.GetViews();
			foreach (IView val in views)
			{
				if (val.Control is WatchListView)
				{
					WatchListView watchListView = (WatchListView)val.Control;
					if (list.Contains(watchListView.PersistenceGuid))
					{
						list.Remove(watchListView.PersistenceGuid);
						watchListView.HandleRefactoring(e);
					}
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (Guid item in list)
			{
				bool flag = false;
				LList<string> val2 = new LList<string>();
				LList<string> exprsRefactored = null;
				LocalOptionsHelper.LoadWatchExpressions(item, out var watchExpressions, out var _);
				if (watchExpressions == null)
				{
					continue;
				}
				StringReader stringReader = new StringReader(watchExpressions);
				for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
				{
					if (text.Trim().Length > 0)
					{
						val2.Add(text);
					}
				}
				try
				{
					flag = WatchListView.RefactorWatchExpressions(e, val2.ToArray(), null, out exprsRefactored);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					flag = false;
				}
				finally
				{
					if (flag && exprsRefactored != null && exprsRefactored.Count > 0)
					{
						watchExpressions = string.Empty;
						for (int j = 0; j < exprsRefactored.Count; j++)
						{
							if (!string.IsNullOrEmpty(exprsRefactored[j]))
							{
								watchExpressions = watchExpressions + exprsRefactored[j] + Environment.NewLine;
							}
						}
						LocalOptionsHelper.SaveWatchExpressions(item, watchExpressions, string.Empty, forceSaveExpandedExpressions: false);
					}
				}
			}
		}
	}
}
