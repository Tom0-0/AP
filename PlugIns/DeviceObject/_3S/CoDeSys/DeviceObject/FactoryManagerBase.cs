#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace _3S.CoDeSys.DeviceObject
{
	public abstract class FactoryManagerBase<TFactory, TObject> where TFactory : class
	{
		private IEnumerable<TFactory> _factories;

		protected TFactory GetFactory(TObject[] objects)
		{
			TFactory result = null;
			int num = 0;
			foreach (TFactory factory in _factories)
			{
				try
				{
					int match = GetMatch(factory, objects);
					if (match > num)
					{
						num = match;
						result = factory;
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(string.Format(GetType().Name + " - {0}.GetMatch() failed: {1}", factory.GetType().FullName, ex.Message));
				}
			}
			return result;
		}

		protected abstract int GetMatch(TFactory factory, TObject[] objects);

		protected FactoryManagerBase(IEnumerable<TFactory> factories)
		{
			_factories = factories;
		}
	}
}
