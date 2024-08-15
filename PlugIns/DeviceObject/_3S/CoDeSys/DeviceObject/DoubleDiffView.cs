#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.ProjectCompare;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DoubleDiffView
	{
		private int _nLeftProjectHandle;

		private int _nRightProjectHandle;

		private Guid _leftObjectGuid;

		private Guid _rightObjectGuid;

		private int _nAdditionsCount;

		private int _nDeletionsCount;

		private int _nChangesCount;

		private DeviceObjectDiffViewModel _leftmodel;

		private DeviceObjectDiffViewModel _rightmodel;

		private LDictionary<DeviceObjectDiffViewNode, DeviceObjectDiffViewNode> _htLeftNodes = new LDictionary<DeviceObjectDiffViewNode, DeviceObjectDiffViewNode>();

		private LDictionary<DeviceObjectDiffViewNode, DeviceObjectDiffViewNode> _htRightNodes = new LDictionary<DeviceObjectDiffViewNode, DeviceObjectDiffViewNode>();

		private LDictionary<IParameterSection, DiffViewNodeNodes> _htSectionsLeft = new LDictionary<IParameterSection, DiffViewNodeNodes>();

		private LDictionary<IParameterSection, DiffViewNodeNodes> _htSectionsRight = new LDictionary<IParameterSection, DiffViewNodeNodes>();

		internal LDictionary<DeviceObjectDiffViewNode, DeviceObjectDiffViewNode> LeftNodes => _htLeftNodes;

		internal LDictionary<DeviceObjectDiffViewNode, DeviceObjectDiffViewNode> RightNodes => _htRightNodes;

		internal int AdditionsCount => _nAdditionsCount;

		internal int DeletionsCount => _nDeletionsCount;

		internal int ChangesCount => _nChangesCount;

		internal DoubleDiffView(int nLeftProjectHandle, Guid leftObjectGuid, int nRightProjectHandle, Guid rightObjectGuid, DeviceObjectDiffViewModel leftmodel, DeviceObjectDiffViewModel rightmodel)
		{
			_nLeftProjectHandle = nLeftProjectHandle;
			_leftObjectGuid = leftObjectGuid;
			_nRightProjectHandle = nRightProjectHandle;
			_rightObjectGuid = rightObjectGuid;
			_leftmodel = leftmodel;
			_rightmodel = rightmodel;
		}

		internal void FillFbInstances(IDriverInfo driverInfo, LList<IFbInstance> liFbInstances)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			foreach (IRequiredLib item in (IEnumerable)driverInfo.RequiredLibs)
			{
				foreach (IFbInstance item2 in (IEnumerable)item.FbInstances)
				{
					IFbInstance val = item2;
					liFbInstances.Add(val);
				}
			}
		}

		internal void FillAddressHashTable(IIoProvider provider, IParameterSet paramSet)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Invalid comparison between Unknown and I4
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Invalid comparison between Unknown and I4
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Invalid comparison between Unknown and I4
			if (((ICollection)paramSet).Count <= 0)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			LateLanguageStartAddresses lateLanguageStartAddresses = new LateLanguageStartAddresses();
			foreach (Parameter item in DeviceObjectHelper.SortedParameterSet(provider))
			{
				if (!flag && (int)item.ChannelType == 1)
				{
					flag = true;
					lateLanguageStartAddresses.startInAddress = (item.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses);
					DeviceObjectHelper.HashIecAddresses[provider] = lateLanguageStartAddresses;
				}
				if (!flag2 && ((int)item.ChannelType == 2 || (int)item.ChannelType == 3))
				{
					flag2 = true;
					lateLanguageStartAddresses.startOutAddress = (item.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses);
					DeviceObjectHelper.HashIecAddresses[provider] = lateLanguageStartAddresses;
				}
			}
		}

		private bool IsConnectorToParent(IConnector connector, Guid guidParent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			foreach (IAdapter item in (IEnumerable)connector.Adapters)
			{
				Guid[] modules = item.Modules;
				for (int i = 0; i < modules.Length; i++)
				{
					if (modules[i] == guidParent)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool CheckConnector(IConnectorCollection Connectors, IConnector con, Guid parentGuid)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			bool result = true;
			if (con.IsExplicit)
			{
				result = false;
			}
			if ((int)con.ConnectorRole == 1)
			{
				if (!IsConnectorToParent(con, parentGuid))
				{
					result = false;
				}
			}
			else
			{
				bool flag = true;
				foreach (IConnector item in (IEnumerable)Connectors)
				{
					IConnector val = item;
					if (val.ConnectorId == con.HostPath && !IsConnectorToParent(val, parentGuid))
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					result = false;
				}
			}
			return result;
		}

		internal void UpdateContents()
		{
			bool flag = false;
			_nAdditionsCount = 0;
			_nDeletionsCount = 0;
			_nChangesCount = 0;
			((DefaultTreeTableModel)_leftmodel).ClearRootNodes();
			((DefaultTreeTableModel)_rightmodel).ClearRootNodes();
			LDictionary<int, ParameterSet> val = new LDictionary<int, ParameterSet>();
			LList<IFbInstance> val2 = new LList<IFbInstance>();
			LDictionary<int, AlwaysMappingMode> val3 = new LDictionary<int, AlwaysMappingMode>();
			LDictionary<int, IDriverInfo5> val4 = new LDictionary<int, IDriverInfo5>();
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nLeftProjectHandle, _leftObjectGuid);
			IDiffViewerDeviceParameterFilterFactory factory = DeviceParameterFactoryManager.Instance.GetFactory(objectToRead);
			if (objectToRead != null && objectToRead.Object is IDeviceObject)
			{
				IObject @object = objectToRead.Object;
				IDeviceObject val5 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				if (val5 is IDeviceObject9)
				{
					flag |= ((IDeviceObject9)((val5 is IDeviceObject9) ? val5 : null)).ShowParamsInDevDescOrder;
				}
				int connectorId = (val5.DeviceParameterSet as ParameterSet).ConnectorId;
				IDriverInfo driverInfo = ((IDeviceObject2)((val5 is IDeviceObject2) ? val5 : null)).DriverInfo;
				val4.Add(connectorId, (IDriverInfo5)(object)((driverInfo is IDriverInfo5) ? driverInfo : null));
				FillFbInstances(((IDeviceObject2)((val5 is IDeviceObject2) ? val5 : null)).DriverInfo, val2);
				val.Add((val5.DeviceParameterSet as ParameterSet).ConnectorId, val5.DeviceParameterSet as ParameterSet);
				IDriverInfo driverInfo2 = ((IDeviceObject2)((val5 is IDeviceObject2) ? val5 : null)).DriverInfo;
				if (((IDriverInfo6)((driverInfo2 is IDriverInfo11) ? driverInfo2 : null)).PlcAlwaysMapping)
				{
					IDriverInfo driverInfo3 = ((IDeviceObject2)((val5 is IDeviceObject2) ? val5 : null)).DriverInfo;
					val3.Add(-2, ((IDriverInfo11)((driverInfo3 is IDriverInfo11) ? driverInfo3 : null)).PlcAlwaysMappingMode);
				}
				if ((val5.DeviceParameterSet as ParameterSet).AlwaysMapping)
				{
					val3.Add((val5.DeviceParameterSet as ParameterSet).ConnectorId, (val5.DeviceParameterSet as ParameterSet).AlwaysMappingMode);
				}
				foreach (IConnector item in (IEnumerable)val5.Connectors)
				{
					IConnector val6 = item;
					if (CheckConnector(val5.Connectors, val6, objectToRead.ParentObjectGuid))
					{
						val.Add(val6.ConnectorId, val6.HostParameterSet as ParameterSet);
						int connectorId2 = val6.ConnectorId;
						IDriverInfo driverInfo4 = ((IConnector2)((val6 is IConnector2) ? val6 : null)).DriverInfo;
						val4.Add(connectorId2, (IDriverInfo5)(object)((driverInfo4 is IDriverInfo5) ? driverInfo4 : null));
						FillFbInstances(((IConnector2)((val6 is IConnector2) ? val6 : null)).DriverInfo, val2);
						FillAddressHashTable((IIoProvider)(object)((val6 is IIoProvider) ? val6 : null), val6.HostParameterSet);
						if (((IConnector6)((val6 is IConnector11) ? val6 : null)).AlwaysMapping)
						{
							val3.Add(val6.ConnectorId, ((IConnector11)((val6 is IConnector11) ? val6 : null)).AlwaysMappingMode);
						}
					}
				}
			}
			if (objectToRead != null && objectToRead.Object is IExplicitConnector)
			{
				IObject object2 = objectToRead.Object;
				IExplicitConnector val7 = (IExplicitConnector)(object)((object2 is IExplicitConnector) ? object2 : null);
				val.Add(((IConnector)val7).ConnectorId, ((IConnector)val7).HostParameterSet as ParameterSet);
				int connectorId3 = ((IConnector)val7).ConnectorId;
				IDriverInfo driverInfo5 = ((IConnector2)((val7 is IConnector2) ? val7 : null)).DriverInfo;
				val4.Add(connectorId3, (IDriverInfo5)(object)((driverInfo5 is IDriverInfo5) ? driverInfo5 : null));
				FillFbInstances(((IConnector2)((val7 is IConnector2) ? val7 : null)).DriverInfo, val2);
				FillAddressHashTable((IIoProvider)(object)((val7 is IIoProvider) ? val7 : null), ((IConnector)val7).HostParameterSet);
				if (((IConnector6)((val7 is IConnector11) ? val7 : null)).AlwaysMapping)
				{
					val3.Add(((IConnector)val7).ConnectorId, ((IConnector11)((val7 is IConnector11) ? val7 : null)).AlwaysMappingMode);
				}
			}
			LDictionary<int, ParameterSet> val8 = new LDictionary<int, ParameterSet>();
			LList<IFbInstance> val9 = new LList<IFbInstance>();
			LDictionary<int, IDriverInfo5> val10 = new LDictionary<int, IDriverInfo5>();
			LDictionary<int, AlwaysMappingMode> val11 = new LDictionary<int, AlwaysMappingMode>();
			IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nRightProjectHandle, _rightObjectGuid);
			IDiffViewerDeviceParameterFilterFactory factory2 = DeviceParameterFactoryManager.Instance.GetFactory(objectToRead2);
			if (objectToRead2 != null && objectToRead2.Object is IDeviceObject)
			{
				IObject object3 = objectToRead2.Object;
				IDeviceObject val12 = (IDeviceObject)(object)((object3 is IDeviceObject) ? object3 : null);
				if (val12 is IDeviceObject9)
				{
					flag |= ((IDeviceObject9)((val12 is IDeviceObject9) ? val12 : null)).ShowParamsInDevDescOrder;
				}
				int connectorId4 = (val12.DeviceParameterSet as ParameterSet).ConnectorId;
				IDriverInfo driverInfo6 = ((IDeviceObject2)((val12 is IDeviceObject2) ? val12 : null)).DriverInfo;
				val10.Add(connectorId4, (IDriverInfo5)(object)((driverInfo6 is IDriverInfo5) ? driverInfo6 : null));
				FillFbInstances(((IDeviceObject2)((val12 is IDeviceObject2) ? val12 : null)).DriverInfo, val9);
				val8.Add((val12.DeviceParameterSet as ParameterSet).ConnectorId, val12.DeviceParameterSet as ParameterSet);
				IDriverInfo driverInfo7 = ((IDeviceObject2)((val12 is IDeviceObject2) ? val12 : null)).DriverInfo;
				if (((IDriverInfo6)((driverInfo7 is IDriverInfo11) ? driverInfo7 : null)).PlcAlwaysMapping)
				{
					IDriverInfo driverInfo8 = ((IDeviceObject2)((val12 is IDeviceObject2) ? val12 : null)).DriverInfo;
					val11.Add(-2, ((IDriverInfo11)((driverInfo8 is IDriverInfo11) ? driverInfo8 : null)).PlcAlwaysMappingMode);
				}
				if ((val12.DeviceParameterSet as ParameterSet).AlwaysMapping)
				{
					val11.Add((val12.DeviceParameterSet as ParameterSet).ConnectorId, (val12.DeviceParameterSet as ParameterSet).AlwaysMappingMode);
				}
				foreach (IConnector item2 in (IEnumerable)val12.Connectors)
				{
					IConnector val13 = item2;
					if (CheckConnector(val12.Connectors, val13, objectToRead2.ParentObjectGuid))
					{
						val8.Add(val13.ConnectorId, val13.HostParameterSet as ParameterSet);
						int connectorId5 = val13.ConnectorId;
						IDriverInfo driverInfo9 = ((IConnector2)((val13 is IConnector2) ? val13 : null)).DriverInfo;
						val10.Add(connectorId5, (IDriverInfo5)(object)((driverInfo9 is IDriverInfo5) ? driverInfo9 : null));
						FillFbInstances(((IConnector2)((val13 is IConnector2) ? val13 : null)).DriverInfo, val9);
						FillAddressHashTable((IIoProvider)(object)((val13 is IIoProvider) ? val13 : null), val13.HostParameterSet);
						if (((IConnector6)((val13 is IConnector11) ? val13 : null)).AlwaysMapping)
						{
							val11.Add(val13.ConnectorId, ((IConnector11)((val13 is IConnector11) ? val13 : null)).AlwaysMappingMode);
						}
					}
				}
			}
			if (objectToRead2 != null && objectToRead2.Object is IExplicitConnector)
			{
				IObject object4 = objectToRead2.Object;
				IExplicitConnector val14 = (IExplicitConnector)(object)((object4 is IExplicitConnector) ? object4 : null);
				val8.Add(((IConnector)val14).ConnectorId, ((IConnector)val14).HostParameterSet as ParameterSet);
				int connectorId6 = ((IConnector)val14).ConnectorId;
				IDriverInfo driverInfo10 = ((IConnector2)((val14 is IConnector2) ? val14 : null)).DriverInfo;
				val10.Add(connectorId6, (IDriverInfo5)(object)((driverInfo10 is IDriverInfo5) ? driverInfo10 : null));
				FillFbInstances(((IConnector2)((val14 is IConnector2) ? val14 : null)).DriverInfo, val9);
				FillAddressHashTable((IIoProvider)(object)((val14 is IIoProvider) ? val14 : null), ((IConnector)val14).HostParameterSet);
				if (((IConnector6)((val14 is IConnector11) ? val14 : null)).AlwaysMapping)
				{
					val11.Add(((IConnector)val14).ConnectorId, ((IConnector11)((val14 is IConnector11) ? val14 : null)).AlwaysMappingMode);
				}
			}
			LDictionary<int, LSortedList<long, Parameter>> val15 = new LDictionary<int, LSortedList<long, Parameter>>();
			LDictionary<int, LSortedList<long, Parameter>> val16 = new LDictionary<int, LSortedList<long, Parameter>>();
			LDictionary<int, LSortedList<long, Parameter>> val17 = new LDictionary<int, LSortedList<long, Parameter>>();
			LDictionary<int, LSortedList<long, Parameter>> val18 = new LDictionary<int, LSortedList<long, Parameter>>();
			foreach (KeyValuePair<int, ParameterSet> keyValuePair in val)
			{
				LSortedList<long, Parameter> lsortedList = new LSortedList<long, Parameter>();
				LSortedList<long, Parameter> lsortedList2 = new LSortedList<long, Parameter>();
				foreach (object obj3 in keyValuePair.Value)
				{
					Parameter parameter = (Parameter)obj3;
					if (parameter != null && flag)
					{
						long key = (long)((int)((IParameter6)parameter).IndexInDevDesc);
						if (lsortedList.ContainsKey(key))
						{
							key = lsortedList.Keys[lsortedList.Count - 1] + 1L;
						}
						lsortedList.Add(key, parameter);
					}
					else
					{
						lsortedList.Add(parameter.Id, parameter);
					}
					lsortedList2.Add(parameter.Id, parameter);
				}
				val17.Add(keyValuePair.Value.ConnectorId, lsortedList2);
				val15.Add(keyValuePair.Value.ConnectorId, lsortedList);
			}
			foreach (KeyValuePair<int, ParameterSet> keyValuePair2 in val8)
			{
				LSortedList<long, Parameter> lsortedList3 = new LSortedList<long, Parameter>();
				LSortedList<long, Parameter> lsortedList4 = new LSortedList<long, Parameter>();
				foreach (object obj4 in keyValuePair2.Value)
				{
					Parameter parameter2 = (Parameter)obj4;
					if (parameter2 != null && flag)
					{
						long key2 = (long)((int)((IParameter6)parameter2).IndexInDevDesc);
						if (lsortedList3.ContainsKey(key2))
						{
							key2 = lsortedList3.Keys[lsortedList3.Count - 1] + 1L;
						}
						lsortedList3.Add(key2, parameter2);
					}
					else
					{
						lsortedList3.Add(parameter2.Id, parameter2);
					}
					lsortedList4.Add(parameter2.Id, parameter2);
				}
				val18.Add(keyValuePair2.Value.ConnectorId, lsortedList4);
				val16.Add(keyValuePair2.Value.ConnectorId, lsortedList3);
			}
			foreach (KeyValuePair<int, LSortedList<long, Parameter>> keyValuePair3 in val15)
			{
				LSortedList<long, Parameter> value = keyValuePair3.Value;
				LSortedList<long, Parameter> lsortedList5;
				LSortedList<long, Parameter> lsortedList6;
				LSortedList<long, Parameter> lsortedList7;
				if (val18.TryGetValue(keyValuePair3.Key, out lsortedList5) && val16.TryGetValue(keyValuePair3.Key, out lsortedList6) && val17.TryGetValue(keyValuePair3.Key, out lsortedList7))
				{
					foreach (KeyValuePair<long, Parameter> keyValuePair4 in value)
					{
						Parameter parameterRight;
						if (lsortedList5.TryGetValue(keyValuePair4.Value.Id, out parameterRight))
						{
							this.AddParameterNode(factory, objectToRead, keyValuePair3.Key, keyValuePair4.Value, parameterRight);
						}
						else
						{
							this.AddParameterNode(factory, objectToRead, keyValuePair3.Key, keyValuePair4.Value, null);
						}
					}
					using (IEnumerator<KeyValuePair<long, Parameter>> enumerator4 = lsortedList6.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							KeyValuePair<long, Parameter> keyValuePair5 = enumerator4.Current;
							Parameter parameter3;
							if (!lsortedList7.TryGetValue(keyValuePair5.Value.Id, out parameter3))
							{
								this.AddParameterNode(factory, objectToRead, keyValuePair3.Key, null, keyValuePair5.Value);
							}
						}
						continue;
					}
				}
				foreach (Parameter parameterLeft in value.Values)
				{
					this.AddParameterNode(factory, objectToRead, keyValuePair3.Key, parameterLeft, null);
				}
			}
			foreach (KeyValuePair<int, LSortedList<long, Parameter>> keyValuePair6 in val16)
			{
				LSortedList<long, Parameter> lsortedList8;
				if (!val15.TryGetValue(keyValuePair6.Key, out lsortedList8))
				{
					foreach (Parameter parameterRight2 in keyValuePair6.Value.Values)
					{
						this.AddParameterNode(factory2, objectToRead2, keyValuePair6.Key, null, parameterRight2);
					}
				}
			}
			foreach (IFbInstance5 item7 in val2)
			{
				IFbInstance5 val27 = item7;
				bool flag2 = false;
				foreach (IFbInstance5 item8 in val9)
				{
					IFbInstance5 val28 = item8;
					if (val27.BaseName == val28.BaseName)
					{
						flag2 = true;
						AddFbInstanceNode((IFbInstance)(object)val27, (IFbInstance)(object)val28);
					}
				}
				if (!flag2)
				{
					AddFbInstanceNode((IFbInstance)(object)val27, null);
				}
			}
			foreach (IFbInstance5 item9 in val9)
			{
				IFbInstance5 val29 = item9;
				bool flag3 = false;
				foreach (IFbInstance5 item10 in val2)
				{
					if (item10.BaseName == val29.BaseName)
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					AddFbInstanceNode(null, (IFbInstance)(object)val29);
				}
			}
			foreach (KeyValuePair<int, IDriverInfo5> keyValuePair7 in val4)
			{
				bool flag4 = false;
				foreach (KeyValuePair<int, IDriverInfo5> keyValuePair8 in val10)
				{
					if (keyValuePair7.Key == keyValuePair8.Key && (!string.IsNullOrEmpty(keyValuePair7.Value.BusCycleTask) || keyValuePair7.Value.BusCycleTaskGuid != Guid.Empty) && (!string.IsNullOrEmpty(keyValuePair8.Value.BusCycleTask) || keyValuePair8.Value.BusCycleTaskGuid != Guid.Empty))
					{
						flag4 = true;
						this.AddBusCycleNode(this._nLeftProjectHandle, this._nRightProjectHandle, keyValuePair7.Value, keyValuePair8.Value);
					}
				}
				if (!flag4 && (!string.IsNullOrEmpty(keyValuePair7.Value.BusCycleTask) || keyValuePair7.Value.BusCycleTaskGuid != Guid.Empty))
				{
					this.AddBusCycleNode(this._nLeftProjectHandle, this._nRightProjectHandle, keyValuePair7.Value, null);
				}
			}
			foreach (KeyValuePair<int, IDriverInfo5> keyValuePair9 in val10)
			{
				bool flag5 = false;
				foreach (KeyValuePair<int, IDriverInfo5> keyValuePair10 in val4)
				{
					if (keyValuePair10.Key == keyValuePair9.Key && (!string.IsNullOrEmpty(keyValuePair10.Value.BusCycleTask) || keyValuePair10.Value.BusCycleTaskGuid != Guid.Empty) && (!string.IsNullOrEmpty(keyValuePair9.Value.BusCycleTask) || keyValuePair9.Value.BusCycleTaskGuid != Guid.Empty))
					{
						flag5 = true;
						break;
					}
				}
				if (!flag5 && (!string.IsNullOrEmpty(keyValuePair9.Value.BusCycleTask) || keyValuePair9.Value.BusCycleTaskGuid != Guid.Empty))
				{
					this.AddBusCycleNode(this._nLeftProjectHandle, this._nRightProjectHandle, null, keyValuePair9.Value);
				}
			}
			foreach (KeyValuePair<int, AlwaysMappingMode> keyValuePair11 in val3)
			{
				bool flag6 = false;
				foreach (KeyValuePair<int, AlwaysMappingMode> keyValuePair12 in val11)
				{
					if (keyValuePair11.Key == keyValuePair12.Key)
					{
						flag6 = true;
						this.AddAlwaysMappingNode(new AlwaysMappingMode?(keyValuePair11.Value), new AlwaysMappingMode?(keyValuePair12.Value));
					}
				}
				if (!flag6)
				{
					this.AddAlwaysMappingNode(new AlwaysMappingMode?(keyValuePair11.Value), null);
				}
			}
			foreach (KeyValuePair<int, AlwaysMappingMode> keyValuePair13 in val11)
			{
				bool flag7 = false;
				foreach (KeyValuePair<int, AlwaysMappingMode> keyValuePair14 in val3)
				{
					if (keyValuePair14.Key == keyValuePair13.Key)
					{
						flag7 = true;
						break;
					}
				}
				if (!flag7)
				{
					this.AddAlwaysMappingNode(null, new AlwaysMappingMode?(keyValuePair13.Value));
				}
			}
			this._leftmodel.RaiseStructureChanged(new TreeTableModelEventArgs(null, -1, null));
			this._rightmodel.RaiseStructureChanged(new TreeTableModelEventArgs(null, -1, null));
			if (this._nAdditionsCount == 0 && this._nDeletionsCount == 0 && this._nChangesCount == 0 && !new DeviceObjectComparer().CheckEquality(objectToRead.Object, objectToRead2.Object, true, true, true))
			{
				this._nChangesCount++;
				DeviceObjectDiffViewNodeData data = new DeviceObjectDiffViewNodeData(Strings.HiddenChanges, Color.Red, Color.LightGray, FontStyle.Bold, null);
				this._leftmodel.AddRootNode(new MessageNode(data));
				data = new DeviceObjectDiffViewNodeData(string.Empty, Color.Red, Color.White, FontStyle.Bold, null);
				this._rightmodel.AddRootNode(new MessageNode(data));
			}
		}

		internal void AddFbInstanceNode(IFbInstance instanceLeft, IFbInstance instanceRight)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			DiffState diffState = (DiffState)0;
			if (instanceLeft == null)
			{
				diffState = (DiffState)2;
				_nDeletionsCount++;
			}
			else if (instanceRight == null)
			{
				diffState = (DiffState)1;
				_nAdditionsCount++;
			}
			else
			{
				if (instanceLeft.Instance.Variable != instanceRight.Instance.Variable)
				{
					diffState = (DiffState)4;
					_nChangesCount++;
				}
				if (instanceLeft.FbName != instanceRight.FbName)
				{
					diffState = (DiffState)4;
					_nChangesCount++;
				}
				if (((IFbInstance3)((instanceLeft is IFbInstance3) ? instanceLeft : null)).FbNameDiag != ((IFbInstance3)((instanceRight is IFbInstance3) ? instanceRight : null)).FbNameDiag)
				{
					diffState = (DiffState)4;
					_nChangesCount++;
				}
			}
			NodeDiffData diffData = new NodeDiffData(diffState);
			FbInstancesDiffNode fbInstancesDiffNode = new FbInstancesDiffNode(_leftmodel, instanceLeft, instanceRight, diffData, ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount);
			FbInstancesDiffNode fbInstancesDiffNode2 = new FbInstancesDiffNode(_rightmodel, instanceLeft, instanceRight, diffData, ((DefaultTreeTableModel)_rightmodel).Sentinel.ChildCount);
			((DefaultTreeTableModel)_leftmodel).AddRootNode((ITreeTableNode)(object)fbInstancesDiffNode);
			((DefaultTreeTableModel)_rightmodel).AddRootNode((ITreeTableNode)(object)fbInstancesDiffNode2);
			_htLeftNodes.Add((DeviceObjectDiffViewNode)fbInstancesDiffNode, (DeviceObjectDiffViewNode)fbInstancesDiffNode2);
			_htRightNodes.Add((DeviceObjectDiffViewNode)fbInstancesDiffNode2, (DeviceObjectDiffViewNode)fbInstancesDiffNode);
		}

		internal void AddBusCycleNode(int nLeftProjectHandle, int nRightProjectHandle, IDriverInfo5 instanceLeft, IDriverInfo5 instanceRight)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			DiffState diffState = (DiffState)0;
			if (instanceLeft == null)
			{
				diffState = (DiffState)2;
				_nDeletionsCount++;
			}
			else if (instanceRight == null)
			{
				diffState = (DiffState)1;
				_nAdditionsCount++;
			}
			else if (instanceLeft.BusCycleTaskGuid != instanceRight.BusCycleTaskGuid || ((IDriverInfo)instanceLeft).BusCycleTask != ((IDriverInfo)instanceRight).BusCycleTask)
			{
				diffState = (DiffState)4;
				_nChangesCount++;
			}
			NodeDiffData diffData = new NodeDiffData(diffState);
			BusCycleDiffNode busCycleDiffNode = new BusCycleDiffNode(_leftmodel, nLeftProjectHandle, nRightProjectHandle, instanceLeft, instanceRight, diffData, ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount);
			BusCycleDiffNode busCycleDiffNode2 = new BusCycleDiffNode(_rightmodel, nLeftProjectHandle, nRightProjectHandle, instanceLeft, instanceRight, diffData, ((DefaultTreeTableModel)_rightmodel).Sentinel.ChildCount);
			((DefaultTreeTableModel)_leftmodel).AddRootNode((ITreeTableNode)(object)busCycleDiffNode);
			((DefaultTreeTableModel)_rightmodel).AddRootNode((ITreeTableNode)(object)busCycleDiffNode2);
			_htLeftNodes.Add((DeviceObjectDiffViewNode)busCycleDiffNode, (DeviceObjectDiffViewNode)busCycleDiffNode2);
			_htRightNodes.Add((DeviceObjectDiffViewNode)busCycleDiffNode2, (DeviceObjectDiffViewNode)busCycleDiffNode);
		}

		internal void AddAlwaysMappingNode(AlwaysMappingMode? instanceLeft, AlwaysMappingMode? instanceRight)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			DiffState diffState = (DiffState)0;
			if (!instanceLeft.HasValue)
			{
				diffState = (DiffState)2;
				_nDeletionsCount++;
			}
			else if (!instanceRight.HasValue)
			{
				diffState = (DiffState)1;
				_nAdditionsCount++;
			}
			else if (instanceLeft != instanceRight)
			{
				diffState = (DiffState)4;
				_nChangesCount++;
			}
			NodeDiffData diffData = new NodeDiffData(diffState);
			AlwaysMappingDiffNode alwaysMappingDiffNode = new AlwaysMappingDiffNode(_leftmodel, instanceLeft, instanceRight, diffData, ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount);
			AlwaysMappingDiffNode alwaysMappingDiffNode2 = new AlwaysMappingDiffNode(_rightmodel, instanceLeft, instanceRight, diffData, ((DefaultTreeTableModel)_rightmodel).Sentinel.ChildCount);
			((DefaultTreeTableModel)_leftmodel).AddRootNode((ITreeTableNode)(object)alwaysMappingDiffNode);
			((DefaultTreeTableModel)_rightmodel).AddRootNode((ITreeTableNode)(object)alwaysMappingDiffNode2);
			_htLeftNodes.Add((DeviceObjectDiffViewNode)alwaysMappingDiffNode, (DeviceObjectDiffViewNode)alwaysMappingDiffNode2);
			_htRightNodes.Add((DeviceObjectDiffViewNode)alwaysMappingDiffNode2, (DeviceObjectDiffViewNode)alwaysMappingDiffNode);
		}

		internal void AddParameterNode(IDiffViewerDeviceParameterFilterFactory factory, IMetaObject mo, int nConnectionId, IParameter parameterLeft, IParameter parameterRight)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Invalid comparison between Unknown and I4
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Invalid comparison between Unknown and I4
			DeviceObjectDiffViewNode parentLeftNode = null;
			DeviceObjectDiffViewNode parentRightNode = null;
			ParameterSection parameterSection = null;
			ParameterSection parameterSection2 = null;
			bool flag = false;
			if (factory != null && (factory.HideParameter(mo, parameterLeft) || factory.HideParameter(mo, parameterRight)))
			{
				return;
			}
			if (parameterLeft != null)
			{
				flag |= (int)parameterLeft.GetAccessRight(false) > 0;
				if (parameterLeft.Section != null)
				{
					parameterSection = parameterLeft.Section as ParameterSection;
				}
			}
			if (parameterRight != null)
			{
				flag |= (int)parameterRight.GetAccessRight(false) > 0;
				if (parameterRight.Section != null)
				{
					parameterSection2 = parameterRight.Section as ParameterSection;
				}
			}
			DeviceObjectDiffViewNode newParentLeftNode = null;
			DeviceObjectDiffViewNode newParentRightNode = null;
			if (parameterSection == null && parameterSection2 != null)
			{
				AddParameterSection((IParameterSection)(object)parameterSection, (IParameterSection)(object)parameterSection2, parentLeftNode, parentRightNode, out newParentLeftNode, out newParentRightNode);
				parentLeftNode = newParentLeftNode;
				parentRightNode = newParentRightNode;
			}
			if (parameterSection2 == null && parameterSection != null)
			{
				AddParameterSection((IParameterSection)(object)parameterSection, (IParameterSection)(object)parameterSection2, parentLeftNode, parentRightNode, out newParentLeftNode, out newParentRightNode);
				parentLeftNode = newParentLeftNode;
				parentRightNode = newParentRightNode;
			}
			if (parameterSection2 != null && parameterSection != null)
			{
				AddParameterSection((IParameterSection)(object)parameterSection, (IParameterSection)(object)parameterSection2, parentLeftNode, parentRightNode, out newParentLeftNode, out newParentRightNode);
				parentLeftNode = newParentLeftNode;
				parentRightNode = newParentRightNode;
			}
			if (!flag && parameterLeft == null && parameterRight != null)
			{
				flag = true;
			}
			if (!flag && parameterLeft != null && parameterRight == null)
			{
				flag = true;
			}
			if (!flag && parameterRight != null && parameterLeft != null && ((IDataElement)parameterLeft).Value != ((IDataElement)parameterRight).Value)
			{
				flag = true;
			}
			if (flag)
			{
				AddParameterSubNode(nConnectionId, parameterLeft as Parameter, parameterRight as Parameter, (IDataElement)(object)parameterLeft, (IDataElement)(object)parameterRight, parentLeftNode, parentRightNode);
			}
		}

		internal void AddParameterSubNode(int nConnectionId, Parameter leftParameter, Parameter rightParameter, IDataElement dataElementLeft, IDataElement dataElementRight, DeviceObjectDiffViewNode parentLeftNode, DeviceObjectDiffViewNode parentRightNode)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Invalid comparison between Unknown and I4
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Invalid comparison between Unknown and I4
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Invalid comparison between Unknown and I4
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Expected O, but got Unknown
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Expected O, but got Unknown
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Expected O, but got Unknown
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			DiffState diffState = (DiffState)0;
			if (dataElementLeft != null && !(dataElementLeft is Parameter) && dataElementLeft is IDataElement6 && (int)((IDataElement6)((dataElementLeft is IDataElement6) ? dataElementLeft : null)).GetAccessRight(false) == 0)
			{
				return;
			}
			if (dataElementLeft == null)
			{
				diffState = (DiffState)2;
				_nDeletionsCount++;
			}
			else if (dataElementRight == null)
			{
				diffState = (DiffState)1;
				_nAdditionsCount++;
			}
			else
			{
				string text = string.Empty;
				string text2 = string.Empty;
				string description = dataElementLeft.Description;
				string description2 = dataElementRight.Description;
				string text3 = string.Empty;
				string text4 = string.Empty;
				if (!dataElementLeft.HasSubElements)
				{
					text3 = dataElementLeft.Value;
				}
				if (!dataElementRight.HasSubElements)
				{
					text4 = dataElementRight.Value;
				}
				if (dataElementLeft.IoMapping != null && dataElementLeft.IoMapping.VariableMappings != null && ((ICollection)dataElementLeft.IoMapping.VariableMappings).Count > 0)
				{
					text = dataElementLeft.IoMapping.VariableMappings[0]
						.Variable;
				}
				if (dataElementRight.IoMapping != null && dataElementRight.IoMapping.VariableMappings != null && ((ICollection)dataElementRight.IoMapping.VariableMappings).Count > 0)
				{
					text2 = dataElementRight.IoMapping.VariableMappings[0]
						.Variable;
				}
				if (text3 != text4 || text != text2 || description != description2)
				{
					diffState = (DiffState)4;
					_nChangesCount++;
				}
				if (dataElementLeft.IoMapping.AutomaticIecAddress != dataElementRight.IoMapping.AutomaticIecAddress)
				{
					diffState = (DiffState)4;
					_nChangesCount++;
				}
				else if ((!dataElementRight.IoMapping.AutomaticIecAddress || !dataElementLeft.IoMapping.AutomaticIecAddress) && dataElementLeft.IoMapping.IecAddress != dataElementRight.IoMapping.IecAddress)
				{
					diffState = (DiffState)4;
					_nChangesCount++;
				}
			}
			DeviceObjectDiffData deviceObjectDiffData = new DeviceObjectDiffData(nConnectionId, leftParameter, rightParameter, dataElementLeft, dataElementRight);
			NodeDiffData nodeDiff = new NodeDiffData(diffState);
			ParameterDiffViewNode parameterDiffViewNode = new ParameterDiffViewNode(_leftmodel, parentLeftNode, deviceObjectDiffData, nodeDiff, parentLeftNode?.ChildCount ?? ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount);
			ParameterDiffViewNode parameterDiffViewNode2 = new ParameterDiffViewNode(_rightmodel, parentRightNode, deviceObjectDiffData, nodeDiff, parentRightNode?.ChildCount ?? ((DefaultTreeTableModel)_rightmodel).Sentinel.ChildCount);
			_htLeftNodes.Add((DeviceObjectDiffViewNode)parameterDiffViewNode, (DeviceObjectDiffViewNode)parameterDiffViewNode2);
			_htRightNodes.Add((DeviceObjectDiffViewNode)parameterDiffViewNode2, (DeviceObjectDiffViewNode)parameterDiffViewNode);
			if (dataElementLeft is Parameter || dataElementRight is Parameter)
			{
				Parameter parameter = null;
				if (dataElementRight != null)
				{
					parameter = dataElementRight as Parameter;
				}
				if (dataElementLeft != null)
				{
					parameter = dataElementLeft as Parameter;
				}
				if ((int)parameter.ChannelType == 1)
				{
					deviceObjectDiffData.Image = DeviceObjectDiffViewNodeRenderer.s_inputChannelImage;
				}
				if ((int)parameter.ChannelType == 2 || (int)parameter.ChannelType == 3)
				{
					deviceObjectDiffData.Image = DeviceObjectDiffViewNodeRenderer.s_outputChannelImage;
				}
			}
			if (deviceObjectDiffData.Image == null)
			{
				bool flag = false;
				if (dataElementLeft != null)
				{
					flag |= dataElementLeft.HasSubElements;
				}
				if (dataElementRight != null)
				{
					flag |= dataElementRight.HasSubElements;
				}
				if (flag)
				{
					deviceObjectDiffData.Image = DeviceObjectDiffViewNodeRenderer.s_structuredParamImage;
				}
				else
				{
					deviceObjectDiffData.Image = DeviceObjectDiffViewNodeRenderer.s_paramImage;
				}
			}
			if (parentLeftNode == null)
			{
				((DefaultTreeTableModel)_leftmodel).AddRootNode((ITreeTableNode)(object)parameterDiffViewNode);
			}
			else
			{
				parentLeftNode.AddChildNode(parameterDiffViewNode);
			}
			if (parentRightNode == null)
			{
				((DefaultTreeTableModel)_rightmodel).AddRootNode((ITreeTableNode)(object)parameterDiffViewNode2);
			}
			else
			{
				parentRightNode.AddChildNode(parameterDiffViewNode2);
			}
			if (dataElementLeft != null && dataElementLeft.HasSubElements)
			{
				foreach (IDataElement item in (IEnumerable)dataElementLeft.SubElements)
				{
					IDataElement val = item;
					bool flag2 = false;
					if (dataElementRight != null && dataElementRight.HasSubElements)
					{
						foreach (IDataElement item2 in (IEnumerable)dataElementRight.SubElements)
						{
							IDataElement val2 = item2;
							if (val.Identifier == val2.Identifier)
							{
								flag2 = true;
								AddParameterSubNode(nConnectionId, leftParameter, rightParameter, val, val2, parameterDiffViewNode, parameterDiffViewNode2);
							}
						}
					}
					if (!flag2)
					{
						AddParameterSubNode(nConnectionId, leftParameter, rightParameter, val, null, parameterDiffViewNode, parameterDiffViewNode2);
					}
				}
			}
			if (dataElementRight == null || !dataElementRight.HasSubElements)
			{
				return;
			}
			foreach (IDataElement item3 in (IEnumerable)dataElementRight.SubElements)
			{
				IDataElement val3 = item3;
				bool flag3 = false;
				if (dataElementLeft != null && dataElementLeft.HasSubElements)
				{
					foreach (IDataElement item4 in (IEnumerable)dataElementLeft.SubElements)
					{
						if (item4.Identifier == val3.Identifier)
						{
							flag3 = true;
						}
					}
				}
				if (!flag3)
				{
					AddParameterSubNode(nConnectionId, null, rightParameter, null, val3, parameterDiffViewNode, parameterDiffViewNode2);
				}
			}
		}

		internal void AddParameterSection(IParameterSection sectionLeft, IParameterSection sectionRight, DeviceObjectDiffViewNode parentLeftNode, DeviceObjectDiffViewNode parentRightNode, out DeviceObjectDiffViewNode newParentLeftNode, out DeviceObjectDiffViewNode newParentRightNode)
		{
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			newParentLeftNode = null;
			newParentRightNode = null;
			if (sectionLeft != null && _htSectionsLeft.ContainsKey(sectionLeft))
			{
				newParentLeftNode = _htSectionsLeft[sectionLeft].leftnode;
				newParentRightNode = _htSectionsLeft[sectionLeft].rightnode;
				return;
			}
			if (sectionRight != null && _htSectionsRight.ContainsKey(sectionRight))
			{
				newParentLeftNode = _htSectionsRight[sectionRight].leftnode;
				newParentRightNode = _htSectionsRight[sectionRight].rightnode;
				return;
			}
			ParameterSection parameterSection = null;
			ParameterSection parameterSection2 = null;
			if (sectionLeft != null && sectionLeft.Section != null)
			{
				parameterSection = sectionLeft.Section as ParameterSection;
			}
			if (sectionRight != null && sectionRight.Section != null)
			{
				parameterSection2 = sectionRight.Section as ParameterSection;
			}
			if (parameterSection == null && parameterSection2 != null)
			{
				AddParameterSection((IParameterSection)(object)parameterSection, (IParameterSection)(object)parameterSection2, parentLeftNode, parentRightNode, out newParentLeftNode, out newParentRightNode);
				parentLeftNode = newParentLeftNode;
				parentRightNode = newParentRightNode;
			}
			if (parameterSection2 == null && parameterSection != null)
			{
				AddParameterSection((IParameterSection)(object)parameterSection, (IParameterSection)(object)parameterSection2, parentLeftNode, parentRightNode, out newParentLeftNode, out newParentRightNode);
				parentLeftNode = newParentLeftNode;
				parentRightNode = newParentRightNode;
			}
			if (parameterSection2 != null && parameterSection != null)
			{
				AddParameterSection((IParameterSection)(object)parameterSection, (IParameterSection)(object)parameterSection2, parentLeftNode, parentRightNode, out newParentLeftNode, out newParentRightNode);
				parentLeftNode = newParentLeftNode;
				parentRightNode = newParentRightNode;
			}
			DiffState diffState = (DiffState)0;
			if (sectionLeft == null)
			{
				diffState = (DiffState)2;
				_nDeletionsCount++;
			}
			else if (sectionRight == null)
			{
				diffState = (DiffState)1;
				_nAdditionsCount++;
			}
			else if (sectionLeft.Name != sectionRight.Name)
			{
				diffState = (DiffState)4;
				_nChangesCount++;
			}
			DeviceObjectDiffData deviceObjectDiffData = new DeviceObjectDiffData(sectionLeft, sectionRight);
			deviceObjectDiffData.Image = DeviceObjectDiffViewNodeRenderer.s_SectionImage;
			NodeDiffData nodeDiff = new NodeDiffData(diffState);
			ParameterDiffViewNode parameterDiffViewNode = new ParameterDiffViewNode(_leftmodel, parentLeftNode, deviceObjectDiffData, nodeDiff, parentLeftNode?.ChildCount ?? ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount);
			ParameterDiffViewNode parameterDiffViewNode2 = new ParameterDiffViewNode(_rightmodel, parentRightNode, deviceObjectDiffData, nodeDiff, parentRightNode?.ChildCount ?? ((DefaultTreeTableModel)_rightmodel).Sentinel.ChildCount);
			_htLeftNodes.Add((DeviceObjectDiffViewNode)parameterDiffViewNode, (DeviceObjectDiffViewNode)parameterDiffViewNode2);
			_htRightNodes.Add((DeviceObjectDiffViewNode)parameterDiffViewNode2, (DeviceObjectDiffViewNode)parameterDiffViewNode);
			if (parentLeftNode == null)
			{
				((DefaultTreeTableModel)_leftmodel).AddRootNode((ITreeTableNode)(object)parameterDiffViewNode);
			}
			else
			{
				parentLeftNode.AddChildNode(parameterDiffViewNode);
			}
			if (parentRightNode == null)
			{
				((DefaultTreeTableModel)_rightmodel).AddRootNode((ITreeTableNode)(object)parameterDiffViewNode2);
			}
			else
			{
				parentRightNode.AddChildNode(parameterDiffViewNode2);
			}
			DiffViewNodeNodes diffViewNodeNodes = new DiffViewNodeNodes();
			diffViewNodeNodes.leftnode = parameterDiffViewNode;
			diffViewNodeNodes.rightnode = parameterDiffViewNode2;
			if (sectionLeft != null)
			{
				_htSectionsLeft.Add(sectionLeft, diffViewNodeNodes);
			}
			if (sectionRight != null)
			{
				_htSectionsRight.Add(sectionRight, diffViewNodeNodes);
			}
			newParentLeftNode = parameterDiffViewNode;
			newParentRightNode = parameterDiffViewNode2;
		}

		internal DeviceObjectDiffViewNode GetNextDiff(DeviceObjectDiffViewNode currentNode)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (currentNode == null)
			{
				throw new ArgumentNullException("currentNode");
			}
			do
			{
				currentNode = GetNextNode(currentNode);
			}
			while (currentNode != null && Common.IsEqual(currentNode.DiffState));
			return currentNode;
		}

		private DeviceObjectDiffViewNode GetNextNode(DeviceObjectDiffViewNode currentNode)
		{
			Debug.Assert(currentNode != null);
			if (currentNode.ChildCount > 0)
			{
				return (DeviceObjectDiffViewNode)(object)currentNode.GetChild(0);
			}
			DeviceObjectDiffViewNode nextSiblingNode = GetNextSiblingNode(currentNode);
			if (nextSiblingNode != null)
			{
				return nextSiblingNode;
			}
			while (currentNode.Parent != null)
			{
				nextSiblingNode = GetNextSiblingNode((DeviceObjectDiffViewNode)(object)currentNode.Parent);
				if (nextSiblingNode != null)
				{
					return nextSiblingNode;
				}
				currentNode = (DeviceObjectDiffViewNode)(object)currentNode.Parent;
			}
			return null;
		}

		private DeviceObjectDiffViewNode GetNextSiblingNode(DeviceObjectDiffViewNode currentNode)
		{
			Debug.Assert(currentNode != null);
			int num = currentNode.IndexInParent + 1;
			if (currentNode.Parent == null)
			{
				if (num < ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount)
				{
					return ((DefaultTreeTableModel)_leftmodel).Sentinel.GetChild(num) as DeviceObjectDiffViewNode;
				}
				return null;
			}
			if (num < currentNode.Parent.ChildCount)
			{
				return currentNode.Parent.GetChild(num) as DeviceObjectDiffViewNode;
			}
			return null;
		}

		internal DeviceObjectDiffViewNode GetPreviousDiff(DeviceObjectDiffViewNode currentNode)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (currentNode == null)
			{
				throw new ArgumentNullException("currentNode");
			}
			do
			{
				currentNode = GetPreviousNode(currentNode);
			}
			while (currentNode != null && Common.IsEqual(currentNode.DiffState));
			return currentNode;
		}

		private DeviceObjectDiffViewNode GetPreviousNode(DeviceObjectDiffViewNode currentNode)
		{
			Debug.Assert(currentNode != null);
			DeviceObjectDiffViewNode previousSiblingNode = GetPreviousSiblingNode(currentNode);
			if (previousSiblingNode != null && previousSiblingNode.ChildCount > 0)
			{
				currentNode = (DeviceObjectDiffViewNode)(object)previousSiblingNode.GetChild(previousSiblingNode.ChildCount - 1);
				while (currentNode.ChildCount > 0)
				{
					currentNode = (DeviceObjectDiffViewNode)(object)currentNode.GetChild(currentNode.ChildCount - 1);
				}
				return currentNode;
			}
			if (previousSiblingNode != null)
			{
				return previousSiblingNode;
			}
			return (DeviceObjectDiffViewNode)(object)currentNode.Parent;
		}

		private DeviceObjectDiffViewNode GetPreviousSiblingNode(DeviceObjectDiffViewNode currentNode)
		{
			Debug.Assert(currentNode != null);
			int num = currentNode.IndexInParent - 1;
			if (currentNode.Parent == null)
			{
				if (num >= 0)
				{
					return (DeviceObjectDiffViewNode)(object)((DefaultTreeTableModel)_leftmodel).Sentinel.GetChild(num);
				}
				return null;
			}
			if (num >= 0)
			{
				return (DeviceObjectDiffViewNode)(object)currentNode.Parent.GetChild(num);
			}
			return null;
		}

		internal bool IsDirty()
		{
			for (int i = 0; i < ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount; i++)
			{
				DeviceObjectDiffViewNode deviceObjectDiffViewNode = ((DefaultTreeTableModel)_leftmodel).Sentinel.GetChild(i) as DeviceObjectDiffViewNode;
				if (deviceObjectDiffViewNode != null && deviceObjectDiffViewNode.IsDirty())
				{
					return true;
				}
			}
			return false;
		}

		internal int? GetAcceptionCount(DiffState diffState)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount; i++)
			{
				DeviceObjectDiffViewNode deviceObjectDiffViewNode = ((DefaultTreeTableModel)_leftmodel).Sentinel.GetChild(i) as DeviceObjectDiffViewNode;
				if (deviceObjectDiffViewNode != null && deviceObjectDiffViewNode.DiffState == diffState)
				{
					num += deviceObjectDiffViewNode.GetAcceptedCount(diffState);
				}
			}
			return num;
		}

		internal bool IsEverythingAccepted()
		{
			for (int i = 0; i < ((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount; i++)
			{
				DeviceObjectDiffViewNode deviceObjectDiffViewNode = ((DefaultTreeTableModel)_leftmodel).Sentinel.GetChild(i) as DeviceObjectDiffViewNode;
				if (deviceObjectDiffViewNode != null && !deviceObjectDiffViewNode.IsEverythingAccepted())
				{
					return false;
				}
			}
			return false;
		}

		internal void ApplyAcceptions()
		{
			if (((DefaultTreeTableModel)_leftmodel).Sentinel.ChildCount == 0)
			{
				return;
			}
			DeviceObjectDiffViewNode deviceObjectDiffViewNode = (DeviceObjectDiffViewNode)(object)((DefaultTreeTableModel)_leftmodel).Sentinel.GetChild(0);
			Debug.Assert(deviceObjectDiffViewNode != null);
			IMetaObject2 val = null;
			try
			{
				val = (IMetaObject2)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nLeftProjectHandle, _leftObjectGuid);
				if (val == null)
				{
					return;
				}
				LList<ParameterSet> val2 = new LList<ParameterSet>();
				if (val != null && ((IMetaObject)val).Object is IDeviceObject)
				{
					IObject @object = ((IMetaObject)val).Object;
					IDeviceObject val3 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
					val2.Add(val3.DeviceParameterSet as ParameterSet);
					foreach (IConnector item in (IEnumerable)val3.Connectors)
					{
						IConnector val4 = item;
						val2.Add(val4.HostParameterSet as ParameterSet);
					}
				}
				if (val != null && ((IMetaObject)val).Object is IExplicitConnector)
				{
					IObject object2 = ((IMetaObject)val).Object;
					IExplicitConnector val5 = (IExplicitConnector)(object)((object2 is IExplicitConnector) ? object2 : null);
					val2.Add(((IConnector)val5).HostParameterSet as ParameterSet);
				}
				do
				{
					if (Common.IsContentAccepted(deviceObjectDiffViewNode.AcceptState))
					{
						if (deviceObjectDiffViewNode is ParameterDiffViewNode)
						{
							StringBuilder stringBuilder = new StringBuilder();
							ParameterDiffViewNode parameterDiffViewNode = deviceObjectDiffViewNode as ParameterDiffViewNode;
							while (parameterDiffViewNode != null && parameterDiffViewNode != null && parameterDiffViewNode.RightElement != null)
							{
								stringBuilder.Insert(0, parameterDiffViewNode.RightElement.Identifier);
								parameterDiffViewNode = parameterDiffViewNode.Parent as ParameterDiffViewNode;
							}
							ApplyContentAcception(deviceObjectDiffViewNode as ParameterDiffViewNode, val2, stringBuilder.ToString());
						}
						if (deviceObjectDiffViewNode is FbInstancesDiffNode)
						{
							FbInstancesDiffNode fbInstancesDiffNode = deviceObjectDiffViewNode as FbInstancesDiffNode;
							LList<IFbInstance> val6 = new LList<IFbInstance>();
							if (val != null && ((IMetaObject)val).Object is IDeviceObject)
							{
								IObject object3 = ((IMetaObject)val).Object;
								IDeviceObject val7 = (IDeviceObject)(object)((object3 is IDeviceObject) ? object3 : null);
								FillFbInstances(((IDeviceObject2)((val7 is IDeviceObject2) ? val7 : null)).DriverInfo, val6);
								foreach (IConnector item2 in (IEnumerable)val7.Connectors)
								{
									IConnector val8 = item2;
									if (CheckConnector(val7.Connectors, val8, ((IMetaObject)val).ParentObjectGuid))
									{
										FillFbInstances(((IConnector2)((val8 is IConnector2) ? val8 : null)).DriverInfo, val6);
									}
								}
							}
							if (val != null && ((IMetaObject)val).Object is IExplicitConnector)
							{
								IObject object4 = ((IMetaObject)val).Object;
								IExplicitConnector val9 = (IExplicitConnector)(object)((object4 is IExplicitConnector) ? object4 : null);
								FillFbInstances(((IConnector2)((val9 is IConnector2) ? val9 : null)).DriverInfo, val6);
							}
							foreach (IFbInstance item3 in val6)
							{
								if (item3.Instance.Variable == fbInstancesDiffNode.LeftInstance.Instance.Variable)
								{
									item3.Instance.Variable=(fbInstancesDiffNode.RightInstance.Instance.Variable);
								}
							}
						}
						if (deviceObjectDiffViewNode is BusCycleDiffNode)
						{
							BusCycleDiffNode busCycleDiffNode = deviceObjectDiffViewNode as BusCycleDiffNode;
							if (val != null && ((IMetaObject)val).Object is IDeviceObject2)
							{
								IObject object5 = ((IMetaObject)val).Object;
								IDeviceObject2 val10 = (IDeviceObject2)(object)((object5 is IDeviceObject2) ? object5 : null);
								IDriverInfo driverInfo = val10.DriverInfo;
								if (((driverInfo is IDriverInfo5) ? driverInfo : null).BusCycleTask == ((IDriverInfo)busCycleDiffNode.LeftInstance).BusCycleTask || (val10.DriverInfo as IDriverInfo5).BusCycleTaskGuid == busCycleDiffNode.LeftInstance.BusCycleTaskGuid)
								{
									IDriverInfo driverInfo2 = val10.DriverInfo;
									((driverInfo2 is IDriverInfo5) ? driverInfo2 : null).BusCycleTask=(((IDriverInfo)busCycleDiffNode.RightInstance).BusCycleTask);
									IDriverInfo driverInfo3 = val10.DriverInfo;
									((IDriverInfo5)((driverInfo3 is IDriverInfo5) ? driverInfo3 : null)).BusCycleTaskGuid=(busCycleDiffNode.RightInstance.BusCycleTaskGuid);
								}
								foreach (IConnector item4 in (IEnumerable)((IDeviceObject)val10).Connectors)
								{
									IConnector val11 = item4;
									IDriverInfo driverInfo4 = ((IConnector2)((val11 is IConnector2) ? val11 : null)).DriverInfo;
									if (((driverInfo4 is IDriverInfo5) ? driverInfo4 : null).BusCycleTask == ((IDriverInfo)busCycleDiffNode.LeftInstance).BusCycleTask || ((val11 as IConnector2).DriverInfo as IDriverInfo5).BusCycleTaskGuid == busCycleDiffNode.LeftInstance.BusCycleTaskGuid)
									{
										IDriverInfo driverInfo5 = ((IConnector2)((val11 is IConnector2) ? val11 : null)).DriverInfo;
										((driverInfo5 is IDriverInfo5) ? driverInfo5 : null).BusCycleTask=(((IDriverInfo)busCycleDiffNode.RightInstance).BusCycleTask);
										IDriverInfo driverInfo6 = ((IConnector2)((val11 is IConnector2) ? val11 : null)).DriverInfo;
										((IDriverInfo5)((driverInfo6 is IDriverInfo5) ? driverInfo6 : null)).BusCycleTaskGuid=(busCycleDiffNode.RightInstance.BusCycleTaskGuid);
									}
								}
							}
							if (val != null && ((IMetaObject)val).Object is IExplicitConnector)
							{
								IObject object6 = ((IMetaObject)val).Object;
								IObject obj = ((object6 is IExplicitConnector) ? object6 : null);
								IDriverInfo driverInfo7 = ((IConnector2)((obj is IConnector2) ? obj : null)).DriverInfo;
								((driverInfo7 is IDriverInfo5) ? driverInfo7 : null).BusCycleTask=(((IDriverInfo)busCycleDiffNode.RightInstance).BusCycleTask);
								IDriverInfo driverInfo8 = ((IConnector2)((obj is IConnector2) ? obj : null)).DriverInfo;
								((IDriverInfo5)((driverInfo8 is IDriverInfo5) ? driverInfo8 : null)).BusCycleTaskGuid=(busCycleDiffNode.RightInstance.BusCycleTaskGuid);
							}
						}
						if (deviceObjectDiffViewNode is AlwaysMappingDiffNode)
						{
							AlwaysMappingDiffNode alwaysMappingDiffNode = deviceObjectDiffViewNode as AlwaysMappingDiffNode;
							if (val != null && ((IMetaObject)val).Object is IDeviceObject2)
							{
								IObject object7 = ((IMetaObject)val).Object;
								IDeviceObject2 val12 = (IDeviceObject2)(object)((object7 is IDeviceObject2) ? object7 : null);
								IDriverInfo driverInfo9 = val12.DriverInfo;
								if (((IDriverInfo6)((driverInfo9 is IDriverInfo11) ? driverInfo9 : null)).PlcAlwaysMapping && (val12.DriverInfo as IDriverInfo11).PlcAlwaysMappingMode == alwaysMappingDiffNode.LeftInstance)
								{
									IDriverInfo driverInfo10 = val12.DriverInfo;
									((IDriverInfo6)((driverInfo10 is IDriverInfo11) ? driverInfo10 : null)).PlcAlwaysMapping=(alwaysMappingDiffNode.RightInstance.HasValue);
									if (alwaysMappingDiffNode.RightInstance.HasValue)
									{
										IDriverInfo driverInfo11 = val12.DriverInfo;
										((IDriverInfo11)((driverInfo11 is IDriverInfo11) ? driverInfo11 : null)).PlcAlwaysMappingMode=(alwaysMappingDiffNode.RightInstance.Value);
									}
								}
								IParameterSet deviceParameterSet = ((IDeviceObject)val12).DeviceParameterSet;
								if (((IParameterSet4)((deviceParameterSet is IParameterSet5) ? deviceParameterSet : null)).AlwaysMapping && (val12.DeviceParameterSet as IParameterSet5).AlwaysMappingMode == alwaysMappingDiffNode.LeftInstance)
								{
									IParameterSet deviceParameterSet2 = ((IDeviceObject)val12).DeviceParameterSet;
									((IParameterSet4)((deviceParameterSet2 is IParameterSet5) ? deviceParameterSet2 : null)).AlwaysMapping=(alwaysMappingDiffNode.RightInstance.HasValue);
									if (alwaysMappingDiffNode.RightInstance.HasValue)
									{
										IParameterSet deviceParameterSet3 = ((IDeviceObject)val12).DeviceParameterSet;
										((IParameterSet5)((deviceParameterSet3 is IParameterSet5) ? deviceParameterSet3 : null)).AlwaysMappingMode=(alwaysMappingDiffNode.RightInstance.Value);
									}
								}
								foreach (IConnector item5 in (IEnumerable)((IDeviceObject)val12).Connectors)
								{
									IConnector val13 = item5;
									if (((IConnector6)((val13 is IConnector11) ? val13 : null)).AlwaysMapping && (AlwaysMappingMode?)((IConnector11)((val13 is IConnector11) ? val13 : null)).AlwaysMappingMode == alwaysMappingDiffNode.LeftInstance)
									{
										((IConnector6)((val13 is IConnector11) ? val13 : null)).AlwaysMapping=(alwaysMappingDiffNode.RightInstance.HasValue);
										if (alwaysMappingDiffNode.RightInstance.HasValue)
										{
											((IConnector11)((val13 is IConnector11) ? val13 : null)).AlwaysMappingMode=(alwaysMappingDiffNode.RightInstance.Value);
										}
									}
								}
							}
							if (val != null && ((IMetaObject)val).Object is IExplicitConnector)
							{
								IObject object8 = ((IMetaObject)val).Object;
								IExplicitConnector val14 = (IExplicitConnector)(object)((object8 is IExplicitConnector) ? object8 : null);
								if (((IConnector6)((val14 is IConnector11) ? val14 : null)).AlwaysMapping)
								{
									((IConnector6)((val14 is IConnector11) ? val14 : null)).AlwaysMapping=(alwaysMappingDiffNode.RightInstance.HasValue);
									if (alwaysMappingDiffNode.RightInstance.HasValue)
									{
										((IConnector11)((val14 is IConnector11) ? val14 : null)).AlwaysMappingMode=(alwaysMappingDiffNode.RightInstance.Value);
									}
								}
							}
						}
					}
					deviceObjectDiffViewNode = GetNextNode(deviceObjectDiffViewNode);
				}
				while (deviceObjectDiffViewNode != null);
			}
			catch
			{
			}
			finally
			{
				if (val != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject((IMetaObject)(object)val, true, (object)this);
				}
			}
		}

		private void ApplyMapping(ParameterSet paramset, IDataElement dataelement, ParameterDiffViewNode node)
		{
			IStringTable stringTable = paramset.StringTable;
			if (stringTable != null && node.RightElement != null)
			{
				IStringRef description = stringTable.CreateStringRef("", "", node.RightElement.Description);
				if (dataelement is IDataElement4)
				{
					((IDataElement4)((dataelement is IDataElement4) ? dataelement : null)).SetDescription(description);
				}
			}
			IVariableMappingCollection variableMappings = dataelement.IoMapping.VariableMappings;
			IVariableMappingCollection val = null;
			val = ((!(dataelement is IParameter)) ? node.RightElement.IoMapping.VariableMappings : ((IDataElement)node.RightParameter).IoMapping.VariableMappings);
			if (dataelement.IoMapping.AutomaticIecAddress != node.RightElement.IoMapping.AutomaticIecAddress || (dataelement.IoMapping.IecAddress != node.RightElement.IoMapping.IecAddress && !node.RightElement.IoMapping.AutomaticIecAddress && !dataelement.IoMapping.AutomaticIecAddress))
			{
				dataelement.IoMapping.AutomaticIecAddress=(((IDataElement)node.RightParameter).IoMapping.AutomaticIecAddress);
				if (!dataelement.IoMapping.AutomaticIecAddress)
				{
					dataelement.IoMapping.IecAddress=(((IDataElement)node.RightParameter).IoMapping.IecAddress);
				}
			}
			string text = string.Empty;
			if (val != null && ((ICollection)val).Count > 0)
			{
				text = val[0].Variable;
				_=val[0].CreateVariable;
			}
			if (variableMappings == null)
			{
				return;
			}
			if (text != string.Empty)
			{
				if (((ICollection)variableMappings).Count == 0)
				{
					variableMappings.AddMapping(text, true);
				}
				else
				{
					variableMappings[0].Variable=(text);
				}
			}
			else if (((ICollection)variableMappings).Count > 0)
			{
				variableMappings.RemoveAt(0);
			}
		}

		private void ApplyDataElement(ParameterSet paramset, IDataElement dataelement, ParameterDiffViewNode node, ChannelType channeltype, string stCurentPath, string stNodePath)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			if (dataelement.HasSubElements)
			{
				foreach (IDataElement item in (IEnumerable)dataelement.SubElements)
				{
					IDataElement val = item;
					ApplyDataElement(paramset, val, node, channeltype, stCurentPath + val.Identifier, stNodePath);
				}
			}
			if (!(stCurentPath == stNodePath))
			{
				return;
			}
			if ((int)channeltype == 0)
			{
				if (((IDataElement2)((dataelement is IDataElement2) ? dataelement : null)).HasBaseType)
				{
					dataelement.Value=(node.RightElement.Value);
				}
			}
			else
			{
				ApplyMapping(paramset, dataelement, node);
			}
		}

		private void ApplyContentAcception(ParameterDiffViewNode node, LList<ParameterSet> leftParamSet, string stPath)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (node.LeftParameter == null)
			{
				return;
			}
			foreach (ParameterSet item in leftParamSet)
			{
				if (item.ConnectorId != node.ConnectionId)
				{
					continue;
				}
				foreach (Parameter item2 in item)
				{
					if (item2.Identifier == ((IDataElement)node.LeftParameter).Identifier)
					{
						ApplyDataElement(item, (IDataElement)(object)item2, node, item2.ChannelType, item2.Identifier, stPath);
					}
				}
			}
		}
	}
}
