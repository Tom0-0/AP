using System;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ImplicitObject
	{
		private IObjectFactory _factory;

		private IObject _object;

		private ChildObject _description;

		private Guid _DeviceGuid;

		private bool _bNewlyCreated;

		internal bool NewlyCreated
		{
			get
			{
				return _bNewlyCreated;
			}
			set
			{
				_bNewlyCreated = value;
			}
		}

		internal IObjectFactory Factory => _factory;

		internal IObject Object => _object;

		internal ChildObject Description => _description;

		internal Guid DeviceGuid
		{
			get
			{
				return _DeviceGuid;
			}
			set
			{
				_DeviceGuid = value;
			}
		}

		internal ImplicitObject(IObjectFactory factory, IObject obj, ChildObject description)
		{
			_factory = factory;
			_object = obj;
			_description = description;
		}
	}
}
