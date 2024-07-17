using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Text.RegularExpressions;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ExceptionInformation
    {
        private static Regex s_areaValueRegex = new Regex("<area>(.*?)</area>", RegexOptions.Compiled);

        private static Regex s_offsetValueRegex = new Regex("<off>(.*?)</off>", RegexOptions.Compiled);

        private Guid _appGuid;

        private ICompileContext11 _comcon;

        private ushort _area = ushort.MaxValue;

        private uint _areaOffset;

        private ISourcePosition _sourcePosition;

        private string _completePOUName = "";

        private string _libraryName = "";

        private ICompiledPOU _crashedPOU;

        public string POUName => _completePOUName;

        public string LibraryName => _libraryName;

        public ISourcePosition SourcePosition => _sourcePosition;

        public ExceptionInformation()
        {
        }

        public ExceptionInformation(Guid appGuid)
        {
            _appGuid = appGuid;
        }

        public void TranslateOffsetToPOUAndSourcePosition(string logEntry)
        {
            if (!LogEntryHasAreaAndOffset(logEntry))
            {
                return;
            }
            ref ICompileContext11 comcon = ref _comcon;
            ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(_appGuid);
            comcon = (ICompileContext11)(object)((compileContext is ICompileContext11) ? compileContext : null);
            if (_comcon == null)
            {
                return;
            }
            ExtractAreaAndOffsetFromMessage(logEntry);
            IBreakpoint val = ((ICompileContext2)_comcon).FindBreakpointByCodePosition(_area, _areaOffset, true, out _crashedPOU);
            if (_crashedPOU != null)
            {
                CreateCompletePOUName(_crashedPOU);
                if (val != null)
                {
                    _sourcePosition = _crashedPOU.GetSourcePositionOfBreakpoint(val);
                }
            }
        }

        private bool LogEntryHasAreaAndOffset(string logEntry)
        {
            if (logEntry.Contains("<area>") && logEntry.Contains("<off>"))
            {
                return true;
            }
            return false;
        }

        private void ExtractAreaAndOffsetFromMessage(string exceptionMessage)
        {
            Match match = s_areaValueRegex.Match(exceptionMessage);
            if (match.Success)
            {
                _area = Convert.ToUInt16(match.Groups[1].Value);
            }
            Match match2 = s_offsetValueRegex.Match(exceptionMessage);
            if (match2.Success)
            {
                _areaOffset = Convert.ToUInt32(match2.Groups[1].Value);
            }
        }

        private void CreateCompletePOUName(ICompiledPOU crashedPOU)
        {
            ISignature signatureById = ((ICompileContext)_comcon).GetSignatureById(crashedPOU.SignatureId);
            ISignature4 val = (ISignature4)(object)((signatureById is ISignature4) ? signatureById : null);
            ISignature signatureById2 = ((ICompileContext)_comcon).GetSignatureById(((ISignature)val).ParentSignatureId);
            ISignature4 val2 = (ISignature4)(object)((signatureById2 is ISignature4) ? signatureById2 : null);
            if (val2 != null)
            {
                _libraryName = ((ISignature)val2).LibraryPath;
                if (((ISignature)val).Name.Equals("__MAIN"))
                {
                    _completePOUName = ((ISignature)val2).OrgName;
                }
                else
                {
                    _completePOUName = ((ISignature)val2).OrgName + "." + ((ISignature)val).OrgName;
                }
            }
            else
            {
                _libraryName = ((ISignature)val).LibraryPath;
                _completePOUName = ((ISignature)val).OrgName;
            }
        }
    }
}
