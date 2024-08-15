using System;
using System.Collections;
using System.Drawing;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ParameterDiffViewNode : DeviceObjectDiffViewNode
	{
		private DiffState _diffStateCombined;

		private DeviceObjectDiffData _diffData;

		internal IParameter LeftParameter => _diffData.LeftParameter;

		internal IParameter RightParameter => _diffData.RightParameter;

		internal IDataElement LeftElement => _diffData.LeftElement;

		internal IDataElement RightElement => _diffData.RightElement;

		internal int ConnectionId => _diffData.ConnectionId;

		public string LeftValue
		{
			get
			{
				return _diffData.LeftValue;
			}
			set
			{
				_diffData.LeftValue = value;
			}
		}

		public string LeftMapping
		{
			get
			{
				return _diffData.LeftMapping;
			}
			set
			{
				_diffData.LeftMapping = value;
			}
		}

		public string LeftAddress
		{
			get
			{
				return _diffData.LeftAddress;
			}
			set
			{
				_diffData.LeftAddress = value;
			}
		}

		public string LeftDescription
		{
			get
			{
				return _diffData.LeftDescription;
			}
			set
			{
				_diffData.LeftDescription = value;
			}
		}

		internal IParameterSection LeftSection => _diffData.LeftSection;

		internal IParameterSection RightSection => _diffData.RightSection;

		internal ParameterDiffViewNode(DeviceObjectDiffViewModel model, DeviceObjectDiffViewNode parent, DeviceObjectDiffData diffData, NodeDiffData nodeDiff, int nIndexInParent)
			: base(model, parent, nodeDiff, nIndexInParent)
		{
			_diffData = diffData;
		}

		public override object GetValue(int nColumnIndex)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0627: Unknown result type (might be due to invalid IL or missing references)
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
					if (LeftElement != null)
					{
						text = LeftElement.VisibleName;
					}
					if (LeftSection != null)
					{
						text = LeftSection.Name;
					}
					image = _diffData.Image;
					break;
				case 1:
					if (LeftElement != null && !LeftElement.HasSubElements)
					{
						text = LeftElement.BaseType;
					}
					break;
				case 2:
					if ((int)base.AcceptState != 0)
					{
						text = LeftValue;
					}
					else if (LeftParameter != null && (int)LeftParameter.ChannelType == 0)
					{
						if (LeftElement != null && LeftElement.IsEnumeration && LeftElement is IEnumerationDataElement)
						{
							int num = default(int);
							IEnumerationValue enumerationValue = ((IEnumerationDataElement)LeftElement).GetEnumerationValue((object)LeftElement.Value, out num);
							if (enumerationValue != null)
							{
								text = enumerationValue.VisibleName;
							}
						}
						else if (LeftElement != null && !LeftElement.HasSubElements)
						{
							text = LeftElement.Value;
						}
					}
					else
					{
						val = (DiffState)0;
					}
					break;
				case 3:
					if ((int)base.AcceptState != 0)
					{
						text = LeftMapping;
					}
					else if (LeftElement != null && LeftElement.IoMapping != null && LeftElement.IoMapping.VariableMappings != null && ((ICollection)LeftElement.IoMapping.VariableMappings).Count > 0)
					{
						text = LeftElement.IoMapping.VariableMappings[0]
							.Variable;
					}
					break;
				case 4:
					if ((int)base.AcceptState != 0 && !string.IsNullOrEmpty(LeftAddress))
					{
						text = LeftAddress;
						break;
					}
					if (LeftElement != null && (int)LeftParameter.ChannelType != 0)
					{
						text = LeftElement.IoMapping.IecAddress;
					}
					if (RightElement != null && LeftElement != null && (int)RightParameter.ChannelType != 0 && ((text == RightElement.IoMapping.IecAddress && !LeftElement.IoMapping.AutomaticIecAddress && !RightElement.IoMapping.AutomaticIecAddress) || (LeftElement.IoMapping.AutomaticIecAddress && RightElement.IoMapping.AutomaticIecAddress)))
					{
						val = (DiffState)0;
					}
					break;
				case 5:
					if ((int)base.AcceptState != 0)
					{
						text = LeftDescription;
					}
					else if (LeftElement != null && (int)LeftParameter.ChannelType != 0)
					{
						text = LeftElement.Description;
					}
					if (RightElement != null && text == RightElement.Description)
					{
						val = (DiffState)0;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException("nColumnIndex");
				}
			}
			else
			{
				switch (nColumnIndex)
				{
				case 0:
					if (RightElement != null)
					{
						text = RightElement.VisibleName;
					}
					if (RightSection != null)
					{
						text = RightSection.Name;
					}
					image = _diffData.Image;
					break;
				case 1:
					if (RightElement != null && !RightElement.HasSubElements)
					{
						text = RightElement.BaseType;
					}
					break;
				case 2:
					if (RightParameter != null && (int)RightParameter.ChannelType == 0)
					{
						if (RightElement != null && RightElement.IsEnumeration && RightElement is IEnumerationDataElement)
						{
							int num2 = default(int);
							IEnumerationValue enumerationValue2 = ((IEnumerationDataElement)RightElement).GetEnumerationValue((object)RightElement.Value, out num2);
							if (enumerationValue2 != null)
							{
								text = enumerationValue2.VisibleName;
							}
						}
						else if (RightElement != null && !RightElement.HasSubElements)
						{
							text = RightElement.Value;
						}
					}
					else
					{
						val = (DiffState)0;
					}
					break;
				case 3:
					if (RightElement != null && RightElement.IoMapping != null && RightElement.IoMapping.VariableMappings != null && ((ICollection)RightElement.IoMapping.VariableMappings).Count > 0)
					{
						text = RightElement.IoMapping.VariableMappings[0]
							.Variable;
					}
					break;
				case 4:
					if (RightElement != null && (int)RightParameter.ChannelType != 0)
					{
						text = RightElement.IoMapping.IecAddress;
					}
					if (RightElement != null && LeftElement != null && (int)LeftParameter.ChannelType != 0 && ((text == LeftElement.IoMapping.IecAddress && !LeftElement.IoMapping.AutomaticIecAddress && !RightElement.IoMapping.AutomaticIecAddress) || (LeftElement.IoMapping.AutomaticIecAddress && RightElement.IoMapping.AutomaticIecAddress)))
					{
						val = (DiffState)0;
					}
					break;
				case 5:
					if (RightElement != null && (int)RightParameter.ChannelType != 0)
					{
						text = RightElement.Description;
					}
					if (LeftElement != null && text == LeftElement.Description)
					{
						val = (DiffState)0;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException("nColumnIndex");
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
