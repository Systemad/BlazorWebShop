using Microsoft.EntityFrameworkCore;
using Web.Client.Models;
using Web.Data;

namespace Web.Services;

public class OrderService
{
    private ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrder(
        ApplicationUser applicationUser,
        ShoppingCart cart,
        OrderForm orderForm
    )
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == applicationUser.Id);
        var order = new Order
        {
            UserId = cart.UserId,
            FirstName = orderForm.FirstName,
            LastName = orderForm.LastName,
            Email = orderForm.Email,
            Address = orderForm.Email,
            CreatedAt = DateTime.UtcNow,
            OrderItems = new()
        };
        foreach (var item in cart.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.Product.Id);
            if (item.Quantity > 0 && item.Quantity <= product.Stock)
            {
                product.Stock -= item.Quantity;
                order.OrderItems.Add(
                    new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Quantity = item.Quantity,
                        Price = product.Price,
                        //OrderId = orderId,
                        ProductId = item.ProductId,
                    }
                );
            }
            else
            {
                throw new InvalidOperationException("Insufficient product quantity");
            }
        }
        user.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> GetOrdersFromUser(string userId)
    {
        var user = await _context
            .Users.Include(o => o.Orders)
            .ThenInclude(p => p.OrderItems)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user.Orders;
    }
}
