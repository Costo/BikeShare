using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Services.Entities;

namespace BikeShare.Services
{
    public class BikeShareReadService
    {
        public IEnumerable<Station> GetStations()
        {
            var stations = Cache.Client.Get<int[]>("stations");
            if (stations == null) yield break;
            foreach(var id in stations )
            {
                var station = Cache.Client.Get<Station>("station_" + id);
                if (station != null) yield return station;
            }
        }
    }
}
