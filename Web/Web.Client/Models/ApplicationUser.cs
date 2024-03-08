using Microsoft.AspNetCore.Identity;

namespace Web.Client.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public virtual List<Order> Orders { get; set; } = new();
    public virtual ShoppingCart? ShoppingCart { get; set; } = new();
}
