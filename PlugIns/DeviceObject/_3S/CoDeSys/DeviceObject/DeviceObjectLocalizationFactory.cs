using System;
using System.Collections;
using System.Collections.Generic;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{5B0DE8CC-A9B0-4A4E-AE6B-D266D69C0A0A}")]
	public class DeviceObjectLocalizationFactory : ILocalizationFactory
	{
		public string Name => "Device Object Localization Factory";

		public IList<ILocalizableText> GetLocalizableTexts(IObject obj, LocalizationEventArgs args)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Expected O, but got Unknown
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Expected O, but got Unknown
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Expected O, but got Unknown
			LList<ILocalizableText> val = new LList<ILocalizableText>();
			if (APEnvironment.LocalizationManagerOrNull != null)
			{
				DeviceObjectBase deviceObjectBase = obj as DeviceObjectBase;
				if (deviceObjectBase != null)
				{
					foreach (IRequiredLib item in (IEnumerable)((IDeviceObject2)((deviceObjectBase is IDeviceObject2) ? deviceObjectBase : null)).DriverInfo.RequiredLibs)
					{
						foreach (IFbInstance item2 in (IEnumerable)item.FbInstances)
						{
							IFbInstance val2 = item2;
							ILocalizableText val3 = APEnvironment.LocalizationManagerOrNull.Helper.CreateText(args.Content, (LocalizationContent)2, val2.Instance.Variable);
							APEnvironment.LocalizationManagerOrNull.Helper.AddToResultWithPositionCheck(args, obj.MetaObject.ObjectGuid, val3, 1, (IList<ILocalizableText>)val);
						}
					}
					foreach (IParameter item3 in (IEnumerable)deviceObjectBase.DeviceParameterSet)
					{
						IParameter val4 = item3;
						FillLocalizableText(obj, args, val4 as Parameter, val);
					}
					{
						foreach (IConnector item4 in (IEnumerable)deviceObjectBase.Connectors)
						{
							IConnector val5 = item4;
							foreach (IRequiredLib item5 in (IEnumerable)((IConnector2)((val5 is IConnector2) ? val5 : null)).DriverInfo.RequiredLibs)
							{
								foreach (IFbInstance item6 in (IEnumerable)item5.FbInstances)
								{
									IFbInstance val6 = item6;
									ILocalizableText val7 = APEnvironment.LocalizationManagerOrNull.Helper.CreateText(args.Content, (LocalizationContent)2, val6.Instance.Variable);
									APEnvironment.LocalizationManagerOrNull.Helper.AddToResultWithPositionCheck(args, obj.MetaObject.ObjectGuid, val7, 1, (IList<ILocalizableText>)val);
								}
							}
							foreach (IParameter item7 in (IEnumerable)val5.HostParameterSet)
							{
								IParameter val8 = item7;
								FillLocalizableText(obj, args, val8 as Parameter, val);
							}
						}
						return (IList<ILocalizableText>)val;
					}
				}
			}
			return (IList<ILocalizableText>)val;
		}

		public bool AcceptsObjectType(Type objectType)
		{
			return typeof(DeviceObjectBase).IsAssignableFrom(objectType);
		}

		private void FillLocalizableText(IObject obj, LocalizationEventArgs args, Parameter parameter, LList<ILocalizableText> liResult)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			if (parameter != null && ((int)parameter.ChannelType == 1 || (int)parameter.ChannelType == 2 || (int)parameter.ChannelType == 3))
			{
				FillLocalizableText(obj, args, parameter.DataElementBase, liResult);
			}
		}

		private void FillLocalizableText(IObject obj, LocalizationEventArgs args, DataElementBase dataelement, LList<ILocalizableText> liResult)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			if (dataelement == null)
			{
				return;
			}
			foreach (DataElementBase item in (IEnumerable)dataelement.SubElements)
			{
				FillLocalizableText(obj, args, item, liResult);
			}
			if (!string.IsNullOrEmpty(dataelement.Description))
			{
				ILocalizableText val = APEnvironment.LocalizationManagerOrNull.Helper.CreateText(args.Content, (LocalizationContent)8, dataelement.Description);
				APEnvironment.LocalizationManagerOrNull.Helper.AddToResultWithPositionCheck(args, obj.MetaObject.ObjectGuid, val, 1, (IList<ILocalizableText>)liResult);
			}
			if (dataelement.IoMapping == null || dataelement.IoMapping.VariableMappings == null)
			{
				return;
			}
			foreach (VariableMapping item2 in (IEnumerable)dataelement.IoMapping.VariableMappings)
			{
				if (item2.CreateVariable)
				{
					ILocalizableText val2 = APEnvironment.LocalizationManagerOrNull.Helper.CreateText(args.Content, (LocalizationContent)2, item2.Variable);
					APEnvironment.LocalizationManagerOrNull.Helper.AddToResultWithPositionCheck(args, obj.MetaObject.ObjectGuid, val2, 1, (IList<ILocalizableText>)liResult);
				}
			}
		}
	}
}
