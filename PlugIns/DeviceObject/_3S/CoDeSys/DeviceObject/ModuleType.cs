using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;

namespace _3S.CoDeSys.DeviceObject
{
    [TypeGuid("{0F5FE9F2-6F78-44e8-BE24-5879C0C99599}")]
    [StorageVersion("3.3.2.0")]
    public class ModuleType : GenericObject2, IExplicitConProperty, IObjectProperty, IGenericObject, IArchivable, ICloneable, IComparable
    {
        public static readonly Guid My_Guid = new Guid("{0F5FE9F2-6F78-44e8-BE24-5879C0C99599}");

        [DefaultSerialization("ModuleType")]
        [StorageVersion("3.3.2.0")]
        [DefaultDuplication(DuplicationMethod.Deep)]
        protected int _nModuleType;

        public static Guid Guid => My_Guid;

        int IExplicitConProperty.ModuleType => _nModuleType;

        public ModuleType()
        {
        }

        internal ModuleType(int nModuleType)
            : this()
        {
            _nModuleType = nModuleType;
        }

        internal ModuleType(ModuleType original)
            : this()
        {
            _nModuleType = original._nModuleType;
        }

        public override object Clone()
        {
            return new ModuleType(this);
        }
    }
}
