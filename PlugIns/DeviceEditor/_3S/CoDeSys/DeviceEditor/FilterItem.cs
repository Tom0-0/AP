namespace _3S.CoDeSys.DeviceEditor
{
	public class FilterItem
	{
		public IIoMappingEditorFilterFactory MappingFilter { get; set; }

		public FilterItem(IIoMappingEditorFilterFactory filter)
		{
			MappingFilter = filter;
		}

		public override string ToString()
		{
			if (MappingFilter == null)
			{
				return Strings.IoFilterShowAll;
			}
			return MappingFilter.FilterName;
		}
	}
}
