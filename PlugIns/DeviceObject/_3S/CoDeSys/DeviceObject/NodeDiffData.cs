using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	internal class NodeDiffData
	{
		private DiffState _diffState;

		private AcceptState _acceptState;

		internal DiffState DiffState
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _diffState;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_diffState = value;
			}
		}

		internal AcceptState AcceptState
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _acceptState;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_acceptState = value;
			}
		}

		public NodeDiffData(DiffState diffState)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			_diffState = diffState;
		}
	}
}
