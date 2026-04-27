using Backend.Models;

namespace Backend.Services;

public interface IArticleService
{
    Task<BaseResponse> GetArticlesAsync();
    Task<BaseResponse> GetArticleByIdAsync(int id);
    Task<BaseResponse> CreateArticleAsync(CreateArticleRequest request);
    Task<BaseResponse> UpdateArticleAsync(int id, UpdateArticleRequest request);
    Task<BaseResponse> DeleteArticleAsync(int id);
}
