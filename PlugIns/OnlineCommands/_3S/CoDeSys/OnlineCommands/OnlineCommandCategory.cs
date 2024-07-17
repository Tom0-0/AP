using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{8DDBE3C7-2966-4ba9-A27B-7DB46265241D}")]
    public class OnlineCommandCategory : ICommandCategory
    {
        public string Text => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "OnlineCommandCategory_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "OnlineCommandCategory_Description");

        public static Guid Guid => ((TypeGuidAttribute)typeof(OnlineCommandCategory).GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;
    }
}
