namespace Day25
{
    public class Puzzle
    {
        private readonly IEnumerable<string> _snafus;

        private static long SnafuToInt(string snafu)
        {
            if (snafu is "" or "0") return 0;
            
            string remainingDigits = snafu.Remove(snafu.Length - 1);
            long remainder = snafu[^1] switch
            {
                '=' => -2,
                '-' => -1,
                '0' => 0,
                '1' => 1,
                '2' => 2,
                _ => throw new ArgumentException("Invalid SNAFU value")
            };
            
            return SnafuToInt(remainingDigits) * 5 + remainder;
        }
        
        private static string IntToSnafu(long value)
        {
            if (value == 0) return "";

            long quotient = Math.DivRem(value + 2, 5, out long remainder);
            char lastDigit = remainder switch
            {
                0 => '=',
                1 => '-',
                2 => '0',
                3 => '1',
                4 => '2',
                _ => throw new ArgumentException("Invalid SNAFU value")
            };

            return IntToSnafu(quotient) + lastDigit;
        }
        
        private Puzzle(string inputFile)
        {
            _snafus = File.ReadAllLines(inputFile);
        }

        private string ComputeSolution1()
        {
            long solution = _snafus.Select(SnafuToInt).Sum();
            return IntToSnafu(solution);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day25\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be ?)");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day25\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
        }
    }
}