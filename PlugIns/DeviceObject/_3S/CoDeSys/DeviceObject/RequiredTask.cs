using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{3E66182A-53E1-4AAF-91DB-C6EA7070EDC0}")]
	[StorageVersion("3.3.0.0")]
	public abstract class RequiredTask : GenericObject2
	{
		[DefaultSerialization("Priority")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(0)]
		protected int _nPriority;

		[DefaultSerialization("Name")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		protected string _stTaskName;

		[DefaultSerialization("Implicit")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		protected bool _bImplicit;

		[DefaultSerialization("WithinSPSTimeSlicing")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		protected bool _bWithinSPSTimeSlicing;

		[DefaultSerialization("TaskPou")]
		[StorageVersion("3.3.2.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageIgnorable]
		protected ArrayList _alTaskPou = new ArrayList();

		[DefaultSerialization("watchdogEnabled")]
		[StorageVersion("3.4.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		protected bool _bWatchdogEnabled;

		[DefaultSerialization("watchdogTime")]
		[StorageVersion("3.4.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("")]
		protected string _stWatchdogTime = string.Empty;

		[DefaultSerialization("watchdogTimeUnit")]
		[StorageVersion("3.4.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("")]
		protected string _stWatchdogTimeUnit = string.Empty;

		[DefaultSerialization("watchdogSensitivity")]
		[StorageVersion("3.4.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("")]
		protected string _stWatchdogSensitivity = string.Empty;

		[DefaultSerialization("DeviceApplicationOnly")]
		[StorageVersion("3.5.10.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		protected bool _bDeviceApplicationOnly;

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected Hashtable _htUnknownValues;

		internal abstract string TypeOfTask { get; }

		public int Priority
		{
			get
			{
				return _nPriority;
			}
			set
			{
				_nPriority = value;
			}
		}

		public string TaskName
		{
			get
			{
				return _stTaskName;
			}
			set
			{
				_stTaskName = value;
			}
		}

		public bool Implicit
		{
			get
			{
				return _bImplicit;
			}
			set
			{
				_bImplicit = value;
			}
		}

		public bool WithinSPSTimeSlicing
		{
			get
			{
				return _bWithinSPSTimeSlicing;
			}
			set
			{
				_bWithinSPSTimeSlicing = value;
			}
		}

		public ArrayList TaskPou => _alTaskPou;

		public bool WatchdogEnabled
		{
			get
			{
				return _bWatchdogEnabled;
			}
			set
			{
				_bWatchdogEnabled = value;
			}
		}

		public string WatchdogTime
		{
			get
			{
				return _stWatchdogTime;
			}
			set
			{
				_stWatchdogTime = value;
			}
		}

		public string WatchdogTimeUnit
		{
			get
			{
				return _stWatchdogTimeUnit;
			}
			set
			{
				_stWatchdogTimeUnit = value;
			}
		}

		public string WatchdogSensitivity
		{
			get
			{
				return _stWatchdogSensitivity;
			}
			set
			{
				_stWatchdogSensitivity = value;
			}
		}

		public bool DeviceApplicationOnly
		{
			get
			{
				return _bDeviceApplicationOnly;
			}
			set
			{
				_bDeviceApplicationOnly = value;
			}
		}

		public RequiredTask()
			: base()
		{
		}

		internal RequiredTask(XmlElement node)
			: this()
		{
			_nPriority = DeviceObjectHelper.ParseInt(node.GetAttribute("priority"), 0);
			_stTaskName = node.GetAttribute("taskname");
			_bImplicit = DeviceObjectHelper.ParseBool(node.GetAttribute("implicit"), bDefault: false);
			_bWithinSPSTimeSlicing = DeviceObjectHelper.ParseBool(node.GetAttribute("withinspstimeslicing"), bDefault: true);
			_bWatchdogEnabled = DeviceObjectHelper.ParseBool(node.GetAttribute("watchdogEnabled"), bDefault: false);
			_stWatchdogTime = DeviceObjectHelper.ParseString(node.GetAttribute("watchdogTime"), string.Empty);
			_stWatchdogTimeUnit = DeviceObjectHelper.ParseString(node.GetAttribute("watchdogTimeUnit"), string.Empty);
			_stWatchdogSensitivity = DeviceObjectHelper.ParseString(node.GetAttribute("watchdogSensitivity"), string.Empty);
			_bDeviceApplicationOnly = DeviceObjectHelper.ParseBool(node.GetAttribute("deviceApplicationOnly"), bDefault: false);
			if (_stTaskName == null)
			{
				DeviceMessage deviceMessage = new DeviceMessage(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ErrorMissingTaskName"), (Severity)4);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
			}
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (childNode.Name == "taskpou")
				{
					_alTaskPou.Add(childNode.InnerText.Trim());
				}
			}
		}

		internal virtual void SerializeToHashtable(Hashtable htValues)
		{
			if (_htUnknownValues != null)
			{
				foreach (DictionaryEntry htUnknownValue in _htUnknownValues)
				{
					htValues.Add(htUnknownValue.Key, htUnknownValue.Value);
				}
			}
			htValues["Priority"] = _nPriority;
			htValues["Name"] = _stTaskName;
			htValues["Implicit"] = _bImplicit;
			htValues["WithinSPSTimeSlicing"] = _bWithinSPSTimeSlicing;
			htValues["TaskPou"] = _alTaskPou;
			htValues["watchdogEnabled"] = _bWatchdogEnabled;
			htValues["watchdogTime"] = _stWatchdogTime;
			htValues["watchdogTimeUnit"] = _stWatchdogTimeUnit;
			htValues["watchdogSensitivity"] = _stWatchdogSensitivity;
			htValues["DeviceApplicationOnly"] = _bDeviceApplicationOnly;
		}

		internal virtual void DeserializeFromHashtable(HashtableReader reader)
		{
			if (reader.Contains("Priority"))
			{
				_nPriority = (int)reader.Read("Priority");
			}
			else
			{
				_nPriority = 0;
			}
			if (reader.Contains("Name"))
			{
				_stTaskName = (string)reader.Read("Name");
			}
			else
			{
				_stTaskName = string.Empty;
			}
			if (reader.Contains("Implicit"))
			{
				_bImplicit = (bool)reader.Read("Implicit");
			}
			else
			{
				_bImplicit = false;
			}
			if (reader.Contains("WithinSPSTimeSlicing"))
			{
				_bWithinSPSTimeSlicing = (bool)reader.Read("WithinSPSTimeSlicing");
			}
			else
			{
				_bWithinSPSTimeSlicing = false;
			}
			if (reader.Contains("TaskPou"))
			{
				_alTaskPou = (ArrayList)reader.Read("TaskPou");
			}
			if (reader.Contains("watchdogEnabled"))
			{
				_bWatchdogEnabled = (bool)reader.Read("watchdogEnabled");
			}
			if (reader.Contains("watchdogTime"))
			{
				_stWatchdogTime = (string)reader.Read("watchdogTime");
			}
			if (reader.Contains("watchdogTimeUnit"))
			{
				_stWatchdogTimeUnit = (string)reader.Read("watchdogTimeUnit");
			}
			if (reader.Contains("watchdogSensitivity"))
			{
				_stWatchdogSensitivity = (string)reader.Read("watchdogSensitivity");
			}
			if (reader.Contains("DeviceApplicationOnly"))
			{
				_bDeviceApplicationOnly = (bool)reader.Read("DeviceApplicationOnly");
			}
			if (reader.UnreadValues.Count > 0)
			{
				_htUnknownValues = new Hashtable(reader.UnreadValues);
			}
			else
			{
				_htUnknownValues = null;
			}
		}
	}
}
