using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching;

namespace BikeShare.Services
{
    public static class Cache
    {
        static MemcachedClient client;
        public static MemcachedClient Client
        {
            get
            {
                return client
                    ?? ( client = new MemcachedClient());
            }
        }

        public static void Initialize(Enyim.Caching.Configuration.MemcachedClientConfiguration config)
        {
            if (client != null)
            {
                throw new Exception("MemcachedClient has already been initialized");
            }
            client = new MemcachedClient(config);
        }
    }
}
