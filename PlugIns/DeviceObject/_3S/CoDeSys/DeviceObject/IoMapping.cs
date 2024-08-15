using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{d6d934cf-5ec0-42c3-b628-2a7aea7d364c}")]
	[StorageVersion("3.3.0.0")]
	public class IoMapping : GenericObject2, IIoMapping
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AutomaticAddress")]
		[StorageVersion("3.3.0.0")]
		private bool _bAutomaticIecAddress = true;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("IecAddress")]
		[StorageVersion("3.3.0.0")]
		private string _stIecAddress = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Mappings")]
		[StorageVersion("3.3.0.0")]
		private VariableMappingCollection _mappings = new VariableMappingCollection();

		private IDirectVariable _intermediateAddress;

		private IIoProvider _ioProvider;

		private IDataElementParent _parent;

		internal IDataElementParent Parent
		{
			set
			{
				_parent = value;
				_mappings.Parent = _parent;
			}
		}

		public string IecAddress
		{
			get
			{
				if (_ioProvider == null)
				{
					return string.Empty;
				}
				if (AutomaticIecAddress)
				{
					IAddressAssignmentStrategy strategy = _ioProvider.Strategy;
					if (_intermediateAddress == null)
					{
						strategy.UpdateAddresses(_ioProvider);
					}
					if (strategy is FlatAdressAssignmentStrategy)
					{
						if (DeviceObjectHelper.HashIecAddresses == null || DeviceObjectHelper.HashIecAddresses.Count == 0)
						{
							DeviceObjectHelper.FillIecAddresstable(_ioProvider.GetHost());
						}
						return ((object)(strategy as FlatAdressAssignmentStrategy).ResolveAddress(_intermediateAddress, _ioProvider, DeviceObjectHelper.HashIecAddresses)).ToString();
					}
					return ((object)strategy.ResolveAddress(_intermediateAddress, _ioProvider)).ToString();
				}
				return _stIecAddress;
			}
			set
			{
				if (AutomaticIecAddress)
				{
					throw new InvalidOperationException("Cannot set IEC address when automatic address assignment is enabled.");
				}
				if (_stIecAddress != value)
				{
					_stIecAddress = value;
					Notify();
				}
			}
		}

		public IVariableMappingCollection VariableMappings => (IVariableMappingCollection)(object)_mappings;

		public bool AutomaticIecAddress
		{
			get
			{
				return _bAutomaticIecAddress;
			}
			set
			{
				_bAutomaticIecAddress = value;
			}
		}

		internal IDirectVariable IntermediateAddress => _intermediateAddress;

		internal IIoProvider IoProvider => _ioProvider;

		internal IAddressAssignmentStrategy Strategy
		{
			get
			{
				if (_ioProvider != null)
				{
					return _ioProvider.Strategy;
				}
				return null;
			}
		}

		public IoMapping()
			: base()
		{
		}

		private IoMapping(IoMapping original)
			: this()
		{
			_bAutomaticIecAddress = original._bAutomaticIecAddress;
			_stIecAddress = original._stIecAddress;
			_mappings = (VariableMappingCollection)((GenericObject)original._mappings).Clone();
		}

		public override object Clone()
		{
			IoMapping ioMapping = new IoMapping(this);
			((GenericObject)ioMapping).AfterClone();
			return ioMapping;
		}

		public void SetIntermediateAddress(IDirectVariable addr)
		{
			_intermediateAddress = addr;
		}

		public IDirectVariable GetIecAddress(Hashtable htStartAddresses)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			IAddressAssignmentStrategy strategy = _ioProvider.Strategy;
			if (htStartAddresses != null && strategy is FlatAdressAssignmentStrategy && _intermediateAddress != null && htStartAddresses.ContainsKey(IoProvider))
			{
				_ = htStartAddresses[IoProvider];
				if (AutomaticIecAddress)
				{
					_=_intermediateAddress.Location;
					_ = 1;
					return (strategy as FlatAdressAssignmentStrategy).ResolveAddress(_intermediateAddress, _ioProvider, htStartAddresses);
				}
				return (IDirectVariable)(object)FlatAdressAssignmentStrategy.ParseIecAddress(_stIecAddress);
			}
			return (IDirectVariable)(object)FlatAdressAssignmentStrategy.ParseIecAddress(IecAddress);
		}

		internal void Notify()
		{
			_parent.Notify(_parent.DataElement, new string[0]);
		}

		internal void SetIoProvider(IIoProvider ioProvider)
		{
			_ioProvider = ioProvider;
			if (_mappings == null)
			{
				return;
			}
			foreach (VariableMapping mapping in _mappings)
			{
				mapping.IoProvider = ioProvider;
			}
		}
	}
}
