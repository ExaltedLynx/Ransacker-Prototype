using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

//Implemented using a min heap
//NOTE: n / 2 is the index of queue[n]'s parent node
//      queue[n] * 2 + 1 is the index of queue[n]'s left child
//      queue[n] * 2 + 2 is the index of queue[n]'s right child
public class PriorityQueue<T> : IPriorityQueue<T>
{
    private readonly List<T> queue = new List<T>();
    private readonly IComparer<T> comparer;
    public int Count => queue.Count;
    public PriorityQueue(IComparer<T> comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
    }

    public void Enqueue(T item)
    {
        queue.Add(item);
        CascadeUp();
    }

    //returns T with the highest priority and removes it from the queue
    public T Dequeue()
    {
        if (queue.Count == 0)
            Debug.LogError("Can't remove from an empty queue");

        T removed = queue[0]; //gets root node
        queue[0] = queue[queue.Count - 1]; //copy last node to the root
        queue.RemoveAt(queue.Count - 1); // remove original last node
        CascadeDown(0); //move new root node down to maintain heap structure
        return removed;
    }

    public T Peek()
    {
        return queue.Count > 0 ? queue[0] : default;
    }

    public void Clear()
    {
        queue.Clear();
    }

    /*
    public bool Contains(T item, Func<T, T, bool> predicate)
    {
        foreach (T element in queue)
        {
            if (predicate(item, element))
                return true;
        }
        return false;
    }
    */

    private void CascadeUp()
    {
        var currIndex = queue.Count - 1;
        while (currIndex + 1 / 2 > 0)
        {
            int parentIndex = currIndex / 2;
            int prioDiff = ComparePriority(currIndex, parentIndex);
            switch (prioDiff)
            {
                case -1: //currentIndex is higher priority than it's parent node
                    SwitchElements(currIndex, parentIndex);
                    break;
                case 1: //Stop cascading
                    return;
                case 0: //Same priority
                    RemoveAt(parentIndex); //effectively replaces the duplicate priority at parentIndex with the node at currIndex
                    break;
            }
            currIndex = parentIndex;
        }

    }

    private void CascadeDown(int index)
    {
        int queueLength = queue.Count - 1;
        int currentNode = index;
        int minChild;
        while (currentNode * 2 + 1 <= queueLength)
        {
            int leftChild = currentNode * 2 + 1;
            int rightChild = currentNode * 2 + 2;
                
            //Find node with the higher priority
            if (rightChild > queueLength || ComparePriority(leftChild, rightChild) == -1)
                minChild = leftChild;
            else
                minChild = rightChild;

            int prioDiff = ComparePriority(currentNode, minChild);
            switch(prioDiff)
            {
                case -1: //currentNode is higher priority
                    return;
                case 1: //currentNode is lower priority than minChild
                    SwitchElements(currentNode, minChild);
                    break;
                case 0: //Same priority
                    break;
            }
            currentNode = minChild; //moves current index to minChild's index
        }

    }

    private void RemoveAt(int index)
    {
        queue[index] = queue[queue.Count - 1];
        queue.RemoveAt(queue.Count - 1);
        CascadeDown(index);
    }

    private void SwitchElements(int index1, int index2)
    {
        var h = queue[index1];
        queue[index1] = queue[index2];
        queue[index2] = h;
    }

    private int ComparePriority(int index1, int index2)
    {
        return comparer.Compare(queue[index1], queue[index2]);
    }

    public List<T> GetQueue()
    {
        return queue;
    }
}
