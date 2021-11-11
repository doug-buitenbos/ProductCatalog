using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductCatalogApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogApi.Controllers
{
    [ApiController]
    [Route("productcatalog")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly CatalogContext _context;

        public ProductCatalogController(CatalogContext context)
        {
            _context = context;
        }

        #region Get
        [HttpGet]
        public IEnumerable<Product> Get() => _context.Set<Product>().OrderBy(p => p.Name);

        [HttpGet("{id}")]
        public Product Get(int id) => _context.Set<Product>().FirstOrDefault(p => p.Id == id);

        [HttpGet("{id}/pricehistory")]
        public IEnumerable<PriceUpdate> GetPriceHistory(int id) => _context.Set<Product>().FirstOrDefault(p => p.Id == id).PriceHistory;
        #endregion Get

        #region Post
        [HttpPost]
        public ActionResult<Product> Post(Product newProduct)
        {
            var product = _context.Add(newProduct).Entity;
            _context.SaveChanges();
            return product;
        }
        #endregion Post

        #region Patch
        [HttpPatch("{id}")]
        public ActionResult<Product> Patch(int id, [FromBody] JsonPatchDocument<Product> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest(ModelState);

            var product = _context.Set<Product>().FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            patchDocument.ApplyTo(product);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            _context.SaveChanges();
            return product;
        }
        #endregion Patch

        #region Delete
        [HttpDelete("{id}")]
        public ActionResult<Product> Delete(int id)
        {
            var product = _context.Set<Product>().FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            _context.Remove(product);
            _context.SaveChanges();

            return product;
        }
        #endregion Delete
    }
}
