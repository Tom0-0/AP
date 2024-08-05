using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal interface IParameterTreeTable
	{
		IUndoManager UndoManager { get; set; }

		TreeTableView View { get; }

		WeakMulticastDelegate ConfigurationNodeChanged { get; set; }

		bool DefaultColumnForInputsEditable { get; }

		int MapColumn(int nColumnIndex);

		int GetIndexOfColumn(int nColumn);

		void RaiseChanged(IParameterTreeNode node);

		void RaiseInserted(TreeTableModelEventArgs e);

		void RaiseRemoved(TreeTableModelEventArgs e);
	}
}
