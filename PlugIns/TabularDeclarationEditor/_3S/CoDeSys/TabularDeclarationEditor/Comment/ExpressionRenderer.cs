using _3S.CoDeSys.Controls.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	/// <summary>
	/// 单元格呈现器
	/// </summary>
    internal class ExpressionRenderer : LabelTreeTableViewRenderer
    {
		internal ExpressionRenderer() : base(false)
		{ }
		protected override FontStyle GetFontStyle(TreeTableViewNode node, int nColumnIndex)
		{
			InitValueNode initValueNode = node.View.GetModelNode(node) as InitValueNode;
			if (initValueNode != null)
			{
				if (initValueNode.HasChildren)
				{
					return FontStyle.Regular;
				}
				return FontStyle.Bold;
			}
			if (node.View.GetModelNode(node) is TooManyValuesMarkerNode)
			{
				return FontStyle.Italic;
			}
			return FontStyle.Regular;
		}
	}
}
