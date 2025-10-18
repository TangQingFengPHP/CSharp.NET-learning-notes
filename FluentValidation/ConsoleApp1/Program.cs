using System.ComponentModel.DataAnnotations;
using ConsoleApp1;
using FluentValidation;

var user = new UserDto { Username = "", Email = "bad", Age = 10 };
// var validator = new UserDtoValidator();
// var result = validator.Validate(user);
//
// if (!result.IsValid)
// {
//     foreach (var error in result.Errors)
//     {
//         Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
//     }
// }

// 使用规则集
var validator = new UserValidator();
var result = validator.Validate(user, options =>
{
    options.IncludeRuleSets("Admin", "PaymentInfo");
});

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}