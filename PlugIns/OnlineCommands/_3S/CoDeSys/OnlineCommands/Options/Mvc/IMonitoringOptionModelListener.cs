namespace _3S.CoDeSys.OnlineCommands.Options.Mvc
{
    internal interface IMonitoringOptionModelListener
    {
        void ShowError(EEditorPosition editorPosition, string stErrorMsg);

        void ShowIntegerDisplayMode(int iDisplayMode);

        void ShowDecimalPlaces(int? iCountDecimalPlaces);

        void SetDecimalPlacesMinMax(int iMin, int iMax);
    }
}
