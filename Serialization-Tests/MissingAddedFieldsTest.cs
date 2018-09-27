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
    public class MissingAddedFieldsTest
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

        #region Primitive Types

        [Test]
        public void MissingMovieTest()
        {
            Movie obj1 = new Movie();
            obj1.Name = "Star Wars";
            obj1.Id = 54;
            obj1.Length = 121;
            obj1.ReleaseDate = new DateTime(1977,5,25);
            obj1.MoneyMade = 775398007M;

            byte[] bytes = _serializer.Serialize(obj1);

            TestClasses2.Movie obj2 =_serializer.Deserialize<TestClasses2.Movie>(bytes);
            Assert.AreEqual(obj1.Name,obj2.Name);
            Assert.AreEqual(obj1.Id,obj2.Id);
            Assert.AreEqual(obj1.Length,(int) obj2.Length);
            Assert.AreEqual(DateTime.MinValue,obj2.DvdDate);
            Assert.AreEqual(null,obj2.SeenIt);

        }



        #endregion
    }
}
