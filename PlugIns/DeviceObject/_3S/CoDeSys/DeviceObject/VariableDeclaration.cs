#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;

namespace _3S.CoDeSys.DeviceObject
{
    internal class VariableDeclaration
    {
        private class NameComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                VariableDeclaration variableDeclaration = x as VariableDeclaration;
                VariableDeclaration variableDeclaration2 = y as VariableDeclaration;
                Debug.Assert(variableDeclaration != null && variableDeclaration2 != null);
                return string.Compare(variableDeclaration.Variable.GetPlainVariableName(), variableDeclaration.Variable.GetPlainVariableName(), StringComparison.OrdinalIgnoreCase);
            }
        }

        public static readonly IComparer COMPARER_BY_NAME = new NameComparer();

        private VariableMapping _variable;

        private ConnectorMap _connectorMap;

        private ChannelMap _channelMap;

        private string _stStaticError;

        public VariableMapping Variable => _variable;

        public ConnectorMap Connector => _connectorMap;

        public ChannelMap Channel => _channelMap;

        public bool HasStaticError => _stStaticError != null;

        public VariableDeclaration(VariableMapping variable, ConnectorMap connectorMap, ChannelMap channelMap)
        {
            Debug.Assert(variable != null);
            Debug.Assert(connectorMap != null);
            Debug.Assert(channelMap != null);
            _variable = variable;
            _connectorMap = connectorMap;
            _channelMap = channelMap;
        }

        public void SetStaticError(string stError)
        {
            _stStaticError = stError;
        }

        public string GetErrorPragma()
        {
            string text = _channelMap.Type;
            if (text == "BIT")
            {
                text = "BOOL";
            }
            if (text == "SAFEBIT")
            {
                text = "SAFEBOOL";
            }
            if (_stStaticError != null)
            {
                return _stStaticError;
            }
            if (_variable.CreateVariable)
            {
                return "";
            }
            return string.Format("\r\n{IF (NOT hastype (variable: {0}, {1}))}\r\n\t{error 'Types of channel and mapped variable do not match' show_compile}\r\n{END_IF}", _variable.GetPlainVariableName(), text);
        }
    }
}
