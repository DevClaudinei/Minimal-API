namespace AppServices;

public class CreateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public string? Nickname { get; set; }
}
