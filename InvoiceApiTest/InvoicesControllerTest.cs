using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Moq;
using InvoiceApi.Controllers;
using InvoiceApi.Data.Repository;
using InvoiceApi.Data.Models;
using InvoiceApi.Services;

namespace InvoiceApiTest
{
    public class InvoicesControllerTest
    {
        private readonly string DefaultCurrency = "EUR";
        private readonly decimal DefaultExcangeRate = 1.5m;

        [Test]
        public async Task GetInvoices_Empty()
        {
            var repository = new Mock<IInvoiceRepository>();
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.GetInvoices();

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetInvoices_SingleElement()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.GetInvoices())
                .ReturnsAsync(new[] { CreateTestInvoce() });
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.GetInvoices();

            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(TestHelper.AreEqualByJson(CreateTestInvoce(), result.First()));
        }

        [Test]
        public async Task GetInvoice_Empty()
        {
            var repository = new Mock<IInvoiceRepository>();
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.GetInvoice(new Guid());

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetInvoice_Exists()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.GetInvoiceById(It.IsAny<Guid>()))
                .ReturnsAsync(CreateTestInvoce());
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.GetInvoice(new Guid());

            Assert.IsTrue(TestHelper.AreEqualByJson(CreateTestInvoce(), result.Value));
        }

        [Test]
        public async Task GetInvoice_FailExchangeRate()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.GetInvoiceById(It.IsAny<Guid>()))
                .ReturnsAsync(CreateTestInvoce());
            var exchageService = new Mock<IExchangeService>();
            exchageService.Setup(x => x.GetRateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((decimal?)null);
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.GetInvoice(new Guid(), "EUR");

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetInvoice_ExchangeRate()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.GetInvoiceById(It.IsAny<Guid>()))
                .ReturnsAsync(CreateTestInvoce());
            var exchageService = new Mock<IExchangeService>();
            exchageService.Setup(x => x.GetRateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(DefaultExcangeRate);
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.GetInvoice(new Guid(), DefaultCurrency);

            var invoiceTest = CreateTestInvoce();
            invoiceTest.Currency = DefaultCurrency;
            invoiceTest.Amount *= DefaultExcangeRate;
            Assert.IsTrue(TestHelper.AreEqualByJson(invoiceTest, result.Value));
        }

        [Test]
        public async Task PostInvoice_FailSave()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.Save())
                .ThrowsAsync(new RepositoryException());
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.PostInvoice(new Invoice());

            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var objectResult = (ObjectResult)result.Result;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Test]
        public async Task PostInvoice_Save()
        {
            var repository = new Mock<IInvoiceRepository>();
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.PostInvoice(CreateTestInvoce());

            repository.Verify(x => x.InsertInvoice(It.IsAny<Invoice>()), Times.Once);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createAtActionResult = (CreatedAtActionResult)result.Result;
            Assert.IsInstanceOf<Invoice>(createAtActionResult.Value);
            var invoiceReturned = (Invoice)createAtActionResult.Value;
            var invoiceTest = CreateTestInvoce();
            invoiceTest.InvoiceId = invoiceReturned.InvoiceId;
            Assert.IsTrue(TestHelper.AreEqualByJson(invoiceTest, invoiceReturned));
            //TODO: comprobar que el GUID es nuevo??
        }

        [Test]
        public async Task PutInvoice_BadId()
        {
            var repository = new Mock<IInvoiceRepository>();
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.PutInvoice(Guid.NewGuid(), new Invoice());

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task PutInvoice_NotExists()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.ExistsInvoice(It.IsAny<Guid>()))
                .Returns(false);
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var invoice = CreateTestInvoce();
            var result = await controler.PutInvoice(invoice.InvoiceId, invoice);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task PutInvoice_FailSave()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.ExistsInvoice(It.IsAny<Guid>()))
                .Returns(true);
            repository.Setup(x => x.Save())
                .ThrowsAsync(new RepositoryException());
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var invoice = CreateTestInvoce();
            var result = await controler.PutInvoice(invoice.InvoiceId, invoice);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Test]
        public async Task PutInvoice_Save()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.ExistsInvoice(It.IsAny<Guid>()))
                .Returns(true);
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var invoice = CreateTestInvoce();
            var result = await controler.PutInvoice(invoice.InvoiceId, invoice);

            repository.Verify(x => x.UpdateInvoice(It.IsAny<Invoice>()), Times.Once);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteInvoice_NotExists()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.GetInvoiceById(It.IsAny<Guid>()))
                .ReturnsAsync((Invoice)null);
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.DeleteInvoice(new Guid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteInvoice_SaveFail()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.GetInvoiceById(It.IsAny<Guid>()))
                .ReturnsAsync(new Invoice());
            repository.Setup(x => x.Save())
                .ThrowsAsync(new RepositoryException());
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.DeleteInvoice(new Guid());

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Test]
        public async Task DeleteInvoice_Save()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository.Setup(x => x.GetInvoiceById(It.IsAny<Guid>()))
                .ReturnsAsync(new Invoice());
            var exchageService = new Mock<IExchangeService>();
            var logger = TestHelper.CreateDefaultLogger();
            var controler = new InvoicesController(repository.Object, exchageService.Object, logger.Object);

            var result = await controler.DeleteInvoice(new Guid());

            repository.Verify(x => x.DeleteInvoice(It.IsAny<Invoice>()), Times.Once);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        private static Invoice CreateTestInvoce()
        {
            return new Invoice
            {
                InvoiceId = new Guid("c252e806-690c-40d7-bc46-e35960b08d77"),
                Amount = (decimal)100,
                Currency = "EUR",
                DateIssued = new DateTime(),
                Description = "Description",
                Supplier = "Suplier"
            };
        }   
    }
}