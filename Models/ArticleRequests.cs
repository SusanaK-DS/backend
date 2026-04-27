namespace Backend.Models;

public record CreateArticleRequest(string Title, string Author, DateOnly? PublishedDate);

public record UpdateArticleRequest(string Title, string Author, DateOnly? PublishedDate);
