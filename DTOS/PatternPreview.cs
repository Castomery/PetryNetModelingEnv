using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.DTOS
{
    public class PatternPreview
    {
        public PatternDTO Pattern { get; set; }
        public string PreviewImagePath { get; set; }
        public string Name => Pattern.Name;
    }
}
