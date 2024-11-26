using System;
using System.Collections.Generic;
using System.Text;

namespace KT.Models.Common.Options
{
    public class JWTTokenOptions
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string Audience { get; set; }
    }
}
