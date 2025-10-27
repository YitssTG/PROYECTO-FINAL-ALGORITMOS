public class ListNode<T>
{
    public T Value { get; private set; }
    public ListNode<T> Next { get; private set; }

    public ListNode(T value)
    {
        Value = value;
    }

    public void SetNext(ListNode<T> nextNode)
    {
        Next = nextNode;
    }
}
