using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain.Services
{
    public interface IInvoiceService
    {
        /// <summary>
        /// Processes a payment
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        string ProcessPayment(Payment payment);
    }
}
