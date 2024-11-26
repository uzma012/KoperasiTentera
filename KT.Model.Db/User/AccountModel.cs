using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KT.Models.DB.User
{
    public class AccountModel
    {
        [Key]
        public long AccountId { get; set; }

        public int UserId { get; set; }
        public string AccountName {get;set;}

        [ForeignKey("UserId")]
        public ApplicationUserModel ApplicationUser { get; set; }
    }
}
