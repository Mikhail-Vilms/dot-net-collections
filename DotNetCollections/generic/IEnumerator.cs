namespace DotNetCollections.generic
{
    public interface IEnumerator<out T> : IEnumerator
    {
        new T Current
        {
            get;
        }
    }
}
