#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	internal static class Helper
	{
		private static readonly string EXPANDED_OBJECTS = "ExpandedObjects";

		private static readonly string COLUMNWIDTHS_OBJECTS = "ColumnWidths";

		private static readonly string SUB_KEY = "{3BA1B372-7386-4793-A483-00A301C89BFE}";

		private static IOptionKey GetOptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).CreateSubKey(SUB_KEY);

		internal static Image GetImage(IMetaObjectStub mos)
		{
			Debug.Assert(mos != null);
			IFrame frame = ((IEngine)APEnvironment.Engine).Frame;
			if (frame != null && frame is IFrame2)
			{
				IFrame2 val = (IFrame2)(object)((frame is IFrame2) ? frame : null);
				Image complexIconForObject = val.GetComplexIconForObject(mos, false);
				if (complexIconForObject != null)
				{
					return complexIconForObject;
				}
				complexIconForObject = val.GetComplexIconForObject(mos, true);
				if (complexIconForObject != null)
				{
					return complexIconForObject;
				}
			}
			Icon icon = GetIcon(mos);
			if (icon != null)
			{
				return Bitmap.FromHicon(icon.Handle);
			}
			return null;
		}

		internal static Icon GetIcon(IMetaObjectStub mos)
		{
			Debug.Assert(mos != null);
			Icon iconForObject = ((IEngine)APEnvironment.Engine).Frame.GetIconForObject(mos, false);
			if (iconForObject == null)
			{
				iconForObject = ((IEngine)APEnvironment.Engine).Frame.GetIconForObject(mos, true);
			}
			return iconForObject;
		}

		internal static string GetObjectName(IMetaObjectStub mos)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (typeof(IHasLocalizedDisplayName).IsAssignableFrom(mos.ObjectType))
			{
				return ((IHasLocalizedDisplayName)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mos.ProjectHandle, mos.ObjectGuid).Object).LocalizedDisplayName;
			}
			return mos.Name;
		}

		public static void RestoreColumnWidths(SimpleMappingTreeTableModel model)
		{
			LDictionary<int, int> val = new LDictionary<int, int>();
			try
			{
				val = GetColumnWidths();
			}
			catch
			{
			}
			int width = default(int);
			foreach (ColumnHeader column in model.View.Columns)
			{
				if (val.TryGetValue(model.MapColumn(column.Index), out width))
				{
					column.Width = width;
				}
				else
				{
					model.View.AdjustColumnWidth(column.Index, true);
				}
			}
		}

		public static void SaveColumdWidths(SimpleMappingTreeTableModel model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			ArrayList arrayList = new ArrayList();
			foreach (ColumnHeader column in model.View.Columns)
			{
				arrayList.Add(model.MapColumn(column.Index) + "," + column.Width);
			}
			GetOptionKey[COLUMNWIDTHS_OBJECTS]= (object)arrayList;
		}

		internal static LDictionary<int, int> GetColumnWidths()
		{
			LDictionary<int, int> val = new LDictionary<int, int>();
			if (GetOptionKey.HasValue(COLUMNWIDTHS_OBJECTS, typeof(ArrayList)))
			{
				foreach (string item in (ArrayList)GetOptionKey[COLUMNWIDTHS_OBJECTS])
				{
					string[] array = item.Split(',');
					if (array.Length == 2 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
					{
						val.Add(result, result2);
					}
				}
				return val;
			}
			return val;
		}

		public static LList<string> GetExpandedObjects()
		{
			if (GetOptionKey.HasValue(EXPANDED_OBJECTS, typeof(ArrayList)))
			{
				ArrayList obj = (ArrayList)GetOptionKey[EXPANDED_OBJECTS];
				LList<string> val = new LList<string>();
				{
					foreach (string item in obj)
					{
						val.Add(item);
					}
					return val;
				}
			}
			return new LList<string>();
		}

		public static void SetExpandedObjects(LList<string> liExpanded)
		{
			if (liExpanded == null)
			{
				throw new ArgumentNullException("liExpanded");
			}
			ArrayList arrayList = new ArrayList((ICollection)liExpanded);
			GetOptionKey[EXPANDED_OBJECTS]= (object)arrayList;
		}
	}
}
