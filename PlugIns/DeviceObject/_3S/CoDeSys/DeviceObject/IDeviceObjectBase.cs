using System;
using System.Xml;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.DeviceObject
{
	public interface IDeviceObjectBase : IDeviceObject16, IDeviceObject15, IDeviceObject14, IDeviceObject13, IDeviceObject12, IDeviceObject11, IDeviceObject10, IDeviceObject9, IDeviceObject8, IDeviceObject7, IDeviceObject6, IDeviceObject5, IDeviceObject4, IDeviceObject3, IDeviceObject2, IDeviceObject, IObject, IGenericObject, IArchivable, ICloneable, IComparable, ILanguageModelProvider3, ILanguageModelProvider2, ILanguageModelProvider, ILanguageModelProviderWithDependencies, IOrderedSubObjects, IHasAssociatedOnlineHelpTopic, IKnowMyOrderedSubObjectsInAdvance
	{
		XmlNode GetFunctionalSection();

		bool CheckLanguageModelGuids();

		void UpdateLanguageModelGuids(bool bUpgrade);

		void OnAfterAdded();

		void OnAfterCreated();

		void OnRenamed(string stOldDeviceName);

		void AddLateLanguageModel(int nProjectHandle, AddLanguageModelEventArgs e);

		IMetaObject GetMetaObject();

		void PreparePaste(bool bOnlyChildConnectors = false);

		void UpdatePasteModuleGuid(Guid oldGuid, Guid newGuid);
	}
}
