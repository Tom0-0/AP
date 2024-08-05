using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MyIconLabelTreeTableViewRenderer : IconLabelTreeTableViewRenderer
	{
		private static ITreeTableViewRenderer s_normalString = (ITreeTableViewRenderer)(object)new MyIconLabelTreeTableViewRenderer(null, bPathEllipses: false);

		private static readonly int MAX_STRING_LENGTH = 4096;

		internal Func<string> HighlightSubstringDelegate { get; private set; }

		public static ITreeTableViewRenderer NormalString => s_normalString;

		public MyIconLabelTreeTableViewRenderer(Func<string> highlightSubstringDelegate, bool bPathEllipses)
			: base(bPathEllipses)
		{
			HighlightSubstringDelegate = highlightSubstringDelegate;
		}

		protected override FontStyle GetFontStyle(TreeTableViewNode node, int nColumnIndex)
		{
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Expected O, but got Unknown
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			if (dataElementNode != null && !dataElementNode.PlcNode.AlwaysMapToNew)
			{
				bool flag = dataElementNode != null && nColumnIndex == dataElementNode.GetAddressIndex();
				if (flag && dataElementNode.HasInvalidIecAddress)
				{
					return base.GetFontStyle(node, nColumnIndex) | FontStyle.Strikeout;
				}
				if (flag)
				{
					bool flag2 = false;
					IDataElement dataElement = dataElementNode.DataElement;
					if (dataElement != null && dataElement.IoMapping != null && dataElement.IoMapping.VariableMappings != null && ((ICollection)dataElement.IoMapping.VariableMappings).Count > 0 && dataElement.IoMapping.VariableMappings[0]
						.CreateVariable)
					{
						flag2 = true;
					}
					if (!flag2)
					{
						for (DataElementNode dataElementNode2 = dataElementNode.Parent as DataElementNode; dataElementNode2 != null; dataElementNode2 = dataElementNode2.Parent as DataElementNode)
						{
							IDataElement dataElement2 = dataElementNode2.DataElement;
							if (dataElement2 is IDataElement5 && ((IDataElement5)((dataElement2 is IDataElement5) ? dataElement2 : null)).IsUnion)
							{
								foreach (IDataElement item in (IEnumerable)dataElement2.SubElements)
								{
									IDataElement val = item;
									if (val != null && val.IoMapping != null && val.IoMapping.VariableMappings != null && ((ICollection)val.IoMapping.VariableMappings).Count > 0 && !val.IoMapping.VariableMappings[0]
										.CreateVariable)
									{
										return base.GetFontStyle(node, nColumnIndex) | FontStyle.Strikeout;
									}
								}
							}
							if (dataElement2 != null && dataElement2.IoMapping != null && dataElement2.IoMapping.VariableMappings != null && ((ICollection)dataElement2.IoMapping.VariableMappings).Count > 0 && !dataElement2.IoMapping.VariableMappings[0]
								.CreateVariable)
							{
								return base.GetFontStyle(node, nColumnIndex) | FontStyle.Strikeout;
							}
						}
					}
				}
			}
			return base.GetFontStyle(node, nColumnIndex);
		}

		public override int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			object obj = node.CellValues[nColumnIndex];
			IconLabelTreeTableViewCellData val = (IconLabelTreeTableViewCellData)((obj is IconLabelTreeTableViewCellData) ? obj : null);
			if (val != null && val.Image == null)
			{
				return ((IconLabelTreeTableViewRenderer)this).GetPreferredWidth(node, nColumnIndex, g) - SystemInformation.SmallIconSize.Width - 4;
			}
			return ((IconLabelTreeTableViewRenderer)this).GetPreferredWidth(node, nColumnIndex, g);
		}

		public override Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			Rectangle editableArea = ((IconLabelTreeTableViewRenderer)this).GetEditableArea(node, nColumnIndex, g);
			object obj = node.CellValues[nColumnIndex];
			IconLabelTreeTableViewCellData val = (IconLabelTreeTableViewCellData)((obj is IconLabelTreeTableViewCellData) ? obj : null);
			if (val != null && val.Image == null)
			{
				editableArea.X -= SystemInformation.SmallIconSize.Width + 4;
				editableArea.Width += SystemInformation.SmallIconSize.Width + 4;
				return editableArea;
			}
			return ((IconLabelTreeTableViewRenderer)this).GetEditableArea(node, nColumnIndex, g);
		}

		protected override Brush GetForeColorBrush(TreeTableViewNode node, int nColumnIndex)
		{
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			if (dataElementNode != null && dataElementNode.ReadOnly)
			{
				return new SolidBrush(ControlPaint.LightLight(((Control)(object)node.View).ForeColor));
			}
			return base.GetForeColorBrush(node, nColumnIndex);
		}

		public override void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			TreeTableView view = node.View;
			if (view == null)
			{
				throw new ArgumentException("The node is not associated with a view.");
			}
			object obj = node.CellValues[nColumnIndex];
			IconLabelTreeTableViewCellData val = (IconLabelTreeTableViewCellData)((obj is IconLabelTreeTableViewCellData) ? obj : null);
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
			if (val == null)
			{
				return;
			}
			int num = 6;
			if (val.Image != null && bounds.Width > SystemInformation.SmallIconSize.Width + 4)
			{
				num += SystemInformation.SmallIconSize.Width;
				g.DrawImageUnscaled(val.Image, bounds.Left + 2, bounds.Top + (bounds.Height - SystemInformation.SmallIconSize.Height) / 2, SystemInformation.SmallIconSize.Width, SystemInformation.SmallIconSize.Height);
				num = val.Image.Width + 6;
			}
			string text = base.ConvertToString((object)val);
			bounds.X += num;
			bounds.Width -= num;
			if (text == null || bounds.Width <= 0)
			{
				return;
			}
			text = text.Replace("\r\n", "  ");
			text = text.Replace("\r", "  ");
			text = text.Replace("\n", "  ");
			base.GetStringFormat(view, nColumnIndex);
			using (Brush brush = base.GetBackColorBrush(node, nColumnIndex))
			{
				g.FillRectangle(brush, bounds);
			}
			using (base.GetForeColorBrush(node, nColumnIndex))
			{
				using Font font = new Font(((Control)(object)view).Font, base.GetFontStyle(node, nColumnIndex));
				if (text.Length > MAX_STRING_LENGTH)
				{
					text = text.Substring(0, MAX_STRING_LENGTH);
				}
				ITreeTableNode modelNode = node.View.GetModelNode(node);
				if (modelNode is DataElementNode || modelNode is FBCreateNode)
				{
					string text2 = ((HighlightSubstringDelegate != null) ? HighlightSubstringDelegate() : null);
					int num2 = ((!string.IsNullOrEmpty(text2)) ? text.IndexOf(text2, StringComparison.OrdinalIgnoreCase) : (-1));
					if (num2 >= 0)
					{
						string text3 = text.Substring(0, num2);
						string text4 = text.Substring(num2, text2.Length);
						int num3 = TextRenderer.MeasureText("<" + text3 + ">", font).Width - TextRenderer.MeasureText("<>", font).Width;
						int width = TextRenderer.MeasureText("<" + text4 + ">", font).Width - TextRenderer.MeasureText("<>", font).Width;
						Rectangle rect = new Rectangle(bounds.X + num3, bounds.Y, width, bounds.Height);
						rect.Intersect(bounds);
						g.FillRectangle(Brushes.Yellow, rect);
					}
				}
				TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;
				if (modelNode is TaskUpdateNode && (modelNode as TaskUpdateNode).TaskGuids.Count == 0)
				{
					TextRenderer.DrawText(g, text, font, bounds, Color.LightGray, flags);
				}
				else
				{
					TextRenderer.DrawText(g, text, font, bounds, Color.Black, flags);
				}
			}
		}
	}
}
