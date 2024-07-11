using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
    public interface IMonitoringRangeSetup
    {
        bool SetupMonitoringRangeContext(ref MonitoringRangeContext monitoringRangeContext);
    }
}
