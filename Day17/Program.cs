namespace Day17
{
    internal enum JetDirection
    {
        Left,
        Right
    }
    
    internal readonly struct Rock
    {
        private readonly byte[] _rows;
        
        public int Height => _rows.Length;
        public byte this[int i] => _rows[i];

        public Rock(IEnumerable<byte> shape)
        {
            _rows = shape.ToArray();
        }
        
        public static Rock operator +(Rock rock, JetDirection direction)
        {
            if (direction == JetDirection.Left)
            {
                if (rock._rows.Any(b => (b & 0b1000000) != 0))
                {
                    // Would hit a wall when moving
                    return rock;
                }
                
                return new Rock(rock._rows.Select(b => (byte)((b << 1) & 0xFF)).ToArray());
            }
            
            if (direction == JetDirection.Right)
            {
                if (rock._rows.Any(b => (b & 0b1) != 0))
                {
                    // Would hit a wall when moving
                    return rock;
                }
                
                return new Rock(rock._rows.Select(b => (byte)((b >> 1) & 0xFF)).ToArray());
            }

            throw new ArgumentException("Illegal jet direction");
        }
    }

    internal class Fingerprint : IEquatable<Fingerprint>
    {
        private readonly IReadOnlyList<byte> _rows;
        private readonly int _rock;
        private readonly int _jet;

        public Fingerprint(IEnumerable<byte> rows, int rockIndex, int jetIndex)
        {
            _rows = rows.ToList();
            _rock = rockIndex;
            _jet = jetIndex;
        }

        public bool Equals(Fingerprint? other)
        {
            if (other == null) return false;
            if (_rock != other._rock || _jet != other._jet) return false;
            if (_rows.Count != other._rows.Count) return false;
            return _rows.Zip(other._rows, (aa, bb) => aa == bb).All(v => v);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Fingerprint);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_rock.GetHashCode(), _rock, _jet);
        }
    }
    
    internal class Chamber
    {
        private const int Width = 7;
        
        private static readonly Rock[] Rocks =
        {
            new(new byte[]{ 0b0011110 }),
            new(new byte[]{ 0b0001000, 0b0011100, 0b0001000 }),
            new(new byte[]{ 0b0011100, 0b0000100, 0b0000100 }),
            new(new byte[]{ 0b0010000, 0b0010000, 0b0010000, 0b0010000 }),
            new(new byte[]{ 0b0011000, 0b0011000 })
        };

        private readonly JetDirection[] _jets;
        private readonly List<byte> _rows = new();

        private int _rocks;
        private int _nextRockIndex;
        private int _nextJetIndex;

        private const int FingerprintWindow = 25;
        private const int JetIndexObversationPeriod = 50000;
        private readonly Dictionary<(int Height, int Rocks), Fingerprint> _fingerprints = new();
        private readonly Dictionary<int, int> _jetIndices = new();
        private int _cycleJetIndex = -1;
        
        public int CycleLength { get; private set; }
        public int CycleRocks { get; private set; }
        public bool CycleDetected => CycleLength > 0;

        public int Height => _rows.Count;

        public Chamber(string jets)
        {
            _jets = jets.Trim().Select(s => s == '<' ? JetDirection.Left : JetDirection.Right).ToArray();
        }

        public override string ToString()
        {
            var lines = _rows
                .Select(s => Convert.ToString(s, 2).PadLeft(Width, '0'))
                .Select(s => new string(s.Select(c => c == '0' ? '.' : '#').ToArray()))
                .Reverse();
            return string.Join("\n", lines);
        }
        
        public void DropRock()
        {
            // Get shape of next rock and advance index to the following shape
            Rock rock = Rocks[_nextRockIndex];
            if (++_nextRockIndex >= Rocks.Length)
            {
                _nextRockIndex = 0;
            }

            int y = Height + 3;
            while (y >= 0)
            {
                // Apply jet of air that moves the rock
                Rock pushedRock = rock + _jets[_nextJetIndex];
                if (++_nextJetIndex >= _jets.Length)
                {
                    _nextJetIndex = 0;
                }
                
                // Check whether the rock would collide with another rock
                if (!Collides(pushedRock, y))
                {
                    rock = pushedRock;
                }
                
                // Attempt to move downwards, check whether the rock would collide
                if (Collides(rock, y - 1)) break;
                
                y--;
            }
            
            // Drop rock into place
            for (int i = 0; i < rock.Height; i++)
            {
                int row = y + i;
                if (row >= Height)
                {
                    _rows.Add(rock[i]);
                }
                else
                {
                    _rows[row] = (byte)((_rows[row] | rock[i]) & 0xFF);
                }
            }

            _rocks++;
            
            // Fingerprinting to find cycles
            if (Height <= JetIndexObversationPeriod)
            {
                _jetIndices[_nextJetIndex] = _jetIndices.GetValueOrDefault(_nextJetIndex, 0) + 1;
            }
            else if (_cycleJetIndex < 0)
            {
                _cycleJetIndex = _jetIndices.MaxBy(item => item.Value).Key;
            }
            else if (_nextJetIndex == _cycleJetIndex && CycleLength == 0)
            {
                Fingerprint fingerprint = new Fingerprint(_rows.TakeLast(FingerprintWindow), _nextRockIndex, _nextJetIndex);
                foreach (var fp in _fingerprints)
                {
                    if (fp.Value.Equals(fingerprint))
                    {
                        CycleLength = Height - fp.Key.Height;
                        CycleRocks = _rocks - fp.Key.Rocks;
                        break;
                    }
                }
                _fingerprints[(Height, Rocks: _rocks)] = fingerprint;
            }
        }

        private bool Collides(Rock rock, int y)
        {
            // Hitting the floor?
            if (y < 0) return true;
            
            for (int i = 0; i < rock.Height; i++)
            {
                int row = y + i;
                if (row >= Height) return false;
                if ((_rows[row] & rock[i]) != 0) return true;
            }

            return false;
        }
    }
    
    public class Puzzle
    {
        private readonly Chamber _chamber;
        
        private Puzzle(string inputFile)
        {
            string input = File.ReadAllText(inputFile);
            _chamber = new Chamber(input);
        }

        private int ComputeSolution1()
        {
            for (int i = 0; i < 2022; i++)
            {
                _chamber.DropRock();
            }
            
            return _chamber.Height;
        }

        private long ComputeSolution2()
        {
            const long iterations = 1000000000000;
            
            long offset = 0;
            for (long i = 2022; i < iterations; i++)
            {
                _chamber.DropRock();
                
                if (_chamber.CycleDetected && offset == 0)
                {
                    long cycles = (iterations - i) / _chamber.CycleRocks;
                    i += cycles * _chamber.CycleRocks;
                    offset = cycles * _chamber.CycleLength;
                }
            }

            return _chamber.Height + offset;
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day17\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 3068)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 1514285714288)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day17\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}