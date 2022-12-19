namespace Day19
{
    using System.Text.RegularExpressions;

    internal readonly record struct RobotCosts(int Ore, int Clay, int Obsidian);

    internal class Blueprint
    {
        private static readonly Regex DigitPattern = new(@"\d+");
        
        public readonly int Identifier;
        public readonly RobotCosts OreRobotCosts;
        public readonly RobotCosts ClayRobotCosts;
        public readonly RobotCosts ObsidianRobotCosts;
        public readonly RobotCosts GeodeRobotCosts;

        public Blueprint(string description)
        {
            var n = DigitPattern.Matches(description).Select(m => int.Parse(m.Value)).ToArray();
            Identifier = n[0];
            OreRobotCosts = new RobotCosts(n[1], 0, 0);
            ClayRobotCosts = new RobotCosts(n[2], 0, 0);
            ObsidianRobotCosts = new RobotCosts(n[3], n[4], 0);
            GeodeRobotCosts = new RobotCosts(n[5], 0, n[6]);
        }

        public IEnumerable<RobotCosts> Costs()
        {
            yield return OreRobotCosts;
            yield return ClayRobotCosts;
            yield return ObsidianRobotCosts;
            yield return GeodeRobotCosts;
        }
    }
    
    internal record Inventory(
        int Ore = 0,
        int OreRobots = 1,  // we start with one ore-mining robot
        int Clay = 0,
        int ClayRobots = 0,
        int Obsidian = 0,
        int ObsidianRobots = 0,
        int Geode = 0,
        int GeodeRobots = 0)
    {
        public Inventory CollectMaterial()
        {
            return this with
            {
                Ore = Ore + OreRobots,
                Clay = Clay + ClayRobots,
                Obsidian = Obsidian + ObsidianRobots,
                Geode = Geode + GeodeRobots
            };
        }

        public bool CanAfford(RobotCosts robotCosts)
        {
            return robotCosts.Ore <= Ore && robotCosts.Clay <= Clay && robotCosts.Obsidian <= Obsidian;
        }

        private Inventory Build(RobotCosts robotCosts)
        {
            return this with
            {
                Ore = Ore - robotCosts.Ore,
                Clay = Clay - robotCosts.Clay,
                Obsidian = Obsidian - robotCosts.Obsidian
            };
        }

        public Inventory BuildOreRobot(RobotCosts robotCosts)
        {
            return Build(robotCosts) with { OreRobots = OreRobots + 1 };
        }
        
        public Inventory BuildClayRobot(RobotCosts robotCosts)
        {
            return Build(robotCosts) with { ClayRobots = ClayRobots + 1 };
        }
        
        public Inventory BuildObsidianRobot(RobotCosts robotCosts)
        {
            return Build(robotCosts) with { ObsidianRobots = ObsidianRobots + 1 };
        }
        
        public Inventory BuildGeodeRobot(RobotCosts robotCosts)
        {
            return Build(robotCosts) with { GeodeRobots = GeodeRobots + 1 };
        }
    }
    
    public class Puzzle
    {
        private readonly Blueprint[] _blueprints;
        private Puzzle(string inputFile)
        {
            _blueprints = File.ReadLines(inputFile).Select(line => new Blueprint(line)).ToArray();
        }

        private static int FindLargestGeodeYield(Blueprint blueprint, int timeLimit)
        {
            int maxOreCost = blueprint.Costs().Max(c => c.Ore);

            Queue<(int Time, Inventory Inventory)> queue = new();
            queue.Enqueue((0, new Inventory()));

            HashSet<Inventory> seen = new();

            int maxGeodes = 0;
            while (queue.TryDequeue(out var state))
            {
                int minutes = state.Time;
                Inventory inventory = state.Inventory;
                
                // Reached the time limit?
                if (minutes >= timeLimit)
                {
                    if (inventory.Geode > maxGeodes)
                    {
                        maxGeodes = inventory.Geode;
                    }
                    continue;
                }
                
                // Seen this state before (i.e. with same or fewer minutes to go)? Won't get to a better result
                // then, so we can stop following this path. Limit the inventory to the maximally needed amount
                // of ore, clay and obsidian to recognize similar states.
                Inventory cappedInventory = inventory with
                {
                    Ore = Math.Min(blueprint.GeodeRobotCosts.Ore * 2, inventory.Ore),
                    Clay = Math.Min(blueprint.ObsidianRobotCosts.Clay, inventory.Clay),
                    Obsidian = Math.Min(blueprint.GeodeRobotCosts.Obsidian, inventory.Obsidian)
                };
                
                if (seen.Contains(cappedInventory))
                {
                    continue;
                }

                seen.Add(cappedInventory);
                
                // Build a new ore mining robot?
                if (inventory.OreRobots < maxOreCost && inventory.CanAfford(blueprint.OreRobotCosts))
                {
                    queue.Enqueue((minutes + 1, inventory.CollectMaterial().BuildOreRobot(blueprint.OreRobotCosts)));
                }
                
                // Build a new clay mining robot?
                if (inventory.ClayRobots < blueprint.ObsidianRobotCosts.Clay && inventory.CanAfford(blueprint.ClayRobotCosts))
                {
                    queue.Enqueue((minutes + 1, inventory.CollectMaterial().BuildClayRobot(blueprint.ClayRobotCosts)));
                }
                
                // Build a new obsidian mining robot?
                if (inventory.ObsidianRobots < blueprint.GeodeRobotCosts.Obsidian && inventory.CanAfford(blueprint.ObsidianRobotCosts))
                {
                    queue.Enqueue((minutes + 1, inventory.CollectMaterial().BuildObsidianRobot(blueprint.ObsidianRobotCosts)));
                }
                
                // Build a new geode mining robot?
                if (inventory.CanAfford(blueprint.GeodeRobotCosts))
                {
                    queue.Enqueue((minutes + 1, inventory.CollectMaterial().BuildGeodeRobot(blueprint.GeodeRobotCosts)));
                }
                else
                {
                    // Buying a geode mining robot is always best, but if we can't afford one right now,
                    // we wait one round and just mine more materials
                    queue.Enqueue((minutes + 1, inventory.CollectMaterial()));
                }
            }
            
            return maxGeodes;
        }

        private int ComputeSolution1()
        {
            return _blueprints
                .Select(blueprint => blueprint.Identifier * FindLargestGeodeYield(blueprint, 24))
                .Sum();
        }

        private int ComputeSolution2()
        {
            return _blueprints
                .Take(3)
                .Select(blueprint => FindLargestGeodeYield(blueprint, 32))
                .Aggregate(1, (t, v) => t * v);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day19\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 33)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be {56 * 62})");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day19\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}