using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using P2PLoan.Clients;

namespace P2PLoan;

public class MonnifyApiService
{
    private readonly MonnifyClient monnifyClient;

    public MonnifyApiService(MonnifyClient monnifyClient)
    {
        this.monnifyClient = monnifyClient;
    }
    public async Task CreateWallet(CreateWalletDto createWalletDto)
    {
        var jsonContent = JsonSerializer.Serialize(createWalletDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await monnifyClient.Client.PostAsync("/api/v1/disbursements/wallet", content);

        if (response.IsSuccessStatusCode)
        {
            // Handle successful response if needed
            var successContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(successContent);
        }
        else
        {
            // Handle error response
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error creating wallet: {response.StatusCode}, {errorContent}");
        }
    }

    public Task CreditWallet()
    {
        throw new NotImplementedException();
    }

    public Task DebitWallet()
    {
        throw new NotImplementedException();
    }

    public Task GetWalletBalance()
    {
        throw new NotImplementedException();
    }

    public Task GetWalletTransactions()
    {
        throw new NotImplementedException();
    }

    public Task WalletTransfer()
    {
        throw new NotImplementedException();
    }
    public Task VerifyNIN()
    {
        throw new NotImplementedException();
    }
}
