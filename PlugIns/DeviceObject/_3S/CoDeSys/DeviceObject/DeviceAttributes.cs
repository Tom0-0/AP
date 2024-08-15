using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
    [TypeGuid("{1fdbedd8-b518-4ebd-99df-805f2f77c071}")]
    [StorageVersion("3.3.0.0")]
    public class DeviceAttributes : GenericObject2, IDeviceAttributesCollection, ICollection, IEnumerable
    {
        [DefaultSerialization("Attributes")]
        [StorageVersion("3.3.0.0")]
        [DefaultDuplication(/*Could not decode attribute arguments.*/)]
        private Hashtable _htAttributes = new Hashtable();

        public string this[string stAttributeName]
        {
            get
            {
                return (string)_htAttributes[stAttributeName];
            }
            set
            {
                if (value == null)
                {
                    _htAttributes.Remove(stAttributeName);
                }
                else
                {
                    _htAttributes[stAttributeName] = value;
                }
            }
        }

        public bool IsSynchronized => false;

        public int Count => _htAttributes.Count;

        public object SyncRoot => _htAttributes;

        public DeviceAttributes()
            : this()
        {
        }

        private DeviceAttributes(DeviceAttributes original)
            : this()
        {
            _htAttributes = new Hashtable(original._htAttributes);
        }

        public override object Clone()
        {
            DeviceAttributes deviceAttributes = new DeviceAttributes(this);
            ((GenericObject)deviceAttributes).AfterClone();
            return deviceAttributes;
        }

        public void AddAttribute(string stKey, string stValue)
        {
            _htAttributes[stKey] = stValue;
        }

        public void Clear()
        {
            _htAttributes.Clear();
        }

        public string GetAttribute(string stKey)
        {
            return (string)_htAttributes[stKey];
        }

        public void CopyTo(Array array, int index)
        {
            _htAttributes.Keys.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return _htAttributes.Keys.GetEnumerator();
        }
    }
}
