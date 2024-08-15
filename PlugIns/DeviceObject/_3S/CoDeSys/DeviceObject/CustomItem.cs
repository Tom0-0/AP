using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{dd6be685-9be1-40f4-855e-c5c0352a5200}")]
	[StorageVersion("3.3.0.0")]
	public class CustomItem : GenericObject2, ICustomItem
	{
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("Name")]
		[StorageVersion("3.3.0.0")]
		private string _stName = "";

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("Data")]
		[StorageVersion("3.3.0.0")]
		private string _stData = "";

		public string Data => _stData;

		public string Name => _stName;

		public CustomItem()
			: this()
		{
		}

		internal CustomItem(CustomItem original)
			: this()
		{
			_stName = original._stName;
			_stData = original._stData;
		}

		public override object Clone()
		{
			CustomItem customItem = new CustomItem(this);
			((GenericObject)customItem).AfterClone();
			return customItem;
		}

		public CustomItem(XmlElement xeNode)
			: this()
		{
			_stName = xeNode.Name;
			_stData = xeNode.OuterXml;
		}

		public CustomItem(string stData)
			: this()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(stData);
			_stName = xmlDocument.DocumentElement.Name;
			_stData = xmlDocument.DocumentElement.OuterXml;
		}
	}
}
