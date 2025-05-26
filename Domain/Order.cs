namespace TeamLab.Domain
{
    public class Order
    {
        public string OrderItem {  get; set; }
        public string DeliveryInfo { get; set; }
        public DateTime DeliveryTime { get; set; }
        public double TotalPrice { get; set; }
    }
}
