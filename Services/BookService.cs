using Backend.Models;
using Dapper;
using Npgsql;
using System.Net;

namespace Backend.Services;

public class BookService : IBookService
{
    private readonly NpgsqlDataSource _dataSource;

    public BookService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<BaseResponse> GetBooksAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var sql = "SELECT id, title, author FROM books ORDER BY id";
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var books = await connection.QueryAsync<Book>(command);

        return new BaseResponse
        {
            Data = books.ToList()
        };
    }

    public async Task<BaseResponse> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var sql = "SELECT id, title, author FROM books WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        var book = await connection.QuerySingleOrDefaultAsync<Book>(command);

        if (book is null)
        {
            return CreateErrorResponse("Book not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = book
        };
    }

    public async Task<BaseResponse> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Author))
        {
            return CreateErrorResponse("Title and author are required.", HttpStatusCode.BadRequest);
        }

        var title = request.Title.Trim();
        var author = request.Author.Trim();

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var sql =
            """
            INSERT INTO books (title, author)
            VALUES (@Title, @Author)
            RETURNING id, title, author;
            """;
        var command = new CommandDefinition(sql, new { Title = title, Author = author }, cancellationToken: cancellationToken);
        var created = await connection.QuerySingleAsync<Book>(command);

        return new BaseResponse
        {
            Data = created,
            HttpStatus = HttpStatusCode.Created
        };
    }

    public async Task<BaseResponse> UpdateBookAsync(int id, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Author))
        {
            return CreateErrorResponse("Title and author are required.", HttpStatusCode.BadRequest);
        }

        var title = request.Title.Trim();
        var author = request.Author.Trim();

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var sql =
            """
            UPDATE books
            SET title = @Title, author = @Author
            WHERE id = @Id
            RETURNING id, title, author;
            """;
        var command = new CommandDefinition(
            sql,
            new { Id = id, Title = title, Author = author },
            cancellationToken: cancellationToken);
        var updated = await connection.QuerySingleOrDefaultAsync<Book>(command);

        if (updated is null)
        {
            return CreateErrorResponse("Book not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = updated
        };
    }

    public async Task<BaseResponse> DeleteBookAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var sql = "DELETE FROM books WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        var rowsAffected = await connection.ExecuteAsync(command);

        if (rowsAffected == 0)
        {
            return CreateErrorResponse("Book not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = new { Message = "Book deleted successfully." }
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
}
