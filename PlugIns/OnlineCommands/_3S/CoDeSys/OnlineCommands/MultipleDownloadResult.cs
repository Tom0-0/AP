using _3S.CoDeSys.OnlineUI;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    internal struct MultipleDownloadResult
    {
        internal IMultipleDownloadItem Item;

        internal MultipleDownloadResultState State;

        internal string ErrorMessage;

        internal void SetApplication(Guid applicationGuid)
        {
            ApplicationMultipleDownloadItem applicationMultipleDownloadItem = new ApplicationMultipleDownloadItem();
            applicationMultipleDownloadItem.ApplicationGuid = applicationGuid;
            Item = applicationMultipleDownloadItem;
        }

        internal void SetExtension(IMultipleDownloadExtension extension)
        {
            ExtensionMultipleDownloadItem extensionMultipleDownloadItem = new ExtensionMultipleDownloadItem();
            extensionMultipleDownloadItem.Extension = extension;
            Item = extensionMultipleDownloadItem;
        }
    }
}
