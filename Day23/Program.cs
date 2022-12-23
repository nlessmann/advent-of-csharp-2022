namespace Day23
{
    internal class Grove
    {
        private enum Direction
        {
            North,
            South,
            West,
            East
        }

        private static Direction NextDirection(Direction direction)
        {
            direction += 1;
            return Enum.IsDefined(direction) ? direction : 0;
        }

        private readonly record struct Elf(int X, int Y)
        {
            public IEnumerable<Elf> Neighbors()
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx != 0 || dy != 0)
                        {
                            yield return new Elf(X + dx, Y + dy);
                        }
                    }
                }
            }

            public IEnumerable<Elf> Neighbors(Direction direction)
            {
                switch (direction)
                {
                    case Direction.North:
                        yield return this with { Y = Y - 1 };
                        yield return new Elf(X - 1, Y - 1);
                        yield return new Elf(X + 1, Y - 1);
                        break;

                    case Direction.South:
                        yield return this with { Y = Y + 1 };
                        yield return new Elf(X - 1, Y + 1);
                        yield return new Elf(X + 1, Y + 1);
                        break;

                    case Direction.West:
                        yield return this with { X = X - 1 };
                        yield return new Elf(X - 1, Y + 1);
                        yield return new Elf(X - 1, Y - 1);
                        break;
                    
                    case Direction.East:
                        yield return this with { X = X + 1 };
                        yield return new Elf(X + 1, Y + 1);
                        yield return new Elf(X + 1, Y - 1);
                        break;
                    
                    default:
                        throw new ArgumentException("Invalid direction");
                }
            }
        }

        private readonly HashSet<Elf> _elves = new();
        private Direction _direction;
        
        public int Rounds { get; private set; }

        public Grove(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            for (int row = 0; row < lines.Length; row++)
            {
                string line = lines[row];
                for (int col = 0; col < line.Length; col++)
                {
                    if (line[col] != '#') continue;
                    _elves.Add(new Elf(col, row));
                }
            }
        }

        private IEnumerable<Direction> Directions()
        {
            Direction direction = _direction;
            do
            {
                yield return direction;
                direction = NextDirection(direction);
            } while (direction != _direction);
        }

        public bool MoveElves()
        {
            // Collect proposals where all the elves would like to move
            Dictionary<Elf, Elf> proposals = new();
            Direction[] directions = Directions().ToArray();
            foreach (Elf elf in _elves.Where(e => e.Neighbors().Any(_elves.Contains)))
            {
                foreach (Direction direction in directions)
                {
                    if (!elf.Neighbors(direction).Any(_elves.Contains))
                    {
                        proposals[elf] = elf.Neighbors(direction).First();
                        break;
                    }
                }
            }
            
            // Apply proposals that do not clash with other proposals
            var uniqueProposals = proposals
                .GroupBy(kv => kv.Value)
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .ToArray();
            
            foreach (var move in uniqueProposals)
            {
                _elves.Remove(move.Key);
                _elves.Add(move.Value);
            }
            
            // Start with another direction next time
            _direction = NextDirection(_direction);

            // Increment round counter and indicate whether any elf moved at all
            Rounds += 1;
            return uniqueProposals.Any();
        }

        public int CountEmptyTiles()
        {
            int xMin = _elves.Min(e => e.X);
            int yMin = _elves.Min(e => e.Y);
            int xMax = _elves.Max(e => e.X);
            int yMax = _elves.Max(e => e.Y);

            int size = (xMax - xMin + 1) * (yMax - yMin + 1);
            return size - _elves.Count;
        }
    }
    
    public class Puzzle
    {
        private readonly Grove _grove;
        
        private Puzzle(string inputFile)
        {
            _grove = new Grove(inputFile);
        }

        private int ComputeSolution1()
        {
            for (int i = 0; i < 10; i++)
            {
                _grove.MoveElves();
            }
            return _grove.CountEmptyTiles();
        }

        private int ComputeSolution2()
        {
            while (_grove.MoveElves());
            return _grove.Rounds;
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day23\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 110)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 20)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day23\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}