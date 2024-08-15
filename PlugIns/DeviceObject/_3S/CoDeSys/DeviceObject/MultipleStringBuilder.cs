using System;
using System.Collections.Generic;
using System.Text;

namespace _3S.CoDeSys.DeviceObject
{
	public class MultipleStringBuilder
	{
		private List<string> _stringlist = new List<string>();

		private StringBuilder _innerStringBuilder;

		private int _iBlockSize;

		public List<string> StringList
		{
			get
			{
				List<string> list = new List<string>();
				list.AddRange(_stringlist);
				list.Add(_innerStringBuilder.ToString());
				return list;
			}
		}

		public MultipleStringBuilder()
			: this(32767)
		{
		}

		public MultipleStringBuilder(int iBlockSize)
		{
			_innerStringBuilder = new StringBuilder(iBlockSize);
			_iBlockSize = iBlockSize;
		}

		public void Append(string st)
		{
			if (_innerStringBuilder.Length + st.Length > _iBlockSize)
			{
				_stringlist.Add(_innerStringBuilder.ToString());
				_innerStringBuilder.Remove(0, _innerStringBuilder.Length);
			}
			_innerStringBuilder.Append(st);
		}

		public void AppendLine(string st)
		{
			Append(st);
			Append(Environment.NewLine);
		}

		public List<string> Release()
		{
			List<string> stringList = StringList;
			_innerStringBuilder.Remove(0, _innerStringBuilder.Length);
			_stringlist = new List<string>();
			return stringList;
		}
	}
}
