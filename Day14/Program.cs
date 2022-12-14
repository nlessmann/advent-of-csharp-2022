namespace Day14
{
    internal readonly record struct Point(int X, int Y)
    {
        public static Point FromString(string xy)
        {
            int[] values = xy.Trim().Split(",").Select(int.Parse).ToArray();
            return new Point(values[0], values[1]);
        }

        public static Point operator +(Point a, Vector b) => new(a.X + b.X, a.Y + b.Y);
    }

    internal readonly record struct Vector(int X, int Y);

    internal readonly record struct LineSegment(Point A, Point B)
    {
        public IEnumerable<Point> Points()
        {
            Vector vector = new Vector(Math.Sign(B.X - A.X), Math.Sign(B.Y - A.Y));
            
            Point point = A;
            while (point != B)
            {
                yield return point;
                point += vector;
            }

            yield return B;
        }

        public static LineSegment operator -(LineSegment segment, int xOffset)
        {
            Point a = segment.A with { X = segment.A.X - xOffset };
            Point b = segment.B with { X = segment.B.X - xOffset };
            return new LineSegment(a, b);
        }
    }

    internal readonly record struct BoundingBox(Point LowerCorner, Point UpperCorner)
    {
        public bool Contains(Point point)
        {
            if (point.X < LowerCorner.X) return false;
            if (point.X > UpperCorner.X) return false;
            if (point.Y < LowerCorner.Y) return false;
            if (point.Y > UpperCorner.Y) return false;
            return true;
        }
    }
    
    internal class Cave
    {
        private readonly BoundingBox _bb;
        private readonly Point _source;
        private readonly HashSet<Point> _occupied = new();
        private readonly int _floorHeight;
        
        private static readonly Vector[] Moves =
        {
            new(0, 1),
            new(-1, 1),
            new(1, 1)
        };

        private bool HasEndlessFloor => _floorHeight > 0;
        
        public Cave(string filename, bool hasFloor = false)
        {
            // Parse given points into line fragments
            List<LineSegment> walls = new();
            foreach (string line in File.ReadLines(filename))
            {
                var points = line.Split(" -> ").Select(Point.FromString).ToArray();
                for (int i = 1; i < points.Length; i++)
                {
                    walls.Add(new LineSegment(points[i - 1], points[i]));
                }
            }
            
            // Determine bounding box
            Point lowerCorner = new Point(
                walls.Min(w => Math.Min(w.A.X, w.B.X)),
                0  // height of the source of sand
            );
            Point upperCorner = new Point(
                walls.Max(w => Math.Max(w.A.X, w.B.X)),
                walls.Max(w => Math.Max(w.A.Y, w.B.Y))
            );

            if (hasFloor)
            {
                _floorHeight = upperCorner.Y + 2;
                upperCorner = upperCorner with { Y = _floorHeight };
            }
            else
            {
                _floorHeight = -1;
            }

            // Subtract x-offset so that the x coordinates start at 0
            int xOffset = lowerCorner.X;
            lowerCorner = lowerCorner with { X = 0 };
            upperCorner = upperCorner with { X = upperCorner.X - xOffset };
            _bb = new BoundingBox(lowerCorner, upperCorner);
            _source = new Point(500 - xOffset, 0);
            
            // Fill in the walls
            foreach (LineSegment wall in walls)
            {
                LineSegment w = wall - xOffset;
                foreach (Point p in w.Points())
                {
                    _occupied.Add(p);
                }
            }
        }

        private bool IsOccupied(Point pos)
        {
            if (_occupied.Contains(pos)) return true;
            return HasEndlessFloor && pos.Y == _floorHeight;
        }

        private bool IsInside(Point pos)
        {
            return _bb.Contains(HasEndlessFloor ? pos with { X = 0 } : pos);
        }

        public int FillWithSand()
        {
            int count = 0;
            Point pos = _source;
            
            while (IsInside(pos))
            {
                // Test whether we can move anywhere
                bool moved = false;
                foreach (Vector move in Moves)
                {
                    Point newPos = pos + move;
                    if (!IsInside(newPos) || !IsOccupied(newPos))
                    {
                        pos = newPos;
                        moved = true;
                        break;
                    }
                }
                
                if (moved) continue;
                
                // We cannot move, but are we in a spot where we can stay?
                if (!IsOccupied(pos))
                {
                    _occupied.Add(pos);
                    count++;

                    if (pos == _source) break;
                    pos = _source;
                }
                else
                {
                    break;
                }
            }
            
            return count;
        }
    }
    
    public class Puzzle
    {
        private readonly string _inputFile;
        
        private Puzzle(string inputFile)
        {
            _inputFile = inputFile;
        }

        private int ComputeSolution1()
        {
            Cave cave = new Cave(_inputFile);
            return cave.FillWithSand();
        }

        private int ComputeSolution2()
        {
            Cave cave = new Cave(_inputFile, true);
            return cave.FillWithSand();
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day14\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 24)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 93)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day14\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}