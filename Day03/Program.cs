using System.Text;

namespace Day03
{
    internal class Rucksack
    {
        private readonly IEnumerable<int> _leftCompartment;
        private readonly IEnumerable<int> _rightCompartment;

        private IEnumerable<int> Priorities => _leftCompartment.Concat(_rightCompartment);

        public Rucksack(string content)
        {
            int n = content.Length / 2;
            _leftCompartment = _parseContentIntoPriorities(content[..n]);
            _rightCompartment = _parseContentIntoPriorities(content[n..]);
        }

        private static IEnumerable<int> _parseContentIntoPriorities(string content)
        {
            byte[] ascii = Encoding.ASCII.GetBytes(content);
            return content.Zip(ascii, (c, p) => char.IsUpper(c) ? p - 38 : p - 96);
        }

        public int DuplicateItemPriority()
        {
            return _leftCompartment.Intersect(_rightCompartment).First();
        }

        public int CommonItemPriority(IEnumerable<Rucksack> others)
        {
            var commonPriorities = others
                .Aggregate(Priorities, (common, rucksack) => common.Intersect(rucksack.Priorities));
            return commonPriorities.First();
        }
    }
    
    public class Puzzle
    {
        private readonly List<Rucksack> _rucksacks;
        
        private Puzzle(string inputFile)
        {
            var input = File.ReadAllLines(inputFile).ToList();
            _rucksacks = input.ConvertAll(content => new Rucksack(content));
        }

        private int ComputeSolution1()
        {
            return _rucksacks.Sum(r => r.DuplicateItemPriority());
        }
        
        private int ComputeSolution2()
        {
            int result = 0;
            for (int i = 0; i < _rucksacks.Count; i += 3)
            {
                result += _rucksacks[i].CommonItemPriority(_rucksacks.GetRange(i + 1, 2));
            }

            return result;
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day03\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 157)");
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day03\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle sSolution 2: {puzzle.ComputeSolution2()}");
        }
    }
}