using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using SerializationTests.TestClasses;

namespace SerializationTests
{
    [TestFixture]
    public class SpecialCasesTests
    {
        #region Class Variables
        private CompareObjects _compare = null;
        private Serializer _serializer = null;
        #endregion

        #region One To Many and Many to One
        [Test]
        public void OneToManyManyToOneTest()
        {
            List<Order> orders = new List<Order>();
            Product product = new Product();
            product.Cost = 19.95M;
            product.Name = "Widget";
            product.ProductId = 43;

            for (int i = 1; i <= 1000;i++ )
            {
                Order order = new Order();
                order.OrderId = i * 1000;
                order.Notes = "Thank you for your order";
                order.Buyer = new Person();
                order.Buyer.Name = "Greg";
                order.Buyer.PersonId = i;

                order.OrderDetails = new List<OrderDetail>();
                OrderDetail detail = new OrderDetail();
                detail.Order = order;
                detail.Item = product;
                detail.Quantity = 2;
                order.OrderDetails.Add(detail);

                orders.Add(order);
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            byte[] bytes = _serializer.Serialize(orders);
            watch.Stop();

            Console.WriteLine("Serialize 1000 Objects: {0}", watch.ElapsedMilliseconds);

            watch.Reset();
            watch.Start();
            byte[] compressedBytes = new Compression().CompressBytes(bytes);
            watch.Stop();

            Console.WriteLine("Compress 1000 Objects: {0}", watch.ElapsedMilliseconds);

            System.IO.File.WriteAllBytes("Test.bin",bytes);
            System.IO.File.WriteAllBytes("Test.compressed", compressedBytes);


            IList<Order> listCopy = _serializer.Deserialize<IList<Order>>(bytes);

            if (!_compare.Compare(listCopy, orders))
                throw new Exception(_compare.DifferencesString);

        }
        #endregion

        #region Type.GetType Tests
        [Test]
        public void AssemblyQualifiedVsFullName()
        {
            Type type;
            List<Person> persons = new List<Person>();

            Console.WriteLine("FullName");
            Console.WriteLine(persons.GetType().FullName);
            type = Type.GetType(persons.GetType().FullName);

            Console.WriteLine("AssemblyQualifiedName");
            Console.WriteLine(persons.GetType().AssemblyQualifiedName);
            type = Type.GetType(persons.GetType().AssemblyQualifiedName);

            //Console.WriteLine("ReplaceVersion FullName");
            //Console.WriteLine(Serializer.ReplaceVersion(persons.GetType().FullName));
            //type = Type.GetType(Serializer.ReplaceVersion(persons.GetType().FullName));

            //Console.WriteLine("ReplaceVersion AssemblyQualifiedName");
            //Console.WriteLine(Serializer.ReplaceVersion(persons.GetType().AssemblyQualifiedName));
            //type = Type.GetType(Serializer.ReplaceVersion(persons.GetType().AssemblyQualifiedName));
        }
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

        #region Observable Collection Tests
        [Test]
        public void NestedObservableCollectionTest()
        {
            ObservableCollection<Goal> goals = new ObservableCollection<Goal>();

            Goal goal1 = new Goal();
            goal1.Name = "Lose Weight";
            goal1.Tasks = new ObservableCollection<Task>();
            goal1.Tasks.Add(new Task { Step = "Eat Vegetables 3 Times a Day" });
            goal1.Tasks.Add(new Task {Step = "Exercise 6 Days a Week"});
            goals.Add(goal1);

            Goal goal2 = new Goal();
            goal2.Name = "Dave Ramsey Baby Steps";
            goal2.Tasks = new ObservableCollection<Task>();
            goal2.Tasks.Add(new Task {Step = "$1000 Emergency Fund"});
            goal2.Tasks.Add(new Task { Step = "Get out of debt except for the morgage" });
            goal2.Tasks.Add(new Task { Step = "Emergency fund 3 to 6 months of expenses" });
            goal2.Tasks.Add(new Task { Step = "15% Into Retirement" });
            goal2.Tasks.Add(new Task { Step = "College education for kids" });
            goal2.Tasks.Add(new Task { Step = "Pay off house early" });
            goal2.Tasks.Add(new Task { Step = "Invest and Give" });
            goals.Add(goal2);

            byte[] bytes = _serializer.Serialize(goals);
            ObservableCollection<Goal> goalsCopy = _serializer.Deserialize<ObservableCollection<Goal>>(bytes);

            if (!_compare.Compare(goalsCopy, goals))
                throw new Exception(_compare.DifferencesString);

        }
        #endregion

        #region IList Tests

        [Test]
        public void DeSerializeAListWithAMillionEntries()
        {
            List<string> list = new List<string>();

            for (int i = 0; i < 1000000; i++)
                list.Add(i.ToString());

            byte[] bytes = _serializer.Serialize(list);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<string> list2 = _serializer.Deserialize<List<string>>(bytes);
            watch.Stop();

            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        [Test]
        public void SerializeIList()
        {
            IList<Person> list = new List<Person>();

            for (int i = 1; i <= 10;i++ )
            {
                Person person = new Person();
                person.Name = "Greg";
                list.Add(person);
            }

            byte[] bytes = _serializer.Serialize(list);
           
            IList<Person> listCopy = _serializer.Deserialize<IList<Person>>(bytes);

            if (!_compare.Compare(listCopy, list))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void SerializeIListWithNull()
        {
            IList<Person> list = new List<Person>();

            list.Add(null);

            for (int i = 1; i <= 10; i++)
            {
                Person person = new Person();
                person.Name = "Greg";
                list.Add(person);
            }

            list.Add(null);

            
            byte[] bytes = _serializer.Serialize(list);

            IList<Person> listCopy = _serializer.Deserialize<IList<Person>>(bytes);

            if (!_compare.Compare(listCopy, list))
                throw new Exception(_compare.DifferencesString);
        }

        #endregion

        #region Missing Parameterless Constructor

        [Test]
        public void MissingParameterlessConstructorTest()
        {            
            Starship starship = new Starship(199999999.95M);
            starship.Name = "X-Wing";
            byte[] bytes = _serializer.Serialize(starship);

            bool exceptionThrown = false;

            try
            {
                Starship starshipCopy = _serializer.Deserialize<Starship>(bytes);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual("Please define a parameterless constructor for SerializationTests.TestClasses.Starship",ex.Message);
            }

            Assert.IsTrue(exceptionThrown);
        }

        #endregion

        #region Null Tests
        [Test]
        public void NullObject()
        {
            Person obj1 = null;

            byte[] bytes = _serializer.Serialize(obj1);
            Person obj2 = _serializer.Deserialize<Person>(bytes);

            if (!_compare.Compare(obj1, obj2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void ListOfNullObjects()
        {
            Person p1 = null;
            Person p2 = null;

            List<Person> list1 = new List<Person>();
            list1.Add(p1);
            list1.Add(p2);

            byte[] bytes = _serializer.Serialize(list1);
            List<Person> list2 = _serializer.Deserialize<List<Person>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion

        #region SortedList Tests
        [Test]
        public void TestSortedList()
        {
            Person p1 = new Person();
            p1.DateCreated = DateTime.Now;
            p1.Name = "Anna";
            Person p2 = new Person();
            p2.Name = "Greg";
            p2.DateCreated = DateTime.Now.AddDays(-1);

            SortedList<string, Person> dict1 = new SortedList<string, Person>();
            dict1.Add("1001", p1);
            dict1.Add("1002", p2);

            byte[] bytes = _serializer.Serialize(dict1);
            SortedList<string, Person> dict2 = _serializer.Deserialize<SortedList<string, Person>>(bytes);

            if (!_compare.Compare(dict1, dict2))
                throw new Exception(_compare.DifferencesString);

        }
        #endregion

        #region SortedDictionary Tests
        [Test]
        public void TestSortedDictionary()
        {
            Person p1 = new Person();
            p1.DateCreated = DateTime.Now;
            p1.Name = "Anna";
            Person p2 = new Person();
            p2.Name = "Greg";
            p2.DateCreated = DateTime.Now.AddDays(-1);

            SortedDictionary<string, Person> dict1 = new SortedDictionary<string, Person>();
            dict1.Add("1001", p1);
            dict1.Add("1002", p2);

            byte[] bytes = _serializer.Serialize(dict1);
            SortedDictionary<string, Person> dict2 = _serializer.Deserialize<SortedDictionary<string, Person>>(bytes);

            if (!_compare.Compare(dict1, dict2))
                throw new Exception(_compare.DifferencesString);

        }
        #endregion

        #region Dictionary Tests
        [Test]
        public void DeSerializeADictionaryWithAMillionEntries()
        {
            Dictionary<int,string> dictionary= new Dictionary<int, string>();
            
            for (int i=0;i<1000000;i++)
                dictionary.Add(i,i.ToString());

            byte[] bytes = _serializer.Serialize(dictionary);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            Dictionary<int, string> dictionary2 = _serializer.Deserialize<Dictionary<int, string>>(bytes);
            watch.Stop();

            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        [Test]
        public void TestDictionary()
        {
            Person p1 = new Person();
            p1.DateCreated = DateTime.Now;
            p1.Name = "Anna";
            Person p2 = new Person();
            p2.Name = "Greg";
            p2.DateCreated = DateTime.Now.AddDays(-1);

            Dictionary<string, Person> dict1 = new Dictionary<string, Person>();
            dict1.Add("1001", p1);
            dict1.Add("1002", p2);

            byte[] bytes = _serializer.Serialize(dict1);
            Dictionary<string, Person> dict2 = _serializer.Deserialize<Dictionary<string, Person>>(bytes);

            if (!_compare.Compare(dict1, dict2))
                throw new Exception(_compare.DifferencesString);

        }

        [Test]
        public void TestDictionary2()
        {
            Person p1 = new Person();
            p1.DateCreated = DateTime.Now;
            p1.Name = "Anna";
            Person p2 = new Person();
            p2.Name = "Greg";
            p2.DateCreated = DateTime.Now.AddDays(-1);

            Dictionary<int, Person> dict1 = new Dictionary<int, Person>();
            dict1.Add(1001, p1);
            dict1.Add(1002, p2);

            byte[] bytes = _serializer.Serialize(dict1);
            Dictionary<int, Person> dict2 = _serializer.Deserialize<Dictionary<int, Person>>(bytes);

            if (!_compare.Compare(dict1, dict2))
                throw new Exception(_compare.DifferencesString);

        }
        #endregion

        #region Struct Tests
        [Test]
        public void TestStruct()
        {
            Size size1 = new Size();
            size1.Width = 800;
            size1.Height = 600;

            byte[] bytes = _serializer.Serialize(size1);
            Size size2 = _serializer.Deserialize<Size>(bytes);

            if (!_compare.Compare(size1, size2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TestStructWithProperties()
        {
            SizeWithProperties size1 = new SizeWithProperties();
            size1.Width = 800;
            size1.Height = 600;

            byte[] bytes = _serializer.Serialize(size1);
            SizeWithProperties size2 = _serializer.Deserialize<SizeWithProperties>(bytes);

            if (!_compare.Compare(size1, size2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TestStructList()
        {
            Size size1 = new Size();
            size1.Width = 800;
            size1.Height = 600;

            Size size2 = new Size();
            size2.Width = 1024;
            size2.Height = 768;

            List<Size> list1 = new List<Size>();
            list1.Add(size1);
            list1.Add(size2);

            byte[] bytes= _serializer.Serialize(list1);
            List<Size> list2 = _serializer.Deserialize<List<Size>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TestStructListWithProperties()
        {
            SizeWithProperties size1 = new SizeWithProperties();
            size1.Width = 800;
            size1.Height = 600;

            SizeWithProperties size2 = new SizeWithProperties();
            size2.Width = 1024;
            size2.Height = 768;

            List<SizeWithProperties> list1 = new List<SizeWithProperties>();
            list1.Add(size1);
            list1.Add(size2);

            byte[] bytes = _serializer.Serialize(list1);
            List<SizeWithProperties> list2 = _serializer.Deserialize<List<SizeWithProperties>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion

        #region Enumeration Tests
        [Test]
        public void TestEnumeration()
        {
            List<Deck> list1 = new List<Deck>();
            list1.Add(Deck.Engineering);
            list1.Add(Deck.SickBay);

            byte[] bytes = _serializer.Serialize(list1);
            List<Deck> list2 = _serializer.Deserialize<List<Deck>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TestEnumerationByte()
        {
            List<DeckByte> list1 = new List<DeckByte>();
            list1.Add(DeckByte.Engineering);
            list1.Add(DeckByte.SickBay);

            byte[] bytes = _serializer.Serialize(list1);
            List<DeckByte> list2 = _serializer.Deserialize<List<DeckByte>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TestEnumerationShort()
        {
            List<DeckShort> list1 = new List<DeckShort>();
            list1.Add(DeckShort.Engineering);
            list1.Add(DeckShort.SickBay);

            byte[] bytes = _serializer.Serialize(list1);
            List<DeckShort> list2 = _serializer.Deserialize<List<DeckShort>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TestEnumerationLong()
        {
            List<DeckLong> list1 = new List<DeckLong>();
            list1.Add(DeckLong.Engineering);
            list1.Add(DeckLong.SickBay);

            byte[] bytes = _serializer.Serialize(list1);
            List<DeckLong> list2 = _serializer.Deserialize<List<DeckLong>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void TestNullableEnumeration()
        {
            List<Deck?> list1 = new List<Deck?>();
            list1.Add(Deck.Engineering);
            list1.Add(null);

            byte[] bytes = _serializer.Serialize(list1);
            List<Deck?> list2 = _serializer.Deserialize<List<Deck?>>(bytes);

            if (!_compare.Compare(list1, list2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion

        #region Array Tests
        [Test]
        public void SerializeArrayOfObjects()
        {
            List<object> objectArray = new List<object>();

            objectArray.Add("Bob");
            objectArray.Add(DateTime.Now);

            byte[] bytes = _serializer.Serialize(objectArray.ToArray());
            Console.WriteLine(bytes.Length);

            object[] objectArrayCopy = _serializer.Deserialize<object[]>(bytes);

            if (!_compare.Compare(objectArray.ToArray(),objectArrayCopy))
                Assert.Fail(_compare.DifferencesString);
        }

        [Test]
        public void ArrayTest()
        {
            Person[] array1 = new Person[100];

            for (int i = 0; i < 100; i++)
            {
                Person person = new Person();
                person.Name = "Greg " + i;
                person.DateCreated = DateTime.Now.AddSeconds(i);
                array1[i] = person;
            }

            byte[] bytes= _serializer.Serialize(array1);

            Person[] array2 = _serializer.Deserialize<Person[]>(bytes);

            if (!_compare.Compare(array1, array2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void SingleDimensionalByteArrayTest()
        {
            byte[] bytes1 = new byte[10];
            bytes1[0] = 0;
            bytes1[1] = 1;
            bytes1[2] = 2;
            bytes1[3] = 3;
            bytes1[4] = 4;
            bytes1[5] = 5;
            bytes1[6] = 6;
            bytes1[7] = 7;
            bytes1[8] = 8;
            bytes1[9] = 9;

            byte[] serializedBytes = _serializer.Serialize(bytes1);
            byte[] bytes2 = _serializer.Deserialize<byte[]>(serializedBytes);

            if (!_compare.Compare(bytes1, bytes2))
                throw new Exception(_compare.DifferencesString);
        }

        [Test]
        public void MultiDimensionalByteArrayTest()
        {
            byte[,] bytes1 = new byte[3, 2];

            bytes1[0, 0] = 5;
            bytes1[1, 0] = 10;
            bytes1[2, 0] = 15;
            bytes1[0, 1] = 20;
            bytes1[1, 1] = 25;
            bytes1[2, 1] = 30;

            byte[] serializedBytes = _serializer.Serialize(bytes1);
            byte[,] bytes2 = _serializer.Deserialize<byte[,]>(serializedBytes);

            if (!_compare.Compare(bytes1, bytes2))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion

        #region Entity Tree Tests
        /// <summary>
        /// This might need to pass again once we do indexes
        /// </summary>
        [Test]
        public void TestEntityTree()
        {
            List<Entity> entityTree = new List<Entity>();

            //Brave Sir Robin Security Company
            Entity top1 = new Entity();
            top1.EntityId = 1;
            top1.Description = "Brave Sir Robin Security Company";
            top1.Parent = null;
            top1.EntityLevel = Level.Company;
            entityTree.Add(top1);

            Entity div1 = new Entity();
            div1.EntityId = 2;
            div1.Description = "Minstrils";
            div1.EntityLevel = Level.Division;
            div1.Parent = top1;
            top1.Children.Add(div1);

            Entity div2 = new Entity();
            div2.EntityId = 3;
            div2.Description = "Sub Contracted Fighting";
            div2.EntityLevel = Level.Division;
            div2.Parent = top1;
            top1.Children.Add(div2);

            Entity dep2 = new Entity();
            dep2.EntityId = 4;
            dep2.Description = "Trojan Rabbit Department";
            dep2.EntityLevel = Level.Department;
            dep2.Parent = div2;
            div2.Children.Add(dep2);

            //Roger the Shrubber's Fine Shrubberies
            Entity top1b = new Entity();
            top1b.EntityId = 5;
            top1b.Description = "Roger the Shrubber's Fine Shrubberies";
            top1b.Parent = null;
            top1b.EntityLevel = Level.Company;
            entityTree.Add(top1b);

            Entity div1b = new Entity();
            div1b.EntityId = 6;
            div1b.Description = "Manufacturing";
            div1b.EntityLevel = Level.Division;
            div1b.Parent = top1b;
            top1b.Children.Add(div1b); 

            Entity dep1b = new Entity();
            dep1b.EntityId = 7;
            dep1b.Description = "Design Department";
            dep1b.EntityLevel = Level.Department;
            dep1b.Parent = div1b;
            div1b.Children.Add(dep1b);

            Entity dep2b = new Entity();
            dep2b.EntityId = 8;
            dep2b.Description = "Arranging Department";
            dep2b.EntityLevel = Level.Department;
            dep2b.Parent = div1b;
            div1b.Children.Add(dep2b);

            Entity div2b = new Entity();
            div2b.EntityId = 9;
            div2b.Description = "Sales";
            div2b.EntityLevel = Level.Division;
            div2b.Parent = top1b;
            top1b.Children.Add(div2b);

            byte[] bytes = _serializer.Serialize(entityTree);

            List<Entity> entityTreeCopy = _serializer.Deserialize<List<Entity>>(bytes);

            if (!_compare.Compare(entityTree, entityTreeCopy))
                throw new Exception(_compare.DifferencesString);
        }
        #endregion
    }
}
