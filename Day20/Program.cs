namespace Day20
{
    internal class Decryptor
    {
        private class Item
        {
            public readonly long Value;
            
            public Item? PreviousItem;
            public Item? NextItem;

            public Item(long value)
            {
                Value = value;
            }
        }

        private readonly List<Item> _items = new();
        private readonly Item _null = new(0);
        
        public Decryptor(string filename, long key = 1)
        {
            // Read in values and create wrapper objects
            foreach (string line in File.ReadLines(filename))
            {
                int value = int.Parse(line.Trim());
                _items.Add(value == 0 ? _null : new Item(value * key));
            }
            
            // Add connections between the items
            _items[0].PreviousItem = _items[^1];
            _items[^1].NextItem = _items[0];
            
            for (int i = 1; i < _items.Count; i++)
            {
                _items[i].PreviousItem = _items[i - 1];
            }
            for (int i = 0; i < _items.Count - 1; i++)
            {
                _items[i].NextItem = _items[i + 1];
            }
        }

        public void Mix()
        {
            foreach (Item item in _items)
            {
                // No need to do anything with the 0 item
                if (item == _null) continue;
                
                // Check linkage
                if (item.PreviousItem == null || item.NextItem == null)
                {
                    throw new InvalidOperationException("Broken linkage");
                }
                
                // Remove item from list
                Item previous = item.PreviousItem;
                Item next = item.NextItem;
                previous.NextItem = next;
                next.PreviousItem = previous;
                
                // Find place to insert the item again
                Item neighbor = item;
                int steps = (int)(Math.Abs(item.Value) % (_items.Count - 1));
                
                if (item.Value > 0)
                {
                    for (int i = 0; i < steps; i++)
                    {
                        neighbor = neighbor.NextItem ?? throw new InvalidOperationException("Broken linkage");
                    }
                }
                else
                {
                    for (int i = 0; i <= steps; i++)
                    {
                        neighbor = neighbor.PreviousItem ?? throw new InvalidOperationException("Broken linkage");
                    }
                }

                // Insert the item
                item.PreviousItem = neighbor;
                item.NextItem = neighbor.NextItem ?? throw new InvalidOperationException("Broken linkage");
                item.PreviousItem.NextItem = item;
                item.NextItem.PreviousItem = item;
            }
        }

        public long GroveCoordinates()
        {
            long sum = 0;
            
            Item item = _null;
            for (int i = 1; i <= 3000; i++)
            {
                item = item.NextItem ?? throw new InvalidOperationException("List linkage is broken");
                if (i % 1000 == 0)
                {
                    sum += item.Value;
                }
            }

            return sum;
        }
    }
    
    public class Puzzle
    {
        private readonly string _inputFile;
        
        private Puzzle(string inputFile)
        {
            _inputFile = inputFile;
        }

        private long ComputeSolution1()
        {
            Decryptor decryptor = new Decryptor(_inputFile);
            decryptor.Mix();
            
            return decryptor.GroveCoordinates();
        }

        private long ComputeSolution2()
        {
            Decryptor decryptor = new Decryptor(_inputFile, 811589153);
            for (int i = 0; i < 10; i++)
            {
                decryptor.Mix();
            }

            return decryptor.GroveCoordinates();
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day20\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 3)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 1623178306)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day20\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}