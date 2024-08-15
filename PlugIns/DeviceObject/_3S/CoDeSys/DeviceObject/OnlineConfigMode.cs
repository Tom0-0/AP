using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.OnlineCommands;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{E4E9AA66-6467-45be-A112-9086F990F4BC}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_online_configuration_mode.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/online_config_mode.htm")]
	public class OnlineConfigMode : IToggleCommand2, IToggleCommand, ICommand
	{
		internal static readonly string HIDDENONLINECONFIGAPPLICATION = "HiddenOnlineConfigModeApp";

		public bool RadioCheck => false;

		public bool Checked
		{
			get
			{
				if (!Enabled)
				{
					return false;
				}
				IMetaObjectStub selectedStub = DeviceCommandHelper.GetSelectedStub();
				if (selectedStub != null && selectedStub.ParentObjectGuid == Guid.Empty && DeviceObjectHelper.ConfigModeApplication(selectedStub.ObjectGuid) != Guid.Empty)
				{
					return true;
				}
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject != null)
				{
					IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(primaryProject.Handle, primaryProject.ActiveApplication);
					if (hostStub != null && DeviceObjectHelper.ConfigModeApplication(hostStub.ObjectGuid) != Guid.Empty)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Name");

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Description");

		public string ToolTipText => Name;

		public Icon SmallIcon
		{
			get
			{
				if (!Checked)
				{
					return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectFactory), "_3S.CoDeSys.DeviceObject.Resources.OnlineConfigModeOnSmall.ico");
				}
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceObjectFactory), "_3S.CoDeSys.DeviceObject.Resources.OnlineConfigModeOffSmall.ico");
			}
		}

		public Icon LargeIcon => SmallIcon;

		public bool Enabled
		{
			get
			{
				if (DeviceCommandHelper.GetHostFromSelectedDevice() == null)
				{
					return false;
				}
				return DeviceCommandHelper.IsOffline;
			}
		}

		public string[] BatchCommand => new string[0];

		public string[] CreateBatchArguments(bool bInvokedByContextMenu)
		{
			IMetaObjectStub selectedStub = DeviceCommandHelper.GetSelectedStub();
			if (selectedStub != null)
			{
				IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(selectedStub.ProjectHandle, selectedStub.ObjectGuid);
				return new string[2]
				{
					hostStub.ProjectHandle.ToString(),
					hostStub.ObjectGuid.ToString()
				};
			}
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject != null)
			{
				IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(primaryProject.Handle, primaryProject.ActiveApplication);
				if (hostStub2 != null && DeviceObjectHelper.ConfigModeApplication(hostStub2.ObjectGuid) != Guid.Empty)
				{
					return new string[2]
					{
						hostStub2.ProjectHandle.ToString(),
						hostStub2.ObjectGuid.ToString()
					};
				}
			}
			return new string[0];
		}

		public string[] CreateBatchArguments()
		{
			return CreateBatchArguments(bInvokedByContextMenu: false);
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (bContextMenu)
			{
				bool flag = ((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) != null;
				IMetaObjectStub selectedStub = DeviceCommandHelper.GetSelectedStub();
				return flag && selectedStub != null && selectedStub.ParentObjectGuid == Guid.Empty && DeviceCommandHelper.IsOffline;
			}
			return true;
		}

		private void CollectIoProviders(IIoProvider ioProvider, LDictionary<int, IIoProvider> htIoProviders, ref int nModules)
		{
			htIoProviders.Add(nModules++, ioProvider);
			IIoProvider[] children = ioProvider.Children;
			foreach (IIoProvider ioProvider2 in children)
			{
				CollectIoProviders(ioProvider2, htIoProviders, ref nModules);
			}
		}

		internal static void CollectObjectGuids(LList<Guid> liGuids, int nProjectHandle, Guid[] subGuids, Type objectType, bool bRecursive)
		{
			foreach (Guid guid in subGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (objectType.IsAssignableFrom(metaObjectStub.ObjectType))
				{
					liGuids.Add(guid);
				}
				if (bRecursive)
				{
					CollectObjectGuids(liGuids, nProjectHandle, metaObjectStub.SubObjectGuids, objectType, bRecursive);
				}
			}
		}

		public void ExecuteBatch(string[] arguments)
		{
			DeviceObject deviceObject = null;
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (arguments.Length > 2)
			{
				throw new BatchTooManyArgumentsException(this.BatchCommand, arguments.Length, 2);
			}
			if (arguments.Length < 2)
			{
				throw new BatchTooFewArgumentsException(this.BatchCommand, arguments.Length, 2);
			}
			int nProjectHandle;
			Guid objectGuid;
			if (arguments.Length == 2 && int.TryParse(arguments[0], out nProjectHandle) && Guid.TryParse(arguments[1], out objectGuid))
			{
				deviceObject = (APEnvironment.ObjectMgr.GetObjectToRead(nProjectHandle, objectGuid).Object as DeviceObject);
			}
			if (deviceObject != null)
			{
				IMetaObject plcLogic = deviceObject.GetPlcLogic();
				if (plcLogic != null)
				{
					Guid objectGuid2 = deviceObject.MetaObject.ObjectGuid;
					bool flag = false;
					LList<string> llist = new LList<string>();
					IApplicationObject deviceApplicationObject = deviceObject.GetDeviceApplicationObject(plcLogic);
					try
					{
						IOnlineDevice onlineDevice = APEnvironment.OnlineMgr.GetOnlineDevice(deviceObject.MetaObject.ObjectGuid);
						if (onlineDevice != null)
						{
							bool isConnected = onlineDevice.IsConnected;
							IMetaObject application = deviceObject.GetApplication();
							if (application != null)
							{
								Guid objectGuid3 = application.ObjectGuid;
								if (!isConnected)
								{
									onlineDevice.Connect();
								}
								string[] array = onlineDevice.ReadApplicationList();
								if (array.Length != 0)
								{
									foreach (string text in array)
									{
										if (!(text != OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION))
										{
											flag = false;
											llist.Clear();
											break;
										}
										if (deviceApplicationObject == null || !(deviceApplicationObject is IOnlineApplicationObject2) || !((deviceApplicationObject as IOnlineApplicationObject2).ApplicationName == text))
										{
											llist.Add(text);
											flag = true;
										}
									}
								}
								if (!isConnected)
								{
									onlineDevice.Disconnect();
								}
							}
						}
					}
					catch (Exception ex)
					{
						if (!(ex is InteractiveLoginFailedException) || !((InteractiveLoginFailedException)ex).ShouldBeHandledSilently)
						{
							APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
						}
						return;
					}
					bool flag2 = false;
					if (flag && DeviceObjectHelper.EnableAdditionalParameters(deviceObject))
					{
						ConfigModeForm configModeForm = new ConfigModeForm();
						configModeForm.Applications(llist);
						DialogResult dialogResult = configModeForm.ShowDialog(APEnvironment.FrameForm);
						if (dialogResult == DialogResult.Yes)
						{
							flag2 = true;
						}
						if (dialogResult == DialogResult.Cancel)
						{
							return;
						}
					}
					if (DeviceObjectHelper.ConfigModeApplication(deviceObject.MetaObject.ObjectGuid) == DeviceObjectHelper.ParamModeGuid)
					{
						flag2 = true;
					}
					if (flag2)
					{
						try
						{
							IOnlineDevice onlineDevice2 = APEnvironment.OnlineMgr.GetOnlineDevice(deviceObject.MetaObject.ObjectGuid);
							if (onlineDevice2 != null)
							{
								if (onlineDevice2.IsConnected)
								{
									onlineDevice2.Disconnect();
									DeviceObjectHelper.ConfigModeApplication(deviceObject.MetaObject.ObjectGuid, Guid.Empty);
									return;
								}
								IMetaObject application2 = deviceObject.GetApplication();
								if (application2 != null)
								{
									Guid objectGuid4 = application2.ObjectGuid;
									onlineDevice2.Connect();
									string[] array3 = onlineDevice2.ReadApplicationList();
									onlineDevice2.Disconnect();
									if (array3 != null && array3.Length != 0)
									{
										string[] array2 = array3;
										for (int i = 0; i < array2.Length; i++)
										{
											if (array2[i] == (application2.Object as IOnlineApplicationObject2).ApplicationName)
											{
												Guid a;
												Guid a2;
												APEnvironment.LanguageModelMgr.GetDownloadIds(objectGuid4, out a, out a2);
												if (a == Guid.Empty || a2 == Guid.Empty)
												{
													APEnvironment.LanguageModelMgr.Compile(objectGuid4);
													APEnvironment.LanguageModelMgr.GenerateCode(objectGuid4, false, false);
												}
												if (onlineDevice2 != null && !onlineDevice2.IsConnected)
												{
													LDictionary<int, IIoProvider> ldictionary = new LDictionary<int, IIoProvider>();
													int num = 0;
													IIoProvider ioProvider = deviceObject;
													if (ioProvider != null)
													{
														this.CollectIoProviders(ioProvider, ldictionary, ref num);
													}
													onlineDevice2.Connect();
													LDictionary<IOnlineVarRef2, IIoProvider> ldictionary2 = new LDictionary<IOnlineVarRef2, IIoProvider>();
													foreach (IIoProvider ioProvider2 in ldictionary.Values)
													{
														try
														{
															IMetaObject metaObject = ioProvider2.GetMetaObject();
															if (!(metaObject.Object is SlotDeviceObject) || (metaObject.Object as SlotDeviceObject).HasDevice)
															{
																int nConnectorId = (ioProvider2 is IConnector) ? ((IConnector)ioProvider2).ConnectorId : -1;
																IOnlineVarRef2 onlineVarRef = new DataElementOnlineVarRef(metaObject.ProjectHandle, metaObject.ObjectGuid, nConnectorId, 1879048209L, 0, "dint", false);
																if (onlineVarRef != null)
																{
																	ldictionary2.Add(onlineVarRef, ioProvider2);
																}
															}
														}
														catch
														{
														}
													}
													EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
													for (int j = 0; j < 100; j++)
													{
														Application.DoEvents();
														eventWaitHandle.WaitOne(10, false);
														bool flag3 = true;
														using (LDictionary<IOnlineVarRef2, IIoProvider>.KeyCollection.Enumerator enumerator2 = ldictionary2.Keys.GetEnumerator())
														{
															while (enumerator2.MoveNext())
															{
																if (enumerator2.Current.State != VarRefState.Good)
																{
																	flag3 = false;
																}
															}
														}
														if (flag3)
														{
															break;
														}
													}
													bool flag4 = true;
													foreach (KeyValuePair<IOnlineVarRef2, IIoProvider> keyValuePair in ldictionary2)
													{
														IOnlineVarRef2 key = keyValuePair.Key;
														if (key.State == VarRefState.Good)
														{
															IIoProvider value = keyValuePair.Value;
															if (ioProvider != null)
															{
																IMetaObject metaObject2 = value.GetMetaObject();
																if (metaObject2 != null)
																{
																	DeviceObject deviceObject2 = metaObject2.Object as DeviceObject;
																	if (deviceObject2 != null)
																	{
																		int hashCode = deviceObject2.DeviceIdentificationNoSimulation.GetHashCode();
																		if ((int)key.Value != hashCode)
																		{
																			flag4 = false;
																		}
																	}
																}
															}
														}
														else
														{
															flag4 = false;
														}
														key.Release();
													}
													if (!flag4)
													{
														APEnvironment.MessageService.Error(Strings.ErrorParamMode, "ErrorParamMode", Array.Empty<object>());
														onlineDevice2.Disconnect();
														return;
													}
													DeviceObjectHelper.ConfigModeApplication(deviceObject.MetaObject.ObjectGuid, DeviceObjectHelper.ParamModeGuid);
													return;
												}
											}
										}
									}
								}
							}
						}
						catch (Exception ex2)
						{
							APEnvironment.MessageService.Error(ex2.Message, "Exception", Array.Empty<object>());
							return;
						}
					}
					if (flag && DeviceObjectHelper.ConfigModeApplication(objectGuid2) == Guid.Empty && APEnvironment.MessageService.Prompt(Strings.OnlineConfigModeWarning, PromptChoice.YesNo, PromptResult.No, "OnlineConfigModeWarning", Array.Empty<object>()) == PromptResult.No)
					{
						return;
					}
					Guid guid = Guid.Empty;
					if (deviceApplicationObject != null)
					{
						string applicationName = (deviceApplicationObject as IOnlineApplicationObject2).ApplicationName;
					}
					LDictionary<string, string> ldictionary3 = new LDictionary<string, string>();
					if (deviceApplicationObject != null)
					{
						foreach (Guid guid2 in deviceApplicationObject.MetaObject.SubObjectGuids)
						{
							IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(deviceApplicationObject.MetaObject.ProjectHandle, guid2);
							if (objectToRead.Object is IApplicationObject && objectToRead.Name == OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION)
							{
								guid = guid2;
								break;
							}
						}
					}
					else
					{
						foreach (Guid guid3 in plcLogic.SubObjectGuids)
						{
							IMetaObject objectToRead2 = APEnvironment.ObjectMgr.GetObjectToRead(plcLogic.ProjectHandle, guid3);
							if (objectToRead2.Object is IApplicationObject && objectToRead2.Name == OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION)
							{
								guid = guid3;
								break;
							}
						}
					}
					if (guid == Guid.Empty)
					{
						IUndoManager undoManager = APEnvironment.ObjectMgr.GetUndoManager(deviceObject.MetaObject.ProjectHandle);
						try
						{
							if (undoManager != null)
							{
								undoManager.BeginCompoundAction("OnlineMode");
							}
							Guid ioApplication = (deviceObject.DriverInfo as IDriverInfo2).IoApplication;
							string hiddenonlineconfigapplication = OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION;
							IMetaObject metaObject3 = null;
							try
							{
								APEnvironment.OnlineUIMgr.BeginAllowOnlineModification();
								if (deviceApplicationObject != null)
								{
									metaObject3 = LogicalIOHelper.CreateApplikation(deviceApplicationObject.MetaObject.ProjectHandle, deviceApplicationObject.MetaObject.ObjectGuid, hiddenonlineconfigapplication, true, Guid.Empty);
								}
								else
								{
									metaObject3 = LogicalIOHelper.CreateApplikation(plcLogic.ProjectHandle, plcLogic.ObjectGuid, hiddenonlineconfigapplication, true, Guid.Empty);
								}
							}
							finally
							{
								APEnvironment.OnlineUIMgr.EndAllowOnlineModification();
							}
							if (metaObject3 != null)
							{
								if (ioApplication != Guid.Empty && APEnvironment.ObjectMgr.ExistsObject(plcLogic.ProjectHandle, ioApplication))
								{
									IMetaObject objectToRead3 = APEnvironment.ObjectMgr.GetObjectToRead(plcLogic.ProjectHandle, ioApplication);
									if (objectToRead3.Object is IApplicationObject6)
									{
										IApplicationObject6 applicationObject = objectToRead3.Object as IApplicationObject6;
										if (applicationObject.OverrideTargetMemorySizes)
										{
											(metaObject3.Object as IApplicationObject6).OverrideTargetMemorySizes = applicationObject.OverrideTargetMemorySizes;
											(metaObject3.Object as IApplicationObject6).TargetInputSize = applicationObject.TargetInputSize;
											(metaObject3.Object as IApplicationObject6).TargetOutputSize = applicationObject.TargetOutputSize;
											(metaObject3.Object as IApplicationObject6).TargetMemorySize = applicationObject.TargetMemorySize;
										}
									}
								}
								if (metaObject3.Object is IOnlineApplicationObject5)
								{
									(metaObject3.Object as IOnlineApplicationObject5).BootApplicationSettings.CreateBootApplicationOnDownload = false;
								}
								DeviceObjectHelper.ConfigModeApplication(objectGuid2, metaObject3.ObjectGuid);
								IMetaObject metaObject4 = LogicalIOHelper.CreateTaskConfigObject(metaObject3);
								if (metaObject4 != null)
								{
									bool flag5 = false;
									if (ioApplication != Guid.Empty && APEnvironment.ObjectMgr.ExistsObject(plcLogic.ProjectHandle, ioApplication))
									{
										IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(plcLogic.ProjectHandle, ioApplication);
										LList<Guid> llist2 = new LList<Guid>();
										if (deviceApplicationObject != null)
										{
											OnlineConfigMode.CollectObjectGuids(llist2, metaObjectStub.ProjectHandle, deviceApplicationObject.MetaObject.SubObjectGuids, typeof(ITaskObject), true);
										}
										else
										{
											foreach (Guid objectGuid5 in metaObjectStub.SubObjectGuids)
											{
												IMetaObjectStub metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(plcLogic.ProjectHandle, objectGuid5);
												if (typeof(ITaskConfigObject).IsAssignableFrom(metaObjectStub2.ObjectType))
												{
													OnlineConfigMode.CollectObjectGuids(llist2, metaObjectStub.ProjectHandle, metaObjectStub2.SubObjectGuids, typeof(ITaskObject), false);
												}
											}
										}
										foreach (Guid objectGuid6 in metaObjectStub.SubObjectGuids)
										{
											IMetaObjectStub metaObjectStub3 = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObjectStub.ProjectHandle, objectGuid6);
											if (typeof(ILibManObject5).IsAssignableFrom(metaObjectStub3.ObjectType))
											{
												ILibManObject5 libManObject = APEnvironment.ObjectMgr.GetObjectToRead(metaObjectStub3.ProjectHandle, objectGuid6).Object as ILibManObject5;
												string[] redirectedPlaceholderNames = libManObject.PlaceholderRedirection.GetRedirectedPlaceholderNames();
												if (redirectedPlaceholderNames != null)
												{
													foreach (string text2 in redirectedPlaceholderNames)
													{
														string redirectedPlaceholderResolution = libManObject.PlaceholderRedirection.GetRedirectedPlaceholderResolution(text2);
														ldictionary3.Add(text2, redirectedPlaceholderResolution);
													}
												}
											}
										}
										if (llist2.Count > 0)
										{
											flag5 = true;
											int num2 = 0;
											foreach (Guid objectGuid7 in llist2)
											{
												IMetaObject objectToRead4 = APEnvironment.ObjectMgr.GetObjectToRead(metaObjectStub.ProjectHandle, objectGuid7);
												if (objectToRead4.Object is ITaskObject)
												{
													bool flag6 = false;
													foreach (Guid objectGuid8 in metaObject4.SubObjectGuids)
													{
														if (APEnvironment.ObjectMgr.GetMetaObjectStub(metaObjectStub.ProjectHandle, objectGuid8).Name == objectToRead4.Name)
														{
															flag6 = true;
															break;
														}
													}
													if (!flag6)
													{
														try
														{
															ITaskObject taskObject = objectToRead4.Object.Clone() as ITaskObject;
															taskObject.POUs.Clear();
															taskObject.POUs.Add(taskObject.CreatePOU("ConfigModePou"));
															string stName = "ClonedTask" + num2.ToString() + "_" + objectToRead4.Name;
															num2++;
															APEnvironment.ObjectMgr.AddObject(metaObjectStub.ProjectHandle, metaObject4.ObjectGuid, Guid.NewGuid(), taskObject, stName, -1);
														}
														catch
														{
														}
													}
												}
											}
										}
									}
									if (!flag5)
									{
										ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceObject.DeviceIdentificationNoSimulation);
										int intValue = LocalTargetSettings.DefaultTaskPriority.GetIntValue(targetSettingsById);
										string stringValue = LocalTargetSettings.CycleTimeDefault.GetStringValue(targetSettingsById);
										LogicalIOHelper.CreateTaskObject(plcLogic.ProjectHandle, metaObject4.ObjectGuid, "ConfigMode", "ConfigModePou", false, string.Empty, stringValue, intValue.ToString(), false, string.Empty, string.Empty, string.Empty);
									}
									LogicalIOHelper.CreateEmptyTaskPou(metaObject3, "ConfigModePou");
									guid = metaObject3.ObjectGuid;
								}
							}
						}
						catch
						{
						}
						finally
						{
							if (undoManager != null)
							{
								undoManager.EndCompoundAction();
							}
						}
					}
					bool flag7 = false;
					if (guid != Guid.Empty)
					{
						IOnlineApplication10 onlineApplication = APEnvironment.OnlineMgr.GetApplication(guid) as IOnlineApplication10;
						if (onlineApplication != null)
						{
							if (!onlineApplication.IsLoggedIn)
							{
								APEnvironment.DeviceController.RemoveApplication(guid);
								this.UpdateLanguageModel(plcLogic.ProjectHandle, new Guid[]
								{
									objectGuid2
								});
								foreach (Guid objectGuid9 in APEnvironment.ObjectMgr.GetMetaObjectStub(plcLogic.ProjectHandle, guid).SubObjectGuids)
								{
									IMetaObjectStub metaObjectStub4 = APEnvironment.ObjectMgr.GetMetaObjectStub(plcLogic.ProjectHandle, objectGuid9);
									if (typeof(ILibManObject5).IsAssignableFrom(metaObjectStub4.ObjectType))
									{
										IMetaObject metaObject5 = null;
										try
										{
											metaObject5 = APEnvironment.ObjectMgr.GetObjectToModify(metaObjectStub4.ProjectHandle, objectGuid9);
											if (metaObject5 != null)
											{
												ILibManObject5 libManObject2 = metaObject5.Object as ILibManObject5;
												foreach (KeyValuePair<string, string> keyValuePair2 in ldictionary3)
												{
													libManObject2.PlaceholderRedirection.SetRedirectedPlaceholderResolution(keyValuePair2.Key, keyValuePair2.Value);
												}
											}
										}
										catch
										{
										}
										finally
										{
											if (metaObject5 != null && metaObject5.IsToModify)
											{
												APEnvironment.ObjectMgr.SetObject(metaObject5, true, null);
											}
										}
									}
								}
								ILoginService2 loginService = APEnvironment.CreateLoginServiceWrapper();
								List<Guid> guidApplications = new List<Guid>(1)
								{
									guid
								};
								try
								{
									if (!onlineApplication.Login(true))
									{
										if (onlineApplication.OnlineDevice != null)
										{
											foreach (string text3 in onlineApplication.OnlineDevice.ReadApplicationList())
											{
												if (deviceApplicationObject == null || !(deviceApplicationObject is IOnlineApplicationObject2) || !((deviceApplicationObject as IOnlineApplicationObject2).ApplicationName == text3))
												{
													onlineApplication.OnlineDevice.DeleteApplication(text3);
												}
											}
										}
										onlineApplication.Logout();
									}
									loginService.ConfigureLogin(this, guidApplications, LoginServiceFlags.ForceDownload | LoginServiceFlags.RemoveOrphanedApps | LoginServiceFlags.SuppressAllDialogs, null, null);
									if (loginService.BeginLogin(this))
									{
										APEnvironment.OnlineUIMgr.Login(guid);
										if (onlineApplication.IsLoggedIn)
										{
											onlineApplication.Start();
										}
										flag7 = true;
									}
									goto IL_EE4;
								}
								catch (Exception ex3)
								{
									DeviceObjectHelper.ConfigModeApplication(objectGuid2, Guid.Empty);
									if (onlineApplication.OnlineDevice != null)
									{
										onlineApplication.OnlineDevice.DeleteApplication(OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION);
										onlineApplication.Logout();
									}
									APEnvironment.MessageService.Error(ex3.Message, "Exception", Array.Empty<object>());
									goto IL_EE4;
								}
								finally
								{
									if (loginService != null)
									{
										try
										{
											loginService.EndLogin(this);
										}
										catch
										{
										}
									}
								}
							}
							if (deviceApplicationObject != null)
							{
								IOnlineApplication10 onlineApplication2 = APEnvironment.OnlineMgr.GetApplication(deviceApplicationObject.MetaObject.ObjectGuid) as IOnlineApplication10;
								if (onlineApplication2 != null)
								{
									try
									{
										onlineApplication2.Reset(ResetOption.Cold);
										onlineApplication2.Logout();
									}
									catch
									{
									}
								}
							}
							try
							{
								onlineApplication.Stop();
								onlineApplication.Reset(ResetOption.Original);
							}
							catch
							{
							}
							APEnvironment.LanguageModelMgr.ClearDownloadContext(guid);
							DeviceObjectHelper.ConfigModeApplication(objectGuid2, Guid.Empty);
							onlineApplication.Logout();
						}
					IL_EE4:
						if (!flag7)
						{
							DeviceObjectHelper.ConfigModeApplication(objectGuid2, Guid.Empty);
							this.UpdateLanguageModel(plcLogic.ProjectHandle, new Guid[]
							{
								objectGuid2
							});
							APEnvironment.LanguageModelMgr.ClearDownloadContext((deviceObject.DriverInfo as IDriverInfo2).IoApplication);
							APEnvironment.DeviceController.RemoveApplication(guid);
							try
							{
								if (onlineApplication != null)
								{
									if (onlineApplication.OnlineDevice == null)
									{
										onlineApplication.Login(true);
									}
									if (onlineApplication.OnlineDevice != null)
									{
										onlineApplication.OnlineDevice.DeleteApplication(OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION);
										onlineApplication.Logout();
									}
								}
								if (guid != Guid.Empty && APEnvironment.ObjectMgr.ExistsObject(plcLogic.ProjectHandle, guid))
								{
									APEnvironment.ObjectMgr.RemoveObject(plcLogic.ProjectHandle, guid);
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}

		internal void UpdateLanguageModel(int nProjectHandle, Guid[] objectGuids)
		{
			foreach (Guid guid in objectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
				if (metaObjectStub != null)
				{
					try
					{
						DeviceObjectHelper.SuppressUpdateObjects(bUpdateObjectSuppressed: true);
						((IEngine2)APEnvironment.Engine).UpdateLanguageModel(nProjectHandle, guid);
					}
					finally
					{
						DeviceObjectHelper.SuppressUpdateObjects(bUpdateObjectSuppressed: false);
					}
				}
				if (metaObjectStub != null)
				{
					UpdateLanguageModel(nProjectHandle, metaObjectStub.SubObjectGuids);
				}
			}
		}
	}
}
