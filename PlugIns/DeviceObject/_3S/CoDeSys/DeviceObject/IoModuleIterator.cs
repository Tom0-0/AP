using System;

namespace _3S.CoDeSys.DeviceObject
{
	public class IoModuleIterator : IIoModuleIterator2, IIoModuleIterator, IDisposable
	{
		private IoModuleReferenceBase _current;

		private bool _bDisposed;

		public IIoModuleReference Current => (IIoModuleReference)(object)_current;

		internal IoModuleIterator(IoModuleReferenceBase start)
		{
			_current = start;
		}

		public bool MoveToNextInConnectorList()
		{
			return LanguageModelHelper.MoveIoIteratorInConnectorListOrder((IIoModuleIterator)(object)this);
		}

		public bool MoveToParent()
		{
			CheckValidState();
			IoModuleReferenceBase parent = _current.GetParent();
			if (parent == null)
			{
				return false;
			}
			_current = parent;
			return true;
		}

		public bool MoveToFirstChild()
		{
			CheckValidState();
			IoModuleReferenceBase firstChild = _current.GetFirstChild();
			if (firstChild == null)
			{
				return false;
			}
			_current = firstChild;
			return true;
		}

		public bool MoveToNextSibling()
		{
			CheckValidState();
			IoModuleReferenceBase nextSibling = _current.GetNextSibling();
			if (nextSibling == null)
			{
				return false;
			}
			_current = nextSibling;
			return true;
		}

		public bool MoveToPrevSibling()
		{
			CheckValidState();
			IoModuleReferenceBase prevSibling = _current.GetPrevSibling();
			if (prevSibling == null)
			{
				return false;
			}
			_current = prevSibling;
			return true;
		}

		public void Dispose()
		{
			_bDisposed = true;
			_current = null;
		}

		private void CheckValidState()
		{
			if (_bDisposed)
			{
				throw new ObjectDisposedException("IIoModuleIterator");
			}
		}
	}
}
