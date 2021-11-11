using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;
using ProductCatalogApi.Controllers;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;

namespace ProductCatalogTests
{
    public class CatalogControllerTests : CatalogControllerTestBase, IDisposable
    {
        private readonly DbConnection _connection;
        private Random rng = new Random();

        public CatalogControllerTests()
            : base(new DbContextOptionsBuilder<CatalogContext>()
                  .UseSqlite(CreateInMemoryDatabase())
                  .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
        public void Dispose() => _connection.Dispose();

        [Fact]
        public void Can_get_products()
        {
            using (var context = new CatalogContext(ContextOptions))
            {
                var controller = new ProductCatalogController(context);
                var controllerProducts = controller.Get().ToList();
                Assert.Equal(context.Products.Count(), controllerProducts.Count);
                var dbProducts = context.Products.OrderBy(p => p.Name).ToList();
                for(int x = 0; x < controllerProducts.Count; x++)
                {
                    Assert.Equal(dbProducts[x].Name, controllerProducts[x].Name);
                }
            }
        }

        [Fact]
        public void Can_get_product()
        {
            using (var context = new CatalogContext(ContextOptions))
            {
                var controller = new ProductCatalogController(context);
                int toSkip = rng.Next(0, context.Products.Count());
                var dbProduct = context.Products.Skip(toSkip).Take(1).FirstOrDefault();
                var cProduct = controller.Get(dbProduct.Id);
                Assert.Equal(dbProduct.Name, cProduct.Name);
            }
        }

        [Fact]
        public void Can_get_product_history()
        {
            using (var context = new CatalogContext(ContextOptions))
            {
                var controller = new ProductCatalogController(context);
                int toSkip = rng.Next(0, context.Products.Count());
                var dbProduct = context.Products.Skip(toSkip).Take(1).FirstOrDefault();
                var cProductHistory = controller.GetPriceHistory(dbProduct.Id);
                Assert.Equal(dbProduct.PriceHistory.Count, cProductHistory.Count());
                for(int x = 0; x < cProductHistory.Count(); x++)
                {
                    Assert.Equal(dbProduct.PriceHistory.ElementAt(x).UpdatedDateTime, cProductHistory.ElementAt(x).UpdatedDateTime);
                    Assert.Equal(dbProduct.PriceHistory.ElementAt(x).Amount, cProductHistory.ElementAt(x).Amount);
                }
            }
        }

        [Fact]
        public void Can_add_product()
        {
            using (var context = new CatalogContext(ContextOptions))
            {
                var controller = new ProductCatalogController(context);
                Product newProduct = new Product("Fuzzy Dice", "112233456", 2.99m);
                int oldProductCount = context.Products.Count();
                newProduct = controller.Post(newProduct).Value;
                Assert.Equal(oldProductCount + 1, context.Products.Count());
                Assert.True(context.Products.Contains(newProduct));
            }
        }

        [Fact]
        public void Can_update_product()
        {
            using (var context = new CatalogContext(ContextOptions))
            {
                var controller = new ProductCatalogController(context);
                int toSkip = rng.Next(0, context.Products.Count());
                Product oldDbProduct = context.Products.Skip(toSkip).Take(1).FirstOrDefault();
                
                //Check name update
                string newName = oldDbProduct.Name + " XL";
                JsonPatchDocument<Product> namePatchDocument = new JsonPatchDocument<Product>().Replace(e => e.Name, newName);
                Product updatedProduct = controller.Patch(oldDbProduct.Id, namePatchDocument).Value;
                Product updatedDbProduct = context.Products.SingleOrDefault(p => p.Id == oldDbProduct.Id);
                Assert.Equal(newName, updatedProduct.Name);
                Assert.Equal(newName, updatedDbProduct.Name);

                //Check Sku update
                string newSku = oldDbProduct.Sku + "123";
                JsonPatchDocument<Product> skuPatchDocument = new JsonPatchDocument<Product>().Replace(e => e.Sku, newSku);
                updatedProduct = controller.Patch(oldDbProduct.Id, skuPatchDocument).Value;
                updatedDbProduct = context.Products.SingleOrDefault(p => p.Id == oldDbProduct.Id);
                Assert.Equal(newSku, updatedProduct.Sku);
                Assert.Equal(newSku, updatedDbProduct.Sku);

                //Check Price update
                decimal newPrice = oldDbProduct.Price + 1m;
                JsonPatchDocument<Product> pricePatchDocument = new JsonPatchDocument<Product>().Replace(e => e.Price, newPrice);
                updatedProduct = controller.Patch(oldDbProduct.Id, pricePatchDocument).Value;
                updatedDbProduct = context.Products.SingleOrDefault(p => p.Id == oldDbProduct.Id);
                Assert.Equal(newPrice, updatedProduct.Price);
                Assert.Equal(newPrice, updatedDbProduct.Price);
                var cProductHistory = controller.GetPriceHistory(oldDbProduct.Id);
                Assert.Equal(updatedDbProduct.PriceHistory.Count, cProductHistory.Count());
                for (int x = 0; x < cProductHistory.Count(); x++)
                {
                    Assert.Equal(updatedDbProduct.PriceHistory.ElementAt(x).UpdatedDateTime, cProductHistory.ElementAt(x).UpdatedDateTime);
                    Assert.Equal(updatedDbProduct.PriceHistory.ElementAt(x).Amount, cProductHistory.ElementAt(x).Amount);
                }
            }
        }

        [Fact]
        public void Can_delete_product()
        {
            using (var context = new CatalogContext(ContextOptions))
            {
                var controller = new ProductCatalogController(context);
                int toSkip = rng.Next(0, context.Products.Count());
                Product dbProduct = context.Products.Skip(toSkip).Take(1).FirstOrDefault();
                Product deletedProduct = controller.Delete(dbProduct.Id).Value;

                Assert.Equal(dbProduct.Id, deletedProduct.Id);
                Assert.False(context.Products.Any(p => p.Id == deletedProduct.Id));
            }
        }
    }
}
