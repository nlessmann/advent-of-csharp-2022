namespace Day12
{
    using System.Text;
    
    internal class Map
    {
        private readonly int _rows;
        private readonly int _cols;
        private readonly int[,] _heights;

        private readonly record struct Point(int X, int Y);

        public Map(string filename)
        {
            var input = File.ReadLines(filename).ToArray();
            _rows = input.Length;
            _cols = input[0].Length;
            _heights = new int[_cols, _rows];

            for (int y = 0; y < _rows; y++)
            {
                byte[] ascii = Encoding.ASCII.GetBytes(input[y]);
                for (int x = 0; x < _cols; x++)
                {
                    _heights[x, y] = input[y][x] switch
                    {
                        'S' => 0,
                        'E' => 27,
                        _ => ascii[x] - 96
                    };
                }
            }
        }

        public int FindPath(int part = 1)
        {
            // Dijkstra's algorithm to find the shortest path
            PriorityQueue<Point, int> queue = new();
            HashSet<Point> visited = new();

            // Find starting point and destination, travel backwards
            Point origin = FindPoint(27) ?? throw new InvalidOperationException("No destination");
            Point destination = FindPoint(0) ?? throw new InvalidOperationException("No starting point");
            queue.Enqueue(origin, 0);
            
            // Keep track of all "a" levels that we reach
            Dictionary<Point, int> stepsToStartingPositions = new();

            while (queue.TryDequeue(out Point pos, out int steps))
            {
                // Have we reached our destination (the start)?
                if (GetHeight(pos) == 1)
                {
                    stepsToStartingPositions[pos] = steps;
                }

                // Have we reached this point already?
                if (visited.Contains(pos))
                {
                    continue;
                }

                // Mark position as visited, add neighbors to queue
                visited.Add(pos);
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        // We can move up, down, left or right
                        if (!((dx == 0) ^ (dy == 0))) continue;
                        
                        // Is the neighboring tile on the map?
                        Point neighbor = new Point(pos.X + dx, pos.Y + dy);
                        if (neighbor.X < 0 || neighbor.X >= _cols) continue;
                        if (neighbor.Y < 0 || neighbor.Y >= _rows) continue;
                        
                        // Has this neighbor been visited before via a better route?
                        if (visited.Contains(neighbor)) continue;
                        
                        // Is the step to this neighbor an illegal move?
                        if (GetHeight(pos) > GetHeight(neighbor) + 1) continue;
                        
                        // Okay, this neighbor is a valid move
                        queue.Enqueue(neighbor, steps + 1);
                    }
                }
            }

            return part == 1 ? stepsToStartingPositions[destination] : stepsToStartingPositions.Values.Min();
        }

        private Point? FindPoint(int height)
        {
            for (int x = 0; x < _cols; x++)
            for (int y = 0; y < _rows; y++)
                if (_heights[x, y] == height)
                    return new Point(x, y);

            return null;
        }

        private int GetHeight(Point p)
        {
            return Math.Max(1, Math.Min(26, _heights[p.X, p.Y]));
        }
    }
    
    public class Puzzle
    {
        private readonly Map _map;
        
        private Puzzle(string inputFile)
        {
            _map = new Map(inputFile);
        }

        private int ComputeSolution1()
        {
            return _map.FindPath(1);
        }

        private int ComputeSolution2()
        {
            return _map.FindPath(2);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day12\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 31)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 29)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day12\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}