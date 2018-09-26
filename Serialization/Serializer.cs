#region Includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
#if !NETSTANDARD
using System.Windows.Media.Imaging;
#endif
#endregion

namespace KellermanSoftware.Serialization
{
    /// <summary>
    /// Binary serializer that will convert all public fields and properties to a byte array
    /// </summary>
    public class Serializer
    {
        #region Class Variables

        /// <summary>
        /// You can pass in a list of properties to ignore for a type and they will not be serialized or deserialized
        /// </summary>
        public static Dictionary<Type,List<string>> IgnoreProperties = new Dictionary<Type, List<string>>();

        private Queue<SerializeItem> _serializeQueue;
        private Queue<DeserializeItem> _deserializeQueue;
        private static Assembly _callingAssembly;

        private List<object> _knownCustomTypeObjects;

        private List<DeserializedObject> _deserializedObjects;

        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> _fieldInfoDictionary;
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyInfoDictionary;
        private static readonly Dictionary<string, Type> _typesByName;
        private static readonly Dictionary<string, Type> _nameToTypeDictionary = new Dictionary<string, Type>();

        private const string VERSION_REGEX_STRING =
            @"\,\sVersion=\d+\.\d+\.\d+\.\d+\,\sCulture=[^\,]+\,\sPublicKeyToken=\w+";
#if SILVERLIGHT
        private static Regex _versionRegex = new Regex(VERSION_REGEX_STRING);
#else
        private static Regex _versionRegex = new Regex(VERSION_REGEX_STRING,RegexOptions.Compiled);
#endif

        private const string TYPE_NOT_FOUND = "Cannot find type {0}";
        private const string DICTIONARY = "System.Collections.Generic.Dictionary";
        private const string GENERIC_LIST = "System.Collections.Generic.List";
        private const string OBSERVABLE_COLLECTION = "System.Collections.ObjectModel.ObservableCollection";
        private const string DRAWING = "System.Drawing";

        #endregion

