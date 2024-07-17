using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class MultipleDownloadResultNode : ITreeTableNode
    {
        private MultipleDownloadResult _result;

        private static readonly Image IMAGE_OK = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(MultipleDownloadResultNode), "_3S.CoDeSys.OnlineCommands.Resources.OK.ico").Handle);

        private static readonly Image IMAGE_SKIPPED = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(MultipleDownloadResultNode), "_3S.CoDeSys.OnlineCommands.Resources.Skipped.ico").Handle);

        private static readonly Image IMAGE_ERROR = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(MultipleDownloadResultNode), "_3S.CoDeSys.OnlineCommands.Resources.Error.ico").Handle);

        public int ChildCount => 0;

        public bool HasChildren => false;

        public ITreeTableNode Parent => null;

        internal MultipleDownloadResultNode(MultipleDownloadResult result)
        {
            _result = result;
        }

        public ITreeTableNode GetChild(int nIndex)
        {
            throw new ArgumentOutOfRangeException("nIndex");
        }

        public int GetIndex(ITreeTableNode node)
        {
            return -1;
        }

        public object GetValue(int nColumnIndex)
        {
            //IL_0015: Unknown result type (might be due to invalid IL or missing references)
            //IL_001a: Unknown result type (might be due to invalid IL or missing references)
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            //IL_003d: Expected I4, but got Unknown
            //IL_00bb: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c1: Expected O, but got Unknown
            //IL_00c7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00cc: Unknown result type (might be due to invalid IL or missing references)
            //IL_00cd: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ef: Expected I4, but got Unknown
            switch (nColumnIndex)
            {
                case 0:
                    {
                        MultipleDownloadResultState state = _result.State;
                        Image image;
                        switch ((int)state)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                image = IMAGE_OK;
                                break;
                            case 4:
                                image = IMAGE_SKIPPED;
                                break;
                            default:
                                image = IMAGE_ERROR;
                                break;
                        }
                        string text = ((_result.Item is ApplicationMultipleDownloadItem) ? OnlineCommandHelper.GetDevicePrefixedApplicationName(((ApplicationMultipleDownloadItem)_result.Item).ApplicationGuid) : ((!(_result.Item is ExtensionMultipleDownloadItem)) ? "?" : ((ExtensionMultipleDownloadItem)_result.Item).Extension.Name));
                        return (object)new IconLabelTreeTableViewCellData(image, (object)text);
                    }
                case 1:
                    {
                        MultipleDownloadResultState state = _result.State;
                        switch ((int)state)
                        {
                            case 0:
                                return Strings.MultipleDownloadResult_Created;
                            case 1:
                                return Strings.MultipleDownloadResult_NotChanged;
                            case 2:
                                return Strings.MultipleDownloadResult_OnlineChanged;
                            case 3:
                                return Strings.MultipleDownloadResult_Downloaded;
                            case 5:
                                if (_result.ErrorMessage != null)
                                {
                                    string arg = _result.ErrorMessage.Replace('\r', ' ').Replace('\n', ' ');
                                    return $"{Strings.MultipleDownloadResult_Error}: {arg}";
                                }
                                return Strings.MultipleDownloadResult_Error;
                            case 6:
                                return Strings.MultipleDownloadResult_CancelledByUser;
                            case 4:
                                return Strings.MultipleDownloadResult_SkippedDueToImpossibleOnlineChange;
                            default:
                                return "?";
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException("nColumnIndex");
            }
        }

        public bool IsEditable(int nColumnIndex)
        {
            return false;
        }

        public void SetValue(int nColumnIndex, object value)
        {
            throw new InvalidOperationException("This node is read-only.");
        }

        public void SwapChildren(int nIndex1, int nIndex2)
        {
            throw new ArgumentOutOfRangeException("nIndex1");
        }
    }
}
