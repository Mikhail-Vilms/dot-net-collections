using System;

namespace DotNetCollections.generic
{
    public class List<T> : IList, IList<T>, IReadOnlyList<T>
    {
        #region Fields

        private const int _defaultCapacity = 4;

        private T[] _items;

        private int _size;

        private object _syncRoot;

        #endregion Fields 

        #region Properties

        // Sets or Gets the element at the given index.
        public T this[int index]
        {
            get
            {
                if (index >= _size)
                {
                    throw new Exception("Index is out of range");
                }
                return _items[index];
            }

            set
            {
                if (index >= _size)
                {
                    throw new Exception("Index is out of range");
                }
                _items[index] = value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new Exception("Wrong value type; correct type: " + typeof(T));
                }
            }
        }

        public int Count
        {
            get
            {
                return _size;
            }
        }

        #endregion Properties


        #region Constructors

        public List()
        {
            _items = new T[0];
        }

        public List(int capacity)
        {
            if (capacity < 0)
            {
                throw new Exception("Capacity has to be non-negative integer value");
            }

            _items = new T[capacity];
        }

        // Is this List read-only?
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        #endregion Constructors

        #region Public Methods

        public void Add(T item)
        {
            if (_size == _items.Length)
            {
                DoubleCapacity();
            }

            _items[_size++] = item;
        }

        // Inserts an element into this list at a given index. The size of the list
        // is increased by one. If required, the capacity of the list is doubled
        // before inserting the new element.
        public void Insert(int index, T item)
        {
            if (index > _size)
            {
                throw new Exception("Can't insert new element: index is out of range");
            }

            if (_size == _items.Length)
            {
                DoubleCapacity();
            }

            Array.Copy(_items, index, _items, index + 1, _size - index);
            _items[index] = item;
            _size++;
        }

        public void Clear()
        {
            if (_size == 0)
            {
                return;
            }
            Array.Clear(_items, 0, _size);
        }

        // Contains returns true if the specified element is in the List.
        // It does a linear, O(n) search.
        public bool Contains(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < _size; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _size; i++)
                {
                    if (_items[i].Equals(item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Returns the index of the first occurrence of a given value in a range of this list.
        // The list is searched forwards from beginning to end.
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }

        // Returns the index of the first occurrence of a given value in a range of this list.
        // The list is searched forwards, starting at index index and ending at count number of elements.
        public int IndexOf(T item, int index)
        {
            if (index > _size)
            {
                throw new Exception("Index is out of range");
            }

            return Array.IndexOf(_items, item, index, _size - index);
        }

        // Returns the index of the first occurrence of a given value in a range of this list. 
        // The list is searched forwards, starting at index index and upto count number of elements.
        public int IndexOf(T item, int index, int count)
        {
            if (index > _size)
            {
                throw new Exception("Index is out of range");
            }

            if (count < 0 || index > _size - count)
            {
                throw new Exception("Count parameter has to be non-integer");
            }

            return Array.IndexOf(_items, item, index, count);
        }

        // Removes the element at the given index. The size of the list is decreased by one.
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index > _size)
            {
                throw new Exception("Index is out of range");
            }

            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            
            _items[_size] = default;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        private void DoubleCapacity()
        {
            int newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;
            T[] newItems = new T[newCapacity];
            Array.Copy(_items, 0, newItems, 0, _size);
            _items = newItems;
        }

        #endregion Private Methods

        #region Uniplemented Methods

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }
        
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        #endregion Uniplemented Methods

        public bool IsFixedSize => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        #region Embedded Classes

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private List<T> list;
            private int index;
            private T current;

            public Enumerator(List<T> list)
            {
                this.list = list;
                index = 0;
                current = default;
            }

            public T Current
            {
                get 
                { 
                    return current; 
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == list._size + 1)
                    {
                        throw new Exception("Enum operation can't happen");
                    }

                    return current;
                }
            }

            public void Reset()
            {
                index = 0;
                current = default;
            }

            public bool MoveNext()
            {
                List<T> localList = list;

                throw new NotImplementedException();
            }


        }

        #endregion Embedded Classes 
    }
}
