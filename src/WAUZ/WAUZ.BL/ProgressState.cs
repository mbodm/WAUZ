namespace WAUZ.BL
{
    public enum ProgressState
    {
        Started,
        UnzipAddon,
        UnzippedAddon,
        ClearDestFolder,
        ClearedDestFolder,
        MoveFromTempToDest,
        MovedFromTempToDest,
        Finished,
    }
}
