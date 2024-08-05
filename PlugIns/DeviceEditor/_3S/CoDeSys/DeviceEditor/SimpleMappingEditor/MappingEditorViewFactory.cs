using System;
using System.Drawing;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	[TypeGuid("{4CCD2DFD-C0E5-4F0F-AFD2-FC63555F5672}")]
	public class MappingEditorViewFactory : IViewFactory
	{
		public static Guid TypeGuid => ((TypeGuidAttribute)typeof(MappingEditorViewFactory).GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;

		public string Name => Strings.SimpleMappingName;

		public string Description => Strings.SimpleMappingDescription;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public IView Create()
		{
			return (IView)(object)new SimpleMappingView();
		}
	}
}
