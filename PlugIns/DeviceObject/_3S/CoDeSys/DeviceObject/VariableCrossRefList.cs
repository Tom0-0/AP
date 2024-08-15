using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
    internal class VariableCrossRefList
    {
        private ArrayList _alCrossRefs = new ArrayList();

        public void Add(VariableCrossRef vcr)
        {
            _alCrossRefs.Add(vcr);
        }

        public VariableCrossRef[] GetCrossRefs()
        {
            VariableCrossRef[] array = new VariableCrossRef[_alCrossRefs.Count];
            _alCrossRefs.CopyTo(array);
            return array;
        }
    }
}
