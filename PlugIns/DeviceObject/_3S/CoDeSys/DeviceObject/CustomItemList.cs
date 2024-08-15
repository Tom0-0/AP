using System;
using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{7d23305a-7bb9-46e6-b58b-01ea5694add1}")]
	[StorageVersion("3.3.0.0")]
	public class CustomItemList : GenericObject2, ICustomItemList3, ICustomItemList2, ICustomItemList, IList, ICollection, IEnumerable
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("items")]
		[StorageVersion("3.3.0.0")]
		private ArrayList _alItems = new ArrayList();

		private bool _bReadOnly;

		public ICustomItem this[int nIndex] => (ICustomItem)_alItems[nIndex];

		public bool IsReadOnly
		{
			get
			{
				return _bReadOnly;
			}
			set
			{
				_bReadOnly = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return _alItems[index];
			}
			set
			{
				if (_bReadOnly)
				{
					throw new NotSupportedException("This collection is readonly");
				}
				if (value is ICustomItem)
				{
					_alItems[index] = value;
					return;
				}
				if (value is string)
				{
					_alItems[index] = new CustomItem((string)value);
					return;
				}
				throw new ArgumentException("<value> must either be ICustomItem or a string representing the data of the custom item");
			}
		}

		public bool IsFixedSize => true;

		public bool IsSynchronized => false;

		public int Count => _alItems.Count;

		public object SyncRoot => _alItems.SyncRoot;

		public ICustomItem[] CustomItems
		{
			get
			{
				ICustomItem[] array = (ICustomItem[])(object)new ICustomItem[_alItems.Count];
				_alItems.CopyTo(array);
				return array;
			}
		}

		public CustomItemList()
			: base()
		{
		}

		public CustomItemList(XmlElement xeNode)
			: this()
		{
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xeNode2 = (XmlElement)childNode;
					_alItems.Add(ParameterDataCache.AddCustomItem(new CustomItem(xeNode2)));
					_bReadOnly = true;
				}
			}
		}

		internal CustomItemList(CustomItemList original)
			: this()
		{
			_alItems = new ArrayList(original._alItems.Count);
			foreach (CustomItem alItem in original._alItems)
			{
				_alItems.Add(ParameterDataCache.AddCustomItem(new CustomItem(alItem)));
			}
		}

		public override object Clone()
		{
			CustomItemList customItemList = new CustomItemList(this);
			((GenericObject)customItemList).AfterClone();
			return customItemList;
		}

		public ICustomItem Add(string stData, int nIndex)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			CustomItem item = new CustomItem(stData);
			int index = nIndex;
			if (nIndex == -1)
			{
				index = _alItems.Add(ParameterDataCache.AddCustomItem(item));
			}
			else
			{
				_alItems.Insert(nIndex, ParameterDataCache.AddCustomItem(item));
			}
			return (ICustomItem)_alItems[index];
		}

		public void RemoveAt(int nIndex)
		{
			_alItems.RemoveAt(nIndex);
		}

		public void Remove(ICustomItem item)
		{
			_alItems.Remove(item);
		}

		public int[] FindCustomItems(string stName)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < _alItems.Count; i++)
			{
				if (((CustomItem)_alItems[i]).Name == stName)
				{
					arrayList.Add(i);
				}
			}
			int[] array = new int[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public void Insert(int index, object value)
		{
			if (_bReadOnly)
			{
				throw new NotSupportedException("This collection is readonly");
			}
			_alItems.Insert(index, value);
		}

		public void Remove(object value)
		{
			if (_bReadOnly)
			{
				throw new NotSupportedException("This collection is readonly");
			}
			_alItems.Remove(value);
		}

		public bool Contains(object value)
		{
			return _alItems.Contains(value);
		}

		public void Clear()
		{
			if (_bReadOnly)
			{
				throw new NotSupportedException("This collection is readonly");
			}
			_alItems.Clear();
		}

		public int IndexOf(object value)
		{
			return _alItems.IndexOf(value);
		}

		public int Add(object value)
		{
			if (_bReadOnly)
			{
				throw new NotSupportedException("This collection is readonly");
			}
			if (value is CustomItem)
			{
				return _alItems.Add(ParameterDataCache.AddCustomItem(value as CustomItem));
			}
			return _alItems.Add(value);
		}

		public void CopyTo(Array array, int index)
		{
			_alItems.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alItems.GetEnumerator();
		}

		public int[] FindCustomItems(string stName, string stNameSpaceURI)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < _alItems.Count; i++)
			{
				object obj = _alItems[i];
				ICustomItem val = (ICustomItem)((obj is ICustomItem) ? obj : null);
				string text = val.Name;
				if (text.Contains(":"))
				{
					int startIndex = text.LastIndexOf(':') + 1;
					text = text.Substring(startIndex);
				}
				if (!(text == stName))
				{
					continue;
				}
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(val.Data);
					if (xmlDocument.DocumentElement.NamespaceURI.Equals(stNameSpaceURI))
					{
						arrayList.Add(i);
					}
				}
				catch
				{
				}
			}
			int[] array = new int[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}
	}
}
