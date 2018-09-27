using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using SerializationTests.TestClasses;

namespace SerializationTests
{
    [TestFixture]
    public class PrimitiveFieldsClassTests
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

        #region Tests

        /// <summary>
        /// Test serialization/deserialization of primitive fields in a class
        /// </summary>
        [Test]
        public void PrimitiveFieldsClassTest()
        {
            PrimitiveFields obj1 = new PrimitiveFields();
            obj1.Setup();

            byte[] bytes = _serializer.Serialize(obj1);
            obj1.ClearStatic();
            PrimitiveFields obj2 = _serializer.Deserialize<PrimitiveFields>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        /// <summary>
        /// Test serialization/deserialization of primitive fields in a class in a list
        /// </summary>
        [Test]
        public void PrimitiveFieldsListTest()
        {
            List<PrimitiveFields> list1 = new List<PrimitiveFields>();

            PrimitiveFields obj1 = new PrimitiveFields();
            obj1.Setup();
            list1.Add(obj1);

            byte[] bytes = _serializer.Serialize(list1);
            List<PrimitiveFields> list2 = _serializer.Deserialize<List<PrimitiveFields>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion
    }
}
