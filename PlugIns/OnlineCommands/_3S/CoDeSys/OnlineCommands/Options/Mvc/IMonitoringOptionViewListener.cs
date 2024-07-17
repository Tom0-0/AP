namespace _3S.CoDeSys.OnlineCommands.Options.Mvc
{
    internal interface IMonitoringOptionViewListener
    {
        void Initialize(IMonitoringOptionModelListener modelListener);

        void DisplayModeChanged(int iDisplayMode);

        void DecimalPlacesChanged(int? iCountDecimalPlaces);

        bool Save(ref string stMessage, ref EEditorPosition eFailedControl);
    }
}
