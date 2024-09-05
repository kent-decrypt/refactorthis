using RefactorThis.Persistence.Enums;
using System;
using System.Collections.Generic;

namespace RefactorThis.Persistence.Entities
{
    /// <summary>
    /// Entity class for Invoices
    /// </summary>
    public class Invoice
    {
        private const decimal TaxRate = 0.14m;
        private Invoice() { }

        /// <summary>
        /// Creates a new instance of an Invoice
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="amountPaid"></param>
        /// <param name="taxAmount"></param>
        /// <param name="payments"></param>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public static Invoice Save(decimal amount, decimal amountPaid, InvoiceType invoiceType, decimal taxAmount, List<Payment> payments)
        {
            return new Invoice()
            {
                Amount = amount,
                AmountPaid = amountPaid,
                TaxAmount = taxAmount,
                Payments = payments,
                Type = invoiceType,
            };
        }

        /// <summary>
        /// Creates a new instance of an Invoice
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="amountPaid"></param>
        /// <param name="invoiceType"></param>
        /// <param name="taxAmount"></param>
        /// <returns></returns>
        public static Invoice Save(decimal amount, decimal amountPaid, InvoiceType invoiceType, decimal taxAmount)
        {
            return Save(amount, amountPaid, invoiceType, taxAmount, new List<Payment>());
        }

        /// <summary>
        /// Creates a new instance of an Invoice
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="amountPaid"></param>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public static Invoice Save(decimal amount, decimal amountPaid, InvoiceType invoiceType)
        {
            return Save(amount, amountPaid, invoiceType, 0);
        }

        /// <summary>
        /// Adds a payment to the current invoice
        /// </summary>
        /// <param name="payment"></param>
        public void AddPayment(Payment payment)
        {
            AmountPaid += payment.Amount;

            if(Type == InvoiceType.Commercial)
            {
                TaxAmount += payment.Amount * TaxRate;
            }

            Payments.Add(payment);
        }

        public decimal Amount { get; protected set; }
        public decimal AmountPaid { get; protected set; }
        public decimal TaxAmount { get; protected set; }
        public ICollection<Payment> Payments { get; protected set; }
        public InvoiceType Type { get; protected set; }
    }
}