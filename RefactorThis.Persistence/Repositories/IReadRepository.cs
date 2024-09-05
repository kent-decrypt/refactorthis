using RefactorThis.Persistence.Entities;
using System.Linq;

namespace RefactorThis.Persistence.Repositories
{
    public interface IReadRepository
    {
        IQueryable<Invoice> InvoicesView { get; }
        IQueryable<Payment> PaymentsView { get; }
    }
}
