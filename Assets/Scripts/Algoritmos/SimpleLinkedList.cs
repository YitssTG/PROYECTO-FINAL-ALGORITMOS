//using UnityEngine;

//public class SimpleLinkedList<T>
//{
//    public Node<T> Head = null;
//    public Node<T> Tail = null;

//    // Añadir nodo al final
//    public virtual void AddNode(Node<T> node)
//    {
//        if (Head == null && Tail == null)
//        {
//            Head = node;
//            Tail = node;
//            return;
//        }

//        Tail.SetNext(node);
//        Tail = node;
//    }

//    // Eliminar nodo inicial
//    public virtual void RemoveHead()
//    {
//        if (Head == null) return;
//        Node<T> temp = Head;
//        Head = Head.Next;
//        temp.SetNext(null);
//    }

//    // Limpiar lista
//    public virtual void Clear()
//    {
//        Head = null;
//        Tail = null;
//    }

//    // Leer todos los nodos (debug)
//    public virtual void ReadAllNodes()
//    {
//        Node<T> current = Head;
//        while (current != null)
//        {
//            Debug.Log(current.Value);
//            current = current.Next;
//        }
//    }
//}
