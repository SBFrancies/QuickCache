using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickCache.Interface;

namespace QuickCache.Internal
{
    internal class ListNode<T>
    {
        public T? Value { get; }

        public ListNode<T>? Previous { get; set; }

        public ListNode<T>? Next { get; set; }

        public ListNode(T? value)
        {
            Value = value;
        }

        public ListNode(T? value, ListNode<T>? previous, ListNode<T>? next) : this(value)
        {
            Previous = previous;
            Next = next;
        }
    }
}
