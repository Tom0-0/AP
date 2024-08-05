using System;
using System.Collections;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FbInstanceProvider : IFbInstanceProvider, IEnumerable
	{
		private IOMappingEditor _editor;

		public IOMappingEditor Editor => _editor;

		internal FbInstanceProvider(IOMappingEditor editor)
		{
			_editor = editor;
		}

		public IFbInstance GetAt(int nIndex, bool bToModify)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			int num = 0;
			IEnumerator fbInstanceEnumerator = GetFbInstanceEnumerator(bToModify);
			while (fbInstanceEnumerator.MoveNext())
			{
				IFbInstance result = (IFbInstance)fbInstanceEnumerator.Current;
				if (num == nIndex)
				{
					return result;
				}
				num++;
			}
			throw new IndexOutOfRangeException();
		}

		public IEnumerator GetEnumerator()
		{
			return GetFbInstanceEnumerator(bToModify: false);
		}

		public IEnumerator GetFbInstanceEnumerator(bool bToModify)
		{
			return new FbInstanceEnumerator(_editor.GetDriverInfo(bToModify));
		}
	}
}
