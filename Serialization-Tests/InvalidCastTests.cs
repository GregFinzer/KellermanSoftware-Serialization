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
using SerializationTests.TestClasses;

#endregion

namespace SerializationTests
{
    [TestFixture]
    public class InvalidCastTests
    {
        #region Class Variables
        private const string CAST_MESSAGE="cast";

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

        #region Primitive Types

        [Test]
        public void ShortToSByte()
        {
            bool exceptionThrown = false;
            short obj1 = short.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                sbyte obj2 = _serializer.Deserialize<sbyte>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void ShortToByte()
        {
            bool exceptionThrown = false;
            short obj1 = short.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                byte obj2 = _serializer.Deserialize<byte>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void IntToShort()
        {
            bool exceptionThrown = false;
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                short obj2 = _serializer.Deserialize<short>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void IntToUShort()
        {
            bool exceptionThrown = false;
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                ushort obj2 = _serializer.Deserialize<ushort>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void LongToInt()
        {
            bool exceptionThrown = false;
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                int obj2 = _serializer.Deserialize<int>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void LongToUInt()
        {
            bool exceptionThrown = false;
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                uint obj2 = _serializer.Deserialize<uint>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void FloatToLong()
        {
            bool exceptionThrown = false;
            float obj1 = float.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                long obj2 = _serializer.Deserialize<long>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void FloatToULong()
        {
            bool exceptionThrown = false;
            float obj1 = float.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                ulong obj2 = _serializer.Deserialize<ulong>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void UShortToChar()
        {
            bool exceptionThrown = false;
            ushort obj1 = ushort.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                char obj2 = _serializer.Deserialize<char>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void UShortDoubleToFloatToChar()
        {
            bool exceptionThrown = false;
            double obj1 = double.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                float obj2 = _serializer.Deserialize<float>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        #endregion

        #region Nullable To Non-Nullable
        [Test]
        public void NullableDoubleToNonNullableDouble()
        {
            bool exceptionThrown = false;
            double? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                double obj2 = _serializer.Deserialize<double>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void NullableDoubleWithValueToNonNullableDouble()
        {
            bool exceptionThrown = false;
            double? obj1 = double.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                double obj2 = _serializer.Deserialize<double>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        #endregion

        #region Invalid Class Casts
        [Test]
        public void CastToAnObjectOfDifferentType()
        {
            bool exceptionThrown=false;
            Person obj1 = new Person();
            obj1.Name = "Greg";
            obj1.DateCreated = DateTime.Now;

            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                Entity obj2 = _serializer.Deserialize<Entity>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown= true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0,ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        [Test]
        public void CastLongToAnObjectOfDifferentType()
        {
            bool exceptionThrown = false;
            long obj1 = long.MaxValue;

            byte[] bytes = _serializer.Serialize(obj1);

            try
            {
                Person obj2 = _serializer.Deserialize<Person>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.IsTrue(ex.Message.IndexOf(CAST_MESSAGE) >= 0, ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        #endregion
    }
}
