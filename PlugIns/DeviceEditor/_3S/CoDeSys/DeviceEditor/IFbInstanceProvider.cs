using System.Collections;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal interface IFbInstanceProvider : IEnumerable
	{
		IFbInstance GetAt(int nIndex, bool bToModify);
	}
}
