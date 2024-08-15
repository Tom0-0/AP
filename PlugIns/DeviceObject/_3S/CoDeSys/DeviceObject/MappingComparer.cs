using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
    internal class MappingComparer : IComparer
    {
        private bool _bReverse;

        public MappingComparer(bool bReverse)
        {
            _bReverse = bReverse;
        }

        public int Compare(object x, object y)
        {
            int num = (int)x;
            int value = (int)y;
            if (_bReverse)
            {
                return -num.CompareTo(value);
            }
            return num.CompareTo(value);
        }
    }
}
