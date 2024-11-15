using Microsoft.EntityFrameworkCore;

namespace OrderBookApp
{
    public class OrderBookServiceClass
    {
        private readonly OrderBookContext _context;

        public OrderBookServiceClass(OrderBookContext context)
        {
            _context = context;
        }

        public async Task SaveOrderAsync(OrderBookEntry entry)
        {
            _context.OrderBookEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderBookEntry>> GetOrdersAsync(string instrument, DateTime fromTime)
        {
            return await _context.OrderBookEntries
                .Where(o => o.Instrument == instrument && o.Timestamp >= fromTime)
                .ToListAsync();
        }
    }
}
