using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using NUnit.Framework;
using RefactorThis.Domain.Common;
using RefactorThis.Domain.Services;
using RefactorThis.Persistence;
using RefactorThis.Persistence.Entities;
using RefactorThis.Persistence.Enums;
using RefactorThis.Persistence.Repositories;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
	public class InvoicePaymentProcessorTests
	{
		private Mock<IInvoiceService> _invoiceService;
		private Mock<IWriteRepository> _writeRepository;
		private Mock<IReadRepository> _readRepository;

        [SetUp]
        public void SetUp()
        {
            _writeRepository = new Mock<IWriteRepository>();
            _readRepository = new Mock<IReadRepository>();
            _invoiceService = new Mock<IInvoiceService>();
        }

        /// <summary>
        /// The system should throw an error if no invoice is found for the provided payment reference. (GPT-ed)
        /// </summary>
        [Test]
		public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
		{
			// Arrange
			var payment = new Payment();

			_invoiceService.Setup(service => service.ProcessPayment(payment))
				.Throws(new InvalidOperationException(Messages.Invoices.Exceptions.INVOICE_NOT_FOUND));

			var invoiceService = _invoiceService.Object;

			// Act
			var exceptionResult = Assert.Throws<InvalidOperationException>(() => invoiceService.ProcessPayment(payment));

            // Assert
            Assert.AreEqual(Messages.Invoices.Exceptions.INVOICE_NOT_FOUND, exceptionResult.Message);
		}

        /// <summary>
        /// The system should return a failure message if no payment is required. (GPT-ed)
        /// </summary>
        [Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
		{
			// Arrange
			var invoice = Invoice.Create(
				amount: 0, 
				amountPaid: 0, 
				invoiceType: InvoiceType.Standard, 
				taxAmount: 0, 
				payments: null);
            var payment = new Payment();

			_writeRepository.Setup(service => service.Invoices.Add(invoice))
				.Returns(invoice);
			_writeRepository.Setup(service => service.SaveChanges())
				.Verifiable();
			_invoiceService.Setup(service => service.ProcessPayment(payment))
				.Returns(Messages.Invoices.NO_PAYMENT_NEEDED);

			var writeRepository = _writeRepository.Object;
			var invoiceService = _invoiceService.Object;

			// Act
			writeRepository.Invoices.Add(invoice);
			writeRepository.SaveChanges();

			var result = invoiceService.ProcessPayment(payment);

			Assert.AreEqual(Messages.Invoices.NO_PAYMENT_NEEDED, result);
		}

        /// <summary>
        /// The system should return a failure message if the invoice has already been fully paid. (GPT-ed)
        /// </summary>
        [Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
		{
            // Arrange
            var invoice = Invoice.Create(
				amount: 10, 
				amountPaid: 10, 
				invoiceType: InvoiceType.Standard, 
				taxAmount: 0, 
				payments: new List<Payment>
                {
                    new Payment
                    {
                        Amount = 10
                    }
                });
            var payment = new Payment();

            _writeRepository.Setup(service => service.Invoices.Add(invoice))
                .Returns(invoice);
            _writeRepository.Setup(service => service.SaveChanges())
                .Verifiable();
            _invoiceService.Setup(service => service.ProcessPayment(payment))
                .Returns(Messages.Invoices.FULLY_PAID);

			var writeRepository = _writeRepository.Object;
			var invoiceService = _invoiceService.Object;

			// Act
			writeRepository.Invoices.Add(invoice);
			writeRepository.SaveChanges();

			var result = invoiceService.ProcessPayment(payment);

			// Assert
			Assert.AreEqual(Messages.Invoices.FULLY_PAID, result);
		}

        /// <summary>
        /// The system should return a failure message when a previous partial payment exists, but the amount paid exceeds the remaining balance. (GPT-ed)
        /// </summary>
        [Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
		{
            // Arrange
            var invoice = Invoice.Create(
                amount: 10,
                amountPaid: 5,
                invoiceType: InvoiceType.Standard,
                taxAmount: 0,
                payments: new List<Payment>
                {
                    new Payment
                    {
                        Amount = 5
                    }
                });
            var payment = new Payment() { Amount = 6 };

            _writeRepository.Setup(service => service.Invoices.Add(invoice))
                .Returns(invoice);
            _writeRepository.Setup(service => service.SaveChanges())
                .Verifiable();
            _invoiceService.Setup(service => service.ProcessPayment(payment))
                .Returns(Messages.Invoices.Errors.EXCESS_PARTIAL_PAYMENT);

            var writeRepository = _writeRepository.Object;
            var invoiceService = _invoiceService.Object;

            // Act
            writeRepository.Invoices.Add(invoice);
            writeRepository.SaveChanges();

            var result = invoiceService.ProcessPayment(payment);

			// Assert
            Assert.AreEqual(Messages.Invoices.Errors.EXCESS_PARTIAL_PAYMENT, result);
		}

        /// <summary>
        /// The system should return a failure message when no prior partial payment exists, but the amount paid exceeds the total invoice amount. (GPT-ed)
        /// </summary>
        [Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
		{
            // Arrange
            var invoice = Invoice.Create(
                amount: 5,
                amountPaid: 0,
                invoiceType: InvoiceType.Standard,
                taxAmount: 0,
                payments: new List<Payment>());
            var payment = new Payment() { Amount = 6 };

            _writeRepository.Setup(service => service.Invoices.Add(invoice))
                .Returns(invoice);
            _writeRepository.Setup(service => service.SaveChanges())
                .Verifiable();
            _invoiceService.Setup(service => service.ProcessPayment(payment))
                .Returns(Messages.Invoices.Errors.EXCESS_PAYMENT);

            var writeRepository = _writeRepository.Object;
            var invoiceService = _invoiceService.Object;

            // Act
            writeRepository.Invoices.Add(invoice);
            writeRepository.SaveChanges();

            var result = invoiceService.ProcessPayment(payment);

            // Assert
            Assert.AreEqual(Messages.Invoices.Errors.EXCESS_PAYMENT, result);
		}

        /// <summary>
        /// The system should return a message indicating the payment is fully completed when a previous partial payment exists, and the amount paid covers the remaining balance. (GPT-ed)
        /// </summary>
        [Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
		{
            // Arrange
            var invoice = Invoice.Create(
                amount: 10,
                amountPaid: 5,
                invoiceType: InvoiceType.Standard,
                taxAmount: 0,
                payments: new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 5
                    }
                });
            var payment = new Payment() { Amount = 5 };

            _writeRepository.Setup(service => service.Invoices.Add(invoice))
                .Returns(invoice);
            _writeRepository.Setup(service => service.SaveChanges())
                .Verifiable();
            _invoiceService.Setup(service => service.ProcessPayment(payment))
                .Returns(Messages.Invoices.FULL_PAYMENT_RECEIVED);

            var writeRepository = _writeRepository.Object;
            var invoiceService = _invoiceService.Object;

            // Act
            writeRepository.Invoices.Add(invoice);
            writeRepository.SaveChanges();

            var result = invoiceService.ProcessPayment(payment);

            // Assert
            Assert.AreEqual(Messages.Invoices.FULL_PAYMENT_RECEIVED, result);
		}

        /// <summary>
        /// The system should return a message indicating the payment is fully completed when there’s no previous partial payment and the amount paid matches the total invoice amount. (GPT-ed)
        /// </summary>
        [Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
		{
            // Arrange
            var invoice = Invoice.Create(
                amount: 10,
                amountPaid: 0,
                invoiceType: InvoiceType.Standard,
                taxAmount: 0,
                payments: new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 10
                    }
                });
            var payment = new Payment() { Amount = 10 };

            _writeRepository.Setup(service => service.Invoices.Add(invoice))
                .Returns(invoice);
            _writeRepository.Setup(service => service.SaveChanges())
                .Verifiable();
            _invoiceService.Setup(service => service.ProcessPayment(payment))
                .Returns(Messages.Invoices.FULLY_PAID);

            var writeRepository = _writeRepository.Object;
            var invoiceService = _invoiceService.Object;

            // Act
            writeRepository.Invoices.Add(invoice);
            writeRepository.SaveChanges();

            var result = invoiceService.ProcessPayment(payment);

            // Assert
            Assert.AreEqual(Messages.Invoices.FULLY_PAID, result);
		}

        /// <summary>
        /// The system should return a message indicating that the payment is only partially complete when a partial payment has been made and the amount paid is less than what is still owed (GPT-ed)
        /// </summary>
        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            // Arrange
            var invoice = Invoice.Create(
                amount: 10,
                amountPaid: 5,
                invoiceType: InvoiceType.Standard,
                taxAmount: 0,
                payments: new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 5
                    }
                });
            var payment = new Payment() { Amount = 1 };

            _writeRepository.Setup(service => service.Invoices.Add(invoice))
                .Returns(invoice);
            _writeRepository.Setup(service => service.SaveChanges())
                .Verifiable();
            _invoiceService.Setup(service => service.ProcessPayment(payment))
                .Returns(Messages.Invoices.ANOTHER_PARTIAL_PAYMENT_RECEIVED);

            var writeRepository = _writeRepository.Object;
            var invoiceService = _invoiceService.Object;

            // Act
            writeRepository.Invoices.Add(invoice);
            writeRepository.SaveChanges();

            var result = invoiceService.ProcessPayment(payment);

            // Assert
            Assert.AreEqual(Messages.Invoices.ANOTHER_PARTIAL_PAYMENT_RECEIVED, result);
        }

        /// <summary>
        /// The system should return a message indicating partial payment when no prior partial payment has been made, but the current payment is less than the total invoice amount. (GPT-ed)
        /// </summary>
        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            // Arrange
            var invoice = Invoice.Create(
                amount: 10,
                amountPaid: 0,
                invoiceType: InvoiceType.Standard,
                taxAmount: 0,
                payments: new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 5
                    }
                });
            var payment = new Payment() { Amount = 1 };

            _writeRepository.Setup(service => service.Invoices.Add(invoice))
                .Returns(invoice);
            _writeRepository.Setup(service => service.SaveChanges())
                .Verifiable();
            _invoiceService.Setup(service => service.ProcessPayment(payment))
                .Returns(Messages.Invoices.PARTIAL_PAYMENT_RECEIVED);

            var writeRepository = _writeRepository.Object;
            var invoiceService = _invoiceService.Object;

            // Act
            writeRepository.Invoices.Add(invoice);
            writeRepository.SaveChanges();

            var result = invoiceService.ProcessPayment(payment);

            // Assert
            Assert.AreEqual(Messages.Invoices.PARTIAL_PAYMENT_RECEIVED, result);
        }
    }
}