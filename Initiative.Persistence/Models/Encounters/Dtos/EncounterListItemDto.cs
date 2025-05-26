using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Persistence.Models.Encounters.Dtos
{
    public class EncounterListItemDto
    {
        public string EncounterId { get; set; }
        public string EncounterName { get; set; }
        public int NumberOfCreatures { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
