using Microsoft.EntityFrameworkCore.Storage;

namespace Parcorpus.Core.Interfaces;

public interface ITransactionManager
{
    Task<IDbContextTransaction> BeginTransactionAsync();
}