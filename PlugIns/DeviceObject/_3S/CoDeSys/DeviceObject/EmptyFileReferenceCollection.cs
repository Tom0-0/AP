namespace _3S.CoDeSys.DeviceObject
{
    internal class EmptyFileReferenceCollection : IRepositoryFileReferenceCollection
    {
        private string[] _emtpyKeys = new string[0];

        public IRepositoryFileReference this[string stKey] => null;

        public string[] FileRefKeys => _emtpyKeys;
    }
}
