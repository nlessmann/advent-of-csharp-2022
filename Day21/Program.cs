namespace Day21
{
    using System.Text.RegularExpressions;

    internal interface IMonkey
    {
        public long Yell();
        public bool DependsOnHuman();
    }

    internal class YellingMonkey : IMonkey
    {
        private readonly long _value;
        
        public YellingMonkey(long value)
        {
            _value = value;
        }

        public long Yell()
        {
            return _value;
        }

        public bool DependsOnHuman()
        {
            return false;
        }
    }

    internal class CalculatingMonkey : IMonkey
    {
        private const string Human = "humn";
        private static readonly Regex Equation = new(@"([a-z]+)\s([+\-\*/])\s([a-z]+)", RegexOptions.Compiled);

        private readonly char _operator;
        private readonly string _monkeyName1;
        private readonly string _monkeyName2;
        
        private readonly IReadOnlyDictionary<string, IMonkey> _monkeys;

        private IMonkey Monkey1 => _monkeys[_monkeyName1];
        private IMonkey Monkey2 => _monkeys[_monkeyName2];

        public CalculatingMonkey(string equation, IReadOnlyDictionary<string, IMonkey> monkeys)
        {
            Match match = Equation.Match(equation);
            if (!match.Success) throw new ArgumentException("Malformed equation");

            _operator = match.Groups[2].Value[0];
            _monkeyName1 = match.Groups[1].Value;
            _monkeyName2 = match.Groups[3].Value;
            _monkeys = monkeys;
        }
        
        public long Yell()
        {
            long val1 = Monkey1.Yell();
            long val2 = Monkey2.Yell();
            return _operator switch
            {
                '+' => val1 + val2,
                '-' => val1 - val2,
                '*' => val1 * val2,
                '/' => val1 / val2,
                _ => throw new InvalidOperationException("Illegal operator")
            };
        }
        
        public bool DependsOnHuman()
        {
            if (_monkeyName1 == Human || _monkeyName2 == Human) return true;
            return Monkey1.DependsOnHuman() || Monkey2.DependsOnHuman();
        }

        public long SolveForHuman()
        {
            if (Monkey1.DependsOnHuman() || _monkeyName1 == Human)
            {
                long value = Monkey2.Yell();
                return Monkey1 is CalculatingMonkey monkey ? monkey.SolveForHuman(value) : value;
            }
            else
            {
                long value = Monkey1.Yell();
                return Monkey2 is CalculatingMonkey monkey ? monkey.SolveForHuman(value) : value;
            }
        }

        private long SolveForHuman(long value)
        {
            if (Monkey1.DependsOnHuman() || _monkeyName1 == Human)
            {
                long otherValue = Monkey2.Yell();
                long newValue = _operator switch
                {
                    '+' => value - otherValue,
                    '-' => otherValue + value,
                    '*' => value / otherValue,
                    '/' => value * otherValue,
                    _ => throw new InvalidOperationException("Illegal operator")
                };

                return Monkey1 is CalculatingMonkey monkey ? monkey.SolveForHuman(newValue) : newValue;
            }
            else
            {
                long otherValue = Monkey1.Yell();
                long newValue = _operator switch
                {
                    '+' => value - otherValue,
                    '-' => otherValue - value,
                    '*' => value / otherValue,
                    '/' => otherValue / value,
                    _ => throw new InvalidOperationException("Illegal operator")
                };
                
                return Monkey2 is CalculatingMonkey monkey ? monkey.SolveForHuman(newValue) : newValue;
            }
        }
    }
    
    public class Puzzle
    {
        private static readonly Regex Digits = new(@"^\d+$");
        
        private readonly Dictionary<string, IMonkey> _monkeys = new();
        private readonly CalculatingMonkey _root;
        
        private Puzzle(string inputFile)
        {
            CalculatingMonkey? root = null;
            
            foreach (string line in File.ReadLines(inputFile))
            {
                string[] parts = line.Split(": ");
                string name = parts[0];
                string equation = parts[1];

                if (Digits.IsMatch(equation))
                {
                    _monkeys.Add(name, new YellingMonkey(int.Parse(equation)));
                }
                else
                {
                    CalculatingMonkey monkey = new CalculatingMonkey(equation, _monkeys);
                    _monkeys.Add(name, monkey);

                    if (name == "root")
                    {
                        root = monkey;
                    }
                }
            }

            _root = root ?? throw new ArgumentException("The input does not contain a root monkey");
            if (!root.DependsOnHuman()) throw new ArgumentException("The input does not contain a human");
        }

        private long ComputeSolution1()
        {
            return _root.Yell();
        }

        private long ComputeSolution2()
        {
            return _root.SolveForHuman();
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day21\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 152)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 301)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day21\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}