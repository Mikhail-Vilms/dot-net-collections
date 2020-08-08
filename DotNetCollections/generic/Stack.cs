using System;

namespace DotNetCollections.generic
{
    public class Stack<T> : IReadOnlyCollection<T>, ICollection
    {
        #region Fields

        private const int _defaultCapacity = 4;

        private T[] _array;     // Storage for stack elements
        private int _size;      // Number of items in the stack

        private Object _syncRoot; // #TODO ???

        #endregion Fields

        #region Constructors

        public Stack()
        {
            _array = new T[0];
            _size = 0;
        }

        // Create a stack with a specific initial capacity
        public Stack(int capacity)
        {
            if (capacity < 0)
            {
                throw new Exception("Capacity value has to be a non-negative integer");
            }

            _array = new T[capacity];
            _size = 0;
        }

        #endregion Constructors
        
        #region Properties

        public int Count
        {
            get { return _size; }
        }

        bool DotNetCollections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        // #TODO ???
        object DotNetCollections.ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }

                return _syncRoot;
            }
        }

        #endregion Properties

        #region Methods

        // Removes all Objects from the Stack.
        public void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
        }

        public bool Contains(T item)
        {
            int count = _size;
            while (count-- > 0)
            {
                if (item == null && _array[count] == null)
                {
                    return true;
                }

                if (item.Equals(_array[count]))
                {
                    return true;
                }
            }

            return false;
        }

        // Returns the top object on the stack without removing it.
        public T Peek()
        {
            if (_size == 0)
            {
                throw new Exception("Stack is empty");
            }

            return _array[_size - 1];
        }

        // Pops an item from the top of the stack.
        public T Pop()
        {
            if (_size == 0)
            {
                throw new Exception("Stack is empty");
            }

            T item = _array[--_size];
            _array[_size] = default;

            return item;
        }

        // Pushes an item to the top of the stack.
        public void Push(T item)
        {
            // resize if there is no more space
            if (_size == _array.Length)
            {
                T[] newArray = new T[_array.Length == 0 ? _defaultCapacity : 2 * _array.Length];
                Array.Copy(_array, 0, newArray, 0, _size);
                _array = newArray;
            }


            _array[_size++] = item;
        }

        // Copies the Stack to an array, in the same order Pop would return the items.
        public T[] ToArray()
        {
            T[] objArray = new T[_size];

            int i = 0;
            while (i < _size)
            {
                objArray[i] = _array[_size - i - 1];
                i++;
            }

            return objArray;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public Enumerator GetEnumarator()
        {
            return new Enumerator(this);
        }

        generic.IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion Methods

        #region Inner classes

        public struct Enumerator : IEnumerator<T>, DotNetCollections.IEnumerator
        {
            private Stack<T> _stack;
            private int _index;
            private T currentElement;

            internal Enumerator(Stack<T> stack)
            {
                _stack = stack;
                _index = -2;
                currentElement = default;
            }

            public void Dispose()
            {
                _index = -1;
            }

            public bool MoveNext()
            {
                bool retval;

                if (_index == -2) // First call to enumerator.
                {
                    _index = _stack._size - 1;
                    retval = _index >= 0;
                    if (retval)
                    {
                        currentElement = _stack._array[_index];
                    }

                    return retval;
                }

                if (_index == -1) // end of enumeration
                {
                    return false;
                }

                retval = --_index >= 0;
                if (retval)
                {
                    currentElement = _stack._array[_index];
                }
                else
                {
                    currentElement = default;
                }

                return retval;
            }

            public T Current
            {
                get
                {
                    if (_index == -2)
                    {
                        throw new Exception("Enumeration has not started");
                    }

                    if (_index == -2)
                    {
                        throw new Exception("Enumeration has not started");
                    }

                    return currentElement;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_index == -2)
                    {
                        throw new Exception("Enumeration has not started");
                    }

                    if (_index == -1)
                    {
                        throw new Exception("Enumeration has not started");
                    }

                    return currentElement;
                }
            }

            public void Reset()
            {
                _index = -2;
                currentElement = default;
            }
        }

        #endregion Inner classes
    }
}
