namespace SerializationTests.TestClasses
{
    public struct Size
    {
        public int Width;
        public int Height;
    }

    public struct SizeWithProperties
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
