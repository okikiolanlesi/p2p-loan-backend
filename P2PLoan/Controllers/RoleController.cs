using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Attributes;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/role")]

public class RoleController : ControllerBase
{
    private readonly IRoleService roleService;
    public RoleController(IRoleService roleService)
    {
        this.roleService = roleService;

    }

}