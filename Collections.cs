
using System;
using System.Collections;
using System.Collections.Generic;

namespace MuonhoryoLibrary.Collections
{
    public sealed class SingleLinkedList<T> : ICollection<T>
    {
        public struct SingleLinkedListEnumerator : IEnumerator<T>
        {
            internal SingleLinkedListEnumerator(SingleLinkedList<T> Source)
            {
                this.Source= Source;
                CurrentNode = null;
                version = Source.Version;
                Index_ = 0;
            }
            private readonly SingleLinkedList<T> Source;
            private SingleLinkedListNode CurrentNode;
            private readonly int version;
            public int Index_ { get; private set; }
            public T Current =>CurrentNode.Value;
            object IEnumerator.Current =>CurrentNode.Value;
            public void Dispose()
            {
            }
            public bool MoveNext()
            {
                if (version != Source.Version)
                {
                    throw new InvalidOperationException("Collection was changed.");
                }
                if (CurrentNode == null)
                {
                    if (Source.Count > 0)
                    {
                        Index_++;
                        CurrentNode = Source.Head;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (CurrentNode.Next == null)
                    {
                        return false;
                    }
                    else
                    {
                        CurrentNode = CurrentNode.Next;
                        Index_++;
                        return true;
                    }
                }
            }
            public void Reset()
            {
                if(version != Source.Version)
                {
                    throw new InvalidOperationException("Collection was changed.");
                }
                CurrentNode = Source.Head;
                Index_ = 0;
            }
        }
        public sealed class SingleLinkedListNode
        {
            private SingleLinkedListNode() { }
            internal SingleLinkedListNode(T value, SingleLinkedListNode next=null)
            {
                Value = value;
                Next = next;
            }
            public T Value;
            internal SingleLinkedListNode Next;
            public static implicit operator T (SingleLinkedListNode node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }
                return node.Value;
            }
        }
        public SingleLinkedList()
        {
            Count = 0;
        }
        public SingleLinkedList(T headValue)
        {
            Head = new SingleLinkedListNode(headValue);
            Count = 1;
        }
        public SingleLinkedList(IEnumerable<T> collection)
        {
            foreach(var item in collection)
            {
                AddLast(item);
            }
        }
        public int Count_ => Count;
        private SingleLinkedListNode Head;
        private int Count;
        int ICollection<T>.Count => Count_;
        bool ICollection<T>.IsReadOnly => false;
        private int Version=0;
        public T this[int index]
        {
            get => GetAtIndex(index);
            set => GetAtIndex(index).Value = value;
        }

