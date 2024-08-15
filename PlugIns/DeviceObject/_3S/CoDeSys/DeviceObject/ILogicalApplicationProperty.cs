using _3S.CoDeSys.Core.Objects;
using System;

namespace _3S.CoDeSys.DeviceObject
{
    public interface ILogicalApplicationProperty : IObjectProperty, IGenericObject, IArchivable, ICloneable, IComparable
    {
        Guid LogicalApplication { get; }
    }
}
