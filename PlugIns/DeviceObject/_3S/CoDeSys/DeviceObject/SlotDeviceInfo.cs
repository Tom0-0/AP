using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class SlotDeviceInfo : IDeviceInfo
	{
		private IDeviceObject2 _device;

		private static EmptyFileReferenceCollection _emptyFileRefs = new EmptyFileReferenceCollection();

		public IDeviceObject2 DeviceObject
		{
			get
			{
				return _device;
			}
			set
			{
				_device = value;
			}
		}

		public IRepositoryFileReferenceCollection AdditionalFiles
		{
			get
			{
				if (_device == null)
				{
					return (IRepositoryFileReferenceCollection)(object)_emptyFileRefs;
				}
				return ((IDeviceObject)_device).DeviceInfo.AdditionalFiles;
			}
		}

		public Image Image
		{
			get
			{
				if (_device == null)
				{
					return null;
				}
				return ((IDeviceObject)_device).DeviceInfo.Image;
			}
		}

		public int[] Categories
		{
			get
			{
				if (_device == null)
				{
					return new int[0];
				}
				return ((IDeviceObject)_device).DeviceInfo.Categories;
			}
		}

		public string Vendor
		{
			get
			{
				if (_device == null)
				{
					return "";
				}
				return ((IDeviceObject)_device).DeviceInfo.Vendor;
			}
		}

		public string Description
		{
			get
			{
				if (_device == null)
				{
					return "";
				}
				return ((IDeviceObject)_device).DeviceInfo.Description;
			}
		}

		public string Custom
		{
			get
			{
				if (_device == null)
				{
					return null;
				}
				return ((IDeviceObject)_device).DeviceInfo.Custom;
			}
		}

		public string[] Families
		{
			get
			{
				if (_device == null)
				{
					return new string[0];
				}
				return ((IDeviceObject)_device).DeviceInfo.Families;
			}
		}

		public string Name
		{
			get
			{
				if (_device == null)
				{
					return Strings.EmptySlotName;
				}
				return ((IDeviceObject)_device).DeviceInfo.Name;
			}
		}

		public string OrderNumber
		{
			get
			{
				if (_device == null)
				{
					return "";
				}
				return ((IDeviceObject)_device).DeviceInfo.OrderNumber;
			}
		}

		public Icon Icon
		{
			get
			{
				if (_device == null)
				{
					if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("DeviceObject", "EmptySlotIcon"))
					{
						Icon icon = ((IEngine3)APEnvironment.Engine).OEMCustomization.GetValue("DeviceObject", "EmptySlotIcon") as Icon;
						if (icon != null)
						{
							return icon;
						}
					}
					return ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceObject.Resources.EmptySlot.ico");
				}
				Icon icon2 = null;
				if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("DeviceObject", "UsedSlotIcon"))
				{
					icon2 = ((IEngine3)APEnvironment.Engine).OEMCustomization.GetValue("DeviceObject", "UsedSlotIcon") as Icon;
				}
				if (icon2 == null)
				{
					icon2 = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceObject.Resources.UsedSlot.ico");
				}
				Icon icon3 = ((IDeviceObject)_device).DeviceInfo.Icon;
				if (icon3 == null)
				{
					return icon2;
				}
				return CombineIcons(icon2, icon3);
			}
		}

		private static Icon CombineIcons(Icon iconSlot, Icon iconDev)
		{
			using Icon icon = new Icon(iconDev, 16, 16);
			using Bitmap bitmap = new Bitmap(24, 24);
			Graphics graphics = Graphics.FromImage(bitmap);
			Rectangle targetRect = new Rectangle(8, 4, 16, 16);
			Rectangle targetRect2 = new Rectangle(0, 4, 16, 16);
			graphics.DrawIcon(iconSlot, targetRect2);
			graphics.DrawIcon(icon, targetRect);
			graphics.Dispose();
			Color pixel = bitmap.GetPixel(8, 4);
			if (pixel.A != 0)
			{
				bitmap.MakeTransparent(pixel);
			}
			return GraphicsHelper.IconFromBitmap(bitmap);
		}
	}
}
