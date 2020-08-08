using System;

namespace DotNetCollections.generic
{
    /*
     * Implementation notes:
     * 
     * This uses an array-based implementation similar to Dictionary<T>, using a buckets array
     * to map hash values to the Slots array. Items in the Slots array that hash to the same value
     * are chained together through the "next" indices. 
     * 
     * The capacity is always prime; so during resizing, the capacity is chosen as the next prime
     * greater than double the last capacity. 
     * 
     * The underlying data structures are lazily initialized. Because of the observation that, 
     * in practice, hashtables tend to contain only a few elements, the initial capacity is
     * set very small (3 elements) unless the ctor with a collection is used.
     * 
     * The +/- 1 modifications in methods that add, check for containment, etc allow us to 
     * distinguish a hash code of 0 from an uninitialized bucket. This saves us from having to 
     * reset each bucket to -1 when resizing. See Contains, for example.
     * 
     * Set methods such as UnionWith, IntersectWith, ExceptWith, and SymmetricExceptWith modify
     * this set.
     * 
     * Some operations can perform faster if we can assume "other" contains unique elements
     * according to this equality comparer. The only times this is efficient to check is if
     * other is a hashset. Note that checking that it's a hashset alone doesn't suffice; we
     * also have to check that the hashset is using the same equality comparer. If other 
     * has a different equality comparer, it will have unique elements according to its own
     * equality comparer, but not necessarily according to ours. Therefore, to go these 
     * optimized routes we check that other is a hashset using the same equality comparer.
     * 
     * A HashSet with no elements has the properties of the empty set. (See IsSubset, etc. for 
     * special empty set checks.)
     * 
     * A couple of methods have a special case if other is this (e.g. SymmetricExceptWith). 
     * If we didn't have these checks, we could be iterating over the set and modifying at
     * the same time.
     */
    public class HashSet<T> : ISet<T>
    {
        #region Fields

        private int[] m_buckets;
        private Slot[] m_slots;
        private int m_count;
        private int m_lastIndex;
        private int m_freeList;

        #endregion Fields

        #region Constructors

        public HashSet()
        {
            m_lastIndex = 0;
            m_count = 0;
            m_freeList = -1;
        }

        public HashSet(int capacity) : this()
        {
            if (capacity < 0)
            {
                throw new System.Exception("Cpacity value has to be non-negative integer");
            }

            m_buckets = new int[capacity];
            m_slots = new Slot[capacity];
        }

        #endregion Constructors

        #region Properties

        // Number of elements in this hashset
        public int Count
        {
            get { return m_count; }
        }

        // Whether this is readonly
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }
        
        #endregion Properties

        #region ICollection<T> methods

        // Add item to this hashset.
        // This is the explicit implementation of the ICollection<T> interface.
        // The other Add method returns bool indicating whether item was added.
        void ICollection<T>.Add(T item)
        {
            AddIfNotPresent(item);
        }

        // Remove all items from this set.
        // This clears the elements but not the underlying buckets and slots array.
        public void Clear()
        {
            if (m_lastIndex == 0){
                return;
            }

            // Clear the elements so that the gc can reclaim the references.
            // Clear only up to m_lastIndex for m_slots 
            Array.Clear(m_slots, 0, m_lastIndex);
            Array.Clear(m_buckets, 0, m_buckets.Length);

            m_lastIndex = 0;
            m_count = 0;
            m_freeList = -1;
        }