        public void AddLast(T item)
        {
            AddLast(new SingleLinkedListNode(item));
        }
        public void AddLast(SingleLinkedListNode item)
        {
            ValidateNode(item);
            if (Head == null)
            {
                Head = item;
            }
            else
            {
                SingleLinkedListNode node = Head;
                while (node.Next != null)
                {
                    node = node.Next;
                }
                node.Next = item;
            }
            Version++;
            Count++;
        }
        public void AddFirst(T item)
        {
            SingleLinkedListNode newNode = new SingleLinkedListNode(item, Head);
            Head = newNode;
            Version++;
            Count++;
        }
        public void AddFirst(SingleLinkedListNode item)
        {
            ValidateNode(item);
            item.Next = Head;
            Head = item;
            Version++;
            Count++;
        }
        public void AddAtIndex(int index, T item)
        {
            ValidateIndex(index);
            if (index == 0)
            {
                AddFirst(new SingleLinkedListNode(item));
            }
            InternalAddAtIndex(index, new SingleLinkedListNode(item));
        }
        public void AddAtIndex(int index, SingleLinkedListNode item)
        {
            ValidateIndex(index);
            ValidateNode(item);
            if (index == 0)
            {
                AddFirst(item);
            }
            InternalAddAtIndex(index, item);
        }
        public void InsertAfter(SingleLinkedListNode node,T item)
        {
            ValidateNode(node);
            InternalInsertAfter(node, new SingleLinkedListNode(item));
        }
        public void InsertAfter(SingleLinkedListNode node,SingleLinkedListNode item)
        {
            ValidateNode(node);
            ValidateNode(item);
            InternalInsertAfter(node, item);
        }
        public bool Remove(T item)
        {
            if (Head.Value.Equals(item))
            {
                InternalRemoveFirst();
                return true;
            }
            else
            {
                SingleLinkedListNode node = Head;
                while (node.Next != null)
                {
                    if (node.Next.Value.Equals(item))
                    {
                        InternalRemoveAfter(node);
                        return true;
                    }
                    node = node.Next;
                }
                return false;
            }
        }
        public bool Remove(SingleLinkedListNode item)
        {
            if (Head.Equals(item))
            {
                InternalRemoveFirst();
                return true;
            }
            else
            {
                SingleLinkedListNode node = Head;
                while (node.Next != null)
                {
                    if (node.Value.Equals(item))
                    {
                        InternalRemoveAfter(node);
                        return true;
                    }
                }
                return false;
            }
        }
        public void RemoveFirst()
        {
            ValidateNode(Head);
            InternalRemoveFirst();
        }
        public void RemoveLast()
        {
            RemoveAtIndex(Count_ - 1);
        }
        public void RemoveAtIndex(int index)
        {
            ValidateIndex(index);
            if (index == 0)
            {
                InternalRemoveFirst();
            }
            InternalRemoveAtIndex(index);
        }
        /// <summary>
        /// Remove first matching element.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveAtPredicate(Predicate<T> predicate)
        {
            if(predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (predicate(Head.Value))
            {
                InternalRemoveFirst();
                return true;
            }
            else
            {
                SingleLinkedListNode node = Head;
                while(node.Next != null)
                {
                    if (predicate(node.Next.Value))
                    {
                        InternalRemoveAfter(node);
                        return true;
                    }
                    node= node.Next;
                }
                return false;
            }
        }
        public void RemoveAfter(SingleLinkedListNode item)
        {
            ValidateNode(item);
            InternalRemoveAfter(item);
        }
        public int IndexOf(T item)
        {
            if (Head.Value.Equals(item))
            {
                return 0;
            }
            else
            {
                SingleLinkedListNode node = Head;
                for(int i = 1; i < Count; i++)
                {
                    if (node.Next.Value.Equals(item))
                    {
                        return i;
                    }
                    node = node.Next;
                }
                return -1;
            }
        }
        public int IndexOf(SingleLinkedListNode item)
        {
            ValidateNode(item);
            return InternalIndexOf(item);
        }
        public SingleLinkedListNode GetAtIndex(int index)
        {
            ValidateIndex(index);
            if (index == 0)
            {
                return Head;
            }
            SingleLinkedListNode node = Head;
            for(int i = 1; i <= index; i++)
            {
                node = node.Next;
            }
            return node;
        }
        public void Clear()
        {
            Head = null;
            Count = 0;
            Version++;
        }
        public bool Contains(T item)
        {
            if (Count == 0)
            {
                return false;
            }
            else if (Head.Value.Equals(item))
            {
                return true;
            }
            else
            {
                SingleLinkedListNode node = Head;
                while (node.Next != null)
                {
                    node = node.Next;
                    if (node.Value.Equals(item))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (Count - arrayIndex > array.Length)
            {
                throw new IndexOutOfRangeException("Array is too small to copy.");
            }
            if ( arrayIndex<0)
            {
                throw new IndexOutOfRangeException("arrayIndex must be more than or equal zero");
            }
            if (arrayIndex >= Count)
            {
                throw new IndexOutOfRangeException("arraIndex must be less than list's Count");
            }
            array = new T[Count];
            SingleLinkedListNode node = Head;
            for(int i = arrayIndex; i < Count; i++)
            {
                array[i] = node;
                node = node.Next;
            }
        }
        public T[] ToArray()
        {
            T[] array = new T[Count];
            SingleLinkedListNode node = Head;
            for(int i = 0; i < Count; i++)
            {
                array[i] = node;
                node = Head.Next;
            }
            return array;
        }
        public void Sort(Comparison<T> comparer)
        {
            List<T> list=new List<T>(this);
            list.Sort(comparer);
            Head = null;
            foreach(T item in list)
            {
                AddLast(item);
            }
        }
        public void Sort()
        {
            List<T> list = new List<T>(this);
            list.Sort();
            Head = null;
            foreach (T item in list)
            {
                AddLast(item);
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new SingleLinkedListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SingleLinkedListEnumerator(this);
        }
        void ICollection<T>.Add(T item)
        {
            AddLast(item);
        }
        private int InternalIndexOf(SingleLinkedListNode item)
        {
            if (Head.Equals(item))
            {
                return 0;
            }
            else
            {
                SingleLinkedListNode node = Head.Next;
                for (int i = 1; i < Count; i++)
                {
                    if (node.Equals(item))
                    {
                        return i;
                    }
                    node = node.Next;
                }
                return -1;
            }
        }
        private void InternalRemoveFirst()
        {
            Head = Head.Next;
            Version++;
            Count--;
        }
        private void InternalRemoveAtIndex(int index)
        {
            SingleLinkedListNode node = Head;
            for (int i = 1; i < index; i++)
            {
                node = node.Next;
            }
            InternalRemoveAfter(node);
        }
        private void InternalRemoveAfter(SingleLinkedListNode item)
        {
            if (item.Next.Next != null)
            {
                item.Next = item.Next.Next;
            }
            else
            {
                item.Next = null;
            }
            Version++;
            Count--;
        }
        private void InternalAddAtIndex(int index, SingleLinkedListNode item)
        {
            SingleLinkedListNode node = Head;
            for (int i = 1; i < index; i++)
            {
                node = node.Next;
            }
            InternalInsertAfter(node, item);
        }
        private void InternalInsertAfter(SingleLinkedListNode node, SingleLinkedListNode item)
        {
            item.Next = node.Next;
            node.Next = item;
            Count++;
            Version++;
        }
        private void ValidateNode(SingleLinkedListNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
        }
        private void ValidateIndex(int index)
        {
            if (index >= Count || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }
    }
}
