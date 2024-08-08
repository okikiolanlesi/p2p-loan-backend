using System;

namespace P2PLoan.Models;

public class UpdateRoleRequestDto
{
    public Guid id { get; set; }
    public string Name {get; set;}
    public string Description {get; set;}   

}