using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InvoiceApi.Data.Repository;
using InvoiceApi.Data.Models;
using InvoiceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace InvoiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceRepository _repository;
        private readonly IExchangeService _exchangeService;
        private readonly ILogger _logger;

        public InvoicesController(IInvoiceRepository repository, IExchangeService exchangeService, ILoggerFactory logger)
        {
            _repository = repository;
            _exchangeService = exchangeService;
            _logger = logger.CreateLogger(nameof(InvoicesController));
        }

        [HttpGet]
        public async Task<IEnumerable<Invoice>> GetInvoices()
        {
            return await _repository.GetInvoices();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(Guid id, [FromQuery] string currency = null)
        {
            var invoice = await _repository.GetInvoiceById(id);
            if (invoice == null)
            {
                _logger.LogWarning("GetInvoice({Id}) NOT FOUND", id);
                return NotFound();
            }

            if (!string.IsNullOrEmpty(currency))
            {
                var rate = await _exchangeService.GetRateAsync(invoice.Currency, currency);
                if (!rate.HasValue)
                {
                    _logger.LogWarning("GetRate({from}, {to}) NOT FOUND", invoice.Currency, currency);
                    return NotFound();
                }

                invoice.Currency = currency;
                invoice.Amount *= rate.Value;
            }
            
            return invoice;
        }

        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            invoice.InvoiceId = new Guid();
            _repository.InsertInvoice(invoice);
            try
            {
                await _repository.Save();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, invoice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(Guid id, Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                _logger.LogWarning("PutInvoice({Id}) BAD REQUEST", id);
                return BadRequest();
            }
            if (!_repository.ExistsInvoice(id))
            {
                _logger.LogWarning("PutInvoice({Id}) NOT FOUND", id);
                return NotFound();
            }

            _repository.UpdateInvoice(invoice);
            try
            {
                await _repository.Save();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            var invoice = await _repository.GetInvoiceById(id);
            if (invoice == null)
            {
                _logger.LogWarning("DeleteInvoice({Id}) NOT FOUND", id);
                return NotFound();
            }

            _repository.DeleteInvoice(invoice);
            try
            {
                await _repository.Save();
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return NoContent();
        }
    }
}