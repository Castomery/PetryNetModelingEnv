using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.DTOS
{
    public class ArcDTO
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public int Weight { get; set; } = 1;
    }
}
