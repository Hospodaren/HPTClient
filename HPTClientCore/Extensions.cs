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
    }
}
