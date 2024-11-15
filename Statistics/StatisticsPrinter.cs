namespace OrderBookApp
{
    public class StatisticsPrinter
    {
        private readonly OrderBookServiceClass _service;

        public StatisticsPrinter(OrderBookServiceClass service)
        {
            _service = service;
        }

        public async Task PrintStatisticsAsync()
        {
            while (true)
            {
                var now = DateTime.UtcNow;
                var fiveSecondsAgo = now.AddSeconds(-5);
                
                foreach (var instrument in new[] { "btcusd", "ethusd" })
                {
                    var orders = await _service.GetOrdersAsync(instrument, fiveSecondsAgo);

                    if (orders.Any())
                    {
                        var prices = orders.Select(o => o.Price).ToList();
                        var quantities = orders.Select(o => o.Quantity).ToList();

                        Console.WriteLine($"Instrument: {instrument.ToUpper()}");
                        Console.WriteLine($"  Max Price: {prices.Max()}");
                        Console.WriteLine($"  Min Price: {prices.Min()}");
                        Console.WriteLine($"  Avg Price: {prices.Average():F2}");
                        Console.WriteLine($"  Avg Qty (5s): {quantities.Average():F2}");
                    }
                }

                await Task.Delay(5000);
            }
        }
    }
}
