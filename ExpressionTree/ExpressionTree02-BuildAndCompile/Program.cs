using System.Linq.Expressions;

Console.WriteLine("=== 02 手动构建与编译 ===");

var input = Expression.Parameter(typeof(int), "x");
var multiply = Expression.Multiply(input, Expression.Constant(2));
var body = Expression.Add(multiply, Expression.Constant(1));
var lambda = Expression.Lambda<Func<int, int>>(body, input);

Console.WriteLine($"表达式树: {lambda}");

var compiled = lambda.Compile();
Console.WriteLine($"compiled(10) = {compiled(10)}");
Console.WriteLine($"compiled(25) = {compiled(25)}");

Expression<Func<int, bool>> evenExpr = BuildEvenExpression();
Console.WriteLine();
Console.WriteLine($"偶数判断表达式: {evenExpr}");
Console.WriteLine($"8 => {evenExpr.Compile()(8)}");
Console.WriteLine($"9 => {evenExpr.Compile()(9)}");

static Expression<Func<int, bool>> BuildEvenExpression()
{
    var number = Expression.Parameter(typeof(int), "number");
    var modulo = Expression.Modulo(number, Expression.Constant(2));
    var compare = Expression.Equal(modulo, Expression.Constant(0));
    return Expression.Lambda<Func<int, bool>>(compare, number);
}
