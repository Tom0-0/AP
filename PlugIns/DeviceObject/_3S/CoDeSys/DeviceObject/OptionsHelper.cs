using System;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class OptionsHelper
	{
		public static readonly string COMMUNICATION_SETTINGS_KEY = "ShowGenericConfiguration";

		public static readonly string SUB_KEY = "{874995d7-28a0-458c-b5e4-211fab86c29e}";

		public static readonly string CREATE_CROSS_REFERENCES = "CreateCrossReferences";

		public static readonly string SUB_KEY_DEVICEEDITOR = "{5F370A46-A40D-41dd-9B9E-8094E2158F4C}";

		public static bool CreateCrossReferences
		{
			get
			{
				if (OptionKey.HasValue(CREATE_CROSS_REFERENCES, typeof(bool)))
				{
					return (bool)OptionKey[CREATE_CROSS_REFERENCES];
				}
				return false;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey(SUB_KEY_DEVICEEDITOR);

		public static void SetCommunicationSettings(Guid guidDeviceObject, CommunicationSettingsBase settings)
		{
			GetCommunicationSettingsKey(bCreate: true)[guidDeviceObject.ToString()]= (object)settings;
		}

		public static CommunicationSettingsBase GetCommunicationSettings(Guid guidDeviceObject)
		{
			IOptionKey communicationSettingsKey = GetCommunicationSettingsKey(bCreate: false);
			string text = guidDeviceObject.ToString();
			if (communicationSettingsKey != null && communicationSettingsKey.HasValue(text, typeof(CommunicationSettingsBase)))
			{
				return (CommunicationSettingsBase)communicationSettingsKey[text];
			}
			return null;
		}

		private static IOptionKey GetCommunicationSettingsKey(bool bCreate)
		{
			IOptionKey userProjectOptionKey = GetUserProjectOptionKey(bCreate);
			if (bCreate)
			{
				return userProjectOptionKey.CreateSubKey(COMMUNICATION_SETTINGS_KEY);
			}
			if (userProjectOptionKey != null)
			{
				return userProjectOptionKey.OpenSubKey(COMMUNICATION_SETTINGS_KEY);
			}
			return null;
		}

		private static IOptionKey GetUserProjectOptionKey(bool bCreate)
		{
			if (bCreate)
			{
				return APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).CreateSubKey(SUB_KEY);
			}
			return APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).OpenSubKey(SUB_KEY);
		}
	}
}
