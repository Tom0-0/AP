using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class Editable<T> where T : class
	{
		private bool _canEdit = true;

		private T _t;

		public bool CanEdit
		{
			get
			{
				return _canEdit;
			}
			set
			{
				_canEdit = value;
			}
		}

		public Editable(T t)
		{
			_t = t;
		}

		public Editable(T t, bool canEdit)
		{
			_t = t;
			_canEdit = canEdit;
		}

		public static Editable<T>[] GetEditDecorate(T[] array, bool canEdit)
		{
			Editable<T>[] array2 = new Editable<T>[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new Editable<T>(array[i], canEdit);
			}
			return array2;
		}
	}
}
