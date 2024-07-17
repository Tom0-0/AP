using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{9FD7DA9F-D0AC-462f-A8A2-5A619E8BBD24}")]
    [StorageVersion("3.3.0.0")]
    public class OnlineViewInfoTable : GenericObject2
    {
        [DefaultSerialization("Table")]
        [StorageVersion("3.3.0.0")]
        private Hashtable _table = new Hashtable();

        public OnlineViewInfo[] GetOnlineViews(Guid onlineApplicationGuid)
        {
            ArrayList arrayList = _table[onlineApplicationGuid] as ArrayList;
            if (arrayList != null)
            {
                OnlineViewInfo[] array = new OnlineViewInfo[arrayList.Count];
                arrayList.CopyTo(array);
                return array;
            }
            return new OnlineViewInfo[0];
        }

        public void ClearOnlineViews(Guid onlineApplicationGuid)
        {
            _table.Remove(onlineApplicationGuid);
        }

        public void ClearOnlineView(Guid onlineApplicationGuid, Guid objectGuid, string instancePath)
        {
            ArrayList arrayList = _table[onlineApplicationGuid] as ArrayList;
            if (arrayList == null)
            {
                return;
            }
            for (int num = arrayList.Count - 1; num >= 0; num--)
            {
                OnlineViewInfo onlineViewInfo = (OnlineViewInfo)arrayList[num];
                if (onlineViewInfo.ObjectGuid == objectGuid && onlineViewInfo.InstancePath == instancePath)
                {
                    arrayList.RemoveAt(num);
                }
            }
        }

        public void AddOnlineView(Guid onlineApplicationGuid, OnlineViewInfo onlineViewInfo)
        {
            ArrayList arrayList = _table[onlineApplicationGuid] as ArrayList;
            if (arrayList == null)
            {
                arrayList = new ArrayList();
                _table[onlineApplicationGuid] = arrayList;
            }
            arrayList.Add(onlineViewInfo);
        }

        public OnlineViewInfoTable()
            : base()
        {
        }
    }
}
