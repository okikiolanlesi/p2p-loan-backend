using System;
using P2PLoan.Constants;

namespace P2PLoan.Models;

public class CreateModuleRequestDto
{
    public Guid id { get; set; }
    public string Description {get; set;}   

    public Modules Identifier {get; set;}

}


