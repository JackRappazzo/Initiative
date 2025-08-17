using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Persistence.Models.Encounters.Dtos
{
    public class GetAvailableBestiaryDto
    {
        public required string Name { get; set; }
        public required string Id { get; set; }
        public required string? OwnerId { get; set; }
    }
}
