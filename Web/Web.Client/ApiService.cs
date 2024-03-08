using System.Net.Http.Json;
using Web.Client.Models;

namespace Web.Client;

public class ApiService
{
    private HttpClient HttpClient { get; set; }

    public ApiService(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<Order> GetOrder(string orderId)
    {
        var obj =
            await HttpClient.GetFromJsonAsync<Order>(
                $"https://localhost:7043/api/v1/order/{orderId}"
            ); // ?? new();
        return obj;
    }
    
    public async Task<ExchangeRateResponse> GetExchangeRate(CurrencyEnum currencyEnum)
    {
        var url = $"https://localhost:7043/exchangerate/{currencyEnum switch
        {
            CurrencyEnum.EUR => "EUR",
            CurrencyEnum.GBP => "GBP",
            _ => "USD"
        }}";
        
        var rate = await HttpClient.GetFromJsonAsync<ExchangeRateResponse>(url);
        return rate;
    }
}
