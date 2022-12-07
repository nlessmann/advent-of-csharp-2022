namespace Day07
{
    internal class Directory
    {
        public const int FileSystemCapacity = 70000000;
        
        private readonly IEnumerable<Directory> _directories;
        private readonly int _fileSize;

        public Directory(Queue<string> terminal)
        {
            var directories = new List<Directory>();
            _fileSize = 0;
            
            while (terminal.Count > 0)
            {
                string cmd = terminal.Dequeue().Trim();
                
                if (cmd == "$ ls" || cmd.StartsWith("dir "))
                    continue;  // we don't really need these lines
                if (cmd == "$ cd ..")
                    break;  // done with the current directory
                
                if (cmd.StartsWith("$ cd "))
                {
                    directories.Add(new Directory(terminal));
                }
                else
                {
                    var s = cmd.Split(' ');
                    _fileSize += int.Parse(s[0]);
                }
            }

            _directories = directories;
        }

        public int Size => _fileSize + _directories.Sum(d => d.Size);
        
        private IEnumerable<int> DirectorySizes()
        {
            yield return Size;
            foreach (Directory dir in _directories)
            {
                foreach (int size in dir.DirectorySizes())
                {
                    yield return size;
                }
            }
        }

        public int SizeOfSmallDirectories()
        {
            return DirectorySizes().Where(size => size <= 100000).Sum();
        }

        public int FreeUpSpace(int requiredSpace)
        {
            int needed = requiredSpace - (FileSystemCapacity - Size);
            return DirectorySizes().Where(size => size >= needed).Min();
        }
    }
    
    public class Puzzle
    {
        private readonly Directory _root;
        
        private Puzzle(string inputFile)
        {
            var input = File.ReadAllLines(inputFile);
            _root = new Directory(new Queue<string>(input));
        }

        private int ComputeSolution1()
        {
            return _root.SizeOfSmallDirectories();
        }
        
        private int ComputeSolution2()
        {
            return _root.FreeUpSpace(30000000);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day07\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 95437)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 24933642)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day07\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}