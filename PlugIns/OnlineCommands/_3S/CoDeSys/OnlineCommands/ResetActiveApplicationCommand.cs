using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Online;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    public abstract class ResetActiveApplicationCommand : IStandardCommand, ICommand
    {
        private bool _bIsExecutionInProgress;

        public abstract ResetOption ResetOption { get; }

        public Guid Category => OnlineCommandCategory.Guid;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public string ToolTipText => Description;

        public Icon SmallIcon => null;

        public Icon LargeIcon => null;

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8))
                {
                    return OnlineCommandHelper.CanReset(activeAppObjectGuid);
                }
                return false;
            }
        }

        public virtual string[] BatchCommand => new string[0];

        public string[] CreateBatchArguments()
        {
            //IL_00a1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a6: Invalid comparison between I4 and Unknown
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            try
            {
                _bIsExecutionInProgress = true;
                if (OnlineCommandHelper.PromptExecuteOperation_ActiveApplication((ICommand)(object)this, bPromptInNormalMode: true, null))
                {
                    Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                    string[] array = new string[1];
                    Guid empty = Guid.Empty;
                    array[0] = empty.ToString();
                    if (activeAppObjectGuid == Guid.Empty)
                    {
                        return array;
                    }
                    bool bHaltOnBreakpoint = false;
                    OnlineCommandHelper.CanDownload(activeAppObjectGuid, out bHaltOnBreakpoint);
                    if (bHaltOnBreakpoint && !OnlineCommandHelper.HasTaskKillTargetSetting(activeAppObjectGuid))
                    {
                        string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "QueryFinishCycleReset_Text");
                        if (2 == (int)APEnvironment.MessageService.Prompt(@string, (PromptChoice)2, (PromptResult)2, "QueryFinishCycleReset_Text", Array.Empty<object>()))
                        {
                            array[0] = activeAppObjectGuid.ToString();
                        }
                    }
                    else
                    {
                        array[0] = activeAppObjectGuid.ToString();
                    }
                    return array;
                }
                return null;
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_003d: Unknown result type (might be due to invalid IL or missing references)
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 0);
            }
            Guid guid = OnlineCommandHelper.ActiveAppObjectGuid;
            if (arguments.Length == 1)
            {
                guid = new Guid(arguments[0]);
            }
            if (!(guid == Guid.Empty))
            {
                try
                {
                    OnlineCommandHelper.ResetApplication(guid, ResetOption);
                }
                catch (CancelledByUserException)
                {
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex2)
                {
                    APEnvironment.MessageService.Error(ex2.Message, "ErrorResetActiveApp", Array.Empty<object>());
                }
            }
        }
    }
}
