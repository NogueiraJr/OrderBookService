using System.Text;
using System.Text.Json;
using System.Net.WebSockets;

namespace OrderBookApp
{
    public class BitstampWebSocketClient
    {
        private readonly ClientWebSocket _webSocket = new();
        private readonly OrderBookServiceClass _service;

        public BitstampWebSocketClient(OrderBookServiceClass service)
        {
            _service = service;
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _webSocket.ConnectAsync(new Uri("wss://ws.bitstamp.net"), CancellationToken.None);
                Console.WriteLine("Conectado ao WebSocket");

                await SubscribeToChannelsAsync(new[] { "btcusd", "ethusd" });
                await StartProcessingMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na conexão WebSocket: {ex.Message}");
            }
        }

        private async Task StartProcessingMessagesAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Console.WriteLine($"Mensagem recebida: {message}");

                try
                {
                    var json = JsonSerializer.Deserialize<JsonElement>(message);
                    if (json.GetProperty("event").GetString() == "data")
                    {
                        var data = json.GetProperty("data");

                        var channelString = json.GetProperty("channel").GetString();
                        if (channelString != null)
                        {
                            var splitChannel = channelString.Split('_');
                            if (splitChannel.Length > 2)
                            {
                                var instrument = splitChannel[2];
                                var entry = new OrderBookEntry
                                {
                                    Instrument = instrument,
                                    Price = data.GetProperty("price").GetDecimal(),
                                    Quantity = data.GetProperty("amount").GetDecimal(),
                                    Timestamp = DateTime.UtcNow
                                };

                                await _service.SaveOrderAsync(entry);
                            }
                            else
                            {
                                Console.WriteLine("Formato do canal inválido: " + channelString);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Canal não encontrado ou é nulo.");
                        }
                    }
                }
                catch (JsonException e)
                {
                    Console.WriteLine($"Erro ao processar a mensagem: {e.Message}");
                }
            }
        }

        private async Task SubscribeToChannelsAsync(string[] channels)
        {
            foreach (var channel in channels)
            {
                var subscriptionMessage = new
                {
                    @event = "bts:subscribe",
                    data = new { channel = channel }
                };

                var message = JsonSerializer.Serialize(subscriptionMessage);
                Console.WriteLine($"Enviando mensagem de assinatura: {message}");

                await _webSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
