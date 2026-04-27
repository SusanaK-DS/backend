using Backend.Models;

namespace Backend.Services;

public interface IAuthorService
{
    Task<BaseResponse> GetAuthorsAsync();
    Task<BaseResponse> GetAuthorByIdAsync(int id);
    Task<BaseResponse> CreateAuthorAsync(CreateAuthorRequest request);
    Task<BaseResponse> UpdateAuthorAsync(int id, UpdateAuthorRequest request);
    Task<BaseResponse> DeleteAuthorAsync(int id);
}
