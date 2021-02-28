using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using InvoiceApi.Data.Models;

namespace InvoiceApi.Data.Repository
{
    public class EntityFrameworkInvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceContext _context;

        public EntityFrameworkInvoiceRepository(InvoiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetInvoices()
        {
            return await _context.Invoices.AsNoTracking().ToListAsync();
        }

        public async Task<Invoice> GetInvoiceById(Guid id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public bool ExistsInvoice(Guid id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }

        public void InsertInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
        }

        public void UpdateInvoice(Invoice invoice)
        {
            _context.Entry(invoice).State = EntityState.Modified;
        }

        public void DeleteInvoice(Invoice invoice)
        {
            _context.Invoices.Remove(invoice);
        }

        public async Task Save()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Save Fail", ex);
            }
        }
    }
}
