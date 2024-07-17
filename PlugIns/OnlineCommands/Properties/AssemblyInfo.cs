using _3S.CoDeSys.Core.Components;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Online Commands")]
[assembly: AssemblyDescription("Implements the CoDeSys online commands")]
[assembly: AssemblyCompany("CODESYS Development GmbH")]
[assembly: AssemblyProduct("CODESYS")]
[assembly: AssemblyCopyright("Copyright (c) 2017-2021 CODESYS Development GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.")]
[assembly: PlugInGuid("{56719006-DA81-45dd-8EDB-C0DB99178F38}")]
[assembly: PlugInKey("1HlvrTUDSdUBTebpGRhQLc7yiEdjhKJT/mUV2k8yKcCTd+2lIyzxvL+pIpF9ibJc", "APFull;LoggedOnPassword")]
[assembly: SuppressIldasm]
[assembly: AssemblyVersion("3.5.16.50")]
[module: SuppressMessage("Automation Platform", "AP0016:SpecifyMessageKey", Scope = "member", Target = "_3S.CoDeSys.OnlineCommands.MultipleDownloadCommand+MyMessageService.#ErrorWithKey(System.String,System.String)", Justification = "Compatibility code for old message service must not be changed.")]
[module: SuppressMessage("Automation Platform", "AP0016:SpecifyMessageKey", Scope = "member", Target = "_3S.CoDeSys.OnlineCommands.MultipleDownloadCommand+MyMessageService.#Warning(System.String)", Justification = "Compatibility code for old message service must not be changed.")]
[module: SuppressMessage("Automation Platform", "AP0001:AssociatedOnlineHelpTopicSpecification", Scope = "type", Target = "_3S.CoDeSys.OnlineCommands.FileTransferDownload", Justification = "No F1 help available")]
[module: SuppressMessage("Automation Platform", "AP0001:AssociatedOnlineHelpTopicSpecification", Scope = "type", Target = "_3S.CoDeSys.OnlineCommands.FileTransferUpload", Justification = "No F1 help available")]
[module: SuppressMessage("Automation Platform", "AP0001:AssociatedOnlineHelpTopicSpecification", Scope = "type", Target = "_3S.CoDeSys.OnlineCommands.LogoffUserCommand", Justification = "No F1 help available")]
[module: SuppressMessage("Automation Platform", "AP0001:AssociatedOnlineHelpTopicSpecification", Scope = "type", Target = "_3S.CoDeSys.OnlineCommands.ShowCompileChangeDetailsCommand", Justification = "No F1 help available, opens dialog on doubleklick in status field. Help is to be shown for opened dialog.")]
