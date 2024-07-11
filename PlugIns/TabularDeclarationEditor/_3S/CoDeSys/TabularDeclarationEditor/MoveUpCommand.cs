using System;
using System.Drawing;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{4CBEEBB2-181F-40eb-83EE-4073F5323FA1}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_declaration_editor_basics.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/core_vardeclobject_editor_home.htm")]
	[AssociatedOnlineHelpTopic("core.VarDeclObject.Editor.chm::/home.htm")]
	public class MoveUpCommand : AbstractCommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "tabulardeclaration", "moveup" };

		public override string Name => Resources.MoveUpCommandName;

		public override string Description => Resources.MoveUpCommandDescription;

		public override Icon SmallIcon => APEnvironment.Engine.ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.TabularDeclarationEditor.Resources.MoveUp.ico");

		public override bool Enabled => GetActiveEditor()?.CanMoveUp ?? false;

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
			GetActiveEditor()?.MoveUp();
		}
	}
}
