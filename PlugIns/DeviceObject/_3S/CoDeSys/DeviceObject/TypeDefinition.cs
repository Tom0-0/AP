using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System.Xml;

namespace _3S.CoDeSys.DeviceObject
{
    [TypeGuid("{4ef0e791-9981-44ab-abc0-9bb59a838243}")]
    [StorageVersion("3.5.6.0")]
    public class TypeDefinition : GenericObject2, ITypeDefinition
    {
        private TypeList _typeList;

        private string _stName;

        private bool _bCreatedType;

        internal string Name => _stName;

        public string Identifier => _stName;

        public bool CreatedType
        {
            get
            {
                return _bCreatedType;
            }
            set
            {
                _bCreatedType = value;
            }
        }

        public TypeDefinition()
            : this()
        {
        }

        public TypeDefinition(TypeList typeList, XmlReader reader)
            : this()
        {
            _stName = reader.GetAttribute("name");
            _typeList = typeList;
        }

        public TypeDefinition(TypeList typeList, XmlElement xeNode)
            : this()
        {
            _stName = xeNode.GetAttribute("name");
            _typeList = typeList;
        }

        internal void SetTypeList(TypeList typeList, string stName)
        {
            _typeList = typeList;
            _stName = stName;
        }
    }
}
