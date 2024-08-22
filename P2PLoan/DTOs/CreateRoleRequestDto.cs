using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.Models;

public class CreateRoleRequestDto
{
    [Required]
    public string Name {get; set;}
    [Required]
    public string Description {get; set;}

}


