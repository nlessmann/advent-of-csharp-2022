namespace Day04
{
    internal class SectionRange
    {
        public readonly int First;
        public readonly int Last;

        private SectionRange(int first, int last)
        {
            First = first;
            Last = last;
        }

        public SectionRange(string range)
        {
            string[] values = range.Split('-');
            First = int.Parse(values[0]);
            Last = int.Parse(values[1]);
        }

        public bool FullyContains(SectionRange other)
        {
            return First <= other.First && Last >= other.Last;
        }

        public SectionRange? Intersect(SectionRange other)
        {
            if (Last < other.First || First > other.Last) return null;

            int newFirst = Math.Max(First, other.First);
            int newLast = Math.Min(Last, other.Last);
            return new SectionRange(newFirst, newLast);
        }
    }
    
    internal readonly struct ElfPair
    {
        public readonly SectionRange Left;
        public readonly SectionRange Right;
        
        public ElfPair(string assignment)
        {
            string[] elfs = assignment.Split(',');
            Left = new SectionRange(elfs[0]);
            Right = new SectionRange(elfs[1]);
        }
    }
    
    public class Puzzle
    {
        private readonly IEnumerable<ElfPair> _pairs;
        
        private Puzzle(string inputFile)
        {
            List<string> input = File.ReadAllLines(inputFile).ToList();
            _pairs = input.ConvertAll(s => new ElfPair(s));
        }

        private int ComputeSolution1()
        {
            return _pairs.Count(p => p.Left.FullyContains(p.Right) || p.Right.FullyContains(p.Left));
        }
        
        private int ComputeSolution2()
        {
            return _pairs.Count(p => p.Left.Intersect(p.Right) != null);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day04\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 2)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 4)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day04\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}