namespace _3S.CoDeSys.DeviceEditor
{
	internal class EditorInfo
	{
		private IBaseDeviceEditor _editor;

		private IEditorPage[] _pages;

		private bool _bShowEditor;

		public IBaseDeviceEditor Editor
		{
			get
			{
				return _editor;
			}
			set
			{
				_editor = value;
			}
		}

		public IEditorPage[] Pages
		{
			get
			{
				return _pages;
			}
			set
			{
				_pages = value;
			}
		}

		public bool ShowEditor
		{
			get
			{
				return _bShowEditor;
			}
			set
			{
				_bShowEditor = value;
			}
		}

		public EditorInfo(IBaseDeviceEditor editor)
		{
			_editor = editor;
		}
	}
}
