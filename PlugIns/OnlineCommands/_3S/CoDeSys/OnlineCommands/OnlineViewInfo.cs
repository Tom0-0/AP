using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{52D7B7D2-AF8B-47cc-A14D-983518010B49}")]
    [StorageVersion("3.3.0.0")]
    public class OnlineViewInfo : GenericObject2
    {
        [DefaultSerialization("ObjectGuid")]
        [StorageVersion("3.3.0.0")]
        private Guid _objectGuid;

        [DefaultSerialization("FactoryGuid")]
        [StorageVersion("3.3.0.0")]
        private Guid _factoryGuid;

        [DefaultSerialization("InstancePath")]
        [StorageVersion("3.3.0.0")]
        private string _stInstancePath;

        private Rectangle _floatingRect;

        public Guid ObjectGuid => _objectGuid;

        public Guid FactoryGuid => _factoryGuid;

        public string InstancePath => _stInstancePath;

        public Rectangle FloatingRect => _floatingRect;

        [DefaultSerialization("FloatingRectX")]
        [StorageVersion("3.5.9.10")]
        [StorageDefaultValue(0)]
        private int FloatingRectX
        {
            get
            {
                return _floatingRect.X;
            }
            set
            {
                _floatingRect.X = value;
            }
        }

        [DefaultSerialization("FloatingRectY")]
        [StorageVersion("3.5.9.10")]
        [StorageDefaultValue(0)]
        private int FloatingRectY
        {
            get
            {
                return _floatingRect.Y;
            }
            set
            {
                _floatingRect.Y = value;
            }
        }

        [DefaultSerialization("FloatingRectWidth")]
        [StorageVersion("3.5.9.10")]
        [StorageDefaultValue(0)]
        private int FloatingRectWidth
        {
            get
            {
                return _floatingRect.Width;
            }
            set
            {
                _floatingRect.Width = value;
            }
        }

        [DefaultSerialization("FloatingRectHeight")]
        [StorageVersion("3.5.9.10")]
        [StorageDefaultValue(0)]
        private int FloatingRectHeight
        {
            get
            {
                return _floatingRect.Height;
            }
            set
            {
                _floatingRect.Height = value;
            }
        }

        public OnlineViewInfo()
            : base()
        {
        }

        public OnlineViewInfo(Guid objectGuid, Guid factoryGuid, string stInstancePath, Rectangle floatingRect)
            : this()
        {
            _objectGuid = objectGuid;
            _factoryGuid = factoryGuid;
            _stInstancePath = stInstancePath;
            _floatingRect = floatingRect;
        }
    }
}
