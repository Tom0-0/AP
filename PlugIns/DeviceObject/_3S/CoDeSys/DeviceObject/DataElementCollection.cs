#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{45b6f676-61a0-4c78-a679-8c725851fa1e}")]
	[StorageVersion("3.3.0.0")]
	public class DataElementCollection : GenericObject2, IDataElementCollection, ICollection, IEnumerable, IDataElementParent
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("elements")]
		[StorageVersion("3.3.0.0")]
		[StorageSaveAsNonGenericCollection("3.3.0.0-3.5.0.255")]
		private LList<IDataElement> _elementsOrdered = new LList<IDataElement>();

		private LDictionary<string, IDataElement> _elementsDictionary = new LDictionary<string, IDataElement>();

		private IDataElementParent _parent;

		public IIoProvider IoProvider
		{
			get
			{
				if (_parent != null)
				{
					return _parent.IoProvider;
				}
				return null;
			}
		}

		public IDataElement DataElement => null;

		public bool IsParameter => false;

		internal IDataElementParent Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		public IDataElement this[int nIndex]
		{
			get
			{
				Debug.Assert(_elementsDictionary.Count == _elementsOrdered.Count);
				return _elementsOrdered[nIndex];
			}
		}

		public IDataElement this[string stIdentifier]
		{
			get
			{
				Debug.Assert(_elementsDictionary.Count == _elementsOrdered.Count);
				IDataElement result = default(IDataElement);
				_elementsDictionary.TryGetValue(stIdentifier, out result);
				return result;
			}
		}

		public int Count => _elementsOrdered.Count;

		public bool IsSynchronized => false;

		public object SyncRoot => this;

		public DataElementCollection()
			: base()
		{
		}

		internal DataElementCollection(DataElementCollection original)
			: this()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			_elementsOrdered = new LList<IDataElement>(original._elementsOrdered.Count);
			foreach (IDataElement item in original._elementsOrdered)
			{
				_elementsOrdered.Add((IDataElement)((ICloneable)item).Clone());
			}
		}

		public override object Clone()
		{
			DataElementCollection dataElementCollection = new DataElementCollection(this);
			((GenericObject)dataElementCollection).AfterClone();
			return dataElementCollection;
		}

		public void Add(IDataElement element)
		{
			DataElementBase dataElementBase = element as DataElementBase;
			if (dataElementBase != null && _parent != null)
			{
				dataElementBase.Parent = this;
			}
			_elementsDictionary.Add(element.Identifier, element);
			try
			{
				_elementsOrdered.Add(element);
			}
			catch
			{
				_elementsDictionary.Remove(element.Identifier);
				throw;
			}
		}

		public void Notify(IDataElement dataelement, string[] path)
		{
			if (_parent == null)
			{
				throw new InvalidOperationException("Cannot notify a change whithout a parent");
			}
			_parent.Notify(dataelement, path);
		}

		public Parameter GetParameter()
		{
			if (_parent == null)
			{
				return null;
			}
			return _parent.GetParameter();
		}

		public long GetBitOffset(IDataElement elem)
		{
			return _parent.GetBitOffset(elem);
		}

		public bool Contains(string stIdentifier)
		{
			return _elementsDictionary.ContainsKey(stIdentifier);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_elementsOrdered).CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _elementsOrdered.GetEnumerator();
		}

		public override void AfterDeserialize()
		{
			foreach (IDataElement item in _elementsOrdered)
			{
				if (item is DataElementBase)
				{
					((DataElementBase)(object)item).Parent = this;
				}
				_elementsDictionary.Add(item.Identifier, item);
			}
		}

		public override void AfterClone()
		{
			foreach (IDataElement item in _elementsOrdered)
			{
				if (item is DataElementBase)
				{
					((DataElementBase)(object)item).Parent = this;
				}
				_elementsDictionary.Add(item.Identifier, item);
			}
		}

		internal void Clear()
		{
			_elementsOrdered.Clear();
			_elementsDictionary.Clear();
		}
	}
}
