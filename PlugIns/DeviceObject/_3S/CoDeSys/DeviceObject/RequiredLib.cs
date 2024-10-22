using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{94d8f601-f8e4-4aaa-8f48-f3c3ec33bec1}")]
	[StorageVersion("3.3.0.0")]
	public class RequiredLib : GenericObject2, IRequiredLib4, IRequiredLib3, IRequiredLib2, IRequiredLib
	{
		[DefaultSerialization("LibName")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stLibName = "";

		[DefaultSerialization("Vendor")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stVendor = "";

		[DefaultSerialization("Version")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stVersion = "";

		[DefaultSerialization("Identifier")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stIdentifier = "";

		[DefaultSerialization("FbInstances")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private ArrayList _alFbInstances = new ArrayList();

		[DefaultSerialization("PlaceHolderLib")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stPlaceHolderLib = "";

		[DefaultSerialization("loadAsSystemLibrary")]
		[StorageVersion("3.3.2.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(true)]
		private bool _bLoadAsSystemLibrary = true;

		[DefaultSerialization("IsDiagnosisLib")]
		[StorageVersion("3.5.0.0")]
		[StorageDefaultValue(false)]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bIsDiagnosisLib;

		[DefaultSerialization("AddedByAP")]
		[StorageVersion("3.5.13.0")]
		[StorageDefaultValue(false)]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bAddedByAP;

		[DefaultSerialization("Client")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stClient = "";

		internal bool IsDiagnosisLib
		{
			get
			{
				return _bIsDiagnosisLib;
			}
			set
			{
				_bIsDiagnosisLib = value;
			}
		}

		internal bool AddedByAP
		{
			get
			{
				return _bAddedByAP;
			}
			set
			{
				_bAddedByAP = value;
			}
		}

		public string LibName
		{
			get
			{
				return _stLibName;
			}
			set
			{
				_stLibName = value;
			}
		}

		public string Vendor
		{
			get
			{
				return _stVendor;
			}
			set
			{
				_stVendor = value;
			}
		}

		public string Version
		{
			get
			{
				if (string.IsNullOrEmpty(_stVersion))
				{
					return "*";
				}
				return _stVersion;
			}
			set
			{
				_stVersion = value;
			}
		}

		public string Identifier
		{
			get
			{
				return _stIdentifier;
			}
			set
			{
				_stIdentifier = value;
			}
		}

		public IFbInstanceList FbInstances => (IFbInstanceList)(object)new FbInstanceList(_alFbInstances);

		public string PlaceHolderLib
		{
			get
			{
				return _stPlaceHolderLib;
			}
			set
			{
				_stPlaceHolderLib = value;
			}
		}

		public bool LoadAsSystemLibrary
		{
			get
			{
				return _bLoadAsSystemLibrary;
			}
			set
			{
				_bLoadAsSystemLibrary = true;
			}
		}

		internal ArrayList AlFbInstances => _alFbInstances;

		public string Client => _stClient;

		public RequiredLib()
		{
		}

		public RequiredLib(XmlElement xeNode)
			: this()
		{
			Import(xeNode, bUpdate: false);
		}

		internal void Update(XmlElement xmlElement)
		{
			Import(xmlElement, bUpdate: true);
		}

		public RequiredLib(RequiredLib original)
			: this()
		{
			_stLibName = original._stLibName;
			_stVendor = original._stVendor;
			_stVersion = original._stVersion;
			_stIdentifier = original._stIdentifier;
			_stClient = original._stClient;
			_stPlaceHolderLib = original._stPlaceHolderLib;
			_bLoadAsSystemLibrary = original._bLoadAsSystemLibrary;
			_bIsDiagnosisLib = original._bIsDiagnosisLib;
			_bAddedByAP = original._bAddedByAP;
			_alFbInstances = new ArrayList(original._alFbInstances.Count);
			foreach (FBInstance alFbInstance in original._alFbInstances)
			{
				_alFbInstances.Add(((GenericObject)alFbInstance).Clone());
			}
		}

		public override object Clone()
		{
			RequiredLib requiredLib = new RequiredLib(this);
			((GenericObject)requiredLib).AfterClone();
			return requiredLib;
		}

		private void Import(XmlElement xeNode, bool bUpdate)
		{
			_stLibName = DeviceObjectHelper.ParseString(xeNode.GetAttribute("libname"), "");
			_stVendor = DeviceObjectHelper.ParseString(xeNode.GetAttribute("vendor"), "");
			_stVersion = DeviceObjectHelper.ParseString(xeNode.GetAttribute("version"), "*");
			_stIdentifier = DeviceObjectHelper.ParseString(xeNode.GetAttribute("identifier"), "");
			_stClient = DeviceObjectHelper.ParseString(xeNode.GetAttribute("client"), "");
			_stPlaceHolderLib = DeviceObjectHelper.ParseString(xeNode.GetAttribute("placeholderlib"), "");
			_bLoadAsSystemLibrary = DeviceObjectHelper.ParseBool(xeNode.GetAttribute("loadAsSystemLibrary"), bDefault: true);
			ArrayList arrayList = new ArrayList();
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				XmlElement xmlElement = (XmlElement)childNode;
				string name = xmlElement.Name;
				if (!(name == "FBInstance"))
				{
					continue;
				}
				FBInstance fBInstance = new FBInstance(xmlElement);
				if (bUpdate)
				{
					FBInstance fBInstance2 = FindMatchingFbInstance(fBInstance);
					if (fBInstance2 != null)
					{
						fBInstance2.Update(xmlElement);
						fBInstance = fBInstance2;
					}
				}
				arrayList.Add(fBInstance);
			}
			_alFbInstances = arrayList;
		}

		private FBInstance FindMatchingFbInstance(FBInstance instToMatch)
		{
			foreach (FBInstance alFbInstance in _alFbInstances)
			{
				if (alFbInstance.FbName == instToMatch.FbName && alFbInstance.BaseName == instToMatch.BaseName)
				{
					return alFbInstance;
				}
			}
			return null;
		}
	}
}
