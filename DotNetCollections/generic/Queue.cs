using System;

namespace DotNetCollections.generic
{

    public class Queue<T> : IReadOnlyCollection<T>, ICollection
    {
        #region Fields

        private T[] _array;
        private int _head;
        private int _tail;
        private int _size;

        #endregion Fields

        #region Constructors

        public Queue() : this(5)
        {
            _array = new T[5];
        }

        public Queue(int capacity)
        {
            if (capacity <= 0)
            {
                throw new Exception("Capacity has to be non-negative integer");
            }

            _array = new T[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        #endregion Constructors

        #region Properties
        
        public int Count
        {
            get { return _size; }
        }
        
        bool ICollection.IsSynchronized
        {
            get { return false;}
        }

        public object SyncRoot => throw new NotImplementedException();

        #endregion Properties

        #region Private Methods

        // PRIVATE Grows or shrinks the buffer to hold capacity objects.
        // Capacity must be >= _size.
        private void SetCapacity(int capacity)
        {
            T[] newArray = new T[capacity];

            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newArray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newArray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newArray, _array.Length - _head, _tail);
                }
            }

            _array = newArray;
            _head = 0;
            _tail = _size == capacity ? 0 : _size;
        }

        #endregion Private Methods

        #region Public Methods

        // Removes all Objects from the queue.
        public void Clear()
        {
            if (_head < _tail)
            {
                Array.Clear(_array, _head, _size);
            }
            else
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _head = 0;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        // Adds item to the tail of the queue.
        public void Enqueue(T item)
        {
            if (_size == _array.Length)
            {
                int newCapacity = _array.Length * 2;
                SetCapacity(newCapacity);
            }

            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length;
            _size++;
        }

        // Removes the object at the head of the queue and returns it. 
        // If the queue is empty, this method simply returns null.
        public T Dequeue()
        {
            if (_size == 0)
            {
                throw new Exception("Queue is empty");
            }

            T removed = _array[_head];
            _array[_head] = default;
            _head = (_head + 1) % _array.Length;
            _size--;

            return removed;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        // Returns the object at the head of the queue. 
        // The object remains in the queue.
        public T Peek()
        {   
            if (_size == 0)
            {
                throw new Exception("Queue is empty");
            }

            return _array[_head];
        }

        // Returns true if the queue contains at least one object equal to item.
        // Equality is determined using item.Equals().
        public bool Contains(T item)
        {
            int index = _head;
            int count = _size;

            while(count > 0)
            {
                if (item == null && _array[index] == null)
                {
                    return true;
                }
                else if (_array[index] != null && count.Equals(_array[index]))
                {
                    return true;
                }
                count--;
            }

            return false;
        }

        #endregion Public Methods

        #region Nested Classes
        
        public struct Enumerator : IEnumerator<T>
        {
            private Queue<T> _q;
            private int _index; // -1 = not started, -2 = ended/disposed
            private T _currentElement;

            internal Enumerator(Queue<T> q)
            {
                _q = q;
                _index = -1;
                _currentElement = default;
            }

            public bool MoveNext()
            {
                if(_index == -2)
                {
                    return false;
                }

                _index++;

                if (_index == _q._size)
                {
                    _index = -2;
                    _currentElement = default;
                    return false;
                }

                _currentElement = _q._array[(_q._head + _index) % _q._array.Length];

                return true;
            }
            public T Current
            {
                get
                {
                    if (_index < 0)
                    {
                        if (_index == -1)
                        {
                            throw new Exception("Invalid Operation: Enum not started");
                        }
                        else // _index == -2
                        {
                            throw new Exception("Invalid Operation: Enum ended");
                        }
                    }
                    return _currentElement;
                } 
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0)
                    {
                        if (_index == -1)
                        {
                            throw new Exception("Invalid Operation: Enum not started");
                        }
                        else // _index == -2
                        {
                            throw new Exception("Invalid Operation: Enum ended");
                        }
                    }
                    return _currentElement;
                }
            }

            void IEnumerator.Reset()
            {
                _index = -1;
                _currentElement = default;
            }
        }

        #endregion Nested Classes
    }
}