using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Moq;
using InvoiceApi.Data.Repository;
using InvoiceApi.Data.Models;
using InvoiceApi.Data;

namespace InvoiceApiTest
{
    public class EntityFrameworkInvoiceRepositoryTest
    {
        [Test]
        public async Task GetInvoices_Empty()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "GetInvoices_Empty")
                .Options;

            using var context = new InvoiceContext(options);
            var repository = new EntityFrameworkInvoiceRepository(context);
            var invoices = await repository.GetInvoices();

            Assert.IsNotNull(invoices);
            Assert.AreEqual(0, invoices.Count());
        }

        [Test]
        public async Task GetInvoices_All()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "GetInvoices_All")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                context.Invoices.AddRange(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 1.1m },
                    new Invoice { InvoiceId = new Guid("777a1a81-3115-474d-9c43-ed103a091eba"), Amount = 2.2m },
                    new Invoice { InvoiceId = new Guid("872db39e-19f5-4be9-8870-29c80c93eb4d"), Amount = 3.3m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                var invoices = await repository.GetInvoices();
                Assert.IsNotNull(invoices);
                Assert.AreEqual(3, invoices.Count());
            }
        }

        [Test]
        public async Task GetInvoiceById_NotExists()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "GetInvoiceById_NotExists")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                var invoice = await repository.GetInvoiceById(new Guid("fe8c5932-c6cc-4db9-a2fb-fce6fdefb40d"));
                Assert.IsNull(invoice);
            }
        }

        [Test]
        public async Task GetInvoiceById_Single()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "GetInvoiceById_Single")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                context.Invoices.AddRange(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 1.1m },
                    new Invoice { InvoiceId = new Guid("777a1a81-3115-474d-9c43-ed103a091eba"), Amount = 2.2m },
                    new Invoice { InvoiceId = new Guid("872db39e-19f5-4be9-8870-29c80c93eb4d"), Amount = 3.3m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                var invoice = await repository.GetInvoiceById(new Guid("777a1a81-3115-474d-9c43-ed103a091eba"));
                Assert.IsNotNull(invoice);
                Assert.AreEqual(2.2m, invoice.Amount);
            }
        }

        [Test]
        public void ExistsInvoice_NotExists()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "ExistsInvoice_NotExists")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                var exists = repository.ExistsInvoice(new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"));
                Assert.IsFalse(exists);
            }
        }

        [Test]
        public void ExistsInvoice_Exists()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "ExistsInvoice_Exists")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                context.Invoices.AddRange(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 1.1m },
                    new Invoice { InvoiceId = new Guid("777a1a81-3115-474d-9c43-ed103a091eba"), Amount = 2.2m },
                    new Invoice { InvoiceId = new Guid("872db39e-19f5-4be9-8870-29c80c93eb4d"), Amount = 3.3m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                var exists = repository.ExistsInvoice(new Guid("777a1a81-3115-474d-9c43-ed103a091eba"));
                Assert.IsTrue(exists);
            }
        }

        [Test]
        public void InsertInvoice_CanInsert()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "InsertInvoice_CanInsert")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                repository.InsertInvoice(
                    new Invoice { InvoiceId = new Guid("777a1a81-3115-474d-9c43-ed103a091eba"), Amount = 1.1m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                Assert.AreEqual(1, context.Invoices.Count());
                Assert.AreEqual(new Guid("777a1a81-3115-474d-9c43-ed103a091eba"), context.Invoices.Single().InvoiceId);
                Assert.AreEqual(1.1m, context.Invoices.Single().Amount);
            }
        }

        [Test]
        public void UpdateInvoice_CanUpdate()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "UpdateInvoice_CanUpdate")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                context.Invoices.Add(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 1.1m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                repository.UpdateInvoice(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 2.2m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                Assert.AreEqual(2.2m, context.Invoices.Single().Amount);
            }
        }

        [Test]
        public void DeleteInvoice_CanDelete()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "DeleteInvoice_CanDelete")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                context.Invoices.Add(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 1.1m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                var repository = new EntityFrameworkInvoiceRepository(context);
                repository.DeleteInvoice(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 1.1m }
                );
                context.SaveChanges();
            }

            using (var context = new InvoiceContext(options))
            {
                Assert.AreEqual(0, context.Invoices.Count());
            }
        }

        [Test]
        public async Task Save_CanSaveContext()
        {
            var options = new DbContextOptionsBuilder<InvoiceContext>()
                .UseInMemoryDatabase(databaseName: "Save_CanSaveContext")
                .Options;

            using (var context = new InvoiceContext(options))
            {
                context.Invoices.Add(
                    new Invoice { InvoiceId = new Guid("050b2d01-67a4-417c-9ef2-625aaa4d81b9"), Amount = 1.1m }
                );
                var repository = new EntityFrameworkInvoiceRepository(context);
                await repository.Save();
            }

            using (var context = new InvoiceContext(options))
            {
                Assert.AreEqual(1, context.Invoices.Count());
            }
        }
    } 
}