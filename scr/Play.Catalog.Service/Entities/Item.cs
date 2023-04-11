using System;

namespace Play.Catalog.Entities
{

    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }


    }
}