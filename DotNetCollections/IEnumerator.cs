namespace DotNetCollections
{
    /* 
     * https://referencesource.microsoft.com/#mscorlib/system/collections/ienumerator.cs
     * https://github.com/microsoft/referencesource/blob/master/mscorlib/system/collections/ienumerator.cs
     */
    public interface IEnumerator
    {
        bool MoveNext();

        object Current
        {
            get;
        }

        void Reset();
    }
}
