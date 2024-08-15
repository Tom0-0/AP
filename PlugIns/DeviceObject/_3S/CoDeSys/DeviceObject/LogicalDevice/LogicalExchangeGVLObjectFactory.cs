using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject.LogicalDevice
{
	[TypeGuid("{A8C563C7-54FC-49A5-856A-834206FCF6FE}")]
	public class LogicalExchangeGVLObjectFactory : IEmbeddedDiffViewerFactory
	{
		public bool AcceptsObjectType(Type objectType, Type[] embeddedObjectTypes)
		{
			if (objectType != null && typeof(ILogicalGVLObject).IsAssignableFrom(objectType))
			{
				return true;
			}
			return false;
		}

		public IEmbeddedDiffViewer Create(int nLeftProjectHandle, Guid leftObjectGuid, int nRightProjectHandle, Guid rightObjectGuid, bool bIgnoreWhitespace, bool bIgnoreComments, bool bIgnoreProperties, string stLeftLabel, string stRightLabel)
		{
			LogicalGVLEmbeddesDiffViewer logicalGVLEmbeddesDiffViewer = new LogicalGVLEmbeddesDiffViewer();
			logicalGVLEmbeddesDiffViewer.Initialize(nLeftProjectHandle, leftObjectGuid, nRightProjectHandle, rightObjectGuid, bIgnoreWhitespace, bIgnoreComments, bIgnoreProperties, stLeftLabel, stRightLabel);
			return (IEmbeddedDiffViewer)(object)logicalGVLEmbeddesDiffViewer;
		}
	}
}
