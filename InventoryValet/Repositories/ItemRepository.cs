using System.Collections.Generic;
using System.Linq;
using InventoryValet.Data;
using InventoryValet.Models;

namespace InventoryValet.Repositories
{
    public class ItemRepository
    {
        private readonly InventoryVDbContext _context;

        public ItemRepository(InventoryVDbContext context)
        {
            _context = context;
        }

        public List<Item> GetItems(int pageNumber, int pageSize)
        {
            return _context.Items
                           .OrderBy(x => x.Id)
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();
        }
    }
}