using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Utils
{
    public static class MatrixUtils
    {
        // Converts a dictionary-based matrix to a 2D array.
        public static int[,] To2DArray(Dictionary<string, Dictionary<string, int>> matrixDict)
        {
            var rowKeys = new List<string>(matrixDict.Keys);
            var colKeys = new List<string>(matrixDict[rowKeys[0]].Keys);

            int[,] result = new int[rowKeys.Count, colKeys.Count];

            for (int i = 0; i < rowKeys.Count; i++)
            {
                var row = matrixDict[rowKeys[i]];
                for (int j = 0; j < colKeys.Count; j++)
                {
                    result[i, j] = row[colKeys[j]];
                }
            }

            return result;
        }

        // Transposes a matrix.
        public static int[,] Transpose(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] result = new int[cols, rows];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[j, i] = matrix[i, j];

            return result;
        }

        // Computes the rank of a matrix using Gaussian elimination.
        public static int ComputeRank(int[,] matrix)
        {
            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);
            double[,] mat = new double[rowCount, colCount];

            // Convert to double for stability
            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount; j++)
                    mat[i, j] = matrix[i, j];

            int rank = 0;
            for (int col = 0; col < colCount; col++)
            {
                int pivotRow = rank;
                while (pivotRow < rowCount && mat[pivotRow, col] == 0)
                    pivotRow++;

                if (pivotRow == rowCount) continue;

                // Swap rows
                for (int j = 0; j < colCount; j++)
                {
                    double temp = mat[rank, j];
                    mat[rank, j] = mat[pivotRow, j];
                    mat[pivotRow, j] = temp;
                }

                // Normalize and eliminate
                double pivotValue = mat[rank, col];
                for (int j = 0; j < colCount; j++)
                    mat[rank, j] /= pivotValue;

                for (int i = 0; i < rowCount; i++)
                {
                    if (i == rank) continue;
                    double factor = mat[i, col];
                    for (int j = 0; j < colCount; j++)
                        mat[i, j] -= factor * mat[rank, j];
                }

                rank++;
            }

            return rank;
        }
    }
}
