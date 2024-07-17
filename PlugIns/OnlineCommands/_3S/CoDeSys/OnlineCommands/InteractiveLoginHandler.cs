using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{3C0BB3AA-A65C-4FC6-A854-93D44AD6D671}")]
    public class InteractiveLoginHandler : IInteractiveLoginHandler
    {
        private IProgressCallback _callback;

        private InteractiveLoginForm _form;

        private InteractiveLoginMode _mode;

        private bool _bCancelledByUser;

        public bool CancelledByUser
        {
            get
            {
                if (_form != null)
                {
                    return _form.IsCancelledByUser;
                }
                return _bCancelledByUser;
            }
        }

        public void BeginInteractiveLogin(InteractiveLoginMode mode, int nTimeoutSeconds, string stDescription, Guid objectGuid, out string stID)
        {
            //IL_0009: Unknown result type (might be due to invalid IL or missing references)
            //IL_000a: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b4: Unknown result type (might be due to invalid IL or missing references)
            //IL_00cb: Unknown result type (might be due to invalid IL or missing references)
            //IL_00fb: Unknown result type (might be due to invalid IL or missing references)
            //IL_0109: Unknown result type (might be due to invalid IL or missing references)
            //IL_010b: Invalid comparison between Unknown and I4
            stID = string.Empty;
            _mode = mode;
            string stDeviceName = "<" + Strings.unknown.ToLowerInvariant() + ">";
            if (objectGuid != Guid.Empty && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, objectGuid))
            {
                try
                {
                    IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, objectGuid).Object;
                    IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
                    if (val != null && val.CommunicationSettings is ICommunicationSettings2 && !string.IsNullOrWhiteSpace(((ICommunicationSettings2)val.CommunicationSettings).Name))
                    {
                        stDeviceName = ((ICommunicationSettings2)val.CommunicationSettings).Name + " [" + val.CommunicationSettings.Address.ToString() + "]";
                    }
                }
                catch
                {
                }
            }
            _form = new InteractiveLoginForm(mode, nTimeoutSeconds, stDescription, stDeviceName);
            if ((int)mode == 1)
            {
                if (_form.ShowDialog((IWin32Window)APEnvironment.FrameForm) == DialogResult.OK)
                {
                    stID = _form.ID;
                }
                return;
            }
            _callback = ((IEngine)APEnvironment.Engine).Frame.StartLengthyOperation();
            _callback.NextTask(Strings.InteractiveLoginName, 100, "");
            _callback.TaskProgress(Strings.InteractiveLoginPressKey);
            _form.Show((IWin32Window)APEnvironment.FrameForm);
            _form.BringToFront();
            _form.Activate();
        }

        public void EndInteractiveLogin(bool bSuccess, Exception ex)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0012: Invalid comparison between Unknown and I4
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            //IL_001e: Invalid comparison between Unknown and I4
            if (_form != null && (int)_mode != 1 && (int)_mode == 2)
            {
                if (_callback != null)
                {
                    _callback.NextTask(Strings.InteractiveLoginName, 1, "");
                    if (bSuccess && ex == null)
                    {
                        _callback.TaskProgress(Strings.InteractiveLoginConfirmed);
                    }
                    else
                    {
                        _callback.TaskProgress(Strings.Aborting);
                    }
                    _callback.Finish();
                }
                if (_form.IsCancelledByUser)
                {
                    _form.Close();
                }
                else if (!_form.IsHandleDestroyed)
                {
                    _form.BringToFront();
                    _form.ConfirmLogin(bSuccess, ex);
                    while (!_form.IsHandleDestroyed && !_form.IsCancelledByUser)
                    {
                        Application.DoEvents();
                    }
                }
            }
            _bCancelledByUser = _form.IsCancelledByUser;
            if (_form != null)
            {
                _form.Dispose();
                _form = null;
            }
        }

        public bool SkipInteractiveLogin(Guid objectGuid)
        {
            return false;
        }
    }
}
