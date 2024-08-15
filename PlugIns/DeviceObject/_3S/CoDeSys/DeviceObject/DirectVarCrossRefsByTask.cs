using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DirectVarCrossRefsByTask
	{
		private DirectVarCrossRefList[] _inputs;

		private DirectVarCrossRefList[] _outputs;

		public DirectVarCrossRefList[] Inputs => _inputs;

		public DirectVarCrossRefList[] Outputs => _outputs;

		public DirectVarCrossRefsByTask(ICompileContext comcon, int nNumTasks)
		{
			_inputs = new DirectVarCrossRefList[nNumTasks];
			_outputs = new DirectVarCrossRefList[nNumTasks];
			for (int i = 0; i < nNumTasks; i++)
			{
				_inputs[i] = new DirectVarCrossRefList();
				_outputs[i] = new DirectVarCrossRefList();
			}
			Fill(comcon);
		}

		public LList<DirectVarCrossRef> GetCrossRefsForTask(byte byTask, bool bInput)
		{
			if (bInput)
			{
				return _inputs[byTask].GetCrossRefs();
			}
			return _outputs[byTask].GetCrossRefs();
		}

		private void Fill(ICompileContext comcon)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Invalid comparison between Unknown and I4
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Invalid comparison between Unknown and I4
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Invalid comparison between Unknown and I4
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Invalid comparison between Unknown and I4
			IDirectVariableCrossRefTable directVariableTable = comcon.GetDirectVariableTable();
			IDirectVariable[] allDirectVariables = directVariableTable.AllDirectVariables;
			foreach (IDirectVariable val in allDirectVariables)
			{
				if (val.Incomplete)
				{
					continue;
				}
				DirectVarCrossRefList[] array;
				if ((int)val.Location == 1)
				{
					array = _inputs;
				}
				else
				{
					if ((int)val.Location != 2)
					{
						continue;
					}
					array = _outputs;
				}
				IAddressCrossReference[] crossReferencesOfDirectVariable = directVariableTable.GetCrossReferencesOfDirectVariable(val);
				foreach (IAddressCrossReference val2 in crossReferencesOfDirectVariable)
				{
					ISignature signatureById = comcon.GetSignatureById(val2.CodeId);
					if (signatureById == null)
					{
						continue;
					}
					if ((int)val.Location == 2 && ((APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0)) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)62))))
					{
						bool flag = false;
						IAddressCodePosition[] positions = val2.Positions;
						for (int k = 0; k < positions.Length; k++)
						{
							if (((int)positions[k].Access & 2) == 2)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							continue;
						}
					}
					byte[] taskReferenceList = signatureById.TaskReferenceList;
					foreach (byte b in taskReferenceList)
					{
						if (!array[b].Contains(val))
						{
							array[b].Add(new DirectVarCrossRef(comcon, val, val2));
						}
					}
				}
			}
		}
	}
}
