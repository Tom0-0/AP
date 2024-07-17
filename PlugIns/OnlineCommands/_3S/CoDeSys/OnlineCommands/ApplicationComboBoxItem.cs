using System;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ApplicationComboBoxItem
    {
        private Guid _applicationGuid;

        private string _stApplication;

        private bool _bDevice;

        internal Guid ApplicationGuid => _applicationGuid;

        internal string Application => _stApplication;

        internal ApplicationComboBoxItem(Guid applicationGuid, string stApplication, bool bDevice)
        {
            if (stApplication == null)
            {
                throw new ArgumentNullException("stApplication");
            }
            _applicationGuid = applicationGuid;
            _stApplication = stApplication;
            _bDevice = bDevice;
        }

        public override string ToString()
        {
            return _stApplication;
        }
    }
}
