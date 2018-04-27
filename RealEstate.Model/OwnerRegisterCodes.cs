using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model
{
    public class OwnerRegisterCodes
    {
        [Key]
        public string OwnerRegisterCode { get; set; }
    }
}