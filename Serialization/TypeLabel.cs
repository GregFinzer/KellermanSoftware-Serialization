namespace KellermanSoftware.Serialization
{
    internal enum TypeLabel : byte
    {
        Custom = 0,
        Bool,
        Byte,
        Char,
        Decimal,
        Double,
        Float,
        Int,
        Long,
        SByte,
        Short,
        String,
        UInt,
        ULong,
        UShort,
        DateTime,
        TimeSpan,
        Guid,
        Enum,
        Uri,
        BitArray,
        WriteableBitmap,
        ObjectArray,
        DateTimeOffset
    }
}