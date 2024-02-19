using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemGame
{
    // Represents the position on the board
    class Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Represents a player
    class Player
    {
        public string Name { get; }
        public Position Position { get; set; }
        public int GemCount { get; set; }

        public Player(string name, Position position)
        {
            Name = name;
            Position = position;
            GemCount = 0;
        }

        public void Move(char direction)
        {
            switch (direction)
            {
                case 'U':
                    Position = new Position(Position.X, Position.Y - 1);
                    break;
                case 'D':
                    Position = new Position(Position.X, Position.Y + 1);
                    break;
                case 'L':
                    Position = new Position(Position.X - 1, Position.Y);
                    break;
                case 'R':
                    Position = new Position(Position.X + 1, Position.Y);
                    break;
                default:
                    break;
            }
        }
    }

    // Represents a cell on the board
    class Cell
    {
        public string Occupant { get; set; }

        public Cell(string occupant = "-")
        {
            Occupant = occupant;
        }
    }

    // Represents the game board
    class Board
    {
        private readonly Cell[,] grid;
        private readonly Player player1;
        private readonly Player player2;
        private readonly Random random;

        public Board()
        {
            grid = new Cell[6, 6];
            player1 = new Player("P1", new Position(0, 0));
            player2 = new Player("P2", new Position(5, 5));
            random = new Random();

            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    grid[x, y] = new Cell();
                }
            }

            // Place obstacles
            for (int i = 0; i < 6; i++)
            {
                int x = random.Next(6);
                int y = random.Next(6);
                grid[x, y].Occupant = "O";
            }

            // Place gems
            for (int i = 0; i < 6; i++)
            {
                int x = random.Next(6);
                int y = random.Next(6);
                if (grid[x, y].Occupant == "-")
                {
                    grid[x, y].Occupant = "G";
                }
            }
        }

        public void Display(Player player1, Player player2)
        {
            Console.WriteLine("Gems Hero - Collect the most gems!");
            Console.WriteLine("Use (UP,DOWN,LEFT,RIGHT) to move.");
            Console.WriteLine();

            Console.WriteLine("*******************");
                               
            for (int y = 0; y < 6; y++)
            {
                Console.Write("|  ");
                for (int x = 0; x < 6; x++)
                {
                    if (player1.Position.X == x && player1.Position.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("P1");
                        Console.ResetColor();
                    }
                    else if (player2.Position.X == x && player2.Position.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("P2");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(grid[x, y].Occupant + " ");
                    }
                }
                Console.WriteLine("   |");
            }

            Console.WriteLine("*******************");
        }

        public bool IsValidMove(Player player, char direction)
        {
            int newX = player.Position.X;
            int newY = player.Position.Y;

            switch (direction)
            {
                case 'U':
                    newY--;
                    break;
                case 'D':
                    newY++;
                    break;
                case 'L':
                    newX--;
                    break;
                case 'R':
                    newX++;
                    break;
                default:
                    return false;
            }

            if (newX < 0 || newX >= 6 || newY < 0 || newY >= 6)
                return false;

            if (grid[newX, newY].Occupant == "O")
                return false;

            return true;
        }

        public void CollectGem(Player player)
        {
            int x = player.Position.X;
            int y = player.Position.Y;

            if (grid[x, y].Occupant == "G")
            {
                player.GemCount++;
                grid[x, y].Occupant = "-";
            }
        }

        public bool AreGemsLeft()
        {
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    if (grid[x, y].Occupant == "G")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Player GetWinner()
        {
            if (!AreGemsLeft())
            {
                if (player1.GemCount > player2.GemCount)
                    return player1;
                else if (player1.GemCount < player2.GemCount)
                    return player2;
                else
                    return null;
            }
            return null;
        }

        public Player Player1 => player1;
        public Player Player2 => player2;
    }

    // Represents the game
    class Game
    {
        private readonly Board board;
        private readonly Player player1;
        private readonly Player player2;
        private Player currentTurn;
        private int totalTurns;
        private int player1Turns;
        private int player2Turns;

        public Game()
        {
            board = new Board();
            player1 = board.Player1;
            player2 = board.Player2;
            currentTurn = player1;
            totalTurns = 0;
            player1Turns = 0;
            player2Turns = 0;
        }

        public void Start()
        {
            while (board.AreGemsLeft())
            {
                Console.Clear();

                Console.WriteLine();
                board.Display(player1, player2);
                Console.WriteLine();
                Console.WriteLine($"Total Turn: {totalTurns}");
                // Uncomment for display total moves and gems
                //Console.WriteLine($"Player 1 Gems: {player1.GemCount} (Turns: {player1Turns}) | Player 2 Gems: {player2.GemCount} (Turns: {player2Turns})");
                Console.WriteLine();
                Console.Write($"Player {currentTurn.Name} move (U,D,L,R):");

                char direction = GetInput();
                if (board.IsValidMove(currentTurn, direction))
                {
                    currentTurn.Move(direction);
                    board.CollectGem(currentTurn);
                    totalTurns++;
                    if (currentTurn == player1)
                        player1Turns++;
                    else
                        player2Turns++;
                    SwitchTurn();
                }
                else
                {
                    Console.WriteLine("Invalid move! Try again.");
                }
            }

            Player winner = board.GetWinner();
            if (winner != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n\n Player {winner.Name} wins with {winner.GemCount} gems!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n\n It's a tie !");
            }

            Console.WriteLine("\nPress any key to close the program...");
            Console.ReadKey();
        }

        private char GetInput()
        {
            ConsoleKeyInfo key = Console.ReadKey();
            return char.ToUpper(key.KeyChar);
        }

        private void SwitchTurn()
        {
            currentTurn = (currentTurn == player1) ? player2 : player1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}
