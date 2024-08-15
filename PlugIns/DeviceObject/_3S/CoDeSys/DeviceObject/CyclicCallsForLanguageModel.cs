#define DEBUG
using System.Diagnostics;
using System.Text;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class CyclicCallsForLanguageModel
	{
		private class CallsInTask
		{
			private StringBuilder[] _calls = new StringBuilder[4];

			public void Add(string stCall, int nCallIndex)
			{
				if (_calls[nCallIndex] == null)
				{
					_calls[nCallIndex] = new StringBuilder();
				}
				_calls[nCallIndex].Append(stCall + "\n");
			}

			public string GetCalls(int nCallIndex)
			{
				if (_calls[nCallIndex] == null)
				{
					return "";
				}
				return _calls[nCallIndex].ToString();
			}
		}

		private int _nBusCycleTask;

		private ITaskInfo[] _taskinfos;

		private CallsInTask[] _callsInTask;

		public ITaskInfo[] TaskInfos => _taskinfos;

		public void Init(ITaskInfo[] taskinfos, int nBusCycleTask)
		{
			_taskinfos = taskinfos;
			_callsInTask = new CallsInTask[taskinfos.Length];
			for (int i = 0; i < taskinfos.Length; i++)
			{
				_callsInTask[i] = new CallsInTask();
			}
			_nBusCycleTask = nBusCycleTask;
		}

		public bool IsTaskValid(string stTask)
		{
			for (int i = 0; i < _taskinfos.Length; i++)
			{
				if (_taskinfos[i].TaskName.Equals(stTask))
				{
					return true;
				}
			}
			return false;
		}

		private string GetCallStatement(IFbInstance fb, ICyclicCall call, bool UpdateIOsInStop, ISignature fbSignature)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			string variable = fb.Instance.Variable;
			if (fbSignature != null && (int)fbSignature.POUType == 118 && fbSignature.Inputs.Length == 3)
			{
				IVariable val = fbSignature["PTASKINFO"];
				IVariable val2 = fbSignature["PAPPLICATIONINFO"];
				if (val != null && val.GetFlag((VarFlag)2) && val2 != null && val2.GetFlag((VarFlag)2))
				{
					string text = "(pTaskInfo := pTaskInfo, pApplicationInfo := pApplicationInfo)";
					return variable + "." + call.MethodName + text + ";";
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!UpdateIOsInStop && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
			{
				stringBuilder.Append("IF NOT bAppHalted THEN\r\n");
			}
			if (string.IsNullOrEmpty(call.MethodName))
			{
				stringBuilder.AppendFormat("{0}();\r\n", variable, call.MethodName);
			}
			else
			{
				stringBuilder.AppendFormat("{0}.{1}();\r\n", variable, call.MethodName);
			}
			if (!UpdateIOsInStop && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
			{
				stringBuilder.Append("END_IF\r\n");
			}
			return stringBuilder.ToString();
		}

		public void Add(IFbInstance fb, ICyclicCall call, bool UpdateIOsInStop, string stUserCall, int nBusCycleTask, ISignature fbSignature)
		{
			int whenToCallIndex = GetWhenToCallIndex(call.WhenToCall);
			if (whenToCallIndex < 0)
			{
				return;
			}
			string callStatement = GetCallStatement(fb, call, UpdateIOsInStop, fbSignature);
			if (!call.Task.StartsWith("#"))
			{
				for (int i = 0; i < _taskinfos.Length; i++)
				{
					if (_taskinfos[i].TaskName.Equals(call.Task))
					{
						_callsInTask[i].Add(callStatement, whenToCallIndex);
						break;
					}
				}
			}
			else if (call.Task == "#buscycletask" && stUserCall == string.Empty)
			{
				if (nBusCycleTask >= 0 && nBusCycleTask < TaskInfos.Length)
				{
					_callsInTask[nBusCycleTask].Add(callStatement, whenToCallIndex);
				}
				else if (_nBusCycleTask >= 0 && _nBusCycleTask < _taskinfos.Length)
				{
					_callsInTask[_nBusCycleTask].Add(callStatement, whenToCallIndex);
				}
			}
			else if (call.Task == "#eachtask")
			{
				for (int j = 0; j < _callsInTask.Length; j++)
				{
					_callsInTask[j].Add(callStatement, whenToCallIndex);
				}
			}
			else
			{
				if (!(call.Task == "#userdeftask") && !(stUserCall != string.Empty))
				{
					return;
				}
				for (int k = 0; k < _taskinfos.Length; k++)
				{
					if (_taskinfos[k].TaskName.Equals(stUserCall))
					{
						_callsInTask[k].Add(callStatement, whenToCallIndex);
						break;
					}
				}
			}
		}

		public string GetCalls(int nTaskId, string stWhenToCall)
		{
			int whenToCallIndex = GetWhenToCallIndex(stWhenToCall);
			Debug.Assert(whenToCallIndex >= 0);
			return _callsInTask[nTaskId].GetCalls(whenToCallIndex);
		}

		protected static int GetWhenToCallIndex(string stWhenToCall)
		{
			return stWhenToCall.ToLowerInvariant() switch
			{
				"beforereadinputs" => 0, 
				"afterreadinputs" => 1, 
				"beforewriteoutputs" => 2, 
				"afterwriteoutputs" => 3, 
				_ => -1, 
			};
		}
	}
}
