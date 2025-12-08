public class NodeInventory<T>
{
    #region properties

    private T value;
    private NodeInventory<T> next;

    #endregion

    public NodeInventory(T value)
    {
        this.value = value;
        next = null;
    }

    #region Setters

    public void SetNext(NodeInventory<T> node)
    {
        next = node;
    }

    #endregion

    #region Getters

    public T Value => value;
    public NodeInventory<T> Next => next;

    #endregion
}
