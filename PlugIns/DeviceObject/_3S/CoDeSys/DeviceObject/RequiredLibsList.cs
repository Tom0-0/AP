using System;
using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	public class RequiredLibsList : IRequiredLibsList2, IRequiredLibsList, ICollection, IEnumerable
	{
		private ArrayList _alRequiredLibs;

		public IRequiredLib this[int nIndex]
		{
			get
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Expected O, but got Unknown
				return (IRequiredLib)_alRequiredLibs[nIndex];
			}
			set
			{
				_alRequiredLibs[nIndex] = value;
			}
		}

		public bool IsSynchronized => false;

		public int Count => _alRequiredLibs.Count;

		public object SyncRoot => _alRequiredLibs.SyncRoot;

		public RequiredLibsList(ArrayList alRequiredLibs)
		{
			_alRequiredLibs = alRequiredLibs;
		}

		public void AddRequiredLib(string sLibName, string sVendor, string sVersion, string sIdentifier, string sPlaceholder)
		{
			try
			{
				RequiredLib requiredLib = new RequiredLib();
				requiredLib.LibName = sLibName;
				requiredLib.Vendor = sVendor;
				requiredLib.Version = sVersion;
				requiredLib.Identifier = sIdentifier;
				requiredLib.PlaceHolderLib = sPlaceholder;
				requiredLib.AddedByAP = true;
				if (!ContainsRequiredLib((IRequiredLib)(object)requiredLib))
				{
					_alRequiredLibs.Add(requiredLib);
				}
			}
			catch
			{
			}
		}

		public void RemoveRequiredLib(IRequiredLib libToRemove)
		{
			try
			{
				if (_alRequiredLibs.Contains(libToRemove))
				{
					_alRequiredLibs.Remove(libToRemove);
				}
			}
			catch
			{
			}
		}

		public void CopyTo(Array array, int index)
		{
			_alRequiredLibs.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alRequiredLibs.GetEnumerator();
		}

		private bool ContainsRequiredLib(IRequiredLib libToCheck)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (IRequiredLib alRequiredLib in _alRequiredLibs)
			{
				IRequiredLib val = alRequiredLib;
				if (val.LibName.Equals(libToCheck.LibName) && val.Vendor.Equals(libToCheck.Vendor) && val.Version.Equals(libToCheck.Version))
				{
					return true;
				}
			}
			return false;
		}
	}
}
