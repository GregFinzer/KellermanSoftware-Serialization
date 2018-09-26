namespace SerializationTests.TestClasses
{
    public class OrderDetail
    {
        //Primary Key
        public long OrderDetailId { get; set; }

        //Many to One
        public Order Order { get; set; }

        //Many to One
        public Product Item { get; set; }

        //Normal Property
        public int Quantity { get; set; }

        //Calculated property
        public decimal LineItemCost
        {
            get
            {
                return Item.Cost * Quantity;
            }
        }
    }
}
