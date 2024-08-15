using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{F2363587-87F8-455B-8B99-881226EBDFFD}")]
	[StorageVersion("3.3.0.0")]
	public class RequiredExternalEventTask : RequiredTask
	{
		public const string TYPE_OF_TASK = "ExternalEvent";

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("Event")]
		[StorageVersion("3.3.0.0")]
		[StorageDefaultValue(null)]
		protected string _stEvent;

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("CycleTime")]
		[StorageVersion("3.5.6.0")]
		[StorageDefaultValue(null)]
		protected string _stCycleTime;

		internal string Event
		{
			get
			{
				return _stEvent;
			}
			set
			{
				_stEvent = value;
			}
		}

		internal string CycleTime
		{
			get
			{
				return _stCycleTime;
			}
			set
			{
				_stCycleTime = value;
			}
		}

		internal override string TypeOfTask => "ExternalEvent";

		public RequiredExternalEventTask()
		{
		}

		internal RequiredExternalEventTask(XmlElement node)
			: base(node)
		{
			_stEvent = node.GetAttribute("event");
			_stCycleTime = node.GetAttribute("cycletime");
		}

		internal override void SerializeToHashtable(Hashtable htValues)
		{
			base.SerializeToHashtable(htValues);
			htValues["Event"] = _stEvent;
			htValues["CycleTime"] = _stCycleTime;
		}

		internal override void DeserializeFromHashtable(HashtableReader reader)
		{
			_stEvent = (string)reader.Read("Event");
			_stCycleTime = (string)reader.Read("CycleTime");
			base.DeserializeFromHashtable(reader);
		}
	}
}
