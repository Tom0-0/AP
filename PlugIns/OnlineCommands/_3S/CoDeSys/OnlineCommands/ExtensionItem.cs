using _3S.CoDeSys.OnlineUI;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    public class ExtensionItem : IMultipleDownloadCheckBoxItem
    {
        private IMultipleDownloadExtension _extension;

        private bool _bIsSelected;

        public Guid ApplicationGuid => Guid.Empty;

        public IMultipleDownloadExtension Extension => _extension;

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

        public string VisibleName => _extension.Name;

        internal ExtensionItem(IMultipleDownloadExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }
            _extension = extension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return _extension.Name == ((ExtensionItem)obj)._extension.Name;
        }

        public override int GetHashCode()
        {
            return _extension.Name.GetHashCode();
        }

        public override string ToString()
        {
            return _extension.Name;
        }
    }
}
