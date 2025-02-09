namespace myFirstProject.Models;

public class UpdateUserRequest
{
    public long Id { get; set; }
    public string NewName { get; set; }
    public string Email { get; set; }
}