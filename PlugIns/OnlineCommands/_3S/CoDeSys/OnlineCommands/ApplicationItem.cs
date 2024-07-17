using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    public class ApplicationItem : IMultipleDownloadCheckBoxItem
    {
        private Guid _objectGuid = Guid.Empty;

        private string _stToString = string.Empty;

        private bool _bIsSelected;

        internal Guid ObjectGuid => _objectGuid;

        public Guid ApplicationGuid => _objectGuid;

        public IMultipleDownloadExtension Extension => null;

        public bool IsChecked
        {
            get
            {
                return _bIsSelected;
            }
            set
            {
                _bIsSelected = value;
            }
        }

        public string VisibleName => _stToString;

        internal ApplicationItem(IMetaObjectStub mos)
        {
            if (mos == null)
            {
                throw new ArgumentNullException("mos");
            }
            _objectGuid = mos.ObjectGuid;
            _stToString = mos.Name;
            for (IMetaObjectStub val = mos; val != null; val = ((val.ParentObjectGuid != Guid.Empty) ? ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val.ProjectHandle, val.ParentObjectGuid) : null))
            {
                if (typeof(IDeviceObject).IsAssignableFrom(val.ObjectType))
                {
                    _stToString = val.Name + ":   " + mos.Name;
                    break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return _objectGuid == ((ApplicationItem)obj)._objectGuid;
        }

        public override int GetHashCode()
        {
            return _objectGuid.GetHashCode();
        }

        public override string ToString()
        {
            return _stToString;
        }
    }
}
