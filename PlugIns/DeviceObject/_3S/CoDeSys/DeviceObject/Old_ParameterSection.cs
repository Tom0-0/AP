using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{0E44E39A-C299-41d8-A091-C0466F9448B8}")]
	[StorageVersion("3.3.0.0")]
	public class Old_ParameterSection : GenericObject2
	{
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("Name")]
		[StorageVersion("3.3.0.0")]
		private StringRef _name = new StringRef();

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("Description")]
		[StorageVersion("3.3.0.0")]
		private StringRef _description = new StringRef();

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("Section")]
		[StorageVersion("3.3.0.0")]
		private IParameterSection _section;

		internal IParameterSection Section => _section;

		public Old_ParameterSection()
			: this()
		{
		}
	}
}
