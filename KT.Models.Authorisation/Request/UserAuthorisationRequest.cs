using System;
using System.Collections.Generic;
using System.Text;

namespace KT.Models.Authorisation.Request
{
    public class UserAuthorisationRequest
    {
        public Guid UUID { get; set; }
        public string UPH { get; set; }
        public string UPOTP { get; set; }
        public string BPOTP { get; set; }
        public Guid  UDID { get; set; }
    }
}
