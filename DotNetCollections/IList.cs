using System;

namespace DotNetCollections
{
    public interface IList : ICollection
    {
        object this[int index]
        {
            get;
            set;
        }

        bool IsReadOnly
        { 
            get; 
        }

        bool IsFixedSize
        {
            get;
        }

        int Add(object value);

        bool Contains(object value);

        void Clear();

        int IndexOf(Object value);

        void Insert(int index, Object value);

        void Remove(Object value);

        void RemoveAt(int index);
    }
}
