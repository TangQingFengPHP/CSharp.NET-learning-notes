using FluentValidation;

namespace ConsoleApp1;

public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .Length(3, 20).WithMessage("用户名长度必须在3到20之间");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确");
        
        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120)
            .WithMessage("年龄必须在18到120之间");
    }
}