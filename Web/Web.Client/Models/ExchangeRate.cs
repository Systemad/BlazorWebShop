using System.Text.Json.Serialization;

namespace Web.Client.Models;


public enum CurrencyEnum
{
    USD,
    EUR,
    GBP
}

public partial class ExchangeRateResponse
{
    [JsonPropertyName("currency_pair")]
    public string CurrencyPair { get; set; }

    [JsonPropertyName("exchange_rate")]
    public decimal ExchangeRate { get; set; }
}