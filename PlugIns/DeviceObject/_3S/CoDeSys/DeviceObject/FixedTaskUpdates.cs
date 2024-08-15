using System.Collections;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class FixedTaskUpdates
	{
		private ITaskInfo[] _taskinfos;

		private ArrayList[] inputs;

		private ArrayList[] outputs;

		internal FixedTaskUpdates(ITaskInfo[] taskinfos)
		{
			inputs = new ArrayList[taskinfos.Length];
			outputs = new ArrayList[taskinfos.Length];
			for (int i = 0; i < taskinfos.Length; i++)
			{
				inputs[i] = new ArrayList();
				outputs[i] = new ArrayList();
			}
			_taskinfos = taskinfos;
		}

		public void Add(FixedTaskUpdate ftu)
		{
			for (int i = 0; i < _taskinfos.Length; i++)
			{
				if (_taskinfos[i].TaskName.Equals(ftu.Task))
				{
					if (ftu.ChannelMap.IsInput)
					{
						inputs[i].Add(ftu);
					}
					else
					{
						outputs[i].Add(ftu);
					}
					break;
				}
			}
		}

		public FixedTaskUpdate[] GetFixedUpdatesForTask(byte byTask, bool bInput)
		{
			ArrayList arrayList = ((!bInput) ? outputs[byTask] : inputs[byTask]);
			FixedTaskUpdate[] array = new FixedTaskUpdate[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}
	}
}
