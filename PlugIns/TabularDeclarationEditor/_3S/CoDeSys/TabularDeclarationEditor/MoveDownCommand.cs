using System;
using System.Drawing;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{77D89F0D-5E4E-49b7-AF9A-82140F4B0055}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_declaration_editor_basics.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/core_vardeclobject_editor_home.htm")]
	[AssociatedOnlineHelpTopic("core.VarDeclObject.Editor.chm::/home.htm")]
	public class MoveDownCommand : AbstractCommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "tabulardeclaration", "movedown" };

		public override string Name => Resources.MoveDownCommandName;

		public override string Description => Resources.MoveDownCommandDescription;

		public override Icon SmallIcon => APEnvironment.Engine.ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.TabularDeclarationEditor.Resources.MoveDown.ico");

		public override bool Enabled => GetActiveEditor()?.CanMoveDown ?? false;

		public override string[] BatchCommand => BATCH_COMMAND;

		public override void ExecuteBatch(string[] arguments)
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
			if (APEnvironment.Engine.Frame == null)
			{
				throw new BatchInteractiveException(BatchCommand);
			}
			GetActiveEditor()?.MoveDown();
		}
	}
}
