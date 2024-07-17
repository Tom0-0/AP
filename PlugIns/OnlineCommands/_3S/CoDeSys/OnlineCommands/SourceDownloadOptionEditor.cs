using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Options;
using System.Drawing;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{C35CA7DD-84A8-4ecc-A972-F1C8D070A075}")]
    public class SourceDownloadOptionEditor : IOptionEditor
    {
        public OptionRoot OptionRoot => (OptionRoot)0;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SourceDownloadOptionEditorName");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SourceDownloadOptionEditorDescription");

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.SourceDownload.ico");

        public Icon LargeIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.SourceDownload.ico");

        public Control CreateControl()
        {
            return new SourceDownloadOptionControl();
        }

        public bool Save(Control control, ref string stMessage, ref Control failedControl)
        {
            return ((SourceDownloadOptionControl)control).Save(ref stMessage, ref failedControl);
        }
    }
}
