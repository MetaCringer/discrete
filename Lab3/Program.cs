using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args) 
        {
            bool[,] matrix;
            try 
            { 
                matrix = GetMatrixFromCSV("graph"); 
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                return;
            }
            Console.WriteLine("Смежная матрица графа:");
            Console.Write($"{"",3}");
            for (int i = 1; i < matrix.GetLength(1)+1; ++i) Console.Write($"{i,3}");
            Console.WriteLine();
            for (int i = 0; i < matrix.GetLength(0); ++i) 
            {
                Console.Write($"{i+1,3}");
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    Console.Write($"{(matrix[i, j] ? 1 : 0) ,3}");
                }
                Console.WriteLine();
            }
            int N;// Порядок графа. the order of graph
            int[] result = demucron(matrix,out N);
            ArrayList arr = new ArrayList();
            Console.WriteLine("Порядок графа: "+(N-1));
            for (int i = 0;i<N;++i)
            {
                arr.Clear();
                for (int j=0;j<result.Length;++j)
                {
                    if (result[j] == i) arr.Add(j+1);
                }

                Console.Write($"N{i}= " + "{ ");
                Console.WriteLine(String.Join(',', arr.ToArray()) + " }");
            }

            

        }

        static int[] demucron(bool[,] matrix,out int n)
        {
            int[] result = new int[matrix.GetLength(0)];
            List<int> todel = new List<int>();
            bool[] del = new bool[result.Length];
            n = 0;
            while (!ArrIsTrue(del))
            {
                todel.Clear();
                for (int i = 0; i < result.Length; ++i)
                {
                    if (del[i]) continue;
                    int j;
                    for (j = 0; j < result.Length; ++j) 
                    {
                        if (del[j]) continue;
                        if (matrix[j, i]) break;
                    }
                    if (j >= result.Length) 
                    { 
                        todel.Add(i);
                        result[i] = n;
                    }
                }
                DebugWriteMatrix(matrix,del,todel,n);
                foreach(int i in todel)
                {
                    del[i] = true;
                }
                ++n;
            }

            return result;
        }
        private static void DebugWriteMatrix(bool[,] matrix,bool[] del,List<int> todel,int N)
        {
            Debug.WriteLine("Порядок N: "+N);
            Debug.Write($"{"",3}");
            for (int i = 1; i < matrix.GetLength(1) + 1; ++i) Debug.Write($"{i,3}");
            Debug.WriteLine("");
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                Debug.Write($"{i + 1,3}");
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    Debug.Write($"{((del[i] || del[j]) ? "x" : (matrix[i, j] ? "1" : "0")),3}");
                }
                Debug.WriteLine("");
            }
            Debug.Write($"N{N}= " + "{ ");
            Debug.WriteLine(String.Join(',',todel)+" }");
        }
        private static bool ArrIsTrue(bool[] arr)
        {
            foreach (bool i in arr)
            {
                if (!i) return false;
            }
            return true;
        }
        static bool[,] GetMatrixFromCSV(string file)
        {
            bool[,] result;
            int n;
            StreamReader r;
            
            String[] row;
            int from, to;
            r = new StreamReader(file + ".csv");
            row = r.ReadLine().Split(";");
            n = int.Parse(row[1]);
            bool orgraph = "true".Equals(row[3]);
            result = new bool[n, n];
            r.ReadLine();
            while (!r.EndOfStream)
            {
                row = r.ReadLine().Split(";");
                if (!int.TryParse((row.Length > 1) ? row[0] : "err", out from))
                    throw new Exception("Не верный формат таблицы");
                for (int i = 1; i < row.Length; ++i)
                {
                    if (int.TryParse(row[i], out to))
                    {
                        result[from - 1, to - 1] = true;
                        if (!orgraph)
                            result[to - 1, from - 1] = true;
                    }
                }
            }
            r.Close();
            return result;
        }
        

    }
}
