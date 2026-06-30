namespace HPTClient
{
    [Flags]
    public enum DataToShowUsage
    {
        All = 1024,
        Vxx = 1,
        Combination = 2,
        Trio = 4,
        Tvilling = 8,
        Double = 16,
        ComplementaryRule = 32,
        HorseList = 64,
        Correction = 128,
        None = 256,
        Everywhere = 512
    }

    public enum ReductionType
    {
        ABCD,
        Rank,
        RowValue,
        TrackSum,
        PercentSum,
        ShareSum,
        Driver,
        Trainer
    }

    public enum HPTPrio
    {
        M,  // Ej reducerad
        A,
        B,
        C,
        D,
        E,
        F
    }
}
