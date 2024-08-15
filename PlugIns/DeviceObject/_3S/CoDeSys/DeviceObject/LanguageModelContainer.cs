#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	public class LanguageModelContainer
	{
		private class ParameterInitialization
		{
			public uint _uiId;

			public string _stVariable;

			public ParameterInitialization(uint uiId, string stVariable)
			{
				_uiId = uiId;
				_stVariable = stVariable;
			}
		}

		public MultipleStringBuilder sbModuleList = new MultipleStringBuilder();

		public StringBuilder sbParameters = new StringBuilder();

		public MultipleStringBuilder sbValues = new MultipleStringBuilder();

		public StringBuilder sbRetains = new StringBuilder();

		public StringBuilder sbMessages = new StringBuilder();

		public StringBuilder sbInitValues = new StringBuilder();

		public StringBuilder sbParamStruct = new StringBuilder();

		public List<IAssignmentExpression> assInitValues;

		public List<IAssignmentExpression> assInitValuesNoBlobInit;

		public List<IStructureInitialization> struinitParameters;

		public ISequenceStatement seqParamStruct;

		public ISequenceStatement seqParamVarDeclNoBlobInit;

		public List<IStructureInitialization> struinitModules;

		public ILanguageModelBuilder lmBuilder;

		public ILanguageModel lmNew;

		private ArrayList _alTypes = new ArrayList();

		private List<ParameterInitialization> _parameterInitializations = new List<ParameterInitialization>();

		public IList StructTypes => _alTypes;

		public void AddParameterInitialization(uint uiParameterId, string stVariableName)
		{
			_parameterInitializations.Add(new ParameterInitialization(uiParameterId, stVariableName));
		}

		public void GetParameterInitializationStatements(string stBaseName, ISequenceStatement seqDecl)
		{
			if (_parameterInitializations.Count != 0)
			{
				List<IExpression> list = new List<IExpression>();
				List<IExpression> list2 = new List<IExpression>();
				List<IExpression> list3 = new List<IExpression>();
				LanguageModelHelper.GetParameterInitializationListNames(stBaseName, out var stCountVariable, out var stIdsVariable, out var stPointersVariable, out var stSizeVariable);
				for (int i = 0; i < _parameterInitializations.Count; i++)
				{
					ParameterInitialization parameterInitialization = _parameterInitializations[i];
					ILiteralExpression item = lmBuilder.CreateLiteralExpression((IExprementPosition)null, (long)parameterInitialization._uiId, (TypeClass)11, 16);
					list.Add((IExpression)(object)item);
					IExpression val = lmBuilder.ParseExpression(parameterInitialization._stVariable);
					IOperatorExpression item2 = lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)33, val);
					list2.Add((IExpression)(object)item2);
					IExprement obj = lmBuilder.DuplicateExprement((IExprement)(object)val);
					IExpression val2 = (IExpression)(object)((obj is IExpression) ? obj : null);
					IOperatorExpression val3 = lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)36, val2);
					ILiteralExpression val4 = lmBuilder.CreateLiteralExpression((IExprementPosition)null, 8L);
					list3.Add((IExpression)(object)lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)159, (IExpression)(object)val4, (IExpression)(object)val3));
				}
				string text = $"ARRAY [0..{_parameterInitializations.Count - 1}] OF DWORD";
				string text2 = $"ARRAY [0..{_parameterInitializations.Count - 1}] OF POINTER TO DWORD";
				ICompiledType val5 = lmBuilder.ParseType(text);
				ICompiledType val6 = lmBuilder.ParseType(text);
				ICompiledType val7 = lmBuilder.ParseType(text2);
				IArrayInitialization val8 = lmBuilder.CreateArrayInitialisation((IExprementPosition)null, list);
				IArrayInitialization val9 = lmBuilder.CreateArrayInitialisation((IExprementPosition)null, list2);
				IArrayInitialization val10 = lmBuilder.CreateArrayInitialisation((IExprementPosition)null, list3);
				IVariableDeclarationStatement val11 = lmBuilder.CreateVariableDeclarationStatement((IExprementPosition)null, stIdsVariable, val5, (IExpression)(object)val8, (IDirectVariable)null);
				IVariableDeclarationStatement val12 = lmBuilder.CreateVariableDeclarationStatement((IExprementPosition)null, stPointersVariable, val7, (IExpression)(object)val9, (IDirectVariable)null);
				IVariableDeclarationStatement val13 = lmBuilder.CreateVariableDeclarationStatement((IExprementPosition)null, stSizeVariable, val6, (IExpression)(object)val10, (IDirectVariable)null);
				ICompiledType val14 = lmBuilder.CreateSimpleType((TypeClass)4);
				ILiteralExpression val15 = lmBuilder.CreateLiteralExpression((IExprementPosition)null, (long)_parameterInitializations.Count);
				IVariableDeclarationStatement val16 = lmBuilder.CreateVariableDeclarationStatement((IExprementPosition)null, stCountVariable, val14, (IExpression)(object)val15, (IDirectVariable)null);
				seqDecl.AddStatement((IStatement)(object)val16);
				seqDecl.AddStatement((IStatement)(object)val11);
				seqDecl.AddStatement((IStatement)(object)val12);
				seqDecl.AddStatement((IStatement)(object)val13);
			}
		}

		public string GetParameterInitializations(string stBaseName)
		{
			if (_parameterInitializations.Count == 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			LanguageModelHelper.GetParameterInitializationListNames(stBaseName, out var stCountVariable, out var stIdsVariable, out var stPointersVariable, out var stSizeVariable);
			stringBuilder2.AppendFormat("{0} : ARRAY [0..{1}] OF DWORD := [", stIdsVariable, _parameterInitializations.Count - 1);
			stringBuilder3.AppendFormat("{0} : ARRAY [0..{1}] OF POINTER TO DWORD := [", stPointersVariable, _parameterInitializations.Count - 1);
			stringBuilder4.AppendFormat("{0} : ARRAY [0..{1}] OF DWORD := [", stSizeVariable, _parameterInitializations.Count - 1);
			char value = ' ';
			for (int i = 0; i < _parameterInitializations.Count; i++)
			{
				ParameterInitialization parameterInitialization = _parameterInitializations[i];
				stringBuilder2.Append(value);
				stringBuilder2.Append("16#" + parameterInitialization._uiId.ToString("X"));
				stringBuilder3.Append(value);
				stringBuilder3.Append("ADR(" + parameterInitialization._stVariable + ")");
				stringBuilder4.Append(value);
				stringBuilder4.Append("8 * SIZEOF(" + parameterInitialization._stVariable + ")");
				value = ',';
			}
			stringBuilder2.Append("];");
			stringBuilder3.Append("];");
			stringBuilder4.Append("];");
			stringBuilder.AppendLine(stCountVariable + " : DWORD := " + _parameterInitializations.Count + ";");
			stringBuilder.AppendLine(stringBuilder2.ToString());
			stringBuilder.AppendLine(stringBuilder3.ToString());
			stringBuilder.AppendLine(stringBuilder4.ToString());
			return stringBuilder.ToString();
		}

		public void AddStructType(string stName, string stStructDefinition, Guid guidStructDef, bool bHide)
		{
			if (guidStructDef == Guid.Empty)
			{
				Debug.Fail("The Guid for a GVL must not be null");
				throw new ArgumentException("The Guid for a GVL must not be null", "guidStructDef");
			}
			foreach (StructDefinition alType in _alTypes)
			{
				if (alType._stName == stName)
				{
					return;
				}
			}
			_alTypes.Add(new StructDefinition(stName, stStructDefinition, guidStructDef, bHide));
		}

		public void AddStructTypeRange(IList structtypes)
		{
			_alTypes.AddRange(structtypes);
		}

		public void AddCompilerMessage(string stMessageType, string stWhenToShow, long lPosition, Guid guidObject, string stMessage)
		{
			if (sbMessages.Length == 0)
			{
				sbMessages.Append("bCompileError : bool;\n");
			}
			sbMessages.AppendFormat("{{messageguid '{0}'}}\n", guidObject);
			if (lPosition != -1)
			{
				sbMessages.AppendFormat("{{p {0} }}\n", lPosition);
			}
			sbMessages.AppendFormat("{{{0} '{1}' {2}}}\n\r", stMessageType, stMessage, stWhenToShow);
		}
	}
}
