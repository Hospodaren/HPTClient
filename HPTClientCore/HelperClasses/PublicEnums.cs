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
        Okänd = 0,
        Rikstoto = 1,
        Riksspel = 2,
        Rikssexan = 3,
        RiksDubbel = 4,
        Solvalla = 5,
        Åby = 6,
        Jägersro = 7,
        Axevalla = 8,
        Bergsåker = 9,
        Internationellt = 10,
        Boden = 11,
        Bollnäs = 12,
        Dannero = 13,
        Eskilstuna = 14,
        Färjestad = 15,
        Gävle = 16,
        Hagmyren = 17,
        Halmstad = 18,
        Kalmar = 19,
        ExtraC = 20,
        Lindesberg = 21,
        Mantorp = 22,
        Romme = 23,
        Rättvik = 24,
        Skellefteå = 25,
        Solänget = 26,
        //Umeå            = 27,
        Umåker = 27,
        Visby = 28,
        Åmål = 29,
        ExtraD = 30,
        Årjäng = 31,
        Örebro = 32,
        Östersund = 33,
        JägersroGalopp = 34,
        TäbyGalopp = 35,
        Arvika = 36,
        Hoting = 37,
        Lycksele = 38,
        Oviken = 39,
        ExtraE = 40,
        Strömsholm = 41,
        Blommeröd = 42,
        Vaggeryd = 43,
        Karlshamn = 44,
        GöteborgGalopp = 45,
        Tingsryd = 46,
        ATGJippo = 47,
        TäbyTrav = 48,
        ExtraJ = 49,
        Mariehamn = 50,

        // Internationellt
        Australien = 51,
        Belgien = 52,
        Canada = 53,
        Danmark = 54,
        Charlottenlund = 55,
        KöpenhamnGalopp = 56,
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
        Nederländerna = 76,
        Norge = 77,
        Bjerke = 78,
        Övrevoll = 79,
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
        Österrike = 94,
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
        Å = 6,
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
        Rä = 24,
        Sk = 25,
        Sä = 26,
        U = 27,
        Vi = 28,
        Åm = 29,
        Xd = 30,
        År = 31,
        Ö = 32,
        Ös = 33,
        Jä = 34,
        Tä = 35,    // Tg?
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
        Kg = 56,    // Köpenhamn Galopp
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
        Nl = 76,    // Nederländerna
        No = 77,    // Norge
        Bj = 78,    // Bjerke
        Öv = 79,    // Övrevoll
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
        At = 94,    // Österrike
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