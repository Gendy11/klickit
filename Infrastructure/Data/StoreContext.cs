using Microsoft.EntityFrameworkCore;
using Core.Entities;
using System.Reflection;
using Core.Entities.OrderAggregate;
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace Infrastructure.Data
{
    public class StoreContext:DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get;set;}
        public DbSet<ProductType> ProductTypes { get;set;}
        public DbSet<Order> Orders { get;set;}
        public DbSet<OrderItem> OrderItems { get;set;}
        public DbSet<DeliveryMethod> DeliveryMethods { get;set;}

        //creates migration so we override it to look for our configs.
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
            modelbuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach(var entityType in modelbuilder.Model.GetEntityTypes())
                {
                    var properties=entityType.ClrType.GetProperties().Where(p=>p.PropertyType==typeof(decimal));
                    var dateTimeProperties=entityType.ClrType.GetProperties().Where(p=>p.PropertyType==typeof(DateTimeOffset));
                    foreach(var property in properties)
                    {
                        modelbuilder.Entity(entityType.Name).Property(property.Name).
                        HasConversion<double>();
                    }
                    foreach(var property in dateTimeProperties)
                    {
                        modelbuilder.Entity(entityType.Name).Property(property.Name)
                            .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }
        }
    }
}
