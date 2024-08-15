using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject.LogicalDevice
{
	[TypeGuid("{9A28F562-0381-417F-B288-AB54B9316B25}")]
	public class LogicalExchangeGVLObjectComparer : IObjectComparer
	{
		public bool AcceptsObjectType(Type objectType, Type[] embeddedObjectTypes)
		{
			if (objectType != null && typeof(ILogicalGVLObject).IsAssignableFrom(objectType))
			{
				return true;
			}
			return false;
		}

		public bool CheckEquality(IObject leftObject, IObject rightObject, bool bIgnoreWhitespace, bool bIgnoreComments, bool bIgnoreProperties)
		{
			string getDeclaration = ((LogicalExchangeGVLObject)(object)leftObject).GetDeclaration;
			string getDeclaration2 = ((LogicalExchangeGVLObject)(object)rightObject).GetDeclaration;
			return APEnvironment.IECTextComparer.CheckEquality(getDeclaration, getDeclaration2, bIgnoreWhitespace, bIgnoreComments);
		}
	}
}
