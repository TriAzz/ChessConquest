using System;
using System.Collections.Generic;
using ChessConquest.Pieces;

namespace ChessConquest.Overworld
{
    // Define a new implementation of MapPosition directly in this file
    public struct MapPosition
    {
        public int X { get; }
        public int Y { get; }

        public MapPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            if (obj is MapPosition other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public class Territory
    {
        public string Name { get; }
        public MapPosition Position { get; }
        public Faction? Owner { get; set; }
        public List<Territory> AdjacentTerritories { get; }
        public List<Piece> Pieces { get; private set; }
        
        public Territory(string name, MapPosition position)
        {
            Name = name;
            Position = position;
            AdjacentTerritories = new List<Territory>();
            Pieces = new List<Piece>();
            Owner = null;
        }
        
        /// <summary>
        /// Add a piece to this territory
        /// </summary>
        public void AddPiece(Piece piece)
        {
            if (!Pieces.Contains(piece))
            {
                Pieces.Add(piece);
            }
        }
        
        /// <summary>
        /// Remove a piece from this territory
        /// </summary>
        public void RemovePiece(Piece piece)
        {
            if (Pieces.Contains(piece))
            {
                Pieces.Remove(piece);
            }
        }
        
        /// <summary>
        /// Add an adjacent territory
        /// </summary>
        public void AddAdjacentTerritory(Territory territory)
        {
            if (!AdjacentTerritories.Contains(territory))
            {
                AdjacentTerritories.Add(territory);
                // Avoid infinite recursion by checking if the territory already has this as adjacent
                if (!territory.AdjacentTerritories.Contains(this))
                {
                    territory.AddAdjacentTerritory(this);
                }
            }
        }
        
        /// <summary>
        /// Check if this territory is adjacent to another
        /// </summary>
        public bool IsAdjacentTo(Territory territory)
        {
            return AdjacentTerritories.Contains(territory);
        }
        
        /// <summary>
        /// Get the display color for this territory
        /// </summary>
        public ConsoleColor GetDisplayColor()
        {
            return Owner?.Color ?? ConsoleColor.Gray;
        }
        
        public override string ToString()
        {
            return $"{Name} {Position}";
        }
    }
}
