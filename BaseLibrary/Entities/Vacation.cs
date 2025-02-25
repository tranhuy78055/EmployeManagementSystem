﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Vacation: OtherBaseEntity
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public int NumberofDays { get; set; }
        public DateTime EndDate =>StartDate.AddDays(NumberofDays);

        //Many to one relationship with Vacation type
        public int VacationTypeId { get; set; }
        [Required]
        public VacationType? VacationType { get; set; }

    }
}
