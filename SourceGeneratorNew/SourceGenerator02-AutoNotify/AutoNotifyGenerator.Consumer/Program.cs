using AutoNotifyGenerator.Consumer.ViewModels;

var vm = new PersonViewModel
{
    Name = "Alice",
    Age = 28
};

Console.WriteLine("=== 02 AutoNotify ===");
Console.WriteLine($"Name={vm.Name}, Age={vm.Age}");
