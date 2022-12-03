namespace Day02
{
    internal interface IRockPaperScissorsRound
    {
        int Score { get; }
        int AlternativeScore { get; }
    }
    
    internal class RockOrLoose : IRockPaperScissorsRound
    {
        public const int ShapeScore = 1;
        public const int OutcomeScore = 0;

        public int Score { get; }
        public int AlternativeScore { get; }
        
        public RockOrLoose(char opponentShape)
        {
            Score = ShapeScore;
            AlternativeScore = OutcomeScore;
            
            switch (opponentShape)
            {
                case 'A':  // rock
                    Score += PaperOrDraw.OutcomeScore;
                    AlternativeScore += ScissorsOrWin.ShapeScore;
                    break;
                
                case 'B':  // paper
                    Score += RockOrLoose.OutcomeScore;
                    AlternativeScore += RockOrLoose.ShapeScore;
                    break;
                
                case 'C':  // scissors
                    Score += ScissorsOrWin.OutcomeScore;
                    AlternativeScore += PaperOrDraw.ShapeScore;
                    break;
            }
        }
    }
    
    internal class PaperOrDraw : IRockPaperScissorsRound
    {
        public const int ShapeScore = 2;
        public const int OutcomeScore = 3;

        public int Score { get; }
        public int AlternativeScore { get; }
        
        public PaperOrDraw(char opponentShape)
        {
            Score = ShapeScore;
            AlternativeScore = OutcomeScore;
            
            switch (opponentShape)
            {
                case 'A':  // rock
                    Score += ScissorsOrWin.OutcomeScore;
                    AlternativeScore += RockOrLoose.ShapeScore;
                    break;
                
                case 'B':  // paper
                    Score += PaperOrDraw.OutcomeScore;
                    AlternativeScore += PaperOrDraw.ShapeScore;
                    break;
                
                case 'C':  // scissors
                    Score += RockOrLoose.OutcomeScore;
                    AlternativeScore += ScissorsOrWin.ShapeScore;
                    break;
            }
        }
    }
    
    internal class ScissorsOrWin : IRockPaperScissorsRound
    {
        public const int ShapeScore = 3;
        public const int OutcomeScore = 6;

        public int Score { get; }
        public int AlternativeScore { get; }
        
        public ScissorsOrWin(char opponentShape)
        {
            Score = ShapeScore;
            AlternativeScore = OutcomeScore;
            
            switch (opponentShape)
            {
                case 'A':  // rock
                    Score += RockOrLoose.OutcomeScore;
                    AlternativeScore += PaperOrDraw.ShapeScore;
                    break;
                
                case 'B':  // paper
                    Score += ScissorsOrWin.OutcomeScore;
                    AlternativeScore += ScissorsOrWin.ShapeScore;
                    break;
                
                case 'C':  // scissors
                    Score += PaperOrDraw.OutcomeScore;
                    AlternativeScore += RockOrLoose.ShapeScore;
                    break;
            }
        }
    }
    
    public class Puzzle
    {
        private readonly List<IRockPaperScissorsRound> _rounds;
        
        private Puzzle(string inputFile)
        {
            string[] input = File.ReadAllLines(inputFile);
            
            _rounds = new List<IRockPaperScissorsRound>();
            foreach (string line in input)
            {
                char opponent = line[0];
                switch (line[2])
                {
                    case 'X':
                        _rounds.Add(new RockOrLoose(opponent));
                        break;
                    case 'Y':
                        _rounds.Add(new PaperOrDraw(opponent));
                        break;
                    case 'Z':
                        _rounds.Add(new ScissorsOrWin(opponent));
                        break;
                }
            }
        }

        private int ComputeSolution1()
        {
            return _rounds.Sum(round => round.Score);
        }
        
        private int ComputeSolution2()
        {
            return _rounds.Sum(round => round.AlternativeScore);
        }
        
        public static void Main()
        {
            var test = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day02\TestInput.txt");
            Console.WriteLine($"Test solution 1: {test.ComputeSolution1()} (should be 15)");
            Console.WriteLine($"Test solution 2: {test.ComputeSolution2()} (should be 12)");
            
            Console.WriteLine("---------------");
            
            var puzzle = new Puzzle(@"D:\Advent\advent-of-csharp-2022\Day02\PuzzleInput.txt");
            Console.WriteLine($"Puzzle solution 1: {puzzle.ComputeSolution1()}");
            Console.WriteLine($"Puzzle sSolution 2: {puzzle.ComputeSolution2()}");
        }
    }
}