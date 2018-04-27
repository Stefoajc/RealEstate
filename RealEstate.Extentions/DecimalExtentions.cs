namespace RealEstate.Extentions
{
    public static class DecimalExtentions
    {
        public static decimal Normalize(this decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }
    }
}