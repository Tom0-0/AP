using _3S.CoDeSys.Controls.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	internal class TooManyValuesMarkerNode : ITreeTableNode
	{
		private ITreeTableNode _parentNode;

		private string _type;

		private string _defaultValue;

		private int _count;

		private const int COLIDX_EXPRESSION = 0;

		private const int COLIDX_INITVALUE = 1;

		private const int COLIDX_DATATYPE = 2;

		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => _parentNode;

		public TooManyValuesMarkerNode(ITreeTableNode parentNode, int count, string type, string def)
		{
			_parentNode = parentNode;
			_count = count;
			_type = type;
			_defaultValue = def;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return null;
		}

		public int GetIndex(ITreeTableNode node)
		{
			return -1;
		}

		public object GetValue(int nColumnIndex)
		{
			return nColumnIndex switch
			{
				0 => $"{_count} remaining values",
				1 => _defaultValue,
				2 => _type,
				_ => string.Empty,
			};
		}

		public bool IsEditable(int nColumnIndex)
		{
			return false;
		}

		public void SetValue(int nColumnIndex, object value)
		{
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
		}
	}
}
