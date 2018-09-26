using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;

namespace SerializationTests
{
    [TestFixture]
    public class SimpleTypeTests
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
            bool obj1 = true;            
            byte[] bytes = _serializer.Serialize(obj1);            
            bool obj2 = _serializer.Deserialize<bool>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void ByteTest()
        {
            byte obj1 = 1;
            byte[] bytes = _serializer.Serialize(obj1);
            byte obj2 = _serializer.Deserialize<byte>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void SByteTest()
        {
            sbyte obj1 = 1;
            byte[] bytes = _serializer.Serialize(obj1);
            sbyte obj2 = _serializer.Deserialize<sbyte>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void IntMaxTest()
        {
            int obj1= int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            int obj2 = _serializer.Deserialize<int>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void IntMinTest()
        {
            int obj1 = int.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            int obj2 = _serializer.Deserialize<int>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void LongMaxTest()
        {
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void LongMinTest()
        {
            long obj1 = long.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            long obj2 = _serializer.Deserialize<long>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void Int16MaxTest()
        {
            Int16 obj1 = Int16.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            Int16 obj2 = _serializer.Deserialize<Int16>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void Int16MinTest()
        {
            Int16 obj1 = Int16.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            Int16 obj2 = _serializer.Deserialize<Int16>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void Int32MaxTest()
        {
            Int32 obj1 = Int32.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            Int32 obj2 = _serializer.Deserialize<Int32>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void Int32MinTest()
        {
            Int32 obj1 = Int32.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            Int32 obj2 = _serializer.Deserialize<Int32>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void UInt32MaxTest()
        {
            UInt32 obj1 = UInt32.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            UInt32 obj2 = _serializer.Deserialize<UInt32>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void UInt32MinTest()
        {
            UInt32 obj1 = UInt32.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            UInt32 obj2 = _serializer.Deserialize<UInt32>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void Int64MaxTest()
        {
            Int64 obj1 = Int64.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            Int64 obj2 = _serializer.Deserialize<Int64>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void Int64MinTest()
        {
            Int64 obj1 = Int64.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            Int64 obj2 = _serializer.Deserialize<Int64>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void UInt64MaxTest()
        {
            UInt64 obj1 = UInt64.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            UInt64 obj2 = _serializer.Deserialize<UInt64>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void UInt64MinTest()
        {
            UInt64 obj1 = UInt64.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            UInt64 obj2 = _serializer.Deserialize<UInt64>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void CharTest()
        {
            char obj1 = 'G';
            byte[] bytes = _serializer.Serialize(obj1);
            char obj2 = (char)_serializer.Deserialize<char>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DoubleMaxTest()
        {
            double obj1 = double.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DoubleMinTest()
        {
            double obj1 = double.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            double obj2 = _serializer.Deserialize<double>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void FloatMaxTest()
        {
            float obj1 = float.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void FloatMinTest()
        {
            float obj1 = float.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            float obj2 = _serializer.Deserialize<float>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void StringTest()
        {
            string obj1 = "Greg";
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DateTimeTest()
        {
            DateTime obj1 = DateTime.Now;
            byte[] bytes = _serializer.Serialize(obj1);
            DateTime obj2 = _serializer.Deserialize<DateTime>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DateTimeMaxTest()
        {
            DateTime obj1 = DateTime.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            DateTime obj2 = _serializer.Deserialize<DateTime>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DateTimeMinTest()
        {
            DateTime obj1 = DateTime.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            DateTime obj2 = _serializer.Deserialize<DateTime>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void GuidTest()
        {
            Guid obj1 = Guid.NewGuid();
            byte[] bytes = _serializer.Serialize(obj1);
            Guid obj2 = _serializer.Deserialize<Guid>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TimeSpanTest()
        {
            TimeSpan obj1 = DateTime.Now.AddMilliseconds(91) - DateTime.Now;
            byte[] bytes = _serializer.Serialize(obj1);
            TimeSpan obj2 = _serializer.Deserialize<TimeSpan>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DecimalMaxTest()
        {
            decimal obj1 = decimal.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void DecimalMinTest()
        {
            decimal obj1 = decimal.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            decimal obj2 = _serializer.Deserialize<decimal>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion
    }
}
