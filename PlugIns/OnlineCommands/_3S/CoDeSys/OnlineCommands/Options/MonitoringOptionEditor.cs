using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.OnlineCommands.Options.Mvc;
using System.Drawing;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands.Options
{
    [TypeGuid("{C8E666FF-6FA3-4193-8310-3E16D33FE659}")]
    public class MonitoringOptionEditor : IOptionEditor
    {
        public OptionRoot OptionRoot => (OptionRoot)2;

        public string Name => Strings.OptionEditorName_Text;

        public string Description => Strings.OptionEditorDescription_Text;

        public Icon LargeIcon => SmallIcon;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.MonitoringSmall.ico");

        public Control CreateControl()
        {
            return new MonitoringOptionView();
        }

        public bool Save(Control control, ref string stMessage, ref Control failedControl)
        {
            return ((MonitoringOptionView)control).Save(ref stMessage, ref failedControl);
        }
    }
}
