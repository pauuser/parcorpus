using Microsoft.Extensions.Logging;

namespace Parcorpus.DataAccess.Repositories;

public abstract class BaseRepository<T>
{
    protected readonly ILogger<T> Logger;

    protected BaseRepository(ILogger<T> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
