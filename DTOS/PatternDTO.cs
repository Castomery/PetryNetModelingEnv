using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.DTOS
{
    public class PatternDTO
    {
        public string Name { get; set; }
        public List<PlaceDTO> Places { get; set; }
        public List<TransitionDTO> Transitions { get; set; }
        public List<ArcDTO> Arcs { get; set; }
    }
}
