using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Heap<T> where T : IHeapItem<T>
{
    T[] _items;
    int _currentItemCount;

    public Heap(int maxHeapSize)
    {
        _items = new T[maxHeapSize];
    }
    public int Count
    {
        get { return _currentItemCount; }
    }

    public void Add(T item)
    {
        item.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item;
        SortUp(item);
        _currentItemCount++;
    }

    public T RemoveFirst(T item)
    {
        T firstItem = _items[0];
        _currentItemCount--;
        _items[0] = _items[_currentItemCount];
        _items[0].HeapIndex = 0;
        SortDown(_items[0]);
        return firstItem;
    }
    public bool Contains(T item)
    {
        return Equals(_items[item.HeapIndex], item);
    }

    private void SortUp(T item)
    {
        int parentItemIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = _items[parentItemIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentItemIndex = (item.HeapIndex - 1) / 2;
        }
    }
    private void SortDown(T item)
    {
        while (true)
        {
            int childItemIndexLeft = item.HeapIndex * 2 + 1;
            int childItemIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childItemIndexLeft < _currentItemCount)
            {
                swapIndex = childItemIndexLeft;
                if (childItemIndexRight < _currentItemCount)
                {
                    if (_items[childItemIndexLeft].CompareTo(_items[childItemIndexRight]) < 0)
                    {
                        swapIndex = childItemIndexRight;
                    }
                }
                if (item.CompareTo(_items[swapIndex]) < 0)
                {
                    Swap(item, _items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Swap(T itemA, T itemB)
    {
        _items[itemA.HeapIndex] = itemB;
        _items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
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