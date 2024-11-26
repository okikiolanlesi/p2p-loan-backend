using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/loan-offer")]
[Authorize]
public class LoanOfferController
{
    private readonly ILoanOfferService loanOfferService;

    public LoanOfferController(ILoanOfferService loanOfferService)
    {
        this.loanOfferService = loanOfferService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoanOfferRequestDto createLoanOfferRequestDto)
    {
        var response = await loanOfferService.Create(createLoanOfferRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] LoanOfferSearchParams searchParams)
    {
        var response = await loanOfferService.GetAll(searchParams);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    [Route("me")]
    public async Task<IActionResult> GetOne([FromQuery] LoanOfferSearchParams searchParams)
    {
        var response = await loanOfferService.GetAllForLoggedInUser(searchParams);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetAllForLoggedInUser(Guid id)
    {
        var response = await loanOfferService.GetOne(id);
        return ControllerHelper.HandleApiResponse(response);
    }
    [HttpGet]
    [Route("disable/{id:guid}")]
    public async Task<IActionResult> DisableLoanOffer(Guid id)
    {
        var response = await loanOfferService.DisableLoanOffer(id);
        return ControllerHelper.HandleApiResponse(response);
    }
}
