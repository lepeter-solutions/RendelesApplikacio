namespace OrderManagementApp
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid(); 

        public string CustomerName { get; set; }
        public string OrderAddress { get; set; }
        public string OrderedItem { get; set; }
        public int Amount { get; set; }
        public string OrderStatus { get; set; }
        public System.DateTime OrderDate { get; set; }

        public override string ToString()
        {
            return $"Customer: {CustomerName}, Item: {OrderedItem}, Amount: {Amount}, Status: {OrderStatus}, Date: {OrderDate.ToShortDateString()}, Address: {OrderAddress}";
        }
    }
}
