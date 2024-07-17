using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{560CFF22-3AAB-4707-853D-91B6D702C666}")]
    public class LoginServiceWrapper : ILoginService3, ILoginService2, ILoginService
    {
        private LoginServiceFlags _flags = (LoginServiceFlags)1;

        private List<Guid> _apps = new List<Guid>();

        private OnlineChangeDialogHandlerDelegate _dlgOnline;

        private LoginDialogHandlerDelegate _dlgPrompt;

        private bool _bGenerateCodeBeforeLogin;

        private bool _bCompileBeforeLogin;

        private bool _bOnlineChange = true;

        public LoginServiceFlags ServiceFlags => _flags;

        public event CompoundLoginStartedHandler CompoundLoginStarted;

        public event CompoundLoginAbortedHandler CompoundLoginAborted;

        public event CompoundLoginFinishedHandler CompoundLoginFinished;

        public event CompoundAfterLoginFinishedHandler CompoundAfterLoginFinished;

        public event EventHandler<BeforeLoginEventArgs> BeforeLogin;

        public LoginServiceWrapper()
        {
            //IL_0002: Unknown result type (might be due to invalid IL or missing references)
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_0030: Expected O, but got Unknown
            //IL_0037: Unknown result type (might be due to invalid IL or missing references)
            //IL_0041: Expected O, but got Unknown
            //IL_0048: Unknown result type (might be due to invalid IL or missing references)
            //IL_0052: Expected O, but got Unknown
            //IL_0059: Unknown result type (might be due to invalid IL or missing references)
            //IL_0063: Expected O, but got Unknown
            InternalOnlineService.LoginStarted += new CompoundLoginStartedHandler(LoginStarted);
            InternalOnlineService.LoginAborted += new CompoundLoginAbortedHandler(LoginAborted);
            InternalOnlineService.LoginFinished += new CompoundLoginFinishedHandler(LoginFinished);
            InternalOnlineService.AfterLoginFinished += new CompoundAfterLoginFinishedHandler(AfterLoginFinished);
            InternalOnlineService.BeforeLogin += InternalBeforeLogin;
        }

        private void InternalBeforeLogin(object sender, BeforeLoginEventArgs e)
        {
            this.BeforeLogin?.Invoke(sender, e);
        }

        private void AfterLoginFinished(object sender, LoginServiceEventArgs e)
        {
            if ((object)this.CompoundAfterLoginFinished != null)
            {
                this.CompoundAfterLoginFinished.Invoke(sender, e);
            }
        }

        private void LoginFinished(object sender, LoginServiceEventArgs e)
        {
            if ((object)this.CompoundLoginFinished != null)
            {
                this.CompoundLoginFinished.Invoke(sender, e);
            }
        }

        private void LoginAborted(object sender, LoginServiceEventArgs e)
        {
            if ((object)this.CompoundLoginAborted != null)
            {
                this.CompoundLoginAborted.Invoke(sender, e);
            }
        }

        private void LoginStarted(object sender, LoginServiceEventArgs e)
        {
            if ((object)this.CompoundLoginStarted != null)
            {
                this.CompoundLoginStarted.Invoke(sender, e);
            }
        }

        public void ConfigureLogin(object owner, List<Guid> guidApplications, LoginServiceFlags flags, OnlineChangeDialogDelegate dlgOnlineChange, PromptDialogDelegate dlgPromptDialog)
        {

            LoginDialogHandlerDelegate dlgPromptDialog2 = null;
            if (dlgPromptDialog != null)
            {
                dlgPromptDialog2 = (LoginDialogHandlerDelegate)delegate (Guid appguid, PromptChoice choice, LoginPromptType prompttype, string[] options)
                {
                    string[] array = new string[Math.Min(3, options.Length)];
                    options.CopyTo(array, 0);
                    prompttype.ToString();
                    return dlgPromptDialog.Invoke(appguid, choice, prompttype.ToString(), array);
                };
            }
            OnlineChangeDialogHandlerDelegate dlgOnlineChange2 = null;
            if (dlgOnlineChange != null)
            {
                dlgOnlineChange2 = (OnlineChangeDialogHandlerDelegate)delegate (Guid appguid, OnlineChangePromptType promptType, out BootProjectTransferMode transferMode)
                {
                    //IL_001d: Unknown result type (might be due to invalid IL or missing references)
                    transferMode = (BootProjectTransferMode)0;
                    return dlgOnlineChange.Invoke(appguid, promptType.ToString(), new string[3]);
                };
            }
            ConfigureLogin(owner, (IList<Guid>)guidApplications, flags, dlgOnlineChange2, dlgPromptDialog2);
        }

        public void ConfigureLogin(object owner, IList<Guid> guidApplications, LoginServiceFlags flags, OnlineChangeDialogHandlerDelegate dlgOnlineChange, LoginDialogHandlerDelegate dlgPromptDialog)
        {

            if (owner == null || InternalOnlineService.Owner != null)
            {
                throw new ArgumentException(Strings.LoginSvc_Error_InvalidOwner);
            }
            LoginServiceFlags val;
            if (((int)flags & 4) > 0 && ((int)flags & 2) > 0)
            {
                string loginSvc_Error_ConflictingFlags = Strings.LoginSvc_Error_ConflictingFlags;
                val = (LoginServiceFlags)4;
                string arg = val.ToString();
                val = (LoginServiceFlags)2;
                throw new ArgumentException(string.Format(loginSvc_Error_ConflictingFlags, arg, val.ToString()));
            }
            if (((int)flags & 1) > 0 && (long)((ulong)flags & 0xFFFFFFFFEL) > 0L)
            {
                string loginSvc_Error_ConflictingFlags2 = Strings.LoginSvc_Error_ConflictingFlags;
                val = (LoginServiceFlags)1;
                throw new ArgumentException(string.Format(loginSvc_Error_ConflictingFlags2, val.ToString(), ((uint)(flags - 1)).ToString("0x00000000")));
            }
            _apps = guidApplications.ToList();
            _flags = flags;
            _dlgOnline = dlgOnlineChange;
            _dlgPrompt = dlgPromptDialog;
            InternalOnlineService.Owner = owner;
            _bGenerateCodeBeforeLogin = ((int)flags & 0x20) > 0;
            _bCompileBeforeLogin = ((int)flags & 0x10) > 0 || _bGenerateCodeBeforeLogin;
            _bOnlineChange = ((int)flags & 1) > 0 || ((int)flags & 2) > 0;
        }

        public void EndLogin(object owner)
        {
            if (owner == null || InternalOnlineService.Owner != owner)
            {
                throw new InvalidOperationException(Strings.LoginSvc_Error_InvalidOwner);
            }
            InternalOnlineService.EndLogin(owner);
        }

        public bool BeginLogin(object owner)
        {
            //IL_0023: Unknown result type (might be due to invalid IL or missing references)
            if (owner == null || InternalOnlineService.Owner != owner)
            {
                throw new InvalidOperationException(Strings.LoginSvc_Error_InvalidOwner);
            }
            _ = string.Empty;
            return InternalOnlineService.BeginLogin(_apps, _flags, _bCompileBeforeLogin, _bGenerateCodeBeforeLogin, _bOnlineChange, _dlgOnline, _dlgPrompt);
        }

        public PromptResult ShowDefaultLoginPrompt(Guid applicationGuid, PromptChoice choice, LoginPromptType promptType, string[] options)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0002: Unknown result type (might be due to invalid IL or missing references)
            //IL_0005: Unknown result type (might be due to invalid IL or missing references)
            return OnlineCommandHelper.DefaultLoginDelegate(applicationGuid, choice, promptType, options);
        }

        public PromptOnlineChangeResult ShowDefaultOnlineChangePrompt(Guid applicationGuid, OnlineChangePromptType promptType, out BootProjectTransferMode transferMode)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0003: Unknown result type (might be due to invalid IL or missing references)
            return OnlineCommandHelper.DefaultOnlineChangeDelegate(applicationGuid, promptType, out transferMode);
        }
    }
}
