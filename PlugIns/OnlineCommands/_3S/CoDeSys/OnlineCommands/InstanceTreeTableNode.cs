using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.Utilities;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class InstanceTreeTableNode : ITreeTableNode, IComparable
    {
        private const long MIN_VAL = -4611686018427387904L;

        private const long MAX_VAL = 4611686018427387903L;

        private Guid _applicationGuid;

        private string _stApplication;

        private string _stInstance;

        private string _stNormalizedInstancePath;

        private IInstanceFormatter _formatter;

        public Guid ApplicationGuid => _applicationGuid;

        public string Application => _stApplication;

        public string Instance => _stInstance;

        public bool HasChildren => false;

        public int ChildCount => 0;

        public ITreeTableNode Parent => null;

        public InstanceTreeTableNode(Guid applicationGuid, string stApplication, string stInstance, IInstanceFormatter formatter)
        {
            if (stApplication == null)
            {
                throw new ArgumentNullException("stApplication");
            }
            if (stInstance == null)
            {
                throw new ArgumentNullException("stInstance");
            }
            _formatter = formatter;
            _applicationGuid = applicationGuid;
            _stApplication = stApplication;
            _stInstance = stInstance;
        }

        public object GetValue(int nColumnIndex)
        {
            if (nColumnIndex != 0)
            {
                throw new ArgumentOutOfRangeException("nColumnIndex");
            }
            string formattedString = ((_formatter == null) ? _stInstance : _formatter.FormatInstance(ApplicationGuid, _stInstance));
            return GetNodeData(formattedString);
        }

        public void SetValue(int nColumnIndex, object value)
        {
            throw new InvalidOperationException("This node is read-only.");
        }

        public int GetIndex(ITreeTableNode node)
        {
            return -1;
        }

        public void SwapChildren(int nIndex1, int nIndex2)
        {
        }

        public ITreeTableNode GetChild(int nIndex)
        {
            throw new ArgumentOutOfRangeException("nIndex");
        }

        public bool IsEditable(int nColumnIndex)
        {
            return false;
        }

        public int CompareTo(object obj)
        {
            InstanceTreeTableNode instanceTreeTableNode = (InstanceTreeTableNode)obj;
            string normalizedInstancePath = GetNormalizedInstancePath(_stInstance);
            string normalizedInstancePath2 = GetNormalizedInstancePath(instanceTreeTableNode._stInstance);
            return normalizedInstancePath.CompareTo(normalizedInstancePath2);
        }

        private string GetNormalizedInstancePath(string stInstancePath)
        {
            //IL_000b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0011: Expected O, but got Unknown
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_002b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0030: Unknown result type (might be due to invalid IL or missing references)
            //IL_0039: Unknown result type (might be due to invalid IL or missing references)
            //IL_003b: Invalid comparison between I4 and Unknown
            //IL_0048: Unknown result type (might be due to invalid IL or missing references)
            //IL_0066: Unknown result type (might be due to invalid IL or missing references)
            //IL_0068: Invalid comparison between I4 and Unknown
            //IL_0071: Unknown result type (might be due to invalid IL or missing references)
            //IL_0076: Invalid comparison between I4 and Unknown
            //IL_00d5: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00dc: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e5: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e7: Invalid comparison between I4 and Unknown
            if (_stNormalizedInstancePath == null)
            {
                try
                {
                    LStringBuilder val = new LStringBuilder();
                    IScanner val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stInstancePath, false, false, false, false);
                    IToken val3 = null;
                    IToken val4 = null;
                    TokenType val5 = (TokenType)0;
                    TokenType next = val2.GetNext(out val3);
                    while (21 != (int)next)
                    {
                        if (14 == (int)next)
                        {
                            ulong num = 0uL;
                            bool flag = false;
                            Operator val6 = (Operator)0;
                            bool flag2 = false;
                            val2.GetInteger(val3, out num, out flag, out val6, out flag2);
                            long num2 = (long)num;
                            long num3 = 1L;
                            if (15 == (int)val5 && 158 == (int)val2.GetOperator(val4))
                            {
                                num3 = -1L;
                            }
                            num2 *= num3;
                            if (-4611686018427387904L <= num2 && num2 <= 4611686018427387903L)
                            {
                                num2 -= -4611686018427387904L;
                            }
                            val.AppendFormat(num2.ToString("0000000000"), Array.Empty<object>());
                        }
                        else
                        {
                            val.Append(val2.GetTokenText(val3));
                        }
                        val4 = val3;
                        val5 = next;
                        next = val2.GetNext(out val3);
                    }
                    _stNormalizedInstancePath = ((object)val).ToString();
                }
                catch
                {
                    _stNormalizedInstancePath = stInstancePath;
                }
            }
            return _stNormalizedInstancePath;
        }

        protected FindResultsNodeData GetNodeData(string formattedString)
        {
            return new FindResultsNodeData(formattedString, FontStyle.Regular);
        }
    }
}
