using PetryNet.Enums;
using PetryNet.Helpers;
using PetryNet.Utils;
using PetryNet.ViewModels.Core;
using PetryNet.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace PetryNet.ViewModels
{
    public class InvariantAnalysisViewModel : BaseViewModel
    {
        private readonly PetryNetViewModel _petryNet;

        public ObservableCollection<InvariantRow> TInvariants { get; } = new();
        public ObservableCollection<InvariantRow> PInvariants { get; } = new();

        public string RepetitionStatus { get; set; } = "Невідомо";
        public string BoundednessStatus { get; set; } = "Невідомо";
        public string LivenessStatus { get; set; } = "Невідомо";
        public string ConservativenessStatus { get; set; } = "Невідомо";
        public string DeadlockFreeStatus { get; set; } = "Невідомо";
        public string ControllabilityStatus { get; set; } = "Невідомо";

        public InvariantAnalysisViewModel(PetryNetViewModel petriNet)
        {
            _petryNet = petriNet;
            AnalyzeInvariants();
            AnalyzeDynamicProperties();
        }

        public void AnalyzeInvariants()
        {
            var wMatrix = IncidenceMatrixGenerator.GenerateMatrix(_petryNet, MatrixType.Combined);

            var workMatrix = ToArrayMatrix(wMatrix);

            // T-Invariants: W * x = 0
            var tInvariants = ComputeTInvariants(workMatrix);

            // P-Invariants: x^T * W = 0 -> (W^T * x = 0)
            var pInvariants = ComputePInvariants(workMatrix);

            TInvariants.Clear();
            PInvariants.Clear();

            // Handle empty tInvariants: If empty, fill with a single row of zeros
            if (tInvariants.GetLength(0) == 0 || tInvariants.GetLength(1) == 0)
            {
                var emptyInvariantRow = new InvariantRow
                {
                    Name = "t0", // Assign a name to the empty T-invariant
                    Vector = new List<int>(new int[workMatrix.GetLength(1)]) // Fill with zeros
                };
                TInvariants.Add(emptyInvariantRow);
            }
            else
            {
                // Add T-Invariants to TInvariants collection
                for (int i = 0; i < tInvariants.GetLength(0); i++) // Iterate over rows
                {
                    var invariantRow = new InvariantRow
                    {
                        Name = $"t{i + 1}", // Assign a name to the T-invariant
                        Vector = new List<int>()
                    };

                    for (int j = 0; j < tInvariants.GetLength(1); j++) // Iterate over columns
                    {
                        invariantRow.Vector.Add(tInvariants[i, j]);
                    }

                    TInvariants.Add(invariantRow);
                }
            }

            // Handle empty pInvariants: If empty, fill with a single row of zeros
            if (pInvariants.GetLength(0) == 0 || pInvariants.GetLength(1) == 0)
            {
                var emptyInvariantRow = new InvariantRow
                {
                    Name = "p0", // Assign a name to the empty P-invariant
                    Vector = new List<int>(new int[workMatrix.GetLength(1)]) // Fill with zeros
                };
                PInvariants.Add(emptyInvariantRow);
            }
            else
            {
                // Add P-Invariants to PInvariants collection
                for (int i = 0; i < pInvariants.GetLength(0); i++) // Iterate over rows
                {
                    var invariantRow = new InvariantRow
                    {
                        Name = $"p{i + 1}", // Assign a name to the P-invariant
                        Vector = new List<int>()
                    };

                    for (int j = 0; j < pInvariants.GetLength(1); j++) // Iterate over columns
                    {
                        invariantRow.Vector.Add(pInvariants[i, j]);
                    }

                    PInvariants.Add(invariantRow);
                }
            }

        }

        //private int[,] BuildMatrix(PetryNetViewModel model)
        //{
        //    int rows = Place.Count;
        //    int cols = matrix[0].Count;
        //    int[,] result = new int[rows, cols];
        //    for (int i = 0; i < rows; i++)
        //        for (int j = 0; j < cols; j++)
        //            result[i, j] = matrix[i][j];
        //    return result;
        //}

        public static int[,] ToArrayMatrix(Dictionary<string, Dictionary<string, int>> matrixDict)
        {
            var rowKeys = matrixDict.Keys.ToList();
            var colKeys = matrixDict.First().Value.Keys.ToList();

            int[,] result = new int[rowKeys.Count, colKeys.Count];

            for (int i = 0; i < rowKeys.Count; i++)
            {
                for (int j = 0; j < colKeys.Count; j++)
                {
                    result[i, j] = matrixDict[rowKeys[i]][colKeys[j]];
                }
            }

            return result;
        }

        private int[,] ComputeTInvariants(int[,] wMatrix)
        {
            var invariants = CheckInvariantsFar(wMatrix);
            ReduceCoefficients(invariants);
            return invariants;
        }

        private int[,] ComputePInvariants(int[,] wMatrix)
        {
            var transposed = TransposeMatrix(wMatrix);
            int[,] invariants = CheckInvariantsFar(transposed);
            ReduceCoefficients(invariants);
            return invariants;
        }

        private int[,] CheckInvariantsFar(int[,] wInv)
        {
            //inverting a matrix
            int[,] w = new int[wInv.GetLength(1), wInv.GetLength(0)];
            for (int i = 0; i < w.GetLength(0); i++)
            {
                for (int j = 0; j < w.GetLength(1); j++)
                {
                    w[i, j] = wInv[j, i];
                }
            }
            int rowCountInit = w.GetLength(0);
            int colCount = w.GetLength(1) + w.GetLength(0);
            List<int[]> c = new List<int[]>();
            //creating augmented matrix
            for (int rowI = 0; rowI < rowCountInit; rowI++)
            {
                c.Add(new int[colCount]);
                c[rowI][rowI] = 1;//invariance
                for (int colI = rowCountInit; colI < colCount; colI++)//incidence
                {
                    c[rowI][colI] = w[rowI, colI - rowCountInit];
                }
            }
            for (int colI = rowCountInit; colI < colCount; colI++)//check incidence matrix columns
            {
                //count pos and neg elements indexes
                List<int> posInds = new List<int>(), negInds = new List<int>();
                for (int rowI = 0; rowI < c.Count; rowI++)
                {
                    if (c[rowI][colI] < 0) negInds.Add(rowI);
                    else if (c[rowI][colI] > 0) posInds.Add(rowI);
                }
                int newRowCount = 0;
                //for each pair add its sum
                for (int pI = 0; pI < posInds.Count; pI++)
                {
                    int j = posInds[pI];
                    for (int nI = 0; nI < negInds.Count; nI++)
                    {
                        int i = negInds[nI];
                        int gcd = GCD(Math.Abs(c[j][colI]), Math.Abs(c[i][colI]));
                        int iMult = Math.Abs(c[j][colI]) / gcd;
                        int jMult = Math.Abs(c[i][colI]) / gcd;
                        int[] newRow = new int[colCount];
                        for (int k = 0; k < colCount; k++)
                        {
                            newRow[k] = c[j][k] * jMult + c[i][k] * iMult;
                        }
                        c.Add(newRow);
                        newRowCount++;
                    }
                }
                //remove originals
                List<int> indsToRemove = posInds;
                indsToRemove.AddRange(negInds);
                indsToRemove.Sort();
                for (int i = indsToRemove.Count - 1; i >= 0; i--) c.RemoveAt(indsToRemove[i]);
                //checking
                for (int r = c.Count - newRowCount; r < c.Count; r++)
                {
                    int[] comp1 = c[r];
                    bool coversSome = false;
                    for (int cp = 0; cp < c.Count; cp++)
                    {
                        if (cp == r) continue;
                        int[] comp2 = c[cp];
                        bool covers = true;
                        for (int col = 0; col < colCount; col++)
                        {
                            if (comp1[col] == 0 && comp2[col] != 0)
                            {
                                covers = false;
                                break;
                            }
                        }
                        if (covers)
                        {
                            coversSome = true;
                            break;
                        }
                    }
                    if (coversSome)
                    {
                        c.RemoveAt(r);
                        r--;
                    }
                }
            }
            int[,] solutions = new int[c.Count, rowCountInit];
            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                for (int j = 0; j < solutions.GetLength(1); j++)
                {
                    solutions[i, j] = c[i][j];
                }
            }
            return solutions;
        }

        private int[,] TransposeMatrix(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] transposed = new int[cols, rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    transposed[j, i] = matrix[i, j];
            return transposed;
        }

        private int[,] ReduceCoefficients(int[,] solutions)
        {
            int[] allParVals = new int[solutions.GetLength(0) * (solutions.GetLength(1) - 1)];
            int count = 0;

            // Extract the values into the allParVals array
            for (int v = 0; v < solutions.GetLength(0); v++)
            {
                for (int p = 1; p < solutions.GetLength(1); p++)
                {
                    allParVals[count++] = solutions[v, p];
                }
            }

            // Check if allParVals has values (to prevent empty array case)
            if (allParVals.Length == 0 || allParVals.All(val => val == 0))
            {
                // No valid solutions found, return the original solutions matrix
                return solutions;
            }

            int gcdArr = GCDArray(allParVals);

            // If there's a GCD, divide the solutions matrix by it
            if (gcdArr != 0)
            {
                for (int v = 0; v < solutions.GetLength(0); v++)
                {
                    for (int p = 0; p < solutions.GetLength(1); p++)
                    {
                        solutions[v, p] /= gcdArr;
                    }
                }
            }

            return solutions;
        }

        //private int[,] ReduceCoefficients(int[,] solutions)
        //{
        //    int[] allParVals = new int[solutions.GetLength(0) * (solutions.GetLength(1) - 1)];
        //    for (int v = 0, i = 0; v < solutions.GetLength(0); v++)
        //    {
        //        for (int p = 1; p < solutions.GetLength(1); p++, i++)
        //        {
        //            allParVals[i] = solutions[v, p];
        //        }
        //    }
        //    int gcdArr = GCDArray(allParVals);
        //    for (int v = 0; v < solutions.GetLength(0); v++)
        //    {
        //        for (int p = 0; p < solutions.GetLength(1); p++)
        //        {
        //            if (gcdArr != 0) solutions[v, p] /= gcdArr;
        //        }
        //    }
        //    return solutions;
        //}

        private int GCD(int a, int b)
        {
            if (a == 0)
                return b;
            return GCD(b % a, a);
        }

        private int GCDArray(int[] numbers)
        {
            int result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                result = GCD(result, Math.Abs(numbers[i]));
                if (result == 1)
                    return 1;
            }
            return result;
        }

        private void AnalyzeDynamicProperties()
        {
            bool[] coveredT = new bool[TInvariants.Count];
            for (int i = 0; i < TInvariants.Count; i++)
            {
                if (TInvariants[i].Vector.Any(v => v != 0))
                    coveredT[i] = true;
            }
            int[] notCoveredIndsT = coveredT
                .Select((covered, index) => (covered, index))
                .Where(x => !x.covered)
                .Select(x => x.index)
                .ToArray();

            if (notCoveredIndsT.Length == 0)
            {
                RepetitionStatus = "Повторювана";
                BoundednessStatus = "Обмежена";
                LivenessStatus = "Жива";
            }
            else
            {
                RepetitionStatus = "Неповторювана";
                BoundednessStatus = "Необмежена";
                LivenessStatus = "Нежива";
            }

            bool[] coveredP = new bool[PInvariants.Count];
            for (int i = 0; i < PInvariants.Count; i++)
            {
                if (PInvariants[i].Vector.Any(v => v != 0))
                    coveredP[i] = true;
            }
            int[] notCoveredIndsP = coveredP
                .Select((covered, index) => (covered, index))
                .Where(x => !x.covered)
                .Select(x => x.index)
                .ToArray();

            if (notCoveredIndsP.Length == 0)
            {
                ConservativenessStatus = "Збережувана";
                DeadlockFreeStatus = "Безконфліктна";
            }
            else
            {
                ConservativenessStatus = "Незбережувана";
                DeadlockFreeStatus = "Конфліктна";
            }


            // Controllability: matrix rank vs number of transitions
            var matrixDict = IncidenceMatrixGenerator.GenerateMatrix(_petryNet, MatrixType.Combined);

            var placeNames = matrixDict.Keys.ToList(); // rows
            var transitionNames = matrixDict.First().Value.Keys.ToList(); // columns

            double[,] matrix = new double[placeNames.Count, transitionNames.Count];

            for (int i = 0; i < placeNames.Count; i++)
            {
                for (int j = 0; j < transitionNames.Count; j++)
                {
                    matrix[i, j] = matrixDict[placeNames[i]][transitionNames[j]];
                }
            }

            var mathNetMatrix = Matrix<double>.Build.DenseOfArray(matrix);
            int rank = mathNetMatrix.Rank();

            ControllabilityStatus = rank == Math.Min(placeNames.Count, transitionNames.Count)? "Контрольована" : $"Не контрольована (ранг={rank})";
        }
    }
}
