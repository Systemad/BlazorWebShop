using Web.Client.Models;

namespace Web.Client;

public static class ExchangeRateHelper
{
    public static string ConvertPrice(decimal original, decimal rate)
    {
        var newPrice = original * rate;
        return newPrice.ToString("0.00");
    }
    
    public static string GetExchangeSymbol(CurrencyEnum currencyEnum)
    {
        return currencyEnum switch
        {
            CurrencyEnum.EUR => "\u20ac",
            CurrencyEnum.GBP => "\u00a3",
            _ => "$"
        };
    }
}