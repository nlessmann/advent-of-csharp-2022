namespace Day05
{
    using System.Text.RegularExpressions;
    
    public class Puzzle
    {
        private readonly struct Move
        {
            public readonly int Items;
            public readonly int Origin;
            public readonly int Destination;
            
            public Move(string instructions)
            {
                // Example: move 3 from 1 to 3
                var matches = Regex.Matches(instructions, @"\d+");
                Items = int.Parse(matches[0].Value);
                Origin = int.Parse(matches[1].Value) - 1;
                Destination = int.Parse(matches[2].Value) - 1;
            }
        }
        
        private readonly IEnumerable<Stack<char>> _stacks;
        private readonly IEnumerable<Move> _moves;
        
        private Puzzle(string inputFile)
        {
            var input = File.ReadAllLines(inputFile);
            
            // Find first row without []
            int i = 0;
            while (input[i].Contains('['))
            {
                i++;
            }
            
            // Find last number in this row = number of stacks
            Regex lastNumberExpr = new Regex(@"\s(?<count>\d+)\s*$");
            var match = lastNumberExpr.Match(input[i]);
            int numStacks = int.Parse(match.Groups["count"].Value);
            var stacks = new Stack<char>[numStacks].Select(s => new Stack<char>()).ToArray();
            
            // Parse lines above from the bottom up
            for (int j = i - 1; j >= 0; j--)
            {
                string line = input[j];
                for (int k = 0; k < numStacks; k++)
                {
                    int idx = 1 + 4 * k;  // 1, 5, 9, ..
                    if (line.Length > idx + 1 && !char.IsWhiteSpace(line[idx]))
                    {
                        stacks[k].Push(line[idx]);
                    }
                }
            }

            _stacks = stacks;
            
            // Rest of the file are the moves
            _moves = input.Skip(i + 2).Select(line => new Move(line)).ToArray();
        }

        private Stack<char>[] CloneStacks()
        {
            return _stacks.Select(s => new Stack<char>(s.Reverse())).ToArray();
        }

        private string ComputeSolution1()
        {
            // Create a local copy of the original stacks
            Stack<char>[] stacks = CloneStacks();

            // Perform moves
            foreach (Move move in _moves)
            {
                Stack<char> src = stacks[move.Origin];
                Stack<char> dst = stacks[move.Destination];

                for (int i = 0; i < move.Items && src.Count > 0; i++)
                {
                    dst.Push(src.Pop());
                }
            }
            
            // Return the top-most items of all stacks
            return new string(stacks.Select(s => s.Peek()).ToArray());
        }
        
        private string ComputeSolution2()
        {
            // Create a local copy of the original stacks
            Stack<char>[] stacks = CloneStacks();

            // Perform moves
            foreach (Move move in _moves)
            {
                Stack<char> src = stacks[move.Origin];
                Stack<char> dst = stacks[move.Destination];
                Stack<char> tmp = new Stack<char>();

                // Move items first onto a temporary stack
                for (int i = 0; i < move.Items && src.Count > 0; i++)
                {
                    tmp.Push(src.Pop());
                }
                
                // Move items from temporary stack to destination
                foreach (char item in tmp)
                {
                    dst.Push(item);
                }
            }
            
            // Return the top-most items of all stacks
            return new string(stacks.Select(s => s.Peek()).ToArray());
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day05\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be CMZ)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be MCD)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day05\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}