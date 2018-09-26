#region Includes

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;
using SerializationTests.TestClasses;
using System.Collections.Generic;
#endregion

namespace SerializationTests
{
    [TestFixture]
    public class SimpleSerializationTests
    {
        private CompareObjects _compareObjects = new CompareObjects();

        [Test] 
        public void SerializationDeserializationTest()
        {
            Serializer serializer = new Serializer();

            //Serialize and Deserialize a simple type
            DateTime date = new DateTime(2010,8,21,10,53,31,555);
            byte[] serializedDate = serializer.Serialize(date);
            DateTime dateCopy = serializer.Deserialize<DateTime>(serializedDate);

            //Serialize and Deserialize a single object
            Person person = new Person();
            person.Name = "John";
            byte[] serialized = serializer.Serialize(person);

            Person personCopy = serializer.Deserialize<Person>(serialized);

            //Serialize and Deserialize a List
            List<Person> personList = new List<Person>();

            Person person1 = new Person();
            person1.Name = "Sally";
            personList.Add(person1);

            Person person2 = new Person();
            person2.Name = "Susan";
            personList.Add(person2);

            byte[] serializedList = serializer.Serialize(personList);

            List<Person> personListCopy = serializer.Deserialize<List<Person>>(serializedList);

            Assert.IsTrue(_compareObjects.Compare(person,personCopy));
            Assert.IsTrue(_compareObjects.Compare(personList, personListCopy));
            Assert.AreEqual(date,dateCopy);
        }
    }
}
