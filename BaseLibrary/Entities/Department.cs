﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Department : BaseEntity
    {
        //Many to one relationship with General Department
        public GeneralDepartment? GeneralDepartment { get; set; }
        public int GeneralDepartmentId { get; set; }

        // one to many relationship with Branch
        public List<Branch>? Branches { get; set; }
    }
}
