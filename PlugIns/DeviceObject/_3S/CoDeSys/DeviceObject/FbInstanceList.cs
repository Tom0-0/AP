using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public class FbInstanceList : IFbInstanceList3, IFbInstanceList2, IFbInstanceList, ICollection, IEnumerable
	{
		private ArrayList _alInstanceList;

		public IFbInstance this[int nIndex]
		{
			get
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Expected O, but got Unknown
				return (IFbInstance)_alInstanceList[nIndex];
			}
			set
			{
				_alInstanceList[nIndex] = value;
			}
		}

		public bool IsSynchronized => false;

		public int Count => _alInstanceList.Count;

		public object SyncRoot => _alInstanceList.SyncRoot;

		public FbInstanceList(ArrayList alInstanceList)
		{
			_alInstanceList = alInstanceList;
		}

		public IFbInstance AddFbInstance(string sFbType, string sInstanceName)
		{
			return AddFbInstance(sFbType, sInstanceName, null);
		}

		public IFbInstance AddFbInstance(string sFbType, string sInstanceName, IIoProvider ioprovider)
		{
			FBInstance fBInstance = new FBInstance();
			try
			{
				string text = sInstanceName;
				if (ioprovider != null)
				{
					IMetaObject metaObject = ioprovider.GetMetaObject();
					text = text.Replace("$(DeviceName)", metaObject.Name);
					(fBInstance.Instance as VariableMapping).CreateVariable = true;
					fBInstance.BaseName = sInstanceName;
					ParameterSet obj = ((ioprovider != null) ? ioprovider.ParameterSet : null) as ParameterSet;
					object obj2;
					if (obj == null)
					{
						obj2 = null;
					}
					else
					{
						IDeviceObject2 device = obj.Device;
						obj2 = ((device != null) ? ((IObject)device).UniqueIdGenerator : null);
					}
					if (obj2 != null)
					{
						fBInstance.LanguageModelPositionId = ((IObject)(ioprovider.ParameterSet as ParameterSet).Device).UniqueIdGenerator.GetNext(true);
					}
				}
				fBInstance.Instance.Variable=(text);
				fBInstance.SetFbType(sFbType);
				if (!ContainsInstance((IFbInstance)(object)fBInstance))
				{
					_alInstanceList.Add(fBInstance);
					return (IFbInstance)(object)fBInstance;
				}
				fBInstance = null;
				return (IFbInstance)(object)fBInstance;
			}
			catch
			{
				return (IFbInstance)(object)fBInstance;
			}
		}

		public void RemoveFbInstance(IFbInstance fbInstanceToRemove)
		{
			try
			{
				if (_alInstanceList.Contains(fbInstanceToRemove))
				{
					_alInstanceList.Remove(fbInstanceToRemove);
				}
			}
			catch
			{
			}
		}

		public void CopyTo(Array array, int index)
		{
			_alInstanceList.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alInstanceList.GetEnumerator();
		}

		private bool ContainsInstance(IFbInstance instanceToCheck)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (IFbInstance alInstance in _alInstanceList)
			{
				IFbInstance val = alInstance;
				if (val.FbName.Equals(instanceToCheck.FbName) && val.Instance.Variable.Equals(instanceToCheck.Instance.Variable))
				{
					return true;
				}
			}
			return false;
		}
	}
}
