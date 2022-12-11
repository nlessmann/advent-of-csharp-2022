namespace Day11
{
    internal class Monkey
    {
        private readonly Queue<int> _items = new();
        
        private readonly int _divisibleBy;
        private readonly Func<long, long> _operation;
        
        public Monkey? TrueDestination { get; set; }
        public Monkey? FalseDestination { get; set; }
        
        public long InspectedItems { get; private set; }
        public long ScalingFactor;
        
        public Monkey(int divisibleBy, Func<long, long> operation)
        {
            _divisibleBy = divisibleBy;
            _operation = operation;
        }

        public void AddItem(int value)
        {
            _items.Enqueue(value);
        }

        public void InspectItems()
        {
            if (TrueDestination == null || FalseDestination == null)
                throw new InvalidOperationException("Recipients of items not set");
            
            while (_items.Count > 0)
            {
                int value = _items.Dequeue();
                int newValue = _inspectItem(value);
                
                if (newValue % _divisibleBy == 0)
                {
                    TrueDestination.AddItem(newValue);
                }
                else
                {
                    FalseDestination.AddItem(newValue);
                }

                InspectedItems++;
            }
        }

        private int _inspectItem(int value)
        {
            long v = _operation(value);
            v = ScalingFactor == 0 ? v / 3 : v % ScalingFactor;
            return (int)v;
        }
    }

    public class Puzzle
    {
        private readonly List<Monkey> _monkeys = new();
        
        private Puzzle(string inputFile, bool scale = false)
        {
            var input = File.ReadAllLines(inputFile);

            // First pass: create monkey instances
            List<int> divs = new();
            for (int i = 0; i < input.Length; i += 7)
            {
                int divisibleBy = int.Parse(input[i + 3].Trim().Split(" ").Last());
                divs.Add(divisibleBy);
                
                string op = input[i + 2].Split("=")[1].Trim();
                if (op == "old * old")
                {
                    _monkeys.Add(new Monkey(divisibleBy, v => v * v));
                }
                else if (op.StartsWith("old +"))
                {
                    int value = int.Parse(op.Split(" + ").Last());
                    _monkeys.Add(new Monkey(divisibleBy, v => v + value));
                }
                else
                {
                    int value = int.Parse(op.Split(" * ").Last());
                    _monkeys.Add(new Monkey(divisibleBy, v => v * value));
                }
            }
            
            // Compute scaling factor
            long scalingFactor = divs.Aggregate(1L, (total, value) => total * value);
            
            // Second pass: add items and connections
            for (int m = 0; m < _monkeys.Count; m++)
            {
                int i = m * 7;
                Monkey monkey = _monkeys[m];

                // Set scaling factor
                if (scale)
                {
                    monkey.ScalingFactor = scalingFactor;
                }

                // Add initial items
                var items = input[i + 1].Split(": ").Last().Split(", ").Select(int.Parse);
                foreach (int item in items)
                {
                    monkey.AddItem(item);
                }

                // Set recipients of items
                monkey.TrueDestination = GetRecipient(input[i + 4]);
                monkey.FalseDestination = GetRecipient(input[i + 5]);
            }
        }

        private Monkey GetRecipient(string line)
        {
            int i = int.Parse(line.Split("monkey").Last().Trim());
            return _monkeys[i];
        }

        private long ComputeSolution(int rounds)
        {
            for (int i = 0; i < rounds; i++)
            {
                foreach (Monkey monkey in _monkeys)
                {
                    monkey.InspectItems();
                }
            }

            return _monkeys
                .Select(m => m.InspectedItems)
                .OrderDescending()
                .Take(2)
                .Aggregate(1L, (total, value) => total * value);
        }
        
        public static void Main()
        {
            var test1 = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day11\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test1.ComputeSolution(20)} (should be 10605)");
            
            var test2 = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day11\TestInput.txt", true);
            Console.WriteLine($"Test solution 2: {test2.ComputeSolution(10000)} (should be 2713310158)");
            
            Console.WriteLine("---------------");
            
            var puzzle1 = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day11\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle1.ComputeSolution(20)}");
            
            var puzzle2 = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day11\PuzzleInput.txt", true);
            Console.WriteLine($"Puzzle solution 2: {puzzle2.ComputeSolution(10000)}");
        }
    }
}