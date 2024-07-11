using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{AC338FBF-8726-4259-B337-555755C0988C}")]
	[StorageVersion("3.4.0.0")]
	internal class SerializableModelToken : GenericObject2
	{
		[DefaultSerialization("Type")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private long _nType;

		[DefaultSerialization("Text")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private string _stText;

		internal ModelTokenType Type
		{
			get
			{
				return (ModelTokenType)_nType;
			}
			set
			{
				_nType = (long)value;
			}
		}

		internal string Text
		{
			get
			{
				return _stText;
			}
			set
			{
				_stText = value;
			}
		}

		public SerializableModelToken()
			: base()
		{
		}

		internal SerializableModelToken(ModelToken token)
			: this()
		{
			_nType = (long)token.Type;
			_stText = token.Text;
		}
	}
}
