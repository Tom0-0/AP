using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.ProjectCompare;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{8DDC66B3-8270-4346-9DFE-A0381C1F176C}")]
	public class DeviceObjectDiffView : UserControl, IEmbeddedDiffViewer2, IEmbeddedDiffViewer
	{
		private string _stLeftLabel;

		private string _stRightLabel;

		private DeviceObjectDiffViewModel _leftmodel;

		private DeviceObjectDiffViewModel _rightmodel;

		private DoubleDiffView _doubleDiffView;

		private TreeTableViewNode _viewRightTopNode;

		private TreeTableViewNode _viewLeftTopNode;

		private IContainer components;

		private TreeTableView _diffView;

		private TreeTableView _diffViewRight;

		private SplitContainer splitContainer1;

		public Control EmbeddedControl => this;

		public Control[] Panes => null;

		public bool CanNextDiff
		{
			get
			{
				DeviceObjectDiffViewNode focusedNode = FocusedNode;
				if (focusedNode != null)
				{
					return _doubleDiffView.GetNextDiff(focusedNode) != null;
				}
				return false;
			}
		}

		public bool CanPreviousDiff
		{
			get
			{
				DeviceObjectDiffViewNode focusedNode = FocusedNode;
				if (focusedNode != null)
				{
					return _doubleDiffView.GetPreviousDiff(focusedNode) != null;
				}
				return false;
			}
		}

		public bool CanAcceptBlock => false;

		public bool CanAcceptSingle
		{
			get
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Invalid comparison between Unknown and I4
				LList<DeviceObjectDiffViewNode> selectedNodes = SelectedNodes;
				if (selectedNodes != null && selectedNodes.Count > 0)
				{
					foreach (DeviceObjectDiffViewNode item in selectedNodes)
					{
						if (item != null && (int)item.DiffState == 4)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool CanAcceptProperties => false;

		public bool CanOpposeChanges => false;

		public string LeftLabel => _stLeftLabel;

		public string RightLabel => _stRightLabel;

		public bool CanHandleIgnoreWhitespace => false;

		public bool CanHandleIgnoreComments => false;

		public bool CanHandleIgnoreProperties => false;

		public int AdditionsCount => _doubleDiffView.AdditionsCount;

		public int DeletionsCount => _doubleDiffView.DeletionsCount;

		public int ChangesCount => _doubleDiffView.ChangesCount;

		private LList<DeviceObjectDiffViewNode> SelectedNodes
		{
			get
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Expected O, but got Unknown
				LList<DeviceObjectDiffViewNode> val = new LList<DeviceObjectDiffViewNode>();
				if (((TreeTableViewNodeCollection)_diffView.SelectedNodes).Count > 0)
				{
					foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)_diffView.SelectedNodes)
					{
						TreeTableViewNode val2 = item;
						ITreeTableNode modelNode = _diffView.GetModelNode(val2);
						if (modelNode is DeviceObjectDiffViewNode)
						{
							val.Add(modelNode as DeviceObjectDiffViewNode);
						}
					}
					return val;
				}
				return val;
			}
		}

		private DeviceObjectDiffViewNode FocusedNode
		{
			get
			{
				if (_diffView.FocusedNode != null)
				{
					return _diffView.GetModelNode(_diffView.FocusedNode) as DeviceObjectDiffViewNode;
				}
				return null;
			}
		}

		private DeviceObjectDiffViewNode SingleSelectedNode
		{
			get
			{
				if (((TreeTableViewNodeCollection)_diffView.SelectedNodes).Count == 1)
				{
					return _diffView.GetModelNode(((TreeTableViewNodeCollection)_diffView.SelectedNodes)[0]) as DeviceObjectDiffViewNode;
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					TreeTableViewNode viewNode = _diffView.GetViewNode((ITreeTableNode)(object)value);
					if (viewNode != null)
					{
						((Control)(object)_diffView).Focus();
						_diffView.DeselectAll();
						viewNode.Selected=(true);
						viewNode.Focus(0);
						viewNode.EnsureVisible(0);
					}
				}
			}
		}

		public AcceptState? CurrentAcceptState => SingleSelectedNode?.AcceptState;

		public DiffState? CurrentDiffState => SingleSelectedNode?.DiffState;

		public DiffDisplayMode CurrentDisplayMode
		{
			get
			{
				if (!splitContainer1.Panel2Collapsed)
				{
					return (DiffDisplayMode)1;
				}
				return (DiffDisplayMode)2;
			}
		}

		public DiffDisplayMode SupportedDisplayModes => (DiffDisplayMode)3;

		public DeviceObjectDiffView()
		{
			InitializeComponent();
		}

		public void Initialize(int nLeftProjectHandle, Guid leftObjectGuid, int nRightProjectHandle, Guid rightObjectGuid, bool bIgnoreWhitespace, bool bIgnoreComments, bool bIgnoreProperties, string stLeftLabel, string stRightLabel)
		{
			_stLeftLabel = stLeftLabel;
			_stRightLabel = stRightLabel;
			_leftmodel = new DeviceObjectDiffViewModel(bIsLeftModel: true);
			_rightmodel = new DeviceObjectDiffViewModel(bIsLeftModel: false);
			_doubleDiffView = new DoubleDiffView(nLeftProjectHandle, leftObjectGuid, nRightProjectHandle, rightObjectGuid, _leftmodel, _rightmodel);
			_diffView.Model=((ITreeTableModel)(object)_leftmodel);
			_diffViewRight.Model=((ITreeTableModel)(object)_rightmodel);
		}

		public void UpdateContents(bool bOpposeChanges)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_diffView.BeginUpdate();
				_diffViewRight.BeginUpdate();
				_doubleDiffView.UpdateContents();
			}
			catch
			{
			}
			finally
			{
				_diffView.EndUpdate();
				_diffViewRight.EndUpdate();
			}
			if (_diffView.GetNodeCount(false) > 0)
			{
				TreeTableViewNode val = _diffView.Nodes[0];
				for (DeviceObjectDiffViewNode deviceObjectDiffViewNode = _diffView.GetModelNode(val) as DeviceObjectDiffViewNode; deviceObjectDiffViewNode != null; deviceObjectDiffViewNode = _doubleDiffView.GetNextDiff(deviceObjectDiffViewNode))
				{
					if ((int)deviceObjectDiffViewNode.DiffState != 0)
					{
						ExpandDiffNodes(deviceObjectDiffViewNode);
					}
				}
			}
			for (int i = 0; i < _diffView.Columns.Count; i++)
			{
				_diffView.AdjustColumnWidth(i, true);
				if (_diffView.Columns[i].Width > 250)
				{
					_diffView.Columns[i].Width = 250;
				}
			}
			for (int j = 0; j < _diffViewRight.Columns.Count; j++)
			{
				_diffViewRight.AdjustColumnWidth(j, true);
				if (_diffViewRight.Columns[j].Width > 250)
				{
					_diffViewRight.Columns[j].Width = 250;
				}
			}
		}

		internal void ExpandDiffNodes(DeviceObjectDiffViewNode node)
		{
			if (node != null)
			{
				if (node.Parent != null)
				{
					ExpandDiffNodes(node.Parent as DeviceObjectDiffViewNode);
				}
				_diffView.GetViewNode((ITreeTableNode)(object)node).Expand();
				DeviceObjectDiffViewNode deviceObjectDiffViewNode = _doubleDiffView.LeftNodes[node];
				if (deviceObjectDiffViewNode != null)
				{
					_diffViewRight.GetViewNode((ITreeTableNode)(object)deviceObjectDiffViewNode).Expand();
				}
			}
		}

		public void Finish(bool bCommit)
		{
			if (bCommit && IsDirty())
			{
				_doubleDiffView.ApplyAcceptions();
			}
		}

		public void NextDiff()
		{
			SingleSelectedNode = _doubleDiffView.GetNextDiff(FocusedNode);
		}

		public void PreviousDiff()
		{
			SingleSelectedNode = _doubleDiffView.GetPreviousDiff(FocusedNode);
		}

		public void AcceptBlock()
		{
		}

		public void AcceptSingle()
		{
			SetAcceptSingle(null, (TreeTableViewNodeCollection)(object)_diffView.SelectedNodes);
		}

		private void SetAcceptSingle(AcceptState? newState, TreeTableViewNodeCollection nodes)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			_diffView.BeginUpdate();
			_diffViewRight.BeginUpdate();
			try
			{
				foreach (TreeTableViewNode node in nodes)
				{
					TreeTableViewNode val = node;
					ITreeTableNode modelNode = _diffView.GetModelNode(val);
					CheckNode(modelNode, newState);
				}
			}
			finally
			{
				_diffView.EndUpdate();
				_diffViewRight.EndUpdate();
			}
		}

		internal void CheckNode(ITreeTableNode node, AcceptState? newState)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Invalid comparison between Unknown and I4
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Invalid comparison between Unknown and I4
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Invalid comparison between Unknown and I4
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Invalid comparison between Unknown and I4
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			bool flag = !newState.HasValue;
			if (node is FbInstancesDiffNode)
			{
				FbInstancesDiffNode fbInstancesDiffNode = node as FbInstancesDiffNode;
				if (!newState.HasValue)
				{
					newState = (AcceptState)(((int)fbInstancesDiffNode.AcceptState != 1) ? 1 : 0);
				}
				bool flag2 = false;
				if (((Enum)newState.Value).HasFlag((Enum)(object)(AcceptState)1) && fbInstancesDiffNode.RightInstance != null)
				{
					if (fbInstancesDiffNode.LeftInstance.Instance.Variable != fbInstancesDiffNode.RightInstance.Instance.Variable)
					{
						fbInstancesDiffNode.InstanceVariable = fbInstancesDiffNode.RightInstance.Instance.Variable;
						fbInstancesDiffNode.DiffVariable = true;
						flag2 = true;
					}
					if (fbInstancesDiffNode.LeftInstance.FbName != fbInstancesDiffNode.RightInstance.FbName)
					{
						IFbInstance leftInstance = fbInstancesDiffNode.LeftInstance;
						((IFbInstance2)((leftInstance is IFbInstance3) ? leftInstance : null)).SetFbName(fbInstancesDiffNode.RightInstance.FbName);
						fbInstancesDiffNode.DiffType = true;
						flag2 = true;
					}
				}
				if (flag2 || (int)newState.Value == 0)
				{
					fbInstancesDiffNode.AcceptState = newState.Value;
				}
				if (_doubleDiffView.LeftNodes.ContainsKey((DeviceObjectDiffViewNode)fbInstancesDiffNode))
				{
					DeviceObjectDiffViewNode deviceObjectDiffViewNode = _doubleDiffView.LeftNodes[(DeviceObjectDiffViewNode)fbInstancesDiffNode];
					if (deviceObjectDiffViewNode != null)
					{
						deviceObjectDiffViewNode.AcceptState = newState.Value;
					}
				}
			}
			if (node is BusCycleDiffNode)
			{
				BusCycleDiffNode busCycleDiffNode = node as BusCycleDiffNode;
				if (!newState.HasValue)
				{
					newState = (AcceptState)(((int)busCycleDiffNode.AcceptState != 1) ? 1 : 0);
				}
				bool flag3 = false;
				if (((Enum)newState.Value).HasFlag((Enum)(object)(AcceptState)1) && busCycleDiffNode.RightInstance != null && (((IDriverInfo)busCycleDiffNode.LeftInstance).BusCycleTask != ((IDriverInfo)busCycleDiffNode.RightInstance).BusCycleTask || busCycleDiffNode.LeftInstance.BusCycleTaskGuid != busCycleDiffNode.RightInstance.BusCycleTaskGuid))
				{
					busCycleDiffNode.BusCycleTask = ((IDriverInfo)busCycleDiffNode.RightInstance).BusCycleTask;
					busCycleDiffNode.BusCycleTaskGuid = busCycleDiffNode.RightInstance.BusCycleTaskGuid;
					flag3 = true;
				}
				if (flag3 || (int)newState.Value == 0)
				{
					busCycleDiffNode.AcceptState = newState.Value;
				}
				if (_doubleDiffView.LeftNodes.ContainsKey((DeviceObjectDiffViewNode)busCycleDiffNode))
				{
					DeviceObjectDiffViewNode deviceObjectDiffViewNode2 = _doubleDiffView.LeftNodes[(DeviceObjectDiffViewNode)busCycleDiffNode];
					if (deviceObjectDiffViewNode2 != null)
					{
						deviceObjectDiffViewNode2.AcceptState = newState.Value;
					}
				}
			}
			if (node is AlwaysMappingDiffNode)
			{
				AlwaysMappingDiffNode alwaysMappingDiffNode = node as AlwaysMappingDiffNode;
				if (!newState.HasValue)
				{
					newState = (AcceptState)(((int)alwaysMappingDiffNode.AcceptState != 1) ? 1 : 0);
				}
				bool flag4 = false;
				if (((Enum)newState.Value).HasFlag((Enum)(object)(AcceptState)1) && alwaysMappingDiffNode.RightInstance.HasValue && alwaysMappingDiffNode.LeftInstance != alwaysMappingDiffNode.RightInstance)
				{
					alwaysMappingDiffNode.AlwaysMappingMode = alwaysMappingDiffNode.RightInstance;
					flag4 = true;
				}
				if (flag4 || (int)newState.Value == 0)
				{
					alwaysMappingDiffNode.AcceptState = newState.Value;
				}
				if (_doubleDiffView.LeftNodes.ContainsKey((DeviceObjectDiffViewNode)alwaysMappingDiffNode))
				{
					DeviceObjectDiffViewNode deviceObjectDiffViewNode3 = _doubleDiffView.LeftNodes[(DeviceObjectDiffViewNode)alwaysMappingDiffNode];
					if (deviceObjectDiffViewNode3 != null)
					{
						deviceObjectDiffViewNode3.AcceptState = newState.Value;
					}
				}
			}
			if (node is ParameterDiffViewNode)
			{
				ParameterDiffViewNode parameterDiffViewNode = node as ParameterDiffViewNode;
				if (!newState.HasValue)
				{
					newState = (AcceptState)(((int)parameterDiffViewNode.AcceptState != 1) ? 1 : 0);
				}
				bool flag5 = false;
				if (((Enum)newState.Value).HasFlag((Enum)(object)(AcceptState)1))
				{
					if (parameterDiffViewNode.RightElement != null && parameterDiffViewNode.RightElement != null)
					{
						IDataElement rightElement = parameterDiffViewNode.RightElement;
						if (rightElement.IoMapping != null && rightElement.IoMapping.VariableMappings != null && ((ICollection)rightElement.IoMapping.VariableMappings).Count > 0)
						{
							IVariableMappingCollection variableMappings = rightElement.IoMapping.VariableMappings;
							IVariableMappingCollection variableMappings2 = parameterDiffViewNode.LeftElement.IoMapping.VariableMappings;
							parameterDiffViewNode.LeftMapping = variableMappings[0].Variable;
							if (((ICollection)variableMappings2).Count == 0 || variableMappings2[0].Variable != variableMappings[0].Variable)
							{
								flag5 = true;
							}
						}
						else if (parameterDiffViewNode.LeftElement != null && ((ICollection)parameterDiffViewNode.LeftElement.IoMapping.VariableMappings).Count > 0)
						{
							flag5 = true;
						}
						if (parameterDiffViewNode.LeftElement != null && parameterDiffViewNode.LeftElement.IoMapping != null && (int)parameterDiffViewNode.LeftParameter.ChannelType != 0 && rightElement.IoMapping != null && ((rightElement.IoMapping.IecAddress != parameterDiffViewNode.LeftElement.IoMapping.IecAddress && !rightElement.IoMapping.AutomaticIecAddress && !parameterDiffViewNode.LeftElement.IoMapping.AutomaticIecAddress) || rightElement.IoMapping.AutomaticIecAddress != parameterDiffViewNode.LeftElement.IoMapping.AutomaticIecAddress))
						{
							parameterDiffViewNode.LeftAddress = rightElement.IoMapping.IecAddress;
							flag5 = true;
						}
						if (parameterDiffViewNode.LeftElement != null && (int)parameterDiffViewNode.LeftParameter.ChannelType != 0)
						{
							if (rightElement.Description != parameterDiffViewNode.LeftElement.Description)
							{
								parameterDiffViewNode.LeftDescription = parameterDiffViewNode.RightElement.Description;
								flag5 = true;
							}
							if (string.IsNullOrEmpty(parameterDiffViewNode.LeftDescription))
							{
								parameterDiffViewNode.LeftDescription = parameterDiffViewNode.RightElement.Description;
							}
						}
					}
					if (parameterDiffViewNode.LeftParameter != null && parameterDiffViewNode.LeftParameter != null && (int)parameterDiffViewNode.LeftParameter.ChannelType == 0 && parameterDiffViewNode.RightElement != null && parameterDiffViewNode.LeftElement != null && parameterDiffViewNode.LeftElement.Value != parameterDiffViewNode.RightElement.Value)
					{
						parameterDiffViewNode.LeftValue = parameterDiffViewNode.RightElement.Value;
						flag5 = true;
					}
				}
				if (flag5 || (int)newState.Value == 0)
				{
					parameterDiffViewNode.AcceptState = newState.Value;
					if (_doubleDiffView.LeftNodes.ContainsKey((DeviceObjectDiffViewNode)parameterDiffViewNode))
					{
						ParameterDiffViewNode parameterDiffViewNode2 = (ParameterDiffViewNode)_doubleDiffView.LeftNodes[(DeviceObjectDiffViewNode)parameterDiffViewNode];
						if (parameterDiffViewNode2 != null)
						{
							parameterDiffViewNode2.AcceptState = newState.Value;
						}
					}
				}
			}
			if (node.HasChildren && !flag)
			{
				for (int i = 0; i < node.ChildCount; i++)
				{
					ITreeTableNode child = node.GetChild(i);
					CheckNode(child, newState);
				}
			}
		}

		public void AcceptProperties()
		{
		}

		public bool IsDirty()
		{
			return _doubleDiffView.IsDirty();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
		}

		private void _diffView_Scroll(object sender, EventArgs e)
		{
			if (sender == _diffView)
			{
				TreeTableViewNode topNode = _diffView.TopNode;
				if (_viewLeftTopNode != topNode)
				{
					_viewLeftTopNode = topNode;
					if (topNode != null)
					{
						DeviceObjectDiffViewNode deviceObjectDiffViewNode = _diffView.GetModelNode(topNode) as DeviceObjectDiffViewNode;
						DeviceObjectDiffViewNode deviceObjectDiffViewNode2 = default(DeviceObjectDiffViewNode);
						if (deviceObjectDiffViewNode != null && _doubleDiffView.LeftNodes.TryGetValue(deviceObjectDiffViewNode, out deviceObjectDiffViewNode2))
						{
							TreeTableViewNode viewNode = _diffViewRight.GetViewNode((ITreeTableNode)(object)deviceObjectDiffViewNode2);
							if (viewNode != null && _diffViewRight.TopNode != viewNode)
							{
								_diffViewRight.TopNode=(viewNode);
							}
						}
					}
				}
			}
			if (sender != _diffViewRight)
			{
				return;
			}
			TreeTableViewNode topNode2 = _diffViewRight.TopNode;
			if (_viewRightTopNode == topNode2)
			{
				return;
			}
			_viewRightTopNode = topNode2;
			if (topNode2 == null)
			{
				return;
			}
			DeviceObjectDiffViewNode deviceObjectDiffViewNode3 = _diffViewRight.GetModelNode(topNode2) as DeviceObjectDiffViewNode;
			DeviceObjectDiffViewNode deviceObjectDiffViewNode4 = default(DeviceObjectDiffViewNode);
			if (deviceObjectDiffViewNode3 != null && _doubleDiffView.RightNodes.TryGetValue(deviceObjectDiffViewNode3, out deviceObjectDiffViewNode4))
			{
				TreeTableViewNode viewNode2 = _diffView.GetViewNode((ITreeTableNode)(object)deviceObjectDiffViewNode4);
				if (viewNode2 != null && _diffView.TopNode != viewNode2)
				{
					_diffView.TopNode=(viewNode2);
				}
			}
		}

		private void _diffView_AfterCollapse(object sender, TreeTableViewEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}
			DeviceObjectDiffViewNode deviceObjectDiffViewNode = _diffView.GetModelNode(e.Node) as DeviceObjectDiffViewNode;
			if (deviceObjectDiffViewNode != null && _doubleDiffView.LeftNodes.ContainsKey(deviceObjectDiffViewNode))
			{
				TreeTableViewNode viewNode = _diffViewRight.GetViewNode((ITreeTableNode)(object)_doubleDiffView.LeftNodes[deviceObjectDiffViewNode]);
				if (viewNode != null)
				{
					viewNode.Collapse();
				}
			}
			deviceObjectDiffViewNode = _diffViewRight.GetModelNode(e.Node) as DeviceObjectDiffViewNode;
			if (deviceObjectDiffViewNode != null && _doubleDiffView.RightNodes.ContainsKey(deviceObjectDiffViewNode))
			{
				TreeTableViewNode viewNode2 = _diffView.GetViewNode((ITreeTableNode)(object)_doubleDiffView.RightNodes[deviceObjectDiffViewNode]);
				if (viewNode2 != null)
				{
					viewNode2.Collapse();
				}
			}
		}

		private void _diffView_AfterExpand(object sender, TreeTableViewEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}
			DeviceObjectDiffViewNode deviceObjectDiffViewNode = _diffView.GetModelNode(e.Node) as DeviceObjectDiffViewNode;
			if (deviceObjectDiffViewNode != null && _doubleDiffView.LeftNodes.ContainsKey(deviceObjectDiffViewNode))
			{
				TreeTableViewNode viewNode = _diffViewRight.GetViewNode((ITreeTableNode)(object)_doubleDiffView.LeftNodes[deviceObjectDiffViewNode]);
				if (viewNode != null)
				{
					viewNode.Expand();
				}
			}
			deviceObjectDiffViewNode = _diffViewRight.GetModelNode(e.Node) as DeviceObjectDiffViewNode;
			if (deviceObjectDiffViewNode != null && _doubleDiffView.RightNodes.ContainsKey(deviceObjectDiffViewNode))
			{
				TreeTableViewNode viewNode2 = _diffView.GetViewNode((ITreeTableNode)(object)_doubleDiffView.RightNodes[deviceObjectDiffViewNode]);
				if (viewNode2 != null)
				{
					viewNode2.Expand();
				}
			}
		}

		public void SetAllAcceptStates(AcceptState acceptState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if ((int)acceptState != 0 && (int)acceptState != 1)
			{
				throw new ArgumentException("acceptState");
			}
			SetAcceptSingle(acceptState, _diffView.Nodes);
		}

		public bool IsEverythingAccepted()
		{
			return _doubleDiffView.IsEverythingAccepted();
		}

		public void SetBlockAcceptState(AcceptState state)
		{
		}

		public void SetSingleAcceptState(AcceptState state)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if ((int)state != 0 && (int)state != 1)
			{
				throw new ArgumentException("acceptState");
			}
			SetAcceptSingle(state, (TreeTableViewNodeCollection)(object)_diffView.SelectedNodes);
		}

		public void SetDisplayMode(DiffDisplayMode mode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			splitContainer1.Panel2Collapsed = ((Enum)mode).HasFlag((Enum)(object)(DiffDisplayMode)2);
		}

		public int? GetAcceptionCount(DiffState state)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _doubleDiffView.GetAcceptionCount(state);
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
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Expected O, but got Unknown
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Expected O, but got Unknown
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Expected O, but got Unknown
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Expected O, but got Unknown
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.DeviceObjectDiffView));
			_diffView = new TreeTableView();
			_diffViewRight = new TreeTableView();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			SuspendLayout();
			((System.Windows.Forms.Control)(object)_diffView).BackColor = System.Drawing.SystemColors.Window;
			_diffView.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			componentResourceManager.ApplyResources(_diffView, "_diffView");
			_diffView.DoNotShrinkColumnsAutomatically=(false);
			_diffView.ForceFocusOnClick=(false);
			_diffView.GridLines=(false);
			_diffView.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Clickable);
			_diffView.HideSelection=(true);
			_diffView.ImmediateEdit=(false);
			_diffView.Indent=(20);
			_diffView.KeepColumnWidthsAdjusted=(false);
			_diffView.Model=((ITreeTableModel)null);
			_diffView.MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_diffView).Name = "_diffView";
			_diffView.NoSearchStrings=(false);
			_diffView.OpenEditOnDblClk=(false);
			_diffView.ReadOnly=(false);
			_diffView.Scrollable=(true);
			_diffView.ShowLines=(true);
			_diffView.ShowPlusMinus=(true);
			_diffView.ShowRootLines=(true);
			_diffView.ToggleOnDblClk=(false);
			_diffView.AfterCollapse+=(new TreeTableViewEventHandler(_diffView_AfterCollapse));
			_diffView.AfterExpand+=(new TreeTableViewEventHandler(_diffView_AfterExpand));
			_diffView.Scroll+=(new System.EventHandler(_diffView_Scroll));
			((System.Windows.Forms.Control)(object)_diffViewRight).BackColor = System.Drawing.SystemColors.Window;
			_diffViewRight.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			componentResourceManager.ApplyResources(_diffViewRight, "_diffViewRight");
			_diffViewRight.DoNotShrinkColumnsAutomatically=(false);
			_diffViewRight.ForceFocusOnClick=(false);
			_diffViewRight.GridLines=(false);
			_diffViewRight.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Clickable);
			_diffViewRight.HideSelection=(true);
			_diffViewRight.ImmediateEdit=(false);
			_diffViewRight.Indent=(20);
			_diffViewRight.KeepColumnWidthsAdjusted=(false);
			_diffViewRight.Model=((ITreeTableModel)null);
			_diffViewRight.MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_diffViewRight).Name = "_diffViewRight";
			_diffViewRight.NoSearchStrings=(false);
			_diffViewRight.OpenEditOnDblClk=(false);
			_diffViewRight.ReadOnly=(false);
			_diffViewRight.Scrollable=(true);
			_diffViewRight.ShowLines=(true);
			_diffViewRight.ShowPlusMinus=(true);
			_diffViewRight.ShowRootLines=(true);
			_diffViewRight.ToggleOnDblClk=(false);
			_diffViewRight.AfterCollapse+=(new TreeTableViewEventHandler(_diffView_AfterCollapse));
			_diffViewRight.AfterExpand+=(new TreeTableViewEventHandler(_diffView_AfterExpand));
			_diffViewRight.Scroll+=(new System.EventHandler(_diffView_Scroll));
			componentResourceManager.ApplyResources(splitContainer1, "splitContainer1");
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Panel1.Controls.Add((System.Windows.Forms.Control)(object)_diffView);
			splitContainer1.Panel2.Controls.Add((System.Windows.Forms.Control)(object)_diffViewRight);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(splitContainer1);
			base.Name = "DeviceObjectDiffView";
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
