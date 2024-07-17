#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.ProjectArchive;
using _3S.CoDeSys.Utilities;
using _3S.CoDeSys.WorkspaceObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class ProjectOptionsHelper
    {
        private static readonly string SUB_KEY = "{FE9F63D4-9E34-43ce-9322-D46747A00F33}";

        private static readonly Guid GUID_WORKSPACEOBJECT = new Guid("{6470A90F-B7CB-43ac-9AE5-94B2338B4573}");

        private static readonly Guid GUID_COMPILE_INFO_ARCHIVE = new Guid("{B0B53F83-AF78-49aa-8133-0063F476BD7C}");

        private static readonly string SOURCEDOWNLOAD_CONTENT = "SourceDownloadContent";

        private static readonly string SOURCEDOWNLOAD_CONTENT2 = "SourceDownloadContent2";

        private static readonly string SOURCEDOWNLOAD_TIMING = "SourceDownloadTiming";

        private static readonly string SOURCEDOWNLOAD_TIMING2 = "SourceDownloadTiming2";

        private static readonly string SOURCEDOWNLOAD_DEVICE = "SourceDownloadDevice";

        private static readonly string SOURCEDOWNLOAD_COMPACT = "SourceDownloadCompact";

        private static readonly string SOURCEDOWNLOAD_WARNED_OF_MISSING_DOWNLOAD_INFO = "SourceDownloadWarnedOfMissingDownloadInfo";

        private static readonly string MULTIPLEDOWNLOAD_APPLICATIONS = "MultipleDownloadApplications";

        private static readonly string MULTIPLEDOWNLOAD_SORTED_APPLICATIONS = "MultipleDownloadSortedApplications";

        private static readonly string MULTIPLEDOWNLOAD_SORTED_EXTENSIONS = "MultipleDownloadSortedExtensions";

        private static readonly string MULTIPLEDOWNLOAD_ONLINECHANGEOPTION = "MultipleDownloadOnlineChangeOption";

        private static readonly string MULTIPLEDOWNLOAD_DELETEOLDAPPS = "MultipleDownloadDeleteOldApps";

        private static readonly string MULTIPLEDOWNLOAD_STARTALLAPPS = "MultipleDownloadStartAllApps";

        private static readonly string MULTIPLEDOWNLOAD_INITPERSISTENTVARS = "MultipleDownloadInitPersistentVars";

        private static readonly string MULTIPLEDOWNLOAD_DONOTRELEASEFORCEDVARIABLES = "MultipleDownloadDoNotReleaseForcedVariables";

        private static readonly string IGNORE_OUTDATED_DEVDESCS = "IgnoreOutdatedDevdescsInTheFuture";

        public static bool IgnoreOutdatedDevdescsInTheFuture
        {
            get
            {
                if (OptionKey.HasValue(IGNORE_OUTDATED_DEVDESCS, typeof(bool)))
                {
                    return (bool)OptionKey[IGNORE_OUTDATED_DEVDESCS];
                }
                return false;
            }
            set
            {
                OptionKey[IGNORE_OUTDATED_DEVDESCS] = (object)value;
            }
        }

        public static Guid[] SourceDownloadContent2
        {
            get
            {
                if (OptionKey.HasValue(SOURCEDOWNLOAD_CONTENT2, typeof(Guid[])))
                {
                    return (Guid[])OptionKey[SOURCEDOWNLOAD_CONTENT2];
                }
                if (OptionKey.HasValue(SOURCEDOWNLOAD_CONTENT, typeof(int)))
                {
                    if ((int)OptionKey[SOURCEDOWNLOAD_CONTENT] == 1)
                    {
                        return APEnvironment.ProjectArchiveCategories.Select((IProjectArchiveCategory c) => TypeGuidAttribute.FromObject((object)c).Guid).ToArray();
                    }
                    return new Guid[0];
                }
                return new Guid[0];
            }
            set
            {
                OptionKey[SOURCEDOWNLOAD_CONTENT2] = (object)value;
                if (value != null && value.Length != 0)
                {
                    OptionKey[SOURCEDOWNLOAD_CONTENT] = (object)1;
                }
                else
                {
                    OptionKey[SOURCEDOWNLOAD_CONTENT] = (object)0;
                }
            }
        }

        public static int SourceDownloadTiming2
        {
            get
            {
                if (OptionKey.HasValue(SOURCEDOWNLOAD_TIMING2, typeof(int)))
                {
                    return (int)OptionKey[SOURCEDOWNLOAD_TIMING2];
                }
                if (OptionKey.HasValue(SOURCEDOWNLOAD_TIMING, typeof(int)))
                {
                    return (int)OptionKey[SOURCEDOWNLOAD_TIMING];
                }
                return 4;
            }
            set
            {
                OptionKey[SOURCEDOWNLOAD_TIMING2] = (object)value;
                if (value > 4)
                {
                    OptionKey[SOURCEDOWNLOAD_TIMING] = (object)2;
                }
                else
                {
                    OptionKey[SOURCEDOWNLOAD_TIMING] = (object)value;
                }
            }
        }

        public static int SourceDownloadTiming
        {
            get
            {
                if (OptionKey.HasValue(SOURCEDOWNLOAD_TIMING, typeof(int)))
                {
                    return (int)OptionKey[SOURCEDOWNLOAD_TIMING];
                }
                return 4;
            }
            set
            {
                OptionKey[SOURCEDOWNLOAD_TIMING] = (object)value;
            }
        }

        public static Guid SourceDownloadDevice
        {
            get
            {
                if (OptionKey.HasValue(SOURCEDOWNLOAD_DEVICE, typeof(Guid)))
                {
                    return (Guid)OptionKey[SOURCEDOWNLOAD_DEVICE];
                }
                return Guid.Empty;
            }
            set
            {
                OptionKey[SOURCEDOWNLOAD_DEVICE] = (object)value;
            }
        }

        public static bool SourceDownloadCompact
        {
            get
            {
                if (OptionKey.HasValue(SOURCEDOWNLOAD_COMPACT, typeof(bool)))
                {
                    return (bool)OptionKey[SOURCEDOWNLOAD_COMPACT];
                }
                return false;
            }
            set
            {
                OptionKey[SOURCEDOWNLOAD_COMPACT] = (object)value;
            }
        }

        public static bool AlreadyWarnedOfMissingDownloadInfo
        {
            get
            {
                if (OptionKey.HasValue(SOURCEDOWNLOAD_WARNED_OF_MISSING_DOWNLOAD_INFO, typeof(bool)))
                {
                    return (bool)OptionKey[SOURCEDOWNLOAD_WARNED_OF_MISSING_DOWNLOAD_INFO];
                }
                return false;
            }
            set
            {
                OptionKey[SOURCEDOWNLOAD_WARNED_OF_MISSING_DOWNLOAD_INFO] = (object)value;
            }
        }

        public static List<Guid> MultipleDownloadSortedApplications
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_SORTED_APPLICATIONS, typeof(Guid[])))
                {
                    Guid[] array = OptionKey[MULTIPLEDOWNLOAD_SORTED_APPLICATIONS] as Guid[];
                    if (array != null)
                    {
                        return new List<Guid>(array);
                    }
                }
                return null;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_SORTED_APPLICATIONS] = (object)value.ToArray();
            }
        }

        public static List<string> MultipleDownloadSortedExtensions
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_SORTED_EXTENSIONS, typeof(string[])))
                {
                    string[] array = OptionKey[MULTIPLEDOWNLOAD_SORTED_EXTENSIONS] as string[];
                    if (array != null)
                    {
                        return new List<string>(array);
                    }
                }
                return null;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_SORTED_EXTENSIONS] = (object)value.ToArray();
            }
        }

        public static Guid[] MultipleDownloadApplications
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_APPLICATIONS, typeof(Guid[])))
                {
                    return (Guid[])OptionKey[MULTIPLEDOWNLOAD_APPLICATIONS];
                }
                return null;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_APPLICATIONS] = (object)value;
            }
        }

        public static int MultipleDownloadOnlineChangeOption
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_ONLINECHANGEOPTION, typeof(int)))
                {
                    return (int)OptionKey[MULTIPLEDOWNLOAD_ONLINECHANGEOPTION];
                }
                return 0;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_ONLINECHANGEOPTION] = (object)value;
            }
        }

        public static bool MultipleDownloadDeleteOldApps
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_DELETEOLDAPPS, typeof(bool)))
                {
                    return (bool)OptionKey[MULTIPLEDOWNLOAD_DELETEOLDAPPS];
                }
                return true;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_DELETEOLDAPPS] = (object)value;
            }
        }

        public static bool MultipleDownloadStartAllApps
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_STARTALLAPPS, typeof(bool)))
                {
                    return (bool)OptionKey[MULTIPLEDOWNLOAD_STARTALLAPPS];
                }
                return true;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_STARTALLAPPS] = (object)value;
            }
        }

        public static bool MultipleDownloadDoNotReleaseForcedVariables
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_DONOTRELEASEFORCEDVARIABLES, typeof(bool)))
                {
                    return (bool)OptionKey[MULTIPLEDOWNLOAD_DONOTRELEASEFORCEDVARIABLES];
                }
                return true;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_DONOTRELEASEFORCEDVARIABLES] = (object)value;
            }
        }

        public static bool MultipleDownloadInitPersistentVars
        {
            get
            {
                if (OptionKey.HasValue(MULTIPLEDOWNLOAD_INITPERSISTENTVARS, typeof(bool)))
                {
                    return (bool)OptionKey[MULTIPLEDOWNLOAD_INITPERSISTENTVARS];
                }
                return true;
            }
            set
            {
                OptionKey[MULTIPLEDOWNLOAD_INITPERSISTENTVARS] = (object)value;
            }
        }

        public static bool ImplicitlyAtBootAppAndDownload => SourceDownloadTiming2 == 5;

        public static bool ImplicitlyAtDownload => SourceDownloadTiming2 == 1;

        public static bool PromptAtDownload => SourceDownloadTiming2 == 2;

        public static bool ImplicitlyAtCreatingBootproject => SourceDownloadTiming2 == 3;

        public static bool OnlyOnDemand => SourceDownloadTiming2 == 4;

        public static Guid GetDestinationDevice => SourceDownloadDevice;

        private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)0).CreateSubKey(SUB_KEY);

        internal static void SaveOptions()
        {
            IMetaObject val = null;
            try
            {
                int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(handle, GUID_WORKSPACEOBJECT);
                ChunkedMemoryStream val2 = new ChunkedMemoryStream();
                APEnvironment.OptionStorage.Save((OptionRoot)0, (Stream)(object)val2);
                ((Stream)(object)val2).Close();
                IObject @object = val.Object;
                IObject obj = ((@object is IWorkspaceObject) ? @object : null);
                Debug.Assert(obj != null);
                ((IWorkspaceObject)obj).OptionData = (val2.ToArray());
                ((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
            }
            catch
            {
                if (val != null && val.IsToModify)
                {
                    ((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
                }
            }
        }

        internal static bool IsCompileInformationIncludedInSource(bool bWarnUserAndOptionallyChangeSourceContent)
        {
            //IL_0077: Unknown result type (might be due to invalid IL or missing references)
            //IL_007c: Invalid comparison between I4 and Unknown
            List<Guid> list = new List<Guid>(SourceDownloadContent2);
            bool flag = list.Contains(GUID_COMPILE_INFO_ARCHIVE);
            bool flag2 = false;
            if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("OnlineCommands", "DisableMissingCompileInfoWarning"))
            {
                flag2 = ((IEngine3)APEnvironment.Engine).OEMCustomization.GetBoolValue("OnlineCommands", "DisableMissingCompileInfoWarning");
            }
            if (!flag2 && bWarnUserAndOptionallyChangeSourceContent && !flag && !AlreadyWarnedOfMissingDownloadInfo)
            {
                if (2 == (int)APEnvironment.MessageService.Prompt(Strings.WarnUserOfMissingCompileInfo, (PromptChoice)2, (PromptResult)2, "WarnUserOfMissingCompileInfo", Array.Empty<object>()))
                {
                    list.Add(GUID_COMPILE_INFO_ARCHIVE);
                    SourceDownloadContent2 = list.ToArray();
                }
                AlreadyWarnedOfMissingDownloadInfo = true;
                SaveOptions();
            }
            return flag;
        }
    }
}
