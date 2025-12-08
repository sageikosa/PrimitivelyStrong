using Microsoft.EntityFrameworkCore;
using PrimitivelyStrong.Support;

namespace SampleUse;

/// <summary>
/// Represents a database context that applies property configuration conventions using the specified configurators.
/// </summary>
/// <remarks>This context applies each configurator to the model configuration builder when conventions are
/// configured. Use this class to centralize and extend property configuration logic for entity models.</remarks>
/// <param name="configurators">A collection of property configurators used to customize model property conventions during context initialization.
/// Cannot be null.</param>
public class SampleDbContext(
    IEnumerable<IPropertyConfigurator> configurators
    ) : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        foreach (var _conf in configurators)
        {
            _conf.ConfigureProperties(configurationBuilder);
        }
        base.ConfigureConventions(configurationBuilder);
    }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF will only ask for new sequence numbers once it's block of 100 runs out
        // this is shared per DbContext model (cached per process), so multiple DbContext
        // instances will share the same sequence
        // this also prevents call-outs to the DB for every insert just to get the next ID
        modelBuilder.HasSequence(nameof(EntityID)).StartsAt(10000).IncrementsBy(100);
        modelBuilder.HasSequence(nameof(TransactionID)).StartsAt(10000).IncrementsBy(100);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Gets or sets the collection of SampleEntity objects that can be queried from or written to the database.
    /// </summary>
    /// <remarks>Use this property to perform CRUD operations on SampleEntity records within the current
    /// context. Changes made to entities in this set are tracked by the context and persisted to the database when
    /// SaveChanges is called.</remarks>
    public DbSet<SampleEntity> SampleEntities { get; set; }

    /// <summary>
    /// Gets or sets the collection of sample transactions in the database context.
    /// </summary>
    /// <remarks>Use this property to query, add, update, or remove <see cref="SampleTransaction"/> entities
    /// within the context. Changes made to this collection are tracked by Entity Framework and persisted to the
    /// database when <c>SaveChanges</c> is called.</remarks>
    public DbSet<SampleTransaction> SampleTransactions { get; set; }
}
