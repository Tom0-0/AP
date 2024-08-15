using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{BC366095-B03E-44BE-B7B4-8F89F7638F57}")]
	[StorageVersion("3.3.0.0")]
	public class RequiredCyclicTask : RequiredTask
	{
		public const string TYPE_OF_TASK = "Cyclic";

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("CycleTime")]
		[StorageVersion("3.3.0.0")]
		[StorageDefaultValue(null)]
		protected string _stCycleTime;

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

		internal override string TypeOfTask => "Cyclic";

		public RequiredCyclicTask()
		{
		}

		internal RequiredCyclicTask(XmlElement node)
			: base(node)
		{
			_stCycleTime = node.GetAttribute("cycletime");
		}

		internal override void SerializeToHashtable(Hashtable htValues)
		{
			base.SerializeToHashtable(htValues);
			htValues["CycleTime"] = _stCycleTime;
		}

		internal override void DeserializeFromHashtable(HashtableReader reader)
		{
			_stCycleTime = (string)reader.Read("CycleTime");
			base.DeserializeFromHashtable(reader);
		}
	}
}
