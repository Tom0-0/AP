using System;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[Flags]
	internal enum ModelTokenType : long
	{
		Function = 0x1L,
		FunctionBlock = 0x2L,
		Interface = 0x4L,
		Method = 0x8L,
		Program = 0x10L,
		Property = 0x20L,
		Type = 0x40L,
		Text = 0x80L,
		Colon = 0x100L,
		Struct = 0x200L,
		Union = 0x400L,
		EnumLeftParenthesis = 0x800L,
		Var = 0x1000L,
		VarAccess = 0x2000L,
		VarConfig = 0x4000L,
		VarExternal = 0x8000L,
		VarGlobal = 0x10000L,
		VarInOut = 0x20000L,
		VarInput = 0x40000L,
		VarOutput = 0x80000L,
		VarStat = 0x100000L,
		VarTemp = 0x200000L,
		DocumentationComment = 0x1000000L,
		EndOfLine = 0x2000000L,
		Whitespace = 0x4000000L,
		Implements = 0x8000000L,
		Pragma = 0x10000000L,
		Extends = 0x20000000L,
		EndVar = 0x40000000L,
		Constant = 0x80000000L,
		Retain = 0x100000000L,
		Persistent = 0x200000000L,
		Semicolon = 0x400000000L,
		At = 0x800000000L,
		Assign = 0x1000000000L,
		CPlusPlusCommentOccupyingWholeLine = 0x2000000000L,
		CPlusPlusCommentInMixedLine = 0x4000000000L,
		PascalCommentOccupyingWholeLine = 0x8000000000L,
		PascalCommentInMixedLine = 0x10000000000L,
		AnyKind = 0x7FL,
		AnyDUTKind = 0xE00L,
		AnyVarBlock = 0x3FF000L,
		AnyComment = 0x1E001000000L,
		AnyBlank = 0x6000000L,
		AnyBlankOrComment = 0x1E007000000L
	}
}
