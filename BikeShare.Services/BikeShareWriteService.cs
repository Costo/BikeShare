using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Services.Entities;
using Enyim.Caching.Memcached;

namespace BikeShare.Services
{
    public class BikeShareWriteService
    {
        public void Store(Station[] stations)
        {
            Cache.Client.Store(StoreMode.Set, "stations", stations.Select(x => x.Id).ToArray());
            foreach (var s in stations)
            {
                Cache.Client.Store(StoreMode.Set, "station_" + s.Id, s);
            }
        }
    }
}
