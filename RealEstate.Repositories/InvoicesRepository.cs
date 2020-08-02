using RealEstate.Data;
using RealEstate.Model.Payment;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class InvoicesRepository : GenericRepository<RealEstateDbContext, Invoices>, IInvoiceRepository
    {
        public InvoicesRepository(RealEstateDbContext db) : base(db) { }
    }
}