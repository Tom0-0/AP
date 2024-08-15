using System;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.TextDocument;
using _3S.CoDeSys.VarDeclObject;

namespace _3S.CoDeSys.DeviceObject.LogicalDevice
{
	internal class LogicalGVLTextDocumentProvider : ITextDocumentProvider
	{
		public ITextDocument GetTextDocument(IObject obj)
		{
			LogicalExchangeGVLObject logicalExchangeGVLObject = obj as LogicalExchangeGVLObject;
			if (logicalExchangeGVLObject != null)
			{
				IVarDeclObject @interface = logicalExchangeGVLObject.Interface;
				ITextVarDeclObject val = (ITextVarDeclObject)(object)((@interface is ITextVarDeclObject) ? @interface : null);
				if (val != null)
				{
					return val.TextDocument;
				}
			}
			throw new ArgumentException("Wrong type.");
		}
	}
}
