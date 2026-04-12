using Backend.Models;
using Dapper;
using Npgsql;
using System.Net;
using System.Net.Mail;

namespace Backend.Services;

public class UserService : IUserService
{
    private readonly NpgsqlDataSource _dataSource;

    public UserService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<BaseResponse> GetUsersAsync()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "SELECT id, name, email FROM library_users ORDER BY id";
        var command = new CommandDefinition(sql);
        var users = await connection.QueryAsync<User>(command);

        return new BaseResponse { Data = users.ToList() };
    }

    public async Task<BaseResponse> GetUserByIdAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "SELECT id, name, email FROM library_users WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id });
        var user = await connection.QuerySingleOrDefaultAsync<User>(command);

        if (user is null)
        {
            return CreateErrorResponse("User not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse { Data = user };
    }

    public async Task<BaseResponse> CreateUserAsync(CreateUserRequest request)
    {
        var validation = ValidateNameEmail(request.Name ?? "", request.Email ?? "");
        if (validation is not null)
        {
            return validation;
        }

        var name = request.Name!.Trim();
        var email = request.Email!.Trim();

        await using var connection = await _dataSource.OpenConnectionAsync();

        var existingUser = await GetUserByEmailAsync(connection, email);
        if (existingUser is not null)
        {
            return CreateErrorResponse("Email is already registered.", HttpStatusCode.BadRequest);
        }

        var sql =
            """
            INSERT INTO library_users (name, email)
            VALUES (@Name, @Email)
            RETURNING id, name, email;
            """;
        var command = new CommandDefinition(sql, new { Name = name, Email = email });
        var created = await connection.QuerySingleAsync<User>(command);

        return new BaseResponse
        {
            Data = created,
            HttpStatus = HttpStatusCode.Created
        };
    }

    public async Task<BaseResponse> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var validation = ValidateNameEmail(request.Name ?? "", request.Email ?? "");
        if (validation is not null)
        {
            return validation;
        }

        var name = request.Name!.Trim();
        var email = request.Email!.Trim();

        await using var connection = await _dataSource.OpenConnectionAsync();

        var existingWithEmail = await GetUserByEmailAsync(connection, email);
        if (existingWithEmail is not null && existingWithEmail.Id != id)
        {
            return CreateErrorResponse("Email is already registered.", HttpStatusCode.BadRequest);
        }

        var sql =
            """
            UPDATE library_users
            SET name = @Name, email = @Email
            WHERE id = @Id
            RETURNING id, name, email;
            """;
        var command = new CommandDefinition(
            sql,
            new { Id = id, Name = name, Email = email });
        var updated = await connection.QuerySingleOrDefaultAsync<User>(command);

        if (updated is null)
        {
            return CreateErrorResponse("User not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse { Data = updated };
    }

    public async Task<BaseResponse> DeleteUserAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "DELETE FROM library_users WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id });
        var rowsAffected = await connection.ExecuteAsync(command);

        if (rowsAffected == 0)
        {
            return CreateErrorResponse("User not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = new { Message = "User deleted successfully." }
        };
    }

    public async Task<BaseResponse> LoginAsync(LoginUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email?.Trim()))
        {
            return CreateErrorResponse("Email is required.", HttpStatusCode.BadRequest);
        }

        await using var connection = await _dataSource.OpenConnectionAsync();

        var email = request.Email.Trim();
        var user = await GetUserByEmailAsync(connection, email);
        if (user is null)
        {
            return CreateErrorResponse("User not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse { Data = user };
    }

    private static async Task<User?> GetUserByEmailAsync(
        NpgsqlConnection connection,
        string email)
    {
        var sql = "SELECT id, name, email FROM library_users WHERE LOWER(email) = LOWER(@Email)";
        var command = new CommandDefinition(sql, new { Email = email });
        return await connection.QuerySingleOrDefaultAsync<User>(command);
    }

    private static BaseResponse? ValidateNameEmail(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
        {
            return CreateErrorResponse("Name and email are required.", HttpStatusCode.BadRequest);
        }

        try
        {
            _ = new MailAddress(email.Trim());
        }
        catch (FormatException)
        {
            return CreateErrorResponse("Email format is invalid.", HttpStatusCode.BadRequest);
        }

        return null;
    }

    private static BaseResponse CreateErrorResponse(string message, HttpStatusCode statusCode)
    {
        return new BaseResponse
        {
            Status = false,
            ErrorCode = 0,
            ErrorMessage = message,
            HttpStatus = statusCode
        };
    }
}
