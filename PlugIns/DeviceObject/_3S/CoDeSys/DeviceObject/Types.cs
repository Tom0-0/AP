#define DEBUG
using System;
using System.Diagnostics;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public abstract class Types
	{
		public const ushort UnknownType = ushort.MaxValue;

		public const long UnknownLength = -1L;

		internal static LDictionary<string, ICompiledType> _dictCompiledTypes = new LDictionary<string, ICompiledType>();

		internal static LDictionary<string, ushort> _dictTypeIds = new LDictionary<string, ushort>();

		internal static LDictionary<string, long> _dictBitSize = new LDictionary<string, long>();

		public static ICompiledType ParseType(string stType)
		{
			ICompiledType val = null;
			if (!_dictCompiledTypes.TryGetValue(stType, out val))
			{
				IScanner val2 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(stType, false, false, false, false);
				val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateParser(val2).ParseTypeDeclaration();
				_dictCompiledTypes[stType]= val;
			}
			return val;
		}

		public static long GetBitSize(string stType)
		{
			long result = default(long);
			if (!_dictBitSize.TryGetValue(stType, out result))
			{
				stType = stType.ToUpperInvariant();
				switch (stType)
				{
				case "BIT":
				case "SAFEBOOL":
				case "SAFEBIT":
					_dictBitSize[stType]= 1L;
					return 1L;
				default:
				{
					ICompiledType val = ParseType(stType);
					try
					{
						long num = val.Size((IScope)null);
						switch (num)
						{
						case -1L:
							_dictBitSize[stType]= -1L;
							return -1L;
						case 0L:
							_dictBitSize[stType]= 1L;
							return 1L;
						default:
							_dictBitSize[stType]= num * 8;
							return num * 8;
						}
					}
					catch (Exception value)
					{
						Debug.WriteLine(value);
						return -1L;
					}
				}
				}
			}
			return result;
		}

		public static ushort GetTypeId(string stType, ushort usDefault)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Invalid comparison between Unknown and I4
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Expected I4, but got Unknown
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Invalid comparison between Unknown and I4
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			ushort num = default(ushort);
			if (!_dictTypeIds.TryGetValue(stType, out num))
			{
				stType = stType.ToUpperInvariant();
				if (stType == "BIT" || stType == "SAFEBIT")
				{
					_dictTypeIds[stType]= (ushort)1;
					return 1;
				}
				ICompiledType val = ParseType(stType);
				if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)0))
				{
					TypeClass @class = ((IType)val).Class;
					switch ((int)@class)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
					case 26:
					case 37:
						return (ushort)((IType)val).Class;
					default:
						Debug.WriteLine("Unknown type '" + stType + "' in " + typeof(Types).FullName + ".GetTypeIds");
						return usDefault;
					}
				}
				if ((int)((IType)val).Class == 28)
				{
					Debug.WriteLine("Unknown type '" + stType + "' in " + typeof(Types).FullName + ".GetTypeIds");
					return usDefault;
				}
				if ((int)((IType)val).Class == 24)
				{
					num = (ushort)((IType)((ISubrangeType2)((val is ISubrangeType2) ? val : null)).BaseType).Class;
					_dictTypeIds[stType]= num;
				}
				else
				{
					num = (ushort)((IType)val).Class;
					_dictTypeIds[stType]= num;
				}
			}
			return num;
		}
	}
}
