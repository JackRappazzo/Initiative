using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Api.Core.Services.Encounters
{
    public class EncounterListItem
    {
        public string EncounterId { get; set; }
        public string EncounterName { get; set; }
        public int NumberOfCreatures { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
