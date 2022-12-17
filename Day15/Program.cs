namespace Day15
{
    using System.Text.RegularExpressions;

    internal readonly record struct Point(int X, int Y)
    {
        public int Distance(Point other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }
    }

    internal readonly record struct HorizontalRange(int From, int To) : IComparable
    {
        public int Width => To - From + 1;
        
        public bool Overlaps(HorizontalRange other)
        {
            return Math.Max(From, other.From) <= Math.Min(To, other.To);
        }

        public static HorizontalRange operator +(HorizontalRange a, HorizontalRange b)
        {
            return new HorizontalRange(Math.Min(a.From, b.From), Math.Max(a.To, b.To));
        }

        public int CompareTo(object? other)
        {
            if (other == null) return 1;

            if (other is HorizontalRange otherRange)
            {
                return From.CompareTo(otherRange.From);
            }

            throw new ArgumentException("Object is not a HorizontalRange");
        }
    }
    
    internal partial class Sensor
    {
        [GeneratedRegex(@"[0-9\-]+", RegexOptions.Compiled)]
        private static partial Regex ReportPattern();

        public readonly Point Position;
        public readonly Point Beacon;
        
        private readonly int _radius;
        
        public Sensor(string report)
        {
            // Sensor at x=2, y=18: closest beacon is at x=-2, y=15
            var matches = ReportPattern().Matches(report);
            var n = matches.Select(s => int.Parse(s.Value)).ToArray();
            Position = new Point(n[0], n[1]);
            Beacon = new Point(n[2], n[3]);
            _radius = Position.Distance(Beacon);
        }

        public bool Covers(Point position)
        {
            return Position.Distance(position) <= _radius;
        }

        public HorizontalRange? CoveredRange(int y)
        {
            Point p = Position with { Y = y };
            int distance = p.Distance(Position);
            if (distance > _radius) return null;
            
            return new HorizontalRange(
                p.X - (_radius - distance),
                p.X + (_radius - distance)
            );
        }

        private bool HasSuitableGap(Sensor other)
        {
            int distance = Position.Distance(other.Position);
            return distance - _radius - other._radius == 2;
        }

        public IEnumerable<Point> Gap(Sensor other)
        {
            if (!HasSuitableGap(other)) yield break;
            
            int dx = Math.Sign(other.Position.X - Position.X);
            int dy = Math.Sign(other.Position.Y - Position.Y);
            
            Point first = Position with { X = Position.X + dx * (_radius + 1) };
            Point last = Position with { Y = Position.Y + dy * (_radius + 1) };

            Point p = first;
            while (p != last)
            {
                yield return p;
                p = new Point(p.X - dx, p.Y + dy);
            }

            yield return last;
        }
    }

    public class Puzzle
    {
        private readonly IReadOnlyList<Sensor> _sensors;

        private Puzzle(string inputFile)
        {
            string[] input = File.ReadAllLines(inputFile);
            _sensors = input.Select(s => new Sensor(s)).ToList();
        }

        private int ComputeSolution1(int y)
        {
            List<HorizontalRange> ranges = _sensors
                .Select(s => s.CoveredRange(y))
                .Where(r => r != null)
                .Cast<HorizontalRange>()
                .Order()
                .ToList();

            bool merged;
            do
            {
                merged = false;
                for (int i = 0; i < ranges.Count - 1; i++)
                {
                    if (!ranges[i].Overlaps(ranges[i + 1])) continue;

                    ranges[i] += ranges[i + 1];
                    ranges.RemoveAt(i + 1);
                    merged = true;
                    break;
                }
            } while (merged);

            // Count number of beacons in this row
            int beacons = _sensors.Select(s => s.Beacon).Where(b => b.Y == y).Distinct().Count();
            
            // Sum up width of the remaining ranges, subtract number of beacons as they don't count
            return ranges.Sum(r => r.Width) - beacons;
        }

        private long ComputeSolution2(int maxCoord)
        {
            for (int i = 0; i < _sensors.Count - 1; i++)
            {
                for (int j = i + 1; j < _sensors.Count; j++)
                {
                    foreach (Point p in _sensors[i].Gap(_sensors[j]))
                    {
                        if (p.X < 0 || p.X > maxCoord || p.Y < 0 || p.Y > maxCoord) continue;
                        
                        if (!_sensors.Any(s => s.Covers(p)))
                        {
                            return p.X * 4000000L + p.Y;
                        }
                    }
                }
            }

            return -1;
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day15\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1(10)} (should be 26)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2(20)} (should be 56000011)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day15\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1(2000000)}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2(4000000)}");
        }
    }
}