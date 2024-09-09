using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.DTOs
{
    public class ResetPinRequestDto
    {
        public string Email { get; set;}

        public string Token { get; set;}

        public string NewPin { get; set;}
        
    }
}