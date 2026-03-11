using System.Linq.Expressions;

Console.WriteLine("=== 03 Visitor 与解析 ===");

Expression<Func<Product, bool>> expr = product => product.Price > 100m && product.Name.Contains("Pro");
Console.WriteLine($"原表达式: {expr}");

var printer = new SimpleExpressionPrinter();
printer.Visit(expr);

Console.WriteLine();
Console.WriteLine("Visitor 调用链:");
var replacer = new ConstantReplaceVisitor(100m, 200m);
var replaced = (Expression<Func<Product, bool>>)replacer.Visit(expr)!;

Console.WriteLine();
Console.WriteLine($"替换常量后的表达式: {replaced}");

public sealed class Product
{
    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }
}

public sealed class SimpleExpressionPrinter : ExpressionVisitor
{
    private int _depth;

    public override Expression? Visit(Expression? node)
    {
        if (node is null)
        {
            return null;
        }

        Console.WriteLine($"{new string(' ', _depth * 2)}- {node.NodeType} ({node.GetType().Name})");
        _depth++;
        var result = base.Visit(node);
        _depth--;
        return result;
    }
}

public sealed class ConstantReplaceVisitor(object oldValue, object newValue) : ExpressionVisitor
{
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        Console.WriteLine($"- VisitLambda: {node}");
        return base.VisitLambda(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Console.WriteLine($"- VisitBinary: {node.NodeType} => {node}");
        return base.VisitBinary(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        Console.WriteLine($"- VisitMethodCall: {node.Method.Name} => {node}");
        return base.VisitMethodCall(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        Console.WriteLine($"- VisitMember: {node.Member.Name} => {node}");
        return base.VisitMember(node);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        Console.WriteLine($"- VisitParameter: {node.Name} ({node.Type.Name})");
        return base.VisitParameter(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        Console.WriteLine($"- VisitUnary: {node.NodeType} => {node}");
        var operand = Visit(node.Operand);
        return node.Update(operand);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        Console.WriteLine($"- VisitConstant: Value={node.Value ?? "null"}, Type={node.Type.Name}");
        if (Equals(node.Value, oldValue))
        {
            Console.WriteLine($"  -> 命中替换: {oldValue} => {newValue}");
            return Expression.Constant(newValue, node.Type);
        }

        return base.VisitConstant(node);
    }
}
