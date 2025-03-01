﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Country:BaseEntity
    {
        //One to many relationship with City
        [JsonIgnore]
        public List<City>? Cities { get; set; }
    }
}
