namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class EnumConv
	{
		internal static ConnectorRole CreateConnectorRole(ConnectorTypeRole connectorTypeRole)
		{
			if (connectorTypeRole != 0 && connectorTypeRole == ConnectorTypeRole.child)
			{
				return (ConnectorRole)1;
			}
			return (ConnectorRole)0;
		}

		internal static ConnectorTypeRole CreateConnectorRoleType(ConnectorRole connectorRole)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			if ((int)connectorRole != 0 && (int)connectorRole == 1)
			{
				return ConnectorTypeRole.child;
			}
			return ConnectorTypeRole.parent;
		}

		internal static AccessRight CreateAccessRight(AccessRightType accessType)
		{
			return (AccessRight)(accessType switch
			{
				AccessRightType.none => 0, 
				AccessRightType.read => 1, 
				AccessRightType.write => 2, 
				_ => 3, 
			});
		}

		internal static AccessRightType CreateAccessRightType(AccessRight accessRight)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected I4, but got Unknown
			return (int)accessRight switch
			{
				0 => AccessRightType.none, 
				1 => AccessRightType.read, 
				2 => AccessRightType.write, 
				_ => AccessRightType.readwrite, 
			};
		}

		internal static ChannelType CreateChannel(ParameterTypeAttributesChannel channelType)
		{
			return (ChannelType)(channelType switch
			{
				ParameterTypeAttributesChannel.input => 1, 
				ParameterTypeAttributesChannel.output => 2, 
				_ => 0, 
			});
		}

		internal static ParameterTypeAttributesChannel CreateChannelType(ChannelType channelType, DiagType diagType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected I4, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected I4, but got Unknown
			return (int)diagType switch
			{
				0 => (int)channelType switch
				{
					2 => ParameterTypeAttributesChannel.output, 
					1 => ParameterTypeAttributesChannel.input, 
					_ => ParameterTypeAttributesChannel.none, 
				}, 
				2 => ParameterTypeAttributesChannel.diagAck, 
				1 => ParameterTypeAttributesChannel.diag, 
				_ => ParameterTypeAttributesChannel.none, 
			};
		}
	}
}
