namespace Day08
{
    internal readonly struct Vector
    {
        public readonly int X;
        public readonly int Y;

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y);
    }
    
    public class Puzzle
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int[,] _trees;
        
        private Puzzle(string inputFile)
        {
            var input = File.ReadAllLines(inputFile);
            _width = input[0].Length;
            _height = input.Length;
            _trees = new int[_height, _width];

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _trees[y, x] = int.Parse(input[y][x].ToString());
                }
            }
        }

        private int ComputeSolution1()
        {
            var visible = new bool[_height * _width];

            for (int y = 0; y < _height; y++)
            {
                // Left to right
                int maxHeight = -1;
                for (int x = 0; x < _width; x++)
                {
                    if (_trees[y, x] > maxHeight)
                    {
                        visible[y * _height + x] = true;
                        maxHeight = _trees[y, x];
                    }
                }
                
                // Right to left
                maxHeight = -1;
                for (int x = _width - 1; x >= 0; x--)
                {
                    if (_trees[y, x] > maxHeight)
                    {
                        visible[y * _height + x] = true;
                        maxHeight = _trees[y, x];
                    }
                }
            }
            
            for (int x = 0; x < _width; x++)
            {
                // Top to bottom
                int maxHeight = -1;
                for (int y = 0; y < _height; y++)
                {
                    if (_trees[y, x] > maxHeight)
                    {
                        visible[y * _height + x] = true;
                        maxHeight = _trees[y, x];
                    }
                }
                
                // Bottom to top
                maxHeight = -1;
                for (int y = _height - 1; y >= 0; y--)
                {
                    if (_trees[y, x] > maxHeight)
                    {
                        visible[y * _height + x] = true;
                        maxHeight = _trees[y, x];
                    }
                }
            }

            return visible.Count(v => v);
        }

        private int ScenicScore(int x, int y)
        {
            List<int> viewingDistances = new List<int>();
            Vector[] viewingDirections = { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };

            int height = _trees[y, x];
            foreach (Vector direction in viewingDirections)
            {
                int distance = 0;
                Vector p = new Vector(x, y) + direction;

                while (p.X >= 0 && p.X < _width && p.Y >= 0 && p.Y < _height)
                {
                    distance++;
                    if (_trees[p.Y, p.X] >= height) break;
                    p += direction;
                }
                
                viewingDistances.Add(distance);
            }
            
            return viewingDistances.Aggregate(1, (total, value) => total * value);
        }

        private int ComputeSolution2()
        {
            int maxScenicScore = 0;
            
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int scenicScore = ScenicScore(x, y);
                    if (scenicScore > maxScenicScore)
                    {
                        maxScenicScore = scenicScore;
                    }
                }
            }

            return maxScenicScore;
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day08\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 21)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 8)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day08\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}