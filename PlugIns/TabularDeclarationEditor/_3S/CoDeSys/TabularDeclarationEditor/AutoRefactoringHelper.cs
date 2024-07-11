using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Refactoring;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{0A8BF98F-0071-49A0-9678-D1F19F76445A}")]
	public class AutoRefactoringHelper : IAutomaticRefactoringOperationDefiner
	{
		private static readonly Guid s_configContextTabDeclEdit = new Guid("{C6A6B571-C6F2-4A09-8EBB-1F2DEB410280}");

		private static readonly Guid s_configDescRenameVar = new Guid("{0A695B9E-82A0-4756-8DE3-714931EE2CF1}");

		private static readonly Lazy<IAutomaticRefactoringOperationContext> s_context = new Lazy<IAutomaticRefactoringOperationContext>(() => APEnvironment.RefactoringService.Configuration.ContextByGuid(s_configContextTabDeclEdit));

		private static readonly Lazy<IAutomaticRefactoringOperationInfo> s_renameVariable = new Lazy<IAutomaticRefactoringOperationInfo>(() => APEnvironment.RefactoringService.Configuration.OperationByGuid(s_configDescRenameVar));

		public IEnumerable<KeyValuePair<Guid, string>> DefinedContexts
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<KeyValuePair<Guid, string>> DefinedOperations
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<KeyValuePair<Guid, Guid>> AvailableOperations
		{
			get
			{
				yield return new KeyValuePair<Guid, Guid>(s_configContextTabDeclEdit, s_configDescRenameVar);
			}
		}

		internal static bool AutoRenameVariableEnabled => APEnvironment.RefactoringService.Configuration.IsEnabled(s_context.Value, s_renameVariable.Value);

		internal static AutomaticRefactoringQueryResult TryQueryAndPerformRenameOfVariable(string stOldName, string stNewName, IEditor objectEditor)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			return APEnvironment.RefactoringService.AutomaticRefactoringHelper.TryQueryAndPerformRenameOfVariable(s_configContextTabDeclEdit, objectEditor.ProjectHandle, objectEditor.ObjectGuid, objectEditor.ObjectGuid, stOldName, stNewName);
		}
	}
}
