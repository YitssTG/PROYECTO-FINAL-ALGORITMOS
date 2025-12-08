using System.Collections.Generic;
using UnityEngine;

public class SimpleLinkedList<T>
{
    public NodeInventory<T> Head = null;
    public NodeInventory<T> Tail = null;

    public void DebugList()
    {
        NodeInventory<T> current = Head;

        Debug.Log("=== RECORRIENDO LISTA ENLAZADA ===");

        while (current != null)
        {
            Debug.Log("Nodo: " + current.Value);
            current = current.Next;
        }
    }
    public virtual void AddNode(NodeInventory<T> node)
    {
        if (Head == null && Tail == null)
        {
            Head = node;
            Tail = node;
            return;
        }

        Tail.SetNext(node);
        Tail = node;
    }

    public virtual void RemoveHead()
    {
        if (Head == null) return;
        NodeInventory<T> temp = Head;
        Head = Head.Next;
        temp.SetNext(null);
    }

    public virtual void Clear()
    {
        Head = null;
        Tail = null;
    }

    public virtual void ReadAllNodes()
    {
        NodeInventory<T> current = Head;
        while (current != null)
        {
            Debug.Log(current.Value);
            current = current.Next;
        }
    }

    public bool RemoveByValue(T value)
    {
        if (Head == null) return false;

        NodeInventory<T> current = Head;
        NodeInventory<T> previous = null;

        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, value))
            {
                if (previous == null)
                {
                    Head = current.Next;
                }
                else
                {
                    previous.SetNext(current.Next);
                }

                if (current == Tail)
                {
                    Tail = previous;
                }

                return true;
            }

            previous = current;
            current = current.Next;
        }

        return false;
    }

}
