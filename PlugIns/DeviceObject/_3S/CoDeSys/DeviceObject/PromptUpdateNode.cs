using System;
using System.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class PromptUpdateNode : ITreeTableNode
	{
		private PromptUpdateModel _model;

		private UpdateInformation _updateInformation;

		private string _stDevName;

		private DeviceVersionInformation _currentVersion;

		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => null;

		internal PromptUpdateNode(PromptUpdateModel model, UpdateInformation updateInformation)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			_model = model;
			if (updateInformation == null)
			{
				throw new ArgumentNullException("updateInformation");
			}
			_updateInformation = updateInformation;
			_stDevName = updateInformation.DevInfo.Name;
			_currentVersion = updateInformation.CurrentVersion;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			throw new ArgumentOutOfRangeException("nIndex");
		}

		public int GetIndex(ITreeTableNode node)
		{
			return -1;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			switch (nColumnIndex)
			{
			case 0:
				return _stDevName;
			case 1:
				return _currentVersion.DeviceVersion;
			case 2:
				if (_updateInformation.AvailableUpdateVersions.Length != 0)
				{
					return _updateInformation.AvailableUpdateVersions[0].DeviceVersion;
				}
				return string.Empty;
			case 3:
				return _updateInformation.SelectedUpdateVersion?.NewConfigurationFormat ?? false;
			case 4:
			{
				LList<string> val = new LList<string>();
				int num = 0;
				val.Add(Strings.DoNotUpdate + "            ");
				for (int i = 0; i < _updateInformation.AvailableUpdateVersions.Length; i++)
				{
					DeviceVersionInformation deviceVersionInformation = _updateInformation.AvailableUpdateVersions[i];
					val.Add(string.Format(Strings.UpdateToVersion, deviceVersionInformation.DeviceVersion));
					if (deviceVersionInformation == _updateInformation.SelectedUpdateVersion)
					{
						num = i + 1;
					}
				}
				return (object)new FixedChoiceCellTreeTableViewCellData(num, (ICollection)val);
			}
			default:
				throw new ArgumentOutOfRangeException("nColumnIndex");
			}
		}

		public bool IsEditable(int nColumnIndex)
		{
			return nColumnIndex == 4;
		}

		public void SetValue(int nColumnIndex, object value)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			if (nColumnIndex != 4)
			{
				throw new InvalidOperationException("This cell is not editable.");
			}
			FixedChoiceCellTreeTableViewCellData val = (FixedChoiceCellTreeTableViewCellData)((value is FixedChoiceCellTreeTableViewCellData) ? value : null);
			if (val == null)
			{
				throw new ArgumentException("value");
			}
			if (val.Selection == 0 || val.Selection > val.Items.Count)
			{
				_updateInformation.SelectedUpdateVersion = null;
			}
			else
			{
				_updateInformation.SelectedUpdateVersion = _updateInformation.AvailableUpdateVersions[val.Selection - 1];
			}
			((AbstractTreeTableModel)_model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, -1, (ITreeTableNode)(object)this));
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
			throw new ArgumentOutOfRangeException("nIndex1");
		}

		public bool SetNewestVersion()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if (_updateInformation.AvailableUpdateVersions.Length != 0 && _updateInformation.SelectedUpdateVersion != _updateInformation.AvailableUpdateVersions[0])
			{
				_updateInformation.SelectedUpdateVersion = _updateInformation.AvailableUpdateVersions[0];
				((AbstractTreeTableModel)_model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, -1, (ITreeTableNode)(object)this));
				return true;
			}
			return false;
		}
	}
}
