using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{262BC6B2-AA25-415f-BD65-E7D545D97CD3}")]
	public class OptionEditor : IOptionEditor
	{
		public OptionRoot OptionRoot => (OptionRoot)4;

		public string Name => Resources.OptionEditorName;

		public string Description => Resources.OptionEditorDescription;

		public Icon SmallIcon => Resources.InsertLine;

		public Icon LargeIcon => SmallIcon;

		public Control CreateControl()
		{
			return new OptionControl();
		}

		public bool Save(Control control, ref string stMessage, ref Control failedControl)
		{
			return true;
		}
	}
}
