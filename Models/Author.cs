namespace Backend.Models;

public class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateOnly? BirthDate { get; set; }
}
