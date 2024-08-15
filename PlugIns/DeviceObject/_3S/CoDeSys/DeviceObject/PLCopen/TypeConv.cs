namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class TypeConv
	{
		internal static ParameterTypeAttributes CreateAttributes(Parameter p)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Invalid comparison between Unknown and I4
			ParameterTypeAttributes parameterTypeAttributes = new ParameterTypeAttributes();
			parameterTypeAttributes.channel = EnumConv.CreateChannelType(p.ChannelType, p.DiagType);
			parameterTypeAttributes.offlineaccess = EnumConv.CreateAccessRightType(p.GetAccessRight(bOnline: false));
			parameterTypeAttributes.onlineaccess = EnumConv.CreateAccessRightType(p.GetAccessRight(bOnline: true));
			parameterTypeAttributes.alwaysmapping = p.AlwaysMapping;
			if ((int)p.AlwaysMappingMode == 1)
			{
				parameterTypeAttributes.alwaysmappingMode = ParameterTypeAttributesAlwaysmappingMode.AlwaysInBusCycle;
			}
			else
			{
				parameterTypeAttributes.alwaysmappingMode = ParameterTypeAttributesAlwaysmappingMode.OnlyIfUnused;
			}
			parameterTypeAttributes.Any = null;
			parameterTypeAttributes.createDownloadStructure = p.CreateDownloadStructure;
			parameterTypeAttributes.createInChildConnector = p.CreateInChildConnector;
			parameterTypeAttributes.createInHostConnector = p.CreateInHostConnector;
			parameterTypeAttributes.download = p.Download;
			parameterTypeAttributes.onlineparameter = p.OnlineParameter;
			parameterTypeAttributes.instanceVariable = p.FbInstanceVariable;
			parameterTypeAttributes.preparedValueAccess = p.PreparedValueAccess;
			parameterTypeAttributes.useRefactoring = p.UseRefactoring;
			parameterTypeAttributes.disableMapping = p.DisableMapping;
			parameterTypeAttributes.bidirectionaloutput = p.BidirectionalOutput;
			if (!string.IsNullOrEmpty(p.DriverSpecific))
			{
				parameterTypeAttributes.driverSpecific = p.DriverSpecific;
			}
			return parameterTypeAttributes;
		}

		internal static DimensionType CreateDimensionType(ITypeArrayBorders arrayTypeBorder)
		{
			if (arrayTypeBorder != null)
			{
				int result = 0;
				int result2 = 0;
				int.TryParse(arrayTypeBorder.LowerBorder, out result);
				int.TryParse(arrayTypeBorder.UpperBorder, out result2);
				return new DimensionType
				{
					LowerBorder = result,
					UpperBorder = result2
				};
			}
			return null;
		}
	}
}
