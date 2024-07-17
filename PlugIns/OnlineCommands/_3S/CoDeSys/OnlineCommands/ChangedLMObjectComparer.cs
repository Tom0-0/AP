using _3S.CoDeSys.Core.LanguageModel;
using System.Collections.Generic;

namespace _3S.CoDeSys.OnlineCommands
{
    internal sealed class ChangedLMObjectComparer : IComparer<IChangedLMObject>
    {
        public int Compare(IChangedLMObject x, IChangedLMObject y)
        {
            //IL_002d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_003f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0045: Unknown result type (might be due to invalid IL or missing references)
            int num = 0;
            bool flag = !x.OnlineChangePossible;
            bool flag2 = !y.OnlineChangePossible;
            if (flag && !flag2)
            {
                num = -1;
            }
            else if (!flag && flag2)
            {
                num = 1;
            }
            else if (x.Change < y.Change)
            {
                num = -1;
            }
            else if (x.Change > y.Change)
            {
                num = 1;
            }
            else
            {
                string pouName = GetPouName(x);
                string pouName2 = GetPouName(y);
                num = pouName.CompareTo(pouName2);
                if (num == 0)
                {
                    string description = x.Description;
                    string description2 = y.Description;
                    num = description.CompareTo(description2);
                }
            }
            return num;
        }

        private string GetPouName(IChangedLMObject changedLMObject)
        {
            if (changedLMObject.Name == null)
            {
                return string.Empty;
            }
            return changedLMObject.Name;
        }
    }
}
