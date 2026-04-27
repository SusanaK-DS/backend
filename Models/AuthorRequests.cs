namespace Backend.Models;

public record CreateAuthorRequest(string? Name, string? Email, DateOnly? BirthDate);

public record UpdateAuthorRequest(string? Name, string? Email, DateOnly? BirthDate);
