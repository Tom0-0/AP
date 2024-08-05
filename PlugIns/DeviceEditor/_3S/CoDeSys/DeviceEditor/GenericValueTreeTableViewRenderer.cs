using System;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class GenericValueTreeTableViewRenderer : ITreeTableViewRenderer
	{
		public int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			return GetRenderer(node).GetPreferredWidth(node, nColumnIndex, g);
		}

		public Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			return GetRenderer(node).GetEditableArea(node, nColumnIndex, g);
		}

		public void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			GetRenderer(node).DrawCell(node, nColumnIndex, g);
		}

		public string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
		{
			return GetRenderer(node).GetStringRepresentation(node, nColumnIndex);
		}

		private ITreeTableViewRenderer GetRenderer(TreeTableViewNode viewNode)
		{
			if (viewNode == null)
			{
				throw new ArgumentNullException("viewNode");
			}
			DataElementNode dataElementNode = (viewNode.View ?? throw new ArgumentException("This node is not associated with a view.")).GetModelNode(viewNode) as DataElementNode;
			if (dataElementNode == null || dataElementNode.DataElement == null)
			{
				return MyLabelTreeTableViewRenderer.NormalString;
			}
			if (dataElementNode.DataElement.IsEnumeration)
			{
				return dataElementNode.GetEnumerationRenderer();
			}
			return MyLabelTreeTableViewRenderer.NormalString;
		}
	}
}
