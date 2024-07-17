using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;
using System.Collections.Generic;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class TextualOnlineChangeDetails
    {
        internal int nCountLocationChanged;

        internal int nCountReinit;

        internal int nCountInitialised;

        internal int nCountCopied;

        internal int nCountVFInitialised;

        internal List<ISignature4> changedsignatures = new List<ISignature4>();

        internal LList<ICompiledPOU> cpouschanged = new LList<ICompiledPOU>();

        internal LStringBuilder stbInformation = new LStringBuilder();

        internal LStringBuilder stbInformationText = new LStringBuilder();
    }
}
