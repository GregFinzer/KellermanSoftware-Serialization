﻿using System;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using SerializationTests.TestClasses;

namespace SerializationTests
{
    [TestFixture]
    public class PrimitivePropertiesClassTests
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
        /// Test serialization/deserialization of primitive types
        /// </summary>
        [Test]
        public void PrimitivePropertiesClassTest()
        {
            PrimitiveProperties obj1 = new PrimitiveProperties();
            obj1.Setup();

            byte[] bytes = _serializer.Serialize(obj1);
            obj1.ClearStatic();
            PrimitiveProperties obj2 = _serializer.Deserialize<PrimitiveProperties>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }


        /// <summary>
        /// Test serialization/deserialization of primitive types in a list
        /// </summary>
        [Test]
        public void PrimitivePropertiesListTest()
        {
            List<PrimitiveProperties> list1 = new List<PrimitiveProperties>();

            PrimitiveProperties obj1 = new PrimitiveProperties();
            obj1.Setup();
            list1.Add(obj1);

            byte[] bytes = _serializer.Serialize(list1);
            obj1.ClearStatic();
            List<PrimitiveProperties> list2 = _serializer.Deserialize<List<PrimitiveProperties>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion
    }
}
