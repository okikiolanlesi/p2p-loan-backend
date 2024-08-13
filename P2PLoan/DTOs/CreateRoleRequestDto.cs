using System;
using System.Collections.Generic;

namespace P2PLoan.Models;

public class CreateRoleRequestDto
{
    public Guid id { get; set; }
    public string Name {get; set;}
    public string Description {get; set;} 
     public List<Guid> PermissionIds { get; set; }  

}


