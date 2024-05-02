using CRUP_App.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUP_App.Controllers
{
   // [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DotnetdbContext _dotnetdbContext;
        public ProductController(DotnetdbContext dotnetdbContext)
        {
            _dotnetdbContext = dotnetdbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var productList = await _dotnetdbContext.Product.ToListAsync();

            if (productList == null || productList.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return productList;
            }
        }

        [HttpPost]
        public async Task<ActionResult> InsertProduct(Product product)
        {
            _dotnetdbContext.Product.Add(product);
            await _dotnetdbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var Product = await _dotnetdbContext.Product.FirstOrDefaultAsync(s => s.Id == id);

            if (Product == null)
            {
                return NotFound();
            }
            else
            {
                return Product;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Product Product)
        {
            if (id != Product.Id)
            {
                return BadRequest();
            }

            _dotnetdbContext.Entry(Product).State = EntityState.Modified;

            try
            {
                await _dotnetdbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var Product = await _dotnetdbContext.Product.FindAsync(id);
            if (Product == null)
            {
                return NotFound();
            }

            _dotnetdbContext.Product.Remove(Product);
            await _dotnetdbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _dotnetdbContext.Product.Any(e => e.Id == id);
        }
    }
}
