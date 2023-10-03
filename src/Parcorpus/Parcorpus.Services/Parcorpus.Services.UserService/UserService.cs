using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.Services.UserService;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    
    private readonly IUserRepository _userRepository;
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    
    private readonly PagingConfiguration _pagingConfiguration;

    public UserService(ILogger<UserService> logger, 
        IUserRepository userRepository, 
        ISearchHistoryRepository searchHistoryRepository,
        IOptions<PagingConfiguration> pagingConfiguration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _searchHistoryRepository = searchHistoryRepository ?? throw new ArgumentNullException(nameof(searchHistoryRepository));
        _pagingConfiguration = pagingConfiguration.Value ?? throw new ArgumentNullException(nameof(pagingConfiguration));
    }

    public async Task<User> GetUserById(Guid userId)
    {
        _logger.LogInformation("Retrieving user with id = {userId}", userId);
        
        return await _userRepository.GetUserById(userId);
    }

    public async Task<Paged<SearchHistoryRecord>> GetUserSearchHistory(Guid userId, PaginationParameters paging)
    {
        if (paging.Specified && (paging.PageSize < _pagingConfiguration.MinPageSize || paging.PageSize > _pagingConfiguration.MaxPageSize))
        {
            _logger.LogError("Invalid paging to get user {userId} search history: {paging}", userId, paging);
            throw new InvalidPagingException($"Invalid paging to get user {userId} search history: {paging}");
        }
        
        _logger.LogInformation("Getting search history for user = {userId}", userId);
        var history = await _searchHistoryRepository.GetSearchHistory(userId, paging);
        if (!history.Items.Any())
        {
            _logger.LogError("User {userId} has empty search history", userId);
            throw new NotFoundException($"User {userId} has empty search history");
        }
        
        return history;
    }

    public async Task<User> UpdateUser(User initialUser, User patchedUser)
    {
        var patchPossible = initialUser.UserId == patchedUser.UserId &&
            Equals(initialUser.Email, patchedUser.Email) &&
            initialUser.PasswordHash == patchedUser.PasswordHash;
        if (!patchPossible)
        {
            _logger.LogError("Attempt to change password, email or userId from user {userId}", initialUser.UserId);
            throw new ImpossiblePatchException("Changing password, email or userId is not allowed");
        }

        var updatedUser = await _userRepository.UpdateUser(patchedUser);

        return updatedUser;
    }
}