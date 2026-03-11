using System.Linq.Expressions;

Console.WriteLine("=== 07 源码分析 ===");

Expression<Func<Customer, bool>> predicate = customer => customer.Age >= 18 && customer.City == "Shanghai";

var compiler = new FakeQueryCompiler();
var sql = compiler.CompileWhere(predicate);

Console.WriteLine($"原表达式: {predicate}");
Console.WriteLine($"翻译结果: {sql}");
Console.WriteLine();
Console.WriteLine("源码分析结论:");
Console.WriteLine("1. 服务层给的是 Expression<Func<T, bool>>");
Console.WriteLine("2. QueryCompiler 负责协调翻译过程");
Console.WriteLine("3. Visitor 负责逐节点把表达式翻译成 SQL-like 语句");

public sealed class Customer
{
    public int Age { get; init; }

    public string City { get; init; } = string.Empty;
}

public sealed class FakeQueryCompiler
{
    public string CompileWhere<T>(Expression<Func<T, bool>> predicate)
    {
        var visitor = new SqlLikeWhereVisitor();
        return visitor.Translate(predicate.Body);
    }
}

public sealed class SqlLikeWhereVisitor : ExpressionVisitor
{
    private readonly Stack<string> _parts = new();

    public string Translate(Expression expression)
    {
        Visit(expression);
        return _parts.Peek();
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Visit(node.Left);
        var left = _parts.Pop();

        Visit(node.Right);
        var right = _parts.Pop();

        var op = node.NodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.AndAlso => "AND",
            _ => node.NodeType.ToString()
        };

        _parts.Push($"({left} {op} {right})");
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        _parts.Push(node.Member.Name);
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        var value = node.Value switch
        {
            string text => $"'{text}'",
            null => "NULL",
            _ => node.Value!.ToString()!
        };

        _parts.Push(value);
        return node;
    }
}
