using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Helpers
{
    public class InvariantRow
    {
        public string Name { get; set; }
        public List<int> Vector { get; set; } = new();
    }
}
