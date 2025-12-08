using System.Collections.Generic;

public class Node<T>
{
    public T Value { get; private set; }
    public List<Node<T>> Neighbors { get; private set; } = new List<Node<T>>();

    public Node(T value)
    {
        Value = value;
    }

    public void Connect(Node<T> node)
    {
        if (!Neighbors.Contains(node))
            Neighbors.Add(node);
    }

    public void Disconnect(Node<T> node)
    {
        if (Neighbors.Contains(node))
            Neighbors.Remove(node);
    }
}