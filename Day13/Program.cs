namespace Day13
{
    using System.Text.RegularExpressions;

    internal partial class Packet : IComparable
    {
        [GeneratedRegex(@"\[((?:\-?\d+,?)*)\]", RegexOptions.Compiled)]
        private static partial Regex ListExpr();
        
        private readonly List<List<int>> _lut = new();

        public Packet(string tokens)
        {
            Match match;
            while ((match = ListExpr().Match(tokens)).Success)
            {
                string group = match.Groups[1].Value;
                _lut.Add(group == "" ? new List<int>() : group.Split(",").Select(int.Parse).ToList());

                int id = -_lut.Count;
                tokens = ListExpr().Replace(tokens, id.ToString(), 1);
            }
        }
        
        private IReadOnlyList<int> Content => _lut[^1];
        
        private IReadOnlyList<int> WrapOrLookUp(int value)
        {
            return value < 0 ? _lut[-value - 1] : new List<int> { value };
        }

        private bool? CompareLists(IReadOnlyList<int> a, Packet other, IReadOnlyList<int> b)
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (i >= b.Count) return false;

                int v1 = a[i];
                int v2 = b[i];

                if (v1 >= 0 && v2 >= 0)
                {
                    if (v1 < v2) return true;
                    if (v1 > v2) return false;
                    continue;
                }
                
                bool? result = CompareLists(WrapOrLookUp(v1), other, other.WrapOrLookUp(v2));
                if (result != null) return result;
            }

            return a.Count < b.Count ? true : null;
        }

        public int CompareTo(object? other)
        {
            if (other == null) return 1;

            if (other is Packet otherPacket)
            {
                bool? result = CompareLists(Content, otherPacket, otherPacket.Content);
                return result switch
                {
                    null => 0,
                    true => 1,
                    false => -1
                };
            }

            throw new ArgumentException("Object is not a Packet");
        }
        
        public static bool operator >=(Packet a, Packet b) => a.CompareTo(b) >= 0;
        public static bool operator <=(Packet a, Packet b) => a.CompareTo(b) <= 0;
    }
    
    public class Puzzle
    {
        private readonly List<Packet> _packets = new();
        
        private Puzzle(string inputFile)
        {
            foreach (string line in File.ReadLines(inputFile))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                _packets.Add(new Packet(line));
            }
        }

        private int ComputeSolution1()
        {
            int score = 0;
            for (int i = 0; i < _packets.Count; i += 2)
            {
                if (_packets[i] >= _packets[i + 1])
                {
                    score += 1 + (i / 2);
                }
            }

            return score;
        }

        private int ComputeSolution2()
        {
            Packet two = new Packet("[[2]]");
            Packet six = new Packet("[[6]]");
            
            _packets.Add(two);
            _packets.Add(six);
            _packets.Sort();
            _packets.Reverse();

            return (1 + _packets.IndexOf(two)) * (1 + _packets.IndexOf(six));
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day13\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 13)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 140)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day13\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}