using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class SanctionType: BaseEntity
    {
        // Many to one relationshipwith vacation
        public List<Sanction>? Sanctions { get; set; }
    }
}
