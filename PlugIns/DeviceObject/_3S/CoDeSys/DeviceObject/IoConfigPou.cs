using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{4B379231-84B0-4343-B002-7B2F5DA4FF3D}")]
	public class IoConfigPou : ILanguageModelProvider, IStructuredLanguageModelProvider
	{
		private Guid _applicationguid;

		private Guid _guidLmIoConfigErrorPou;

		private Guid _logicGuid;

		private Guid _objectGuid;

		private ISequenceStatement _seq;

		public IoConfigPou(Guid applicationguid, Guid logicGuid, Guid objectGuid, Guid guidLmIoConfigErrorPou, ISequenceStatement seq)
		{
			_applicationguid = applicationguid;
			_guidLmIoConfigErrorPou = guidLmIoConfigErrorPou;
			_seq = seq;
			_logicGuid = logicGuid;
			_objectGuid = objectGuid;
		}

		public IoConfigPou()
		{
		}

		public string GetLanguageModel()
		{
			return string.Empty;
		}

		public ILanguageModel GetStructuredLanguageModel(ILanguageModelBuilder lmbuilder)
		{
			ILanguageModel val = lmbuilder.CreateLanguageModel(_applicationguid, _logicGuid, _objectGuid, string.Empty);
			ILanguageModelBuilder3 val2 = (ILanguageModelBuilder3)(object)((lmbuilder is ILanguageModelBuilder3) ? lmbuilder : null);
			if (val2 != null)
			{
				if (_guidLmIoConfigErrorPou == Guid.Empty)
				{
					_guidLmIoConfigErrorPou = LanguageModelHelper.CreateDeterministicGuid(_applicationguid, PouDefinitions.ErrorPou_Name);
				}
				ILMPOU val3 = ((ILanguageModelBuilder)val2).CreatePou(PouDefinitions.ErrorPou_Name, _guidLmIoConfigErrorPou);
				ISequenceStatement2 val4 = ((ILanguageModelBuilder)val2).CreateSequenceStatement((IExprementPosition)null);
				val3.Interface=((ISequenceStatement)(object)val4);
				ISequenceStatement2 val5 = ((ILanguageModelBuilder)val2).CreateSequenceStatement((IExprementPosition)null);
				IVariableDeclarationListStatement val6 = ((ILanguageModelBuilder)val2).CreateVariableDeclarationListStatement((IExprementPosition)null, (VarFlag)1, (ISequenceStatement)(object)val5);
				ICompiledType val7 = ((ILanguageModelBuilder)val2).ParseType("POINTER TO BYTE");
				((ISequenceStatement)val5).AddStatement((IStatement)(object)((ILanguageModelBuilder)val2).CreateSimpleVariableDeclarationStatement((IExprementPosition)null, "pPointer", val7));
				IPOUDeclarationStatement val8 = ((ILanguageModelBuilder)val2).CreatePOUDeclarationStatement((IExprementPosition)null, (Operator)93, PouDefinitions.ErrorPou_Name);
				((ISequenceStatement)val4).AddStatement(val2.CreatePragmaStatement2((IExprementPosition)null, "attribute 'signature_flag' := '1073741824'"));
				((ISequenceStatement)val4).AddStatement(val2.CreatePragmaStatement2((IExprementPosition)null, "implicit"));
				((ISequenceStatement)val4).AddStatement((IStatement)(object)val8);
				((ISequenceStatement)val4).AddStatement((IStatement)(object)val6);
				ISequenceStatement2 body = ((ILanguageModelBuilder)val2).CreateSequenceStatement((IExprementPosition)null);
				val3.Body=((ISequenceStatement)(object)body);
				val3.Body.AddStatement((IStatement)(object)_seq);
				val.AddPou(val3);
			}
			return val;
		}
	}
}
