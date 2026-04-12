namespace Backend.Models;

public record CreateUserRequest(string? Name, string? Email);

public record UpdateUserRequest(string? Name, string? Email);

public record LoginUserRequest(string? Email);
