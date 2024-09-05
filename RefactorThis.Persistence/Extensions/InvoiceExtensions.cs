using RefactorThis.Persistence.Entities;
using System.Linq;

namespace RefactorThis.Persistence.Extensions
{
    public static class InvoiceExtensions
    {
        /// <summary>
        /// Checks if there are previous payments in the invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public static bool HasPayments(this Invoice invoice)
        {
            return invoice.Payments != null && invoice.Payments.Any();
        }

        /// <summary>
        /// Retrieves the remaining balance of an invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public static decimal GetRemainingBalance(this Invoice invoice)
        {
            return invoice.Amount - invoice.Payments.Sum(p => p.Amount);
        }
    }
}
