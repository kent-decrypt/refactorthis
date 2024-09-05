using RefactorThis.Persistence.Entities;
using RefactorThis.Persistence.Repositories;
using System.Data.Entity;
using System.Linq;

namespace RefactorThis.Persistence
{
    public class Context : DbContext, IWriteRepository, IReadRepository
    {
        public Context(string connectionString) : base(connectionString) { }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public IQueryable<Invoice> InvoicesView => Invoices.AsNoTracking();
        public IQueryable<Payment> PaymentsView => Payments.AsNoTracking();
    }
}
