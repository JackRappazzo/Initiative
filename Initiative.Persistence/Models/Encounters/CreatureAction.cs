using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Persistence.Models.Encounters
{
    public class CreatureAction
    {
        public string Name { get; set; }
        public IEnumerable<string> Descriptions { get; set; }
    }
}
