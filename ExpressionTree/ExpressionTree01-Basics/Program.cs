using System.Linq.Expressions;

Console.WriteLine("=== 01 基础概念 ===");

Func<int, int> normalLambda = x => x + 1;
Expression<Func<int, int>> expressionLambda = x => x + 1;

Console.WriteLine($"委托执行结果: {normalLambda(10)}");
Console.WriteLine($"表达式树字符串: {expressionLambda}");
Console.WriteLine($"Body NodeType: {expressionLambda.Body.NodeType}");
Console.WriteLine($"Body Type: {expressionLambda.Body.GetType().Name}");

var binaryBody = (BinaryExpression)expressionLambda.Body;
Console.WriteLine($"Left NodeType: {binaryBody.Left.NodeType}");
Console.WriteLine($"Right NodeType: {binaryBody.Right.NodeType}");

Expression<Func<User, bool>> userExpr = user => user.Age >= 18 && user.Name.StartsWith("A");
Console.WriteLine();
Console.WriteLine($"复杂表达式: {userExpr}");
Console.WriteLine($"根节点: {userExpr.Body.NodeType}");
Console.WriteLine("说明: 表达式树把代码保存成节点结构，而不是直接只剩机器可执行逻辑。\n");

var manualParameter = Expression.Parameter(typeof(int), "x");
var manualExpr = Expression.Lambda<Func<int, int>>(
    Expression.Add(manualParameter, Expression.Constant(2)),
    manualParameter);

Console.WriteLine($"手动构建表达式: {manualExpr}");
Console.WriteLine("注意: 手动构建表达式时，参数对象必须复用同一个实例，否则树无效。");

public sealed class User
{
    public string Name { get; init; } = string.Empty;

    public int Age { get; init; }
}
