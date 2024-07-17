using _3S.CoDeSys.ProjectArchive;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ProjectArchiveItem : IProjectArchiveItem
    {
        private IProjectArchiveCategory _category;

        private string _stId;

        public IProjectArchiveCategory Category => _category;

        public string Id => _stId;

        internal ProjectArchiveItem(IProjectArchiveCategory category, string stId)
        {
            _category = category;
            _stId = stId;
        }
    }
}
