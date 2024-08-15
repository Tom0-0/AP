#define DEBUG
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{35cf7b1a-a4cd-490b-8ebe-385e61fcc6d9}")]
	[StorageVersion("3.3.0.0")]
	public class StringRef : GenericObject2, IStringRef
	{
		internal class EqualityComparer : IEqualityComparer<StringRef>
		{
			public bool Equals(StringRef x, StringRef y)
			{
				if (x == y)
				{
					return true;
				}
				if (string.Equals(x._stIdentifier, y._stIdentifier) && string.Equals(x._stNamespace, y._stNamespace))
				{
					return string.Equals(x._stDefault, y._stDefault);
				}
				return false;
			}

			public int GetHashCode(StringRef x)
			{
				return x._stIdentifier.GetHashCode() ^ x._stNamespace.GetHashCode() ^ x._stDefault.GetHashCode();
			}
		}

		private static readonly char[] NAMESPACE_DELIMITER = new char[1] { ':' };

		public static readonly StringRef Empty = new StringRef();

		[DefaultSerialization("Namespace")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private string _stNamespace = "";

		[DefaultSerialization("Identifier")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private string _stIdentifier = "";

		[DefaultSerialization("Default")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private string _stDefault = "";

		public string Default
		{
			get
			{
				return _stDefault;
			}
			internal set
			{
				_stDefault = value;
			}
		}

		public string Namespace
		{
			get
			{
				return _stNamespace;
			}
			internal set
			{
				_stNamespace = value;
			}
		}

		public string Identifier
		{
			get
			{
				return _stIdentifier;
			}
			internal set
			{
				_stIdentifier = value;
			}
		}

		private void ExtractName(XmlElement xeNode, out string stNamespace, out string stIdentifier)
		{
			string attribute = xeNode.GetAttribute("name");
			if (attribute != null)
			{
				string[] array = attribute.Split(NAMESPACE_DELIMITER);
				if (array.Length == 2)
				{
					stNamespace = string.Intern(array[0]);
					stIdentifier = string.Intern(array[1]);
					return;
				}
			}
			stNamespace = "";
			stIdentifier = "";
		}

		internal bool TestEquals(XmlElement xeNode)
		{
			ExtractName(xeNode, out var stNamespace, out var stIdentifier);
			string text = xeNode.InnerText.Trim();
			if (stNamespace == _stNamespace && stIdentifier == _stIdentifier)
			{
				return _stDefault == text;
			}
			return false;
		}

		internal StringRef(XmlElement xeNode)
			: this()
		{
			ExtractName(xeNode, out _stNamespace, out _stIdentifier);
			_stDefault = string.Intern(xeNode.InnerText.Trim());
		}

		internal StringRef(XmlReader reader)
			: this()
		{
			Debug.Assert(reader.NodeType == XmlNodeType.Element);
			string attribute = reader.GetAttribute("name");
			if (attribute != null)
			{
				string[] array = attribute.Split(NAMESPACE_DELIMITER);
				if (array.Length == 2)
				{
					_stNamespace = string.Intern(array[0]);
					_stIdentifier = string.Intern(array[1]);
				}
				else
				{
					_stNamespace = "";
					_stIdentifier = "";
				}
			}
			_stDefault = string.Intern(reader.ReadElementString().Trim());
		}

		internal StringRef(IStringRef original)
			: this()
		{
			_stNamespace = string.Intern(original.Namespace);
			_stIdentifier = string.Intern(original.Identifier);
			_stDefault = string.Intern(original.Default);
		}

		internal StringRef(string stNamespace, string stIdentifier, string stDefault)
			: this()
		{
			_stNamespace = string.Intern(stNamespace);
			_stIdentifier = string.Intern(stIdentifier);
			_stDefault = string.Intern(stDefault);
		}

		public StringRef()
			: base()
		{
		}

		public override string ToString()
		{
			return _stDefault;
		}

		public override object Clone()
		{
			return ParameterDataCache.AddStringRef(new StringRef(_stNamespace, _stIdentifier, _stDefault));
		}
	}
}
