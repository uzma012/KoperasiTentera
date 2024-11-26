using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT.Models.DB.User
{
    public class UserCredentialModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserCredentialId { get; set; }

        [Required]
        public long UserDeviceId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SharedSecret { get; set; }

        public string? UserPIN { get; set; }

        [Required]
        public bool BiometricEnabled { get; set; }

        public string? BiometericSharedSecret { get; set; }
        public byte RetryCounter { get; set; }

        [ForeignKey("UserDeviceId")]
        public UserDeviceModel UserDevice { get; set; }
    }
}