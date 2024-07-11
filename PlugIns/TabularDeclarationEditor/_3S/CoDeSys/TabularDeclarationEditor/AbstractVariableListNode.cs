using System;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal abstract class AbstractVariableListNode : ITreeTableNode
	{
		private VariableListModel _model;

		protected VariableListModel Model => _model;

		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => null;

		protected AbstractVariableListNode(VariableListModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			_model = model;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			throw new ArgumentOutOfRangeException("nIndex");
		}

		public int GetIndex(ITreeTableNode node)
		{
			return -1;
		}

		public abstract object GetValue(int nColumnIndex);

		public abstract bool IsEditable(int nColumnIndex);

		public abstract void SetValue(int nColumnIndex, object value);

		public void SwapChildren(int nIndex1, int nIndex2)
		{
			throw new ArgumentOutOfRangeException("nIndex1");
		}
	}
}
