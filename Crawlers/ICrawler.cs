﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeShare.Crawlers
{
    public interface ICrawler
    {
        Task Run();
    }
}