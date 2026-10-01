namespace HPTClient;

/// <summary>
/// Represents an HPT application version with support for beta releases.
/// </summary>
internal class HPTVersion : IEquatable<HPTVersion>
{
    /// <summary>
    /// Gets the oldest version of HPT data files still supported.
    /// </summary>
    static HPTVersion OldestAllowedVersion
    {
        get
        {
            if (field is null)
            {
                field = new HPTVersion
                {
                    Beta = false,
                    BetaVersion = 0,
                    Version = 3.63m
                };
            }
            return field;
        }
    }

    /// <summary>
    /// Gets or sets the main version number.
    /// </summary>
    public decimal Version { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a beta release.
    /// </summary>
    public bool Beta { get; set; }

    /// <summary>
    /// Gets or sets the beta version number (only meaningful when Beta is true).
    /// </summary>
    public int BetaVersion { get; set; }

    public static bool operator ==(HPTVersion? hv1, HPTVersion? hv2)
    {
        return hv1 is not null && hv2 is not null
            && hv1.Version == hv2.Version
            && hv1.Beta == hv2.Beta
            && hv1.BetaVersion == hv2.BetaVersion;
    }

    public static bool operator !=(HPTVersion? hv1, HPTVersion? hv2)
    {
        return !(hv1 == hv2);
    }

    public virtual int CompareTo(HPTVersion? hv)
    {
        if (hv is null)
            return 1;

        if (Version != hv.Version)
            return Version > hv.Version ? 1 : -1;

        if (Beta != hv.Beta)
            return hv.Beta ? 1 : -1;

        return BetaVersion.CompareTo(hv.BetaVersion);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as HPTVersion);
    }

    public bool Equals(HPTVersion? other)
    {
        return other is not null
            && Version == other.Version
            && Beta == other.Beta
            && BetaVersion == other.BetaVersion;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Version, Beta, BetaVersion);
    }
}