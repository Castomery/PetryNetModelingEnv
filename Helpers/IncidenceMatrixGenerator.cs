using PetryNet.Enums;
using PetryNet.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Helpers
{
    public static class IncidenceMatrixGenerator
    {
        public static Dictionary<string, Dictionary<string, int>> GenerateMatrix(
            PetryNetViewModel model, MatrixType type)
        {
            var result = new Dictionary<string, Dictionary<string, int>>();

            foreach (var place in model.Places)
            {
                var row = new Dictionary<string, int>();

                foreach (var transition in model.Transitions)
                {
                    int forward = model.Arcs
                        .Where(a => a.Source == place && a.Target == transition)
                        .Sum(a => a.Weight);

                    int backward = model.Arcs
                        .Where(a => a.Source == transition && a.Target == place)
                        .Sum(a => a.Weight);

                    int value = type switch
                    {
                        MatrixType.Forward => forward,
                        MatrixType.Backward => backward,
                        MatrixType.Combined => backward - forward,
                        _ => 0
                    };

                    row[transition.Name] = value;
                }

                result[place.Name] = row;
            }

            return result;
        }
    }
}
