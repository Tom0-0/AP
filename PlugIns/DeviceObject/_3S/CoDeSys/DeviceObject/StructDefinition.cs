using System;

namespace _3S.CoDeSys.DeviceObject
{
    public class StructDefinition
    {
        public string _stName;

        public string _stDefinition;

        public Guid _guidStructDef;

        public StructDefinition(string stName, string stDefinition, Guid guidStructDef, bool bHide)
        {
            _stName = stName;
            if (bHide)
            {
                _stDefinition = LanguageModelHelper.PRAGMA_ATTRIBUTE_HIDE + "\n" + stDefinition;
            }
            else
            {
                _stDefinition = stDefinition;
            }
            _guidStructDef = guidStructDef;
        }
    }
}
