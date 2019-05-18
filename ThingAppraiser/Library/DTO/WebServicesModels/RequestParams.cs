﻿using System.Collections.Generic;

namespace ThingAppraiser.Data.Models
{
    public class RequestParams
    {
        public List<string> ThingNames { get; set; }

        public ConfigRequirements ConfigRequirements { get; set; }


        public RequestParams()
        {
        }
    }
}