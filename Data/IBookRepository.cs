using Backend.Models;

namespace Backend.Data;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Book> CreateAsync(string title, string author, CancellationToken cancellationToken = default);
    Task<Book?> UpdateAsync(int id, string title, string author, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
