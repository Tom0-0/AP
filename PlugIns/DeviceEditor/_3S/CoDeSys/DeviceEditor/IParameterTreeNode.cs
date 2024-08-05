using System.Collections.Generic;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal interface IParameterTreeNode : ITreeTableNode2, ITreeTableNode
	{
		List<IParameterTreeNode> ChildNodes { get; }

		string DevPath { get; }

		DataElementNode Get(IParameter parameter, IDataElement dataelement, string[] path);

		int CompareTo(IParameterTreeNode otherNode, int sortColumn);
	}
}
