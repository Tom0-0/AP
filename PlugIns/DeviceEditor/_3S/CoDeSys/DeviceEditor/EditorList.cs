using System.Collections;
using System.Collections.Generic;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class EditorList : IEnumerable
	{
		private List<EditorInfo> _alEditors = new List<EditorInfo>();

		public void AddEditor(EditorInfo editorInfo)
		{
			_alEditors.Add(editorInfo);
		}

		public void Clear()
		{
			_alEditors.Clear();
		}

		public EditorInfo FindEditor(IBaseDeviceEditor editor, out int nPageIndex)
		{
			nPageIndex = 0;
			foreach (EditorInfo alEditor in _alEditors)
			{
				if (object.Equals(alEditor.Editor, editor))
				{
					return alEditor;
				}
				if (alEditor.ShowEditor)
				{
					nPageIndex += alEditor.Pages.Length;
				}
			}
			nPageIndex = -1;
			return null;
		}

		public EditorInfo UpdateEditorInfo(IBaseDeviceEditor editor, out int nPageIndex)
		{
			nPageIndex = 0;
			foreach (EditorInfo alEditor in _alEditors)
			{
				if (((object)editor).GetType().IsAssignableFrom(((object)alEditor.Editor).GetType()))
				{
					alEditor.Editor = editor;
					return alEditor;
				}
				if (alEditor.ShowEditor)
				{
					nPageIndex += alEditor.Pages.Length;
				}
			}
			nPageIndex = -1;
			return null;
		}

		public EditorInfo UpdateEditorInfoPages(IBaseDeviceEditor editor)
		{
			foreach (EditorInfo alEditor in _alEditors)
			{
				if (((object)editor).GetType().IsAssignableFrom(((object)alEditor.Editor).GetType()))
				{
					alEditor.Pages = editor.Pages;
					return alEditor;
				}
			}
			return null;
		}

		public IEnumerator GetEnumerator()
		{
			return _alEditors.GetEnumerator();
		}
	}
}
