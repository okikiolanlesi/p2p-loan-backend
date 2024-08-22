using System;
using System.ComponentModel.DataAnnotations;
using P2PLoan.Constants;

namespace P2PLoan.Models;

public class CreateModuleRequestDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description {get; set;}   
    [Required]
    public Modules Identifier {get; set;}

}


