#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using _3S.CoDeSys.TextDocument;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	public class TabularDeclarationModel
	{
		private UnresolvedDeclarationContext _unresolvedContext;

		private LinkedList<ModelToken> _tokens = new LinkedList<ModelToken>();

		private TabularDeclarationHeader _header;

		private TabularDeclarationList _list;

		internal TabularDeclarationHeader Header => _header;

		internal TabularDeclarationList List => _list;

		internal TabularDeclarationModel(UnresolvedDeclarationContext unresolvedContext)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			_unresolvedContext = unresolvedContext;
		}

		internal bool ReadText(ITextDocument text, out string stMessage, out ResolvedDeclarationContext resolvedContext)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Invalid comparison between Unknown and I4
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected I4, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Invalid comparison between Unknown and I4
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Invalid comparison between Unknown and I4
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Invalid comparison between Unknown and I4
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Invalid comparison between Unknown and I4
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Invalid comparison between Unknown and I4
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Invalid comparison between Unknown and I4
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected I4, but got Unknown
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Expected I4, but got Unknown
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Invalid comparison between Unknown and I4
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Invalid comparison between Unknown and I4
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Invalid comparison between Unknown and I4
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Invalid comparison between Unknown and I4
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Expected I4, but got Unknown
			_tokens.Clear();
			_header = null;
			_list = null;
			if (!CheckSyntaxAndCalculateResolvedContext(text, out stMessage, out resolvedContext))
			{
				return false;
			}
			IScanner val = APEnvironment.LanguageModelMgr.CreateScanner(text.Text, true, true, true, true);
			Operator val2 = (Operator)0;
			bool flag = false;
			bool flag2 = false;
			IToken val3 = default(IToken);
			while ((int)val.GetNext(out val3) != 21)
			{
				TokenType type = val3.Type;
				ModelTokenType modelTokenType;
				if ((int)type <= 12)
				{
					switch ((int)type - 2)
					{
					case 0:
						goto IL_0342;
					case 1:
						goto IL_0391;
					case 2:
						goto IL_03b2;
					}
					if ((int)type != 12)
					{
						goto IL_03bc;
					}
					modelTokenType = ModelTokenType.EndOfLine;
					flag2 = false;
				}
				else
				{
					if ((int)type == 15)
					{
						Operator @operator = val.GetOperator(val3);
						if ((int)@operator <= 119)
						{
							if ((int)@operator != 63)
							{
								if ((int)@operator != 66)
								{
									switch ((int)@operator - 81)
									{
									case 6:
										break;
									case 7:
										goto IL_019f;
									case 38:
										goto IL_01a8;
									case 37:
										goto IL_01b1;
									case 12:
										goto IL_01ba;
									case 22:
										goto IL_01ce;
									case 18:
										goto IL_01e5;
									case 19:
										goto IL_0202;
									case 24:
										goto IL_0231;
									case 25:
										goto IL_023e;
									case 26:
										goto IL_024b;
									case 27:
										goto IL_0258;
									case 28:
										goto IL_0265;
									case 30:
										goto IL_0272;
									case 29:
										goto IL_027f;
									case 31:
										goto IL_028c;
									case 33:
										goto IL_0299;
									case 32:
										goto IL_02a6;
									case 36:
										goto IL_02b3;
									case 35:
										goto IL_02bd;
									case 0:
										goto IL_02c7;
									case 16:
										goto IL_02db;
									case 10:
										goto IL_02e8;
									default:
										goto IL_032d;
									}
									modelTokenType = ModelTokenType.Function;
								}
								else
								{
									modelTokenType = ModelTokenType.Constant;
								}
							}
							else
							{
								modelTokenType = ModelTokenType.At;
							}
						}
						else
						{
							switch ((int)@operator - 163)
							{
							case 0:
								goto IL_01d8;
							case 4:
								goto IL_020f;
							case 1:
								goto IL_0311;
							case 2:
							case 3:
								goto IL_032d;
							}
							if ((int)@operator != 172)
							{
								if ((int)@operator != 187)
								{
									goto IL_032d;
								}
								modelTokenType = ModelTokenType.Property;
							}
							else
							{
								modelTokenType = ModelTokenType.Semicolon;
								flag = false;
							}
						}
						goto IL_0335;
					}
					if ((int)type != 19)
					{
						goto IL_03bc;
					}
					modelTokenType = ModelTokenType.Whitespace;
				}
				goto IL_03c4;
				IL_0258:
				modelTokenType = ModelTokenType.VarExternal;
				goto IL_0335;
				IL_024b:
				modelTokenType = ModelTokenType.VarConfig;
				goto IL_0335;
				IL_03b2:
				modelTokenType = ModelTokenType.Pragma;
				goto IL_03c4;
				IL_0391:
				modelTokenType = ModelTokenType.DocumentationComment;
				goto IL_03c4;
				IL_0342:
				modelTokenType = (val.GetTokenText(val3).StartsWith("//") ? ((!flag2) ? ModelTokenType.CPlusPlusCommentOccupyingWholeLine : ModelTokenType.CPlusPlusCommentInMixedLine) : ((!flag2) ? ModelTokenType.PascalCommentOccupyingWholeLine : ModelTokenType.PascalCommentInMixedLine));
				goto IL_03c4;
				IL_01e5:
				modelTokenType = (ModelTokenType)((!flag) ? 512 : 128);
				goto IL_0335;
				IL_03c4:
				if (modelTokenType != ModelTokenType.Whitespace && modelTokenType != ModelTokenType.EndOfLine)
				{
					flag2 = true;
				}
				_tokens.AddLast(new ModelToken(modelTokenType, val.GetTokenText(val3), val3.SourceOffset));
				continue;
				IL_01ce:
				modelTokenType = ModelTokenType.Type;
				goto IL_0335;
				IL_0335:
				val2 = val.GetOperator(val3);
				goto IL_03c4;
				IL_01ba:
				modelTokenType = ModelTokenType.Program;
				goto IL_0335;
				IL_01b1:
				modelTokenType = ModelTokenType.Method;
				goto IL_0335;
				IL_0231:
				modelTokenType = ModelTokenType.Var;
				goto IL_0335;
				IL_0202:
				modelTokenType = ModelTokenType.Union;
				goto IL_0335;
				IL_023e:
				modelTokenType = ModelTokenType.VarAccess;
				goto IL_0335;
				IL_01a8:
				modelTokenType = ModelTokenType.Interface;
				goto IL_0335;
				IL_03bc:
				modelTokenType = ModelTokenType.Text;
				goto IL_03c4;
				IL_020f:
				modelTokenType = (ModelTokenType)(((int)val2 != 163) ? 128 : 2048);
				goto IL_0335;
				IL_019f:
				modelTokenType = ModelTokenType.FunctionBlock;
				goto IL_0335;
				IL_01d8:
				modelTokenType = ModelTokenType.Colon;
				goto IL_0335;
				IL_0311:
				if (!flag)
				{
					modelTokenType = ModelTokenType.Assign;
					flag = true;
				}
				else
				{
					modelTokenType = ModelTokenType.Text;
				}
				goto IL_0335;
				IL_032d:
				modelTokenType = ModelTokenType.Text;
				goto IL_0335;
				IL_02e8:
				modelTokenType = ModelTokenType.Persistent;
				goto IL_0335;
				IL_02db:
				modelTokenType = ModelTokenType.Retain;
				goto IL_0335;
				IL_02c7:
				modelTokenType = ModelTokenType.EndVar;
				goto IL_0335;
				IL_02bd:
				modelTokenType = ModelTokenType.Extends;
				goto IL_0335;
				IL_02b3:
				modelTokenType = ModelTokenType.Implements;
				goto IL_0335;
				IL_02a6:
				modelTokenType = ModelTokenType.VarTemp;
				goto IL_0335;
				IL_0299:
				modelTokenType = ModelTokenType.VarStat;
				goto IL_0335;
				IL_028c:
				modelTokenType = ModelTokenType.VarOutput;
				goto IL_0335;
				IL_027f:
				modelTokenType = ModelTokenType.VarInput;
				goto IL_0335;
				IL_0272:
				modelTokenType = ModelTokenType.VarInOut;
				goto IL_0335;
				IL_0265:
				modelTokenType = ModelTokenType.VarGlobal;
				goto IL_0335;
			}
			ConsolidateCommentTokens();
			ConsolidateTextTokens();
			UnresolvedDeclarationContext unresolvedContext = _unresolvedContext;
			switch ((int)unresolvedContext - 1)
			{
			case 1:
			case 3:
			case 4:
				_header = new TabularDeclarationHeader(_tokens);
				_list = new TabularDeclarationList(_tokens, this);
				break;
			case 0:
				_header = null;
				_list = new TabularDeclarationList(_tokens, this);
				break;
			case 2:
				_header = new TabularDeclarationHeader(_tokens);
				_list = null;
				break;
			default:
				Debug.Fail("Unknown context.");
				break;
			}
			if (_list != null)
			{
				string text2 = _list.CheckUniqueVariableNames(null);
				if (text2 != null)
				{
					stMessage = string.Format(Resources.MultipleDeclaration, text2);
					return false;
				}
			}
			return true;
		}

		private bool CheckSyntaxAndCalculateResolvedContext(ITextDocument text, out string stMessage, out ResolvedDeclarationContext resolvedContext)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected I4, but got Unknown
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Invalid comparison between Unknown and I4
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Invalid comparison between Unknown and I4
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Invalid comparison between Unknown and I4
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Invalid comparison between Unknown and I4
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Invalid comparison between Unknown and I4
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Invalid comparison between Unknown and I4
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Invalid comparison between Unknown and I4
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Invalid comparison between Unknown and I4
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Invalid comparison between Unknown and I4
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Invalid comparison between Unknown and I4
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Invalid comparison between Unknown and I4
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Invalid comparison between Unknown and I4
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Invalid comparison between Unknown and I4
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Invalid comparison between Unknown and I4
			Debug.Assert(text != null);
			string text2 = "{allowpaths}" + text.Text;
			IScanner val = APEnvironment.LanguageModelMgr.CreateScanner(text2, false, false, true, false);
			IParser obj = APEnvironment.LanguageModelMgr.CreateParser(val);
			IParser3 val2 = (IParser3)(object)((obj is IParser3) ? obj : null);
			if (val2 == null)
			{
				stMessage = string.Empty;
				resolvedContext = ResolvedDeclarationContext.DataUnitType;
				return false;
			}
			UnresolvedDeclarationContext unresolvedContext = _unresolvedContext;
			switch ((int)unresolvedContext)
			{
			case 2:
			case 3:
			case 4:
			case 5:
			{
				ISignature val6 = val2.ParseInterface();
				if (val6 == null)
				{
					stMessage = string.Empty;
					resolvedContext = ResolvedDeclarationContext.DataUnitType;
					return false;
				}
				if (val6.Messages != null)
				{
					IMessage[] messages = val6.Messages;
					foreach (IMessage val7 in messages)
					{
						if ((int)val7.Severity == 2 || (int)val7.Severity == 1)
						{
							stMessage = val7.Text;
							resolvedContext = ResolvedDeclarationContext.DataUnitType;
							return false;
						}
					}
				}
				if (val6.All != null)
				{
					IVariable[] all = val6.All;
					foreach (IVariable val8 in all)
					{
						if (val8.Initial != null && ((IExprement)val8.Initial).ToString() == "!!!'ERROR'!!!")
						{
							stMessage = string.Format(Resources.ErrorInInitializationValue, val8.OrgName);
							resolvedContext = ResolvedDeclarationContext.DataUnitType;
							return false;
						}
						if (val8.Type != null && (int)val8.Type.Class == 28 && ((object)val8.Type).ToString() == "???")
						{
							stMessage = string.Format(Resources.ErrorInDataType, val8.OrgName);
							resolvedContext = ResolvedDeclarationContext.DataUnitType;
							return false;
						}
					}
				}
				Operator pOUType = val6.POUType;
				if ((int)pOUType <= 88)
				{
					if ((int)pOUType == 87)
					{
						resolvedContext = ResolvedDeclarationContext.Function;
						break;
					}
					if ((int)pOUType == 88)
					{
						resolvedContext = ResolvedDeclarationContext.FunctionBlock;
						break;
					}
				}
				else
				{
					if ((int)pOUType == 93)
					{
						resolvedContext = ResolvedDeclarationContext.Program;
						break;
					}
					if ((int)pOUType == 118)
					{
						resolvedContext = (((int)_unresolvedContext == 5) ? ResolvedDeclarationContext.InterfaceMethod : ResolvedDeclarationContext.POUMethod);
						break;
					}
					if ((int)pOUType == 119)
					{
						resolvedContext = ResolvedDeclarationContext.Interface;
						break;
					}
				}
				resolvedContext = ResolvedDeclarationContext.DataUnitType;
				break;
			}
			case 1:
			{
				ISignature val3 = val2.ParseGlobalVarlist("gvl_" + DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
				if (val3 == null)
				{
					stMessage = string.Empty;
					resolvedContext = ResolvedDeclarationContext.DataUnitType;
					return false;
				}
				if (val3.Messages != null)
				{
					IMessage[] messages = val3.Messages;
					foreach (IMessage val4 in messages)
					{
						if ((int)val4.Severity == 2 || (int)val4.Severity == 1)
						{
							stMessage = val4.Text;
							resolvedContext = ResolvedDeclarationContext.DataUnitType;
							return false;
						}
					}
				}
				if (val3.All != null)
				{
					IVariable[] all = val3.All;
					foreach (IVariable val5 in all)
					{
						if (val5.Initial != null && ((IExprement)val5.Initial).ToString() == "!!!'ERROR'!!!")
						{
							stMessage = string.Format(Resources.ErrorInInitializationValue, val5.OrgName);
							resolvedContext = ResolvedDeclarationContext.DataUnitType;
							return false;
						}
					}
				}
				resolvedContext = ResolvedDeclarationContext.GlobalVariableList;
				break;
			}
			case 0:
				resolvedContext = ResolvedDeclarationContext.DataUnitType;
				break;
			case 8:
				resolvedContext = ResolvedDeclarationContext.InterfacePropertyAccessor;
				break;
			case 7:
				resolvedContext = ResolvedDeclarationContext.POUPropertyAccessor;
				break;
			case 6:
				resolvedContext = ResolvedDeclarationContext.Property;
				break;
			default:
				throw new NotImplementedException();
			}
			val = APEnvironment.LanguageModelMgr.CreateScanner(text.Text, false, false, true, false);
			IToken val9 = default(IToken);
			while ((int)val.GetNext(out val9) != 21)
			{
				if ((int)val9.Type != 4)
				{
					continue;
				}
				string text3 = val.GetPragma(val9).ToLowerInvariant().Trim();
				if (!text3.StartsWith("if"))
				{
					switch (text3)
					{
					case "else":
					case "else_if":
					case "end_if":
						break;
					default:
						continue;
					}
				}
				stMessage = Resources.ConditionalsNotSupported;
				return false;
			}
			stMessage = null;
			return true;
		}

		internal void RefreshList()
		{
			_list = new TabularDeclarationList(_tokens, this);
		}

		internal void WriteText(LStringBuilder sb)
		{
			foreach (ModelToken token in _tokens)
			{
				sb.Append(token.Text);
			}
		}

		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			LStringBuilder val = new LStringBuilder();
			WriteText(val);
			return ((object)val).ToString();
		}

		private void ConsolidateCommentTokens()
		{
			for (LinkedListNode<ModelToken> linkedListNode = _tokens.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.HasType(ModelTokenType.AnyComment))
				{
					ModelTokenType type = linkedListNode.Value.Type;
					LinkedListNode<ModelToken> next = linkedListNode.Next;
					while (next != null)
					{
						if (next.Value.HasType(ModelTokenType.AnyBlank))
						{
							next = next.Next;
							continue;
						}
						if (!next.Value.HasType(type))
						{
							break;
						}
						LinkedListNode<ModelToken> next2 = linkedListNode.Next;
						while (next2 != next)
						{
							linkedListNode.Value.Text += next2.Value.Text;
							LinkedListNode<ModelToken> node = next2;
							next2 = next2.Next;
							_tokens.Remove(node);
						}
						linkedListNode.Value.Text += next.Value.Text;
						_tokens.Remove(next);
						next = linkedListNode.Next;
					}
					if ((type == ModelTokenType.CPlusPlusCommentOccupyingWholeLine || type == ModelTokenType.CPlusPlusCommentInMixedLine || type == ModelTokenType.DocumentationComment) && linkedListNode != null && linkedListNode.Next != null && linkedListNode.Next.Value.HasType(ModelTokenType.EndOfLine))
					{
						linkedListNode.Value.Text = linkedListNode.Value.Text + linkedListNode.Next.Value.Text;
						_tokens.Remove(linkedListNode.Next);
					}
				}
			}
		}

		private void ConsolidateTextTokens()
		{
			LinkedListNode<ModelToken> linkedListNode = _tokens.First;
			while (linkedListNode != null)
			{
				if (linkedListNode.Value.HasType(ModelTokenType.Text))
				{
					LinkedListNode<ModelToken> linkedListNode2 = linkedListNode;
					LinkedListNode<ModelToken> linkedListNode3 = linkedListNode;
					linkedListNode = linkedListNode.Next;
					while (linkedListNode != null)
					{
						if (linkedListNode.Value.HasType(ModelTokenType.AnyBlank))
						{
							linkedListNode = linkedListNode.Next;
							continue;
						}
						if (linkedListNode.Value.HasType(ModelTokenType.Text))
						{
							linkedListNode3 = linkedListNode;
							linkedListNode = linkedListNode.Next;
							continue;
						}
						if (!linkedListNode.Value.HasType(ModelTokenType.AnyComment))
						{
							break;
						}
						linkedListNode = linkedListNode.Next;
					}
					if (linkedListNode2 != linkedListNode3)
					{
						linkedListNode2.Value.Text = new ModelTokenRange(linkedListNode2, linkedListNode3.Next).GetText();
						new ModelTokenRange(linkedListNode2.Next, linkedListNode3.Next).Remove();
					}
				}
				if (linkedListNode != null)
				{
					linkedListNode = linkedListNode.Next;
				}
			}
		}
	}
}
