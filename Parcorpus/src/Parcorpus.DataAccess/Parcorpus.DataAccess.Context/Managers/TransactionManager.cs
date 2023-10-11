using Microsoft.EntityFrameworkCore.Storage;
using Parcorpus.Core.Interfaces;

namespace Parcorpus.DataAccess.Context.Managers;

public class TransactionManager : ITransactionManager
{
    private readonly ParcorpusDbContext _context;

    public TransactionManager(ParcorpusDbContext context)
    {
        _context = context;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }
}