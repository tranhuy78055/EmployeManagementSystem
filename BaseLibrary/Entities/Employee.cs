﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Employee: BaseEntity
    {
        [Required]
        public string? CivilId { get; set; }
        [Required]
        public string? FileNumber { get; set; }
        [Required]
        public string? JobName { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required,DataType(DataType.PhoneNumber)]
        public string? TelephoneNumber { get; set; }
        [Required]
        public string? Photo { get; set; }
        public string? Other { get; set; }

        // Many to one relationship with branch
        // Branch
        public Branch? Branch { get; set; }
        public int BranchId { get; set; }
        // Town
        public Town? Town { get; set; }
        public int TownId { get; set; }
    }
}
