using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KT.Models.Registration.Registration.Response
{
    public class AccessTokenResponse
    {
        public string Pad { get; set; }
        public string AccessToken {get; set;}
        public int ExpiresIn {get; set;}
        public string Scope {get; set;}
        public string UUID { get; set; }
    }
}
