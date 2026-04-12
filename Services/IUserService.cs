using Backend.Models;

namespace Backend.Services;

public interface IUserService
{
    Task<BaseResponse> GetUsersAsync();
    Task<BaseResponse> GetUserByIdAsync(int id);
    Task<BaseResponse> CreateUserAsync(CreateUserRequest request);
    Task<BaseResponse> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<BaseResponse> DeleteUserAsync(int id);
    Task<BaseResponse> LoginAsync(LoginUserRequest request);
}
