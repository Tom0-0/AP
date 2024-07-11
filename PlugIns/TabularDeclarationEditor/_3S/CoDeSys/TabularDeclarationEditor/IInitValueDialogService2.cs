using _3S.CoDeSys.AutoDeclare;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
    [ReleasedInterface]
    interface IInitValueDialogService2 : IInitValueDialogService
    {
        string Invoke(IWin32Window owner, string stVariable, string stType, string stInitValue, string stComment, IMetaObjectStub invokedBy);
    }
}
