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
                case StartCategoryCode.Hemmahast:
                    return "Hemmahästar";
                case StartCategoryCode.Utlandshast:
                    return "Utländska hästar";
                case StartCategoryCode.TrendarUppATG:
                    return "Trendar uppåt (ATG)";
                case StartCategoryCode.TrendarNerATG:
                    return "Trendar neråt (ATG)";
                case StartCategoryCode.TrendarUppHPT:
                    return "Trendar uppåt (HPT)";
                case StartCategoryCode.TrendarNerHPT:
                    return "Trendar neråt (HPT)";
                case StartCategoryCode.AndraTredjeeHandare:
                    return "2- och 3-handare";
                case StartCategoryCode.Mellanspelade:
                    return "Mellanspelade";
                default:
                    return string.Empty;
            }
        }
    }
}
