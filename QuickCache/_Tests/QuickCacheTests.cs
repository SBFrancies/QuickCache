using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace QuickCache._Tests
{
    public class QuickCacheTests
    {
        private readonly ITestOutputHelper _output;

        public QuickCacheTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]

        public void QuickCache_CachePurgeTest()
        {
            var cache = CreateSystemUnderTest<string, int>(5);
            var now = DateTime.UtcNow.Ticks;

            cache.Set("A", 221, 1, now + 100000000000000000L);
            cache.Set("B", 222, 2, now + 100000000000000000L);
            cache.Set("C", 223, 2, now - 100000000000000000L);
            cache.Set("D", 224, 2, now + 100000000000000000L);
            cache.Set("E", 225, 2, now + 100000000000000000L);
            cache.Set("F", 226, 6, now + 100000000000000000L);
            Assert.False(cache.ContainsKey("C"));
            cache.Set("G", 227, 6, now + 100000000000000000L);
            Assert.False(cache.ContainsKey("A"));
            cache.Get("E");
            cache.Get("B");
            cache.Set("H", 228, 6, now + 100000000000000000L);
            Assert.False(cache.ContainsKey("D"));
            cache.Set("I", 229, 6, now + 100000000000000000L);
            Assert.False(cache.ContainsKey("E"));
            cache.Set("J", 220, 6, now + 100000000000000000L);
            Assert.False(cache.ContainsKey("B"));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(200000)]
        [InlineData(400000)]
        [InlineData(600000)]
        [InlineData(800000)]
        [InlineData(1000000)]
        [InlineData(2000000)]
        [InlineData(4000000)]
        [InlineData(6000000)]
        [InlineData(8000000)]
        [InlineData(10000000)]
        public void QuickCache_Performance(int size)
        {
            var random = new Random();
            var cache = CreateSystemUnderTest<int, int>(size);
            var sets = new List<long>();

            for (int i = 0; i < size * 2; i++)
            {
                var sw = Stopwatch.StartNew();
                cache.Set(i, 1, random.Next(0, 5));
                sw.Stop();
                sets.Add(sw.ElapsedTicks);
            }

            var av = sets.Average();
            int key;
            var gets = new List<long>();

            for (var i = 0; i < size; i++)
            {
                do
                {
                    key = random.Next(0, size * 2);
                }
                while (!cache.ContainsKey(key));

                var sw2 = Stopwatch.StartNew();
                cache.Get(key);
                sw2.Stop();
                gets.Add(sw2.ElapsedTicks);
            }

            var av2 = gets.Average();

            _output.WriteLine($"Cache size: {size}, set: {av}");
            _output.WriteLine($"Cache size: {size}, get: {av2}");
        }

        private QuickCache<TKey, TValue> CreateSystemUnderTest<TKey, TValue>(int capacity) where TKey : notnull
        {
            return new QuickCache<TKey, TValue>(capacity);
        }
    }
}
