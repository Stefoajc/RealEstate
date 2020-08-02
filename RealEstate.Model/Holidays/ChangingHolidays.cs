using System;

namespace RealEstate.Model.Holidays
{
    /// <summary>
    /// Like Easter (Великден) 
    /// Palm day (Цветница) 
    /// Whit day () 
    /// Good Friday (Разпети петък) 
    ///     Ascension Day (Възнесение господне)
    /// </summary>
    public class ChangingHolidays
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}