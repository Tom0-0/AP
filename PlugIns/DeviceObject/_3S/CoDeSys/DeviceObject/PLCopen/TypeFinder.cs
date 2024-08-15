using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class TypeFinder
	{
		internal EnumType FindEnumType(DeviceObject deviceObject, DataElementEnumType en)
		{
			foreach (ITypeDefinition typeDefinition in deviceObject.Types.TypeMap.Values)
			{
				if (typeDefinition is EnumType && this.CheckEnumEquivalent(typeDefinition as EnumType, en))
				{
					return typeDefinition as EnumType;
				}
			}
			return null;
		}

		private bool CheckEnumEquivalent(EnumType type, DataElementEnumType data)
		{
			if (type.BaseType == "std:" + data.BaseType && type.Values.Count == data.EnumerationValues.Length)
			{
				int num = 0;
				foreach (EnumTypeValue value in type.Values)
				{
					if (value.Identifier == data.EnumerationValues[num].Identifier && value.Value == data.GetValueForEnumeration(data.EnumerationValues[num]))
					{
						EnumValue obj = data.EnumerationValues[num] as EnumValue;
						StringRef stringRef = ((GenericObject)obj).GetSerializableValue("Name") as StringRef;
						StringRef stringRef2 = ((GenericObject)obj).GetSerializableValue("Description") as StringRef;
						if (!(stringRef.Identifier == value.Name.Identifier) || !(stringRef2.Identifier == value.Description.Identifier))
						{
							return false;
						}
						num++;
						continue;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		internal BitfieldType FindBitType(DeviceObject deviceObject, DataElementBitFieldType bitfield)
		{
			foreach (ITypeDefinition typeDefinition in deviceObject.Types.TypeMap.Values)
			{
				if (typeDefinition is BitfieldType && this.CheckBitEquivalent(typeDefinition as BitfieldType, bitfield))
				{
					return typeDefinition as BitfieldType;
				}
			}
			return null;
		}

		private bool CheckBitEquivalent(BitfieldType type, DataElementBitFieldType data)
		{
			if (type.BaseType == data.BaseType && type.Components.Count == ((ICollection)data.SubElements).Count)
			{
				int num = 0;
				foreach (StructComponent component in type.Components)
				{
					DataElementBitFieldComponent dataElementBitFieldComponent = data.SubElements[num] as DataElementBitFieldComponent;
					if (component.Identifier == dataElementBitFieldComponent.Identifier)
					{
						if ((((GenericObject)dataElementBitFieldComponent).GetSerializableValue("Description") as StringRef).Identifier != component.Description.Identifier)
						{
							return false;
						}
						num++;
						continue;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		internal ArrayType FindArrayType(DeviceObject deviceObject, DataElementArrayType array)
		{
			LDictionary<string, ITypeDefinition> typeMap = deviceObject.Types.TypeMap;
			foreach (ITypeDefinition typeDefinition in typeMap.Values)
			{
				if (typeDefinition is ArrayType)
				{
					ArrayType arrayType = typeDefinition as ArrayType;
					if (this.CheckArrayDimensionEquivalent(typeDefinition as ArrayType, array))
					{
						int num = arrayType.BaseType.IndexOf(":");
						if (num > 0)
						{
							string a = arrayType.BaseType.Substring(0, num);
							string a2 = arrayType.BaseType.Substring(num + 1);
							DataElementBase dataElementBase = array.SubElements[0] as DataElementBase;
							if (a == "std")
							{
								if (a2 == dataElementBase.BaseType)
								{
									return arrayType;
								}
							}
							else if (dataElementBase is DataElementBitFieldType)
							{
								DataElementBitFieldType data = dataElementBase as DataElementBitFieldType;
								ITypeDefinition typeDefinition2 = null;
								if (typeMap.TryGetValue(arrayType.BaseType, out typeDefinition2) && this.CheckBitEquivalent(typeDefinition2 as BitfieldType, data))
								{
									return arrayType;
								}
							}
						}
					}
				}
			}
			return null;
		}

		private bool CheckArrayDimensionEquivalent(ArrayType arrayDef, DataElementArrayType array)
		{
			if (CheckDimensionEqual(arrayDef.FirstDimension, (ITypeArrayBorders)(object)array.Dimension1) && CheckDimensionEqual(arrayDef.SecondDimension, (ITypeArrayBorders)(object)array.Dimension2))
			{
				return CheckDimensionEqual(arrayDef.ThirdDimension, (ITypeArrayBorders)(object)array.Dimension3);
			}
			return false;
		}

		private bool CheckDimensionEqual(ITypeArrayBorders border1, ITypeArrayBorders border2)
		{
			if ((border1 == null && border2 != null) || (border1 != null && border2 == null))
			{
				return false;
			}
			if (border1 == null && border2 == null)
			{
				return true;
			}
			if (border1.LowerBorder == border2.LowerBorder)
			{
				return border1.UpperBorder == border2.UpperBorder;
			}
			return false;
		}

		internal RangeType FindRangeType(DeviceObject deviceObject, DataElementRangeType range)
		{
			foreach (ITypeDefinition typeDefinition in deviceObject.Types.TypeMap.Values)
			{
				TypeDefinition typeDefinition2 = (TypeDefinition)typeDefinition;
				if (typeDefinition2 is RangeType && this.CheckRangeEquivalent(typeDefinition2 as RangeType, range))
				{
					return typeDefinition2 as RangeType;
				}
			}
			return null;
		}

		private bool CheckRangeEquivalent(RangeType type, DataElementRangeType data)
		{
			if (type.BaseType == "std:" + data.BaseType)
			{
				if (type.Max == data.MaxValue)
				{
					return type.Min == data.MinValue;
				}
				return false;
			}
			return false;
		}
	}
}
