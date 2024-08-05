using System;
using System.Collections;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FbCreateTreeTableModel : AbstractTreeTableModel
	{
		private string _stSearchText = string.Empty;

		private LDictionary<string, LibraryNode> _dictNodes = new LDictionary<string, LibraryNode>();

		internal string SearchText
		{
			get
			{
				return _stSearchText;
			}
			set
			{
				UnderlyingModel.ClearRootNodes();
				_dictNodes.Clear();
				_stSearchText = value;
			}
		}

		public FbCreateTreeTableModel()
			: base()
		{
			base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameType"), HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new MyIconLabelTreeTableViewRenderer(() => _stSearchText, bPathEllipses: false), TextBoxTreeTableViewEditor.TextBox, false);
		}

		internal void AddType(ISignature sign, IVariable variable, string stNodeName)
		{
			try
			{
				if (MatchSearchText(sign.OrgName))
				{
					LibraryNode libraryNode = default(LibraryNode);
					if (!_dictNodes.TryGetValue(sign.LibraryPath, out libraryNode))
					{
						libraryNode = new LibraryNode(this, sign, stNodeName);
						UnderlyingModel.AddRootNode((ITreeTableNode)(object)libraryNode);
						_dictNodes.Add(sign.LibraryPath, libraryNode);
					}
					new FBCreateNode(libraryNode, sign, variable);
				}
			}
			catch
			{
			}
		}

		internal bool MatchSearchText(string stNodeName)
		{
			if (!string.IsNullOrEmpty(_stSearchText))
			{
				return stNodeName.IndexOf(_stSearchText, StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return true;
		}

		internal void Sort(int column, SortOrder order)
		{
			if (order != 0)
			{
				UnderlyingModel.Sort((ITreeTableNode)null, true, (IComparer)new FBCreateComparer(column, order));
			}
		}
	}
}
