using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	public class ConnectorComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			Connector obj = x as Connector;
			Connector connector = y as Connector;
			return obj.ConnectorId.CompareTo(connector.ConnectorId);
		}
	}
}
