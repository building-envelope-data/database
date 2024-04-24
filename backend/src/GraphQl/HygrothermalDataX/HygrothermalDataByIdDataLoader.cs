using Database.Data;
using Database.GraphQl.Entities;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class HygrothermalDataByIdDataLoader
    : EntityByIdDataLoader<HygrothermalData>
{
    public HygrothermalDataByIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory
    )
        : base(
            batchScheduler,
            options,
            dbContextFactory,
            dbContext => dbContext.HygrothermalData
        )
    {
    }
}