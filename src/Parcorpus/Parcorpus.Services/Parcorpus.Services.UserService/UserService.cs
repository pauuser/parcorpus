using Microsoft.Extensions.Logging;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.Services.UserService;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ISearchHistoryRepository _searchHistoryRepository;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, ISearchHistoryRepository searchHistoryRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _searchHistoryRepository = searchHistoryRepository ?? throw new ArgumentNullException(nameof(searchHistoryRepository));
    }

    public async Task<User> GetUserById(Guid userId)
    {
        _logger.LogInformation("Retrieving user with id = {userId}", userId);
        
        return await _userRepository.GetUserById(userId);
    }

    public async Task<Paged<SearchHistoryRecord>> GetUserSearchHistory(Guid userId, PaginationParameters paging)
    {
        _logger.LogInformation("Getting search history for user = {userId}", userId);
        var history = await _searchHistoryRepository.GetSearchHistory(userId, paging);
        if (!history.Items.Any())
        {
            _logger.LogError("User {userId} has empty search history", userId);
            throw new NotFoundException($"User {userId} has empty search history");
        }
        
        return history;
    }
}