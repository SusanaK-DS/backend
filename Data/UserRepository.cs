using Backend.Models;
using Dapper;
using Npgsql;

namespace Backend.Data;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            "SELECT id, name, email FROM library_users ORDER BY id",
            cancellationToken: cancellationToken);
        var rows = await conn.QueryAsync<User>(cmd);
        return rows.ToList();
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            "SELECT id, name, email FROM library_users WHERE id = @Id",
            new { Id = id },
            cancellationToken: cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<User>(cmd);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            "SELECT id, name, email FROM library_users WHERE LOWER(email) = LOWER(@Email)",
            new { Email = email },
            cancellationToken: cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<User>(cmd);
    }

    public async Task<User> CreateAsync(string name, string email, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            """
            INSERT INTO library_users (name, email)
            VALUES (@Name, @Email)
            RETURNING id, name, email;
            """,
            new { Name = name, Email = email },
            cancellationToken: cancellationToken);
        return await conn.QuerySingleAsync<User>(cmd);
    }

    public async Task<User?> UpdateAsync(int id, string name, string email, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            """
            UPDATE library_users
            SET name = @Name, email = @Email
            WHERE id = @Id
            RETURNING id, name, email;
            """,
            new { Id = id, Name = name, Email = email },
            cancellationToken: cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<User>(cmd);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        var cmd = new CommandDefinition(
            "DELETE FROM library_users WHERE id = @Id",
            new { Id = id },
            cancellationToken: cancellationToken);
        var affected = await conn.ExecuteAsync(cmd);
        return affected > 0;
    }
}
