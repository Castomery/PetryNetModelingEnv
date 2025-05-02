using PetryNet.Enums;
using PetryNet.Helpers;
using PetryNet.Models;
using PetryNet.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.ViewModels
{
    public class IncidenceMatrixViewModel : BaseViewModel
    {

        public readonly PetryNetViewModel Model;

        public IncidenceMatrixViewModel(PetryNetViewModel model)
        {
            Model = model;
            GenerateAllMatrices();
        }
        public ObservableCollection<MatrixRow> ForwardMatrix { get; } = new();
        public ObservableCollection<MatrixRow> BackwardMatrix { get; } = new();
        public ObservableCollection<MatrixRow> CombinedMatrix { get; } = new();

        private void GenerateAllMatrices()
        {
            GenerateMatrix(ForwardMatrix, MatrixType.Forward);
            GenerateMatrix(BackwardMatrix, MatrixType.Backward);
            GenerateMatrix(CombinedMatrix, MatrixType.Combined);
        }

        private void GenerateMatrix(ObservableCollection<MatrixRow> matrix, MatrixType type)
        {
            matrix.Clear();
            var rawMatrix = IncidenceMatrixGenerator.GenerateMatrix(Model, type);

            foreach (var entry in rawMatrix)
            {
                matrix.Add(new MatrixRow
                {
                    PlaceName = entry.Key,
                    Weights = entry.Value
                });
            }
        }
    }
}
