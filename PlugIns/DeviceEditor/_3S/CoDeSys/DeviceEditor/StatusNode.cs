using System;
using System.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class StatusNode : ITreeTableNode
	{
		private StatusNode _parent;

		private StatusNode[] _children;

		private string _stName;

		private string _stDescription;

		private IOnlineVarRef2 _ovr;

		private ITreeTableModel _model;

		private IDataElement _elem;

		private IConverterToIEC _converterToIec;

		public IOnlineVarRef2 WatchedVariable => _ovr;

		private IDataElement DataElement => _elem;

		public int ChildCount => _children.Length;

		public bool HasChildren => _children.Length != 0;

		public ITreeTableNode Parent => (ITreeTableNode)(object)_parent;

		internal StatusNode(IDataElement elem, StatusNode parent, ITreeTableModel model, IConverterToIEC converter)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Invalid comparison between Unknown and I4
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Invalid comparison between Unknown and I4
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			_parent = parent;
			_stName = elem.VisibleName;
			_stDescription = elem.Description;
			_model = model;
			_elem = elem;
			_converterToIec = converter;
			if (elem.HasSubElements)
			{
				LList<StatusNode> val = new LList<StatusNode>();
				foreach (IDataElement6 item in (IEnumerable)elem.SubElements)
				{
					IDataElement6 val2 = item;
					AccessRight accessRight = val2.GetAccessRight(false);
					if ((int)accessRight == 3 || (int)accessRight == 1)
					{
						val.Add(new StatusNode((IDataElement)(object)val2, this, model, converter));
					}
				}
				_children = new StatusNode[val.Count];
				val.CopyTo(_children);
			}
			else
			{
				_children = new StatusNode[0];
			}
			try
			{
				IDataElement3 val3 = (IDataElement3)(object)((elem is IDataElement3) ? elem : null);
				if (val3 != null && ((IDataElement2)val3).CanWatch)
				{
					_ovr = val3.CreateWatch(false);
					((IOnlineVarRef)_ovr).Changed+=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
					((IOnlineVarRef)_ovr).SuspendMonitoring();
				}
			}
			catch (Exception)
			{
			}
		}

		public void ReleaseMonitoring()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			if (_ovr != null)
			{
				((IOnlineVarRef)_ovr).Release();
				((IOnlineVarRef)_ovr).Changed-=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
				_ovr = null;
			}
		}

		public void SetConverterToIec(IConverterToIEC converter)
		{
			_converterToIec = converter;
			StatusNode[] children = _children;
			for (int i = 0; i < children.Length; i++)
			{
				children[i].SetConverterToIec(converter);
			}
			RaiseChanged();
		}

		private void OnOnlineVarRefChanged(IOnlineVarRef varRef)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			if (varRef == _ovr && (int)varRef.State != 1)
			{
				RaiseChanged();
				if (_model is StatusTreeTableModel)
				{
					(_model as StatusTreeTableModel).AdjustColumnWidth();
				}
			}
		}

		private void RaiseChanged()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			int num = -1;
			if (_parent != null)
			{
				num = _parent.GetIndex((ITreeTableNode)(object)this);
			}
			try
			{
				_model.RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)_parent, num, (ITreeTableNode)(object)this));
			}
			catch
			{
			}
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return (ITreeTableNode)(object)_children[nIndex];
		}

		public int GetIndex(ITreeTableNode node)
		{
			return Array.IndexOf((ITreeTableNode[])(object)_children, node);
		}

		private object GetOnlineValue()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Invalid comparison between Unknown and I4
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (_ovr != null)
				{
					if ((int)((IOnlineVarRef)_ovr).State == 0)
					{
						if (DataElement.IsEnumeration && DataElement is IEnumerationDataElement)
						{
							int num = default(int);
							IEnumerationValue enumerationValue = ((IEnumerationDataElement)DataElement).GetEnumerationValue(((IOnlineVarRef)_ovr).Value, out num);
							if (enumerationValue != null)
							{
								return new RawValueData((TypeClass)29, enumerationValue.VisibleName, _converterToIec, bConstant: false, ((IOnlineVarRef)_ovr).Forced);
							}
						}
						if ((int)((IType)((IOnlineVarRef)_ovr).Expression.Type).Class == 26)
						{
							return new RawValueData(((IType)((IOnlineVarRef)_ovr).Expression.Type).Class, ((IType)((IOnlineVarRef)_ovr).Expression.Type.BaseType).Class, ((IOnlineVarRef)_ovr).Value, _converterToIec, bConstant: false, ((IOnlineVarRef)_ovr).Forced);
						}
						return new RawValueData(((IType)((IOnlineVarRef)_ovr).Expression.Type).Class, ((IOnlineVarRef)_ovr).Value, _converterToIec, bConstant: false, ((IOnlineVarRef)_ovr).Forced);
					}
					string text = ((IOnlineVarRef)_ovr).GetStateMessage();
					if (text == null)
					{
						text = string.Empty;
					}
					return new ErrorValueData(text);
				}
				return EmptyValueData.Empty;
			}
			catch (Exception ex)
			{
				return new ErrorValueData(ex.Message);
			}
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			switch (nColumnIndex)
			{
			case 0:
				return _stName;
			case 1:
				return GetOnlineValue();
			case 2:
				if (_elem.IsEnumeration && _elem is IEnumerationDataElement && _ovr != null && (int)((IOnlineVarRef)_ovr).State == 0)
				{
					int num = default(int);
					IEnumerationValue enumerationValue = ((IEnumerationDataElement)_elem).GetEnumerationValue(((IOnlineVarRef)_ovr).Value, out num);
					if (enumerationValue != null)
					{
						return enumerationValue.Description;
					}
				}
				return _stDescription;
			default:
				throw new IndexOutOfRangeException($"Invalid columnindex {nColumnIndex}");
			}
		}

		public bool IsEditable(int nColumnIndex)
		{
			return false;
		}

		public void SetValue(int nColumnIndex, object value)
		{
			throw new InvalidOperationException();
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
			StatusNode statusNode = _children[nIndex1];
			_children[nIndex1] = _children[nIndex2];
			_children[nIndex2] = statusNode;
		}
	}
}
