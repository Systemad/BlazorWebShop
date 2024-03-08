using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web.Client.Models;

public class ProductsRoot
{
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; }
}

public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; } // USD 

    [JsonPropertyName("stock")]
    public int Stock { get; set; }

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }

    [JsonPropertyName("images")]
    public List<string> Images { get; set; }

    [JsonIgnore]
    public bool SoldOut => Stock is 0;
}

// https://dummyjson.com/products?limit=10&select=id,title,description,price,stock,thumbnail,images
public static class Data
{
    public static List<Product> GetSeedData()
    {
        string fileName = "Data/dataseed.json";
        string jsonString = File.ReadAllText(fileName);
        var products = JsonSerializer.Deserialize<ProductsRoot>(jsonString)!;
        return products.Products;
    }
}