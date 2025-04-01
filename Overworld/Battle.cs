using System;
using ChessConquest.Pieces;

namespace ChessConquest.Overworld
{
    public class Battle
    {
        public Faction AttackingFaction { get; }
        public Faction DefendingFaction { get; }
        public Territory DisputedTerritory { get; }
        public Game? ChessGame { get; private set; }
        
        public Battle(Faction attacker, Faction defender, Territory disputedTerritory)
        {
            AttackingFaction = attacker;
            DefendingFaction = defender;
            DisputedTerritory = disputedTerritory;
        }
        
        /// <summary>
        /// Initialize the chess game for this battle
        /// </summary>
        public void InitializeChessGame(string attackerFormation = "Standard", string defenderFormation = "Standard")
        {
            // Determine if player is attacking or defending
            bool isPlayerAttacking = AttackingFaction.Name.Contains("Player");
            
            // Set up the game with the appropriate player color
            PieceColor playerColor = isPlayerAttacking ? PieceColor.White : PieceColor.Black;
            ChessGame = new Game(playerColor);
            
            // Get the appropriate board formations
            Board attackerBoard = AttackingFaction.GetFormation(attackerFormation);
            Board defenderBoard = DefendingFaction.GetFormation(defenderFormation);
            
            // Combine the formations into a single board
            // For now, we'll just use one of the formations since we don't have custom formations yet
            ChessGame.Board = isPlayerAttacking ? attackerBoard : defenderBoard;
        }
        
        /// <summary>
        /// Start the chess battle
        /// </summary>
        public void StartBattle()
        {
            Console.Clear();
            Console.WriteLine($"Battle for {DisputedTerritory.Name}");
            Console.WriteLine($"{AttackingFaction.Name} vs {DefendingFaction.Name}");
            Console.WriteLine("=============================\n");
            
            // Initialize the chess game
            InitializeChessGame();
            
            // Display the game
            if (ChessGame != null)
            {
                ChessGame.DisplayGame();
            }
            
        }
        
        /// <summary>
        /// Get the winner of the battle
        /// </summary>
        public Faction GetWinner()
        {
            if (ChessGame == null)
            {
                // If no chess game was played, randomly determine winner
                return new Random().Next(2) == 0 ? AttackingFaction : DefendingFaction;
            }
            
            // Get the winner from the chess game
            PieceColor? winner = ChessGame.GetWinner();
            
            if (winner == PieceColor.White)
            {
                return AttackingFaction; // Attacker is always white
            }
            else
            {
                return DefendingFaction; // Defender is always black
            }
        }
        
        /// <summary>
        /// Process the result of the battle
        /// </summary>
        public Faction ResolveBattleOutcome(bool playerWon)
        {
            Faction winner;
            
            if (AttackingFaction.Name.Contains("Player"))
            {
                winner = playerWon ? AttackingFaction : DefendingFaction;
            }
            else
            {
                winner = playerWon ? DefendingFaction : AttackingFaction;
            }
            
            // Update territory ownership
            if (winner == AttackingFaction)
            {
                // Attacker won
                DefendingFaction.RemoveTerritory(DisputedTerritory);
                AttackingFaction.AddTerritory(DisputedTerritory);
                
                Console.WriteLine($"\n{AttackingFaction.Name} has conquered {DisputedTerritory.Name}!");
            }
            else
            {
                // Defender won
                Console.WriteLine($"\n{DefendingFaction.Name} has successfully defended {DisputedTerritory.Name}!");
            }
            
            return winner;
        }
    }
}
