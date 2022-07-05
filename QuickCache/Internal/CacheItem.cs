using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjects.Interface;

namespace QuickCache.Internal
{
    internal class CacheItem<T>
    {
        public int Priority { get; }

        public long Expiry { get; }

        public T? Value { get; }

        public CacheItem(T? value, int priority, long expiry)
        {
            Value = value;
            Priority = priority;
            Expiry = expiry;
        }
    }
}
