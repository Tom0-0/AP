using System.Drawing;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	internal class AlwaysMappingDiffNode : DeviceObjectDiffViewNode
	{
		private AlwaysMappingMode? _AlwaysMappingMode;

		private AlwaysMappingMode? _leftInstance;

		private AlwaysMappingMode? _rightInstance;

		private DiffState _diffStateCombined;

		internal AlwaysMappingMode? LeftInstance => _leftInstance;

		internal AlwaysMappingMode? RightInstance => _rightInstance;

		internal AlwaysMappingMode? AlwaysMappingMode
		{
			get
			{
				return _AlwaysMappingMode;
			}
			set
			{
				_AlwaysMappingMode = value;
			}
		}

		internal AlwaysMappingDiffNode(DeviceObjectDiffViewModel model, AlwaysMappingMode? leftInstance, AlwaysMappingMode? rightInstance, NodeDiffData diffData, int nIndexInParent)
			: base(model, null, diffData, nIndexInParent)
		{
			_leftInstance = leftInstance;
			_rightInstance = rightInstance;
		}

		public override object GetValue(int nColumnIndex)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
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
					text = Strings.AlwaysMappingModeParameterName;
					image = DeviceObjectDiffViewNodeRenderer.s_structuredParamImage;
					break;
				case 2:
					if ((int)base.AcceptState != 0)
					{
						text = MappingMode(_AlwaysMappingMode);
					}
					else if (_leftInstance.HasValue)
					{
						text = MappingMode(_leftInstance);
					}
					break;
				}
			}
			else
			{
				switch (nColumnIndex)
				{
				case 0:
					text = Strings.AlwaysMappingModeParameterName;
					image = DeviceObjectDiffViewNodeRenderer.s_structuredParamImage;
					break;
				case 2:
					if (_rightInstance.HasValue)
					{
						text = MappingMode(_rightInstance);
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

		private string MappingMode(AlwaysMappingMode? mode)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			string result = string.Empty;
			if (mode.HasValue)
			{
				AlwaysMappingMode? val = mode;
				if (val.HasValue)
				{
					AlwaysMappingMode valueOrDefault = val.GetValueOrDefault();
					if ((int)valueOrDefault != 0)
					{
						if ((int)valueOrDefault == 1)
						{
							result = Strings.AlwaysMappingMode2;
						}
					}
					else
					{
						result = Strings.AlwaysMappingMode1;
					}
				}
			}
			return result;
		}
	}
}
