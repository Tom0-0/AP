using System;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal interface IPOUHeaderDialog
	{
		string ChangedComment { get; }

		string ChangedKind { get; }

		string ChangedName { get; }

		string ChangedExtends { get; }

		string ChangedImplements { get; }

		string ChangedReturnType { get; }

		string ChangedAttributes { get; }

		bool RenameWithRefactoring { get; }

		void Initialize(TabularDeclarationModel model, int nProjectHandle, Guid objectGuid, bool bReadOnly);

		DialogResult ShowDialog(IWin32Window owner);
	}
}
