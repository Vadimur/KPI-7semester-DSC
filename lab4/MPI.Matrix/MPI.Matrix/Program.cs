using System;
using System.Collections.Generic;
using System.Linq;

namespace MPI.Matrix
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.Run(ref args, comm =>
            {
                if (comm.Rank == 0)
                {
                    int[,] matrix = GenerateMatrix(comm.Size);
                    PrintMatrix(matrix);

                    int[][] formatedMatrix = FormatMatrix(matrix, comm.Rank);

                    int[] elements = comm.Scatter(formatedMatrix, 0);
                    int sum = elements.Sum();

                    Console.WriteLine($"Process #{comm.Rank} received array: {string.Join(" ", elements)} -> {sum}");

                    int[] sums = comm.Gather(sum, 0);

                    Console.WriteLine($"Sums: {string.Join(" ", sums)}");

                    int[,] finalMatrix = ChangeDiagonal(matrix, sums);

                    Console.WriteLine("\nResult");
                    PrintMatrix(finalMatrix);
                }
                else
                {
                    var elements = comm.Scatter<int[]>(0);
                    int sum = elements.Sum();

                    Console.WriteLine($"Process #{comm.Rank} received array: {string.Join(" ", elements)} -> {sum}");

                    comm.Gather(sum, 0);
                }
            });
        }

        private static int[,] GenerateMatrix(int size)
        {
            Random random = new Random();
            int[,] matrix = new int[size, size];

            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    matrix[row, column] = random.Next(0, 10);
                }
            }

            return matrix;
        }

        private static int[][] FormatMatrix(int[,] matrix, int rank)
        {
            Dictionary<int, List<int>> rowColumnNumbers = new Dictionary<int, List<int>>();
            for (int row = 0; row < matrix.GetLength(0); row++)
            {

                for (int column = 0; column < matrix.GetLength(1); column++)
                {
                    if (row == column) continue;

                    if (!rowColumnNumbers.TryGetValue(row, out _))
                    {
                        rowColumnNumbers[row] = new List<int>();
                    }

                    if (!rowColumnNumbers.TryGetValue(column, out _))
                    {
                        rowColumnNumbers[column] = new List<int>();
                    }

                    rowColumnNumbers[column].Add(matrix[row, column]);                  
                    rowColumnNumbers[row].Add(matrix[row, column]);
                }

                //rowColumnNumbers[row].Add(matrix[row, row]);
            }

            int[][] formatedMatrix = new int[matrix.GetLength(0)][];

            for (int i = 0; i < rowColumnNumbers.Count; i++)
            {
                formatedMatrix[i] = rowColumnNumbers[i].ToArray();
            }

            return formatedMatrix;
        }

        private static int[,] ChangeDiagonal(int[,] matrix, int[] diagonalElements)
        {
            int[,] finalMatrix = (int[,]) matrix.Clone();

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int column = 0; column < matrix.GetLength(1); column++)
                {
                    if (row == column)
                    {
                        finalMatrix[row, column] = diagonalElements[row];
                    }
                }
            }

            return finalMatrix;
        }

        private static void PrintMatrix(int[,] arr)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0, 2} ", arr[i, j]));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
