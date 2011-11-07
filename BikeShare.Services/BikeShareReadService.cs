using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Services.Entities;

namespace BikeShare.Services
{
    public class BikeShareReadService
    {
        public IEnumerable<Station> GetStations(string system)
        {
            return Cache.Client.Get<Station[]>("system_" + system)
                ?? Enumerable.Empty<Station>();
        }
    }
}
