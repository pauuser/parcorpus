using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Parcorpus.UnitTests.Common.Helpers;

public interface IDatabaseFacade
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}

public class MyDatabaseFacade : IDatabaseFacade
{
    private readonly DatabaseFacade _database;

    public MyDatabaseFacade(DatabaseFacade database)
    {
        _database = database;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return _database.BeginTransactionAsync(cancellationToken);
    }
}

