#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;
using _3S.CoDeSys.VersionCompatibilityManager;

namespace _3S.CoDeSys.DeviceObject
{
	public class DeviceVersionSelectionControl : UserControl, IVersionSelectionControl2, IVersionSelectionControl
	{
		private LList<UpdateInformation> _updateInformationList = new LList<UpdateInformation>();

		private IUndoManager _undoMgr;

		private LList<IObject> _ldevs = new LList<IObject>();

		internal const long CONFIGVERSIONPARAMETER = 1879052288L;

		private IContainer components;

		private TreeTableView _treeTableView;

		private Label label1;

		private Label _noteLabel;

		public Guid Provider => VersionSelectionControlProvider.GUID;

		public DeviceVersionSelectionControl()
		{
			InitializeComponent();
			_undoMgr = APEnvironment.CreateUndoMgr();
			Debug.Assert(_undoMgr != null);
		}

		public bool NewVersionAvailable(bool bAutomaticCheck)
		{
			_updateInformationList = GetDevices();
			if (_updateInformationList != null && _updateInformationList.Count > 0)
			{
				return true;
			}
			return false;
		}

		public void Initialize()
		{
			if (_updateInformationList == null || _updateInformationList.Count == 0)
			{
				_updateInformationList = GetDevices();
			}
			if (_updateInformationList != null)
			{
				_treeTableView.Model=((ITreeTableModel)(object)new PromptUpdateModel(_updateInformationList));
				for (int i = 0; i < _treeTableView.Columns.Count; i++)
				{
					_treeTableView.AdjustColumnWidth(i, true);
				}
				_noteLabel.Text = string.Format(Strings.VersionSelectionControlDescription, Strings.UpdateAction);
			}
			else
			{
				_noteLabel.Text = Strings.VersionSelection_NoDevicesFound;
			}
		}

