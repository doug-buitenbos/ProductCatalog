using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace ProductCatalogTests
{
    public class CatalogControllerTestBase
    {
        protected DbContextOptions<CatalogContext> ContextOptions { get; }
        protected CatalogControllerTestBase(DbContextOptions<CatalogContext> options)
        {
            ContextOptions = options;
            SeedDb();
        }
        private void SeedDb()
        {
            using (var context = new CatalogContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var products = new List<Product>
                {
                    new Product("Headphones", "111222333", 
                        new List<PriceUpdate>
                        {
                            new PriceUpdate(new DateTime(2017, 10, 17, 8, 22, 37), 7.99m)
                        }),
                    new Product("Microphone", "444555666",
                        new List<PriceUpdate>
                        {
                            new PriceUpdate(new DateTime(2017, 10, 17, 8, 22, 53), 12.99m),
                            new PriceUpdate(new DateTime(2018, 2, 13, 14, 33, 17), 14.99m),
                            new PriceUpdate(new DateTime(2020, 12, 7, 17, 49, 2), 17.99m),
                            new PriceUpdate(new DateTime(2021, 3, 23, 7, 17, 16), 16.99m)
                        }),
                    new Product("Laser Pointer", "777888999",
                        new List<PriceUpdate>
                        {
                            new PriceUpdate(new DateTime(2017, 10, 17, 8, 24, 10), 25.99m),
                            new PriceUpdate(new DateTime(2019, 6, 6, 13, 22, 3), 27.99m),
                            new PriceUpdate(new DateTime(2019, 11, 29, 0, 0, 0), 25.99m),
                            new PriceUpdate(new DateTime(2020, 1, 2, 0, 0, 0), 14.99m),
                            new PriceUpdate(new DateTime(2020, 2, 1, 9, 30, 12), 10.99m)
                        })
                };

                context.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
