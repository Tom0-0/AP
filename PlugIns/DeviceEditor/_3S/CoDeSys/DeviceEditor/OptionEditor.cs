using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{741C7DC6-0B10-436c-93ED-9DA729B00B8A}")]
	public class OptionEditor : IOptionEditor
	{
		public OptionRoot OptionRoot => (OptionRoot)4;

		public Icon LargeIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceEditor.Resources.DeviceIcon.ico");

		public string Description => Strings.OptionEditor_Description;

		public string Name => Strings.OptionEditor_Name;

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceEditor.Resources.DeviceIconSmall.ico");

		public Control CreateControl()
		{
			return new OptionControl();
		}

		public bool Save(Control control, ref string stMessage, ref Control failedControl)
		{
			return ((OptionControl)control).Save(ref stMessage, ref failedControl);
		}
	}
}
