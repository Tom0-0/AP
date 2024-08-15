using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
    internal class LocalUniqueIdGenerator : DefaultUniqueIdGenerator, IUniqueIdGenerator
    {
        public LocalUniqueIdGenerator()
            : this()
        {
        }

        void IUniqueIdGenerator.Reset()
        {
            ((DefaultUniqueIdGenerator)this).Reset();
        }

        long IUniqueIdGenerator.GetNext(bool bMarkAsUsed)
        {
            return ((DefaultUniqueIdGenerator)this).GetNext(bMarkAsUsed);
        }

        void IUniqueIdGenerator.Use(long nId)
        {
            ((DefaultUniqueIdGenerator)this).Use(nId);
        }

        bool IUniqueIdGenerator.IsUsed(long nId)
        {
            return ((DefaultUniqueIdGenerator)this).IsUsed(nId);
        }

        void IUniqueIdGenerator.RestoreFromString(string st)
        {
            ((DefaultUniqueIdGenerator)this).RestoreFromString(st);
        }

        string IUniqueIdGenerator.StoreToString()
        {
            return ((DefaultUniqueIdGenerator)this).StoreToString();
        }
    }
}
