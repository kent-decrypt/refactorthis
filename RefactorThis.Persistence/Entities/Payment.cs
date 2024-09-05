namespace RefactorThis.Persistence.Entities
{
    /// <summary>
    /// Entity class for Payments
    /// </summary>
    public class Payment
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}