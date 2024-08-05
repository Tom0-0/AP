using System.Collections.Generic;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{2C2402C1-6CC4-44C6-B321-D0CDDF09F998}")]
	public class DeviceEditorExportableOption : IExportableOption
	{
		private static readonly string[] PATH = new string[1] { OptionsHelper.SUB_KEY };

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ExportableOption_DeviceEditor");

		public IEnumerable<string> Path => PATH;

		public OptionRoot Root => (OptionRoot)4;
	}
}
