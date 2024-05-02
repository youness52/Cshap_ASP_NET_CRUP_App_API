using CRUP_App.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUP_App.Controllers
{
    [Authorize] // Requires authentication
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DotnetdbContext _dotnetdbContext;
        public OrderController(DotnetdbContext dotnetdbContext)
        {
            _dotnetdbContext = dotnetdbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get()
        {
            var orderList = await _dotnetdbContext.Order
                .Include(o => o.User)    // Include the User navigation property
                .Include(o => o.Product) // Include the Product navigation property
                .ToListAsync();

            if (orderList == null || orderList.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return orderList;
            }
        }


        [HttpPost]
        public async Task<ActionResult> InsertOrder(Order Order)
        {
            _dotnetdbContext.Order.Add(Order);
            await _dotnetdbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = Order.OrderId }, Order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _dotnetdbContext.Order
                .Include(o => o.User)    // Include the User navigation property
                .Include(o => o.Product) // Include the Product navigation property
                .FirstOrDefaultAsync(s => s.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
            else
            {
                return order;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrder(int id, Order Order)
        {
            if (id != Order.OrderId)
            {
                return BadRequest();
            }

            _dotnetdbContext.Entry(Order).State = EntityState.Modified;

            try
            {
                await _dotnetdbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var Order = await _dotnetdbContext.Order.FindAsync(id);
            if (Order == null)
            {
                return NotFound();
            }

            _dotnetdbContext.Order.Remove(Order);
            await _dotnetdbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _dotnetdbContext.Order.Any(e => e.OrderId == id);
        }

    }
}
