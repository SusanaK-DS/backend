using Backend.Models;
using Dapper;
using Npgsql;
using System.Net;
using System.Net.Mail;

namespace Backend.Services;

public class AuthorService : IAuthorService
{
    private readonly NpgsqlDataSource _dataSource;

    public AuthorService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<BaseResponse> GetAuthorsAsync()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "SELECT id, name, email, birthdate AS BirthDate FROM author ORDER BY id";
        var command = new CommandDefinition(sql);
        var authors = await connection.QueryAsync<Author>(command);

        return new BaseResponse
        {
            Data = authors.ToList()
        };
    }

    public async Task<BaseResponse> GetAuthorByIdAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "SELECT id, name, email, birthdate AS BirthDate FROM author WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id });
        var author = await connection.QuerySingleOrDefaultAsync<Author>(command);

        if (author is null)
        {
            return CreateErrorResponse("Author not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = author
        };
    }

    public async Task<BaseResponse> CreateAuthorAsync(CreateAuthorRequest request)
    {
        var validation = ValidateAuthor(request.Name, request.Email, request.BirthDate);
        if (validation is not null)
        {
            return validation;
        }

        var name = request.Name!.Trim();
        var email = request.Email!.Trim();
        var birthDate = request.BirthDate!.Value;
        var birthDateValue = birthDate.ToDateTime(TimeOnly.MinValue);

        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql =
            """
            INSERT INTO author (name, email, birthdate)
            VALUES (@Name, @Email, @BirthDate)
            RETURNING id, name, email, birthdate AS BirthDate;
            """;
        var command = new CommandDefinition(sql, new { Name = name, Email = email, BirthDate = birthDateValue });
        var created = await connection.QuerySingleAsync<Author>(command);

        return new BaseResponse
        {
            Data = created,
            HttpStatus = HttpStatusCode.Created
        };
    }

    public async Task<BaseResponse> UpdateAuthorAsync(int id, UpdateAuthorRequest request)
    {
        var validation = ValidateAuthor(request.Name, request.Email, request.BirthDate);
        if (validation is not null)
        {
            return validation;
        }

        var name = request.Name!.Trim();
        var email = request.Email!.Trim();
        var birthDate = request.BirthDate!.Value;
        var birthDateValue = birthDate.ToDateTime(TimeOnly.MinValue);

        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql =
            """
            UPDATE author
            SET name = @Name, email = @Email, birthdate = @BirthDate
            WHERE id = @Id
            RETURNING id, name, email, birthdate AS BirthDate;
            """;
        var command = new CommandDefinition(
            sql,
            new { Id = id, Name = name, Email = email, BirthDate = birthDateValue });
        var updated = await connection.QuerySingleOrDefaultAsync<Author>(command);

        if (updated is null)
        {
            return CreateErrorResponse("Author not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = updated
        };
    }

    public async Task<BaseResponse> DeleteAuthorAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "DELETE FROM author WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id });
        var rowsAffected = await connection.ExecuteAsync(command);

        if (rowsAffected == 0)
        {
            return CreateErrorResponse("Author not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = new { Message = "Author deleted successfully." }
        };
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

    private static BaseResponse? ValidateAuthor(string? name, string? email, DateOnly? birthDate)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || birthDate is null)
        {
            return CreateErrorResponse("Name, email, and birth date are required.", HttpStatusCode.BadRequest);
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
}
