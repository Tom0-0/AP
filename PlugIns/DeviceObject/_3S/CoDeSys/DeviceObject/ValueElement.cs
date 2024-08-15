using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{bc7beaed-6d8b-44a9-8e86-c58ec672e488}")]
	[StorageVersion("3.5.6.0")]
	public class ValueElement : GenericObject2
	{
		[DefaultSerialization("SubElements")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private ArrayList _alSubElements;

		[DefaultSerialization("Value")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue("")]
		private string _stValue = "";

		[DefaultSerialization("Name")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue("")]
		private string _stName = "";

		private ValueElement _parent;

		private AccessRight _accessRightOffline = (AccessRight)3;

		private AccessRight _accessRightOnline = (AccessRight)3;

		private bool _bHasOnlineAccessRight;

		private bool _bHasOfflineAccessRight;

		public ArrayList SubElements
		{
			get
			{
				if (_alSubElements == null)
				{
					_alSubElements = new ArrayList();
				}
				return _alSubElements;
			}
		}

		public string Value
		{
			get
			{
				return _stValue;
			}
			set
			{
				_stValue = value;
			}
		}

		public string Name => _stName;

		public bool HasOfflineAccessRight
		{
			get
			{
				bool flag = _parent != null && _parent._bHasOfflineAccessRight;
				return _bHasOfflineAccessRight || flag;
			}
		}

		public AccessRight OfflineAccess
		{
			get
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				if (_bHasOfflineAccessRight)
				{
					return _accessRightOffline;
				}
				if (_parent != null)
				{
					_ = _parent._bHasOfflineAccessRight;
				}
				return Parent.OfflineAccess;
			}
		}

		public bool HasOnlineAccessRight
		{
			get
			{
				bool flag = _parent != null && _parent._bHasOnlineAccessRight;
				return _bHasOnlineAccessRight || flag;
			}
		}

		public AccessRight OnlineAccess
		{
			get
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				if (_bHasOnlineAccessRight)
				{
					return _accessRightOnline;
				}
				if (_parent != null)
				{
					_ = _parent._bHasOnlineAccessRight;
				}
				return Parent.OnlineAccess;
			}
		}

		public ValueElement Parent => _parent;

		public ValueElement()
			: this()
		{
		}//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)


		public ValueElement(XmlElement xeNode, TypeList typeList, ValueElement parent)
			: this()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_alSubElements = new ArrayList();
			_parent = parent;
			Init(xeNode, typeList);
		}

		public ValueElement(string stValue)
			: this()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_alSubElements = new ArrayList();
			_stValue = stValue;
		}

		private void Init(XmlElement xeNode, TypeList typeList)
		{
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			_stName = xeNode.GetAttribute("name");
			if (_stName == null)
			{
				_stName = "";
			}
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					string name = childNode.Name;
					if (name == "Element")
					{
						_alSubElements.Add(new ValueElement((XmlElement)childNode, typeList, this));
					}
				}
				else if (childNode.NodeType == XmlNodeType.Text)
				{
					_stValue += ((XmlText)childNode).Value;
				}
				else if (childNode.NodeType == XmlNodeType.CDATA)
				{
					_stValue += ((XmlCDataSection)childNode).Value;
				}
			}
			_stValue = _stValue.Trim();
			_bHasOnlineAccessRight = xeNode.HasAttribute("onlineaccess");
			_bHasOfflineAccessRight = xeNode.HasAttribute("offlineaccess");
			if (_bHasOnlineAccessRight)
			{
				string attribute = xeNode.GetAttribute("onlineaccess");
				_accessRightOnline = Parameter.ParseAccessRight(attribute, _accessRightOnline);
			}
			if (_bHasOfflineAccessRight)
			{
				string attribute2 = xeNode.GetAttribute("offlineaccess");
				_accessRightOffline = Parameter.ParseAccessRight(attribute2, _accessRightOffline);
			}
		}

		public ValueElement GetSubElement(string stName)
		{
			if (_alSubElements != null)
			{
				foreach (ValueElement alSubElement in _alSubElements)
				{
					if (alSubElement.Name == stName)
					{
						return alSubElement;
					}
				}
			}
			return null;
		}

		private void UpdateParents()
		{
			if (_alSubElements == null)
			{
				return;
			}
			foreach (ValueElement alSubElement in _alSubElements)
			{
				alSubElement._parent = this;
			}
		}

		public override void AfterClone()
		{
			((GenericObject)this).AfterClone();
			UpdateParents();
		}

		public override void AfterDeserialize()
		{
			((GenericObject)this).AfterDeserialize();
			UpdateParents();
		}
	}
}
