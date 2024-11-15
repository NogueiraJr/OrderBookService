public class OrderBookEntry
{
    public int Id { get; set; }
    public string Instrument { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime Timestamp { get; set; }
}
