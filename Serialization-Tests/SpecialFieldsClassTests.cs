using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using SerializationTests.TestClasses;

namespace SerializationTests
{
    [TestFixture]
    public class SpecialFieldsClassTests
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
        /// Test serialization/deserialization of special fields in a class
        /// </summary>
        [Test]
        public void SpecialFieldsClassTest()
        {
            SpecialFields obj1 = new SpecialFields();
            obj1.Setup();

            byte[] bytes = _serializer.Serialize(obj1);
            SpecialFields obj2 = _serializer.Deserialize<SpecialFields>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }



        /// <summary>
        /// Test serialization/deserialization of special fields
        /// </summary>
        [Test]
        public void SpecialFieldsListTest()
        {
            List<SpecialFields> list1 = new List<SpecialFields>();

            SpecialFields obj1 = new SpecialFields();
            obj1.Setup();
            list1.Add(obj1);

            byte[] bytes = _serializer.Serialize(list1);
            List<SpecialFields> list2 = _serializer.Deserialize <List<SpecialFields>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion
    }
}
