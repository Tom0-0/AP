using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class EditableComboBoxValue
	{
		private string _stText = string.Empty;

		private List<string> _items;

		public bool IsDefaultValue { get; private set; }

		public List<string> Items => _items;

		public string Text => _stText;

		public EditableComboBoxValue(string stText, bool isDefaultValue)
		{
			_stText = stText;
			IsDefaultValue = isDefaultValue;
		}

		public EditableComboBoxValue(List<string> items, string stText, bool isDefaultValue)
		{
			_items = items;
			_stText = stText;
			IsDefaultValue = isDefaultValue;
		}

		public override string ToString()
		{
			return _stText;
		}
	}
}
