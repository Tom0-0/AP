using System.Drawing;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class BoldRenderer : LabelTreeTableViewRenderer
	{
		internal BoldRenderer()
			: base(false)
		{
		}

		protected override FontStyle GetFontStyle(TreeTableViewNode node, int nColumnIndex)
		{
			return FontStyle.Bold;
		}
	}
}
