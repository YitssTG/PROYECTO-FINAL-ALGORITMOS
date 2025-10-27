using System.Collections.Generic;
using UnityEngine;

public class OrientedGraph<T>
{
    private List<Node<T>> nodes = new List<Node<T>>();

    public Node<T> AddNode(T value)
    {
        Node<T> node = new Node<T>(value);
        nodes.Add(node);
        return node;
    }

    public void AddEdge(Node<T> from, Node<T> to)
    {
        if (from == null || to == null) return;
        from.Connect(to);
    }

    public void RemoveEdge(Node<T> from, Node<T> to)
    {
        if (from == null || to == null) return;
        from.Disconnect(to);
    }

    public void PrintAdjacencyList()
    {
        Debug.Log("📋 LISTA DE ADYACENCIA:");
        foreach (var node in nodes)
        {
            string neighbors = "";
            foreach (var n in node.Neighbors)
                neighbors += n.Value.ToString() + " ";
            Debug.Log($"{node.Value}: {neighbors}");
        }
    }

    public void PrintAdjacencyMatrix()
    {
        Debug.Log("🧮 MATRIZ DE ADYACENCIA:");
        for (int i = 0; i < nodes.Count; i++)
        {
            string row = "";
            for (int j = 0; j < nodes.Count; j++)
            {
                row += nodes[i].Neighbors.Contains(nodes[j]) ? "1 " : "0 ";
            }
            Debug.Log(row);
        }
    }

    public List<Node<T>> BFS(Node<T> startNode)
    {
        List<Node<T>> visited = new List<Node<T>>();
        Queue<Node<T>> queue = new Queue<Node<T>>();

        if (startNode == null || !nodes.Contains(startNode)) return visited;

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Node<T> current = queue.Dequeue();
            foreach (Node<T> neighbor in current.Neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        return visited;
    }

    public List<Node<T>> DFS(Node<T> startNode)
    {
        List<Node<T>> visited = new List<Node<T>>();
        DFSRecursive(startNode, visited);
        return visited;
    }

    private void DFSRecursive(Node<T> node, List<Node<T>> visited)
    {
        if (node == null || visited.Contains(node)) return;
        visited.Add(node);

        foreach (var neighbor in node.Neighbors)
            DFSRecursive(neighbor, visited);
    }

    public int Count => nodes.Count;
    public List<Node<T>> Nodes => nodes;
}
