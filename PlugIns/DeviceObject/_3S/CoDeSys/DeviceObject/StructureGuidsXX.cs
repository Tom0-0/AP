#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{2094eb0d-1229-49f2-8194-1b4c49a4dae3}")]
	[StorageVersion("3.3.0.0")]
	public class StructureGuidsXX : GenericObject2
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Guids")]
		[StorageVersion("3.3.0.0")]
		private Hashtable _htGuids = new Hashtable();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("DeviceGuid")]
		[StorageVersion("3.3.0.0")]
		private Guid _guidDevice;

		private bool _bInitializing;

		public bool Initializing
		{
			get
			{
				return _bInitializing;
			}
			set
			{
				_bInitializing = value;
			}
		}

		public Guid DeviceGuid => _guidDevice;

		public StructureGuidsXX()
		{
		}

		public StructureGuidsXX(Guid guidDevice)
			: this()
		{
			_guidDevice = guidDevice;
		}

		public Guid GetGuidForStructure(string stName)
		{
			if (!_htGuids.Contains(stName))
			{
				_htGuids[stName] = LanguageModelHelper.CreateDeterministicGuid(_guidDevice, stName);
			}
			return (Guid)_htGuids[stName];
		}

		public void CreateGuidForStructure(string stName)
		{
			Debug.Fail("");
		}
	}
}
