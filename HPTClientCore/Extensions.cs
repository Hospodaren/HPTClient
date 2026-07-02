using System.Linq;

namespace HPTClient;

/// <summary>
/// Extension methods for collections and enum formatting.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Computes the population standard deviation of a decimal sequence in a single pass.
    /// Uses Welford's online algorithm for numerical stability.
    /// </summary>
    public static decimal StdDev(this IEnumerable<decimal> values)
    {
        decimal count = 0;
        decimal mean = 0m;
        decimal m2 = 0m;

        foreach (decimal value in values)
        {
            count++;
            decimal delta = value - mean;
            mean += delta / count;
            decimal delta2 = value - mean;
            m2 += delta * delta2;
        }

        return count > 1 ? (decimal)Math.Sqrt((double)(m2 / count)) : 0m;
    }

    /// <summary>
    /// Gets a human-readable Swedish string for the given StartCategoryCode.
    /// </summary>
    public static string GetString(this StartCategoryCode categoryCode)
    {
        return categoryCode switch
        {
            StartCategoryCode.None => "Ingen",
            StartCategoryCode.Favorit => "Favoriter",
            StartCategoryCode.Storfavorit => "Storfavoriter",
            StartCategoryCode.EjStorfavorit => "Ej storfavoriter",
            StartCategoryCode.Megafavorit => "Megafavoriter",
            StartCategoryCode.EjMegafavorit => "Ej megafavoriter",
            StartCategoryCode.KnappFavorit => "Knappa favoriter",
            StartCategoryCode.Overraskning => "Överraskning",
            StartCategoryCode.Skrall => "Skräll",
            StartCategoryCode.Storskrall => "Storskräll",
            StartCategoryCode.Hemmahast => "Hemmahästar",
            StartCategoryCode.Utlandshast => "Utländska hästar",
            StartCategoryCode.TrendarUppATG => "Trendar uppåt (ATG)",
            StartCategoryCode.TrendarNerATG => "Trendar neråt (ATG)",
            StartCategoryCode.TrendarUppHPT => "Trendar uppåt (HPT)",
            StartCategoryCode.TrendarNerHPT => "Trendar neråt (HPT)",
            StartCategoryCode.AndraTredjeeHandare => "2- och 3-handare",
            StartCategoryCode.Mellanspelade => "Mellanspelade",
            _ => string.Empty
        };
    }
}