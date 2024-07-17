using System;

namespace _3S.CoDeSys.OnlineCommands
{
    [Flags]
    public enum CompileDiffState : byte
    {
        Equal = 0x0,
        Added = 0x1,
        Deleted = 0x2,
        CodeDifferent = 0x4,
        InterfaceDifferent = 0x8,
        LocationDifferent = 0x10,
        VFTableDifferent = 0x20,
        PropertyAccessorChanged = 0x40,
        AnythingDifferent = 0x7C
    }
}
