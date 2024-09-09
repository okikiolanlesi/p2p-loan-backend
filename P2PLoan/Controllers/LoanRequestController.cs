using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/loan-request")]
public class LoanRequestController
{
    private readonly ILoanRequestService loanRequestService;

    public LoanRequestController(ILoanRequestService loanRequestService)
    {
        this.loanRequestService = loanRequestService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreateLoanRequestRequestDto createLoanRequestDto)
    {
        var response = await loanRequestService.Create(createLoanRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] LoanRequestSearchParams searchParams)
    {
        var response = await loanRequestService.GetAll(searchParams);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetAllForLoggedInUser([FromQuery] LoanRequestSearchParams searchParams)
    {
        var response = await loanRequestService.GetAllForLoggedInUser(searchParams);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("{loanRequestId:guid}")]
    public async Task<IActionResult> GetOne(Guid loanRequestId)
    {
        var response = await loanRequestService.GetOne(loanRequestId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("admin/{loanRequestId:guid}")]
    public async Task<IActionResult> GetOneAdmin(Guid loanRequestId)
    {
        var response = await loanRequestService.GetOneAdmin(loanRequestId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpDelete("remove/{loanRequestId:guid}")]
    public async Task<IActionResult> Delete(Guid loanRequestId)
    {
        var response = await loanRequestService.Delete(loanRequestId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpDelete("remove/admin/{loanRequestId:guid}")]
    public async Task<IActionResult> DeleteAdmin(Guid loanRequestId)
    {
        var response = await loanRequestService.DeleteAdmin(loanRequestId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost("accept/{loanRequestId:guid}")]
    public async Task<IActionResult> Accept(Guid loanRequestId)
    {
        var response = await loanRequestService.Accept(loanRequestId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost("decline/{loanRequestId:guid}")]
    public async Task<IActionResult> Decline(Guid loanRequestId)
    {
        var response = await loanRequestService.Decline(loanRequestId);
        return ControllerHelper.HandleApiResponse(response);
    }

}
