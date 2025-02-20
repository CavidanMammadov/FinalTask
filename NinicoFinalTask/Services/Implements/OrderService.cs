using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;
using NinicoFinalTask.Services.Abstracts;

namespace NinicoFinalTask.Services.Implements
{
    public class OrderService(NinicoDbContext _context) : IOrderService
    {
        public async Task CreateOrderAsync(string userId, decimal amount)
        {
                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = amount,
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
        }

        public async Task ConfirmOrderAsync(string userId)
        {
            var order = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Pending")
                .OrderByDescending(o => o.CreatedTime)
                .FirstOrDefaultAsync();

            if (order != null)
            {
                order.Status = "Paid";
                await _context.SaveChangesAsync();
            }
        }

    }
}
