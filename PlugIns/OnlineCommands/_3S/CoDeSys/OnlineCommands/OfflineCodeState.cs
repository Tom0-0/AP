namespace _3S.CoDeSys.OnlineCommands
{
    internal enum OfflineCodeState : byte
    {
        NO_CODE_GENERATED,
        CODE_UP_TO_DATE,
        ONLINECHANGE_POSSIBLE,
        DOWNLOAD_NEEDED,
        UNKNOWN
    }
}
