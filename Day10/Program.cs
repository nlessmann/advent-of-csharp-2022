namespace Day10
{
    internal class Processor
    {
        private int _register = 1;
        private int _cycles = 0;
        private List<char> _screen = new();

        private readonly IEnumerable<int> _operations;

        public Processor(IEnumerable<string> commands)
        {
            Queue<int> offsets = new Queue<int>();
            foreach (string cmd in commands)
            {
                // All commands require one cycle in which they do nothing
                offsets.Enqueue(0);
                
                // The addx command has another cycle after which it changes the register value
                if (cmd.StartsWith("addx "))
                {
                    int offset = int.Parse(cmd.Split(" ")[1]);
                    offsets.Enqueue(offset);
                }
            }

            _operations = offsets;
        }

        public int Run(ISet<int> cyclesOfInterest)
        {
            List<int> signalStrengths = new List<int>();
            foreach (int offset in _operations)
            {
                // Draw if position of sprite matches current cycle
                int pos = _cycles % 40;
                if (pos >= _register - 1 && pos <= _register + 1)
                {
                    _screen.Add('#');
                }
                else
                {
                    _screen.Add('.');
                }
                
                // Perform operation
                _cycles += 1;
                
                if (cyclesOfInterest.Contains(_cycles))
                {
                    signalStrengths.Add(_cycles * _register);
                }
                
                _register += offset;
            }

            return signalStrengths.Sum();
        }

        public void Render()
        {
            foreach (var line in _screen.Chunk(40))
            {
                Console.WriteLine(new string(line));
            }
        }
    }
    
    public class Puzzle
    {
        private readonly Processor _cpu;
        
        private Puzzle(string inputFile)
        {
            var commands = File.ReadAllLines(inputFile);
            _cpu = new Processor(commands);
        }

        private int ComputeSolution1()
        {
            return _cpu.Run(new SortedSet<int>() { 20, 60, 100, 140, 180, 220 });
        }

        private void ComputeSolution2()
        {
            _cpu.Render();
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day10\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 13140)");
            test.ComputeSolution2();
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day10\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            puzzle.ComputeSolution2();
        }
    }
}