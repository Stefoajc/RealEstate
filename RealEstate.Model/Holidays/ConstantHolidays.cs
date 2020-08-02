namespace RealEstate.Model.Holidays
{
    /// <summary>
    /// Names days, national holidays
    /// </summary>
    public class ConstantHolidays
    {
        public int Id { get; set; }
        public int DayOfMonth { get; set; }
        public int Month { get; set; }
        public string Name { get; set; }
    }
}