using System;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class DeviceComboBoxItem
    {
        public readonly Guid ObjectGuid;

        public readonly string Name;

        public DeviceComboBoxItem(Guid objectGuid, string stName)
        {
            ObjectGuid = objectGuid;
            Name = stName;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            DeviceComboBoxItem deviceComboBoxItem = (DeviceComboBoxItem)obj;
            return ObjectGuid == deviceComboBoxItem.ObjectGuid;
        }

        public override int GetHashCode()
        {
            Guid objectGuid = ObjectGuid;
            return objectGuid.GetHashCode();
        }
    }
}
