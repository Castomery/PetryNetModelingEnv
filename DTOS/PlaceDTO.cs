using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.DTOS
{
    public class PlaceDTO
    {
        public string Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int TokenLimit {  get; set; }
    }
}