        // Checks if this hashset contains the item
        public bool Contains(T item)
        {
            if (m_buckets == null)
            {
                return false;
            }

            int hashCode = item.GetHashCode();
            int bucketIndx = hashCode % m_buckets.Length;

            // See note at "HashSet" level describing why "- 1" appears in for loop
            for (int i = m_buckets[bucketIndx] - 1; i >= 0; i = m_slots[i].next)
            {
                if (m_slots[i].hashCode == hashCode && m_slots[i].value.Equals(item))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Remove(T item)
        {
            if (m_buckets == null)
            {
                return false;
            }

            int hashCode = item.GetHashCode();
            int bucketIndx = hashCode % m_buckets.Length;
            int last = -1;

            for (int i = m_buckets[bucketIndx] - 1; i >= 0; last = i, i = m_slots[i].next)
            {
                if (m_slots[i].hashCode == hashCode && item.Equals(m_slots[i].value))
                {
                    if (last < 0)
                    {
                        // first iteration; update buckets
                        m_buckets[bucketIndx] = m_slots[i].next + 1;
                    }
                    else
                    {
                        // subsequent iterations; update 'next' pointers
                        m_slots[last].next = m_slots[i].next;
                    }

                    m_slots[i].hashCode = -1;
                    m_slots[i].value = default;
                    m_slots[i].next = m_freeList;

                    m_count--;

                    if (m_count == 0)
                    {
                        m_lastIndex = 0;
                        m_freeList = -1;
                    }
                    else
                    {
                        m_freeList = i;
                    }

                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        #endregion ICollection<T> methods

        #region IEnumerable methods

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion IEnumerable Methods

        #region HashSet Methods

        public bool Add(T item)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        #endregion HashSet Methods

        #region Helper Methods

        /// <summary>
        /// Initializes buckets and slots arrays. Uses suggested capacity by finding next prime
        /// greater than or equal to capacity.
        /// </summary>
        /// <param name="capacity"></param>
        private void Initialize(int capacity)
        {
            m_buckets = new int[capacity];
            m_slots = new Slot[capacity];
        }

        /// <summary> Expand to new capacity. </summary>
        private void IncreaseCapacity(bool forceNewHashCodes)
        {
            int newSize = m_buckets.Length * 2 + 1;

            Slot[] newSlots = new Slot[newSize];
            
            if (m_slots != null)
            {
                Array.Copy(m_slots, 0, newSlots, 0, m_lastIndex);
            }

            if (forceNewHashCodes)
            {
                for(int i = 0; i < m_lastIndex; i++)
                {
                    if(newSlots[i].hashCode != -1)
                    {
                        newSlots[i].hashCode = newSlots[i].value.GetHashCode();
                    }
                }
            }

            int[] newBuckets = new int[newSize];

            for(int i = 0; i < m_lastIndex; i++)
            {
                int bucket = newSlots[i].hashCode % newSize;
                newSlots[i].next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }

            m_slots = newSlots;
            m_buckets = newBuckets;
        }

        /// <summary>
        ///     Adds value to HashSet if not contained already
        ///     Returns true if added and false if already present
        /// </summary>
        /// <param name="value">value to find</param>
        /// <returns></returns>
        private bool AddIfNotPresent(T value)
        {
            if (m_buckets == null)
            {
                Initialize(0);
            }

            int hashCode = value.GetHashCode();
            int bucket = hashCode % m_buckets.Length;
            
            for(int i = m_buckets[hashCode % m_buckets.Length] - 1; i >=0; i = m_slots[i].next)
            {
                if (m_slots[i].hashCode == hashCode && value.Equals(m_slots[i].value))
                {
                    return false;
                }
            }

            int index;

            if (m_freeList >= 0)
            {
                index = m_freeList;
                m_freeList = m_slots[index].next;
            }
            else
            {
                if (m_lastIndex == m_slots.Length)
                {
                    IncreaseCapacity(false);
                    // recalculate bucket
                    bucket = hashCode % m_buckets.Length;
                }
                
                index = m_lastIndex;
                m_lastIndex++;
            }

            m_slots[index].hashCode = hashCode;
            m_slots[index].value = value;
            m_slots[index].next = m_buckets[bucket] - 1;
            m_buckets[bucket] = index + 1;
            m_count++;

            return true;
        }

        #endregion Helper Methods

        #region Internal Classes and Structs

        internal struct Slot
        {
            internal int hashCode;      // Lower 31 bits of hash code, -1 if unused
            internal int next;          // Index of next entry, -1 if last
            internal T value;
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private HashSet<T> set;
            private int index;
            private T current;

            internal Enumerator(HashSet<T> set)
            {
                this.set = set;
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

            public bool MoveNext()
            {
                while(index < set.m_lastIndex)
                {
                    if (set.m_slots[index].hashCode < 0)
                    {
                        index++;
                        continue;
                    }

                    current = set.m_slots[index].value;
                    index++;
                    return true;
                }

                index = set.m_lastIndex + 1;
                current = default;
                return false;
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == set.m_lastIndex + 1)
                    {
                        throw new Exception("Enum operation can't happen");
                    }

                    return Current;
                }   
            }

            void IEnumerator.Reset()
            {
                index = 0;
                current = default;
            }
        }

        #endregion Internal Classes and Structs
    }
}
