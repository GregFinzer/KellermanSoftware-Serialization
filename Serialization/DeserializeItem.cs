using System;
using System.Reflection;

namespace KellermanSoftware.Serialization
{
    internal sealed class DeserializeItem
    {
        public DeserializeItem(int parentIndex,
                               Type type,
                               FieldInfo fieldInfo,
                               PropertyInfo propertyInfo,
                               int? indexInCollection)
        {
            ParentIndex = parentIndex;
            Type = type;
            FieldInfo = fieldInfo;
            PropertyInfo = propertyInfo;
            IndexInCollection = indexInCollection;
        }

        public int ParentIndex { get; private set; }
        public Type Type { get; set; }
        public FieldInfo FieldInfo { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public int? IndexInCollection { get; private set; }
    }
}