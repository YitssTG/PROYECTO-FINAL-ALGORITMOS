using System.Collections.Generic;

public class Queue<T>
{
    private LinkedList<T> list = new LinkedList<T>();

    public int Count => list.Count;

    public void Enqueue(T item)
    {
        list.AddLast(item);
    }

    public T Dequeue()
    {
        if (list.Count == 0) return default;
        T value = list.First.Value;
        list.RemoveFirst();
        return value;
    }

    public T Peek()
    {
        return list.Count > 0 ? list.First.Value : default;
    }

    public void Clear()
    {
        list.Clear();
    }
}
