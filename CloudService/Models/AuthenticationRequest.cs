using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Models
{
    public class AuthenticationRequest
    {
        public string UserId { get; set; }
        public string Passwd { get; set; }
    }
}
