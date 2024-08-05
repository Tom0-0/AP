using System.ComponentModel;
using System.Drawing;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceEditor
{
	internal static class UserOptionsHelper
	{
		private static readonly string IO_CHANNEL_WINDOW_LOCATION;

		private static readonly string IO_CHANNEL_WINDOW_SIZE;

		private static readonly string IO_CHANNEL_WINDOW_CONTAINER_HEIGHT;

		private static readonly string SUB_KEY;

		private static readonly TypeConverter POINT_CONVERTER;

		private static readonly TypeConverter SIZE_CONVERTER;

		internal static Point IOChannelWindowLocation
		{
			get
			{
				if (OptionKey.HasValue(IO_CHANNEL_WINDOW_LOCATION, typeof(string)))
				{
					return (Point)POINT_CONVERTER.ConvertFromInvariantString((string)OptionKey[IO_CHANNEL_WINDOW_LOCATION]);
				}
				return Point.Empty;
			}
			set
			{
				OptionKey[IO_CHANNEL_WINDOW_LOCATION]= (object)POINT_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static Size IOChannelWindowSize
		{
			get
			{
				if (OptionKey.HasValue(IO_CHANNEL_WINDOW_SIZE, typeof(string)))
				{
					return (Size)SIZE_CONVERTER.ConvertFromInvariantString((string)OptionKey[IO_CHANNEL_WINDOW_SIZE]);
				}
				return new Size(627, 600);
			}
			set
			{
				OptionKey[IO_CHANNEL_WINDOW_SIZE]= (object)SIZE_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static int IOChannelWindowContainerHeight
		{
			get
			{
				if (OptionKey.HasValue(IO_CHANNEL_WINDOW_CONTAINER_HEIGHT, typeof(int)))
				{
					return (int)OptionKey[IO_CHANNEL_WINDOW_CONTAINER_HEIGHT];
				}
				return 0;
			}
			set
			{
				OptionKey[IO_CHANNEL_WINDOW_CONTAINER_HEIGHT]= (object)value;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey(SUB_KEY);

		static UserOptionsHelper()
		{
			IO_CHANNEL_WINDOW_LOCATION = "IoChannelDeviceWindowLocation";
			IO_CHANNEL_WINDOW_SIZE = "IoChannelDeviceWindowSize";
			IO_CHANNEL_WINDOW_CONTAINER_HEIGHT = "IoChannelDeviceWindowContainerHeight";
			SUB_KEY = "{61CEEAAE-7543-4222-940B-0DD56AED0F71}";
			POINT_CONVERTER = TypeDescriptor.GetConverter(typeof(Point));
			SIZE_CONVERTER = TypeDescriptor.GetConverter(typeof(Size));
		}
	}
}
