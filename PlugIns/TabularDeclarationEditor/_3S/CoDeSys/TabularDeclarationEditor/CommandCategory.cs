using System;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{033A8621-611F-4ddb-A98E-6979055A41F1}")]
	public class CommandCategory : ICommandCategory
	{
		internal static readonly Guid TYPEGUID = new Guid("{033A8621-611F-4ddb-A98E-6979055A41F1}");

		public string Text => Resources.CommandCategoryName;

		public string Description => Resources.CommandCategoryDescription;
	}
}
