
using Play.Catalog.Service.Contracts;
using Play.Catalog.Service.Entitties;
using Play.Common;

namespace Play.Catalog.Service.Repositories
{
    public class ItemsRepository : GenericRepository<Item>, IItemsRepository
    {
        private readonly ApplicationContext _context;
        public ItemsRepository(ApplicationContext Context):base(Context)
        {
            _context = Context;
        }
    }
}