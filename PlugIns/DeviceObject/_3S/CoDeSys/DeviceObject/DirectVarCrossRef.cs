using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DirectVarCrossRef
	{
		private IDirectVariable _var;

		private IAddressCrossReference _cref;

		private IDataLocation _dataloc;

		private BitDataLocation _directbitlocation;

		public IDirectVariable DirectVariable => _var;

		public IAddressCrossReference CrossRef => _cref;

		public IDataLocation DataLocation => _dataloc;

		public BitDataLocation BitDataLocation => _directbitlocation;

		public DirectVarCrossRef(ICompileContext comcon, IDirectVariable var, IAddressCrossReference cref)
		{
			_var = var;
			_cref = cref;
			bool flag = default(bool);
			_dataloc = comcon.LocateAddress(ref flag, var);
			if (!flag)
			{
				_directbitlocation = new BitDataLocation(_dataloc);
			}
		}
	}
}
