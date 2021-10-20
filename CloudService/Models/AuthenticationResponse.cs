using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Models
{
    public class AuthenticationResponse
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Tocken { get; set; }

        public AuthenticationResponse(string userId, string tocken)
        {
            UserId = userId;
            Tocken = tocken;
        }
    }
}
