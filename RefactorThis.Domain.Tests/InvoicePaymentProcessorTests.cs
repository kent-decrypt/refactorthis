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
		private IInvoiceService _invoiceService;
		private Mock<IWriteRepository> _writeRepository;
		private Mock<IReadRepository> _readRepository;

        [SetUp]
        public void SetUp()
        {
            _writeRepository = new Mock<IWriteRepository>();
            _readRepository = new Mock<IReadRepository>();
            _invoiceService = new Mock<IInvoiceService>().Object;
        }

        [Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
			var payment = new Payment();
			var failureMessage = "";

			try
			{
				var result = _invoiceService.ProcessPayment(payment);
			}
			catch (InvalidOperationException e)
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual(Messages.Invoices.Exceptions.INVOICE_NOT_FOUND, failureMessage);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded( )
		{
			var invoice = Invoice.Save(0, 0, InvoiceType.Standard, 0, null);

            _writeRepository
				.Setup(repo => repo.Invoices)
				.Returns((DbSet<Invoice>)Enumerable.Empty<Invoice>().AsQueryable());

            var payment = new Payment();

			var result = _invoiceService.ProcessPayment(payment);

			Assert.AreEqual(Messages.Invoices.NO_PAYMENT_NEEDED, result);
		}

		//[Test]
		//public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
		//{
		//	var repo = new InvoiceRepository();

		//	var invoice = new Invoice(repo)
		//	{
		//		Amount = 10,
		//		AmountPaid = 10,
		//		Payments = new List<Payment>
		//		{
		//			new Payment
		//			{
		//				Amount = 10
		//			}
		//		}
		//	};
		//	repo.Add(invoice);

		//	var payment = new Payment();

		//	var result = _invoiceService.ProcessPayment(payment);

		//	Assert.AreEqual("invoice was already fully paid", result);
		//}

		//[Test]
		//public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
		//{
		//	var repo = new InvoiceRepository();
		//	var invoice = new Invoice(repo)
		//	{
		//		Amount = 10,
		//		AmountPaid = 5,
		//		Payments = new List<Payment>
		//		{
		//			new Payment
		//			{
		//				Amount = 5
		//			}
		//		}
		//	};
		//	repo.Add(invoice);

		//	var paymentProcessor = new InvoiceService(repo);

		//	var payment = new Payment()
		//	{
		//		Amount = 6
		//	};

		//	var result = paymentProcessor.ProcessPayment(payment);

		//	Assert.AreEqual("the payment is greater than the partial amount remaining", result);
		//}

		//[Test]
		//public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
		//{
		//	var repo = new InvoiceRepository();
		//	var invoice = new Invoice(repo)
		//	{
		//		Amount = 5,
		//		AmountPaid = 0,
		//		Payments = new List<Payment>()
		//	};
		//	repo.Add(invoice);

		//	var paymentProcessor = new InvoiceService(repo);

		//	var payment = new Payment()
		//	{
		//		Amount = 6
		//	};

		//	var result = paymentProcessor.ProcessPayment(payment);

		//	Assert.AreEqual("the payment is greater than the invoice amount", result);
		//}

		//[Test]
		//public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
		//{
		//	var repo = new InvoiceRepository();
		//	var invoice = new Invoice(repo)
		//	{
		//		Amount = 10,
		//		AmountPaid = 5,
		//		Payments = new List<Payment>
		//		{
		//			new Payment
		//			{
		//				Amount = 5
		//			}
		//		}
		//	};

		//	repo.Add(invoice);

		//	var paymentProcessor = new InvoiceService(repo);

		//	var payment = new Payment()
		//	{
		//		Amount = 5
		//	};

		//	var result = paymentProcessor.ProcessPayment(payment);

		//	Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
		//}

		//[Test]
		//public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
		//{
		//	var repo = new InvoiceRepository();
		//	var invoice = new Invoice(repo)
		//	{
		//		Amount = 10,
		//		AmountPaid = 0,
		//		Payments = new List<Payment>() { new Payment() { Amount = 10 } }
		//	};
		//	repo.Add(invoice);

		//	var paymentProcessor = new InvoiceService(repo);

		//	var payment = new Payment()
		//	{
		//		Amount = 10
		//	};

		//	var result = paymentProcessor.ProcessPayment(payment);

		//	Assert.AreEqual("invoice was already fully paid", result);
		//}

		//[Test]
		//public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
		//{
		//	var repo = new InvoiceRepository();
		//	var invoice = new Invoice(repo)
		//	{
		//		Amount = 10,
		//		AmountPaid = 5,
		//		Payments = new List<Payment>
		//		{
		//			new Payment
		//			{
		//				Amount = 5
		//			}
		//		}
		//	};
		//	repo.Add(invoice);

		//	var paymentProcessor = new InvoiceService(repo);

		//	var payment = new Payment()
		//	{
		//		Amount = 1
		//	};

		//	var result = paymentProcessor.ProcessPayment(payment);

		//	Assert.AreEqual("another partial payment received, still not fully paid", result);
		//}

		//[Test]
		//public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
		//{
		//	var repo = new InvoiceRepository();
		//	var invoice = new Invoice(repo)
		//	{
		//		Amount = 10,
		//		AmountPaid = 0,
		//		Payments = new List<Payment>()
		//	};
		//	repo.Add(invoice);

		//	var paymentProcessor = new InvoiceService(repo);

		//	var payment = new Payment()
		//	{
		//		Amount = 1
		//	};

		//	var result = paymentProcessor.ProcessPayment(payment);

		//	Assert.AreEqual("invoice is now partially paid", result);
		//}
	}
}