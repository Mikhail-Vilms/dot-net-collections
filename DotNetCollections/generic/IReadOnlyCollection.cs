namespace DotNetCollections.generic
{
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count { 
            get; 
        }
    }
}
