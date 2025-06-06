
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.Models;
using OrderAPI.Services;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IAzureServiceBusPublisher _bus;

        public OrdersController(OrderDbContext context, IAzureServiceBusPublisher bus)
        {
            _context = context;
            _bus = bus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _context.Orders.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            order.Id = Guid.NewGuid();
            order.Status = "Pendente";
            order.DataCriacao = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _bus.SendMessageAsync(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}
