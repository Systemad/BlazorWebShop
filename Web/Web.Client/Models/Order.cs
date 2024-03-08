using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Client.Models;

public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual List<OrderItem> OrderItems { get; set; } = new();

    [NotMapped]
    public decimal TotalCost => OrderItems?.Sum(item => item.TotalCost) ?? 0m;
}

public class OrderItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    //public string OrderId { get; set; }
    //public Order Order { get; set; }

    public int ProductId { get; set; }
    public virtual Product Product { get; set; }

    [NotMapped]
    public decimal TotalCost => Price * Quantity;
}

public class OrderForm
{
    [Required]
    [StringLength(20, ErrorMessage = "First name must be more than 3 letters")]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "Last name must be more than 3 letters")]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    //[StringLength(30, ErrorMessage = "Email must be more than 3 letters")]
    public string? Email { get; set; }

    [Required]
    [StringLength(25, ErrorMessage = "Address must be more than 3 letters")]
    public string? Address { get; set; }
}
