using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{83132DD3-7AE4-43df-A185-69E1AA5ADEE3}")]
	public class DeviceObjectDiffViewer : IEmbeddedDiffViewerFactory
	{
		public bool AcceptsObjectType(Type objectType, Type[] embeddedObjectTypes)
		{
			if (typeof(IDeviceObject).IsAssignableFrom(objectType) || typeof(IExplicitConnector).IsAssignableFrom(objectType))
			{
				return true;
			}
			return false;
		}

		public IEmbeddedDiffViewer Create(int nLeftProjectHandle, Guid leftObjectGuid, int nRightProjectHandle, Guid rightObjectGuid, bool bIgnoreWhitespace, bool bIgnoreComments, bool bIgnoreProperties, string stLeftLabel, string stRightLabel)
		{
			IDeviceObjectEmbeddedDiffViewerFactory factory = DeviceObjectDiffViewerFactoryManager.Instance.GetFactory(nLeftProjectHandle, leftObjectGuid, nRightProjectHandle, rightObjectGuid);
			if (factory != null)
			{
				return factory.Create(nLeftProjectHandle, leftObjectGuid, nRightProjectHandle, rightObjectGuid, bIgnoreWhitespace, bIgnoreComments, bIgnoreProperties, stLeftLabel, stRightLabel);
			}
			DeviceObjectDiffView deviceObjectDiffView = new DeviceObjectDiffView();
			deviceObjectDiffView.Initialize(nLeftProjectHandle, leftObjectGuid, nRightProjectHandle, rightObjectGuid, bIgnoreWhitespace, bIgnoreComments, bIgnoreProperties, stLeftLabel, stRightLabel);
			return (IEmbeddedDiffViewer)(object)deviceObjectDiffView;
		}
	}
}
