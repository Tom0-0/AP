using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class LibraryElementInfo : ICompiledElementInfoStruct
    {
        private string _stName;

        public string Name
        {
            get
            {
                return _stName;
            }
            set
            {
                _stName = value;
            }
        }

        internal LibraryElementInfo(string stName)
        {
            _stName = stName;
        }
    }
}
