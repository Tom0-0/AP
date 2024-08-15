using System;
using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	public class CyclicCallsList : ICyclicCallsList2, ICyclicCallsList, ICollection, IEnumerable
	{
		private ArrayList _alCyclicCalls;

		public ICyclicCall this[int nIndex] => (ICyclicCall)_alCyclicCalls[nIndex];

		public bool IsSynchronized => false;

		public int Count => _alCyclicCalls.Count;

		public object SyncRoot => _alCyclicCalls.SyncRoot;

		public CyclicCallsList(ArrayList alCyclicCalls)
		{
			_alCyclicCalls = alCyclicCalls;
		}

		public void AddCyclicCall(string _stMethodName, string _stWhenToCall, string _stTask)
		{
			CyclicCall cyclicCall = new CyclicCall();
			cyclicCall.MethodName = _stMethodName;
			cyclicCall.WhenToCall = _stWhenToCall;
			cyclicCall.Task = _stTask;
			_alCyclicCalls.Add(cyclicCall);
		}

		public void RemoveCyclicCall(string _stMethodName, string _stWhenToCall, string _stTask)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (ICyclicCall alCyclicCall in _alCyclicCalls)
			{
				ICyclicCall val = alCyclicCall;
				if (val.MethodName.Equals(_stMethodName) && val.WhenToCall.Equals(_stWhenToCall) && val.Task.Equals(_stTask))
				{
					_alCyclicCalls.Remove(val);
					break;
				}
			}
		}

		public void CopyTo(Array array, int index)
		{
			_alCyclicCalls.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alCyclicCalls.GetEnumerator();
		}
	}
}
