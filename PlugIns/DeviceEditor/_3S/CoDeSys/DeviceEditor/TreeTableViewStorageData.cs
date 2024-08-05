using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{3EE6DCA2-A720-4F75-B0E0-C5904C69BA56}")]
	[StorageVersion("3.5.10.90-3.5.10.255;3.5.11.50")]
	public class TreeTableViewStorageData : GenericObject2
	{
		[DefaultSerialization("ObjectGuid")]
		[StorageVersion("3.5.10.90-3.5.10.255;3.5.11.50")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private Guid _objectGuid;

		[DefaultSerialization("IdentificationGuid")]
		[StorageVersion("3.5.10.90-3.5.10.255;3.5.11.50")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private Guid _identificationGuid;

		[DefaultSerialization("DictData")]
		[StorageVersion("3.5.10.90-3.5.10.255;3.5.11.50")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private Hashtable _dictData = new Hashtable();

		internal Hashtable DictData => _dictData;

		internal Guid ObjectGuid => _objectGuid;

		internal Guid IdentificationGuid => _identificationGuid;

		public TreeTableViewStorageData()
			: base()
		{
		}

		internal TreeTableViewStorageData(Guid objectGuid, Guid identificationGuid)
			: this()
		{
			_objectGuid = objectGuid;
			_identificationGuid = identificationGuid;
		}
	}
}
