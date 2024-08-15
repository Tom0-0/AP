using System;
using System.Collections;
using System.Text;

namespace _3S.CoDeSys.DeviceObject
{
    public class DeviceLanguageModelEventArgs : EventArgs, IDeviceLanguageModelEventArgs
    {
        protected ArrayList _alLanguageModels;

        private StringBuilder _sbCompileMessages = new StringBuilder();

        private int _nProjectHandle;

        private Guid _guidDeviceObject;

        private Guid _guidPlcLogic;

        private Guid _guidApp;

        public int ProjectHandle => _nProjectHandle;

        public Guid DeviceObjectGuid => _guidDeviceObject;

        public Guid PlcLogicGuid => _guidPlcLogic;

        public Guid ApplicationGuid => _guidApp;

        public DeviceLanguageModelEventArgs(int nProjectHandle, Guid guidDeviceObject, Guid guidPlcLogic, Guid guidApp)
        {
            _nProjectHandle = nProjectHandle;
            _guidDeviceObject = guidDeviceObject;
            _guidPlcLogic = guidPlcLogic;
            _guidApp = guidApp;
        }

        public string GetCompileMessages()
        {
            return _sbCompileMessages.ToString();
        }

        public void AddLanguageModelContribution(string stLanguageModel)
        {
            if (_alLanguageModels == null)
            {
                _alLanguageModels = new ArrayList();
            }
            _alLanguageModels.Add(stLanguageModel);
        }

        public void AddMessage(Guid guidObject, long lPosition, string stMessage, CompilerMessage messageType, CompilerMessageWhen whenToShow)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_001f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0022: Invalid comparison between Unknown and I4
            //IL_002c: Unknown result type (might be due to invalid IL or missing references)
            //IL_002f: Invalid comparison between Unknown and I4
            string arg = "";
            string arg2 = "";
            if ((int)messageType == 0)
            {
                arg = "error";
            }
            else
            {
                stMessage = "warning";
            }
            if ((int)whenToShow == 2)
            {
                arg2 = "show_compile";
            }
            else if ((int)whenToShow == 1)
            {
                arg2 = "show_precompile";
            }
            _sbCompileMessages.AppendFormat("{{messageguid '{0}'}}\n", guidObject);
            if (lPosition != -1)
            {
                _sbCompileMessages.AppendFormat("{{p {0} }}\n", lPosition);
            }
            _sbCompileMessages.AppendFormat("{{ {0} '{1}' {2} }}\n", arg, stMessage, arg2);
        }
    }
}
