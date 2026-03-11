using System.Linq.Expressions;

Console.WriteLine("=== 05 项目实践 ===");

var repository = new UserRepository();
var service = new UserQueryService(repository);

var request = new UserQueryRequest
{
    NameKeyword = "li",
    MinAge = 18,
    OnlyActive = true
};

var users = service.Search(request);

Console.WriteLine("查询结果:");
foreach (var user in users)
{
    Console.WriteLine($"- {user.Name}, Age={user.Age}, Active={user.IsActive}");
}

public sealed class User
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public int Age { get; init; }

    public bool IsActive { get; init; }
}

public sealed class UserQueryRequest
{
    public string? NameKeyword { get; init; }

    public int? MinAge { get; init; }

    public bool OnlyActive { get; init; }
}

public sealed class UserQueryService(UserRepository repository)
{
    public IReadOnlyList<User> Search(UserQueryRequest request)
    {
        var predicate = BuildPredicate(request);
        Console.WriteLine($"服务层拼出来的表达式: {predicate}");
        return repository.Find(predicate);
    }

    private static Expression<Func<User, bool>> BuildPredicate(UserQueryRequest request)
    {
        var user = Expression.Parameter(typeof(User), "user");
        Expression body = Expression.Constant(true);

        if (!string.IsNullOrWhiteSpace(request.NameKeyword))
        {
            var name = Expression.Property(user, nameof(User.Name));
            var keyword = Expression.Constant(request.NameKeyword);
            var contains = Expression.Call(name, nameof(string.Contains), Type.EmptyTypes, keyword);
            body = Expression.AndAlso(body, contains);
        }

        if (request.MinAge.HasValue)
        {
            var age = Expression.Property(user, nameof(User.Age));
            var minAge = Expression.Constant(request.MinAge.Value);
            body = Expression.AndAlso(body, Expression.GreaterThanOrEqual(age, minAge));
        }

        if (request.OnlyActive)
        {
            var active = Expression.Property(user, nameof(User.IsActive));
            body = Expression.AndAlso(body, Expression.Equal(active, Expression.Constant(true)));
        }

        return Expression.Lambda<Func<User, bool>>(body, user);
    }
}

public sealed class UserRepository
{
    private readonly List<User> _users =
    [
        new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
        new() { Id = 2, Name = "Bob", Age = 17, IsActive = true },
        new() { Id = 3, Name = "Lily", Age = 30, IsActive = true },
        new() { Id = 4, Name = "Milo", Age = 22, IsActive = false }
    ];

    public IReadOnlyList<User> Find(Expression<Func<User, bool>> predicate)
    {
        return _users.Where(predicate.Compile()).ToList();
    }
}
