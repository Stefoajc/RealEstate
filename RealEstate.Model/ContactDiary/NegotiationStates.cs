using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.ContactDiary
{
    /// <summary>
    /// Не установен контакт
    /// Последващо обаждане за разясняване
    /// Последващо обаждане за среща
    /// Отказва да работи с нас
    /// Приема да работи с нас
    /// </summary>
    public class NegotiationStates
    {
        [Key]
        public int Id { get; set; }
        public string State { get; set; }

        public string Color { get; set; }
    }
}
