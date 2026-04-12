using Backend.Models;

namespace Backend.Services;

public interface IUserService
{
    Task<BaseResponse> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<BaseResponse> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BaseResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<BaseResponse> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<BaseResponse> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    Task<BaseResponse> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
}
