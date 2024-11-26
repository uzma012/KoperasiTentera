using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT.Models.DB.User
{
    public class UserDeviceModel : ICloneable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserDeviceId { get; set; }

        [Required]
        public int UserId { get; set; }

        public string? DeviceProfile { get; set; }

        public string? DeviceProfileHash { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUserModel ApplicationUser { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? UDID { get; set; } = Guid.NewGuid();

       
        public object Clone()
        {
            return this;
        }
    }
}