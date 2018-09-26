using System;

namespace KellermanSoftware.Serialization
{
    internal sealed class SerializeItem
    {
        public SerializeItem(object item, Type type)
        {
            Item = item;
            if (type == typeof(object) && item != null)
            {
                Type = item.GetType();
            }
            else if (item == null)
            {
                Type = type;
            }
            else
            {
                Type = type.IsInterface ? item.GetType() : type;
            }
        }

        public object Item { get; private set; }
        public Type Type { get; private set; }
    }
}