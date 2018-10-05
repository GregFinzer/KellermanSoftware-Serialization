namespace KellermanSoftware.Serialization
{
    public enum CompressionType
    {
        /// <summary>
        /// http://en.wikipedia.org/wiki/Gzip
        /// </summary>
        GZip,
        /// <summary>
        /// http://en.wikipedia.org/wiki/Deflate
        /// </summary>
        Deflate,
        /// <summary>
        /// http://en.wikipedia.org/wiki/Lzo
        /// </summary>
        MiniLZO
    }
}
