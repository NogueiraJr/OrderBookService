using OrderBookApp;
using Microsoft.EntityFrameworkCore;

namespace OrderBookService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var context = new OrderBookContext();
            await context.Database.MigrateAsync();

        var service = new OrderBookServiceClass(new OrderBookContext());
            var webSocketClient = new BitstampWebSocketClient(service);

            await webSocketClient.ConnectAsync();

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}