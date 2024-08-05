using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.UtilitiesContrib;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MyCheckBoxTreeTableViewRenderer : CheckBoxTreeTableViewRenderer
	{
		private static Image s_checkedImage = ResourceHelper.LoadBitmap(typeof(TreeTableView), "_3S.CoDeSys.Controls.Controls.Crossed.bmp", Point.Empty);

		private static Image s_uncheckedImage = ResourceHelper.LoadBitmap(typeof(TreeTableView), "_3S.CoDeSys.Controls.Controls.Unchecked.bmp", Point.Empty);

		private static Image s_BusCycleImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(MyCheckBoxTreeTableViewRenderer), "_3S.CoDeSys.DeviceEditor.Resources.Buscycle.ico").Handle);

		private static ITreeTableViewRenderer s_checkBox = (ITreeTableViewRenderer)(object)new MyCheckBoxTreeTableViewRenderer();

		public static ITreeTableViewRenderer CheckBox => s_checkBox;

		public override void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			object obj = node.CellValues[nColumnIndex];
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
			if (!(obj is bool))
			{
				return;
			}
			TaskUpdateNode taskUpdateNode = node.View.GetModelNode(node) as TaskUpdateNode;
			if (taskUpdateNode != null)
			{
				TaskInfo columnTaskInfo = taskUpdateNode.Model.GetColumnTaskInfo(nColumnIndex);
				if (columnTaskInfo != null && taskUpdateNode.BusTaskGuid == columnTaskInfo.taskInfo.TaskGuid)
				{
					g.DrawImageUnscaled(s_BusCycleImage, bounds.Left + (bounds.Width - s_BusCycleImage.Width) / 2 - s_BusCycleImage.Width, bounds.Top + (bounds.Height - s_BusCycleImage.Height) / 2, s_BusCycleImage.Width, s_BusCycleImage.Height);
				}
			}
			Image image = (((bool)obj) ? s_checkedImage : s_uncheckedImage);
			g.DrawImageUnscaled(image, bounds.Left + (bounds.Width - image.Width) / 2, bounds.Top + (bounds.Height - image.Height) / 2, image.Width, image.Height);
			if (taskUpdateNode != null && taskUpdateNode.TaskGuids.Count == 0)
			{
				using Brush brush = new SolidBrush(Color.FromArgb(160, ((Control)(object)node.View).BackColor));
				g.FillRectangle(brush, bounds.Left + (bounds.Width - image.Width) / 2, bounds.Top + (bounds.Height - image.Height) / 2, image.Width, image.Height);
			}
		}

		public MyCheckBoxTreeTableViewRenderer()
			: base()
		{
		}
	}
}
