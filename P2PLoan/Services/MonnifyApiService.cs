using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using P2PLoan.Clients;

namespace P2PLoan;

public class MonnifyApiService : IMonnifyApiService
{
    private readonly MonnifyClient monnifyClient;

    public MonnifyApiService(MonnifyClient monnifyClient)
    {
        this.monnifyClient = monnifyClient;
    }
    public async Task<MonnifyApiResponse<MonnifyCreateWalletResponseBody>> CreateWallet(CreateWalletDto createWalletDto)
    {
        var jsonContent = JsonSerializer.Serialize(createWalletDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await monnifyClient.Client.PostAsync("/api/v1/disbursements/wallet", content);

        if (response.IsSuccessStatusCode)
        {
            // Handle successful response if needed
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<MonnifyApiResponse<MonnifyCreateWalletResponseBody>>(successContent);

            return data;
        }
        else
        {
            // Handle error response
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error creating wallet: {response.StatusCode}, {errorContent}");
        }
    }


    public async Task<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>> GetWalletTransactions(string accountNumber)
    {
        var requestUri = $"/api/v1/disbursements/wallet/balance?accountNumber={Uri.EscapeDataString(accountNumber)}";

        var response = await monnifyClient.Client.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>>(successContent);
            return data;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error getting wallet transactions: {response.StatusCode}, {errorContent}");
        }
    }

    public Task Transfer()
    {
        throw new NotImplementedException();
    }
    public Task VerifyNIN()
    {
        throw new NotImplementedException();
    }

    public async Task<MonnifyApiResponse<MonnifyGetBalanceResponseBody>> GetWalletBalance(string walletUniqueReference)
    {
        // Create the query string with the walletUniqueReference parameter
        var requestUri = $"/api/v1/disbursements/wallet/balance?walletReference={Uri.EscapeDataString(walletUniqueReference)}";

        var response = await monnifyClient.Client.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var successContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<MonnifyApiResponse<MonnifyGetBalanceResponseBody>>(successContent);
            return data;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error getting wallet balance: {response.StatusCode}, {errorContent}");
        }

    }
}

