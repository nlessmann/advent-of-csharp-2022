namespace Day06
{
    internal class Message
    {
        private readonly string _signal;
        
        public Message(string signal)
        {
            _signal = signal.Trim();
        }

        public int FindDistinctCharacterBlock(int size)
        {
            Queue<char> window = new Queue<char>();
            foreach (char c in _signal.Take(size))
            {
                window.Enqueue(c);
            }

            int position = size;
            while (window.Distinct().Count() != size)
            {
                if (position >= _signal.Length)
                    return -1;  // reached end of signal without finding block
                
                window.Dequeue();
                window.Enqueue(_signal[position++]);
            }
            
            return position;
        }
    }
    
    public class Puzzle
    {
        private readonly Message _message;
        
        private Puzzle(string inputFile)
        {
            var input = File.ReadAllLines(inputFile);
            _message = new Message(input[0]);
        }

        private int ComputeSolution1()
        {
            return _message.FindDistinctCharacterBlock(4);
        }
        
        private int ComputeSolution2()
        {
            return _message.FindDistinctCharacterBlock(14);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day06\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 11)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 26)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day06\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}