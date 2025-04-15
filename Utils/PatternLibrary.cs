using Newtonsoft.Json;
using PetryNet.DTOS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Utils
{
    internal class PatternLibrary
    {
        public static ObservableCollection<PatternPreview> LoadAllPatternPreviews(string path)
        {
            var patternFiles = Directory.GetFiles(path, "*.json");
            var previews = new ObservableCollection<PatternPreview>();

            foreach (var file in patternFiles)
            {
                var json = File.ReadAllText(file);
                var pattern = JsonConvert.DeserializeObject<PatternDTO>(json);

                string nameWithoutExt = Path.GetFileNameWithoutExtension(file);
                string imagePath = Path.Combine(path, nameWithoutExt + ".png");

                if (!File.Exists(imagePath))
                    imagePath = "Images/default.png"; // запасне зображення

                previews.Add(new PatternPreview
                {
                    Pattern = pattern,
                    PreviewImagePath = imagePath
                });
            }

            return previews;
        }
    }
}
