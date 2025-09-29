using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] _items;
    int _currentItemCount;

    public Heap(int maxHeapSize)
    {
        _items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item;
        SortUp(item);
        _currentItemCount++;
    }

    void SortUp(T item)
    {
        
    }

}

public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex
    {
        get;
        set;
    }
}