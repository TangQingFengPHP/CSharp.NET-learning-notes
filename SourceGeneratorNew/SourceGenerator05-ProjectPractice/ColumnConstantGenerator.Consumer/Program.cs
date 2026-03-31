using ColumnConstantGenerator.Consumer.Entities;

Console.WriteLine("=== 05 Project Practice: Column Constants ===");
Console.WriteLine(string.Join(", ", UserEntityColumns.All));
Console.WriteLine($"Order status column => {OrderEntityColumns.Status}");
