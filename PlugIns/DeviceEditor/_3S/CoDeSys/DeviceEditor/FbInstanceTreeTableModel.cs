using System;
using System.Collections;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FbInstanceTreeTableModel : DefaultTreeTableModel
	{
		private IUndoManager _undoMgr;

		private LDictionary<long, FbInstanceTreeTableNode> _positionToNode = new LDictionary<long, FbInstanceTreeTableNode>();

		public const int COLIDX_VARIABLE = 0;

		public const int COLIDX_MAPPING = 1;

		public const int COLIDX_TYP = 2;

		private bool _bLocalizationActive;

		private IFbInstanceProvider _fbInstanceProvider;

		public FbInstanceProvider InstanceProvider => _fbInstanceProvider as FbInstanceProvider;

		public bool LocalizationActive
		{
			get
			{
				return _bLocalizationActive;
			}
			set
			{
				_bLocalizationActive = value;
			}
		}

		public IUndoManager UndoManager
		{
			get
			{
				return _undoMgr;
			}
			set
			{
				_undoMgr = value;
			}
		}

		public FbInstanceTreeTableModel(IFbInstanceProvider fbInstanceProvider)
			: base()
		{
			_fbInstanceProvider = fbInstanceProvider;
			((DefaultTreeTableModel)this).AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameVariable"), HorizontalAlignment.Left, IconLabelTreeTableViewRenderer.NormalString, InstanceIconTextBoxTreeTableViewEditor.TextBox, true);
			((DefaultTreeTableModel)this).AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameMapping"), HorizontalAlignment.Left, MappingTreeTableViewRenderer.Singleton, MappingTreeTableViewEditor.Singleton, true);
			((DefaultTreeTableModel)this).AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameType"), HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			Refresh();
		}

		public void Refresh()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			_positionToNode.Clear();
			((DefaultTreeTableModel)this).ClearRootNodes();
			foreach (IFbInstance item in _fbInstanceProvider)
			{
				IFbInstance val = item;
				FbInstanceTreeTableNode fbInstanceTreeTableNode = new FbInstanceTreeTableNode(val, this);
				((DefaultTreeTableModel)this).AddRootNode((ITreeTableNode)(object)fbInstanceTreeTableNode);
				if (val is IFbInstance5)
				{
					_positionToNode[((IFbInstance5)((val is IFbInstance5) ? val : null)).LanguageModelPositionId]= fbInstanceTreeTableNode;
				}
			}
			((DefaultTreeTableModel)this).Sort((ITreeTableNode)null, false, (IComparer)new FBInstanceComparer());
		}

		public void RaiseChanged(FbInstanceTreeTableNode node)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			int num = ((node.Parent != null) ? node.Parent.GetIndex((ITreeTableNode)(object)node) : ((DefaultTreeTableModel)this).Sentinel.GetIndex((ITreeTableNode)(object)node));
			try
			{
				((DefaultTreeTableModel)this).RaiseChanged(new TreeTableModelEventArgs(node.Parent, num, (ITreeTableNode)(object)node));
			}
			catch
			{
			}
		}

		public FbInstanceTreeTableNode GetNodeByPosition(long lPosition)
		{
			FbInstanceTreeTableNode result = null;
			_positionToNode.TryGetValue(lPosition, out result);
			return result;
		}
	}
}
