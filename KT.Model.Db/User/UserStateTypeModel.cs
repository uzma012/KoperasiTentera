using System.ComponentModel.DataAnnotations;

namespace KT.Models.DB.User
{
    public class UserStateTypeModel
    {
        [Required]
        [Key]
        public byte UserStateTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}