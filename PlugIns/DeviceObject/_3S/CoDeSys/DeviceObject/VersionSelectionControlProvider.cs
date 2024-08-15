using System;
using System.Windows.Forms;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.VersionCompatibilityManager;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{FBD3A229-80C9-480f-B153-DEC4ABB6A898}")]
	public class VersionSelectionControlProvider : IVersionSelectionControlProvider
	{
		internal static readonly Guid GUID = new Guid("{FBD3A229-80C9-480f-B153-DEC4ABB6A898}");

		public string ProviderName => Strings.VersionSelectionProviderName;

		public UserControl GetControl()
		{
			return new DeviceVersionSelectionControl();
		}
	}
}
