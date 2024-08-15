using System.Drawing;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	internal class FbInstancesDiffNode : DeviceObjectDiffViewNode
	{
		private bool _bDiffVariable;

		private bool _bDiffType;

		private string _stInstanceVariable = string.Empty;

		private IFbInstance _leftInstance;

		private IFbInstance _rightInstance;

		private DiffState _diffStateCombined;

		internal IFbInstance LeftInstance => _leftInstance;

		internal IFbInstance RightInstance => _rightInstance;

		internal string InstanceVariable
		{
			get
			{
				return _stInstanceVariable;
			}
			set
			{
				_stInstanceVariable = value;
			}
		}

		internal bool DiffType
		{
			get
			{
				return _bDiffType;
			}
			set
			{
				_bDiffType = value;
			}
		}

		internal bool DiffVariable
		{
			get
			{
				return _bDiffVariable;
			}
			set
			{
				_bDiffVariable = value;
			}
		}

		internal FbInstancesDiffNode(DeviceObjectDiffViewModel model, IFbInstance leftInstance, IFbInstance rightInstance, NodeDiffData diffData, int nIndexInParent)
			: base(model, null, diffData, nIndexInParent)
		{
			_leftInstance = leftInstance;
			_rightInstance = rightInstance;
		}

		public override object GetValue(int nColumnIndex)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			string text = string.Empty;
			Image image = null;
			DiffState val = base.DiffState;
			if (nColumnIndex == 0)
			{
				val = (DiffState)0;
			}
			if (nColumnIndex == 1 && !_bDiffType)
			{
				val = (DiffState)0;
			}
			if (nColumnIndex == 3 && !_bDiffVariable)
			{
				val = (DiffState)0;
			}
			if (base.Model.IsLeftModel)
			{
				switch (nColumnIndex)
				{
				case 0:
					text = Strings.IECObjectParameterName;
					image = DeviceObjectDiffViewNodeRenderer.s_structuredParamImage;
					break;
				case 1:
					if (_leftInstance != null)
					{
						text = _leftInstance.FbName;
					}
					break;
				case 3:
					if ((int)base.AcceptState != 0 && !string.IsNullOrEmpty(_stInstanceVariable))
					{
						text = _stInstanceVariable;
					}
					else if (_leftInstance != null)
					{
						text = _leftInstance.Instance.Variable;
					}
					break;
				}
			}
			else
			{
				switch (nColumnIndex)
				{
				case 0:
					text = Strings.IECObjectParameterName;
					image = DeviceObjectDiffViewNodeRenderer.s_structuredParamImage;
					break;
				case 1:
					if (_rightInstance != null)
					{
						text = _rightInstance.FbName;
					}
					break;
				case 3:
					if (_rightInstance != null)
					{
						text = _rightInstance.Instance.Variable;
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
