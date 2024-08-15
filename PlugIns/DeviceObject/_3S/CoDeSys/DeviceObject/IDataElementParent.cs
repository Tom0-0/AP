namespace _3S.CoDeSys.DeviceObject
{
	internal interface IDataElementParent
	{
		IDataElement DataElement { get; }

		IIoProvider IoProvider { get; }

		bool IsParameter { get; }

		void Notify(IDataElement dataelement, string[] path);

		Parameter GetParameter();

		long GetBitOffset(IDataElement child);
	}
}
