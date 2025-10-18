namespace HPTClient
{
    public enum GUIProfile
    {
        Simple,
        Normal,
        Advanced,
        Custom
    }

    public enum ReservHandling
    {
        Own = 0,
        None = 1,
        MarksSelected = 2,
        MarksNotSelected = 3,
        OwnRankSelected = 4,
        OwnRankNotSelected = 5,
        RankMeanSelected = 6,
        RankMeanNotSelected = 7,
        OddsSelected = 8,
        OddsNotSelected = 9,
        NextRankSelected = 10,
        NextRankNotSelected = 11
    }

    public enum CouponCompression
    {
        Default,
        Fastest,
        LargestCouponSize,
        SingleRows,
        V6BetMultiplier
    }

    public enum BetTypeEnum
    {
        V3,
        V4,
        V5,
        V64,
        V65,
        V75,
        DD,
        LD,
        Trio,
        VP,
        Komb,
        Raket,
        Tvilling,
        GS75
    }

    public enum BetTypeCategory
    {
        None,
        V4,
        V5,
        V6X,
        V75,
        V86,
        Double,
        Trio,
        Twin,
        GS75,
        V85
    }

    public enum TrackNameEnum
    {
        Ok�nd = 0,
        Rikstoto = 1,
        Riksspel = 2,
        Rikssexan = 3,
        RiksDubbel = 4,
        Solvalla = 5,
        �by = 6,
        J�gersro = 7,
        Axevalla = 8,
        Bergs�ker = 9,
        Internationellt = 10,
        Boden = 11,
        Bolln�s = 12,
        Dannero = 13,
        Eskilstuna = 14,
        F�rjestad = 15,
        G�vle = 16,
        Hagmyren = 17,
        Halmstad = 18,
        Kalmar = 19,
        ExtraC = 20,
        Lindesberg = 21,
        Mantorp = 22,
        Romme = 23,
        R�ttvik = 24,
        Skellefte� = 25,
        Sol�nget = 26,
        //Ume�            = 27,
        Um�ker = 27,
        Visby = 28,
        �m�l = 29,
        ExtraD = 30,
        �rj�ng = 31,
        �rebro = 32,
        �stersund = 33,
        J�gersroGalopp = 34,
        T�byGalopp = 35,
        Arvika = 36,
        Hoting = 37,
        Lycksele = 38,
        Oviken = 39,
        ExtraE = 40,
        Str�msholm = 41,
        Blommer�d = 42,
        Vaggeryd = 43,
        Karlshamn = 44,
        G�teborgGalopp = 45,
        Tingsryd = 46,
        ATGJippo = 47,
        T�byTrav = 48,
        ExtraJ = 49,
        Mariehamn = 50,

        // Internationellt
        Australien = 51,
        Belgien = 52,
        Canada = 53,
        Danmark = 54,
        Charlottenlund = 55,
        K�penhamnGalopp = 56,
        UAE_Dubai = 57,
        Estland = 58,
        Finland = 59,
        Vermo = 60,
        StMikkeli = 61,
        Frankrike = 62,
        Vincennes = 63,
        CagnesSurMer = 64,
        Enghien = 65,
        Longchamps = 66,
        England = 67,
        HongKong = 68,
        Irland = 69,
        Italien = 70,
        Milano = 71,
        Neapel = 72,
        Rom = 73,
        NyaZeeland = 74,
        Japan = 75,
        Nederl�nderna = 76,
        Norge = 77,
        Bjerke = 78,
        �vrevoll = 79,
        Momarken = 80,
        Forus = 81,
        Jarlsberg = 82,
        Polen = 83,
        Ryssland = 84,
        Schweiz = 85,
        Tyskland = 86,
        Gelsenkirchen = 87,
        Berlin = 88,
        Munchen = 89,
        Ungern = 90,
        USA = 91,
        Meadowlands = 92,
        PompanoPark = 93,
        �sterrike = 94,
        Spanien = 95,
        Tjeckien = 96,
        Malta = 97,
        Slovenien = 98,
        BreedersCrown = 99
    }

    public enum TrackCodeEnum
    {
        NN = 0,
        Rt = 1,
        Rs = 2,
        Rx = 3,
        Rd = 4,
        S = 5,
        � = 6,
        J = 7,
        Ax = 8,
        B = 9,
        Is = 10,
        Bo = 11,
        Bs = 12,
        D = 13,
        E = 14,
        F = 15,
        G = 16,
        H = 17,
        Hd = 18,
        Kr = 19,
        Xc = 20,
        L = 21,
        Mp = 22,
        Ro = 23,
        R� = 24,
        Sk = 25,
        S� = 26,
        U = 27,
        Vi = 28,
        �m = 29,
        Xd = 30,
        �r = 31,
        � = 32,
        �s = 33,
        J� = 34,
        T� = 35,    // Tg?
        Ar = 36,
        Hg = 37,
        Ly = 38,
        Ov = 39,
        Xe = 40,
        St = 41,    // Sg?
        Bl = 42,
        Vg = 43,
        Kh = 44,
        Gg = 45,
        Ti = 46,
        Ji = 47,
        Tt = 48,
        Xj = 49,
        Ma = 50,

        // Internationella
        Au = 51,    // Australien
        Be = 52,    // Belgien
        Ca = 53,    // Canada
        Dk = 54,    // Danmark
        Ch = 55,    // Charlottenlund
        Kg = 56,    // K�penhamn Galopp
        Ae = 57,    // UAE - Dubai
        Ee = 58,    // Estland
        Fi = 59,    // Finland
        Ve = 60,    // Vermo
        Mk = 61,    // St Mikkeli
        Fr = 62,    // Frankrike
        V = 63,     // Vincennes
        Cm = 64,    // Cagnes sur Mer
        En = 65,    // Enghien
        Lo = 66,    // Longchamps
        Gb = 67,    // England
        Hk = 68,    // Hong Kong
        Ie = 69,    // Irland
        It = 70,    // Italien
        Mi = 71,    // Milano
        Ne = 72,    // Neapel
        Rm = 73,    // Rom
        Nz = 74,    // Nya Zeeland
        Jp = 75,    // Japan
        Nl = 76,    // Nederl�nderna
        No = 77,    // Norge
        Bj = 78,    // Bjerke
        �v = 79,    // �vrevoll
        Mo = 80,    // Momarken
        Fs = 81,    // Forus
        Ja = 82,    // Jarlsberg
        Pl = 83,    // Polen
        Ru = 84,    // Ryssland
        Sz = 85,    // Schweiz
        De = 86,    // Tyskland
        Ge = 87,    // Gelsenkirchen
        Bn = 88,    // Berlin
        Mu = 89,    // Munchen
        Hu = 90,    // Ungern
        Us = 91,    // USA
        Me = 92,    // Meadowlands
        Pp = 93,    // Pompano Park
        At = 94,    // �sterrike
        Es = 95,    // Spanien
        Cz = 96,    // Tjeckien
        Mt = 97,    // Malta
        Si = 98,    // Slovenien
        Bc = 99     // Breeders Crown
    }

    public enum ABCDPriorityEnum
    {
        EJ = 0,
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6
    }

    public enum DragDropTypeEnabled
    {
        None = 0,
        RankOwn = 1,
        RankAlternate = 2
    }

    public enum RankTypeEnum
    {
        ATGTurordning,
        StreckProcent,  // OBSOLET
        Streckbarhet,
        Vinnarodds,
        EgenRank,
        Aftonbladet,
        Travpunkten
    }

    public enum PersonReductionType
    {
        Driver,
        Trainer,
        Owner,
        Breeder
    }
}