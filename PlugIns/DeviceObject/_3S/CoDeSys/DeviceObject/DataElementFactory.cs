#define DEBUG
using System;
using System.Diagnostics;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DataElementFactory
	{
		private TypeList _typeList;

		internal DataElementFactory(TypeList tl)
		{
			_typeList = tl;
		}

		public DataElementBase Create(string stIdentifier, LList<ValueElement> defaultValue, string stType, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, IDataElementParent parent, DataElementBase deOriginal, bool bUpdate, bool bCreateBitChannels)
		{
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Invalid comparison between Unknown and I4
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Invalid comparison between Unknown and I4
			bool flag = bUpdate;
			bool flag2 = false;
			bool flag3 = false;
			TypeDefinition typeDefinition = null;
			DataElementBase dataElementBase = null;
			string text = string.Empty;
			string text2 = stType.Trim();
			string[] array = stType.Split(':');
			if (array.Length >= 2)
			{
				text = array[0].Trim();
				text2 = array[1].Trim();
				flag2 = text.ToLowerInvariant().Equals("std");
				if (!flag2)
				{
					flag3 = text.ToLowerInvariant().Equals("external");
				}
			}
			if (flag2)
			{
				if (bUpdate && deOriginal is DataElementSimpleType)
				{
					((DataElementSimpleType)deOriginal).Import(array[1], stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate: true, parent);
				}
				else
				{
					if ((parent is IParameter && (int)((IParameter)((parent is IParameter) ? parent : null)).ChannelType != 0) || (parent is DataElementArrayType && (parent.DataElement as DataElementArrayType).CreatedFromParameter))
					{
						ICompiledType val = Types.ParseType(text2);
						if ((int)((IType)val).Class == 26)
						{
							dataElementBase = ((!(deOriginal is DataElementArrayType)) ? new DataElementArrayType() : deOriginal);
							(dataElementBase as DataElementArrayType).CreatedFromParameter = true;
							((DataElementArrayType)dataElementBase).Import((IArrayType)(object)((val is IArrayType) ? val : null), text + ":" + ((object)val.BaseType).ToString(), this, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate, bCreateBitChannels, parent);
							bUpdate = false;
						}
						if (bCreateBitChannels && val.IsInteger)
						{
							long bitSize = Types.GetBitSize(text2);
							if (bitSize >= 8 && bitSize <= 64)
							{
								dataElementBase = ((!(dataElementBase is DataElementBitFieldType)) ? new DataElementBitFieldType() : deOriginal);
								((DataElementBitFieldType)dataElementBase).Import(text2, this, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate, parent);
								if (bUpdate && deOriginal != null && dataElementBase.GetBitSize() != deOriginal.GetBitSize())
								{
									bUpdate = false;
								}
							}
						}
					}
					if (dataElementBase == null)
					{
						dataElementBase = new DataElementSimpleType();
						((DataElementSimpleType)dataElementBase).Import(text2, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate: false, parent);
						bUpdate = false;
					}
				}
				if (bUpdate)
				{
					dataElementBase = deOriginal;
				}
				Debug.Assert(dataElementBase != null);
			}
			else
			{
				string text3 = stType;
				IArrayType val2 = null;
				if (text2.ToUpperInvariant().Replace(" ", "").StartsWith("ARRAY["))
				{
					ICompiledType val3 = Types.ParseType(text2);
					if ((int)((IType)val3).Class == 26)
					{
						val2 = (IArrayType)(object)((val3 is IArrayType) ? val3 : null);
						text3 = text + ":" + val3.BaseType;
					}
				}
				typeDefinition = _typeList.GetTypeDefinition(text3);
				if (typeDefinition == null)
				{
					if (!flag3)
					{
						throw new ArgumentException($"Undefined type: '{text3}'");
					}
					bUpdate = false;
					if (val2 != null)
					{
						if (!bUpdate || !(deOriginal is DataElementArrayType))
						{
							dataElementBase = new DataElementArrayType();
							(dataElementBase as DataElementArrayType).CreatedFromParameter = true;
							bUpdate = false;
						}
					}
					else
					{
						dataElementBase = new DataElementSimpleType();
						((DataElementSimpleType)dataElementBase).Import(text2, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate: false, parent);
					}
				}
				else if (val2 != null)
				{
					if (!bUpdate || !(deOriginal is DataElementArrayType))
					{
						dataElementBase = new DataElementArrayType();
						(dataElementBase as DataElementArrayType).CreatedFromParameter = true;
						bUpdate = false;
					}
				}
				else if (typeDefinition is RangeType)
				{
					if (!bUpdate || !(deOriginal is DataElementRangeType))
					{
						dataElementBase = new DataElementRangeType();
						bUpdate = false;
					}
				}
				else if (typeDefinition is StructType)
				{
					if (!bUpdate || !(deOriginal is DataElementStructType))
					{
						dataElementBase = new DataElementStructType();
						bUpdate = false;
					}
				}
				else if (typeDefinition is UnionType)
				{
					if (!bUpdate || !(deOriginal is DataElementUnionType))
					{
						dataElementBase = new DataElementUnionType();
						bUpdate = false;
					}
				}
				else if (typeDefinition is BitfieldType)
				{
					if (!bUpdate || !(deOriginal is DataElementBitFieldType))
					{
						dataElementBase = new DataElementBitFieldType();
						bUpdate = false;
					}
				}
				else if (typeDefinition is EnumType)
				{
					if (!bUpdate || !(deOriginal is DataElementEnumType))
					{
						dataElementBase = new DataElementEnumType();
						bUpdate = false;
					}
				}
				else
				{
					if (!(typeDefinition is ArrayType))
					{
						throw new ArgumentException("Unhandled type");
					}
					if (!bUpdate || !(deOriginal is DataElementArrayType))
					{
						dataElementBase = new DataElementArrayType();
						bUpdate = false;
					}
				}
				if (bUpdate)
				{
					dataElementBase = deOriginal;
				}
				Debug.Assert(dataElementBase != null);
				if (val2 != null)
				{
					((DataElementArrayType)dataElementBase).Import(val2, text3, this, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate, bCreateBitChannels, parent);
				}
				else if (typeDefinition != null)
				{
					if (dataElementBase is DataElementArrayType)
					{
						(dataElementBase as DataElementArrayType).CreatedFromParameter = true;
						((DataElementArrayType)dataElementBase).Import(typeDefinition, this, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate, bCreateBitChannels, parent);
					}
					else
					{
						dataElementBase.Import(typeDefinition, this, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate, parent);
					}
				}
			}
			dataElementBase.Parent = parent;
			if (flag && dataElementBase != deOriginal && dataElementBase != null && deOriginal != null)
			{
				dataElementBase.Merge(deOriginal);
			}
			return dataElementBase;
		}
	}
}
