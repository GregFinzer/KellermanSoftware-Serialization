using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;

namespace SerializationConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SerializationDeserializationTest();
        }

        public static void SerializationDeserializationTest()
        {
            Console.WriteLine("Start");
            Serializer serializer = new Serializer();

            //Serialize and Deserialize a simple type
            DateTime date = new DateTime(2010, 8, 21, 10, 53, 31, 555);
            Console.WriteLine("Serialize Simple");
            byte[] serializedDate = serializer.Serialize(date);

            Console.WriteLine("Deserialize Simple");
            DateTime dateCopy = serializer.Deserialize<DateTime>(serializedDate);

            //Serialize and Deserialize a single object
            Person person = new Person();
            person.Name = "John";
            Console.WriteLine("Serialize Single");
            byte[] serialized = serializer.Serialize(person);

            Console.WriteLine("Deserialize Single");
            Person personCopy = serializer.Deserialize<Person>(serialized);

            //Serialize and Deserialize a List
            List<Person> personList = new List<Person>();

            Person person1 = new Person();
            person1.Name = "Sally";
            personList.Add(person1);

            Person person2 = new Person();
            person2.Name = "Susan";
            personList.Add(person2);

            Console.WriteLine("Serialize List");
            byte[] serializedList = serializer.Serialize(personList);

            Console.WriteLine("Deserialize List");
            List<Person> personListCopy = serializer.Deserialize<List<Person>>(serializedList);

            CompareObjects compareObjects = new CompareObjects();


            if (!compareObjects.Compare(person, personCopy))
                Console.WriteLine(compareObjects.DifferencesString);

            if (!compareObjects.Compare(personList, personListCopy))
                Console.WriteLine(compareObjects.DifferencesString);

            if (date != dateCopy)
                Console.WriteLine("Dates Different");

            Console.WriteLine("Press enter");
            Console.ReadLine();
        }
    }
}
