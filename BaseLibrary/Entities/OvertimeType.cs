using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class OvertimeType: BaseEntity
    {
        // Relationship : One to many with Overtime
        public List<Overtime> Overtimes = new List<Overtime>();
    }
}
