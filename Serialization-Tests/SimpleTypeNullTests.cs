using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;

namespace SerializationTests
{
    [TestFixture]
    public class SimpleTypeNullTests
    {
        #region Class Variables
        private CompareObjects _compare = null;
        private Serializer _serializer = null;
        #endregion

        #region Setup/Teardown

        /// <summary>
        /// Code that is run once for a suite of tests
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

        }

        /// <summary>
        /// Code that is run once after a suite of tests has finished executing
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {

        }

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

        #region Tests

        [Test]
        public void BoolTest()
        {
            bool? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            bool? obj2 = _serializer.Deserialize<bool?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void ByteTest()
        {
            byte? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            byte? obj2 = _serializer.Deserialize<byte?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void SByteTest()
        {
            sbyte? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            sbyte? obj2 = _serializer.Deserialize<sbyte?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void IntNullTest()
        {
            int? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            int? obj2 = _serializer.Deserialize<int?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }



        [Test]
        public void LongNullTest()
        {
            long? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            long? obj2 = _serializer.Deserialize<long?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void Int16NullTest()
        {
            Int16? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            Int16? obj2 = _serializer.Deserialize<Int16?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }


        [Test]
        public void Int32NullTest()
        {
            Int32? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            Int32? obj2 = _serializer.Deserialize<Int32?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }


        [Test]
        public void UInt32NullTest()
        {
            UInt32? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            UInt32? obj2 = _serializer.Deserialize<UInt32?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }


        [Test]
        public void Int64NullTest()
        {
            Int64? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            Int64? obj2 = _serializer.Deserialize<Int64?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }


        [Test]
        public void UInt64NullTest()
        {
            UInt64? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            UInt64? obj2 = _serializer.Deserialize<UInt64?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }



        [Test]
        public void CharTest()
        {
            char? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            char? obj2 = _serializer.Deserialize<char?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DoubleNullTest()
        {
            double? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            double? obj2 = _serializer.Deserialize<double?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void FloatNullTest()
        {
            float? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            float? obj2 = _serializer.Deserialize<float?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }


        [Test]
        public void StringTest()
        {
            string obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DateTimeTest()
        {
            DateTime? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            DateTime? obj2 = _serializer.Deserialize<DateTime?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void GuidTest()
        {
            Guid? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            Guid? obj2 = _serializer.Deserialize<Guid?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TimeSpanTest()
        {
            TimeSpan? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            TimeSpan? obj2 = _serializer.Deserialize<TimeSpan?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DecimalNullTest()
        {
            decimal? obj1 = null;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal? obj2 = _serializer.Deserialize<decimal?>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }


        #endregion
    }
}
