namespace Day01
{
    public class Puzzle
    {
        private readonly IEnumerable<int> _caloriesPerElf;
        
        private Puzzle(string inputFile)
        {
            // Read puzzle input
            string[] input = File.ReadAllLines(inputFile);
            
            // Accumulate calories per elf
            var caloriesPerElf = new List<int>();
            int elf = 0;
            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    caloriesPerElf.Add(elf);
                    elf = 0;
                }
                else
                {
                    elf += int.Parse(line);
                }
            }
            caloriesPerElf.Add(elf);
            
            // Sort in descending order
            _caloriesPerElf = caloriesPerElf.OrderBy(calories => -calories).ToList();
        }

        private int ComputeSolution1()
        {
            return _caloriesPerElf.First();
        }

        private int ComputeSolution2()
        {
            return _caloriesPerElf.Take(3).Sum();
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day01\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 24000)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 45000)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day01\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}