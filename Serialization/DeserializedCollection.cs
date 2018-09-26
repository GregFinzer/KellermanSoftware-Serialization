#region Includes

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace KellermanSoftware.Serialization
{
    internal sealed class DeserializedCollection
    {
        #region Class Variables
        private readonly List<CollectionItem> _collectionItems;
        private readonly object _collection;

        private sealed class CollectionItem
        {
            public CollectionItem(int index, object item)
            {
                Index = index;
                Item = item;
            }

            public int Index { get; private set; }
            public object Item { get; private set; }
        }
        #endregion

        #region Constructor
        public DeserializedCollection(CollectionType type, object collection)
        {
            Type = type;
            _collection = collection;
            _collectionItems = new List<CollectionItem>();
        }
        #endregion

        #region Properties
        public CollectionType Type { get; private set; }

        public object Collection
        {
            get
            {
                IOrderedEnumerable<CollectionItem> orderedItems =
                    _collectionItems.OrderBy(x => x.Index);
                switch (Type)
                {
                    case CollectionType.Array:
                        Array array = (Array)_collection;
                        int arrayRank = array.Rank;
                        int[] dimensionsLengths = new int[arrayRank];
                        for (int i = 0; i < arrayRank; ++i)
                        {
                            dimensionsLengths[i] = array.GetLength(i);
                        }
                        int[] indices = new int[arrayRank];
                        int itemIndex = 0;
                        foreach (CollectionItem collectionItem in orderedItems)
                        {
                            CalculateIndices(itemIndex++, indices, dimensionsLengths);
                            array.SetValue(collectionItem.Item, indices);
                        }
                        break;
                    case CollectionType.List:
                        foreach (CollectionItem collectionItem in orderedItems)
                        {
                            ((IList)_collection).Add(collectionItem.Item);
                        }
                        break;
                    case CollectionType.Dictionary:
                        bool isValue = false;
                        object key = null;
                        ((IDictionary)_collection).Clear();
                        foreach (CollectionItem collectionItem in orderedItems)
                        {
                            if (isValue)
                            {
                                ((IDictionary)_collection).Add(key,
                                                               collectionItem.Item);
                            }
                            else
                            {
                                key = collectionItem.Item;
                            }
                            isValue = !isValue;
                        }
                        break;
                    default:
                        throw new SerializerException(string.Format("Unsupported collection type: {0}",
                                                                               Type));
                }
                return _collection;
            }
        }
        #endregion

        #region Methods

        public void AddItem(int index, object item)
        {
            _collectionItems.Add(new CollectionItem(index, item));
        }



        private static void CalculateIndices(int index,
                                             int[] indices,
                                             int[] dimensionsLengths)
        {
            int rank = indices.Length;
            bool continueCalculation = true;
            int remainder = index;
            for (int i = 0; i < rank; ++i)
            {
                if (continueCalculation)
                {
                    int capacity = CalculateDimensionCapacity(i, dimensionsLengths);
                    indices[i] = remainder / capacity;
                    remainder %= capacity;
                    if (remainder == 0)
                    {
                        continueCalculation = false;
                    }
                }
                else
                {
                    indices[i] = 0;
                }
            }
        }

        private static int CalculateDimensionCapacity(int dimension, int[] dimensionsLengths)
        {
            int capacity = 1;
            for (int i = dimension + 1; i < dimensionsLengths.Length; ++i)
            {
                capacity *= dimensionsLengths[i];
            }
            return capacity;
        }
        #endregion
    }
}