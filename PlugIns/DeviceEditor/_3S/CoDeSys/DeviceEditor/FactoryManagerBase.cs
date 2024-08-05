#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace _3S.CoDeSys.DeviceEditor
{
	public abstract class FactoryManagerBase<TFactory, TObject> where TFactory : class
	{
		private IEnumerable<TFactory> _factories;

		protected FactoryManagerBase(IEnumerable<TFactory> factories)
		{
			_factories = factories;
		}

		public IEnumerable<FactoryListElement<TFactory>> GetFactories(TObject obj)
		{
			List<FactoryListElement<TFactory>> list = new List<FactoryListElement<TFactory>>();
			foreach (TFactory factory in _factories)
			{
				try
				{
					int index = 0;
					int match = GetMatch(factory, obj);
					int num = 0;
					while (num < list.Count)
					{
						FactoryListElement<TFactory> factoryListElement = list[num];
						if (factoryListElement != null && match > factoryListElement.Match)
						{
							break;
						}
						num++;
						index = num;
					}
					list.Insert(index, new FactoryListElement<TFactory>(factory, match));
				}
				catch (Exception ex)
				{
					Debug.WriteLine(string.Format(GetType().Name + " - {0}.GetMatch() failed: {1}", factory.GetType().FullName, ex.Message));
				}
			}
			return list;
		}

		public TFactory GetFactory(TObject obj)
		{
			TFactory result = null;
			int num = -1;
			foreach (TFactory factory in _factories)
			{
				try
				{
					int match = GetMatch(factory, obj);
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

		protected abstract int GetMatch(TFactory factory, TObject obj);
	}
}
