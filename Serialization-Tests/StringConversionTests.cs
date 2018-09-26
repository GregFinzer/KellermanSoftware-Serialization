#region Includes

using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;

#endregion

namespace SerializationTests
{
    [TestFixture]
    public class StringConversionTests
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
        public void BoolToString()
        {
            bool obj1 = true;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void ByteToString()
        {
            byte obj1 = byte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void SByteToString()
        {
            sbyte obj1 = sbyte.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void IntMinToString()
        {
            int obj1 = int.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void IntMaxToString()
        {
            int obj1 = int.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void LongMinToString()
        {
            long obj1 = long.MinValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void LongMaxToString()
        {
            long obj1 = long.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void CharToString()
        {
            char obj1 = 'G';
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void DoubleToString()
        {
            double obj1 = double.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void FloatToString()
        {
            float obj1 = float.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void DecimalToString()
        {
            decimal obj1 = decimal.MaxValue;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }

        [Test]
        public void DateToString()
        {
            DateTime obj1 = DateTime.Now;
            byte[] bytes = _serializer.Serialize(obj1);
            string obj2 = _serializer.Deserialize<string>(bytes);
            Assert.IsTrue(obj1.ToString() == obj2);
        }
        #endregion
    }
}
