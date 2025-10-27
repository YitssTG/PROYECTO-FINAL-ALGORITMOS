using System.Collections.Generic;

public class CustomLinkedList<T>
{
    private ListNode<T> head;
    private int count;

    public int Count => count;

    public void Add(T value)
    {
        ListNode<T> newNode = new ListNode<T>(value);

        if (head == null)
        {
            head = newNode;
        }
        else
        {
            ListNode<T> temp = head;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }
            temp.SetNext(newNode);
        }

        count++;
    }

    public bool Remove(T value)
    {
        if (head == null) return false;

        if (EqualityComparer<T>.Default.Equals(head.Value, value))
        {
            head = head.Next;
            count--;
            return true;
        }

        ListNode<T> prev = head;
        ListNode<T> curr = head.Next;

        while (curr != null)
        {
            if (EqualityComparer<T>.Default.Equals(curr.Value, value))
            {
                prev.SetNext(curr.Next);
                count--;
                return true;
            }

            prev = curr;
            curr = curr.Next;
        }

        return false;
    }

    public IEnumerable<T> GetAll()
    {
        ListNode<T> temp = head;
        while (temp != null)
        {
            yield return temp.Value;
            temp = temp.Next;
        }
    }

    public void Clear()
    {
        head = null;
        count = 0;
    }
}
