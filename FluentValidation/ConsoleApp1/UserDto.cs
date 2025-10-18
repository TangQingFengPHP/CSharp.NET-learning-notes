namespace ConsoleApp1;

public class UserDto
{
    public string Username { get; set; }
    public string Email    { get; set; }
    public int    Age      { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public string CreditCardNumber { get; set; }
    
    public string CreditCardExpiry { get; set; }
}