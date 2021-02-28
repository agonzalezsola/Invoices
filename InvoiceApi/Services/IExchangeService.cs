using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceApi.Services
{
    public interface IExchangeService
    {
        Task<decimal?> GetRateAsync(string from, string to);
    }
}
