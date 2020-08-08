using System;

namespace DotNetCollections
{
    /* 
     * https://referencesource.microsoft.com/#mscorlib/system/collections/icollection.cs
     * https://github.com/microsoft/referencesource/blob/master/mscorlib/system/collections/icollection.cs
     */
    public interface ICollection : IEnumerable
    {
        void CopyTo(Array array, int index);

        int Count
        { 
            get; 
        }

        object SyncRoot
        { 
            get; 
        }

        bool IsSynchronized
        { 
            get; 
        }
    }
}
