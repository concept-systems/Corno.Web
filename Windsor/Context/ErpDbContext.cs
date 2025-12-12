using System;
using System.Data.Entity;
using Corno.Web.Areas.Kitchen.Models.Put_To_Light;
using Corno.Web.Models.Grn;
using Corno.Web.Models.Location;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Models.Sales;

namespace Corno.Web.Windsor.Context;

public class ErpDbContext : CornoDbContext
{
    public ErpDbContext(string connectionString)
        : base(connectionString)
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        //RegisterMaps(modelBuilder);

        // Call base class function
        base.OnModelCreating(modelBuilder);

        // Masters
        modelBuilder.Entity<Product>().ToTable(nameof(Product));
        modelBuilder.Entity<ProductItemDetail>().ToTable(nameof(ProductItemDetail));
        modelBuilder.Entity<ProductPacketDetail>().ToTable(nameof(ProductPacketDetail));
        modelBuilder.Entity<ProductStockDetail>().ToTable(nameof(ProductStockDetail));
        modelBuilder.Entity<Item>().ToTable(nameof(Item));
        modelBuilder.Entity<ItemProcessDetail>().ToTable(nameof(ItemProcessDetail));
        modelBuilder.Entity<ItemMachineDetail>().ToTable(nameof(ItemMachineDetail));
        modelBuilder.Entity<ItemPacketDetail>().ToTable(nameof(ItemPacketDetail));
        modelBuilder.Entity<Customer>().ToTable(nameof(Customer));
        modelBuilder.Entity<CustomerUserDetail>().ToTable(nameof(CustomerUserDetail));
        modelBuilder.Entity<CustomerProductDetail>().ToTable(nameof(CustomerProductDetail));
        modelBuilder.Entity<Supplier>().ToTable(nameof(Supplier));
        modelBuilder.Entity<SupplierItemDetail>().ToTable(nameof(SupplierItemDetail));
        modelBuilder.Entity<Employee>().ToTable(nameof(Employee));
        modelBuilder.Entity<Location>().ToTable(nameof(Location));
        modelBuilder.Entity<LocationItemDetail>().ToTable(nameof(LocationItemDetail));
        modelBuilder.Entity<LocationUserDetail>().ToTable(nameof(LocationUserDetail));
        modelBuilder.Entity<LocationStockDetail>().ToTable(nameof(LocationStockDetail));

        // Store
        modelBuilder.Entity<Grn>().ToTable(nameof(Grn));
        modelBuilder.Entity<GrnDetail>().ToTable(nameof(GrnDetail));

        modelBuilder.Entity<Carton>().ToTable(nameof(Carton));
        modelBuilder.Entity<CartonDetail>().ToTable(nameof(CartonDetail));
        modelBuilder.Entity<CartonRackingDetail>().ToTable(nameof(CartonRackingDetail));
        modelBuilder.Entity<TrolleyConfig>().ToTable(nameof(TrolleyConfig));
        modelBuilder.Entity<TrolleyConfigDetail>().ToTable(nameof(TrolleyConfigDetail));
        modelBuilder.Entity<Label>().ToTable(nameof(Label));
        modelBuilder.Entity<LabelDetail>().ToTable(nameof(LabelDetail));
        modelBuilder.Entity<Plan>().ToTable(nameof(Plan));
        modelBuilder.Entity<PlanItemDetail>().ToTable(nameof(PlanItemDetail));
        modelBuilder.Entity<PackingList>().ToTable(nameof(PackingList));
        modelBuilder.Entity<PackingListDetail>().ToTable(nameof(PackingListDetail));
        modelBuilder.Entity<PlanPacketDetail>().ToTable(nameof(PlanPacketDetail));

        // Sales
        modelBuilder.Entity<SalesOrder>().ToTable(nameof(SalesOrder));
        modelBuilder.Entity<SalesOrderDetail>().ToTable(nameof(SalesOrderDetail));
        modelBuilder.Entity<SalesInvoice>().ToTable(nameof(SalesInvoice));
        modelBuilder.Entity<SalesInvoiceDetail>().ToTable(nameof(SalesInvoiceDetail));
        modelBuilder.Entity<SalesReturn>().ToTable(nameof(SalesReturn));
        modelBuilder.Entity<SalesReturnDetail>().ToTable(nameof(SalesReturnDetail));
    }

    protected override void Dispose(bool disposing)
    {
        Console.WriteLine(@"DbContext disposed");
        base.Dispose(disposing);
    }
}