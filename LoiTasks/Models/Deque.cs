using System;
using System.Collections;
using System.Collections.Generic;
using LoiLL.Models;

namespace LoiTasks.Models
{
    public class Deque<T> : IEnumerable<T>
    {
        private DoublyNode<T> head;
        private DoublyNode<T> tail;
        private int count; 

        public void AddLast(T data)
        {
            var node = new DoublyNode<T>(data);

            if (head == null)
                head = node;
            else
            {
                tail.Next = node;
                node.Previous = tail;
            }

            tail = node;
            count++;
        }

        public void AddFirst(T data)
        {
            var node = new DoublyNode<T>(data);
            var temp = head;
            node.Next = temp;
            head = node;
            if (count == 0)
                tail = head;
            else
                temp.Previous = node;
            count++;
        }

        public T RemoveFirst()
        {
            if (count == 0)
                throw new InvalidOperationException();
            var output = head.Data;
            if (count == 1)
            {
                head = tail = null;
            }
            else
            {
                head = head.Next;
                head.Previous = null;
            }

            count--;
            return output;
        }

        public T RemoveLast()
        {
            if (count == 0)
                throw new InvalidOperationException();
            var output = tail.Data;
            if (count == 1)
            {
                head = tail = null;
            }
            else
            {
                tail = tail.Previous;
                tail.Next = null;
            }

            count--;
            return output;
        }

        public T First
        {
            get
            {
                if (IsEmpty)
                    throw new InvalidOperationException();
                return head.Data;
            }
        }

        public T Last
        {
            get
            {
                if (IsEmpty)
                    throw new InvalidOperationException();
                return tail.Data;
            }
        }

        public int Count => count;

        public bool IsEmpty => count == 0;

        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public bool Contains(T data)
        {
            var current = head;
            while (current != null)
            {
                if (current.Data.Equals(data))
                    return true;
                current = current.Next;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
    }
}