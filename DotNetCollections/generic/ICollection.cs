namespace DotNetCollections.generic
{
    public interface ICollection<T> : IEnumerable<T>
    {
        int Count { 
            get; 
        }

        bool IsReadOnly
        {
            get;
        }

        void Add(T item);

        void Clear();

        bool Contains(T item);

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
        void CopyTo(T[] array, int arrayIndex);

        bool Remove(T item);
    }
}
