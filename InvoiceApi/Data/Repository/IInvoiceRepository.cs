using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApi.Data.Models;

namespace InvoiceApi.Data.Repository
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetInvoices();
        Task<Invoice> GetInvoiceById(Guid id);
        bool ExistsInvoice(Guid id);
        void InsertInvoice(Invoice invoice);
        void UpdateInvoice(Invoice invoice);
        void DeleteInvoice(Invoice invoice);
        /// <exception cref="RepositoryException">Thrown when save fail</exception>
        Task Save();
    }
}
