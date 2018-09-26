namespace SerializationTests.TestClasses
{
    public class Product
    {
        public long ProductId { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public byte[] ImageBytes { get; set; }
    }
}
