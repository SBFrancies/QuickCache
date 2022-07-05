using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCache.Internal
{
    internal class DictionaryList<TKey, TValue> where TKey : notnull
    {
        private readonly IDictionary<TKey, ListNode<(TKey key, TValue value)>> _dictionary = new Dictionary<TKey, ListNode<(TKey key, TValue value)>>();

        private readonly LinkedList<(TKey key, TValue value)> _list = new LinkedList<(TKey key, TValue value)>();

        public int Count => _dictionary.Count;

        public DictionaryList()
        {
        }

        public void AddItem(TKey key, TValue value)
        {
            var node = new ListNode<(TKey key, TValue value)>((key, value));
            _dictionary.Add(key, node);
            _list.AddToStart(node);
        }

        public void RemoveItem(TKey key)
        {
            if (_dictionary.Remove(key, out var node))
            {
                _list.Remove(node);
            }

            else
            {
                throw new KeyNotFoundException(nameof(key));
            }
        }

        internal (TKey key, TValue value) GetFirstItem()
        {
            return _list.GetFirst()!;
        }

        internal (TKey key, TValue value) GetLastItem()
        {
            return _list.GetLast()!;
        }
    }
}
