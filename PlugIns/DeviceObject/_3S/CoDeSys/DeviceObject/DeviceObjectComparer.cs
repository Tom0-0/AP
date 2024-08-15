using System;
using System.Collections;
using System.IO;
using System.Text;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.ProjectCompare;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{3ED2487E-429E-40FC-9B13-E31D4921B631}")]
	public class DeviceObjectComparer : IObjectComparer
	{
		public bool AcceptsObjectType(Type objectType, Type[] embeddedObjectTypes)
		{
			if (objectType != null)
			{
				if (!typeof(IDeviceObject).IsAssignableFrom(objectType))
				{
					return typeof(IExplicitConnector).IsAssignableFrom(objectType);
				}
				return true;
			}
			return false;
		}

		public bool CheckEquality(IObject leftObject, IObject rightObject, bool bIgnoreWhitespace, bool bIgnoreComments, bool bIgnoreProperties)
		{
			IDeviceObjectComparerFactory factory = DeviceObjectCompareFactoryManager.Instance.GetFactory(leftObject, rightObject);
			if (factory != null)
			{
				return factory.CheckEquality(leftObject, rightObject, bIgnoreWhitespace, bIgnoreComments, bIgnoreProperties);
			}
			return DefaultCheckEquality(leftObject, rightObject, bIgnoreWhitespace, bIgnoreComments, bIgnoreProperties);
		}

		internal static bool DefaultCheckEquality(IObject leftObject, IObject rightObject, bool bIgnoreWhitespace, bool bIgnoreComments, bool bIgnoreProperties)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			if (leftObject == null && rightObject == null)
			{
				return true;
			}
			if (leftObject == null || rightObject == null)
			{
				return false;
			}
			ChunkedMemoryStream val = new ChunkedMemoryStream();
			ChunkedMemoryStream val2 = new ChunkedMemoryStream();
			try
			{
				BinaryWriter writer = new BinaryWriter((Stream)(object)val, Encoding.Unicode);
				BinaryWriter writer2 = new BinaryWriter((Stream)(object)val2, Encoding.Unicode);
				if (leftObject.MetaObject != null && rightObject.MetaObject != null)
				{
					ExportDevice(leftObject.MetaObject.Object, writer);
					ExportDevice(rightObject.MetaObject.Object, writer2);
				}
				else
				{
					ExportDevice(leftObject, writer);
					ExportDevice(rightObject, writer2);
				}
				if (((Stream)(object)val).Length != ((Stream)(object)val2).Length)
				{
					return false;
				}
				((Stream)(object)val).Position = 0L;
				((Stream)(object)val2).Position = 0L;
				int num;
				int num2;
				do
				{
					num = ((Stream)(object)val).ReadByte();
					num2 = ((Stream)(object)val2).ReadByte();
				}
				while (num == num2 && num != -1);
				return num - num2 == 0;
			}
			catch
			{
				return true;
			}
			finally
			{
				((Stream)(object)val).Close();
				((Stream)(object)val2).Close();
			}
		}

		internal static void ExportDevice(object obj, BinaryWriter writer)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			if (!(obj is IArchivable))
			{
				return;
			}
			IArchivable val = (IArchivable)obj;
			val.BeforeSerialize();
			string[] serializableValueNames = val.SerializableValueNames;
			if (obj is IAdapter || obj is IAdapterList || obj is ICommunicationSettings || obj is ICommunicationSettingsWithStorageLocation)
			{
				return;
			}
			string[] array = serializableValueNames;
			foreach (string text in array)
			{
				if (obj is IDeviceObject)
				{
					switch (text)
					{
					case "UniqueIdGenerator":
					case "UserManagement":
					case "RightsManagement":
					case "PlaceholderResolution":
						continue;
					}
				}
				object serializableValue = val.GetSerializableValue(text);
				if (serializableValue is ICollection)
				{
					bool flag = false;
					foreach (object item in serializableValue as ICollection)
					{
						if (item is IArchivable)
						{
							flag = true;
							ExportDevice(item, writer);
						}
					}
					if (flag)
					{
						continue;
					}
				}
				if (serializableValue is IArchivable)
				{
					ExportDevice(serializableValue, writer);
				}
				else
				{
					if (serializableValue == null)
					{
						continue;
					}
					if (serializableValue is Guid)
					{
						switch (text)
						{
						case "BusCycleTaskGuid":
						case "IoApp":
						case "GuidBusCycleTask":
							break;
						default:
							continue;
						}
					}
					switch (text)
					{
					case "Id":
						if (obj is VariableMapping)
						{
							continue;
						}
						break;
					case "PositionId":
					case "PositionIds":
					case "EditorPositionId":
					case "LogicalLanguageModelPositionId":
					case "IndexInDevDesc":
						continue;
					}
					writer.Write(text);
					if (serializableValue is ICollection)
					{
						foreach (object item2 in serializableValue as ICollection)
						{
							if (item2 != null)
							{
								writer.Write(item2.ToString().Replace("\r", "").Replace("\n", ""));
							}
						}
					}
					else
					{
						writer.Write(serializableValue.ToString().Replace("\r", "").Replace("\n", ""));
					}
					writer.Write('\r');
				}
			}
		}
	}
}
