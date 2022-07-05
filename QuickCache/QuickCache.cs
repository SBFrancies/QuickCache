using QuickCache.Interface;
using QuickCache.Internal;

namespace QuickCache
{
    public class QuickCache<TKey, TValue> : IQuickCache<TKey, TValue> where TKey : notnull
    {
        private Func<long> GetCurrentDate { get; } = () => DateTimeOffset.UtcNow.Ticks;
        private readonly IDictionary<TKey, CacheItem<TValue>> _internalDictionary;
        private readonly IDictionary<long, IDictionary<TKey, CacheItem<TValue>>> _expiryCache;
        private readonly IDictionary<int, DictionaryList<TKey, CacheItem<TValue>>> _priorityCache;

        public QuickCache(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            Capacity = capacity;
            _internalDictionary = new Dictionary<TKey, CacheItem<TValue>>(capacity);
            _expiryCache = new SortedDictionary<long, IDictionary<TKey, CacheItem<TValue>>>();
            _priorityCache = new SortedDictionary<int, DictionaryList<TKey, CacheItem<TValue>>>();
        }

        public QuickCache(int capacity, Func<long> getCurrentDate) : this(capacity)
        {
            GetCurrentDate = getCurrentDate ?? throw new ArgumentNullException(nameof(getCurrentDate));
        }

        public int Capacity { get; }

        public TValue? Get(TKey key)
        {
            ArgumentNullException.ThrowIfNull(nameof(key));

            if (_internalDictionary.TryGetValue(key, out var value) && value.Expiry > GetCurrentDate())
            {
                _priorityCache[value.Priority].RemoveItem(key);
                _priorityCache[value.Priority].AddItem(key, value);

                return value.Value;
            }
            throw new KeyNotFoundException(nameof(key));
        }

        public void Set(TKey key, TValue? item, int priority = 0, long expiry = long.MaxValue)
        {
            ArgumentNullException.ThrowIfNull(nameof(key));

            var cacheItem = new CacheItem<TValue>(item, priority, expiry);

            if (_internalDictionary.ContainsKey(key))
            {
                PurgeItem(key);
            }

            else if (_internalDictionary.Count == Capacity)
            {
                PurgeItem();
            }

            AddItem(key, cacheItem);
        }

        public void ClearCache()
        {
            _internalDictionary.Clear();
            _expiryCache.Clear();
            _priorityCache.Clear();
        }

        internal bool ContainsKey(TKey key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        private void AddItem(TKey key, CacheItem<TValue> cacheItem)
        {
            _internalDictionary.Add(key, cacheItem);

            if (!_expiryCache.ContainsKey(cacheItem.Expiry))
            {
                _expiryCache.Add(cacheItem.Expiry, new Dictionary<TKey, CacheItem<TValue>>());
            }

            if (!_priorityCache.ContainsKey(cacheItem.Priority))
            {
                _priorityCache.Add(cacheItem.Priority, new DictionaryList<TKey, CacheItem<TValue>>());
            }

            _expiryCache[cacheItem.Expiry].Add(key, cacheItem);
            _priorityCache[cacheItem.Priority].AddItem(key, cacheItem);
        }

        private void PurgeItem(TKey key)
        {
            var cacheItem = _internalDictionary[key];
            _internalDictionary.Remove(key);
            _expiryCache[cacheItem.Expiry].Remove(key);

            if (_expiryCache[cacheItem.Expiry].Count == 0)
            {
                _expiryCache.Remove(cacheItem.Expiry);
            }

            _priorityCache[cacheItem.Priority].RemoveItem(key);

            if (_priorityCache[cacheItem.Priority].Count == 0)
            {
                _priorityCache.Remove(cacheItem.Priority);
            }
        }

        private void PurgeItem()
        {
            var firstExpired = _expiryCache.First();

            if (firstExpired.Key < GetCurrentDate())
            {
                PurgeItem(firstExpired.Value.First().Key);
            }

            else
            {
                var lowestPriority = _priorityCache.First();
                PurgeItem(lowestPriority.Value.GetLastItem().key);
            }
        }
    }
}