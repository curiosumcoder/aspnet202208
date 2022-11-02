﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using WA80.Models;

namespace WA80.Controllers
{
    //[ResponseCache(CacheProfileName = "Basic")]
    public class HomeController : Controller
    {
        private IMemoryCache _cache;

        public HomeController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            var cacheEntry = _cache.Get<DateTime?>(CacheKeys.Entry);
            return View(cacheEntry);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Basic
        public IActionResult CacheTryGetValueSet()
        {
            DateTime cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue(CacheKeys.Entry, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = DateTime.Now;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    //.SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                // Save data in cache.
                _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
                //_cache.Set(CacheKeys.Entry, cacheEntry);
            }

            return View("Index", cacheEntry);
        }

        public IActionResult CacheGetOrCreate()
        {
            var cacheEntry = _cache.GetOrCreate(CacheKeys.Entry, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(3);
                return DateTime.Now;
            });

            return View("Index", cacheEntry);
        }

        [ActionName("CacheGetOrCreateAsyncronus")]
        public async Task<IActionResult> CacheGetOrCreateAsync()
        {
            var cacheEntry = await
                _cache.GetOrCreateAsync(CacheKeys.Entry, entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromSeconds(3);
                    return Task.FromResult(DateTime.Now);
                });

            return View("Index", cacheEntry);
        }

        public IActionResult CacheRemove()
        {
            _cache.Remove(CacheKeys.Entry);
            return RedirectToAction("Index");
        }
        #endregion

        #region Call Back
        public IActionResult GetCallbackEntry()
        {
            return View("Callback", new CallbackViewModel
            {
                CachedTime = _cache.Get<DateTime?>(CacheKeys.CallbackEntry),
                Message = _cache.Get<string>(CacheKeys.CallbackMessage)
            });
        }

        public IActionResult CreateCallbackEntry()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Pin to cache.
                .SetPriority(CacheItemPriority.NeverRemove)
                // Add eviction callback
                .RegisterPostEvictionCallback(callback: EvictionCallback, state: this);

            _cache.Set(CacheKeys.CallbackEntry, DateTime.Now, cacheEntryOptions);

            return RedirectToAction("GetCallbackEntry");
        }

        private static void EvictionCallback(object key, object value,
    EvictionReason reason, object state)
        {
            var message = $"Entry was evicted. Reason: {reason}.";
            ((HomeController)state)._cache.Set(CacheKeys.CallbackMessage, message);
        }

        public IActionResult RemoveCallbackEntry()
        {
            _cache.Remove(CacheKeys.CallbackEntry);
            return RedirectToAction("GetCallbackEntry");
        }
        #endregion

        #region Dependent
        public IActionResult GetDependentEntries()
        {
            return View("Dependent", new DependentViewModel
            {
                ParentCachedTime = _cache.Get<DateTime?>(CacheKeys.Parent),
                ChildCachedTime = _cache.Get<DateTime?>(CacheKeys.Child),
                Message = _cache.Get<string>(CacheKeys.DependentMessage)
            });
        }

        public IActionResult CreateDependentEntries()
        {
            var cts = new CancellationTokenSource();
            _cache.Set(CacheKeys.DependentCTS, cts);

            // https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-6.0#cache-dependencies
            using (var entry = _cache.CreateEntry(CacheKeys.Parent))
            {
                // expire this entry if the dependant entry expires.
                entry.Value = DateTime.Now;
                entry.RegisterPostEvictionCallback(DependentEvictionCallback, this);

                _cache.Set(CacheKeys.Child,
                    DateTime.Now,
                    new CancellationChangeToken(cts.Token));
            }

            return RedirectToAction("GetDependentEntries");
        }

        private static void DependentEvictionCallback(object key, object value,
            EvictionReason reason, object state)
        {
            var message = $"Parent entry was evicted. Reason: {reason}.";
            ((HomeController)state)._cache.Set(CacheKeys.DependentMessage, message);
        }

        public IActionResult RemoveChildEntry()
        {
            _cache.Get<CancellationTokenSource>(CacheKeys.DependentCTS).Cancel();
            return RedirectToAction("GetDependentEntries");
        }
        #endregion

        public IActionResult CancelTest()
        {
            var cachedVal = DateTime.Now.Second.ToString();
            CancellationTokenSource cts = new CancellationTokenSource();
            _cache.Set<CancellationTokenSource>(CacheKeys.CancelTokenSource, cts);

            // Don't use previous message.
            _cache.Remove(CacheKeys.CancelMsg);

            _cache.Set(CacheKeys.Ticks, cachedVal,
                new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(cts.Token))
                .RegisterPostEvictionCallback(
                    (key, value, reason, substate) =>
                    {
                        var cm = $"'{key}':'{value}' was evicted because: {reason}";
                        _cache.Set<string>(CacheKeys.CancelMsg, cm);
                    }
                ));

            return RedirectToAction("CheckCancel");
        }

        public IActionResult CheckCancel(int? id = 0)
        {
            if (id > 0)
            {
                CancellationTokenSource cts =
                   _cache.Get<CancellationTokenSource>(CacheKeys.CancelTokenSource);
                //cts.CancelAfter(100);
                cts.Cancel();
                // Cancel immediately with cts.Cancel();
            }

            ViewData["CachedTime"] = _cache.Get<string>(CacheKeys.Ticks);
            ViewData["Message"] = _cache.Get<string>(CacheKeys.CancelMsg); ;

            return View();
        }

        public IActionResult TagTest()
        {
            ViewBag.varyValue = new Random().Next(1, 5);

            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //[ResponseCache(Duration = 10,
        //    Location = ResponseCacheLocation.Client,
        //    VaryByQueryKeys = new string[] { "param1" })]
        [ResponseCache(CacheProfileName = "NoCaching")]
        //[ResponseCache(Duration = 10, Location = ResponseCacheLocation.Client)]
        public IActionResult Privacy()
        {
            System.Diagnostics.Debug.WriteLine($"Privacy: {DateTime.Now}");
            return View();
        }
    }
}