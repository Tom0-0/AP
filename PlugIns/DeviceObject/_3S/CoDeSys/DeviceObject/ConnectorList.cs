using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{52a6325f-063a-4748-86b6-ec8aa02d3232}")]
	[StorageVersion("3.3.0.0")]
	public class ConnectorList : GenericObject2, IConnectorCollection2, IConnectorCollection, ICollection, IEnumerable
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("InnerList")]
		[StorageVersion("3.3.0.0")]
		private ArrayList _alConnectors = new ArrayList();

		public IConnector this[int nIndex] => (IConnector)_alConnectors[nIndex];

		public bool IsSynchronized => false;

		public int Count => _alConnectors.Count;

		public object SyncRoot => _alConnectors.SyncRoot;

		public ConnectorList()
			: base()
		{
		}

		private ConnectorList(ConnectorList original)
			: this()
		{
			_alConnectors = new ArrayList(original._alConnectors.Count);
			foreach (Connector alConnector in original._alConnectors)
			{
				_alConnectors.Add(((GenericObject)alConnector).Clone());
			}
		}

		public override object Clone()
		{
			ConnectorList connectorList = new ConnectorList(this);
			((GenericObject)connectorList).AfterClone();
			return connectorList;
		}

		public void Add(Connector connector)
		{
			int num = _alConnectors.BinarySearch(connector, new ConnectorComparer());
			if (num < 0)
			{
				num = ~num;
				_alConnectors.Insert(num, connector);
			}
			else
			{
				_alConnectors[num] = connector;
			}
		}

		public void AddRange(IList list)
		{
			foreach (Connector item in list)
			{
				Add(item);
			}
		}

		public IConnector GetById(int nConnectorId)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (IConnector alConnector in _alConnectors)
			{
				IConnector val = alConnector;
				if (val.ConnectorId == nConnectorId)
				{
					return val;
				}
			}
			throw new ArgumentException("Collection does not contain a connector with id " + nConnectorId);
		}

		public bool ContainsConnectorId(int nConnectorId)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			foreach (IConnector alConnector in _alConnectors)
			{
				if (alConnector.ConnectorId == nConnectorId)
				{
					return true;
				}
			}
			return false;
		}

		internal void RemoveById(int nConnectorId)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _alConnectors.Count; i++)
			{
				if (((IConnector)_alConnectors[i]).ConnectorId == nConnectorId)
				{
					_alConnectors.RemoveAt(i);
					break;
				}
			}
		}

		internal void Remove(IConnector conRemove)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			for (int i = 0; i < _alConnectors.Count; i++)
			{
				if (((object)(IConnector)_alConnectors[i]).Equals((object)conRemove))
				{
					_alConnectors.RemoveAt(i);
					break;
				}
			}
		}

		internal void Clear()
		{
			_alConnectors.Clear();
		}

		public void CopyTo(Array array, int index)
		{
			_alConnectors.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alConnectors.GetEnumerator();
		}
	}
}