		public void Save()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Expected O, but got Unknown
			if (_updateInformationList == null || _updateInformationList.Count <= 0)
			{
				return;
			}
			IUndoManager val = null;
			foreach (TreeTableViewNode node in _treeTableView.Nodes)
			{
				node.EndEdit(4, false);
			}
			IDeviceCollection allDevices = ((IDeviceRepository)APEnvironment.DeviceRepository).GetAllDevices();
			try
			{
				if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
				{
					int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
					val = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(handle);
					if (val != null)
					{
						val.BeginCompoundAction("Update environment");
					}
				}
				foreach (UpdateInformation updateInformation in _updateInformationList)
				{
					if (updateInformation.SelectedUpdateVersion == null)
					{
						continue;
					}
					if (updateInformation.Factory != null)
					{
						foreach (IObject ldev in _ldevs)
						{
							IDeviceIdentification val2 = null;
							if (ldev is IDeviceObject5)
							{
								val2 = ((IDeviceObject5)((ldev is IDeviceObject5) ? ldev : null)).DeviceIdentificationNoSimulation;
							}
							if (ldev is IExplicitConnector)
							{
								IDeviceObject deviceObject = ((IConnector)((ldev is IExplicitConnector) ? ldev : null)).GetDeviceObject();
								val2 = ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation;
							}
							if (val2 != null && val2.Type == updateInformation.DevId.Type && val2.Id == updateInformation.DevId.Id)
							{
								updateInformation.Factory.UpdateConfigVersion(ldev.MetaObject, updateInformation.CurrentVersion.DeviceVersion, updateInformation.SelectedUpdateVersion.DeviceVersion);
							}
						}
						continue;
					}
					foreach (IObject ldev2 in _ldevs)
					{
						if (!(ldev2 is IDeviceObject5))
						{
							continue;
						}
						new LList<Version>();
						IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((ldev2 is IDeviceObject5) ? ldev2 : null)).DeviceIdentificationNoSimulation;
						if (deviceIdentificationNoSimulation.Type != updateInformation.DevId.Type || !(deviceIdentificationNoSimulation.Id == updateInformation.DevId.Id) || deviceIdentificationNoSimulation is IModuleIdentification)
						{
							continue;
						}
						string input = GetVersion(deviceIdentificationNoSimulation.Version).ToString();
						DeviceIdentification deviceIdentification = new DeviceIdentification(updateInformation.DevId.Type, updateInformation.DevId.Id, updateInformation.SelectedUpdateVersion.DeviceVersion.ToString(), string.Empty);
						bool flag = false;
						foreach (IDeviceDescription item in (IEnumerable)allDevices)
						{
							IDeviceDescription val3 = item;
							if (!((object)val3.DeviceIdentification).Equals((object)deviceIdentification) || !(val3 is IDeviceDescription5))
							{
								continue;
							}
							foreach (string compatibleVersion in ((IDeviceDescription5)((val3 is IDeviceDescription5) ? val3 : null)).CompatibleVersions)
							{
								if (new Regex(compatibleVersion).Match(input).Success)
								{
									flag = true;
									deviceIdentification.Version = val3.DeviceIdentification.Version;
									break;
								}
							}
						}
						if (flag)
						{
							try
							{
								new ReplaceDeviceAction(ldev2.MetaObject.ProjectHandle, ldev2.MetaObject.ObjectGuid, (IDeviceIdentification)(object)deviceIdentification, null).Redo();
							}
							catch
							{
							}
						}
					}
				}
			}
			finally
			{
				if (val != null)
				{
					val.EndCompoundAction();
				}
			}
		}

		public void Cancel()
		{
		}

		public Dictionary<string, string> SetAllItemsToNewestVersion()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (_treeTableView.Model != null)
			{
				for (int i = 0; i < _treeTableView.Model.Sentinel.ChildCount; i++)
				{
					PromptUpdateNode promptUpdateNode = _treeTableView.Model.Sentinel.GetChild(i) as PromptUpdateNode;
					if (promptUpdateNode == null)
					{
						continue;
					}
					Version version = (Version)promptUpdateNode.GetValue(1);
					Version version2 = (Version)promptUpdateNode.GetValue(2);
					if (version.ToString() != version2.ToString() && promptUpdateNode.SetNewestVersion())
					{
						string key = string.Concat(promptUpdateNode.GetValue(0), ": ", version.ToString());
						if (!dictionary.ContainsKey(key))
						{
							dictionary.Add(key, version2.ToString());
						}
					}
				}
			}
			if (dictionary.Count > 0)
			{
				return dictionary;
			}
			return null;
		}

		private static Version GetVersion(string stVersion)
		{
			if (Version.TryParse(stVersion, out var result))
			{
				return result;
			}
			return null;
		}

		private static Version ReadConfigVersion(IConnector con)
		{
			Version result = null;
			if (((con != null) ? con.HostParameterSet : null) != null && con.HostParameterSet.Contains(1879052288L))
			{
				IParameter parameter = con.HostParameterSet.GetParameter(1879052288L);
				if (!string.IsNullOrEmpty(((IDataElement)parameter).Value))
				{
					IConverterFromIEC converterFromIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
					try
					{
						object value = default(object);
						TypeClass val = default(TypeClass);
						converterFromIEC.GetInteger(((IDataElement)parameter).Value, out value, out val);
						byte[] bytes = BitConverter.GetBytes(Convert.ToUInt32(value));
						if (bytes.Length == 4)
						{
							result = new Version(bytes[3], bytes[2], bytes[1], bytes[0]);
							return result;
						}
						return result;
					}
					catch
					{
						return result;
					}
				}
			}
			return result;
		}

		private LList<UpdateInformation> GetDevices()
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Expected O, but got Unknown
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Expected O, but got Unknown
			LList<UpdateInformation> val = new LList<UpdateInformation>();
			_ldevs = GetDevicesObjectList();
			if (_ldevs.Count == 0)
			{
				return null;
			}
			IDeviceCollection allDevices = ((IDeviceRepository)APEnvironment.DeviceRepository).GetAllDevices();
			foreach (IObject ldev in _ldevs)
			{
				LList<IConnector> val2 = new LList<IConnector>();
				IDeviceInfo devInfo = null;
				IDeviceIdentification val3 = null;
				if (ldev is IExplicitConnector)
				{
					try
					{
						IDeviceObject deviceObject = ((IConnector)((ldev is IExplicitConnector) ? ldev : null)).GetDeviceObject();
						devInfo = deviceObject.DeviceInfo;
						val3 = ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation;
						val2.Add((IConnector)(object)((ldev is IExplicitConnector) ? ldev : null));
					}
					catch
					{
					}
				}
				if (ldev is IDeviceObject5)
				{
					val3 = ((IDeviceObject5)((ldev is IDeviceObject5) ? ldev : null)).DeviceIdentificationNoSimulation;
					devInfo = ((IDeviceObject)((ldev is IDeviceObject) ? ldev : null)).DeviceInfo;
					foreach (IConnector item in (IEnumerable)((IDeviceObject)((ldev is IDeviceObject) ? ldev : null)).Connectors)
					{
						IConnector val4 = item;
						val2.Add(val4);
					}
				}
				Version version = null;
				foreach (IConnector item2 in val2)
				{
					version = ReadConfigVersion(item2);
					if (version != null)
					{
						break;
					}
				}
				if (version != null)
				{
					LList<DeviceVersionInformation> val5 = new LList<DeviceVersionInformation>();
					DeviceVersionInformation deviceVersionInformation = new DeviceVersionInformation();
					IConfigVersionUpdateEnvironmentFactory val6 = null;
					deviceVersionInformation.DeviceVersion = version;
					try
					{
						if (APEnvironment.CreateCheckUpdateVersionFactories() != null)
						{
							uint num = 0u;
							foreach (IConfigVersionUpdateEnvironmentFactory item3 in APEnvironment.CreateCheckUpdateVersionFactories())
							{
								Version version2 = null;
								uint num2 = item3.CheckUpdateConfigVersion(ldev.MetaObject, deviceVersionInformation.DeviceVersion, out version2);
								if (version2 != null && num2 > num)
								{
									DeviceVersionInformation deviceVersionInformation2 = new DeviceVersionInformation
									{
										DeviceVersion = version2,
										NewConfigurationFormat = true
									};
									val6 = item3;
									val5.Clear();
									val5.Add(deviceVersionInformation2);
								}
							}
						}
					}
					catch
					{
					}
					if (deviceVersionInformation != null && val5.Count > 0 && val6 != null)
					{
						bool flag = false;
						UpdateInformation updateInformation = new UpdateInformation(val3, devInfo, deviceVersionInformation, val5.ToArray(), val6);
						foreach (UpdateInformation item4 in val)
						{
							if (item4.DevId.Type == updateInformation.DevId.Type && item4.DevId.Id == updateInformation.DevId.Id)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							val.Add(updateInformation);
						}
					}
				}
				if (!(ldev is IDeviceObject5))
				{
					continue;
				}
				LList<DeviceVersionInformation> val7 = new LList<DeviceVersionInformation>();
				if (val3 == null || val3 is IModuleIdentification)
				{
					continue;
				}
				DeviceVersionInformation deviceVersionInformation3 = new DeviceVersionInformation
				{
					DeviceVersion = GetVersion(val3.Version)
				};
				if (deviceVersionInformation3.DeviceVersion == null)
				{
					continue;
				}
				foreach (IDeviceDescription item5 in (IEnumerable)allDevices)
				{
					IDeviceDescription val8 = item5;
					if (!(val8 is IDeviceDescription5))
					{
						continue;
					}
					bool flag2 = false;
					foreach (string compatibleVersion in ((IDeviceDescription5)((val8 is IDeviceDescription5) ? val8 : null)).CompatibleVersions)
					{
						string pattern = compatibleVersion;
						try
						{
							if (compatibleVersion.Trim().StartsWith("*"))
							{
								pattern = deviceVersionInformation3.DeviceVersion.Major + compatibleVersion.Substring(1);
							}
							if (new Regex(pattern).Match(deviceVersionInformation3.DeviceVersion.ToString()).Success)
							{
								flag2 = true;
								goto IL_03af;
							}
						}
						catch
						{
						}
					}
					goto IL_03af;
					IL_03af:
					if (!flag2 || val8.DeviceIdentification.Type != val3.Type || !(val8.DeviceIdentification.Id == val3.Id))
					{
						continue;
					}
					try
					{
						Version version3 = GetVersion(val8.DeviceIdentification.Version);
						if (!(version3 != null) || !(deviceVersionInformation3.DeviceVersion != null) || !(version3 > deviceVersionInformation3.DeviceVersion))
						{
							continue;
						}
						bool newConfigurationFormat = false;
						foreach (IConnector item6 in (IEnumerable)val8.Connectors)
						{
							Version version4 = ReadConfigVersion(item6);
							if ((version == null && version4 != null) || (version != null && version4 != null && version4 > version))
							{
								newConfigurationFormat = true;
								break;
							}
						}
						val7.Add(new DeviceVersionInformation
						{
							DeviceVersion = version3,
							NewConfigurationFormat = newConfigurationFormat
						});
					}
					catch
					{
					}
				}
				val7.Sort();
				if (val7.Count <= 0)
				{
					continue;
				}
				bool flag3 = false;
				UpdateInformation updateInformation2 = new UpdateInformation(val3, ((IDeviceObject)((ldev is IDeviceObject5) ? ldev : null)).DeviceInfo, deviceVersionInformation3, val7.ToArray(), null);
				foreach (UpdateInformation item7 in val)
				{
					if (item7.DevId.Type == updateInformation2.DevId.Type && item7.DevId.Id == updateInformation2.DevId.Id)
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					val.Add(updateInformation2);
				}
			}
			return val;
		}

		private LList<IObject> GetDevicesObjectList()
		{
			LList<IObject> val = new LList<IObject>();
			int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
			Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
			foreach (Guid guid in allObjects)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					try
					{
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guid);
						IObject @object = objectToRead.Object;
						val.Add((@object is IDeviceObject) ? @object : null);
					}
					catch
					{
					}
				}
				if (typeof(ISlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					try
					{
						IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guid).Object;
						IDeviceObject deviceObject = ((ISlotDeviceObject)((object2 is ISlotDeviceObject) ? object2 : null)).GetDeviceObject();
						if (deviceObject != null)
						{
							val.Add((IObject)(object)deviceObject);
						}
					}
					catch
					{
					}
				}
				if (!typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					continue;
				}
				try
				{
					IObject object3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guid).Object;
					IExplicitConnector val2 = (IExplicitConnector)(object)((object3 is IExplicitConnector) ? object3 : null);
					if (val2 != null)
					{
						val.Add((IObject)(object)val2);
					}
				}
				catch
				{
				}
			}
			return val;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.DeviceVersionSelectionControl));
			_treeTableView = new TreeTableView();
			label1 = new System.Windows.Forms.Label();
			_noteLabel = new System.Windows.Forms.Label();
			SuspendLayout();
			_treeTableView.AllowColumnReorder=(false);
			componentResourceManager.ApplyResources(_treeTableView, "_treeTableView");
			_treeTableView.AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_treeTableView).BackColor = System.Drawing.SystemColors.Window;
			_treeTableView.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			_treeTableView.DoNotShrinkColumnsAutomatically=(false);
			_treeTableView.ForceFocusOnClick=(false);
			_treeTableView.GridLines=(false);
			_treeTableView.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Nonclickable);
			_treeTableView.HideSelection=(false);
			_treeTableView.ImmediateEdit=(false);
			_treeTableView.Indent=(20);
			_treeTableView.KeepColumnWidthsAdjusted=(false);
			_treeTableView.Model=((ITreeTableModel)null);
			_treeTableView.MultiSelect=(false);
			((System.Windows.Forms.Control)(object)_treeTableView).Name = "_treeTableView";
			_treeTableView.NoSearchStrings=(false);
			_treeTableView.OnlyWhenFocused=(false);
			_treeTableView.OpenEditOnDblClk=(true);
			_treeTableView.ReadOnly=(false);
			_treeTableView.Scrollable=(true);
			_treeTableView.ShowLines=(false);
			_treeTableView.ShowPlusMinus=(false);
			_treeTableView.ShowRootLines=(false);
			_treeTableView.ToggleOnDblClk=(false);
			componentResourceManager.ApplyResources(label1, "label1");
			label1.Name = "label1";
			componentResourceManager.ApplyResources(_noteLabel, "_noteLabel");
			_noteLabel.BackColor = System.Drawing.SystemColors.Info;
			_noteLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			_noteLabel.ForeColor = System.Drawing.Color.Blue;
			_noteLabel.Name = "_noteLabel";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(_noteLabel);
			base.Controls.Add((System.Windows.Forms.Control)(object)_treeTableView);
			base.Controls.Add(label1);
			base.Name = "DeviceVersionSelectionControl";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
