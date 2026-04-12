using Backend.Models;

namespace Backend.Services;

public interface IBookService
{
    Task<BaseResponse> GetBooksAsync();
    Task<BaseResponse> GetBookByIdAsync(int id);
    Task<BaseResponse> CreateBookAsync(CreateBookRequest request);
    Task<BaseResponse> UpdateBookAsync(int id, UpdateBookRequest request);
    Task<BaseResponse> DeleteBookAsync(int id);
}
