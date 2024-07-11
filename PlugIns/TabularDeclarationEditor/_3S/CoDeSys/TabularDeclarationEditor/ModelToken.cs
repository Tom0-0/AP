namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class ModelToken
	{
		internal ModelTokenType Type;

		internal string Text;

		internal int Offset;

		internal ModelToken(ModelTokenType type, string stText)
			: this(type, stText, -1)
		{
		}

		internal ModelToken(ModelTokenType type, string stText, int nOffset)
		{
			Type = type;
			Text = stText;
			Offset = nOffset;
		}

		internal bool HasType(ModelTokenType type)
		{
			return (Type & type) != 0;
		}
	}
}
