using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{7E01CF3C-2350-4b44-B59D-447C3B632DDD}")]
	[StorageVersion("3.4.2.0")]
	public class LogicalApplicationProperty : GenericObject2, ILogicalApplicationProperty, IObjectProperty, IGenericObject, IArchivable, ICloneable, IComparable
	{
		public static readonly Guid My_Guid = new Guid("{7E01CF3C-2350-4b44-B59D-447C3B632DDD}");

		[DefaultSerialization("DeviceIdentification")]
		[StorageVersion("3.4.2.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private Guid _applicationGuid = Guid.Empty;

		public Guid LogicalApplication => _applicationGuid;

		public LogicalApplicationProperty(Guid applicationGuid)
			: this()
		{
			_applicationGuid = applicationGuid;
		}

		public LogicalApplicationProperty()
			: this()
		{
		}
	}
}
