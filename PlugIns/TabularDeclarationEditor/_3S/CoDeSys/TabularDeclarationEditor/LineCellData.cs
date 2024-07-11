namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class LineCellData
	{
		internal readonly int LineNumber;

		internal bool Bookmark;

		internal LineCellData(int nLineNumber, bool bBookmark)
		{
			LineNumber = nLineNumber;
			Bookmark = bBookmark;
		}
	}
}
