using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class InternalOnlineService
    {
        private static object _owner;

        internal static object Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        internal static event CompoundLoginStartedHandler LoginStarted;

        internal static event CompoundLoginAbortedHandler LoginAborted;

        internal static event CompoundLoginFinishedHandler LoginFinished;

        internal static event CompoundAfterLoginFinishedHandler AfterLoginFinished;

        internal static event EventHandler<BeforeLoginEventArgs> BeforeLogin;

        internal static bool BeginLogin(List<Guid> apps, LoginServiceFlags flags, bool bCompileBeforeLogin, bool bGenerateCodeBeforeLogin, bool bOnlineChange, OnlineChangeDialogHandlerDelegate dlgOnline, LoginDialogHandlerDelegate dlgPrompt)
        {
            string text = string.Empty;
            bool flag = ((int)flags & 0x400) > 0;
            if ((object)InternalOnlineService.LoginStarted != null)
            {
                InternalOnlineService.LoginStarted.Invoke(_owner, new LoginServiceEventArgs(apps, string.Empty));
            }
            if (InternalOnlineService.BeforeLogin != null)
            {
                BeforeLoginEventArgs val = new BeforeLoginEventArgs(flags, dlgOnline, dlgPrompt);
                InternalOnlineService.BeforeLogin(_owner, val);
                flags = val.Flags;
                dlgOnline = val.OnlineChangeDialogHandlerDelegate;
                dlgPrompt = val.LoginDialogHandlerDelegate;
            }
            foreach (Guid app in apps)
            {
                bool flag2 = true;
                bool flag3 = false;
                if (OnlineCommandHelper.IsUpToDate(app) && OnlineCommandHelper.MessageStorageContainsErrors())
                {
                    flag3 = true;
                }
                try
                {
                    if (!(bCompileBeforeLogin || flag3))
                    {
                        goto IL_0111;
                    }
                    if (!(!APEnvironment.LanguageModelMgr.IsUpToDate(app) || flag3))
                    {
                        goto IL_010e;
                    }
                    if (((ILanguageModelManager21)APEnvironment.LanguageModelMgr).Compile(app))
                    {
                        goto IL_00e0;
                    }
                    flag2 = false;
                    text = string.Format(Strings.LoginSvc_Error_CompileCodeFailed, OnlineCommandHelper.GetApplicationNameByGuid(app));
                    if (!flag)
                    {
                        goto IL_00e0;
                    }
                    goto end_IL_009b;
                IL_00e0:
                    if (!bGenerateCodeBeforeLogin || ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GenerateCode(app, bOnlineChange, false))
                    {
                        goto IL_010e;
                    }
                    flag2 = false;
                    text = string.Format(Strings.LoginSvc_Error_GenerateCodeFailed, OnlineCommandHelper.GetApplicationNameByGuid(app));
                    if (!flag)
                    {
                        goto IL_010e;
                    }
                    goto end_IL_009b;
                IL_010e:
                    flag3 = false;
                    goto IL_0111;
                IL_0111:
                    if (!OnlineCommandHelper.LoginWithParameters(app, flags, dlgOnline, dlgPrompt))
                    {
                        flag2 = false;
                        text = string.Format(Strings.LoginSvc_Error_LoginFailed, OnlineCommandHelper.GetApplicationNameByGuid(app));
                        if (!flag)
                        {
                            continue;
                        }
                        break;
                    }
                    flag2 = true;
                    continue;
                end_IL_009b:;
                }
                catch (Exception ex)
                {
                    flag2 = false;
                    text = ex.Message;
                    if (flag)
                    {
                        break;
                    }
                    continue;
                }
                finally
                {
                    if (!flag2)
                    {
                        RaiseLoginAborted(app, text);
                    }
                }
                break;
            }
            if ((object)InternalOnlineService.LoginFinished != null)
            {
                InternalOnlineService.LoginFinished.Invoke(_owner, new LoginServiceEventArgs(apps, string.Empty));
            }
            if ((object)InternalOnlineService.AfterLoginFinished != null)
            {
                InternalOnlineService.AfterLoginFinished.Invoke(_owner, new LoginServiceEventArgs(apps, string.Empty));
            }
            return text == string.Empty;
        }

        internal static void EndLogin(object owner)
        {
            if (_owner == owner)
            {
                _owner = null;
            }
        }

        private static void RaiseLoginAborted(Guid guidFailedApp, string stError)
        {
            //IL_0021: Unknown result type (might be due to invalid IL or missing references)
            //IL_002b: Expected O, but got Unknown
            if ((object)InternalOnlineService.LoginAborted != null)
            {
                List<Guid> list = new List<Guid>(1);
                list.Add(guidFailedApp);
                InternalOnlineService.LoginAborted.Invoke(_owner, new LoginServiceEventArgs(list, stError));
            }
        }
    }
}
