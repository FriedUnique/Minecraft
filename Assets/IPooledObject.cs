public interface IPooledObject
{
    void OnPooled(object[] args);
    void AddToList();
}
