using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Client.Models;

public class ShoppingCart
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public virtual List<CartItem> Items { get; set; } = new();

    [NotMapped]
    public decimal TotalCost => Items?.Sum(item => item.Quantity * item.Product.Price) ?? 0m;
}

public class CartItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Quantity { get; set; }

    public int ProductId { get; set; }
    public virtual Product Product { get; set; }

    public string ShoppingCartId { get; set; }
    public virtual ShoppingCart ShoppingCart { get; set; }
}
