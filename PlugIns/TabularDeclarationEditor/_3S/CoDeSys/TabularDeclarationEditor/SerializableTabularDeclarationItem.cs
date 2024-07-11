using System.Collections.Generic;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{EF25D87B-D429-44ba-A384-FD430A4A7F44}")]
	[StorageVersion("3.4.0.0")]
	internal class SerializableTabularDeclarationItem : GenericObject2
	{
		[DefaultSerialization("Tokens")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private SerializableModelToken[] _tokens;

		[DefaultSerialization("Scope")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private long _nScope;

		[DefaultSerialization("Constant")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private bool _bConstant;

		[DefaultSerialization("Retain")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private bool _bRetain;

		[DefaultSerialization("Persistent")]
		[StorageVersion("3.4.0.0")]
		//[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private bool _bPersistent;

		internal SerializableModelToken[] Tokens => _tokens;

		internal ModelTokenType Scope
		{
			get
			{
				return (ModelTokenType)_nScope;
			}
			set
			{
				SerializableModelToken[] tokens = _tokens;
				foreach (SerializableModelToken serializableModelToken in tokens)
				{
					if (serializableModelToken.Type == Scope)
					{
						serializableModelToken.Type = value;
						serializableModelToken.Text = Common.GetScopeText(value, bConstant: false, bRetain: false, bPersistent: false);
						break;
					}
				}
				_nScope = (long)value;
			}
		}

		internal bool Constant => _bConstant;

		internal bool Retain => _bRetain;

		internal bool Persistent => _bPersistent;

		public SerializableTabularDeclarationItem()
			: base()
		{
		}

		internal SerializableTabularDeclarationItem(TabularDeclarationItem item)
			: this()
		{
			ModelTokenRange modelTokenRange = item.CalculateLineBoundaryRange();
			LList<SerializableModelToken> val = new LList<SerializableModelToken>();
			for (LinkedListNode<ModelToken> linkedListNode = modelTokenRange.First; linkedListNode != modelTokenRange.Next; linkedListNode = linkedListNode.Next)
			{
				val.Add(new SerializableModelToken(linkedListNode.Value));
			}
			_tokens = val.ToArray();
			_nScope = (long)item.Block.Scope;
			_bConstant = item.Block.Constant;
			_bRetain = item.Block.Retain;
			_bPersistent = item.Block.Persistent;
		}
	}
}
