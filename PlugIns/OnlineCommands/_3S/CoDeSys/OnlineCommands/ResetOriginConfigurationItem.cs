using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.OnlineCommands
{
    public class ResetOriginConfigurationItem : IResetOriginConfigurationItem
    {
        public bool Active { get; set; }

        public bool Delete { get; set; }

        public bool CanDelete { get; set; }

        public string Description { get; set; }
    }
}
