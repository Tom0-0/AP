using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{85B118F6-621A-4225-9772-94579DCE517F}")]
	internal class DeviceObjectAttributeProvider : IAttributeProvider
	{
		private LList<IAttribute> _attributes = new LList<IAttribute>();

		public IEnumerable<IAttribute> ProvidedAttributes => (IEnumerable<IAttribute>)_attributes;

		public DeviceObjectAttributeProvider()
		{
			_attributes.Add((IAttribute)(object)new GenericAttribute(LanguageModelHelper.ATTRIBUTEIGNORE, Strings.AttributeIgnoreDescription, new Version(3, 0, 0, 0), (AttributeScope)3));
			_attributes.Add((IAttribute)(object)new GenericAttribute("io_function_block", Strings.AttributeIgnoreDescription, new Version(3, 5, 13, 0), (AttributeScope)3));
			_attributes.Add((IAttribute)(object)new GenericAttribute("io_function_block_mapping", Strings.AttributeIgnoreDescription, new Version(3, 5, 13, 0), (AttributeScope)3));
		}
	}
}
