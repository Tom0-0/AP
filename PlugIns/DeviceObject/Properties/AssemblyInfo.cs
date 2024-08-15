using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

[assembly: AssemblyTitle("Device Object")]
[assembly: AssemblyDescription("The Device object")]
[assembly: AssemblyCompany("CODESYS Development GmbH")]
[assembly: AssemblyProduct("CODESYS")]
[assembly: AssemblyCopyright("Copyright (c) 2017-2021 CODESYS Development GmbH")]
[assembly: PlugInGuid("{2A785A6D-F546-47d1-9107-6BCAA0F232CF}")]
[assembly: PlugInKey("EY8fCYkiUNURUtWT1Xhl4TfYEaJ9AGVGk1FWtgFXnI0pDj2AxCvngSqjW0YdtB/6", "APFull")]
[assembly: SuppressIldasm]
[assembly: AssociatedOnlineHelpDefaultTopic("codesys.chm::/f_device_editors.htm")]
[assembly: AssemblyVersion("3.5.16.60")]
[module: SuppressMessage("Automation Platform", "AP0004:AvoidOverridingCertainGenericObject2Methods", Scope = "member", Target = "_3S.CoDeSys.DeviceObject.CommunicationSettings.#SetSerializableValue(System.String,System.Object,_3S.CoDeSys.Core.Objects.IArchiveReporter)", Justification = "Historic code. StR and JS agree on suppressing this message instead of breaking functionality.")]
[module: SuppressMessage("Automation Platform", "AP0004:AvoidOverridingCertainGenericObject2Methods", Scope = "member", Target = "_3S.CoDeSys.DeviceObject.CommunicationSettings.#GetSerializableValue(System.String)", Justification = "Historic code. StR and JS agree on suppressing this message instead of breaking functionality.")]
[module: SuppressMessage("Automation Platform", "AP0004:AvoidOverridingCertainGenericObject2Methods", Scope = "member", Target = "_3S.CoDeSys.DeviceObject.CommunicationSettings.#SerializableValueNames", Justification = "Historic code. StR and JS agree on suppressing this message instead of breaking functionality.")]
[module: SuppressMessage("Automation Platform", "AP0004:AvoidOverridingCertainGenericObject2Methods", Scope = "member", Target = "_3S.CoDeSys.DeviceObject.CommunicationSettings.#SetSerializableValue(System.String,System.Object)", Justification = "Historic code. StR and JS agree on suppressing this message instead of breaking functionality.")]
