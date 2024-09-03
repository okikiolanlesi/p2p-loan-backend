using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using P2PLoan.Clients;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;

namespace P2PLoan.Services;

public class MonnifyApiService : IMonnifyApiService
{
    private readonly MonnifyClient monnifyClient;

    public MonnifyApiService(MonnifyClient monnifyClient)
    {
        this.monnifyClient = monnifyClient;
    }
    public async Task<MonnifyApiResponse<MonnifyCreateWalletResponseBody>> CreateWallet(CreateWalletDto createWalletDto)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var jsonContent = System.Text.Json.JsonSerializer.Serialize(createWalletDto, options);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await monnifyClient.Client.PostAsync("/api/v1/disbursements/wallet", content);

        if (response.IsSuccessStatusCode)
        {
            // Handle successful response if needed
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<MonnifyApiResponse<MonnifyCreateWalletResponseBody>>(successContent);

            return data;
        }
        else
        {
            // Handle error response
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error creating wallet: {response.StatusCode}, {errorContent}");
        }
    }


    public async Task<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>> GetWalletTransactions(string accountNumber, int pageSize = 10, int pageNo = 1)
    {
        var requestUri = $"/api/v1/disbursements/wallet/transactions?accountNumber={Uri.EscapeDataString(accountNumber)}&pageSize={Uri.EscapeDataString($"{pageSize}")}&pageNo={Uri.EscapeDataString($"{pageNo}")}";

        var response = await monnifyClient.Client.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>>(successContent);
            return data;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error getting wallet transactions: {response.StatusCode}, {errorContent}");
        }
    }

    public async Task<MonnifyApiResponse<MonnifyGetBalanceResponseBody>> GetWalletBalance(string walletUniqueReference)
    {
        // Create the query string with the walletUniqueReference parameter
        var requestUri = $"/api/v1/disbursements/wallet/balance?walletReference={Uri.EscapeDataString(walletUniqueReference)}&accountNumber={Uri.EscapeDataString(walletUniqueReference)}";

        var response = await monnifyClient.Client.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<MonnifyApiResponse<MonnifyGetBalanceResponseBody>>(successContent);
            return data;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error getting wallet balance: {response.StatusCode}, {errorContent}");
        }

    }

    public async Task<MonnifyApiResponse<MonnifyGetSingleTransferResponseBody>> Transfer(MonnifyTransferRequestBodyDto transferDto)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var jsonContent = System.Text.Json.JsonSerializer.Serialize(transferDto, options);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await monnifyClient.Client.PostAsync("/api/v1/disbursements/single", content);

        if (response.IsSuccessStatusCode)
        {
            // Handle successful response if needed
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<MonnifyApiResponse<MonnifyGetSingleTransferResponseBody>>(successContent);

            return data;
        }
        else
        {
            // Handle error response
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error executing transfer: {response.StatusCode}, {errorContent}");
        }
    }

    public async Task<MonnifyApiResponse<MonnifyVerifyBVNResponseBody>> VerifyBVN(MonnifyVerifyBVNRequestDto verifyBVNDto)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var jsonContent = System.Text.Json.JsonSerializer.Serialize(verifyBVNDto, options);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await monnifyClient.Client.PostAsync("/api/v1/vas/bvn-details-match", content);

        if (response.IsSuccessStatusCode)
        {
            // Handle successful response if needed
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<MonnifyApiResponse<MonnifyVerifyBVNResponseBody>>(successContent);

            return data;
        }
        else
        {
            // Handle error response
            var errorContent = await response.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);

            throw new HttpRequestException($"{error.responseCode}:{error.responseMessage}");
        }
    }

    public async Task<MonnifyApiResponse<MonnifyVerifyAccountDetailsResponseBody>> VerifyAccountDetails(MonnifyVerifyAccountDetailsRequestDto verifyAccountDetailsRequestDto)
    {
        var queryString = $"?accountNumber={verifyAccountDetailsRequestDto.AccountNumber}&bankCode={verifyAccountDetailsRequestDto.BankCode}";
    
        var url = $"/api/v1/disbursements/account/validate{queryString}";
        var response = await monnifyClient.Client.GetAsync(url);
        
         if(response.IsSuccessStatusCode)
        {
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<MonnifyApiResponse<MonnifyVerifyAccountDetailsResponseBody>>(successContent);

            return data;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);

            throw new HttpRequestException($"{error.responseCode}:{error.responseMessage}");

        }

        
    }

    public async Task<MonnifyApiResponse<List<BankDto>>> GetBanks()
    {
       var url = $"/api/v1/banks";

       var response = await monnifyClient.Client.GetAsync(url);

       if(response.IsSuccessStatusCode)
       {
        var successContent = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<MonnifyApiResponse<List<BankDto>>>(successContent);

        return data;    

       }
       else
       {
        var errorContent = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
          throw new HttpRequestException($"{error.responseCode}:{error.responseMessage}");

       }
       
    
        
        
    }
}

class ErrorResponse
{
    public bool requestSuccessful { get; set; }
    public string responseMessage { get; set; }
    public string responseCode { get; set; }
}
