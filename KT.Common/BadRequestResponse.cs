using System.Collections.Generic;

namespace KT.Models.Common
{
    public class BadRequestResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        //public List<ErrorDetails> Errors { get; set; }
    }
}