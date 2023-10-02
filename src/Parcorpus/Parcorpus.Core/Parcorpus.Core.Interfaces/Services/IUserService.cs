using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface IUserService
{
    Task<User> GetUserById(Guid userId);

    Task<Paged<SearchHistoryRecord>> GetUserSearchHistory(Guid userId, PaginationParameters paging);
}