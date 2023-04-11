using Play.Catalog.Service.DTOS;
using Play.Catalog.Service.Entitties;
using Play.Common;

namespace Play.Catalog.Service.Extensions
{
    public static class Extension
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreateDate);
        }
    }
}