namespace _3S.CoDeSys.OnlineCommands.Options.Mvc
{
    internal sealed class MonitoringOptionController : IMonitoringOptionViewListener
    {
        private const int MAX_DECIMAL_PLACES = 20;

        private IMonitoringOptionModelListener _modelListener;

        private MonitoringOptionModel _model;

        private bool ValidateDecimalPlaces()
        {
            bool flag = false;
            string stErrorMsg = string.Empty;
            if (-1 == _model.CountDecimalPlaces)
            {
                flag = true;
            }
            else
            {
                flag = _model.CountDecimalPlaces <= 20;
                if (!flag)
                {
                    stErrorMsg = string.Format(Strings.TooManyDecimalPlaces, 20);
                }
            }
            _modelListener.ShowError(EEditorPosition.DecimalPlaces, stErrorMsg);
            return flag;
        }

        public void Initialize(IMonitoringOptionModelListener modelListener)
        {
            _modelListener = modelListener;
            _model = new MonitoringOptionModel();
            _model.DisplayMode = OptionsHelper.DisplayMode;
            _model.CountDecimalPlaces = OptionsHelper.DecimalPlaces;
            int? iCountDecimalPlaces = null;
            if (-1 != _model.CountDecimalPlaces)
            {
                iCountDecimalPlaces = _model.CountDecimalPlaces;
            }
            _modelListener.ShowIntegerDisplayMode(_model.DisplayMode);
            _modelListener.SetDecimalPlacesMinMax(0, 20);
            _modelListener.ShowDecimalPlaces(iCountDecimalPlaces);
        }

        public void DisplayModeChanged(int iDisplayMode)
        {
            _model.DisplayMode = iDisplayMode;
        }

        public void DecimalPlacesChanged(int? iCountDecimalPlaces)
        {
            if (iCountDecimalPlaces.HasValue)
            {
                _model.CountDecimalPlaces = iCountDecimalPlaces.Value;
            }
            else
            {
                _model.CountDecimalPlaces = -1;
            }
            ValidateDecimalPlaces();
        }

        public bool Save(ref string stMessage, ref EEditorPosition eFailedControl)
        {
            bool flag = ValidateDecimalPlaces();
            eFailedControl = ((!flag) ? EEditorPosition.DecimalPlaces : EEditorPosition.Undefined);
            if (flag)
            {
                OptionsHelper.DisplayMode = _model.DisplayMode;
                OptionsHelper.DecimalPlaces = _model.CountDecimalPlaces;
            }
            return flag;
        }
    }
}
