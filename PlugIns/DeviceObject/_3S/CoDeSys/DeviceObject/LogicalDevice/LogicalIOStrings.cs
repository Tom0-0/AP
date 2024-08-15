using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace _3S.CoDeSys.DeviceObject.LogicalDevice
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class LogicalIOStrings
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("_3S.CoDeSys.DeviceObject.LogicalDevice.LogicalIOStrings", typeof(LogicalIOStrings).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string ErrorInputSizeExceeded => ResourceManager.GetString("ErrorInputSizeExceeded", resourceCulture);

		internal static string ErrorLogicalDeviceMultipleMapped => ResourceManager.GetString("ErrorLogicalDeviceMultipleMapped", resourceCulture);

		internal static string ErrorLogicalDeviceNotCompatible => ResourceManager.GetString("ErrorLogicalDeviceNotCompatible", resourceCulture);

		internal static string ErrorLogicalDeviceNotMapped => ResourceManager.GetString("ErrorLogicalDeviceNotMapped", resourceCulture);

		internal static string ErrorMappingNotValid => ResourceManager.GetString("ErrorMappingNotValid", resourceCulture);

		internal static string ErrorMergingParams => ResourceManager.GetString("ErrorMergingParams", resourceCulture);

		internal static string ErrorOutputSizeExceeded => ResourceManager.GetString("ErrorOutputSizeExceeded", resourceCulture);

		internal static string InfoMultipleMapping => ResourceManager.GetString("InfoMultipleMapping", resourceCulture);

		internal static string LogicalExchangeGVLDescription => ResourceManager.GetString("LogicalExchangeGVLDescription", resourceCulture);

		internal static string LogicalExchangeGVLName => ResourceManager.GetString("LogicalExchangeGVLName", resourceCulture);

		internal static string LogicalIODescription => ResourceManager.GetString("LogicalIODescription", resourceCulture);

		internal static string LogicalIOName => ResourceManager.GetString("LogicalIOName", resourceCulture);

		internal static string LogicalIOObjectDescription => ResourceManager.GetString("LogicalIOObjectDescription", resourceCulture);

		internal static string LogicalIOObjectName => ResourceManager.GetString("LogicalIOObjectName", resourceCulture);

		internal static string NameMustBeIdentifier => ResourceManager.GetString("NameMustBeIdentifier", resourceCulture);

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal LogicalIOStrings()
		{
		}
	}
}
