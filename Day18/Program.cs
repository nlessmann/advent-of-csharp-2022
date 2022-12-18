namespace Day18
{
    internal readonly record struct Point(int X, int Y, int Z)
    {
        public IEnumerable<Point> Neighbors()
        {
            yield return this with { X = X - 1 };
            yield return this with { X = X + 1 };
            yield return this with { Y = Y - 1 };
            yield return this with { Y = Y + 1 };
            yield return this with { Z = Z - 1 };
            yield return this with { Z = Z + 1 };
        }
    }

    internal class VoxelGrid
    {
        private enum VoxelContent : ushort
        {
            Empty, Lava, Steam, Air
        }
        
        private readonly VoxelContent[,,] _voxels;
        private readonly int _width;
        private readonly int _height;
        private readonly int _depth;

        private VoxelContent this[Point p]
        {
            get => _voxels[p.X, p.Y, p.Z];
            set => _voxels[p.X, p.Y, p.Z] = value;
        } 

        public VoxelGrid(string filename)
        {
            // Read in all cubes and determine bounding box (assuming 0,0,0 to be the min)
            List<Point> cubes = new();
            foreach (string line in File.ReadLines(filename))
            {
                var values = line.Trim().Split(",").Select(int.Parse).ToArray();
                cubes.Add(new Point(values[0], values[1], values[2]));
            }

            // Add a margin of 1 voxel in all directions
            _width = cubes.Max(p => p.X) + 1 + 2;
            _height = cubes.Max(p => p.Y) + 1 + 2;
            _depth = cubes.Max(p => p.Z) + 1 + 2;
            
            // Create 3D grid and fill with lava
            _voxels = new VoxelContent[_width, _height, _depth];
            foreach (Point p in cubes)
            {
                _voxels[p.X + 1, p.Y + 1, p.Z + 1] = VoxelContent.Lava;
            }
        }

        private IEnumerable<Point> Voxels()
        {
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
            for (int z = 0; z < _depth; z++)
                yield return new Point(x, y, z);
        }

        private bool Contains(Point voxel)
        {
            if (voxel.X < 0 || voxel.X >= _width) return false;
            if (voxel.Y < 0 || voxel.Y >= _height) return false;
            if (voxel.Z < 0 || voxel.Z >= _depth) return false;
            return true;
        }

        public int CountSurfaceArea()
        {
            int count = 0;
            foreach (Point p in Voxels())
            {
                if (this[p] != VoxelContent.Lava) continue;

                foreach (Point n in p.Neighbors())
                {
                    if (!Contains(n) || this[n] == VoxelContent.Empty || this[n] == VoxelContent.Steam)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void FillAirPockets()
        {
            // Fill with steam - we added some margin around the grid so that
            // we know now for sure that 0,0,0 is an empty voxel from which we
            // can start to run region growing
            Stack<Point> stack = new();
            stack.Push(new Point(0, 0, 0));

            while (stack.Count > 0)
            {
                Point p = stack.Pop();
                this[p] = VoxelContent.Steam;

                foreach (Point n in p.Neighbors())
                {
                    if (Contains(n) && this[n] == VoxelContent.Empty)
                    {
                        stack.Push(n);
                    }
                }
            }

            // Remaining empty voxels have to be air
            foreach (Point p in Voxels())
            {
                if (this[p] == VoxelContent.Empty)
                {
                    this[p] = VoxelContent.Air;
                }
            }
        }
    }
    
    public class Puzzle
    {
        private readonly VoxelGrid _grid;
        
        private Puzzle(string inputFile)
        {
            _grid = new VoxelGrid(inputFile);
        }

        private int ComputeSolution1()
        {
            return _grid.CountSurfaceArea();
        }

        private int ComputeSolution2()
        {
            _grid.FillAirPockets();
            return _grid.CountSurfaceArea();
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day18\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 64)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 58)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day18\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}