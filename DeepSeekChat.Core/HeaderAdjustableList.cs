using System;
using System.Collections;
using System.Collections.Generic;

namespace DeepSeekChat.Core;

/// <summary>
/// A list that allows you to set a element to the head of the list. It can use to such storing the selection history and so on.
/// </summary>
/// <typeparam name="T">The type of list elements</typeparam>
/// [0] tail         [1]         head [2]
///  a -------------- b -------------- c
///         next             next
///  a  ----------->  b  ----------->  c
///       previous         previous
///  a  <-----------  b  <-----------  c

public class HeaderAdjustableList<T> : IList<T>
{
    class LinkedListItem
    {
        public LinkedListItem Next { get; set; }
        public LinkedListItem Previous { get; set; }
        public T? Data { get; set; }
        public LinkedListItem(T item)
        {
            Data = item;
        }

        public static IEnumerable<LinkedListItem> GetEnumerable(LinkedListItem item)
        {
            var current = item;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        public static IEnumerable<LinkedListItem> GetEnumerableReverse(LinkedListItem item)
        {
            var current = item;
            while (current != null)
            {
                yield return current;
                current = current.Previous;
            }
        }

        public static void ForeachLinkItem(LinkedListItem startItem, Action<LinkedListItem> action, bool reverse = false)
        {
            var enumerable = reverse ? GetEnumerableReverse(startItem) : GetEnumerable(startItem);
            foreach (LinkedListItem i in enumerable)
            {
                action(i);
            }
        }

        public static void ForeachLinkItem(LinkedListItem startItem, Func<LinkedListItem, bool> action, bool reverse = false)
        {
            var enumerable = reverse ? GetEnumerableReverse(startItem) : GetEnumerable(startItem);
            foreach (LinkedListItem i in enumerable)
            {
                var r = action(i);
                if (r)
                    return;
            }
        }

        public static IEnumerable<T> GetValueEnumerable(LinkedListItem item)
        {
            var current = item;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        public static IEnumerable<T> GetValueEnumerableReverse(LinkedListItem item)
        {
            var current = item;
            while (current != null)
            {
                yield return current.Data;
                current = current.Previous;
            }
        }
    }

    private LinkedListItem _head;
    private LinkedListItem _tail;
    private int _count;

    public HeaderAdjustableList()
    {
        _head = null;
        _tail = null;
        _count = 0;
    }

    public HeaderAdjustableList(IEnumerable<T> collection)
    {
        _head = null;
        _tail = null;
        _count = 0;
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    public bool RaiseToHead(T item)
    {
        LinkedListItem current = _tail;
        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Data, item))
            {
                if (current == _head)
                    return true;
                if (current.Previous != null)
                    current.Previous.Next = current.Next;
                else
                    _head = current.Next;
                if (current.Next != null)
                    current.Next.Previous = current.Previous;
                else
                    _tail = current.Previous;
                current.Previous = _head;
                current.Next = null;
                _head.Next = current;
                _head = current;
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public T this[int index]
    {
        set
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException(nameof(index));
            LinkedListItem.ForeachLinkItem(_tail, item =>
            {
                if (index == 0)
                {
                    item.Data = value;
                    return;
                }
                index--;
            }, true);
        }
        get
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException(nameof(index));
            var current = _head;
            for (int i = 0; i < index; i++)
            {
                current = current?.Previous;
            }
            return current.Data;
        }
    }

    public int Count => _count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (_count == 0)
        {
            var newHeadItem = new LinkedListItem(item)
            {
                Previous = null,
                Next = null
            };
            _head = newHeadItem;
            _tail = newHeadItem;
        }
        else if (_count == 1)
        {
            var newItem = new LinkedListItem(item)
            {
                Previous = _tail,
                Next = null
            };
            _head = newItem;
            _tail.Next = _head;

        }
        else
        {
            var newItem = new LinkedListItem(item)
            {
                Previous = _head,
                Next = null
            };
            _head.Next = newItem;
            _head = newItem;
        }

        _count++;
    }

    public void Clear()
    {
        LinkedListItem current = _head;
        while (current != null)
        {
            var next = current.Previous;
            current.Data = default;
            current.Next = null;
            current.Previous = null;
            current = next;
        }
        _head = null;
        _tail = null;
        _count = 0;

        GC.Collect();
    }

    public bool Contains(T item)
    {
        bool found = false;
        LinkedListItem.ForeachLinkItem(_head, listLinkItem =>
        {
            if (EqualityComparer<T>.Default.Equals(listLinkItem.Data, item))
            {
                found = true;
            }
        });
        return found;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array.Length - arrayIndex < _count)
            throw new ArgumentException("The number of elements in the source list is greater than the available space from arrayIndex to the end of the destination array.");

        LinkedListItem.ForeachLinkItem(_tail, item =>
        {
            array[arrayIndex] = item.Data;
            arrayIndex++;
        });
    }

    public IEnumerator<T> GetEnumerator()
    {
        return LinkedListItem.GetValueEnumerableReverse(_head).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        int index = _count - 1;

        LinkedListItem.ForeachLinkItem(_tail, listLinkItem =>
        {
            if (EqualityComparer<T>.Default.Equals(listLinkItem.Data, item))
            {
                return true;
            }
            index--;
            return false;
        });

        return index;
    }

    public void Insert(int index, T item)
    {
        throw new InvalidOperationException("Insert is not supported.");
    }

    public bool Remove(T item)
    {
        LinkedListItem current = _head;
        while (current != null)
        {
            var next = current.Next;
            if (EqualityComparer<T>.Default.Equals(current.Data, item))
            {
                current.Data = default;
                if (current.Previous != null)
                    current.Previous.Next = current.Next;
                else
                    _head = current.Next;
                if (current.Next != null)
                    current.Next.Previous = current.Previous;
                else
                    _tail = current.Previous;
                _count--;
                return true;
            }
            current = next;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        LinkedListItem current = _tail;
        while (current != null)
        {
            if (index == 0)
            {
                if (current.Previous != null)
                    current.Previous.Next = current.Next;
                else
                    _head = current.Next;
                if (current.Next != null)
                    current.Next.Previous = current.Previous;
                else
                    _tail = current.Previous;
                _count--;
                return;
            }
            index--;
            current = current.Previous;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return LinkedListItem.GetValueEnumerableReverse(_head).GetEnumerator();
    }
}
