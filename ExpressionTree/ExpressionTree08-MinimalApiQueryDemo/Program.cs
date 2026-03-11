using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ProductQueryService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new
{
    ok = true,
    serverTime = DateTimeOffset.Now,
    endpoint = "/api/products/search"
}));

app.MapGet("/api/products/search", (
    string? keyword,
    string? category,
    decimal? minPrice,
    decimal? maxPrice,
    bool? onlyInStock,
    ProductQueryService service) =>
{
    var filter = new ProductSearchRequest
    {
        Keyword = keyword,
        Category = category,
        MinPrice = minPrice,
        MaxPrice = maxPrice,
        OnlyInStock = onlyInStock ?? false
    };

    var result = service.Search(filter);
    return Results.Ok(result);
});

app.Run();

public sealed class ProductQueryService
{
    private readonly List<Product> _products =
    [
        new() { Id = 1, Name = "MacBook Pro", Category = "Laptop", Price = 15999, InStock = true },
        new() { Id = 2, Name = "Magic Mouse", Category = "Accessory", Price = 599, InStock = true },
        new() { Id = 3, Name = "iPad Air", Category = "Tablet", Price = 4799, InStock = false },
        new() { Id = 4, Name = "Pro Display", Category = "Monitor", Price = 12999, InStock = true },
        new() { Id = 5, Name = "Gaming Laptop", Category = "Laptop", Price = 8999, InStock = true },
        new() { Id = 6, Name = "Travel Keyboard", Category = "Accessory", Price = 399, InStock = false }
    ];

    public ProductSearchResponse Search(ProductSearchRequest request)
    {
        var predicate = ProductSearchExpressionBuilder.Build(request);
        var compiled = predicate.Compile();
        var items = _products.Where(compiled).ToList();

        return new ProductSearchResponse
        {
            Filter = request,
            Expression = predicate.ToString(),
            Count = items.Count,
            Items = items
        };
    }
}

public static class ProductSearchExpressionBuilder
{
    public static Expression<Func<Product, bool>> Build(ProductSearchRequest request)
    {
        var product = Expression.Parameter(typeof(Product), "product");
        Expression body = Expression.Constant(true);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var name = Expression.Property(product, nameof(Product.Name));
            var keyword = Expression.Constant(request.Keyword);
            var contains = Expression.Call(name, nameof(string.Contains), Type.EmptyTypes, keyword);
            body = Expression.AndAlso(body, contains);
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var category = Expression.Property(product, nameof(Product.Category));
            var categoryValue = Expression.Constant(request.Category);
            var equals = Expression.Equal(category, categoryValue);
            body = Expression.AndAlso(body, equals);
        }

        if (request.MinPrice.HasValue)
        {
            var price = Expression.Property(product, nameof(Product.Price));
            var minPrice = Expression.Constant(request.MinPrice.Value);
            var greaterThanOrEqual = Expression.GreaterThanOrEqual(price, minPrice);
            body = Expression.AndAlso(body, greaterThanOrEqual);
        }

        if (request.MaxPrice.HasValue)
        {
            var price = Expression.Property(product, nameof(Product.Price));
            var maxPrice = Expression.Constant(request.MaxPrice.Value);
            var lessThanOrEqual = Expression.LessThanOrEqual(price, maxPrice);
            body = Expression.AndAlso(body, lessThanOrEqual);
        }

        if (request.OnlyInStock)
        {
            var inStock = Expression.Property(product, nameof(Product.InStock));
            var compare = Expression.Equal(inStock, Expression.Constant(true));
            body = Expression.AndAlso(body, compare);
        }

        return Expression.Lambda<Func<Product, bool>>(body, product);
    }
}

public sealed class Product
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public bool InStock { get; init; }
}

public sealed class ProductSearchRequest
{
    public string? Keyword { get; init; }

    public string? Category { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public bool OnlyInStock { get; init; }
}

public sealed class ProductSearchResponse
{
    public ProductSearchRequest Filter { get; init; } = new();

    public string Expression { get; init; } = string.Empty;

    public int Count { get; init; }

    public IReadOnlyList<Product> Items { get; init; } = [];
}
