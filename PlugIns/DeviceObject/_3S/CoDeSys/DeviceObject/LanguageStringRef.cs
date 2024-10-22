using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
    [TypeGuid("{BC711AA2-6E73-46BD-9A85-EADE2B13BAA1}")]
    [StorageVersion("3.5.3.0")]
    public class LanguageStringRef : StringRef
    {
        [DefaultSerialization("Language")]
        [StorageVersion("3.5.3.0")]
        private string _stLanguage = "";

        public string Language => _stLanguage;

        [DefaultDuplication(DuplicationMethod.Shallow)]
        protected string LanguageSerialization
        {
            get
            {
                return _stLanguage;
            }
            set
            {
                _stLanguage = string.Intern(value);
            }
        }

        public LanguageStringRef()
        {
        }

        internal LanguageStringRef(string stLanguage, string stNamespace, string stIdentifier, string stDefault)
        {
            _stLanguage = stLanguage;
            base.Namespace = string.Intern(stNamespace);
            base.Identifier = string.Intern(stIdentifier);
            base.Default = string.Intern(stDefault);
        }

        public override object Clone()
        {
            return new LanguageStringRef(_stLanguage, base.Namespace, base.Identifier, base.Default);
        }
    }
}
