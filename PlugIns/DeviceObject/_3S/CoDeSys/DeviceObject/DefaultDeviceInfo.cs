using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{59a90934-c888-40db-a9df-306fcf36d75c}")]
	[StorageVersion("3.3.0.0")]
	public class DefaultDeviceInfo : GenericObject2, IDeviceInfo2, IDeviceInfo
	{
		[DefaultSerialization("Name")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public string _stName;

		[DefaultSerialization("Description")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public string _stDescription;

		[DefaultSerialization("Vendor")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public string _stVendor;

		[DefaultSerialization("OrderNumber")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public string _stOrderNumber;

		[DefaultSerialization("Categories")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		public int[] _categories = new int[0];

		[DefaultSerialization("Families")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		public string[] _families = new string[0];

		[DefaultSerialization("Custom")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public string _stCustom;

		[DefaultSerialization("DefaultInstanceName")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		public string _stDefaultInstanceName;

		public string Name => _stName;

		public string Description => _stDescription;

		public string Vendor => _stVendor;

		public string OrderNumber => _stOrderNumber;

		public int[] Categories => _categories;

		public string[] Families => _families;

		public string Custom => _stCustom;

		public Icon Icon => null;

		public Image Image => null;

		public IRepositoryFileReferenceCollection AdditionalFiles => (IRepositoryFileReferenceCollection)(object)new EmptyFileReferenceCollection();

		public string DefaultInstanceName => _stDefaultInstanceName;

		public DefaultDeviceInfo()
			: base()
		{
		}

		internal DefaultDeviceInfo(IDeviceInfo info)
			: this()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			_stCustom = info.Custom;
			_stDescription = info.Description;
			_stName = info.Name;
			_stOrderNumber = info.OrderNumber;
			_stVendor = info.Vendor;
			_categories = info.Categories;
			_families = info.Families;
			if (info is IDeviceInfo2)
			{
				_stDefaultInstanceName = ((IDeviceInfo2)info).DefaultInstanceName;
			}
		}

		public override object Clone()
		{
			DefaultDeviceInfo defaultDeviceInfo = new DefaultDeviceInfo((IDeviceInfo)(object)this);
			((GenericObject)defaultDeviceInfo).AfterClone();
			return defaultDeviceInfo;
		}
	}
}
