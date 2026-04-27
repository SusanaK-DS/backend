using Backend.Models;

namespace Backend.Services;

public interface IAuthService
{
   Task<bool> LoginAsync(string username, string password);
}