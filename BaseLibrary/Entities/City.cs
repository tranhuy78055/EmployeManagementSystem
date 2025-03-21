﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class City:BaseEntity
    {
        //Many to one relationship with Country
        public Country? Country { get; set; }
        public int CountryId { get; set; }

        // One to many relationship with Town 
        [JsonIgnore]
        public List<Town>? Towns { get; set; }
    }
}
