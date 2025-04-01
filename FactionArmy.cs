using System;
using System.Collections.Generic;
using ChessConquest;
using ChessConquest.Pieces;

namespace ChessConquestGUI
{
    /// <summary>
    /// Manages the chess piece lineup for each faction
    /// </summary>
    public class FactionArmy
    {
        private readonly Dictionary<string, Func<PieceColor, List<Piece>>> armySetups;
        
        public FactionArmy()
        {
            // Initialize the dictionary of army setups
            armySetups = new Dictionary<string, Func<PieceColor, List<Piece>>>
            {
                { "Iron Legion", GetStandardArmy },
                { "Crimson Order", GetStandardArmy },
                { "Emerald Covenant", GetStandardArmy },
                { "Golden Dynasty", GetStandardArmy },
                { "Shadow Collective", GetStandardArmy }
            };
        }
        
        /// <summary>
        /// Get the piece lineup for a specific faction
        /// </summary>
        /// <param name="factionName">Name of the faction</param>
        /// <param name="color">Color of the pieces (White or Black)</param>
        /// <returns>List of pieces for the faction's army</returns>
        public List<Piece> GetFactionArmy(string factionName, PieceColor color)
        {
            if (armySetups.TryGetValue(factionName, out var setupFunc))
            {
                return setupFunc(color);
            }
            
            // Default to standard army if faction not found
            return GetStandardArmy(color);
        }
        
        /// <summary>
        /// Standard chess piece lineup used by default
        /// </summary>
        private List<Piece> GetStandardArmy(PieceColor color)
        {
            List<Piece> pieces = new List<Piece>();
            int pawnRow = color == PieceColor.White ? 6 : 1;
            int backRow = color == PieceColor.White ? 7 : 0;
            
            // Add pawns and vanguards
            for (int col = 0; col < 8; col++)
            {
                // Replace the middle pawns (columns 3 and 4) with Vanguards
                if (col == 3 || col == 4)
                {
                    pieces.Add(new Vanguard(color, new Position(pawnRow, col)));
                }
                else
                {
                    pieces.Add(new Pawn(color, new Position(pawnRow, col)));
                }
            }
            
            // Add rooks
            pieces.Add(new Rook(color, new Position(backRow, 0)));
            pieces.Add(new Rook(color, new Position(backRow, 7)));
            
            // Add knights
            pieces.Add(new Knight(color, new Position(backRow, 1)));
            pieces.Add(new Knight(color, new Position(backRow, 6)));
            
            // Add bishops
            pieces.Add(new Bishop(color, new Position(backRow, 2)));
            pieces.Add(new Bishop(color, new Position(backRow, 5)));
            
            // Add queen
            pieces.Add(new Queen(color, new Position(backRow, 3)));
            
            // Add king
            pieces.Add(new King(color, new Position(backRow, 4)));
            
            return pieces;
        }
        
        // Future army setups can be added as methods here
        // For example:
        /*
        private List<Piece> GetIronLegionArmy(PieceColor color)
        {
            // Custom piece lineup for Iron Legion
            // ...
        }
        
        private List<Piece> GetCrimsonOrderArmy(PieceColor color)
        {
            // Custom piece lineup for Crimson Order
            // ...
        }
        */
    }
}
