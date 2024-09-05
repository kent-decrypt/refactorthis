namespace RefactorThis.Domain.Common
{
    public static class Messages
    {
        public static class Invoices
        {
            public const string NO_PAYMENT_NEEDED = "No payment needed";
            public const string FULLY_PAID = "Invoice was already fully paid";
            public const string FINAL_PARTIAL_PAYMENT_RECEIVED = "Final partial payment received, invoice is now fully paid.";
            public const string ANOTHER_PARTIAL_PAYMENT_RECEIVED = "Another partial payment received, still not fully paid";
            public const string PARTIAL_PAYMENT_RECEIVED = "Invoice is now partially paid";
            public const string FULL_PAYMENT_RECEIVED = "Invoice is now fully paid";

            public static class Exceptions 
            {
                public const string INVOICE_NOT_FOUND = "There is no invoice matching this payment";
                public const string INVALID_INVOICE = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
            }

            public static class Errors
            {
                public const string EXCESS_PARTIAL_PAYMENT = "The payment is greater than the partial amount remaining";
                public const string EXCESS_PAYMENT = "The payment is greater than the invoice amount";
            }
        }
    }
}
