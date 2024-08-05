using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class InstanceIconLabelTreeTableViewCellData : IconLabelTreeTableViewCellData
	{
		private IFbInstance _instance;

		public IFbInstance FbInstance => _instance;

		public InstanceIconLabelTreeTableViewCellData(Image image, object label, IFbInstance instance)
			: base(image, label)
		{
			_instance = instance;
		}
	}
}
