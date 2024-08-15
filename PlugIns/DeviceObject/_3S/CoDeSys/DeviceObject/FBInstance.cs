using System;
using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{1a6d9caa-30e2-4c6c-aa84-7bcee3627153}")]
	[StorageVersion("3.3.0.0")]
	public class FBInstance : GenericObject2, IFbInstance5, IFbInstance4, IFbInstance3, IFbInstance2, IFbInstance
	{
		public const string DEFAULT_BASENAME = "$(DeviceName)";

		[DefaultSerialization("FbName")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stFbName = "";

		[DefaultSerialization("BaseName")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stBaseName = "$(DeviceName)";

		[DefaultSerialization("Instance")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private VariableMapping _instance = new VariableMapping();

		[DefaultSerialization("InitMethod")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stInitMethod = "";

		[DefaultSerialization("CyclicCalls")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private ArrayList _alCyclicCalls = new ArrayList();

		[DefaultSerialization("Location")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private InstanceLocation _location;

		[DefaultSerialization("FbNameDiag")]
		[StorageVersion("3.5.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("")]
		private string _stFbNameDiag = "";

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("PositionId")]
		[StorageVersion("3.5.13.0")]
		[StorageDefaultValue(-1L)]
		private long _lLanguageModelPositionId = -1L;

		public long LanguageModelPositionId
		{
			get
			{
				return _lLanguageModelPositionId;
			}
			internal set
			{
				_lLanguageModelPositionId = value;
			}
		}

		public string FbName => _stFbName;

		public string BaseName
		{
			get
			{
				return _stBaseName;
			}
			set
			{
				_stBaseName = value;
			}
		}

		public IVariableMapping Instance => (IVariableMapping)(object)_instance;

		public string InitMethodName
		{
			get
			{
				return _stInitMethod;
			}
			set
			{
				_stInitMethod = value;
			}
		}

		public ICyclicCallsList CyclicCalls => (ICyclicCallsList)(object)new CyclicCallsList(_alCyclicCalls);

		public InstanceLocation Location => _location;

		public string FbNameDiag
		{
			get
			{
				return _stFbNameDiag;
			}
			set
			{
				_stFbNameDiag = value;
			}
		}

		public FBInstance()
			: base()
		{
		}

		public FBInstance(XmlElement xeNode)
			: this()
		{
			Import(xeNode, bUpdate: false);
		}

		public FBInstance(FBInstance original)
			: this()
		{
			_stFbName = original._stFbName;
			_stBaseName = original._stBaseName;
			_instance = (VariableMapping)((GenericObject)original._instance).Clone();
			_stInitMethod = original._stInitMethod;
			_alCyclicCalls = new ArrayList(original._alCyclicCalls.Count);
			foreach (CyclicCall alCyclicCall in original._alCyclicCalls)
			{
				_alCyclicCalls.Add(((GenericObject)alCyclicCall).Clone());
			}
			_location = original._location;
			_stFbNameDiag = original._stFbNameDiag;
			_lLanguageModelPositionId = original._lLanguageModelPositionId;
		}

		public override object Clone()
		{
			FBInstance fBInstance = new FBInstance(this);
			((GenericObject)fBInstance).AfterClone();
			return fBInstance;
		}

		public void SetFbType(string stFbType)
		{
			_stFbName = stFbType;
		}

		public string ResetInstanceName(IMetaObject moDevice, IMetaObject moApplication)
		{
			return ResetInstanceName(moDevice, moApplication, null);
		}

		public string ResetInstanceName(IMetaObject moDevice, IMetaObject moApplication, IRequiredLibsList requiredlibs, IIoProvider ioProvider = null)
		{
			IPrecompileScope val = null;
			if (moApplication != null)
			{
				IPreCompileContext precompileContext = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetPrecompileContext(moApplication.ObjectGuid);
				if (precompileContext != null)
				{
					val = precompileContext.CreatePrecompileScope(Guid.Empty);
				}
			}
			string baseName = DeviceObjectHelper.GetBaseName(_stBaseName, moDevice.Name);
			string variable = _instance.Variable;
			_instance.Variable = baseName;
			if (val != null)
			{
				int num = 0;
				IVariable val2 = default(IVariable);
				ISignature val3 = default(ISignature);
				IPrecompileScope val4 = default(IPrecompileScope);
				while (_instance.Variable != variable && val.FindDeclaration(_instance.Variable, out val2, out val3, out val4) && val2 != null)
				{
					if (ioProvider != null)
					{
						IMetaObject metaObject = ioProvider.GetMetaObject();
						if (metaObject != null && val2 is IVariable5 && ((IVariable5)((val2 is IVariable5) ? val2 : null)).MessageGuid == metaObject.ObjectGuid)
						{
							break;
						}
					}
					if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)6, (ushort)40) && val2.SourcePosition != null && val2.SourcePosition.ObjectGuid == Guid.Empty && ((object)val2.Type).ToString().Equals("DED.CAADiagDeviceDefault"))
					{
						break;
					}
					if (requiredlibs != null && val2.SourcePosition != null && val2.SourcePosition.ObjectGuid == moDevice.ObjectGuid)
					{
						foreach (RequiredLib item in (IEnumerable)requiredlibs)
						{
							foreach (FBInstance item2 in (IEnumerable)item.FbInstances)
							{
								if (item2 != this && item2.Instance.Variable == _instance.Variable)
								{
									num++;
									_instance.Variable = baseName + "_" + num;
								}
							}
						}
						break;
					}
					num++;
					_instance.Variable = baseName + "_" + num;
				}
			}
			return _instance.Variable;
		}

		private InstanceLocation ParseLocation(string st, InstanceLocation locDefault)
		{
			switch (st)
			{
			case null:
			case "":
				return locDefault;
			case "gvl":
				return InstanceLocation.GVL;
			case "input":
				return InstanceLocation.Input;
			case "output":
				return InstanceLocation.Output;
			case "retain":
				return InstanceLocation.Retain;
			default:
				return locDefault;
			}
		}

		internal void Update(XmlElement xeChild)
		{
			Import(xeChild, bUpdate: true);
		}

		private void Import(XmlElement xeNode, bool bUpdate)
		{
			ArrayList arrayList = new ArrayList();
			if (bUpdate)
			{
				_stInitMethod = "";
			}
			_stFbName = DeviceObjectHelper.ParseString(xeNode.GetAttribute("fbname"), "");
			_stFbNameDiag = DeviceObjectHelper.ParseString(xeNode.GetAttribute("fbnamediag"), "");
			_stBaseName = DeviceObjectHelper.ParseString(xeNode.GetAttribute("basename"), _stBaseName);
			_location = ParseLocation(xeNode.GetAttribute("location"), InstanceLocation.GVL);
			_instance.CreateVariable = true;
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				XmlElement xmlElement = (XmlElement)childNode;
				string name = xmlElement.Name;
				if (!(name == "Initialize"))
				{
					if (name == "CyclicCall")
					{
						arrayList.Add(new CyclicCall(xmlElement));
					}
				}
				else
				{
					_stInitMethod = DeviceObjectHelper.ParseString(xmlElement.GetAttribute("methodName"), "");
				}
			}
			_alCyclicCalls = arrayList;
		}

		public void SetFbName(string stFbName)
		{
			_stFbName = stFbName;
		}
	}
}
