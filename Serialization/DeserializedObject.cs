using System.Reflection;

namespace KellermanSoftware.Serialization
{
    internal sealed class DeserializedObject
    {
        public DeserializedObject(object value,
                                  int parentIndex,
                                  FieldInfo fieldInfo,
                                  PropertyInfo propertyInfo,
                                  int? indexInCollection)
        {
            Value = value;
            ParentIndex = parentIndex;
            FieldInfo = fieldInfo;
            PropertyInfo = propertyInfo;
            IndexInCollection = indexInCollection;
        }

        public object Value { get; set; }
        public int ParentIndex { get; private set; }
        public FieldInfo FieldInfo { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public int? IndexInCollection { get; private set; }
    }
}