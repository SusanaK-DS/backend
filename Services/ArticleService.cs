using Backend.Models;
using Dapper;
using Npgsql;
using System.Net;

namespace Backend.Services;

public class ArticleService : IArticleService
{
    private readonly NpgsqlDataSource _dataSource;

    public ArticleService(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<BaseResponse> GetArticlesAsync()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "SELECT id, title, author, publisheddate AS PublishedDate FROM article ORDER BY id";
        var command = new CommandDefinition(sql);
        var articles = await connection.QueryAsync<Article>(command);

        return new BaseResponse
        {
            Data = articles.ToList()
        };
    }

    public async Task<BaseResponse> GetArticleByIdAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "SELECT id, title, author, publisheddate AS PublishedDate FROM article WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id });
        var article = await connection.QuerySingleOrDefaultAsync<Article>(command);

        if (article is null)
        {
            return CreateErrorResponse("Article not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = article
        };
    }

    public async Task<BaseResponse> CreateArticleAsync(CreateArticleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.Author) ||
            request.PublishedDate is null)
        {
            return CreateErrorResponse("Title, author, and published date are required.", HttpStatusCode.BadRequest);
        }

        var title = request.Title.Trim();
        var author = request.Author.Trim();
        var publishedDate = request.PublishedDate.Value;
        var publishedDateValue = publishedDate.ToDateTime(TimeOnly.MinValue);

        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql =
            """
            INSERT INTO article (title, author, publisheddate)
            VALUES (@Title, @Author, @PublishedDate)
            RETURNING id, title, author, publisheddate AS PublishedDate;
            """;
        var command = new CommandDefinition(
            sql,
            new { Title = title, Author = author, PublishedDate = publishedDateValue });
        var created = await connection.QuerySingleAsync<Article>(command);

        return new BaseResponse
        {
            Data = created,
            HttpStatus = HttpStatusCode.Created
        };
    }

    public async Task<BaseResponse> UpdateArticleAsync(int id, UpdateArticleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.Author) ||
            request.PublishedDate is null)
        {
            return CreateErrorResponse("Title, author, and published date are required.", HttpStatusCode.BadRequest);
        }

        var title = request.Title.Trim();
        var author = request.Author.Trim();
        var publishedDate = request.PublishedDate.Value;
        var publishedDateValue = publishedDate.ToDateTime(TimeOnly.MinValue);

        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql =
            """
            UPDATE article
            SET title = @Title, author = @Author, publisheddate = @PublishedDate
            WHERE id = @Id
            RETURNING id, title, author, publisheddate AS PublishedDate;
            """;
        var command = new CommandDefinition(
            sql,
            new { Id = id, Title = title, Author = author, PublishedDate = publishedDateValue });
        var updated = await connection.QuerySingleOrDefaultAsync<Article>(command);

        if (updated is null)
        {
            return CreateErrorResponse("Article not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = updated
        };
    }

    public async Task<BaseResponse> DeleteArticleAsync(int id)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        var sql = "DELETE FROM article WHERE id = @Id";
        var command = new CommandDefinition(sql, new { Id = id });
        var rowsAffected = await connection.ExecuteAsync(command);

        if (rowsAffected == 0)
        {
            return CreateErrorResponse("Article not found.", HttpStatusCode.NotFound);
        }

        return new BaseResponse
        {
            Data = new { Message = "Article deleted successfully." }
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
