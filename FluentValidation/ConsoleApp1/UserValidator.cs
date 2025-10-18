using FluentValidation;

namespace ConsoleApp1;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        // 创建规则集
        RuleSet("Admin", () => 
        {
            RuleFor(user => user.IsAdmin).Must(b => b == true)
                .WithMessage("管理员用户必须设置管理员标志");
        });

        RuleSet("PaymentInfo", () => {
            RuleFor(c => c.CreditCardNumber).NotEmpty();
            RuleFor(c => c.CreditCardExpiry).NotEmpty();
        });
        
        Include(new UserDtoValidator());
    } 
}