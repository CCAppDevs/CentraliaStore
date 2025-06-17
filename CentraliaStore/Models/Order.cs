namespace CentraliaStore.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
        public DateTime OrderedOn { get; set; }
    }
}
