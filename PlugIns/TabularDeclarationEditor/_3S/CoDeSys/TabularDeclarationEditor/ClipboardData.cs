using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{88EF488C-69B0-4a9d-B0BB-AB70A0A056CC}")]
	[StorageVersion("3.4.0.0")]
	internal class ClipboardData : GenericObject2
	{
		[DefaultSerialization("Items")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private SerializableTabularDeclarationItem[] _items;

		[DefaultSerialization("Names")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private string[] _names;

		public SerializableTabularDeclarationItem[] Items => _items;

		public string[] Names => _names;

		public ClipboardData()
			: base()
		{
		}

		public ClipboardData(SerializableTabularDeclarationItem[] items, string[] names)
			: this()
		{
			_items = items;
			_names = names;
		}
	}
}
