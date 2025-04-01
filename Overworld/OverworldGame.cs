using System;
using System.Collections.Generic;
using System.Linq;
using ChessConquest;
using ChessConquest.Pieces;

namespace ChessConquest.Overworld
{
    public class OverworldGame
    {
        private Overworld Overworld { get; set; }
        private Game? ChessGame { get; set; }
        private Territory? SelectedTerritory { get; set; }
        private bool IsGameOver { get; set; }
        private Random random = new Random();

        public OverworldGame()
        {
            Overworld = new Overworld();
            IsGameOver = false;
        }

        public void Initialize()
        {
            // Initialize the overworld with factions and territories
            Overworld = new Overworld();
            Overworld.InitializeSimpleOverworld();
            
            // Select a random faction as the player
            var factions = Overworld.Factions;
            Overworld.PlayerFaction = factions[random.Next(factions.Count)];
            
            // Assign territories to the player faction
            var availableTerritories = Overworld.Territories
                .Where(t => t.Name.Contains("Capital"))
                .ToList();
                
            if (availableTerritories.Count > 0)
            {
                var capital = availableTerritories[random.Next(availableTerritories.Count)];
                Overworld.PlayerFaction?.AddTerritory(capital);
                capital.Owner = Overworld.PlayerFaction;
            }
        }

