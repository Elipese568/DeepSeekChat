using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Foundation;

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
    class ListLinkItem
    {
        public ListLinkItem? Next { get; set; }
        public ListLinkItem? Previous { get; set; }
        public T? Data { get; set; }
        public ListLinkItem(T item)
        {
            Data = item;
        }

        public static IEnumerable<ListLinkItem> GetEnumerable(ListLinkItem? item)
        {
            var current = item;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        public static IEnumerable<ListLinkItem> GetEnumerableReverse(ListLinkItem? item)
        {
            var current = item;
            while (current != null)
            {
                yield return current;
                current = current.Previous;
            }
        }

        public static void ForeachLinkItem(ListLinkItem? startItem, Action<ListLinkItem> action, bool reverse = false)
        {
            var enumerable = reverse ? GetEnumerableReverse(startItem) : GetEnumerable(startItem);
            foreach (ListLinkItem i in enumerable)
            {
                action(i);
            }
        }

        public static void ForeachLinkItem(ListLinkItem? startItem, Func<ListLinkItem, bool> action, bool reverse = false)
        {
            var enumerable = reverse ? GetEnumerableReverse(startItem) : GetEnumerable(startItem);
            foreach (ListLinkItem i in enumerable)
            {
                var r = action(i);
                if (r)
                    return;
            }
        }

        public static IEnumerable<T> GetValueEnumerable(ListLinkItem? item)
        {
            var current = item;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        public static IEnumerable<T> GetValueEnumerableReverse(ListLinkItem? item)
        {
            var current = item;
            while (current != null)
            {
                yield return current.Data;
                current = current.Previous;
            }
        }
    }

    private ListLinkItem? _head;
    private ListLinkItem? _tail;
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
        ListLinkItem? current = _tail;
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
            ListLinkItem.ForeachLinkItem(_tail, item =>
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
        if(_count == 0)
        {
            var newHeadItem = new ListLinkItem(item)
            {
                Previous = null,
                Next = null
            };
            _head = newHeadItem;
            _tail = newHeadItem;
        }
        else if(_count == 1)
        {
            var newItem = new ListLinkItem(item)
            {
                Previous = _tail,
                Next = null
            };
            _head = newItem;
            _tail.Next = _head;
            
        }
        else
        {
            var newItem = new ListLinkItem(item)
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
        ListLinkItem? current = _head;
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
        ListLinkItem.ForeachLinkItem(_head, listLinkItem =>
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
        if(array.Length - arrayIndex < _count)
            throw new ArgumentException("The number of elements in the source list is greater than the available space from arrayIndex to the end of the destination array.");

        ListLinkItem.ForeachLinkItem(_tail, item =>
        {
            array[arrayIndex] = item.Data;
            arrayIndex++;
        });
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ListLinkItem.GetValueEnumerableReverse(_head).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        int index = _count - 1;

        ListLinkItem.ForeachLinkItem(_tail, listLinkItem =>
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
        ListLinkItem? current = _head;
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

                GC.Collect();
                return true;
            }
            current = next;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        ListLinkItem? current = _tail;
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
        return ListLinkItem.GetValueEnumerableReverse(_head).GetEnumerator();
    }
}
