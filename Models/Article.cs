namespace Backend.Models;

public class Article
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public DateOnly? PublishedDate { get; set; }

}
