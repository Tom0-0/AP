using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Messages;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{142F9DD8-31BF-4221-934C-A7EE8FF50FE0}")]
    public class DownloadMessageCategory : IMessageCategory
    {
        public static readonly DownloadMessageCategory Singleton = new DownloadMessageCategory();

        public Icon Icon => null;

        public string Text => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DownloadMessageCategory");
    }
}