        #region Constructor
        static Serializer()
        {
            _fieldInfoDictionary = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            _propertyInfoDictionary = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            _typesByName = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Serializer()
        {
            _callingAssembly = Assembly.GetCallingAssembly();
            Setup();
        }


        private void Setup()
        {
            _serializeQueue = new Queue<SerializeItem>();
            _deserializeQueue = new Queue<DeserializeItem>();
            _knownCustomTypeObjects = new List<object>();
            _deserializedObjects = new List<DeserializedObject>();
        }
        #endregion

        #region Methods

        /// <summary>Serialize given object into an array of bytes</summary>
        /// <param name="objectToSerialize">The object to serialize</param>
        /// <returns>A byte array representation of the object</returns>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">Serializer serializer = new Serializer();
        ///  
        /// //Serialize and Deserialize a simple type
        /// DateTime date = new DateTime(2010,8,21,10,53,31,555);
        /// byte[] serializedDate = serializer.Serialize(date);
        /// DateTime dateCopy = serializer.Deserialize&lt;DateTime&gt;(serializedDate);
        ///  
        /// //Serialize and Deserialize a single object
        /// Person person = new Person();
        /// person.Name = "John";
        /// byte[] serialized = serializer.Serialize(person);
        ///  
        /// Person personCopy = serializer.Deserialize&lt;Person&gt;(serialized);
        ///  
        /// //Serialize and Deserialize a List
        /// List&lt;Person&gt; personList = new List&lt;Person&gt;();
        ///  
        /// Person person1 = new Person();
        /// person1.Name = "Sally";
        /// personList.Add(person1);
        ///  
        /// Person person2 = new Person();
        /// person2.Name = "Susan";
        /// personList.Add(person2);
        ///  
        /// byte[] serializedList = serializer.Serialize(personList);
        ///  
        /// List&lt;Person&gt; personListCopy = serializer.Deserialize&lt;List&lt;Person&gt;&gt;(serializedList);</code>
        /// 	<code title="Example2" description="" lang="VB.NET">Dim serializer As New Serializer()
        ///  
        /// 'Serialize and Deserialize a simple type
        /// Dim [date] As New Date(2010,8,21,10,53,31,555)
        /// Dim serializedDate() As Byte = serializer.Serialize([date])
        /// Dim dateCopy As Date = serializer.Deserialize(Of Date)(serializedDate)
        ///  
        /// 'Serialize and Deserialize a single object
        /// Dim person As New Person()
        /// person.Name = "John"
        /// Dim serialized() As Byte = serializer.Serialize(person)
        ///  
        /// Dim personCopy As Person = serializer.Deserialize(Of Person)(serialized)
        ///  
        /// 'Serialize and Deserialize a List
        /// Dim personList As New List(Of Person)()
        ///  
        /// Dim person1 As New Person()
        /// person1.Name = "Sally"
        /// personList.Add(person1)
        ///  
        /// Dim person2 As New Person()
        /// person2.Name = "Susan"
        /// personList.Add(person2)
        ///  
        /// Dim serializedList() As Byte = serializer.Serialize(personList)
        ///  
        /// Dim personListCopy As List(Of Person) = serializer.Deserialize(Of List(Of Person))(serializedList)</code>
        /// </example>
        public byte[] Serialize<T>(T objectToSerialize)
        {
            return Serialize(objectToSerialize, typeof(T));
        }

        /// <summary>
        /// Serialize given object into an array of bytes
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize</param>
        /// <param name="typeOfTheObject">Type of the object</param>
        /// <returns>A byte array representation of the object</returns>
        public byte[] Serialize(object objectToSerialize, 
                                Type typeOfTheObject)
        {
            using (MemoryStream outputStream = new MemoryStream())
            using (BinaryWriter outputWriter = new BinaryWriter(outputStream))
            {
                _knownCustomTypeObjects.Clear();
                _serializeQueue.Enqueue(new SerializeItem(objectToSerialize,
                                                          typeOfTheObject));
                while (_serializeQueue.Count > 0)
                {
                    SerializeObject(outputWriter, _serializeQueue.Dequeue());
                }
                return outputStream.ToArray();
            }
        }

        /// <summary>Deserialize the object contained by the given array of bytes</summary>
        /// <param name="serializedBytes">A byte array representation of the object</param>
        /// <returns>The object represented by the array of bytes</returns>
        /// <example>
        /// 	<code title="Example" description="" lang="C#">Serializer serializer = new Serializer();
        ///  
        /// //Serialize and Deserialize a simple type
        /// DateTime date = new DateTime(2010,8,21,10,53,31,555);
        /// byte[] serializedDate = serializer.Serialize(date);
        /// DateTime dateCopy = serializer.Deserialize&lt;DateTime&gt;(serializedDate);
        ///  
        /// //Serialize and Deserialize a single object
        /// Person person = new Person();
        /// person.Name = "John";
        /// byte[] serialized = serializer.Serialize(person);
        ///  
        /// Person personCopy = serializer.Deserialize&lt;Person&gt;(serialized);
        ///  
        /// //Serialize and Deserialize a List
        /// List&lt;Person&gt; personList = new List&lt;Person&gt;();
        ///  
        /// Person person1 = new Person();
        /// person1.Name = "Sally";
        /// personList.Add(person1);
        ///  
        /// Person person2 = new Person();
        /// person2.Name = "Susan";
        /// personList.Add(person2);
        ///  
        /// byte[] serializedList = serializer.Serialize(personList);
        ///  
        /// List&lt;Person&gt; personListCopy = serializer.Deserialize&lt;List&lt;Person&gt;&gt;(serializedList);</code>
        /// </example>
        public T Deserialize<T>(byte[] serializedBytes)
        {
            return (T)Deserialize(serializedBytes, typeof(T));
        }

        /// <summary>
        /// Deserialize the object contained by the given array of bytes
        /// </summary>
        /// <param name = "serializedBytes">A byte array representation of the object</param>
        /// <param name="typeOfTheObject"></param>
        /// <returns>The object represented by the array of bytes</returns>
        public object Deserialize(byte[] serializedBytes, Type typeOfTheObject)
        {
            using (MemoryStream inputStream = new MemoryStream(serializedBytes))
            using (BinaryReader inputReader = new BinaryReader(inputStream))
            {
                _knownCustomTypeObjects.Clear();
                _deserializedObjects.Clear();
                _deserializedObjects.Add(new DeserializedObject(GetDefaultValue(typeOfTheObject),
                                                                -1,
                                                                null,
                                                                null,
                                                                null));

                _deserializeQueue.Enqueue(new DeserializeItem(0,
                                                              typeOfTheObject,
                                                              null,
                                                              null,
                                                              null));
                while (_deserializeQueue.Count > 0)
                {
                    try
                    {
                        DeserializeObject(inputReader, _deserializeQueue.Dequeue());
                    }
                    catch (EndOfStreamException)
                    {
                        throw new BytesCorruptedException();
                    }
                }

                DeserializedObject theObject;
                for (int i = _deserializedObjects.Count - 1; i > 0; --i)
                {
                    theObject = _deserializedObjects[i];
                    if (theObject.Value is DeserializedCollection)
                    {
                        object collection = ((DeserializedCollection)theObject.Value).Collection;
                        if (theObject.FieldInfo != null)
                        {
                            theObject.FieldInfo.SetValue(_deserializedObjects[theObject.ParentIndex].Value,
                                                         collection);
                        }
                        else if (theObject.PropertyInfo != null)
                        {
                            theObject.PropertyInfo.SetValue(_deserializedObjects[theObject.ParentIndex].Value,
                                                            collection,
                                                            null);
                        }
                        else if (theObject.ParentIndex == 0)
                        {
                            _deserializedObjects[0].Value = collection;
                        }
                        else if (theObject.IndexInCollection != null) // check if is collection item
                        {
                            ((DeserializedCollection)_deserializedObjects[theObject.ParentIndex].Value).AddItem(
                                theObject.IndexInCollection.Value,
                                collection);
                        }
                    }
                    else if (theObject.FieldInfo != null)
                    {
                        theObject.FieldInfo.SetValue(_deserializedObjects[theObject.ParentIndex].Value,
                                                     theObject.Value);
                    }
                    else if (theObject.PropertyInfo != null)
                    {
                        theObject.PropertyInfo.SetValue(_deserializedObjects[theObject.ParentIndex].Value,
                                                        theObject.Value,
                                                        null);
                    }
                    else if (theObject.IndexInCollection != null) // check if is collection item
                    {
                        ((DeserializedCollection)_deserializedObjects[theObject.ParentIndex].Value).AddItem(
                            theObject.IndexInCollection.Value,
                            theObject.Value);
                    }
                }

                return _deserializedObjects[0].Value;
            }
        }

        private void SerializeObject(BinaryWriter writer, SerializeItem item)
        {
            bool isNullable = IsNullableType(item.Type);
            Type workingType = isNullable ? Nullable.GetUnderlyingType(item.Type)
                                          : item.Type;
            WriteType(writer, isNullable, item.Type);

            if (IsDefaultValue(item.Item, item.Type))
            {
                // mark is default value
                writer.Write((byte)DefaultLabel.Default);
            }
            else
            {
                // mark is not default value
                writer.Write((byte)DefaultLabel.NotDefault);

                if (IsSimpleType(workingType))
                {
                    WriteSimpleTypeValue(writer, item.Item, workingType);
                }
#if !NETSTANDARD
                else if (item.Item is WriteableBitmap)
                {
                    WriteableBitmapLogic.SerializeWriteableBitmap(writer, 
                                                                  (WriteableBitmap) item.Item);
                }
#endif
                else if (item.Item is BitArray)
                {
                    SerializeBitArray(writer, (BitArray) item.Item);
                }
                else if (typeof(Array).IsAssignableFrom(item.Type))
                {
                    SerializeArray(writer, (Array)item.Item);
                }
                else if (typeof(IList).IsAssignableFrom(item.Type))
                {
                    SerializeList(writer, (IList)item.Item);
                }
                else if (typeof(IDictionary).IsAssignableFrom(item.Type))
                {
                    SerializeDictionary(writer, (IDictionary)item.Item);
                }
                else
                {
                    SerializeCustomTypeObject(writer, item.Item, workingType);
                }
            }
        }

        private void DeserializeObject(BinaryReader reader, DeserializeItem item)
        {
            Type serializedType = ReadType(reader);
            if (item.Type == typeof(object) || item.Type.IsInterface)
            {
                item.Type = serializedType;
            }

            DefaultLabel defaultLabel = (DefaultLabel)reader.ReadByte();

            if (IsSimpleType(serializedType))
            {
                DeserializeSimpleTypeObject(reader,
                                            item,
                                            serializedType,
                                            defaultLabel);
            }
            else if (serializedType == typeof(BitArray))
            {
                DeserializeBitArray(reader, item, serializedType, defaultLabel);
            }
#if !NETSTANDARD
            else if (serializedType == typeof(WriteableBitmap))
            {
                DeserializeWriteableBitmap(reader, item, serializedType, defaultLabel);
            }
#endif
            else if (serializedType.IsArray)
            {
                DeserializeArray(reader, item, serializedType, defaultLabel);
            }
            else if (typeof(IList).IsAssignableFrom(serializedType))
            {
                DeserializeList(reader, item, serializedType, defaultLabel);
            }
            else if (typeof(IDictionary).IsAssignableFrom(serializedType))
            {
                DeserializeDictionary(reader, item, serializedType, defaultLabel);
            }
            else
            {
                try
                {

                DeserializeCustomTypeObject(reader,
                                            item,
                                            serializedType,
                                            defaultLabel);
                }
                catch (MissingMethodException ex)
                {
                    string msg = string.Format("Please define a parameterless constructor for {0}",
                                               serializedType.FullName);
                    throw new MissingMethodException(msg,ex);
                }

            }
        }

#if !NETSTANDARD
        private void DeserializeWriteableBitmap(BinaryReader reader, 
                                                DeserializeItem item, 
                                                Type serializedType, 
                                                DefaultLabel defaultLabel)
        {
            if (defaultLabel == DefaultLabel.Default)
            {
                SetDeserializedValue(item, null, serializedType);
                return;
            }

            SetDeserializedValue(item, 
                                 WriteableBitmapLogic.ReadWriteableBitmap(reader), 
                                 typeof(WriteableBitmap));
        }
#endif

        private void SerializeBitArray(BinaryWriter writer, BitArray bitArray)
        {
            //Write the length
            writer.Write(bitArray.Length);

            //Copy to bool array            
            bool[] bits = new bool[bitArray.Length];
            bitArray.CopyTo(bits, 0);

            foreach (bool bit in bits)
            {
                writer.Write(bit);
            }
        }

        private void SerializeArray(BinaryWriter writer, Array array)
        {
            int arrayRank = array.Rank;
            writer.Write(arrayRank); // write number of array dimensions
            for (int i = 0; i < arrayRank; ++i)
            {
                writer.Write(array.GetLength(i)); // write i-th dimension length
            }

            Type elementType = array.GetType().GetElementType();
            // optimization for array of byte
            if (elementType == typeof(byte) && arrayRank == 1)
            {
                writer.Write((byte[])array, 0, array.Length);
                return;
            }
            foreach (object item in array)
            {
                _serializeQueue.Enqueue(new SerializeItem(item, elementType));
            }
        }

        private void DeserializeBitArray(BinaryReader reader,
                                         DeserializeItem item,
                                         Type arrayType,
                                         DefaultLabel defaultLabel)
        {
            if (defaultLabel == DefaultLabel.Default)
            {
                SetDeserializedValue(item, null, arrayType);
                return;
            }

            int totalLength = reader.ReadInt32();
            bool[] boolArray = new bool[totalLength];

            for (int i = 0; i < totalLength; i++)
                boolArray[i]= reader.ReadBoolean();

            SetDeserializedValue(item, new BitArray(boolArray), typeof(BitArray));
        }

        private void DeserializeArray(BinaryReader reader,
                                      DeserializeItem item,
                                      Type arrayType,
                                      DefaultLabel defaultLabel)
        {
            if (defaultLabel == DefaultLabel.Default)
            {
                SetDeserializedValue(item, null, arrayType);
                return;
            }

            int arrayRank = reader.ReadInt32();
            int[] dimensionsLengths = new int[arrayRank];
            int totalLength = 0;
            for (int i = 0; i < arrayRank; ++i)
            {
                dimensionsLengths[i] = reader.ReadInt32();
                if (totalLength == 0)
                {
                    totalLength = dimensionsLengths[i];
                }
                else
                {
                    totalLength *= dimensionsLengths[i];
                }
            }
            Type elementType = arrayType.GetElementType();
            if (item != null) // check if the item is not missed
            {
                // optimization for array of bytes
                if (elementType == typeof(byte) && arrayRank == 1)
                {
                    byte[] array = reader.ReadBytes(totalLength);
                    SetDeserializedValue(item, array, typeof(byte[]));
                    return;
                }
                _deserializedObjects.Add(
                    new DeserializedObject(new DeserializedCollection(CollectionType.Array,
                                                                      Array.CreateInstance(elementType,
                                                                                           dimensionsLengths)),
                                           item.ParentIndex,
                                           item.FieldInfo,
                                           item.PropertyInfo,
                                           item.IndexInCollection));
            }

            for (int i = 0; i < totalLength; ++i)
            {
                _deserializeQueue.Enqueue(new DeserializeItem(_deserializedObjects.Count - 1,
                                                              elementType,
                                                              null,
                                                              null,
                                                              i));
            }
        }

        private void SerializeList(BinaryWriter writer, IList list)
        {
            int listLength = list.Count;
            writer.Write(listLength); // write list length
            Type listType = list.GetType();
            if (listType.BaseType != null &&
                listType.BaseType.FullName.Contains(GENERIC_LIST))
            {
                listType = listType.BaseType;
            }
            Type elementType = listType.IsGenericType
                                   ? listType.GetGenericArguments()[0]
                                   : typeof(object);
            for (int i = 0; i < listLength; ++i)
            {
                _serializeQueue.Enqueue(new SerializeItem(list[i],
                                                          elementType));
            }
        }

        private void DeserializeList(BinaryReader reader,
                                     DeserializeItem item,
                                     Type listType,
                                     DefaultLabel defaultLabel)
        {
            if (defaultLabel == DefaultLabel.Default)
            {
                SetDeserializedValue(item, null, listType);
                return;
            }

            int listLength = reader.ReadInt32();
            Type underlyingType = listType;
            if (listType.BaseType != null &&
                listType.BaseType.FullName.Contains(GENERIC_LIST))
            {
                underlyingType = listType.BaseType;
            }
            Type elementType = underlyingType.IsGenericType
                                   ? underlyingType.GetGenericArguments()[0]
                                   : typeof(object);

            if (item != null) // check if the item is not missed
            {
                object collection = CreateCollectionInstance(listType, listLength);
                if (collection != null)
                {
                    _deserializedObjects.Add(
                        new DeserializedObject(new DeserializedCollection(CollectionType.List,
                                                                          collection),
                                               item.ParentIndex,
                                               item.FieldInfo,
                                               item.PropertyInfo,
                                               item.IndexInCollection));
                }
            }

            for (int i = 0; i < listLength; ++i)
            {
                _deserializeQueue.Enqueue(new DeserializeItem(_deserializedObjects.Count - 1,
                                                              elementType,
                                                              null,
                                                              null,
                                                              i));
            }
        }

        private void SerializeDictionary(BinaryWriter writer, IDictionary dictionary)
        {
            int dictionaryLength = dictionary.Count;
            writer.Write(dictionaryLength); // write dictionary length
            Type dictionaryType = dictionary.GetType();
            if (dictionaryType.BaseType != null &&
                dictionaryType.BaseType.FullName.Contains(DICTIONARY))
            {
                dictionaryType = dictionaryType.BaseType;
            }
            Type keyType = dictionaryType.IsGenericType
                               ? dictionaryType.GetGenericArguments()[0]
                               : typeof(object);
            Type valueType = dictionaryType.IsGenericType
                                 ? dictionaryType.GetGenericArguments()[1]
                                 : typeof(object);
            foreach (DictionaryEntry entry in dictionary)
            {
                _serializeQueue.Enqueue(new SerializeItem(entry.Key, keyType));
                _serializeQueue.Enqueue(new SerializeItem(entry.Value, valueType));
            }
        }

        private void DeserializeDictionary(BinaryReader reader,
                                           DeserializeItem item,
                                           Type dictionaryType,
                                           DefaultLabel defaultLabel)
        {
            if (defaultLabel == DefaultLabel.Default)
            {
                SetDeserializedValue(item, null, dictionaryType);
                return;
            }

            int dictionaryLength = reader.ReadInt32();

            if (item != null) // check if the item is not missed
            {
                object dictionary = CreateCollectionInstance(dictionaryType, 
                                                             dictionaryLength);
                if (dictionary != null)
                {
                    _deserializedObjects.Add(
                        new DeserializedObject(new DeserializedCollection(CollectionType.Dictionary,
                                                                          dictionary),
                                               item.ParentIndex,
                                               item.FieldInfo,
                                               item.PropertyInfo,
                                               item.IndexInCollection));
                }
            }
            
            if (dictionaryType.BaseType != null &&
                dictionaryType.BaseType.FullName.Contains(DICTIONARY))
            {
                dictionaryType = dictionaryType.BaseType;
            }
            Type keyType = dictionaryType.IsGenericType
                               ? dictionaryType.GetGenericArguments()[0]
                               : typeof(object);
            Type valueType = dictionaryType.IsGenericType
                                 ? dictionaryType.GetGenericArguments()[1]
                                 : typeof(object);
            int indexInCollection = 0;
            for (int i = 0; i < dictionaryLength; ++i)
            {
                _deserializeQueue.Enqueue(new DeserializeItem(_deserializedObjects.Count - 1,
                                                              keyType,
                                                              null,
                                                              null,
                                                              indexInCollection++));
                _deserializeQueue.Enqueue(new DeserializeItem(_deserializedObjects.Count - 1,
                                                              valueType,
                                                              null,
                                                              null,
                                                              indexInCollection++));
            }
        }

        private void SerializeCustomTypeObject(BinaryWriter writer,
                                               object objectToSerialize,
                                               Type objectType)
        {
            int knownObjectIndex =
                _knownCustomTypeObjects.IndexOf(objectToSerialize);
            if (knownObjectIndex == -1) // nothing was found
            {
                writer.Write((byte)KnownObjectLabel.Unknown);
                _knownCustomTypeObjects.Add(objectToSerialize);

                Dictionary<string, FieldInfo> fields = GetFieldInfo(objectType);
                writer.Write((ushort)fields.Count); // write fields number
                foreach (KeyValuePair<string, FieldInfo> fieldPair in fields)
                {
                    _serializeQueue.Enqueue(new SerializeItem(fieldPair.Value.GetValue(objectToSerialize),
                                                              fieldPair.Value.FieldType));
                    writer.Write(fieldPair.Key); // write field name
                }
                Dictionary<string, PropertyInfo> properties = GetPropertyInfo(objectType);
                writer.Write((ushort)properties.Count); // write properties number
                foreach (KeyValuePair<string, PropertyInfo> propertyPair in properties)
                {
                    _serializeQueue.Enqueue(new SerializeItem(propertyPair.Value.GetValue(objectToSerialize,
                                                                                          null),
                                                              propertyPair.Value.PropertyType));
                    writer.Write(propertyPair.Key); // write property name
                }
            }
            else
            {
                writer.Write((byte)KnownObjectLabel.Known);
                writer.Write(knownObjectIndex);
            }
        }

        private void DeserializeSimpleTypeObject(BinaryReader reader,
                                                 DeserializeItem item,
                                                 Type serializedType,
                                                 DefaultLabel defaultLabel)
        {
            object value;
            switch (defaultLabel)
            {
                case DefaultLabel.Default:
                    value = GetDefaultValue(serializedType);
                    break;
                case DefaultLabel.NotDefault:
                    Type workingType = IsNullableType(serializedType)
                                           ? Nullable.GetUnderlyingType(serializedType)
                                           : serializedType;
                    value = ReadSimpleTypeValue(reader, workingType);
                    break;
                default:
                    throw new SerializerException(string.Format("Unknown default label: {0}",
                                                                           defaultLabel));
            }

            SetDeserializedValue(item, value, serializedType);
        }

        private void SetDeserializedValue(DeserializeItem item,
                                          object value,
                                          Type serializedType)
        {
            if (item != null) // check if the item is not missed
            {
                // check if source and target types are equal
                if (serializedType != item.Type)
                {
                    value = ChangeType(value, serializedType, item.Type);
                }

                DeserializedObject parentObject =
                    _deserializedObjects[item.ParentIndex];
                if (item.FieldInfo != null)
                {
                    // set field
                    item.FieldInfo.SetValue(parentObject.Value, value);
                }
                else if (item.PropertyInfo != null)
                {
                    // set property
                    item.PropertyInfo.SetValue(parentObject.Value, value, null);
                }
                else if (item.IndexInCollection != null) // check if is collection item
                {
                    // set collection item
                    ((DeserializedCollection)parentObject.Value).AddItem(item.IndexInCollection.Value,
                                                                          value);
                }
                else
                {
                    // set value for deserialized simple type
                    parentObject.Value = value;
                }
            }
        }

        private void DeserializeCustomTypeObject(BinaryReader reader,
                                                 DeserializeItem item,
                                                 Type serializedType,
                                                 DefaultLabel defaultLabel)
        {
            if (item != null) // check if the item is not missed
            {
                DeserializedObject parentObject =
                    _deserializedObjects[item.ParentIndex];
                // create top level deserialized object
                if (item.ParentIndex == 0 &&
                    Equals(parentObject.Value, GetDefaultValue(item.Type)))
                {
                    if (serializedType != item.Type &&
                        serializedType.Name != item.Type.Name)
                    {
                        throw new SerializerException(string.Format("Cannot cast from {0} to {1}",
                                                                    serializedType,
                                                                    item.Type));
                    }
                    if (defaultLabel == DefaultLabel.Default)
                    {
                        parentObject.Value = GetDefaultValue(item.Type);
                        return;
                    }

                    // read known object label, it can't be known for top level object
                    reader.ReadByte();

                    object value = Activator.CreateInstance(item.Type);
                    parentObject.Value = value;
                    _knownCustomTypeObjects.Add(value);
                }

                if (item.FieldInfo != null)
                {
                    if (defaultLabel == DefaultLabel.Default)
                    {
                        item.FieldInfo.SetValue(parentObject.Value,
                                                GetDefaultValue(item.Type));
                        return;
                    }

                    if ((KnownObjectLabel)reader.ReadByte() == KnownObjectLabel.Known)
                    {
                        int knownObjectIndex = reader.ReadInt32();
                        item.FieldInfo.SetValue(parentObject.Value,
                                                _knownCustomTypeObjects[knownObjectIndex]);
                        return;
                    }

                    object value = Activator.CreateInstance(item.Type);
                    _deserializedObjects.Add(
                        new DeserializedObject(value,
                                               item.ParentIndex,
                                               item.FieldInfo,
                                               item.PropertyInfo,
                                               null));
                    _knownCustomTypeObjects.Add(value);
                }
                else if (item.PropertyInfo != null)
                {
                    if (defaultLabel == DefaultLabel.Default)
                    {
                        item.PropertyInfo.SetValue(parentObject.Value,
                                                   GetDefaultValue(item.Type),
                                                   null);
                        return;
                    }

                    if ((KnownObjectLabel)reader.ReadByte() == KnownObjectLabel.Known)
                    {
                        int knownObjectIndex = reader.ReadInt32();
                        item.PropertyInfo.SetValue(parentObject.Value,
                                                   _knownCustomTypeObjects[knownObjectIndex],
                                                   null);
                        return;
                    }

                    object value = Activator.CreateInstance(item.Type);
                    _deserializedObjects.Add(
                        new DeserializedObject(value,
                                               item.ParentIndex,
                                               item.FieldInfo,
                                               item.PropertyInfo,
                                               null));
                    _knownCustomTypeObjects.Add(value);
                }
                else if (item.IndexInCollection != null) // check if is collection item
                {
                    if (defaultLabel == DefaultLabel.Default)
                    {
                        ((DeserializedCollection)parentObject.Value).AddItem(item.IndexInCollection.Value,
                                                                             GetDefaultValue(item.Type));
                        return;
                    }

                    if ((KnownObjectLabel)reader.ReadByte() == KnownObjectLabel.Known)
                    {
                        int knownObjectIndex = reader.ReadInt32();
                        ((DeserializedCollection)parentObject.Value).AddItem(item.IndexInCollection.Value,
                                                                             _knownCustomTypeObjects[knownObjectIndex]);
                        return;
                    }

                    object value = Activator.CreateInstance(item.Type);
                    _deserializedObjects.Add(
                        new DeserializedObject(value,
                                               item.ParentIndex,
                                               null,
                                               null,
                                               item.IndexInCollection.Value));
                    _knownCustomTypeObjects.Add(value);
                }
            }
            else
            {
                if ((KnownObjectLabel)reader.ReadByte() == KnownObjectLabel.Known)
                {
                    reader.ReadInt32();
                    return;
                }
            }

            ReadFieldsAndProperties(reader, item);
        }

        private void ReadFieldsAndProperties(BinaryReader reader, DeserializeItem item)
        {
            ushort fieldsNumber = reader.ReadUInt16();
            for (int i = 0; i < fieldsNumber; ++i)
            {
                string fieldName = reader.ReadString();
                if (item == null) // check if the item is missed
                {
                    _deserializeQueue.Enqueue(null); // field of the missed item is missed too
                }
                else
                {
                    FieldInfo fieldInfo = GetFieldInfo(item.Type, fieldName);
                    if (fieldInfo == null) // check if the serialized field is missed
                    {
                        _deserializeQueue.Enqueue(null); // missed field
                    }
                    else
                    {
                        _deserializeQueue.Enqueue(
                            new DeserializeItem(_deserializedObjects.Count - 1,
                                                fieldInfo.FieldType,
                                                fieldInfo,
                                                null,
                                                null));
                    }
                }
            }

            ushort propertiesNumber = reader.ReadUInt16();
            for (int i = 0; i < propertiesNumber; ++i)
            {
                string propertyName = reader.ReadString();
                if (item == null) // check if the item is missed
                {
                    _deserializeQueue.Enqueue(null); // property of the missed item is missed too
                }
                else
                {
                    PropertyInfo propertyInfo = GetPropertyInfo(item.Type,
                                                                propertyName);
                    if (propertyInfo == null) // check if the serialized property is missed
                    {
                        _deserializeQueue.Enqueue(null); // missed property
                    }
                    else
                    {
                        _deserializeQueue.Enqueue(
                            new DeserializeItem(_deserializedObjects.Count - 1,
                                                propertyInfo.PropertyType,
                                                null,
                                                propertyInfo,
                                                null));
                    }
                }
            }
        }

        /// <summary>
        /// Return default value for given type
        /// </summary>
        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type)
                                    : null;
        }

