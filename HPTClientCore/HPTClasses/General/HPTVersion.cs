namespace HPTClient
{
    internal class HPTVersion
    {
        static HPTVersion oldestAllowedVersion;
        static HPTVersion OldestAllowedVersion
        {
            get
            {
                if (oldestAllowedVersion == null)
                {
                    oldestAllowedVersion = new HPTVersion();
                    oldestAllowedVersion.Beta = false;
                    oldestAllowedVersion.BetaVersion = 0;
                    oldestAllowedVersion.Version = 3.63M;
                }
                return oldestAllowedVersion;
            }
        }
        public decimal Version { get; set; }

        public bool Beta { get; set; }

        public int BetaVersion { get; set; }

        public static bool operator ==(HPTVersion hv1, HPTVersion hv2)
        {
            return hv1.Version == hv2.Version && hv1.Beta == hv2.Beta && hv1.BetaVersion == hv2.BetaVersion;
        }

        public static bool operator !=(HPTVersion hv1, HPTVersion hv2)
        {
            return !(hv1 == hv2);
        }

        public virtual int CompareTo(HPTVersion hv)
        {
            if (hv.Version != Version)
            {
                return Version - hv.Version > 0 ? 1 : -1;
            }
            if (Beta != hv.Beta)
            {
                return hv.Beta ? 1 : -1;
            }
            if (hv.BetaVersion != BetaVersion)
            {
                return BetaVersion - hv.BetaVersion;
            }
            return 0;
        }
    }
}
