using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{334E3D66-6590-4299-B904-8DC9A6EFFE03}")]
	[StorageVersion("3.5.10.90-3.5.10.255;3.5.11.50")]
	public class DeviceEditorColumnStorage : GenericObject2
	{
		[DefaultSerialization("TreeTableViewStorageList")]
		[StorageVersion("3.5.10.90-3.5.10.255;3.5.11.50")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private ArrayList _treeTableViewStorageList = new ArrayList();

		internal ArrayList TreeTableViewStorage => _treeTableViewStorageList;

		internal void AddStorageData(TreeTableViewStorageData data)
		{
			_treeTableViewStorageList.Add(data);
		}

		public DeviceEditorColumnStorage()
			: base()
		{
		}
	}
}
