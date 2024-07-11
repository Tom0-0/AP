using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

[assembly: AssemblyTitle("Watchlist View")]
[assembly: AssemblyDescription("Contains a view for monitoring expressions.")]
[assembly: AssemblyCompany("CODESYS Development GmbH")]
[assembly: AssemblyProduct("CODESYS")]
[assembly: AssemblyCopyright("Copyright (c) 2017-2020 CODESYS Development GmbH, Copyright (c) 1994-2016 3S-Smart Software Solutions GmbH. All rights reserved.")]
[assembly: PlugInGuid("{7F2108AF-1D6D-46c6-8106-7BD1190BB96A}")]
[assembly: PlugInKey("uaqk4NAfdM5SClfzLDYKnyQPqt+t8W7pK0bJdBr5K6lPNH7J72IVKCDY49a/rqEN", "APFull")]
[assembly: AssociatedOnlineHelpDefaultTopic("codesys.chm::/_cds_using_watchlists.htm")]
[assembly: SuppressIldasm]
[assembly: AssemblyVersion("3.5.16.40")]
[module: SuppressMessage("Automation Platform", "AP0016:SpecifyMessageKey", Scope = "member", Target = "_3S.CoDeSys.WatchList.WatchListModel.#CreateChildNodes(System.String,_3S.CoDeSys.WatchList.WatchListNode)", Justification = "Compatibility code for old versions")]
[module: SuppressMessage("Automation Platform", "AP0016:SpecifyMessageKey", Scope = "member", Target = "_3S.CoDeSys.WatchList.WatchListModel.#IsMaxMonitoringLengthExceeded(System.Int32,System.Int32&)", Justification = "Compatibility code for old versions")]
[module: SuppressMessage("Automation Platform", "AP0016:SpecifyMessageKey", Scope = "member", Target = "_3S.CoDeSys.WatchList.WatchListNode.#IsEditable(System.Int32)", Justification = "Compatibility code for old versions")]
