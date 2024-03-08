using Microsoft.EntityFrameworkCore;
using Web.Client.Models;
using Web.Data;

namespace Web.Services;

public class ShoppingCartService
{
    private ApplicationDbContext _context;

    public ShoppingCartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingCart> GetShoppingCart(ApplicationUser user)
    {
        var applicationUser = await _context
            .Users
            .Include(u => u.ShoppingCart)
            .ThenInclude(cart => cart.Items)
            .ThenInclude(cartItem => cartItem.Product)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        var cart = applicationUser.ShoppingCart;
        return cart;
    }

    public async Task ResetCart(ApplicationUser user)
    {
        var applicationUser = await _context
            .Users.Include(u => u.Orders)
            .Include(applicationUser => applicationUser.ShoppingCart)
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        applicationUser.ShoppingCart = new ShoppingCart();
        await _context.SaveChangesAsync();
    }

    public async Task<ShoppingCart> AddCartItem(ApplicationUser user, int productId)
    {
        var applicationUser = await _context
            .Users
            .Include(applicationUser => applicationUser.ShoppingCart)
            .ThenInclude(cart => cart.Items)
            .ThenInclude(cartItem => cartItem.Product)
            .FirstOrDefaultAsync(user => user.Id == user.Id);

        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (product is null)
        {
            throw new InvalidOperationException("Product doesn't exist");
        }

        var cart = applicationUser.ShoppingCart;
        var cartItem = cart.Items.FirstOrDefault(x => x.Product.Id == productId);
        if (cartItem is not null)
        {
            cartItem.Quantity++;
        }
        else
        {
            cart.Items.Add(
                new CartItem
                {
                    Quantity = 1,
                    ProductId = product.Id,
                    ShoppingCartId = cart.Id,
                }
            );
        }

        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<ShoppingCart> RemoveCartItem(ApplicationUser user, int productId)
    {
        var applicationUser = await _context
            .Users.Include(u => u.Orders)
            .Include(applicationUser => applicationUser.ShoppingCart)
            .ThenInclude(shoppingCart => shoppingCart.Items)
            .ThenInclude(cartItem => cartItem.Product)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        var cart = applicationUser.ShoppingCart;
        var cartItem = cart.Items.FirstOrDefault(x => x.Product.Id == productId);

        if (cartItem is not null)
        {
            if (cartItem.Quantity > 0)
            {
                cartItem.Quantity--;
            }

            if (cartItem.Quantity == 0)
            {
                cart.Items.Remove(cartItem);
            }
        }

        await _context.SaveChangesAsync();
        return cart;
    }
}
