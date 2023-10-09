using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;

namespace Parcorpus.UnitTests.Common.Helpers;

public static class EntityHelper
{
    public static EntityEntry<T> GetMockEntityEntry<T>(T data) where T: class
    {
        return new EntityEntry<T>(new InternalEntityEntry(new Mock<IStateManager>().Object,
            new RuntimeEntityType(typeof(T).Name, typeof(T), false, null, null, null, ChangeTrackingStrategy.Snapshot,
                null, false, null), data));
    }
}