//using UnityEngine;

//public class PriorityNode<T> : Node<T>
//{
//    public int Priority;

//    public PriorityNode(T value, int priority = 0) : base(value)
//    {
//        Priority = priority;
//    }

//    public PriorityNode<T> Next { get; set; }
//}

//public class PriorityQueue<T>
//{
//    private PriorityNode<T> head;
//    private PriorityNode<T> tail;
//    private int count;

//    public int Count => count;

//    public void Enqueue(T value, int priority = 0)
//    {
//        PriorityNode<T> newNode = new PriorityNode<T>(value, priority);

//        if (head == null)
//        {
//            head = newNode;
//            tail = newNode;
//        }
//        else if (priority > head.Priority)
//        {
//            newNode.Next = head;
//            head = newNode;
//        }
//        else
//        {
//            PriorityNode<T> current = head;
//            while (current.Next != null && current.Next.Priority >= priority)
//                current = current.Next;

//            newNode.Next = current.Next;
//            current.Next = newNode;

//            if (newNode.Next == null) tail = newNode;
//        }

//        count++;
//    }

//    public T Dequeue()
//    {
//        if (head == null) return default;

//        T value = head.Value;
//        head = head.Next;
//        count--;
//        if (head == null) tail = null;
//        return value;
//    }

//    public T Peek()
//    {
//        return head != null ? head.Value : default;
//    }
//}
