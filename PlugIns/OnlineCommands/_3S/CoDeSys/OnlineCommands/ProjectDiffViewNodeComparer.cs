using System.Collections;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ProjectDiffViewNodeComparer : IComparer
    {
        internal static readonly ProjectDiffViewNodeComparer Singleton = new ProjectDiffViewNodeComparer();

        public int Compare(object x, object y)
        {
            ProjectDiffViewNode obj = (ProjectDiffViewNode)x;
            ProjectDiffViewNode other = (ProjectDiffViewNode)y;
            return obj.CompareTo(other);
        }
    }
}
