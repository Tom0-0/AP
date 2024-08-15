using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{58ff2753-3af6-4814-bcc4-9ed585515754}")]
	[StorageVersion("3.3.0.0")]
	public class CyclicCall : GenericObject2, ICyclicCall
	{
		public const string TASKTYPE_EACHTASK = "#eachtask";

		public const string TASKTYPE_BUSCYLCETASK = "#buscycletask";

		public const string TASKTYPE_USERDEFTASK = "#userdeftask";

		[DefaultSerialization("MethodName")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private string _stMethodName = "";

		[DefaultSerialization("WhenToCall")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private string _stWhenToCall = "";

		[DefaultSerialization("Task")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private string _stTask = "#eachtask";

		public string MethodName
		{
			get
			{
				return _stMethodName;
			}
			set
			{
				_stMethodName = value;
			}
		}

		public string WhenToCall
		{
			get
			{
				return _stWhenToCall;
			}
			set
			{
				_stWhenToCall = value;
			}
		}

		public string Task
		{
			get
			{
				return _stTask;
			}
			set
			{
				_stTask = value;
			}
		}

		public CyclicCall()
			: this()
		{
		}

		public CyclicCall(XmlElement xeNode)
			: this()
		{
			_stMethodName = DeviceObjectHelper.ParseString(xeNode.GetAttribute("methodname"), "");
			_stWhenToCall = DeviceObjectHelper.ParseString(xeNode.GetAttribute("whentocall"), "");
			_stTask = DeviceObjectHelper.ParseString(xeNode.GetAttribute("task"), "#eachtask");
		}

		public CyclicCall(CyclicCall original)
			: this()
		{
			_stMethodName = original._stMethodName;
			_stWhenToCall = original._stWhenToCall;
			_stTask = original._stTask;
		}

		public override object Clone()
		{
			CyclicCall cyclicCall = new CyclicCall(this);
			((GenericObject)cyclicCall).AfterClone();
			return cyclicCall;
		}
	}
}
