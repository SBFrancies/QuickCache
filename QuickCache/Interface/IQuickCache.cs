using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCache.Interface
{
    public interface IQuickCache<TKey, TValue> where TKey : notnull
    {
        int Capacity { get; }

        TValue? Get(TKey key);

        void Set(TKey key, TValue? item, int priority = 0, long expiry = long.MaxValue);

        void ClearCache();
    }
}
