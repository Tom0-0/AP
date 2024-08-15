using System.IO;
using System.Text;
using System.Xml;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	public class CustomNode : XmlElement
	{
		private string xml;

		public CustomNode(string xml)
			: base(string.Empty, "xml", string.Empty, new XmlDocument())
		{
			this.xml = xml;
		}

		public override void WriteTo(XmlWriter w)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			xmlDocument.DocumentElement.RemoveAttribute("xmlns");
			xmlDocument.WriteTo(w);
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new CustomNode(xml);
		}

		private string FormatXml(string sUnformattedXml)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sUnformattedXml);
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter w = new StringWriter(stringBuilder);
			XmlTextWriter xmlTextWriter = null;
			try
			{
				xmlTextWriter = new XmlTextWriter(w);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlDocument.WriteTo(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter?.Close();
			}
			return stringBuilder.ToString();
		}
	}
}
