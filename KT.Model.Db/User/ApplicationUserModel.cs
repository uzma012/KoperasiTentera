using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT.Models.DB.User
{
    public class ApplicationUserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UUID { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MobileNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ICNumber { get; set; }


        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public int LastModifiedBy { get; set; }


        [Required]
        public byte UserStateTypeId { get; set; }

        [ForeignKey("UserStateTypeId")]
        public UserStateTypeModel UserStateType { get; set; }


      
    }
}