using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Client;
using Web.Client.Models;
using Web.Components;
using Web.Components.Account;
using Web.Data;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<
    AuthenticationStateProvider,
    PersistingRevalidatingAuthenticationStateProvider
>();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder
    .Services.AddIdentityCore<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = true
    )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ApiService>();

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Web.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

var orderGroup = app.MapGroup("api/v1/order");
orderGroup.MapGet("/all", GetOrders).RequireAuthorization();
orderGroup.MapGet("/{orderId}", GetOrder).RequireAuthorization();

var key = app.Configuration["ApiKey"];
app.Map("/exchangerate/{currency}", async (HttpClient httpClient, string currency) =>
{
    httpClient.DefaultRequestHeaders.Add("X-Api-Key", key);
    
    var url = $"https://api.api-ninjas.com/v1/exchangerate?pair=USD_{currency}";
    var rate = await httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);
    return rate;
});

app.Run();


static async Task<List<Order>> GetOrders(HttpContext context, ApplicationDbContext db)
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

    var user = await db
        .Users.Where(u => u.Id == userId)
        .Include(o => o.Orders) // SelectMany?
        .ThenInclude(p => p.OrderItems)
        .FirstOrDefaultAsync();

    return user.Orders;
}

static async Task<Order> GetOrder(HttpContext context, ApplicationDbContext db, string orderId)
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var order = await db
        .Users.Where(u => u.Id == userId)
        .SelectMany(u => u.Orders)
        .Include(o => o.OrderItems)
        .ThenInclude(p => p.Product)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    return order;
}
