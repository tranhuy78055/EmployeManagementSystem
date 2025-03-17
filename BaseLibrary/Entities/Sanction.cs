using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Sanction: OtherBaseEntity
    {
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        public string Punishment { get; set; } = string.Empty;
        [Required]
        public DateTime PunishmentDate { get; set; } = DateTime.Now;
        //public int SanctionTypeId { get; set; }
        public SanctionType? SanctionType { get; set; }
        [Required]
        public int SanctionTypeId { get; set; }

    }
}
