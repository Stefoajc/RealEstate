using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.ContactDiary
{
    // Търсене наем / Предлагане наем дългосрочен / Предлагане наем краткосрочен / Търсене покупка / предлагане покупка
    public class DealTypes
    {
        [Key]
        public int Id { get; set; }
        public string DealType { get; set; }
    }
}