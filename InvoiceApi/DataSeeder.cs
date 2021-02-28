using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InvoiceApi.Data;
using InvoiceApi.Data.Models;
using System;
using System.Linq;

namespace InvoiceApi
{
    public static class DataSeeder
    {
        public static IHost SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<InvoiceContext>();
                SeedInvoices(context);                
            }
            return host;
        }

        private static void SeedInvoices(InvoiceContext context)
        {
            if (!context.Invoices.Any())
            {
                context.Invoices.AddRange(new Invoice[]{
                    new Invoice {
                        InvoiceId = Guid.NewGuid(),
                        Description = "Invoice #1",
                        Supplier = "The Company",
                        Amount = 1150.5m,
                        Currency = "EUR",
                        DateIssued = DateTime.Parse("2021-01-01 10:20:30"),
                    },
                    new Invoice
                    {
                        InvoiceId = Guid.NewGuid(),
                        Description = "Invoice #2",
                        Supplier = "The Company",
                        Amount = 99.5m,
                        Currency = "USD",
                        DateIssued = DateTime.Parse("2019-10-10 13:30:01"),
                    },
                    new Invoice
                    {
                        InvoiceId = Guid.NewGuid(),
                        Description = "Invoice #3",
                        Supplier = "The Agency",
                        Amount = 25.0m,
                        Currency = "EUR",
                        DateIssued = DateTime.Parse("2021-05-01 11:25:11"),
                    },
                });
                context.SaveChanges();
            }
        }
    }
}
