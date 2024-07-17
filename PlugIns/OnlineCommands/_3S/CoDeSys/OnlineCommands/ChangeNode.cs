using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal sealed class ChangeNode : GenericRootNode, ITreeTableNode
    {
        private const int COL_DESCRIPTION = 0;

        private const int COL_PREVENTS_ONLINECHANGE = 1;

        private static Bitmap IMAGE_ATTENTION;

        private static Dictionary<EPouSetChange, Bitmap> s_htBitmaps;

        private static Bitmap BMP_DEFAULT;

        private IChangedLMObject _changedLMObject;

        static ChangeNode()
        {
            IMAGE_ATTENTION = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.AttentionSmall.ico").Handle);
            s_htBitmaps = new Dictionary<EPouSetChange, Bitmap>();
            Bitmap value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.ApplicationSmall.ico").Handle);
            s_htBitmaps.Add((EPouSetChange)34359738368L, value);
            value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.LibManObjectSmall.ico").Handle);
            s_htBitmaps.Add((EPouSetChange)137438953472L, value);
            value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.Add.ico").Handle);
            s_htBitmaps.Add((EPouSetChange)8589934592L, value);
            value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.Delete.ico").Handle);
            s_htBitmaps.Add((EPouSetChange)549755813888L, value);
            value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.Amount.ico").Handle);
            s_htBitmaps.Add((EPouSetChange)4294967296L, value);
            BMP_DEFAULT = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.Textual.ico").Handle);
        }

        internal ChangeNode(IChangedLMObject changedLMObject)
            : base((EPouSetChange)(changedLMObject.Change & (changedLMObject.Change ^ unchecked((EPouSetChange)(-1)))))
        {
            //IL_0002: Unknown result type (might be due to invalid IL or missing references)
            //IL_0008: Unknown result type (might be due to invalid IL or missing references)
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            _changedLMObject = changedLMObject;
        }

        public override object GetValue(int nColumnIndex)
        {
            switch (nColumnIndex)
            {
                case 0:
                    {
                        Bitmap value = BMP_DEFAULT;
                        if (!s_htBitmaps.TryGetValue(_eReason, out value))
                        {
                            value = BMP_DEFAULT;
                        }
                        return (object)new IconLabelTreeTableViewCellData((Image)value, (object)_changedLMObject.Description);
                    }
                case 1:
                    {
                        EPouSetChange val;
                        unchecked
                        {
                            val = _changedLMObject.Change & (EPouSetChange)(-1);
                        }
                        if ((int)val == 0)
                        {
                            return string.Empty;
                        }
                        return (object)new IconLabelTreeTableViewCellData((Image)IMAGE_ATTENTION, (object)GetLocalizedReason(val));
                    }
                default:
                    throw new ArgumentOutOfRangeException("nColumnIndex");
            }
        }

        private string GetLocalizedReason(EPouSetChange eReason)
        {
            string text = null;
            if ((long)eReason <= 1024L)
            {
                if ((long)eReason <= 32L)
                {
                    if ((long)eReason <= 8L)
                    {
                        ulong val = eReason - EPouSetChange.NewCheckFunctionInserted;
                        if ((long)val <= 3L)
                        {
                            switch ((uint)val)
                            {
                                case 0u:
                                    return Strings.Change_NewCheckFunctionInserted;
                                case 1u:
                                    return Strings.Change_InhibitOnlineChange;
                                case 3u:
                                    return Strings.Change_InterfacesRemoved;
                                case 2u:
                                    goto IL_01f5;
                            }
                        }
                        if ((long)eReason == 8)
                        {
                            return Strings.Change_InterfaceSequenceChanged;
                        }
                    }
                    else
                    {
                        if ((long)eReason == 16)
                        {
                            return Strings.Change_BaseExpressionChanged;
                        }
                        if ((long)eReason == 32)
                        {
                            return Strings.Change_SignatureChangedByChecksumAttribute;
                        }
                    }
                }
                else if ((long)eReason <= 128L)
                {
                    if ((long)eReason == 64)
                    {
                        return Strings.Change_CompileOptionsChanged;
                    }
                    if ((long)eReason == 128)
                    {
                        return Strings.Change_ParentContextNull;
                    }
                }
                else
                {
                    if ((long)eReason == 256)
                    {
                        return Strings.Change_ParentContextChanged;
                    }
                    if ((long)eReason == 512)
                    {
                        return Strings.Change_MemorySettingsChanged;
                    }
                    if ((long)eReason == 1024)
                    {
                        return Strings.Change_TaskLocalGvl;
                    }
                }
            }
            else if ((long)eReason <= 16384L)
            {
                if ((long)eReason <= 4096L)
                {
                    if ((long)eReason == 2048)
                    {
                        return Strings.Change_GlobalInitInCycle;
                    }
                    if ((long)eReason == 4096)
                    {
                        return Strings.Change_TaskListChanged;
                    }
                }
                else
                {
                    if ((long)eReason == 8192)
                    {
                        return Strings.Change_DeviceApplication;
                    }
                    if ((long)eReason == 16384)
                    {
                        return Strings.Change_BlobInitChanged;
                    }
                }
            }
            else if ((long)eReason <= 65536L)
            {
                if ((long)eReason == 32768)
                {
                    return Strings.Change_ExternalSignatureFlagChanged;
                }
                if ((long)eReason == 65536)
                {
                    return Strings.Change_NameExpressionChanged;
                }
            }
            else
            {
                if ((long)eReason == 131072)
                {
                    return Strings.Change_InhibitOnlineChangeOnCodeChanges;
                }
                if ((long)eReason == 262144)
                {
                    return Strings.Change_CheckFunction;
                }
                if ((long)eReason == 524288)
                {
                    return Strings.Change_PrecomTypeMayLeaveDeadRefs;
                }
            }
            goto IL_01f5;
        IL_01f5:
            return eReason.ToString();
        }
    }
}
