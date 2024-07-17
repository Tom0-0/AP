#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class GlobalInitExceptionInformation
    {
        private static Regex s_xmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        private Guid _appGuid;

        private ExceptionInformation _exceptionInformation;

        private IOnlineDevice15 _curOnlineDevice;

        private IAsyncResult _asyncResult;

        private string _lastExceptionInLog = "";

        private string _sourcePositionMsgInLog = "";

        private ExceptionInformationAvailableCallback _callback;

        public string POUName => _exceptionInformation.POUName;

        public string LibraryName => _exceptionInformation.LibraryName;

        public ISourcePosition SourcePosition => _exceptionInformation.SourcePosition;

        public string LastExceptionFromDevice => _lastExceptionInLog;

        public GlobalInitExceptionInformation()
        {
        }

        public GlobalInitExceptionInformation(Guid appGuid)
        {
            _appGuid = appGuid;
            _exceptionInformation = new ExceptionInformation(appGuid);
        }

        public void AbortPendingRequests()
        {
            _callback = null;
            if (_asyncResult != null)
            {
                _ = _asyncResult.AsyncState;
            }
        }

        internal void GetLastExceptionFromDevice(ExceptionInformationAvailableCallback callback)
        {
            _curOnlineDevice = GetOnlineDevice();
            if (_curOnlineDevice == null)
            {
                return;
            }
            try
            {
                if (!((IOnlineDevice)_curOnlineDevice).IsConnected)
                {
                    ((IOnlineDevice)_curOnlineDevice).Connect();
                }
                _callback = callback;
                ILoggerServiceHandler obj = ((IOnlineDevice3)_curOnlineDevice).CreateLoggerServiceHandler();
                ILoggerServiceHandler2 val = (ILoggerServiceHandler2)(object)((obj is ILoggerServiceHandler2) ? obj : null);
                _asyncResult = val.BeginGetLoggerEntries((AsyncCallback)CBGetNewLogEntries, (object)val, 0u, 0u, "");
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.Message, "ErrorGettingLastExceptionFromDevice", Array.Empty<object>());
            }
        }

        private IOnlineDevice15 GetOnlineDevice()
        {
            Guid deviceObjectGuid = GetDeviceObjectGuid(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, _appGuid);
            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(deviceObjectGuid);
            return (IOnlineDevice15)(object)((onlineDevice is IOnlineDevice15) ? onlineDevice : null);
        }

        private Guid GetDeviceObjectGuid(int nProjectHandle, Guid applicationGuid)
        {
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, applicationGuid);
            Debug.Assert(metaObjectStub != null);
            while (metaObjectStub.ParentObjectGuid != Guid.Empty)
            {
                metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
                Debug.Assert(metaObjectStub != null);
                if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    return metaObjectStub.ObjectGuid;
                }
            }
            return Guid.Empty;
        }

        private void CBGetNewLogEntries(IAsyncResult asyncResult)
        {
            try
            {
                object asyncState = asyncResult.AsyncState;
                ILoggerServiceHandler val = (ILoggerServiceHandler)((asyncState is ILoggerServiceHandler) ? asyncState : null);
                List<ILoggerEntries> list = new List<ILoggerEntries>();
                list.AddRange(val.EndGetLoggerEntries(asyncResult));
                string text = "";
                DateTime dateTime = DateTime.MinValue;
                DateTime dateTime2 = DateTime.MinValue;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        ILoggerEntries val2 = list[j];
                        //val2.Info;
                        if ((i != 0 || !MessageContainsException(val2)) && (i != 1 || !MessageContainsError(val2)))
                        {
                            continue;
                        }
                        if (val2.TimeStamp >= dateTime)
                        {
                            text = val2.Info;
                            dateTime = val2.TimeStamp;
                            if (j < list.Count - 1 && NextMessageContainsSourcePosition(list[j + 1]))
                            {
                                _sourcePositionMsgInLog = list[j + 1].Info;
                                j++;
                            }
                        }
                        if (val2.TimeStamp >= dateTime2)
                        {
                            dateTime2 = val2.TimeStamp;
                        }
                    }
                    if (text != "")
                    {
                        break;
                    }
                }
                if (!(dateTime2 != dateTime))
                {
                    _exceptionInformation.TranslateOffsetToPOUAndSourcePosition(_sourcePositionMsgInLog);
                    _lastExceptionInLog = RemoveXMLTags(text);
                    if (_callback != null)
                    {
                        _callback();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                _asyncResult = null;
                _callback = null;
            }
        }

        private string RemoveXMLTags(string logEntry)
        {
            return s_xmlRegex.Replace(logEntry, string.Empty);
        }

        private bool MessageContainsException(ILoggerEntries loggerEntry)
        {
            //IL_0004: Unknown result type (might be due to invalid IL or missing references)
            //IL_000a: Invalid comparison between Unknown and I4
            if (loggerEntry != null && (int)loggerEntry.GetSeverity() == 3)
            {
                return true;
            }
            return false;
        }

        private bool MessageContainsError(ILoggerEntries loggerEntry)
        {
            //IL_0004: Unknown result type (might be due to invalid IL or missing references)
            //IL_000a: Invalid comparison between Unknown and I4
            if (loggerEntry != null && (int)loggerEntry.GetSeverity() == 2)
            {
                return true;
            }
            return false;
        }

        private bool NextMessageContainsSourcePosition(ILoggerEntries loggerEntry)
        {
            //IL_0004: Unknown result type (might be due to invalid IL or missing references)
            //IL_000a: Invalid comparison between Unknown and I4
            if (loggerEntry != null && (int)loggerEntry.GetSeverity() == 3 && loggerEntry.Info.Contains("*SOURCEPOSITION*"))
            {
                return true;
            }
            return false;
        }
    }
}
