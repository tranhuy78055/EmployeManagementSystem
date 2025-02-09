﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Employee : BaseEntity
    {
        public string? CivilId { get; set; }
        public string? FileNumber { get; set; }
        public string? Fullname { get; set; }
        public string? JobName { get; set; }
        public string? Address { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? Other { get; set; }

        // Relationship: Many to One
        // GeneralDepartment
        public GeneralDepartment? GeneralDepartment { get; set; }
        public int? GeneralDepartmentId { get; set; }
        // Department
        public Department? Department { get; set; }
        public int? DepartmentId { get; set; }
        // Branch
        public Branch? Branch { get; set; }
        public int? BranchId { get; set; }
        // Town
        public Town? Town { get; set; }
        public int? TownId { get; set; }
    }
}
