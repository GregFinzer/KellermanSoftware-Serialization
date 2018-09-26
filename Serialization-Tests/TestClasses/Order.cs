using System.Collections.Generic;
using System.Linq;

namespace SerializationTests.TestClasses
{
    public class Order
    {
        //Primary Key
        public long OrderId { get; set; }

        //Many To One Relationship
        public Person Buyer { get; set; }

        //One to Many Relationship
        public List<OrderDetail> OrderDetails { get; set; }

        public decimal TotalAmount
        {
            get
            {
                if (OrderDetails == null)
                    return 0;
                else
                    return OrderDetails.Sum(o => o.LineItemCost);
            }
        }

        public string Notes { get; set; }
    }
}