        public void Start()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            while (!IsGameOver)
            {
                // Display the overworld map
                Overworld.DisplayMap();
                
                // Process player turn
                ProcessPlayerTurn();
                
                // Process AI turns
                Overworld.ProcessAITurns();
                
                // Check for game over conditions
                CheckGameOver();
                
                if (!IsGameOver)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();
                }
            }
        }

        private void ProcessPlayerTurn()
        {
            Console.WriteLine($"\n{Overworld.PlayerFaction?.Name}'s Turn");
            Console.WriteLine("Available actions:");
            Console.WriteLine("1. View controlled territories");
            Console.WriteLine("2. Attack adjacent territory");
            Console.WriteLine("3. View territory details");
            Console.WriteLine("4. View piece formations");
            Console.WriteLine("5. End turn");
            
            while (true)
            {
                Console.Write("\nEnter choice (1-5): ");
                string? input = Console.ReadLine();
                
                if (input == null)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    continue;
                }
                
                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            ViewControlledTerritories();
                            break;
                        case 2:
                            AttackTerritory();
                            return;
                        case 3:
                            ViewTerritoryDetails();
                            break;
                        case 4:
                            ViewFormations();
                            break;
                        case 5:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }

        private void ViewControlledTerritories()
        {
            Console.WriteLine($"\n{Overworld.PlayerFaction?.Name}'s Controlled Territories:");
            foreach (var territory in Overworld.PlayerFaction?.Territories ?? new List<Territory>())
            {
                Console.WriteLine($"- {territory.Name}");
                Console.WriteLine($"  Adjacent Territories:");
                foreach (var adjacent in territory.AdjacentTerritories)
                {
                    Console.WriteLine($"    - {adjacent.Name}");
                }
            }
        }

        private void AttackTerritory()
        {
            Console.WriteLine("\nSelect a territory to attack:");
            
            // Get territories adjacent to player's territories
            var attackableTerritories = new List<Territory>();
            foreach (var territory in Overworld.PlayerFaction?.Territories ?? new List<Territory>())
            {
                attackableTerritories.AddRange(
                    territory.AdjacentTerritories
                    .Where(t => t.Owner != Overworld.PlayerFaction)
                );
            }

            if (attackableTerritories.Count == 0)
            {
                Console.WriteLine("No attackable territories found.");
                return;
            }

            // Display attackable territories
            for (int i = 0; i < attackableTerritories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {attackableTerritories[i].Name}");
            }

            Console.Write("\nEnter choice (1-" + attackableTerritories.Count + "): ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Console.WriteLine("Invalid input. Please try again.");
                return;
            }
            
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= attackableTerritories.Count)
            {
                var targetTerritory = attackableTerritories[choice - 1];
                
                // Find an adjacent territory owned by the player
                var attackingTerritory = Overworld.PlayerFaction?.Territories
                    .FirstOrDefault(t => t.IsAdjacentTo(targetTerritory));
                
                if (attackingTerritory != null)
                {
                    // Start a chess battle
                    ChessGame = new Game();
                    ChessGame.Difficulty = 3; // Medium difficulty
                    
                    Console.WriteLine($"\nStarting battle for {targetTerritory.Name}...");
                    
                    // Main chess game loop
                    while (!ChessGame.IsGameOver)
                    {
                        ChessGame.DisplayGame();
                        
                        if (ChessGame.CurrentTurn == PieceColor.White)
                        {
                            // Player's turn
                            Console.Write("\nEnter move (e.g., e2e4) or 'quit': ");
                            string? move = Console.ReadLine()?.Trim();
                            
                            if (move == null)
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                                continue;
                            }
                            
                            if (move.ToLower() == "quit")
                            {
                                break;
                            }
                            
                            ChessGame.MakeMove(move);
                        }
                        else
                        {
                            // AI's turn
                            Console.WriteLine("\nAI is thinking...");
                            ChessGame.MakeAIMove();
                        }
                    }

                    // Resolve the battle
                    if (ChessGame.IsGameOver)
                    {
                        var winner = ChessGame.GetWinner();
                        if (winner == PieceColor.White)
                        {
                            // Player won
                            if (targetTerritory.Owner != null)
                            {
                                targetTerritory.Owner.RemoveTerritory(targetTerritory);
                            }
                            if (Overworld.PlayerFaction != null)
                            {
                                Overworld.PlayerFaction.AddTerritory(targetTerritory);
                            }
                            targetTerritory.Owner = Overworld.PlayerFaction;
                            Console.WriteLine($"\nCongratulations! You've conquered {targetTerritory.Name}!");
                        }
                        else
                        {
                            Console.WriteLine($"\nThe enemy has defended {targetTerritory.Name}!");
                        }
                    }
                }
            }
        }

        private void ViewTerritoryDetails()
        {
            Console.WriteLine("\nSelect a territory to view:");
            
            // Display all territories
            for (int i = 0; i < Overworld.Territories.Count; i++)
            {
                var territory = Overworld.Territories[i];
                Console.WriteLine($"{i + 1}. {territory.Name} (Owner: {territory.Owner?.Name ?? "None"})");
            }

            Console.Write("\nEnter choice (1-" + Overworld.Territories.Count + "): ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Console.WriteLine("Invalid input. Please try again.");
                return;
            }
            
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= Overworld.Territories.Count)
            {
                var territory = Overworld.Territories[choice - 1];
                Console.WriteLine($"\nDetails for {territory.Name}");
                Console.WriteLine($"Owner: {territory.Owner?.Name ?? "None"}");
                Console.WriteLine("Adjacent Territories:");
                foreach (var adjacent in territory.AdjacentTerritories)
                {
                    Console.WriteLine($"- {adjacent.Name} (Owner: {adjacent.Owner?.Name ?? "None"})");
                }
            }
        }

        private void ViewFormations()
        {
            Console.WriteLine("\nCurrent piece formations:");
            
            // Display formations for each territory
            var playerTerritories = Overworld.PlayerFaction?.Territories ?? new List<Territory>();
            foreach (var territory in playerTerritories)
            {
                Console.WriteLine($"\n{territory.Name}");
                
                // Display pieces in the territory
                Console.WriteLine("Pieces:");
                foreach (var piece in territory.Pieces)
                {
                    Console.WriteLine($"- {piece.GetType().Name} at {piece.Position}");
                }
            }
        }

        private void CheckGameOver()
        {
            // Check if player has lost all territories
            if (Overworld.PlayerFaction?.Territories?.Count == 0)
            {
                Console.WriteLine("\nGame Over! You've lost all your territories.");
                IsGameOver = true;
                return;
            }

            // Check if player has conquered all territories
            if (Overworld.Territories.All(t => t.Owner == Overworld.PlayerFaction))
            {
                Console.WriteLine("\nVictory! You've conquered all territories!");
                IsGameOver = true;
                return;
            }
        }
    }
}
