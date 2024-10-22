using System.ComponentModel;
using System.Drawing;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceObject
{
	internal static class UserOptionsHelper
	{
		private static readonly string ADD_DEVICE_WINDOW_LOCATION;

		private static readonly string ADD_DEVICE_WINDOW_SIZE;

		private static readonly string ADD_DEVICE_WINDOW_CONTAINER_HEIGHT;

		private static readonly string SUB_KEY;

		private static readonly TypeConverter POINT_CONVERTER;

		private static readonly TypeConverter SIZE_CONVERTER;

		internal static Point AddDeviceWindowLocation
		{
			get
			{
				if (OptionKey.HasValue(ADD_DEVICE_WINDOW_LOCATION, typeof(string)))
				{
					return (Point)POINT_CONVERTER.ConvertFromInvariantString((string)OptionKey[ADD_DEVICE_WINDOW_LOCATION]);
				}
				return Point.Empty;
			}
			set
			{
				OptionKey[ADD_DEVICE_WINDOW_LOCATION]= (object)POINT_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static Size AddDeviceWindowSize
		{
			get
			{
				if (OptionKey.HasValue(ADD_DEVICE_WINDOW_SIZE, typeof(string)))
				{
					return (Size)SIZE_CONVERTER.ConvertFromInvariantString((string)OptionKey[ADD_DEVICE_WINDOW_SIZE]);
				}
				return new Size(627, 600);
			}
			set
			{
				OptionKey[ADD_DEVICE_WINDOW_SIZE]= (object)SIZE_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static int AddDeviceWindowContainerHeight
		{
			get
			{
				if (OptionKey.HasValue(ADD_DEVICE_WINDOW_CONTAINER_HEIGHT, typeof(int)))
				{
					return (int)OptionKey[ADD_DEVICE_WINDOW_CONTAINER_HEIGHT];
				}
				return 0;
			}
			set
			{
				OptionKey[ADD_DEVICE_WINDOW_CONTAINER_HEIGHT]= (object)value;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey(SUB_KEY);

		static UserOptionsHelper()
		{
			ADD_DEVICE_WINDOW_LOCATION = "AddDeviceWindowLocation";
			ADD_DEVICE_WINDOW_SIZE = "AddDeviceWindowSize";
			ADD_DEVICE_WINDOW_CONTAINER_HEIGHT = "AddDeviceWindowContainerHeight";
			SUB_KEY = "{25CE77F0-AD85-4277-9AA8-CE031F5BEEC4}";
			POINT_CONVERTER = TypeDescriptor.GetConverter(typeof(Point));
			SIZE_CONVERTER = TypeDescriptor.GetConverter(typeof(Size));
		}
	}
}
