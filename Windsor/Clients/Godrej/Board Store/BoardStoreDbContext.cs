using System.Data.Entity;
using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Windsor.Context;

namespace Corno.Web.Windsor.Clients.Godrej.Board_Store;

public class WebDbContext : CornoDbContext
{
    public WebDbContext()
        : base("Name=CornoContext")
    {
    }

    public WebDbContext(string connectionString)
        : base(connectionString)
    {
    }

        
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // Call base class function
        base.OnModelCreating(modelBuilder);

        // Masters
        modelBuilder.Entity<Item>().ToTable(nameof(Item));

        modelBuilder.Entity<Location>().ToTable(nameof(Location));
        modelBuilder.Entity<LocationStack>().ToTable(nameof(LocationStack));
        
        modelBuilder.Entity<Request>().ToTable(nameof(Request));
        modelBuilder.Entity<Stack>().ToTable(nameof(Stack));
        modelBuilder.Entity<StackItem>().ToTable(nameof(StackItem));

        modelBuilder.Entity<Movement>().ToTable(nameof(Movement));
    }
}