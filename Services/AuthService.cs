using Backend.Models;
using Dapper;
using Npgsql;
using System.Net;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly IUserService _userService;

    public AuthService(NpgsqlDataSource dataSource, IUserService userService)
    {
        _dataSource = dataSource;
        _userService = userService;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _userService.GetUserByUsername(username);
        if (response.Data is not User user)
        {
            return false;
        }

        return user.Password == password; // replace with hashed password verification later
    }
}
