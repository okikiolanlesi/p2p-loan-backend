using System;
using System.Collections.Generic;

namespace P2PLoan.Models;

public class CreateRoleRequestDto
{
    public string Name {get; set;}
    public string Description {get; set;}
    public Guid CreatedById { get; set; }
    public Guid ModifiedById { get; set; }

}

