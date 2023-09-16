using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Converters;

namespace Parcorpus.DataAccess.Repositories;

public class CredentialsRepository : BaseRepository<CredentialsRepository>, ICredentialsRepository
{
    private readonly ParcorpusDbContext _context;
    
    public CredentialsRepository(ParcorpusDbContext context, ILogger<CredentialsRepository> logger) : base(logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Credential> GetCredentials(Guid userId)
    {
        try
        {
            var credentials = await _context.Credentials.FirstOrDefaultAsync(c => c.UserId == userId);
            if (credentials is null)
            {
                Logger.LogError("Credentials for user {userId} are not found", userId);
                throw new NotFoundException($"Credentials for user {userId} are not found");
            }

            return CredentialsConverter.ConvertDbModelToAppModel(credentials);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting credentials for user {userId}", userId);
            throw new CredentialsRepositoryException($"Error during getting credentials for user {userId}");
        }
    }

    public async Task<Credential> CreateCredentials(Credential credential)
    {
        try
        {
            var credentialsDb = CredentialsConverter.ConvertAppModelToDbModel(credential);
            var entry = await _context.Credentials.AddAsync(credentialsDb);

            await _context.SaveChangesAsync();
            Logger.LogInformation("Credentials for user {userId} created", credential.UserId);

            return CredentialsConverter.ConvertDbModelToAppModel(entry.Entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating credentials for user {userId}", credential.UserId);
            throw new CredentialsRepositoryException($"Error during creating credentials for user {credential.UserId}");
        }
    }

    public async Task<Credential> UpdateRefreshToken(Guid userId, string newRefreshToken, DateTime expiresAtUtc)
    {
        try
        {
            var credentials = await _context.Credentials.FirstOrDefaultAsync(c => c.UserId == userId);
            if (credentials is null)
            {
                Logger.LogError("Credentials for user {userId} are not found", userId);
                throw new NotFoundException($"Credentials for user {userId} are not found");
            }
            
            credentials.RefreshToken = newRefreshToken;
            credentials.TokenExpiresAtUtc = expiresAtUtc;

            await _context.SaveChangesAsync();
            Logger.LogInformation("Credentials for user {userId} updated", userId);

            return CredentialsConverter.ConvertDbModelToAppModel(credentials);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during updating credentials for user {userId}", userId);
            throw new CredentialsRepositoryException($"Error during creating credentials for user {userId}");
        }
    }
}