using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Refactoring;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{1CE9AFED-6E57-46B1-BF84-FA2D082694AB}")]
	public class DeviceEditorAutomaticRefactoringOperationDefinitions : IAutomaticRefactoringOperationDefiner
	{
		internal static readonly Guid ConfigContext_MappingEditor = new Guid("{5645C603-1246-44CD-BE36-C900D71D60AB}");

		internal static readonly Guid ConfigDesc_RenameVar = new Guid("{0A695B9E-82A0-4756-8DE3-714931EE2CF1}");

		private static readonly Lazy<IAutomaticRefactoringOperationContext> s_context = new Lazy<IAutomaticRefactoringOperationContext>(() => APEnvironment.RefactoringService.Configuration.ContextByGuid(ConfigContext_MappingEditor));

		private static readonly Lazy<IAutomaticRefactoringOperationInfo> s_renameVar = new Lazy<IAutomaticRefactoringOperationInfo>(() => APEnvironment.RefactoringService.Configuration.OperationByGuid(ConfigDesc_RenameVar));

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
				yield return new KeyValuePair<Guid, Guid>(ConfigContext_MappingEditor, ConfigDesc_RenameVar);
			}
		}

		internal static bool AutoRenameVarEnabled
		{
			get
			{
				if (APEnvironment.RefactoringService.AutomaticRefactoringHelper is IAutomaticRefactoringHelper2)
				{
					return APEnvironment.RefactoringService.Configuration.IsEnabled(s_context.Value, s_renameVar.Value);
				}
				return false;
			}
		}
	}
}
