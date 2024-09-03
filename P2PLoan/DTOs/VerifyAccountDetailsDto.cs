using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.DTOs
{
    public class VerifyAccountDetailsDto
    {
        [Required]
        public string AccountNumber { get; set; }
         [Required]
        public string BankCode { get; set;}
        
    }
}