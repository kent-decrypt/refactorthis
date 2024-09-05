using RefactorThis.Persistence.Entities;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;

namespace RefactorThis.Persistence.Repositories
{
    public interface IWriteRepository
    {
        DbSet<Invoice> Invoices { get; }
        DbSet<Payment> Payments { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken token = default);
    }
}
