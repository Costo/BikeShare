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
        public void Store(string system, Station[] stations)
        {
            Cache.Client.Store(StoreMode.Set, "system_" + system, stations);
        }

        public void Store(string system, Station station)
        {
            Cache.Client.Store(StoreMode.Set, "system_" + system + "_station_" + station.Id, station);
        }
    }
}
