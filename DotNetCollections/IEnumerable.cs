namespace DotNetCollections
{
    /* 
     * https://referencesource.microsoft.com/#mscorlib/system/collections/ienumerable.cs
     * https://github.com/microsoft/referencesource/blob/master/mscorlib/system/collections/generic/ienumerator.cs
     */
    public interface IEnumerable
    {
        IEnumerator GetEnumerator();
    }
}
