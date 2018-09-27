//http://www.dotnetspider.com/resources/33345-Type-Conversion-C.aspx
//Convert from (type) 	To (type)
//sbyte 			    short, int, long, float, double, or decimal
//byte 	                short, ushort, int, uint, long, ulong, float, double, or decimal
//short 			    int, long, float, double, or decimal
//ushort			    int, uint, long, ulong, float, double, or decimal
//int 			        long, float, double, or decimal
//uint	               	long, ulong, float, double, or decimal
//long 			        float, double, or decimal
//char			        ushort, int, uint, long,ulong,float, double,or decimal
//float 			    double
//ulong 			    float, double, or decimal

#region Includes

using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;

#endregion

namespace SerializationTests
{
    [TestFixture]
    public class ImplicitConversionTests
    {
        #region Class Variables
        private CompareObjects _compare = null;
        private Serializer _serializer = null;
        #endregion

        #region Setup/Teardown


        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            _compare = new CompareObjects();
            _compare.MaxDifferences = 1000;
            _serializer = new Serializer();
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
        }
        #endregion

        #region Implicit sbyte


        [Test]
        public void SbyteToShort()
        {
            sbyte obj1 = sbyte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            short obj2 = _serializer.Deserialize<short>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void SbyteToInt()
        {
            sbyte obj1 = sbyte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            int obj2 = _serializer.Deserialize<int>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void SbyteToLong()
        {
            sbyte obj1 = sbyte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void SbyteToFloat()
        {
            sbyte obj1 = sbyte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void SbyteToDouble()
        {
            sbyte obj1 = sbyte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void SbyteToDecimal()
        {
            sbyte obj1 = sbyte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit byte


        [Test]
        public void ByteToShort()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            short obj2 = _serializer.Deserialize<short>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToUShort()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            ushort obj2 = _serializer.Deserialize<ushort>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToInt()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            int obj2 = _serializer.Deserialize<int>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToUInt()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            uint obj2 = _serializer.Deserialize<uint>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToLong()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToULong()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            ulong obj2 = _serializer.Deserialize<ulong>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToFloat()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToDouble()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ByteToDecimal()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit short

        [Test]
        public void ShortToInt()
        {
            short obj1 = short.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            int obj2 = _serializer.Deserialize<int>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ShortToLong()
        {
            short obj1 = short.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ShortToFloat()
        {
            short obj1 = short.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ShortToDouble()
        {
            short obj1 = short.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ShortToDecimal()
        {
            short obj1 = short.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit ushort

        [Test]
        public void UShortToInt()
        {
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            int obj2 = _serializer.Deserialize<int>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UShortToUInt()
        {
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            uint obj2 = _serializer.Deserialize<uint>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UShortToLong()
        {
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UShortToULong()
        {
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            ulong obj2 = _serializer.Deserialize<ulong>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UShortToFloat()
        {
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UShortToDouble()
        {
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UShortToDecimal()
        {
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit int

        [Test]
        public void IntToLong()
        {
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void IntToFloat()
        {
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void IntToDouble()
        {
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void IntToDecimal()
        {
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit uint

        [Test]
        public void UIntToLong()
        {
            uint obj1 = uint.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UIntToULong()
        {
            uint obj1 = uint.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            ulong obj2 = _serializer.Deserialize<ulong>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UIntToFloat()
        {
            uint obj1 = uint.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UIntToDouble()
        {
            uint obj1 = uint.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void UIntToDecimal()
        {
            uint obj1 = uint.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit long

        [Test]
        public void LongToFloat()
        {
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void LongToDouble()
        {
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void LongToDecimal()
        {
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit char

        [Test]
        public void CharToUShort()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            ushort obj2 = _serializer.Deserialize<ushort>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void CharToInt()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            int obj2 = _serializer.Deserialize<int>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void CharToUInt()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            uint obj2 = _serializer.Deserialize<uint>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void CharToLong()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void CharToULong()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            ulong obj2 = _serializer.Deserialize<ulong>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void CharToFloat()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void CharToDouble()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void CharToDecimal()
        {
            char obj1 = 'G';
            byte[] chars = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(chars);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit float

        [Test]
        public void FloatToDouble()
        {
            float obj1 = float.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }
        #endregion

        #region Implicit ulong

        [Test]
        public void ULongToFloat()
        {
            ulong obj1 = ulong.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ULongToDouble()
        {
            ulong obj1 = ulong.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        [Test]
        public void ULongToDecimal()
        {
            ulong obj1 = ulong.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);
            Assert.IsTrue(obj1 == obj2);
        }

        #endregion

        #region Implicit nullable
        [Test]
        public void BoolToNullableBool()
        {
            bool obj1 = true;
            byte[] bytes = _serializer.Serialize(obj1);
            bool? obj2 = _serializer.Deserialize<bool?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void ByteToNullableByte()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            byte? obj2 = _serializer.Deserialize<byte?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void IntToNullableInt()
        {
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            int? obj2 = _serializer.Deserialize<int?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void UIntToNullableUInt()
        {
            uint obj1 = uint.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            uint? obj2 = _serializer.Deserialize<uint?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void LongToNullableLong()
        {
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long? obj2 = _serializer.Deserialize<long?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void ULongToNullableULong()
        {
            ulong obj1 = ulong.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            ulong? obj2 = _serializer.Deserialize<ulong?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void CharToNullableChar()
        {
            char obj1 = char.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            char? obj2 = _serializer.Deserialize<char?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void DoubleToNullableDouble()
        {
            double obj1 = double.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double? obj2 = _serializer.Deserialize<double?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void FloatToNullableFloat()
        {
            float obj1 = float.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float? obj2 = _serializer.Deserialize<float?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void DateTimeToNullableDateTime()
        {
            DateTime obj1 = DateTime.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            DateTime? obj2 = _serializer.Deserialize<DateTime?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void DecimalToNullableDecimal()
        {
            Decimal obj1 = Decimal.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            Decimal? obj2 = _serializer.Deserialize<Decimal?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void GuidToNullableGuid()
        {
            Guid obj1 = Guid.NewGuid();
            byte[] bytes = _serializer.Serialize(obj1);
            Guid? obj2 = _serializer.Deserialize<Guid?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }

        [Test]
        public void TimeSpanToNullableTimeSpan()
        {
            TimeSpan obj1 = DateTime.Now.AddMinutes(91) - DateTime.Now;
            byte[] bytes = _serializer.Serialize(obj1);
            TimeSpan? obj2 = _serializer.Deserialize<TimeSpan?>(bytes);
            Assert.IsTrue(obj1 == obj2.Value);
        }
        #endregion
    }
}
