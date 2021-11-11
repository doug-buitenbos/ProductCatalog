using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ProductCatalogApi.Models
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        [JsonIgnore]
        public SortedSet<PriceUpdate> PriceHistory { get; private set; }
        public Decimal Price
        {
            get
            {
                if (PriceHistory == null)
                    return 0m;

                return PriceHistory.Last().Amount;
            }
            set
            {
                if (PriceHistory == null)
                    PriceHistory = new SortedSet<PriceUpdate>();

                PriceHistory.Add(new PriceUpdate(DateTime.UtcNow, value));
            }
        }

        public Product(string name, string sku, List<PriceUpdate> priceHistory)
        {
            this.Name = name;
            this.Sku = sku;
            this.PriceHistory = new SortedSet<PriceUpdate>(priceHistory);
        }

        public Product(string name, string sku, decimal price)
        {
            this.Name = name;
            this.Sku = sku;
            this.Price = price;
        }

        public Product() {}
    }
}
