using System;
using System.Xml;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ChildObject
	{
		private string _stName;

		private Guid _guid;

		private Guid _parentGuid;

		public string Name
		{
			get
			{
				return _stName;
			}
			set
			{
				_stName = value;
			}
		}

		public Guid Guid
		{
			get
			{
				return _guid;
			}
			set
			{
				_guid = value;
			}
		}

		public Guid ParentGuid
		{
			get
			{
				return _parentGuid;
			}
			set
			{
				_parentGuid = value;
			}
		}

		public ChildObject(XmlNode xnChildObject, Guid parentGuid)
		{
			string text = null;
			string text2 = null;
			_parentGuid = parentGuid;
			foreach (XmlNode childNode in xnChildObject.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				string name = childNode.Name;
				if (!(name == "ObjectGuid"))
				{
					if (name == "ObjectName")
					{
						text2 = childNode.InnerText;
					}
				}
				else
				{
					text = childNode.InnerText;
				}
			}
			if (text == null)
			{
				throw new XmlException("Missing nodes ObjectGuid");
			}
			if (text2 == null)
			{
				throw new XmlException("Missing nodes ObjectName");
			}
			_guid = new Guid(text);
			_stName = text2;
			_parentGuid = parentGuid;
		}
	}
}
