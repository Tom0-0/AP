using System;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[Flags]
	internal enum VariableListColumns
	{
		None = 0x0,
		Line = 0x1,
		Scope = 0x2,
		Name = 0x4,
		Address = 0x8,
		DataType = 0x10,
		Initialization = 0x20,
		RetainPersistent = 64,
		Constant = 128,
		Comment = 256,
		Attributes = 512,
		NothingToDeclare = 1024
	}
}
