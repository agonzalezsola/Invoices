using System;
using Microsoft.EntityFrameworkCore;
using InvoiceApi.Data.Models;

namespace InvoiceApi.Data
{
    public class InvoiceContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }

        public InvoiceContext(DbContextOptions<InvoiceContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>().HasData(
                new Invoice
                {
                    InvoiceId = Guid.NewGuid(),
                    Description = "Initializated data 1",
                    Supplier = "Suplier 1",
                    Amount = 100m,
                    Currency = "EUR", 
                    DateIssued = DateTime.Now,
                }
            ); 
        }
    }
}
