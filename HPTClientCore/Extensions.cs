namespace HPTClient
{
    public static class Extensions
    {
        public static decimal StdDev(this IEnumerable<decimal> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                decimal avg = values.Average();

                //Perform the Sum of (value-avg)^2
                decimal sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt((double)sum / count);
            }
            return (decimal)ret;
        }

        public static string GetString(this StartCategoryCode categoryCode)
        {

            switch (categoryCode)
            {
                case StartCategoryCode.None:
                    return "Ingen";
                case StartCategoryCode.Favorit:
                    return "Favoriter";
                case StartCategoryCode.Storfavorit:
                    return "Storfavoriter";
                case StartCategoryCode.EjStorfavorit:
                    return "Ej storfavoriter";
                case StartCategoryCode.Megafavorit:
                    return "Megafavoriter";
                case StartCategoryCode.EjMegafavorit:
                    return "Ej megafavoriter";
                case StartCategoryCode.KnappFavorit:
                    return "Knappa favoriter";
                case StartCategoryCode.Overraskning:
                    return "Överraskning";
                case StartCategoryCode.Skrall:
                    return "Skräll";
                case StartCategoryCode.Storskrall:
                    return "Storskräll";
                default:
                    return string.Empty;
            }
        }
    }
}
