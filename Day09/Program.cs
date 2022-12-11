namespace Day09
{
    internal readonly record struct Vector(int X, int Y);

    internal readonly record struct Knot(int X = 0, int Y= 0)
    {
        public double DistanceTo(Knot other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }

        public Vector StepToward(Knot other)
        {
            return new Vector(Math.Sign(other.X - X), Math.Sign(other.Y - Y));
        }
        
        public static Knot operator +(Knot knot, Vector vec)
            => new(knot.X + vec.X, knot.Y + vec.Y);
    }
    
    internal class Rope
    {
        private readonly List<Knot> _knots = new();
        private readonly HashSet<Knot> _tailPositions = new();

        private static readonly IDictionary<char, Vector> Moves = new Dictionary<char, Vector>
        {
            { 'U', new Vector(0, 1) },
            { 'D', new Vector(0, -1) },
            { 'L', new Vector(-1, 0) },
            { 'R', new Vector(1, 0) },
        };

        public Rope(int knots)
        {
            for (int i = 0; i < knots; i++)
            {
                _knots.Add(new Knot());
            }
            _tailPositions.Add(_knots.Last());
        }

        private static IEnumerable<Vector> ParseInstructions(IEnumerable<string> series)
        {
            foreach (string instructions in series)
            {
                char move = instructions[0];
                int steps = int.Parse(instructions[2..]);
                
                for (int i = 0; i < steps; i++)
                {
                    yield return Moves[move];
                }
            }
        }

        public void Move(IEnumerable<string> series)
        {
            foreach (Vector move in ParseInstructions(series))
            {
                _knots[0] += move;

                for (int i = 1; i < _knots.Count; i++)
                {
                    Knot head = _knots[i - 1];
                    Knot tail = _knots[i];
                    
                    // Move tail as well if needed - if they touch, the distance between
                    // them is either 1 for direct neighbors or sqrt(2) for diagonal neighbors
                    if (tail.DistanceTo(head) > 1.5)
                    {
                        _knots[i] += tail.StepToward(head);
                    }
                }
                
                _tailPositions.Add(_knots.Last());
            }
        }

        public int TailPositions => _tailPositions.Count;
    }

    public class Puzzle
    {
        private readonly string[] _moves;
        
        private Puzzle(string inputFile)
        {
            _moves = File.ReadAllLines(inputFile);
        }

        private int ComputeSolution(int knots)
        {
            Rope rope = new Rope(knots);
            rope.Move(_moves);
            return rope.TailPositions;
        }

        private int ComputeSolution1()
        {
            return ComputeSolution(2);
        }

        private int ComputeSolution2()
        {
            return ComputeSolution(10);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day09\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 13)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 1)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day09\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}