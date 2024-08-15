using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{8a62c05c-4fee-4b4a-8600-a677aedc92c7}")]
	[StorageVersion("3.3.0.0")]
	public class VariableMappingCollection : GenericObject2, IVariableMappingCollection, ICollection, IEnumerable
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Mappings")]
		[StorageVersion("3.3.0.0")]
		[StorageSaveAsNonGenericCollection("3.3.0.0-3.5.0.255")]
		private LList<VariableMapping> _alMappings = new LList<VariableMapping>(1);

		private IDataElementParent _parent;

		internal IDataElementParent Parent
		{
			set
			{
				_parent = value;
				foreach (VariableMapping alMapping in _alMappings)
				{
					alMapping.Parent = _parent;
				}
			}
		}

		public IVariableMapping this[int nIndex] => (IVariableMapping)(object)_alMappings[nIndex];

		public bool IsSynchronized => false;

		public int Count => _alMappings.Count;

		public object SyncRoot => ((ICollection)_alMappings).SyncRoot;

		public VariableMappingCollection()
			: base()
		{
		}

		private VariableMappingCollection(VariableMappingCollection original)
			: this()
		{
			_alMappings = new LList<VariableMapping>(original._alMappings.Count);
			foreach (VariableMapping alMapping in original._alMappings)
			{
				_alMappings.Add((VariableMapping)((GenericObject)alMapping).Clone());
			}
		}

		public override object Clone()
		{
			VariableMappingCollection variableMappingCollection = new VariableMappingCollection(this);
			((GenericObject)variableMappingCollection).AfterClone();
			return variableMappingCollection;
		}

		public IVariableMapping AddMapping(string stVariable, bool bCreateVariable)
		{
			VariableMapping variableMapping = new VariableMapping(-1L, stVariable, bCreateVariable);
			_alMappings.Add(variableMapping);
			variableMapping.Parent = _parent;
			if (_parent != null)
			{
				variableMapping.IoProvider = _parent.IoProvider;
			}
			Notify();
			return (IVariableMapping)(object)variableMapping;
		}

		public void Remove(IVariableMapping mapping)
		{
			Remove(mapping.Id);
		}

		public void Remove(long lId)
		{
			for (int i = 0; i < _alMappings.Count; i++)
			{
				VariableMapping variableMapping = _alMappings[i];
				if (variableMapping.Id == lId)
				{
					variableMapping.Parent = null;
					_alMappings.RemoveAt(i);
					break;
				}
			}
			Notify();
		}

		public void RemoveAt(int nIndex)
		{
			_alMappings[nIndex].Parent = null;
			_alMappings.RemoveAt(nIndex);
			Notify();
		}

		public IVariableMapping GetById(long lId)
		{
			foreach (VariableMapping alMapping in _alMappings)
			{
				if (((IVariableMapping)alMapping).Id == lId)
				{
					return (IVariableMapping)(object)alMapping;
				}
			}
			return null;
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_alMappings).CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alMappings.GetEnumerator();
		}

		private void Notify()
		{
			if (_parent != null)
			{
				_parent.Notify(_parent.DataElement, new string[0]);
			}
		}
	}
}