        /// <summary>
        /// Check if value of the given item is equal to default value for given type
        /// </summary>
        private static bool IsDefaultValue(object item, Type itemType)
        {
            return Equals(item, GetDefaultValue(itemType));
        }

        /// <summary>
        /// Check if the given type is a simple type that can be serialized fast
        /// </summary>
        private static bool IsSimpleType(Type type)
        {
            Type workingType = IsNullableType(type)
                                   ? Nullable.GetUnderlyingType(type)
                                   : type;
            return workingType.IsPrimitive ||
                   workingType == typeof(decimal) ||
                   workingType == typeof(DateTime) ||
                   workingType == typeof(DateTimeOffset) ||
                   workingType == typeof(TimeSpan) ||
                   workingType == typeof(string) ||
                   workingType.IsEnum ||
                   workingType == typeof(Guid) ||
                   workingType == typeof(Uri);
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        private static void WriteType(BinaryWriter writer,
                                      bool isNullable,
                                      Type type)
        {
            writer.Write(isNullable ? (byte)NullableLabel.Nullable
                                    : (byte)NullableLabel.NotNullable);
            Type workingType = isNullable ? Nullable.GetUnderlyingType(type)
                                          : type;
            if (workingType == typeof(bool))
            {
                writer.Write((byte)TypeLabel.Bool);
            }
            else if (workingType == typeof(byte))
            {
                writer.Write((byte)TypeLabel.Byte);
            }
            else if (workingType == typeof(char))
            {
                writer.Write((byte)TypeLabel.Char);
            }
            else if (workingType == typeof(decimal))
            {
                writer.Write((byte)TypeLabel.Decimal);
            }
            else if (workingType == typeof(double))
            {
                writer.Write((byte)TypeLabel.Double);
            }
            else if (workingType == typeof(float))
            {
                writer.Write((byte)TypeLabel.Float);
            }
            else if (workingType == typeof(int))
            {
                writer.Write((byte)TypeLabel.Int);
            }
            else if (workingType == typeof(long))
            {
                writer.Write((byte)TypeLabel.Long);
            }
            else if (workingType == typeof(sbyte))
            {
                writer.Write((byte)TypeLabel.SByte);
            }
            else if (workingType == typeof(short))
            {
                writer.Write((byte)TypeLabel.Short);
            }
            else if (workingType == typeof(string))
            {
                writer.Write((byte)TypeLabel.String);
            }
            else if (workingType == typeof(uint))
            {
                writer.Write((byte)TypeLabel.UInt);
            }
            else if (workingType == typeof(ulong))
            {
                writer.Write((byte)TypeLabel.ULong);
            }
            else if (workingType == typeof(ushort))
            {
                writer.Write((byte)TypeLabel.UShort);
            }
            else if (workingType == typeof(DateTime))
            {
                writer.Write((byte)TypeLabel.DateTime);
            }
            else if (workingType == typeof(DateTimeOffset))
            {
                writer.Write((byte)TypeLabel.DateTimeOffset);
            }
            else if (workingType == typeof(TimeSpan))
            {
                writer.Write((byte)TypeLabel.TimeSpan);
            }
            else if (workingType == typeof(Guid))
            {
                writer.Write((byte)TypeLabel.Guid);
            }
            else if (workingType == typeof(Uri))
            {
                writer.Write((byte) TypeLabel.Uri);
            }
            else if (workingType == typeof(BitArray))
            {
                writer.Write((byte)TypeLabel.BitArray);
            }
#if !NETSTANDARD
            else if (workingType == typeof(WriteableBitmap))
            {
                writer.Write((byte)TypeLabel.WriteableBitmap);
            }
#endif
            else if (workingType.IsEnum)
            {
                writer.Write((byte)TypeLabel.Enum);
            }
            else if (workingType == typeof(object[]))
            {
                writer.Write((byte)TypeLabel.ObjectArray);
            }
            else
            {
                writer.Write((byte)TypeLabel.Custom);
                writer.Write(workingType.AssemblyQualifiedName);
            }
        }

        ///<summary>
        /// Try to get the type by the specific version, then without the version
        ///</summary>
        ///<param name="typeName"></param>
        ///<param name="errorMessage"></param>
        ///<returns></returns>
        ///<exception cref="SerializerException"></exception>
        public static Type GetCustomType(string typeName, string errorMessage)
        {
            if (_nameToTypeDictionary.ContainsKey(typeName))
                return _nameToTypeDictionary[typeName];

            Type type;
            Exception inner = null;

            try
            {
                type = Type.GetType(typeName, false);
            }
            catch (Exception)
            {
                type = null;
            }

            if (type == null)
            {
                try
                {
                    string typeNameWithoutVersion = ReplaceVersion(typeName);
                    type = Type.GetType(typeNameWithoutVersion, false);
                }
                catch (Exception ex)
                {
                    inner = ex;
                }
            }

            if (type == null)
            {
                type = SearchForTypeInAssemblies(_callingAssembly, typeName);
            }

            if (type == null)
            {
                throw new SerializerException(string.Format(errorMessage,
                                                       typeName),inner);
            }

            _nameToTypeDictionary.Add(typeName,type);
            return type;
        }

        /// <summary>
        /// Replace the version in an AssemblyQualifiedName with an empty string
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        private static string ReplaceVersion(string typeString)
        {
            if (typeString.Contains(OBSERVABLE_COLLECTION) 
                || typeString.Contains(DRAWING))
                return typeString;

            return _versionRegex.Replace(typeString, "");
        }

        private static Type ReadType(BinaryReader reader)
        {
            NullableLabel nullableLabel = (NullableLabel)reader.ReadByte();
            TypeLabel typeLabel = (TypeLabel)reader.ReadByte();
            Type type;
            switch (typeLabel)
            {
                case TypeLabel.Custom:
                    string typeName = reader.ReadString();
                    type = GetCustomType(typeName, TYPE_NOT_FOUND);
                    break;
                case TypeLabel.Bool:
                    type = typeof(bool);
                    break;
                case TypeLabel.Byte:
                    type = typeof(byte);
                    break;
                case TypeLabel.Char:
                    type = typeof(char);
                    break;
                case TypeLabel.Decimal:
                    type = typeof(decimal);
                    break;
                case TypeLabel.Double:
                    type = typeof(double);
                    break;
                case TypeLabel.Float:
                    type = typeof(float);
                    break;
                case TypeLabel.Int:
                    type = typeof(int);
                    break;
                case TypeLabel.Long:
                    type = typeof(long);
                    break;
                case TypeLabel.SByte:
                    type = typeof(sbyte);
                    break;
                case TypeLabel.Short:
                    type = typeof(short);
                    break;
                case TypeLabel.String:
                    type = typeof(string);
                    break;
                case TypeLabel.UInt:
                    type = typeof(uint);
                    break;
                case TypeLabel.ULong:
                    type = typeof(ulong);
                    break;
                case TypeLabel.UShort:
                    type = typeof(ushort);
                    break;
                case TypeLabel.DateTime:
                    type = typeof(DateTime);
                    break;
                case TypeLabel.DateTimeOffset:
                    type = typeof(DateTimeOffset);
                    break;
                case TypeLabel.TimeSpan:
                    type = typeof(TimeSpan);
                    break;
                case TypeLabel.Guid:
                    type = typeof(Guid);
                    break;
                case TypeLabel.Uri:
                    type = typeof (Uri);
                    break;
                case TypeLabel.BitArray:
                    type = typeof (BitArray);
                    break;
#if !NETSTANDARD
                case TypeLabel.WriteableBitmap:
                    type = typeof (WriteableBitmap);
                    break;
#endif
                case TypeLabel.Enum:
                    type = typeof(int);
                    break;
                case TypeLabel.ObjectArray:
                    type = typeof (object[]);
                    break;
                default:
                    throw new SerializerException(string.Format("Unknown type label: {0}",
                                                                           typeLabel));
            }
            switch (nullableLabel)
            {
                case NullableLabel.NotNullable:
                    return type;
                case NullableLabel.Nullable:
                    return typeof(Nullable<>).MakeGenericType(type);
                default:
                    throw new SerializerException(string.Format("Unknown nullable label: {0}",
                                                                           typeLabel));
            }
        }

        private static void WriteSimpleTypeValue(BinaryWriter writer,
                                                 object value,
                                                 Type type)
        {
            if (type == typeof(bool))
            {
                writer.Write((bool)value);
            }
            else if (type == typeof(byte))
            {
                writer.Write((byte)value);
            }
            else if (type == typeof(char))
            {
                writer.Write((char)value);
            }
            else if (type == typeof(decimal))
            {
                int[] parts = Decimal.GetBits((decimal)value);
                foreach (int item in parts)
                {
                    writer.Write(item);
                }
            }
            else if (type == typeof(double))
            {
                writer.Write((double)value);
            }
            else if (type == typeof(float))
            {
                writer.Write((float)value);
            }
            else if (type == typeof(int))
            {
                writer.Write((int)value);
            }
            else if (type == typeof(long))
            {
                writer.Write((long)value);
            }
            else if (type == typeof(sbyte))
            {
                writer.Write((sbyte)value);
            }
            else if (type == typeof(short))
            {
                writer.Write((short)value);
            }
            else if (type == typeof(string))
            {
                writer.Write((string)value);
            }
            else if (type == typeof(uint))
            {
                writer.Write((uint)value);
            }
            else if (type == typeof(ulong))
            {
                writer.Write((ulong)value);
            }
            else if (type == typeof(ushort))
            {
                writer.Write((ushort)value);
            }
            else if (type == typeof(DateTime))
            {
                writer.Write(((DateTime)value).Ticks);
            }
            else if (type == typeof(DateTimeOffset))
            {
                writer.Write(((DateTimeOffset)value).ToFileTime());
            }
            else if (type == typeof(TimeSpan))
            {
                writer.Write(((TimeSpan)value).Ticks);
            }
            else if (type == typeof(Guid))
            {
                writer.Write(((Guid)value).ToByteArray());
            }
            else if (type == typeof(Uri))
            {
                writer.Write(((Uri)value).OriginalString);
            }
            else if (type.IsEnum)
            {
                writer.Write(Convert.ToInt32(value));
            }
            else
            {
                throw new SerializerException(string.Format("Unknown simple type: {0}",
                                                                       value.GetType()));
            }
        }

        private static object ReadSimpleTypeValue(BinaryReader reader, Type type)
        {
            if (type == typeof(bool))
            {
                return reader.ReadBoolean();
            }
            if (type == typeof(byte))
            {
                return reader.ReadByte();
            }
            if (type == typeof(char))
            {
                return reader.ReadChar();
            }
            if (type == typeof(decimal))
            {
                int[] parts = new int[4];
                for (int i = 0; i < 4; ++i)
                {
                    parts[i] = reader.ReadInt32();
                }
                bool isNegative = ((parts[3] >> 31) & 0x00000001) == 1;
                byte scale = (byte)((parts[3] >> 16) & 0x000000FF);
                return new Decimal(parts[0], parts[1], parts[2], isNegative, scale);
            }
            if (type == typeof(double))
            {
                return reader.ReadDouble();
            }
            if (type == typeof(float))
            {
                return reader.ReadSingle();
            }
            if (type == typeof(int))
            {
                return reader.ReadInt32();
            }
            if (type == typeof(long))
            {
                return reader.ReadInt64();
            }
            if (type == typeof(sbyte))
            {
                return reader.ReadSByte();
            }
            if (type == typeof(short))
            {
                return reader.ReadInt16();
            }
            if (type == typeof(string))
            {
                return reader.ReadString();
            }
            if (type == typeof(uint))
            {
                return reader.ReadUInt32();
            }
            if (type == typeof(ulong))
            {
                return reader.ReadUInt64();
            }
            if (type == typeof(ushort))
            {
                return reader.ReadUInt16();
            }
            if (type == typeof(DateTime))
            {
                return new DateTime(reader.ReadInt64());
            }
            if (type == typeof(DateTimeOffset))
            {                
                return DateTimeOffset.FromFileTime(reader.ReadInt64());
            }
            if (type == typeof(TimeSpan))
            {
                return new TimeSpan(reader.ReadInt64());
            }
            if (type == typeof(Guid))
            {
                return new Guid(reader.ReadBytes(16));
            }
            if (type == typeof(Uri))
            {
                return new Uri(reader.ReadString());
            }

            throw new SerializerException(string.Format("Unknown simple type: {0}",
                                                                   type));
        }

        private static Dictionary<string, FieldInfo> GetFieldInfo(Type type)
        {
            lock (_fieldInfoDictionary)
            {
                if (_fieldInfoDictionary.ContainsKey(type))
                {
                    return _fieldInfoDictionary[type];
                }

                IEnumerable<FieldInfo> fieldsInfo = 
                    type.GetFields(BindingFlags.Instance |
                                   BindingFlags.Public |
                                   BindingFlags.FlattenHierarchy |
                                   BindingFlags.SetField);

                fieldsInfo = fieldsInfo.Where(f => !ShouldBeIgnored(type, f));

                Dictionary<string, FieldInfo> fields = fieldsInfo.ToDictionary(fieldInfo => fieldInfo.Name);

                _fieldInfoDictionary[type] = fields;
                return fields;
            }
        }

        private static bool ShouldBeIgnored(Type type, MemberInfo info)
        {
            if (IgnoreProperties.ContainsKey(type)
                && IgnoreProperties[type].Contains(info.Name))
            {
                return true;
            }

            string attributeString = GetAttributeString(info);

            if (attributeString.Contains("ForeignKeyAttribute")
                || attributeString.Contains("IgnoreColumnAttribute"))
            {
                return true;
            }

            return false;


        }

        private static string GetAttributeString(MemberInfo info)
        {
            string attributeString = String.Empty;

            if (info != null)
            {
                object[] attributes = info.GetCustomAttributes(true);
                if (attributes != null)
                {
                    foreach (Attribute attribute in attributes)
                    {
                        Type type = attribute.GetType();
                        attributeString += type.Name;
                    }
                }
            }
            return attributeString;
        }

        private static Dictionary<string, PropertyInfo> GetPropertyInfo(Type type)
        {
            lock (_propertyInfoDictionary)
            {
                if (_propertyInfoDictionary.ContainsKey(type))
                {
                    return _propertyInfoDictionary[type];
                }

                IEnumerable<PropertyInfo> propertiesInfo =
                    type.GetProperties(BindingFlags.Instance |
                                       BindingFlags.Public |
                                       BindingFlags.FlattenHierarchy);


                propertiesInfo = propertiesInfo.Where(o => o.GetSetMethod() != null
                                                           && !ShouldBeIgnored(type, o));

                Dictionary<string, PropertyInfo> properties = propertiesInfo.ToDictionary(propertyInfo => propertyInfo.Name);

                _propertyInfoDictionary[type] = properties;
                return properties;
            }
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            Dictionary<string, FieldInfo> fields = GetFieldInfo(type);
            return fields.ContainsKey(fieldName) ? fields[fieldName] : null;
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            Dictionary<string, PropertyInfo> properties = GetPropertyInfo(type);
            return properties.ContainsKey(propertyName) ? properties[propertyName] : null;
        }

        /// <summary>
        /// Change type function that handles nullables and enums
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object ChangeType(object value,
                                        Type valueType,
                                        Type conversionType)
        {
            if (IsNullableType(conversionType))
            {
                if (value == null)
                {
                    return null;
                }
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }

            if (IsNullableType(valueType))
            {
                Type underlyingType = Nullable.GetUnderlyingType(valueType);

                if (conversionType == typeof(string)
                    || (underlyingType == typeof(int) && conversionType.IsEnum))
                {
                    valueType = underlyingType;
                }
                else
                {
                    ThrowCastException(valueType, conversionType, value);
                }
            }

            if (valueType == typeof(char) && conversionType != typeof(string))
            {
                value = Convert.ToUInt16(value);
            }

            if (valueType.IsPrimitive &&
                conversionType == typeof(char) &&
                valueType != conversionType)
            {
                ThrowCastException(valueType, conversionType, value);
            }

            object result = null;
            try
            {
                if (valueType == typeof(int) && conversionType.IsEnum)
                {
                    result = Enum.ToObject(conversionType, value);
                }
                else
                {
                    result = Convert.ChangeType(value,
                                                conversionType,
                                                CultureInfo.CurrentCulture);
                }

                if (conversionType == typeof(float) &&
                    Single.IsInfinity((float)result))
                {
                    ThrowCastException(valueType, conversionType, value);
                }
            }
            catch
            {
                ThrowCastException(valueType, conversionType, value);
            }
            return result;
        }

        private static void ThrowCastException(Type sourceType, Type targetType, object value)
        {
            string valueString = string.Empty;
            try
            {
                valueString = value == null ? "(null)" : value.ToString();
            }
            catch (Exception)
            {
            }

            throw new InvalidCastException(string.Format("Cannot cast from {0} to {1}, value: {2}",
                                                                   sourceType,
                                                                   targetType,
                                                                   valueString));
        }

        private static Type SearchForTypeInAssemblies(Assembly callingAssembly, string typeName)
        {
            lock (_typesByName)
            {
                if (_typesByName.ContainsKey(typeName))
                    return _typesByName[typeName];


                Type type = callingAssembly.GetTypes().FirstOrDefault(o => o.Name == typeName);

                if (type != null)
                {
                    _typesByName.Add(typeName, type);
                    return type;
                }

#if !SILVERLIGHT
                //Try current loaded assemblies
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetTypes().FirstOrDefault(o => o.AssemblyQualifiedName == typeName);

                    if (type != null)
                    {
                        _typesByName.Add(typeName, type);
                        return type;
                    }
                }

                //Try referenced assemblies
                foreach (AssemblyName assemblyName in callingAssembly.GetReferencedAssemblies())
                {
                    Assembly assembly = Assembly.Load(assemblyName.ToString());

                    type = assembly.GetTypes().FirstOrDefault(o => o.Name == typeName);

                    if (type != null)
                    {
                        _typesByName.Add(typeName, type);
                        return type;
                    }
                }
#endif

                return null;
            }
        }

        private static object CreateCollectionInstance(Type type, int length)
        {
            ConstructorInfo constructor = 
                type.GetConstructor(new Type[] { typeof(int) });
            if (constructor == null)
            {
                constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    return constructor.Invoke(null);
                }
            }
            else
            {
                return constructor.Invoke(new object[] { length });
            }
            return null;
        }

        #endregion Methods
    }
}