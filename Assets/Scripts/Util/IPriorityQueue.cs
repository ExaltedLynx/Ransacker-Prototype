using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public interface IPriorityQueue<T>
{
    void Enqueue(T item);
    T Dequeue();
    T Peek();
    void Clear();
    int Count { get; }
}
