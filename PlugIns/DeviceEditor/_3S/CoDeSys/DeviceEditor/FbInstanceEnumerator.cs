#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FbInstanceEnumerator : IEnumerator
	{
		private IDriverInfo _driverInfo;

		private IEnumerator _enumLibs;

		private IEnumerator _enumFbInstances;

		public object Current
		{
			get
			{
				if (_enumFbInstances == null)
				{
					throw new InvalidOperationException("Enumerator not initialized");
				}
				return _enumFbInstances.Current;
			}
		}

		public FbInstanceEnumerator(IDriverInfo driverInfo)
		{
			_driverInfo = driverInfo;
			Reset();
		}

		public void Reset()
		{
			_enumLibs = null;
			_enumFbInstances = null;
			_enumLibs = ((IEnumerable)_driverInfo.RequiredLibs).GetEnumerator();
			MoveToNextLib();
		}

		public bool MoveNext()
		{
			if (_enumFbInstances == null)
			{
				return false;
			}
			if (_enumFbInstances.MoveNext())
			{
				return true;
			}
			if (MoveToNextLib())
			{
				Debug.Assert(_enumFbInstances != null);
				Debug.Assert(_enumFbInstances.MoveNext());
				return true;
			}
			return false;
		}

		private bool MoveToNextLib()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			if (_enumLibs == null)
			{
				return false;
			}
			while (_enumLibs.MoveNext())
			{
				IRequiredLib val = (IRequiredLib)_enumLibs.Current;
				string text = string.Empty;
				if (typeof(IRequiredLib2).IsAssignableFrom(((object)val).GetType()))
				{
					text = ((IRequiredLib2)val).Client;
				}
				if (string.Empty == text && ((ICollection)val.FbInstances).Count > 0)
				{
					_enumFbInstances = ((IEnumerable)val.FbInstances).GetEnumerator();
					return true;
				}
			}
			_enumFbInstances = null;
			_enumLibs = null;
			return false;
		}
	}
}
