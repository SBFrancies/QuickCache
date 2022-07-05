using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCache.Internal
{
    internal class LinkedList<T>
    {
        public int Count { get; private set; }

        private ListNode<T>? Start { get; set; }

        private ListNode<T>? End { get; set; }

        private bool IsEmpty => Count == 0;

        public LinkedList()
        {
        }

        public LinkedList(T? value)
        {
            var node = new ListNode<T>(value);

            Start = node;
            End = node;
            Count = 1;
        }

        internal void AddToEnd(ListNode<T> node)
        {
            ArgumentNullException.ThrowIfNull(nameof(node));
            node.Next = null;
            node.Previous = null;

            if (IsEmpty)
            {
                Start = End = node;
            }
            else
            {
                End!.Next = node;
                node.Previous = End;
                End = node;
            }

            Count++;
        }

        public void AddToStart(ListNode<T> node)
        {
            ArgumentNullException.ThrowIfNull(nameof(node));
            node.Next = null;
            node.Previous = null;

            if (IsEmpty)
            {
                Start = End = node;
            }
            else
            {
                Start!.Previous = node;
                node.Next = Start;
                Start = node;
            }

            Count++;
        }


        public void Remove(ListNode<T> node)
        {
            if (node != Start)
            {
                node.Previous!.Next = node.Next;
            }

            else
            {
                Start = node.Next;
            }

            if (node != End)
            {
                node.Next!.Previous = node.Previous;
            }

            else
            {
                End = node.Previous;
            }

            Count--;
        }

        public T? GetFirst()
        {
            if (!IsEmpty)
            {
                return Start!.Value;
            }

            throw new Exception("Empty list");
        }

        public T? GetLast()
        {
            if (!IsEmpty)
            {
                return End!.Value;
            }

            throw new Exception("Empty list");
        }
    }
}
