using System;
using System.Drawing;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	internal class BusCycleDiffNode : DeviceObjectDiffViewNode
	{
		private string _stBusCycleTask = string.Empty;

		private Guid _BusCycleTaskGuid = Guid.Empty;

		private int _nLeftProjectHandle;

		private int _nRightProjectHandle;

		private IDriverInfo5 _leftInstance;

		private IDriverInfo5 _rightInstance;

		private DiffState _diffStateCombined;

		internal IDriverInfo5 LeftInstance => _leftInstance;

		internal IDriverInfo5 RightInstance => _rightInstance;

		internal string BusCycleTask
		{
			get
			{
				return _stBusCycleTask;
			}
			set
			{
				_stBusCycleTask = value;
			}
		}

		internal Guid BusCycleTaskGuid
		{
			get
			{
				return _BusCycleTaskGuid;
			}
			set
			{
				_BusCycleTaskGuid = value;
			}
		}

		internal BusCycleDiffNode(DeviceObjectDiffViewModel model, int nLeftProjectHandle, int nRightProjectHandle, IDriverInfo5 leftInstance, IDriverInfo5 rightInstance, NodeDiffData diffData, int nIndexInParent)
			: base(model, null, diffData, nIndexInParent)
		{
			_nLeftProjectHandle = nLeftProjectHandle;
			_nRightProjectHandle = nRightProjectHandle;
			_leftInstance = leftInstance;
			_rightInstance = rightInstance;
		}

		public override object GetValue(int nColumnIndex)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			string text = string.Empty;
			Image image = null;
			DiffState val = base.DiffState;
			if (nColumnIndex == 0 || nColumnIndex == 1)
			{
				val = (DiffState)0;
			}
			if (base.Model.IsLeftModel)
			{
				switch (nColumnIndex)
				{
				case 0:
					text = Strings.BusCycleParameterName;
					image = DeviceObjectDiffViewNodeRenderer.s_structuredParamImage;
					break;
				case 2:
					if ((int)base.AcceptState != 0)
					{
						text = _stBusCycleTask;
						if (string.IsNullOrEmpty(text) && _BusCycleTaskGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nLeftProjectHandle, _BusCycleTaskGuid))
						{
							text = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nLeftProjectHandle, _BusCycleTaskGuid).Name;
						}
					}
					else if (_leftInstance != null)
					{
						text = ((IDriverInfo)_leftInstance).BusCycleTask;
						if (string.IsNullOrEmpty(text) && _leftInstance.BusCycleTaskGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nLeftProjectHandle, _leftInstance.BusCycleTaskGuid))
						{
							text = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nLeftProjectHandle, _leftInstance.BusCycleTaskGuid).Name;
						}
					}
					break;
				}
			}
			else
			{
				switch (nColumnIndex)
				{
				case 0:
					text = Strings.BusCycleParameterName;
					image = DeviceObjectDiffViewNodeRenderer.s_structuredParamImage;
					break;
				case 2:
					if (_rightInstance != null)
					{
						text = ((IDriverInfo)_rightInstance).BusCycleTask;
						if (string.IsNullOrEmpty(text) && _rightInstance.BusCycleTaskGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nLeftProjectHandle, _rightInstance.BusCycleTaskGuid))
						{
							text = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nLeftProjectHandle, _rightInstance.BusCycleTaskGuid).Name;
						}
					}
					break;
				}
			}
			if ((int)val != 0)
			{
				_diffStateCombined = val;
			}
			Color backColor = (((int)_diffStateCombined == 0) ? Color.Transparent : Color.LightGray);
			Color foreColor;
			FontStyle fontStyle;
			if (Common.IsEqual(val))
			{
				foreColor = Color.Black;
				fontStyle = FontStyle.Regular;
			}
			else if (Common.IsAdded(val))
			{
				foreColor = Color.Green;
				fontStyle = FontStyle.Bold;
			}
			else if (Common.IsDeleted(val))
			{
				foreColor = Color.Blue;
				fontStyle = FontStyle.Bold;
			}
			else
			{
				foreColor = Color.Red;
				fontStyle = FontStyle.Bold;
			}
			if ((int)base.AcceptState != 0)
			{
				backColor = Color.Gold;
			}
			return new DeviceObjectDiffViewNodeData(text, foreColor, backColor, fontStyle, (text != string.Empty) ? image : null);
		}
	}
}
