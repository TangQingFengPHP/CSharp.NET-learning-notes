using System.Linq.Expressions;

Console.WriteLine("=== 04 动态条件拼接 ===");

var products = new List<Product>
{
    new() { Name = "MacBook Pro", Price = 15999, IsPublished = true },
    new() { Name = "Mechanical Keyboard", Price = 699, IsPublished = true },
    new() { Name = "Prototype Device", Price = 3999, IsPublished = false },
    new() { Name = "Pro Mouse", Price = 299, IsPublished = true }
};

var filter = new ProductFilter
{
    Keyword = "Pro",
    MinPrice = 300,
    IsPublished = true
};

var predicate = ProductPredicateBuilder.Build(filter);
Console.WriteLine($"生成的表达式: {predicate}");

var result = products.Where(predicate.Compile()).ToList();
Console.WriteLine("匹配结果:");
foreach (var product in result)
{
    Console.WriteLine($"- {product.Name}, Price={product.Price}, Published={product.IsPublished}");
}

public sealed class Product
{
    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public bool IsPublished { get; init; }
}

public sealed class ProductFilter
{
    public string? Keyword { get; init; }

    public decimal? MinPrice { get; init; }

    public bool? IsPublished { get; init; }
}

public static class ProductPredicateBuilder
{
    public static Expression<Func<Product, bool>> Build(ProductFilter filter)
    {
        var product = Expression.Parameter(typeof(Product), "product");
        Expression body = Expression.Constant(true);

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var name = Expression.Property(product, nameof(Product.Name));
            var keyword = Expression.Constant(filter.Keyword);
            var contains = Expression.Call(name, nameof(string.Contains), Type.EmptyTypes, keyword);
            body = Expression.AndAlso(body, contains);
        }

        if (filter.MinPrice is not null)
        {
            var price = Expression.Property(product, nameof(Product.Price));
            var minPrice = Expression.Constant(filter.MinPrice.Value);
            var compare = Expression.GreaterThanOrEqual(price, minPrice);
            body = Expression.AndAlso(body, compare);
        }

        if (filter.IsPublished is not null)
        {
            var published = Expression.Property(product, nameof(Product.IsPublished));
            var isPublished = Expression.Constant(filter.IsPublished.Value);
            var compare = Expression.Equal(published, isPublished);
            body = Expression.AndAlso(body, compare);
        }

        return Expression.Lambda<Func<Product, bool>>(body, product);
    }
}
