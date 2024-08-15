using System.Drawing;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DeviceObjectDiffData
	{
		private int _nConnectionId;

		private IParameter _leftParameter;

		private IParameter _rightParameter;

		private IDataElement _leftElement;

		private IDataElement _rightElement;

		private IParameterSection _leftSection;

		private IParameterSection _rightSection;

		private Image _image;

		private string _stLeftValue = string.Empty;

		private string _stLeftDescription = string.Empty;

		private string _stLeftMapping = string.Empty;

		private string _stLeftAddress = string.Empty;

		public string LeftValue
		{
			get
			{
				return _stLeftValue;
			}
			set
			{
				_stLeftValue = value;
			}
		}

		public string LeftMapping
		{
			get
			{
				return _stLeftMapping;
			}
			set
			{
				_stLeftMapping = value;
			}
		}

		public string LeftAddress
		{
			get
			{
				return _stLeftAddress;
			}
			set
			{
				_stLeftAddress = value;
			}
		}

		public string LeftDescription
		{
			get
			{
				return _stLeftDescription;
			}
			set
			{
				_stLeftDescription = value;
			}
		}

		internal Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		internal IParameter LeftParameter => _leftParameter;

		internal IParameter RightParameter => _rightParameter;

		internal IDataElement LeftElement => _leftElement;

		internal IDataElement RightElement => _rightElement;

		internal int ConnectionId => _nConnectionId;

		internal IParameterSection LeftSection => _leftSection;

		internal IParameterSection RightSection => _rightSection;

		internal DeviceObjectDiffData(int nConnectionId, Parameter leftParameter, Parameter rightParameter, IDataElement leftElement, IDataElement rightElement)
		{
			_leftElement = leftElement;
			_rightElement = rightElement;
			_leftParameter = (IParameter)(object)leftParameter;
			_rightParameter = (IParameter)(object)rightParameter;
			_nConnectionId = nConnectionId;
		}

		internal DeviceObjectDiffData(IParameterSection leftSection, IParameterSection rightSection)
		{
			_leftSection = leftSection;
			_rightSection = rightSection;
		}
	}
}
