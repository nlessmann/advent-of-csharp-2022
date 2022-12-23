namespace Day22
{
    using System.Text.RegularExpressions;
    
    internal class Map
    {
        private enum Tile
        {
            Void,
            Open,
            Wall
        }

        private enum Facing
        {
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3
        }

        private readonly record struct Position(int Row, int Column, Facing Facing);

        private readonly int _width;
        private readonly int _height;
        private readonly Tile[,] _map;

        private Position _position;

        public Map(IReadOnlyList<string> scan)
        {
            // Construct map from scan
            _width = scan.Max(s => s.Length);
            _height = scan.Count;
            _map = new Tile[_height, _width];

            bool startingPositionFound = false;

            for (int row = 0; row < _height; row++)
            {
                string line = scan[row];
                for (int col = 0; col < line.Length; col++)
                {
                    _map[row, col] = line[col] switch
                    {
                        '.' => Tile.Open,
                        '#' => Tile.Wall,
                        _ => Tile.Void
                    };

                    if (!startingPositionFound && _map[row, col] == Tile.Open)
                    {
                        _position = new Position(row, col, Facing.Right);
                        startingPositionFound = true;
                    }
                }
            }
        }

        public int FollowPath(string path)
        {
            // Split parse into individual moves
            string[] moves = Regex.Matches(path, @"(\d+|R|L)").Select(m => m.Value).ToArray();

            // Perform all moves
            foreach (string move in moves)
            {
                if (move == "R")
                {
                    Facing f = _position.Facing;
                    _position = _position with
                    {
                        Facing = f == Facing.Up ? Facing.Right : f + 1
                    };
                }
                else if (move == "L")
                {
                    Facing f = _position.Facing;
                    _position = _position with
                    {
                        Facing = f == Facing.Right ? Facing.Up : f - 1
                    };
                }
                else
                {
                    int steps = int.Parse(move);
                    for (int i = 0; i < steps; i++)
                    {
                        Position candidate = _position.Facing switch
                        {
                            Facing.Right => TileOnTheRight(),
                            Facing.Down => TileBelow(),
                            Facing.Left => TileOnTheLeft(),
                            Facing.Up => TileAbove(),
                            _ => throw new ArgumentException("Invalid path")
                        };
                        
                        // Are we standing in front of a wall?
                        if (_map[candidate.Row, candidate.Column] == Tile.Wall)
                        {
                            break;
                        }

                        _position = candidate;
                    }
                }
            }
            
            // Compute score = final password
            return 1000 * (_position.Row + 1) + 4 * (_position.Column + 1) + (int)_position.Facing;
        }

        private Position TileOnTheRight()
        {
            int col = _position.Column + 1;
            if (col >= _width || _map[_position.Row, col] == Tile.Void)
            {
                for (int c = 0; c < col; c++)
                {
                    if (_map[_position.Row, c] != Tile.Void)
                    {
                        col = c;
                        break;
                    }
                }
            }

            return _position with { Column = col };
        }

        private Position TileOnTheLeft()
        {
            int col = _position.Column - 1;
            if (col < 0 || _map[_position.Row, col] == Tile.Void)
            {
                for (int c = _width - 1; c > col; c--)
                {
                    if (_map[_position.Row, c] != Tile.Void)
                    {
                        col = c;
                        break;
                    }
                }
            }

            return _position with { Column = col };
        }

        private Position TileBelow()
        {
            int row = _position.Row + 1;
            if (row >= _height || _map[row, _position.Column] == Tile.Void)
            {
                for (int r = 0; r < row; r++)
                {
                    if (_map[r, _position.Column] != Tile.Void)
                    {
                        row = r;
                        break;
                    }
                }
            }

            return _position with { Row = row };
        }

        private Position TileAbove()
        {
            int row = _position.Row - 1;
            if (row < 0 || _map[row, _position.Column] == Tile.Void)
            {
                for (int r = _height - 1; r > row; r--)
                {
                    if (_map[r, _position.Column] != Tile.Void)
                    {
                        row = r;
                        break;
                    }
                }
            }

            return _position with { Row = row };
        }
    }
    
    public class Puzzle
    {
        private readonly Map _map;
        private readonly string _path;
        
        private Puzzle(string inputFile)
        {
            string[] input = File.ReadAllLines(inputFile);
            _map = new Map(input.TakeWhile(line => line.Trim() != "").ToArray());
            _path = input[^1].Trim();
        }

        private int ComputeSolution1()
        {
            return _map.FollowPath(_path);
        }

        private int ComputeSolution2()
        {
            return 0;
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day22\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 6032)");
            //Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be ?)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day22\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            //Console.WriteLine($"Puzzle solution 2: {puzzle.ComputeSolution2()}");
        }
    }
}