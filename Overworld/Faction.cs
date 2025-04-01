using System;
using System.Collections.Generic;
using ChessConquest.Pieces;

namespace ChessConquest.Overworld
{
    public class Faction
    {
        public string Name { get; }
        public ConsoleColor Color { get; }
        public List<Territory> Territories { get; }
        public Dictionary<string, Func<Board>> Formations { get; }
        
        public Faction(string name, ConsoleColor color)
        {
            Name = name;
            Color = color;
            Territories = new List<Territory>();
            Formations = new Dictionary<string, Func<Board>>();
            
            // Add default formation
            Formations.Add("Standard", CreateStandardFormation);
        }
        
        /// <summary>
        /// Creates a standard chess board formation
        /// </summary>
        private Board CreateStandardFormation()
        {
            Board board = new();
            board.SetupStandardBoard();
            return board;
        }
        
        /// <summary>
        /// Add a new formation to this faction
        /// </summary>
        public void AddFormation(string name, Func<Board> formationSetup)
        {
            Formations[name] = formationSetup;
        }
        
        /// <summary>
        /// Get a board with the specified formation
        /// </summary>
        public Board GetFormation(string formationName = "Standard")
        {
            if (Formations.TryGetValue(formationName, out var formationFunc))
            {
                return formationFunc();
            }
            
            // Fallback to standard formation
            return CreateStandardFormation();
        }
        
        /// <summary>
        /// Add a territory to this faction
        /// </summary>
        public void AddTerritory(Territory territory)
        {
            Territories.Add(territory);
            territory.Owner = this;
        }
        
        /// <summary>
        /// Remove a territory from this faction
        /// </summary>
        public void RemoveTerritory(Territory territory)
        {
            Territories.Remove(territory);
            territory.Owner = null;
        }
    }
}
