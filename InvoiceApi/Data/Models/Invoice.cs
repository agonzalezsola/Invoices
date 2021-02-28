using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApi.Data.Models
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public string Supplier { get; set; }
        public DateTime DateIssued { get; set; }
        [Required]
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
