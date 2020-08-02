using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.ContactDiary
{
    /// <summary>
    /// Стройтелен предприемач
    /// Агенция
    /// Лице с много имоти
    /// Лице с един имот
    /// Бизнесмен
    /// Брокер
    /// Популярна личност
    /// Наемател
    /// Купувач
    /// Наемодател
    /// Продавач
    /// </summary>
    public class ContactedPersonTypes
    {
        [Key]
        public int Id { get; set; }
        public string ContactedPersonType { get; set; }
    }
}
