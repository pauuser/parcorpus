using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Converters;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Repositories;

public class UserRepository : BaseRepository<UserRepository>, IUserRepository
{
    private readonly ParcorpusDbContext _context;
    
    public UserRepository(ILogger<UserRepository> logger, ParcorpusDbContext context) : base(logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<User> GetUserById(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.CountryNavigation)
                .Include(u => u.NativeLanguageNavigation)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user is null)
            {
                Logger.LogError("User with id = {userId} doesn't exist.", userId);
                throw new NotFoundException($"User with id = {userId} doesn't exist.");
            }

            Logger.LogInformation("User with id = {userId} retrieved successfully.", userId);

            return UserConverter.ConvertDbModelToAppModel(user)!;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting user by id = {userId}", userId);
            throw new UserRepositoryException($"Error during getting user by id = {userId}", ex);
        }
    }

    public async Task<User> RegisterUser(User newUser)
    {
        try
        {
            var countryName = newUser.Country.CountryName;
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name.Equals(countryName));
            if (country is null)
            {
                Logger.LogError("Country with name {countryName} is not found.", countryName);
                throw new NotFoundException($"Country with name {countryName} is not found.");
            }

            var language =
                await _context.Languages.FirstOrDefaultAsync(l => l.ShortName == newUser.NativeLanguage.ShortName);
            if (language is null)
            {
                Logger.LogError("Language with short name {languageShortName} is not found.",
                    newUser.NativeLanguage.ShortName);
                throw new NotFoundException(
                    $"Language with short name {newUser.NativeLanguage.ShortName} is not found.");
            }

            var userDb = new UserDbModel(userId: Guid.Empty,
                name: newUser.Name.Name,
                surname: newUser.Name.Surname,
                email: newUser.Email.Address,
                country: country.CountryId,
                nativeLanguage: language.LanguageId,
                passwordHash: newUser.PasswordHash);
            var entry = await _context.Users.AddAsync(userDb);
            var entity = entry.Entity;

            await _context.SaveChangesAsync();
            Logger.LogInformation("User {id} is added", entity.UserId);

            entity.CountryNavigation = country;
            entity.NativeLanguageNavigation = language;

            return UserConverter.ConvertDbModelToAppModel(entity);
        }
        catch (NotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogInformation(ex, "Error during registering user");
            throw new UserRepositoryException("Error during registering user", ex);
        }
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        try
        {
            var user = await _context.Users
                    .Include(u => u.NativeLanguageNavigation)
                    .Include(u => u.CountryNavigation)
                    .FirstOrDefaultAsync(u => u.Email == email);
            
            Logger.LogInformation("Executed extraction for user email {email}", email);

            return UserConverter.ConvertDbModelToAppModel(user);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting user by email {email}", email);
            throw new UserRepositoryException($"Error during getting user by email {email}", ex);
        }
    }

    public async Task<User> UpdateUser(User newUser)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == newUser.UserId);
            if (user is null)
            {
                Logger.LogError("User with id {userId} is not found.", newUser.UserId);
                throw new NotFoundException($"User with id {newUser.UserId} is not found.");
            }

            var countryName = newUser.Country.CountryName;
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name.Equals(countryName));
            if (country is null)
            {
                Logger.LogError("Country with name {countryName} is not found.", countryName);
                throw new NotFoundException($"Country with name {countryName} is not found.");
            }

            var language =
                await _context.Languages.FirstOrDefaultAsync(l => l.ShortName == newUser.NativeLanguage.ShortName);
            if (language is null)
            {
                Logger.LogError("Language with short name {languageShortName} is not found.",
                    newUser.NativeLanguage.ShortName);
                throw new NotFoundException(
                    $"Language with short name {newUser.NativeLanguage.ShortName} is not found.");
            }

            user.Country = country.CountryId;
            user.CountryNavigation = country;
            user.Name = newUser.Name.Name;
            user.Surname = newUser.Name.Surname;
            user.NativeLanguage = language.LanguageId;
            user.NativeLanguageNavigation = language;

            await _context.SaveChangesAsync();

            return UserConverter.ConvertDbModelToAppModel(user)!;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during updating user");
            throw new UserRepositoryException($"Error during updating user", ex);
        }
    }
}