namespace _3S.CoDeSys.OnlineCommands.Options.Mvc
{
    internal sealed class MonitoringOptionModel
    {
        private int _iDisplayMode;

        private int _iCountDecimalPlaces;

        internal int DisplayMode
        {
            get
            {
                return _iDisplayMode;
            }
            set
            {
                _iDisplayMode = value;
            }
        }

        internal int CountDecimalPlaces
        {
            get
            {
                return _iCountDecimalPlaces;
            }
            set
            {
                _iCountDecimalPlaces = value;
            }
        }
    }
}
