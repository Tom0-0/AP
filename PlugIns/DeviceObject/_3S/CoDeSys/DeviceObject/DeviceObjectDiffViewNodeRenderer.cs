using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DeviceObjectDiffViewNodeRenderer : ITreeTableViewRenderer
	{
		private bool _bIndent;

		public static readonly DeviceObjectDiffViewNodeRenderer Indent;

		public static readonly DeviceObjectDiffViewNodeRenderer NoIndent;

		public static readonly Image s_paramImage;

		public static readonly Image s_structuredParamImage;

		public static readonly Image s_inputChannelImage;

		public static readonly Image s_outputChannelImage;

		public static readonly Image s_SectionImage;

		private static readonly StringFormat STRING_FORMAT;

		static DeviceObjectDiffViewNodeRenderer()
		{
			Indent = new DeviceObjectDiffViewNodeRenderer(bIndent: true);
			NoIndent = new DeviceObjectDiffViewNodeRenderer(bIndent: false);
			s_paramImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectDiffViewNodeRenderer), "_3S.CoDeSys.DeviceObject.Resources.ParameterSmall.ico").Handle);
			s_structuredParamImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectDiffViewNodeRenderer), "_3S.CoDeSys.DeviceObject.Resources.StructuredParameterSmall.ico").Handle);
			s_inputChannelImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectDiffViewNodeRenderer), "_3S.CoDeSys.DeviceObject.Resources.InputChannelSmall.ico").Handle);
			s_outputChannelImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectDiffViewNodeRenderer), "_3S.CoDeSys.DeviceObject.Resources.OutputChannelSmall.ico").Handle);
			s_SectionImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectDiffViewNodeRenderer), "_3S.CoDeSys.DeviceObject.Resources.SectionSmall.ico").Handle);
			STRING_FORMAT = new StringFormat(StringFormat.GenericTypographic);
			STRING_FORMAT.Alignment = StringAlignment.Near;
			STRING_FORMAT.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
			STRING_FORMAT.HotkeyPrefix = HotkeyPrefix.None;
			STRING_FORMAT.LineAlignment = StringAlignment.Center;
			STRING_FORMAT.Trimming = StringTrimming.EllipsisCharacter;
		}

		internal DeviceObjectDiffViewNodeRenderer(bool bIndent)
		{
			_bIndent = bIndent;
		}

		public void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
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
			DeviceObjectDiffViewNodeData deviceObjectDiffViewNodeData = node.CellValues[nColumnIndex] as DeviceObjectDiffViewNodeData;
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
			if (_bIndent)
			{
				bounds.X += GetLevel(node) * node.View.Indent;
				bounds.Width -= GetLevel(node) * node.View.Indent;
			}
			if (deviceObjectDiffViewNodeData != null)
			{
				if (deviceObjectDiffViewNodeData.BackColor != Color.Transparent)
				{
					using Brush brush = new SolidBrush(deviceObjectDiffViewNodeData.BackColor);
					g.FillRectangle(brush, _bIndent ? node.GetBounds(nColumnIndex, (CellBoundsPortion)1) : bounds);
				}
				bounds.X += 2;
				bounds.Width -= 2;
				if (deviceObjectDiffViewNodeData.Image != null)
				{
					g.DrawImage(deviceObjectDiffViewNodeData.Image, bounds.X, bounds.Y);
					bounds.X += deviceObjectDiffViewNodeData.Image.Width + 5;
					bounds.Width -= deviceObjectDiffViewNodeData.Image.Width + 5;
				}
				try
				{
					string text = deviceObjectDiffViewNodeData.StLine;
					if (text.Length > 100)
					{
						text = text.Substring(0, 100) + "...";
					}
					using Brush brush2 = new SolidBrush(deviceObjectDiffViewNodeData.ForeColor);
					using Font font = new Font(((Control)(object)view).Font, deviceObjectDiffViewNodeData.FontStyle);
					g.DrawString(text, font, brush2, bounds, STRING_FORMAT);
				}
				catch
				{
				}
			}
			g.DrawLine(Pens.Gray, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
		}

		public Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			_=node.CellValues[nColumnIndex];
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
			if (_bIndent)
			{
				bounds.X += GetLevel(node) * node.View.Indent;
				bounds.Width -= GetLevel(node) * node.View.Indent;
			}
			return bounds;
		}

		public int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
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
			DeviceObjectDiffViewNodeData deviceObjectDiffViewNodeData = node.CellValues[nColumnIndex] as DeviceObjectDiffViewNodeData;
			int num = 0;
			if (deviceObjectDiffViewNodeData != null)
			{
				using (Font font = new Font(((Control)(object)view).Font, deviceObjectDiffViewNodeData.FontStyle))
				{
					num += (int)g.MeasureString(deviceObjectDiffViewNodeData.StLine, font, -1, STRING_FORMAT).Width;
				}
				if (deviceObjectDiffViewNodeData.Image != null)
				{
					num += deviceObjectDiffViewNodeData.Image.Width + 5;
				}
			}
			if (_bIndent)
			{
				num += GetLevel(node) * node.View.Indent;
			}
			return num;
		}

		public string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			DeviceObjectDiffViewNodeData deviceObjectDiffViewNodeData = node.CellValues[nColumnIndex] as DeviceObjectDiffViewNodeData;
			if (deviceObjectDiffViewNodeData == null)
			{
				return string.Empty;
			}
			return deviceObjectDiffViewNodeData.StLine;
		}

		private static int GetLevel(TreeTableViewNode node)
		{
			int num = -1;
			while (node != null)
			{
				num++;
				node = node.Parent;
			}
			return num;
		}
	}
}
