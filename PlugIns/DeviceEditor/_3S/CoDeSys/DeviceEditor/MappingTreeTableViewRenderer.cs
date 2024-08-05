using System;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MappingTreeTableViewRenderer : ITreeTableViewRenderer
	{
		private static readonly Image s_noneImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.None.ico").Handle);

		private static readonly Image s_trueImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.CreateNewVariable.ico").Handle);

		private static readonly Image s_trueLightImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.CreateNewVariableLight.ico").Handle);

		private static readonly Image s_falseImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.MapToExistingVariable.ico").Handle);

		private static readonly Image s_FBInstanceImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.MapFBInstance.ico").Handle);

		public static readonly ITreeTableViewRenderer Singleton = (ITreeTableViewRenderer)(object)new MappingTreeTableViewRenderer();

		public int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			return s_trueImage.Width;
		}

		public Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			return node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
		}

		public void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Invalid comparison between Unknown and I4
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			object obj = node.CellValues[nColumnIndex];
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			bool flag = false;
			if (dataElementNode != null && dataElementNode.Parameter != null)
			{
				flag = (int)dataElementNode.Parameter.ChannelType == 3;
			}
			if (obj is MappingTreeTableViewCellValue)
			{
				Image image = (MappingTreeTableViewCellValue)obj switch
				{
					MappingTreeTableViewCellValue.CreateVariable => (flag || !node.View.IsColumnEditable(nColumnIndex)) ? s_trueLightImage : s_trueImage, 
					MappingTreeTableViewCellValue.MapToExistingVariable => s_falseImage, 
					MappingTreeTableViewCellValue.MapToFBInstance => s_FBInstanceImage, 
					_ => s_noneImage, 
				};
				g.DrawImageUnscaled(image, bounds.Left + (bounds.Width - image.Width) / 2, bounds.Top + (bounds.Height - image.Height) / 2, image.Width, image.Height);
			}
		}

		public string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
		{
			return string.Empty;
		}
	}
}
