using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching;

namespace BikeShare.Services
{
    public static class Cache
    {
        private static MemcachedClient client = new MemcachedClient();
        public static MemcachedClient Client
        {
            get
            {
                return client;
            }
        }
    }
}
