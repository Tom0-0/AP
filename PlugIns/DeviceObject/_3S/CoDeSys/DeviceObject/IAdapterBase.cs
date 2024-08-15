using System;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public interface IAdapterBase : IAdapter, IGenericObject, IArchivable, ICloneable, IComparable
	{
		int SubObjectsCount { get; }

		bool CanAddModule(int nInsertPosition, IObject childDevice);

		void AddModule(int nInsertPosition, IMetaObjectStub mosChild);

		void PreparePaste();

		void UpdatePasteModuleGuid(Guid guidOld, Guid guidNew);

		bool SetExpectedModule(Guid guidModule);

		bool IsExpectedModule(Guid guidModule);

		void UpdateModules(LList<object> alDevices, IConnector7 conUpdate);

		void UpdateModules(LList<object> alDevices, IConnector7 conUpdate, bool bOnlyEmptyAdapter);

		void ClearModules();
	}
}
