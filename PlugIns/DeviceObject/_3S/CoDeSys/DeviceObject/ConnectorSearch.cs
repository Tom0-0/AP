using _3S.CoDeSys.Utilities;
using System;
using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
    internal class ConnectorSearch
    {
        internal static Predicate<Connector> CheckRole(ConnectorRole role)
        {
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Unknown result type (might be due to invalid IL or missing references)
            return (Connector match) => match.ConnectorRole == role;
        }

        internal static Predicate<Connector> CheckID(int connectorID)
        {
            return (Connector match) => match.ConnectorId == connectorID;
        }

        internal static Connector Find(IConnectorCollection connectors, Predicate<Connector> match)
        {
            foreach (Connector item in (IEnumerable)connectors)
            {
                if (match(item))
                {
                    return item;
                }
            }
            return null;
        }

        internal static LList<Connector> FindAll(IConnectorCollection connectors, Predicate<Connector> match)
        {
            LList<Connector> val = new LList<Connector>();
            foreach (Connector item in (IEnumerable)connectors)
            {
                if (match(item))
                {
                    val.Add(item);
                }
            }
            return val;
        }
    }
}
