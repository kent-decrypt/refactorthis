using RefactorThis.Domain.Services;
using RefactorThis.Persistence.Extensions;
using RefactorThis.Persistence.Entities;
using RefactorThis.Persistence.Repositories;
using System;
using System.Linq;
using static RefactorThis.Domain.Common.Messages;

namespace RefactorThis.Domain
{
    /// <summary>
    /// Code implementation for the IInvoiceService
    /// </summary>
    internal class InvoiceService : IInvoiceService
	{
        private readonly IWriteRepository _writeRepository;

        public InvoiceService(IWriteRepository writeRepository)
		{
            _writeRepository = writeRepository;
        }

		public string ProcessPayment(Payment payment)
		{
			var invoice = _writeRepository.Payments
                .Where(i => i.Reference == payment.Reference)
                .Select(i => i.Invoice)
                .FirstOrDefault()
				?? throw new NullReferenceException(Invoices.Exceptions.INVOICE_NOT_FOUND);

            var responseMessage = Validate(invoice);

            // Check if there are previous payments
            responseMessage = invoice.Payments != null && invoice.Payments.Any()
                ? ProcessPartialPayment(invoice, payment)
                : ProcessInitialPayment(invoice, payment);

            _writeRepository.SaveChanges();

			return string.Empty;
		}

        /// <summary>
        /// Validates the invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private string Validate(Invoice invoice)
        {
            if (invoice.Amount <= 0)
            {
                if (!invoice.HasPayments())
                {
                    throw new InvalidOperationException(Invoices.Exceptions.INVALID_INVOICE);
                }

                return Invoices.NO_PAYMENT_NEEDED;
            }

            return string.Empty;
        }

        /// <summary>
        /// Processes the Partial payment of an invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        private string ProcessPartialPayment(Invoice invoice, Payment payment)
        {
            var paymentsTotal = invoice.Payments.Sum(i => i.Amount);
            var remainingBalance = invoice.Amount - invoice.AmountPaid;

            // Invoice is already fully paid
            if (invoice.Amount == paymentsTotal)
            {
                return Invoices.FULLY_PAID;
            }

            // The payment exceeds the remaining balance
            if (payment.Amount > remainingBalance)
            {
                return Invoices.Errors.EXCESS_PARTIAL_PAYMENT;
            }

            invoice.AddPayment(payment);

            return payment.Amount == remainingBalance
                ? Invoices.FINAL_PARTIAL_PAYMENT_RECEIVED
                : Invoices.ANOTHER_PARTIAL_PAYMENT_RECEIVED;
        }

        /// <summary>
        /// Processes the Initial payment of an invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string ProcessInitialPayment(Invoice invoice, Payment payment)
        {
            if (payment.Amount > invoice.Amount)
            {
                return Invoices.Errors.EXCESS_PAYMENT;
            }

            invoice.AddPayment(payment);
            
            return payment.Amount == invoice.Amount
                ? Invoices.FULL_PAYMENT_RECEIVED
                : Invoices.PARTIAL_PAYMENT_RECEIVED;
        }
	}
}