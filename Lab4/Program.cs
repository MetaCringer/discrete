using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args) 
        {
            bool[,] matrix;
            bool orgraph;
            try 
            { 
                matrix = GetMatrixFromCSV("graph",out orgraph); 
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                return;
            }
            matrix = toOrGraph(matrix);
            WriteMatrix(matrix);

            int N;
            int[] result;
            Dictionary<int, SortedSet<int>> colors = new Dictionary<int, SortedSet<int>>();
            
            try
            {
                result = Grandi(matrix, out N);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                return;
            }
            Console.WriteLine($"Хроматическое число: {N}");

            
            for (int i = 0; i < result.Length; ++i) 
            {
                if (!colors.ContainsKey(result[i])) 
                    colors.Add(result[i], new SortedSet<int>());
                colors[result[i]].Add(i+1);
            }
            for (int i =0;i<colors.Count;++i)
            {
                Console.WriteLine($"Color#{i} = {{ {String.Join(',', colors[i])} }}");
            }

            matrix = ArcsToVerticies(matrix);
            matrix = toOrGraph(matrix);
            WriteMatrix(matrix);
            try
            {
                result = Grandi(matrix, out N);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                return;
            }
            Console.WriteLine($"Хроматический класс: {N}");
            colors.Clear();
            for (int i = 0; i < result.Length; ++i)
            {
                if (!colors.ContainsKey(result[i]))
                    colors.Add(result[i], new SortedSet<int>());
                colors[result[i]].Add(i+1);
            }
            for(int i=0;i<colors.Count;++i)
                Console.WriteLine($"Color#{i} = {{ {String.Join(',', colors[i])} }}");

        }
        static void WriteMatrix(bool[,] matrix)
        {
            Console.WriteLine("Смежная матрица графа:");
            Console.Write($"{"",3}");
            for (int i = 1; i < matrix.GetLength(1) + 1; ++i) Console.Write($"{i,3}");
            Console.WriteLine();
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                Console.Write($"{i + 1,3}");
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    Console.Write($"{(matrix[i, j] ? 1 : 0),3}");
                }
                Console.WriteLine();
            }
        }

        static int[] Grandi(bool[,] matrix,out int chromatic)
        {
            int[] result=new int[matrix.GetLength(0)];
            Dictionary<int, int> colors = new Dictionary<int, int>();
            chromatic = 0;
            int c;
            for (int i = 0;i<matrix.GetLength(1);++i)
            {
                if ((c = GetColor(matrix, colors, i)) > chromatic)
                    chromatic = c;
            }
            foreach(int key in colors.Keys)
                result[key] = colors[key];
            return result;
        }



        private static int GetColor(bool[,] matrix,Dictionary<int,int> mapcolor,int v)
        {
            if (mapcolor.ContainsKey(v)) 
                return mapcolor[v];
            SortedSet<int> busy = new SortedSet<int>();
            for (int to=0;to<matrix.GetLength(0);++to)
            {
                if (matrix[v, to]) busy.Add(GetColor(matrix, mapcolor, to));
            }
            int i = 0;
            while (busy.Contains(i)) ++i;
            mapcolor.Add(v,i);
            return i;
        }        
        static bool[,] GetMatrixFromCSV(String file, out bool orgraph)
        {
            bool[,] result;
            int n;
            StreamReader r;
            
            String[] row; 
            int from,to;
            r = new StreamReader(file + ".csv");
            row = r.ReadLine().Split(";");
            n = int.Parse(row[1]);
            orgraph = "true".Equals(row[3]);
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
        static bool[,] toOrGraph(bool[,] matrix)// without contures
        {
            for (int i = 0; i < matrix.GetLength(0); ++i) 
            {
                for (int j = 0; j <= i; ++j) 
                {
                    matrix[i, j] = false;
                }
            }
            return matrix;
        }
        static bool[,] ArcsToVerticies(bool[,] matrix)
        {
            Dictionary<(int,int),SortedSet<(int, int)>> arcs = new Dictionary<(int, int), SortedSet<(int, int)>>();
            int i;
            for (i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j) 
                {
                    if (matrix[i, j])
                        arcs.Add((i,j),GetNeigbourgTo(matrix,(i,j) ));
                }
            }
            bool[,] result = new bool[arcs.Count, arcs.Count];

            Dictionary<(int, int), SortedSet<(int, int)>>.KeyCollection keys = arcs.Keys;
            Dictionary<(int, int), int> indexes = new Dictionary<(int, int), int>();
            i = 0;
            Console.WriteLine("Преобразуем дуги в вершины");
            foreach((int,int) a in keys)
            {
                indexes.Add(a, i++);
                Console.WriteLine($"Дуга ({a.Item1+1}, {a.Item2+1}): вершина № {i}");
            }
            foreach ((int,int) from in keys)
            {
                foreach ((int,int) to in arcs[from])
                {
                    result[indexes[from], indexes[to]] = true;
                }
            }

            return result;
        }
        private static SortedSet<(int, int)> GetNeigbourgTo(bool[,] matrix,(int,int) v)
        {
            SortedSet<(int, int)> result = new SortedSet<(int, int)>();
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                if (matrix[v.Item1, i])
                    result.Add((v.Item1, i));
                if (matrix[i, v.Item1])
                    result.Add((i, v.Item1));
                if (matrix[v.Item2, i])
                    result.Add((v.Item2, i));
                if (matrix[i, v.Item2])
                    result.Add((i, v.Item2));
            }
            result.Remove(v);
            return result;
        }
    }
}
