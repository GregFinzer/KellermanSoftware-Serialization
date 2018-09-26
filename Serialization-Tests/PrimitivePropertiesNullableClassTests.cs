using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using SerializationTests.TestClasses;

namespace SilverlightDbTest
{
    [TestFixture]
    public class PrimitivePropertiesNullableClassTests
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

        /// <summary>
        /// Test serialization/deserialization of primitive types
        /// </summary>
        [Test]
        public void PrimitivePropertiesNullableClassTest()
        {
            PrimitivePropertiesNullable obj1 = new PrimitivePropertiesNullable();
            obj1.Setup();
            byte[] bytes = _serializer.Serialize(obj1);
            obj1.ClearStatic();
            PrimitivePropertiesNullable obj2 = _serializer.Deserialize<PrimitivePropertiesNullable>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }



        /// <summary>
        /// Test serialization/deserialization of primitive types in a list
        /// </summary>
        [Test]
        public void PrimitivePropertiesNullableListTest()
        {
            List<PrimitivePropertiesNullable> list1 = new List<PrimitivePropertiesNullable>();

            PrimitivePropertiesNullable obj1 = new PrimitivePropertiesNullable();
            obj1.Setup();
            list1.Add(obj1);

            byte[] bytes = _serializer.Serialize(list1);
            obj1.ClearStatic();
            List<PrimitivePropertiesNullable> list2 = _serializer.Deserialize<List<PrimitivePropertiesNullable>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion
    }
}
