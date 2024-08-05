using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	public interface ITaskNode : ITreeTableNode2, ITreeTableNode
	{
		LList<ITreeTableNode> Children { get; }
	}
}
