using Newtonsoft.Json;
using PetryNet.DTOS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Utils
{
    class PatternLoader
    {
        private const string PATH = "C:\\Інше\\темп\\Project\\PetryNet\\PetryNet\\Patterns";
        public PatternDTO LoadPattern(string fileName)
        {
            string fullPath = Path.Combine(PATH, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Pattern file not found", fileName);

            var json = File.ReadAllText(fullPath);
            var pattern = JsonConvert.DeserializeObject<PatternDTO>(json);

            if (pattern == null)
                throw new InvalidOperationException("Failed to deserialize pattern file.");

            return pattern;
        }
    }
}
